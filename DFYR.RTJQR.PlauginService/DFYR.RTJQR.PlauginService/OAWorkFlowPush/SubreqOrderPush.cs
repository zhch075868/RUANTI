using Kingdee.BOS;
using Kingdee.BOS.App.Data;
using Kingdee.BOS.Core.DynamicForm;
using Kingdee.BOS.Core.DynamicForm.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Core.Metadata;
using Kingdee.BOS.Core.Metadata.FieldElement;
using Kingdee.BOS.JSON;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.BOS.ServiceHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DFYR.RTJQR.PlauginService.OAWorkFlowPush
{
    [Kingdee.BOS.Util.HotUpdate, Description("委外订单推送至OA")]
    public class SubreqOrderPush : AbstractOperationServicePlugIn
    {
        public override void OnPreparePropertys(PreparePropertysEventArgs e)
        {
            //e.FieldKeys.Add("");
        }

        public override void OnPrepareOperationServiceOption(OnPrepareOperationServiceEventArgs e)
        {
            e.AllowSetOperationResult = false;
        }

        /// <summary>
        /// 操作开始前功能处理
        /// </summary>
        /// <param name="e"></param>
        public override void EndOperationTransaction(EndOperationTransactionArgs e)
        {
            Utils.token = "";
            foreach (DynamicObject item in e.DataEntitys)
            {
                DynamicObject o = BusinessDataServiceHelper.LoadSingle(
                               this.Context,
                               item["Id"].ToString(),
                               this.BusinessInfo.GetDynamicObjectType());

                string id = Convert.ToString(o["Id"]);
                string documentStatus = Convert.ToString(o["DocumentStatus"]);

                //提交校验
                if (!documentStatus.Equals("B"))
                {
                    return;
                }

                string BillNo = Convert.ToString(o["BillNo"]);
                DynamicObject SubOrgId = o["SubOrgId"] as DynamicObject;
                string SubOrgIdName = SubOrgId == null ? "" : Convert.ToString(SubOrgId["F_PYEO_Text_OAID"]);
                string IsRework = Convert.ToString(o["IsRework"]).Equals("True") ? "是" : "否";
                DynamicObject F_SRT_CustId = o["F_PYEO__SRT_CustId"] as DynamicObject;
                string F_SRT_CustIdName = F_SRT_CustId == null ? "" : Convert.ToString(F_SRT_CustId["Name"]);
                DynamicObject F_SRT_Project = o["F_PYEO__SRT_Project"] as DynamicObject;
                string F_SRT_ProjectName = F_SRT_Project == null ? "" : Convert.ToString(F_SRT_Project["Name"]);
                DynamicObject F_SRT_SCZXM = o["F_PYEO__SRT_XSZXM"] as DynamicObject;
                string F_SRT_SCZXMName = F_SRT_SCZXM == null ? "" : Convert.ToString(F_SRT_SCZXM["Name"]);

                JSONArray mainRoot = new JSONArray();
                JSONObject mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "djbh");
                mainRootItem.Add("fieldValue", BillNo);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "erpnumber");
                mainRootItem.Add("fieldValue", BillNo);
                mainRoot.Add(mainRootItem);
                

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "wwzz");
                mainRootItem.Add("fieldValue", SubOrgIdName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "sffg");
                mainRootItem.Add("fieldValue", IsRework);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "kh");
                mainRootItem.Add("fieldValue", F_SRT_CustIdName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xm");
                mainRootItem.Add("fieldValue", F_SRT_ProjectName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xszxm");
                mainRootItem.Add("fieldValue", F_SRT_SCZXMName);
                mainRoot.Add(mainRootItem);

                JSONArray detailData = new JSONArray();

                JSONObject detailDataItem = new JSONObject();
                detailDataItem.Add("tableDBName", "formtable_main_86_dt1");//formtable_main_62_dt1
                JSONArray workflowRequestTableRecords = new JSONArray();

                DynamicObjectCollection TreeEntity = o["TreeEntity"] as DynamicObjectCollection;
                foreach (DynamicObject entry in TreeEntity)
                {
                    DynamicObject MaterialId = entry["MaterialId"] as DynamicObject;
                    string MaterialNumber = MaterialId == null ? "" : Convert.ToString(MaterialId["Number"]);
                    string MaterialName = MaterialId == null ? "" : Convert.ToString(MaterialId["Name"]);
                    string Specification = MaterialId == null ? "" : Convert.ToString(MaterialId["Specification"]);
                    DynamicObject UnitId = entry["UnitId"] as DynamicObject;
                    string UnitIdName = MaterialId == null ? "" : Convert.ToString(UnitId["Name"]);
                    string YieldQty = Convert.ToDecimal(entry["YieldQty"]).ToString("#0.00");
                    string PlanStartDate = Convert.ToDateTime(entry["PlanStartDate"]).ToString("yyyy-MM-dd");
                    string PlanFinishDateD = Convert.ToDateTime(entry["PlanFinishDate"]).ToString("yyyy-MM-dd");

                    JSONObject workflowRequestTableRecordsItem = new JSONObject();
                    workflowRequestTableRecordsItem.Add("recordOrder", "0");
                    JSONArray workflowRequestTableFields = new JSONArray();


                    JSONObject workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "wlbm");
                    workflowRequestTableFieldsItem.Add("fieldValue", MaterialNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "wlmc");
                    workflowRequestTableFieldsItem.Add("fieldValue", MaterialName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "ggxh");
                    workflowRequestTableFieldsItem.Add("fieldValue", Specification);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "dw");
                    workflowRequestTableFieldsItem.Add("fieldValue", UnitIdName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "cpsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", YieldQty);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jhkgsj");
                    workflowRequestTableFieldsItem.Add("fieldValue", PlanStartDate);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jhwgsj");
                    workflowRequestTableFieldsItem.Add("fieldValue", PlanFinishDateD);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableRecordsItem.Add("workflowRequestTableFields", workflowRequestTableFields);
                    workflowRequestTableRecords.Add(workflowRequestTableRecordsItem);
                    
                }
                detailDataItem.Add("workflowRequestTableRecords", workflowRequestTableRecords);
                detailData.Add(detailDataItem);

                DynamicObject userObject = Utils.GetUser(this.Context, Convert.ToString(this.Context.UserId));//当前用户信息
                if (userObject == null)
                {
                    throw new KDException("", "当前用户未绑定员工，无法推送OA");
                }
                string personId = Convert.ToString((userObject["FLinkObject"] as DynamicObject)["Number"]);
                personId = Utils.getPersonOAid(this.Context, personId);
                string resultStr = Utils.wkPostUrl(Utils.pushAddWF,
                    "mainData=" + mainRoot.ToString()
                    + "&requestName=委外订单已到达"
                    + "&workflowId=38&detailData=" + detailData.ToString(), personId);

                JSONObject resultJson = JSONObject.Parse(resultStr);
                string code = Convert.ToString(resultJson["code"]);

                if (code.Equals("SUCCESS"))
                {
                    this.OperationResult.OperateResult.Insert(0, new OperateResult()//返回的错误消息
                    {
                        PKValue = id,
                        MessageType = MessageType.Normal,
                        Message = "提交OA流程成功",
                        Name = "提交OA流程返回",
                        SuccessStatus = true,
                    });
                }
                else
                {
                    string errMsg = Convert.ToString(resultJson["errMsg"]);
                    //提示出谁谁谁没收到消息，请重新发送
                    this.OperationResult.OperateResult.Insert(0, new OperateResult()//返回的错误消息
                    {
                        PKValue = id,
                        MessageType = MessageType.FatalError,
                        Message = "提交流程失败：" + errMsg,
                        Name = "提交OA流程返回",
                        SuccessStatus = false,
                    });

                }
            }
        }
    }
}
