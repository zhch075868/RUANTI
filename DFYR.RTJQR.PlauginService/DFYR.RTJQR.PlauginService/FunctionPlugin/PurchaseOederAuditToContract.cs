using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kingdee.BOS;
using Kingdee.BOS.Util;
using Kingdee.BOS.Orm;
using Kingdee.BOS.Core;
using Kingdee.BOS.Core.Metadata;
using Kingdee.BOS.Core.Metadata.FormElement;
using Kingdee.BOS.Core.DynamicForm;
using Kingdee.BOS.Core.DynamicForm.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Core.Bill;
using Kingdee.BOS.Core.Interaction;
using Kingdee.BOS.ServiceHelper;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.BOS.App.Data;
using System.ComponentModel;

namespace DFYR.RTJQR.PlauginService.FunctionPlugin
{
    public class PurchaseOederAuditToContract : AbstractOperationServicePlugIn
    {
         public override void OnPreparePropertys(PreparePropertysEventArgs e)
        {
            //e.FieldKeys.Add("");
            e.FieldKeys.Add("F_SRT_CONTRACTNAME");
            e.FieldKeys.Add("PurchaseOrgId");
            e.FieldKeys.Add("F_SRT_LSHTH2");
            e.FieldKeys.Add("F_SRT_CheckBox");
            e.FieldKeys.Add("F_SRT_CONTRACTYPE");
        }

        /// <summary>
        /// 操作开始前功能处理
        /// </summary>
        /// <param name="e"></param>
        public override void EndOperationTransaction(EndOperationTransactionArgs e)
        {
            foreach (DynamicObject o in e.DataEntitys)
            {
                DynamicObject contractId = o["F_SRT_LSHTH2"] as DynamicObject;
                if (null != contractId)
                {
                    return;
                }

                string htId = ImportMaterial(o);
                string F_SRT_CheckBox = Convert.ToString(o["F_SRT_CheckBox"]);
                if (F_SRT_CheckBox.Equals("True"))
                {
                    return;
                }

                //反写合同号
                string BillId = Convert.ToString(o["Id"]);
                string sql = string.Format("update t_PUR_POOrder set F_SRT_LSHTH2 = {0} where fid = {1}", htId, BillId); 
                DBUtils.Execute(this.Context, sql);


                //FormMetadata meta = MetaDataServiceHelper.Load(this.Context, "k10630d88da9b45eeb22599a052c66e4f") as FormMetadata;
                //DynamicObject ht = BusinessDataServiceHelper.LoadSingle(
                //                        this.Context,
                //                        htId,
                //                        meta.BusinessInfo.GetDynamicObjectType());
                ////反写合同号
                //string BillId = Convert.ToString(o["Id"]);

                //DynamicObject dd = BusinessDataServiceHelper.LoadSingle(
                //                        this.Context,
                //                        BillId,
                //                        this.BusinessInfo.GetDynamicObjectType());

                //dd["F_SRT_LSHTH2_Id"] = htId;
                //dd["F_SRT_LSHTH2"] = ht;
                //ISaveService saveService = ServiceFactory.GetSaveService(base.Context);
                //IOperationResult result = saveService.Save(this.Context, this.BusinessInfo, new DynamicObject[] { dd });
            }
        }

        private string ImportMaterial(DynamicObject o)
        {
            // 构建一个IBillView实例，通过此实例，可以方便的填写物料各属性     
            IBillView billView = this.CreateMaterialView();
            // 新建一个空白物料         
            //billView.CreateNewModelData();     
            ((IBillViewService)billView).LoadData();
            // 触发插件的OnLoad事件：          
            // 组织控制基类插件，在OnLoad事件中，对主业务组织改变是否提示选项进行初始化。      
            // 如果不触发OnLoad事件，会导致主业务组织赋值不成功      
            DynamicFormViewPlugInProxy eventProxy = billView.GetService<DynamicFormViewPlugInProxy>();
            eventProxy.FireOnLoad();

            this.FillMaterialPropertys(billView,o);
            // 保存物料          
            OperateOption saveOption = OperateOption.Create();
            IOperationResult result = this.SaveMaterial(billView, saveOption);
            
            return result.OperateResult[0].PKValue.ToString(); ;
        }


        private void FillMaterialPropertys(IBillView billView,DynamicObject o)
        {
            string billNo = Convert.ToString(o["BillNo"]);
            string contractName = Convert.ToString(o["F_SRT_CONTRACTNAME"]);
            DynamicObject PurchaseOrgId = o["PurchaseOrgId"] as DynamicObject;
            string PurchaseOrgNumber = Convert.ToString(PurchaseOrgId["Number"]);
            string ContractType = Convert.ToString(o["F_SRT_CONTRACTYPE"]);

            IDynamicFormViewService dynamicFormView = billView as IDynamicFormViewService;
            /********************物料页签上的字段******************/
            dynamicFormView.UpdateValue("FNumber", 0, billNo);
            dynamicFormView.UpdateValue("FName",0, contractName);
            dynamicFormView.SetItemValueByNumber("FCreateOrgId", PurchaseOrgNumber, 0);
            dynamicFormView.SetItemValueByNumber("FCreateOrgId", PurchaseOrgNumber, 0);
            dynamicFormView.UpdateValue("F_PYEO__SRT_CONTRACTYPE", 0, ContractType);

        }

        private IOperationResult SaveMaterial(IBillView billView, OperateOption saveOption)
        {
            // 设置FormId 
            Form form = billView.BillBusinessInfo.GetForm();
            if (form.FormIdDynamicProperty != null)
            {
                form.FormIdDynamicProperty.SetValue(billView.Model.DataObject, form.Id);
            }
            // 调用保存操作  
            IOperationResult saveResult = BusinessDataServiceHelper.Save(
                this.Context,
                billView.BillBusinessInfo,
                billView.Model.DataObject,
                saveOption,
                "Save");
            if (!saveResult.IsSuccess)
            {
                if (saveResult.OperateResult.Count != 0)
                {
                    throw new KDException("", "生成合同失败：" + saveResult.OperateResult[0].Message);
                }
                else
                {
                    throw new KDException("", "生成合同失败：" + saveResult.ValidationErrors[0].Message.ToString());
                }
            }
            object pk = saveResult.OperateResult[0].PKValue;
            BusinessDataServiceHelper.Submit(
                this.Context,
                billView.BillBusinessInfo, new object[] { pk },"Submit");
            OperateOption auditOption = OperateOption.Create();
            BusinessDataServiceHelper.Audit(
                 this.Context,
                 billView.BillBusinessInfo, new object[] { pk }, auditOption);

            return saveResult;
        }

        private IBillView CreateMaterialView()
        {
            // 读取物料的元数据  
            FormMetadata meta = MetaDataServiceHelper.Load(this.Context, "k10630d88da9b45eeb22599a052c66e4f") as FormMetadata;
            Form form = meta.BusinessInfo.GetForm();
            // 创建用于引入数据的单据view          
            Type type = Type.GetType("Kingdee.BOS.Web.Import.ImportBillView,Kingdee.BOS.Web");
            var billView = (IDynamicFormViewService)Activator.CreateInstance(type);
            // 开始初始化billView：  
            // 创建视图加载参数对象，指定各种参数，如FormId, 视图(LayoutId)等   
            BillOpenParameter openParam = CreateOpenParameter(meta);
            // 动态领域模型服务提供类，通过此类，构建MVC实例       
            var provider = form.GetFormServiceProvider();
            billView.Initialize(openParam, provider);
            return billView as IBillView;
        }


        private BillOpenParameter CreateOpenParameter(FormMetadata meta)
        {
            Form form = meta.BusinessInfo.GetForm();
            // 指定FormId, LayoutId          
            BillOpenParameter openParam = new BillOpenParameter(form.Id, meta.GetLayoutInfo().Id);
            // 数据库上下文        
            openParam.Context = this.Context;
            // 本单据模型使用的MVC框架          
            openParam.ServiceName = form.FormServiceName;
            // 随机产生一个不重复的PageId，作为视图的标识 
            openParam.PageId = Guid.NewGuid().ToString();
            // 元数据      
            openParam.FormMetaData = meta;
            // 界面状态：新增 (修改、查看)      
            openParam.Status = OperationStatus.ADDNEW;
            // 单据主键：本案例演示新建物料，不需要设置主键  
            openParam.PkValue = null;
            // 界面创建目的：普通无特殊目的 （为工作流、为下推、为复制等） 
            openParam.CreateFrom = CreateFrom.Default;
            // 基础资料分组维度：基础资料允许添加多个分组字段，每个分组字段会有一个分组维度   
            // 具体分组维度Id，请参阅 form.FormGroups 属性         
            openParam.GroupId = "";
            // 基础资料分组：如果需要为新建的基础资料指定所在分组，请设置此属性    
            openParam.ParentId = 0;
            // 单据类型     
            openParam.DefaultBillTypeId = "";
            // 业务流程           
            openParam.DefaultBusinessFlowId = "";
            // 主业务组织改变时，不用弹出提示界面    
            openParam.SetCustomParameter("ShowConfirmDialogWhenChangeOrg", false);
            // 插件         
            List<AbstractDynamicFormPlugIn> plugs = form.CreateFormPlugIns();
            openParam.SetCustomParameter(FormConst.PlugIns, plugs);
            PreOpenFormEventArgs args = new PreOpenFormEventArgs(this.Context, openParam);
            foreach (var plug in plugs)
            {// 触发插件PreOpenForm事件，供插件确认是否允许打开界面 
                plug.PreOpenForm(args);
            }
            if (args.Cancel == true)
            {// 插件不允许打开界面             
                // 本案例不理会插件的诉求，继续....       
            }
            // 返回     
            return openParam;
        }

        private void ModifyBill(IBillView billView, string pkValue)
        {
            billView.OpenParameter.Status = OperationStatus.EDIT;
            billView.OpenParameter.CreateFrom = CreateFrom.Default;
            billView.OpenParameter.PkValue = pkValue;
            billView.OpenParameter.DefaultBillTypeId = string.Empty;
            ((IDynamicFormViewService)billView).LoadData();
        }
    }
}
