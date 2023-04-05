using Kingdee.BOS;
using Kingdee.BOS.App.Data;
using Kingdee.BOS.Core.DynamicForm;
using Kingdee.BOS.Core.DynamicForm.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Core.Metadata;
using Kingdee.BOS.Core.Metadata.FieldElement;
using Kingdee.BOS.JSON;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.BOS.Orm.Metadata.DataEntity;
using Kingdee.BOS.ServiceHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFYR.RTJQR.PlauginService.OAWorkFlowPush
{
    [Kingdee.BOS.Util.HotUpdate, Description("出库申请单推送至OA")]
    public class OutStockApplyPush: AbstractOperationServicePlugIn
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
                DynamicObject BillType = o["BillTypeID"] as DynamicObject;
                string BillTypeName = BillType == null ? "" : Convert.ToString(BillType["Name"]);
                DynamicObject StockOrgId = o["StockOrgId"] as DynamicObject;
                string StockOrgIdName = StockOrgId == null ? "" : Convert.ToString(StockOrgId["F_PYEO_Text_OAID"]);
                DynamicObject ApplyType = o["ApplyType"] as DynamicObject;
                string ApplyTypeName = ApplyType == null ? "" : Convert.ToString(ApplyType["FDataValue"]);
                string Date = Convert.ToDateTime(o["Date"]).ToString("yyyy-MM-dd");
                DynamicObject DeptId = o["DeptId"] as DynamicObject;
                string DeptIdName = DeptId == null ? "" : Convert.ToString(DeptId["F_PYEO_Text_OAID"]);
                var field = this.BusinessInfo.GetField("FOwnerTypeIdHead") as ComboField; ;
                var enumValue = o["FOwnerTypeIdHead"];
                var FOwnerTypeIdHead = field.GetEnumItemName(enumValue);
                string DeptIdProperty  = string.Empty;
                if(DeptId != null)
                {
                    DynamicObject dpetPropertyId = DeptId["DeptProperty"] as DynamicObject;
                    DeptIdProperty = dpetPropertyId == null ? "" : Convert.ToString(dpetPropertyId["Name"]);
                }
                 
                field = this.BusinessInfo.GetField("FBizType") as ComboField; ;
                enumValue = o["BizType"];
                var BizType = field.GetEnumItemName(enumValue);
                DynamicObject F_PYEO__SRT_XSZXM = o["F_PYEO__SRT_XSZXM"] as DynamicObject;
                string F_PYEO__SRT_XSZXMName = F_PYEO__SRT_XSZXM == null ? "" : Convert.ToString(F_PYEO__SRT_XSZXM["Name"]);
                DynamicObject F_PYEO_Base = o["F_PYEO_Base"] as DynamicObject;
                string F_PYEO_BaseName = F_PYEO_Base == null ? "" : Convert.ToString(F_PYEO_Base["Number"]);
                F_PYEO_BaseName = Utils.getPersonOAid(this.Context, F_PYEO_BaseName);
                DynamicObject F_PYEO_Project = o["F_PYEO_Project"] as DynamicObject;
                string F_PYEO_ProjectName = F_PYEO_Project == null ? "" : Convert.ToString(F_PYEO_Project["Name"]);
                string F_PYEO_Amount1 = Convert.ToDecimal(o["F_PYEO_Amount1"]).ToString("#0.00");
                string F_PYEO_ContractNo1 = Convert.ToString(o["F_PYEO_ContractNo1"]);
                DynamicObject F_PYEO_ContractNo2 = o["F_PYEO_ContractNo2"] as DynamicObject;
                string F_PYEO_ContractNo2Name = F_PYEO_ContractNo2 == null ? "" : Convert.ToString(F_PYEO_ContractNo2["Number"]);
                string Note = Convert.ToString(o["Note"]);
                DynamicObject custId = o["CustId"] as DynamicObject;
                string custIdName = custId == null?"": Convert.ToString(custId["Name"]);

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
                mainRootItem.Add("fieldName", "djlx");
                mainRootItem.Add("fieldValue", BillTypeName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "sqzz");
                mainRootItem.Add("fieldValue", StockOrgIdName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "sqlx");
                mainRootItem.Add("fieldValue", ApplyTypeName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "sqrq");
                mainRootItem.Add("fieldValue", Date);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "lybm");
                mainRootItem.Add("fieldValue", DeptIdName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "hzlx");
                mainRootItem.Add("fieldValue", FOwnerTypeIdHead);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "lyzx");
                mainRootItem.Add("fieldValue", DeptIdProperty);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "ywlx");
                mainRootItem.Add("fieldValue", BizType);
                mainRoot.Add(mainRootItem);

                //mainRootItem = new JSONObject();
                //mainRootItem.Add("fieldName", "xszxm");
                //mainRootItem.Add("fieldValue", F_PYEO__SRT_XSZXMName);
                //mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "lyr");
                mainRootItem.Add("fieldValue", F_PYEO_BaseName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xm");
                mainRootItem.Add("fieldValue", F_PYEO_ProjectName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "je");
                mainRootItem.Add("fieldValue", F_PYEO_Amount1);
                mainRoot.Add(mainRootItem);

                //mainRootItem = new JSONObject();
                //mainRootItem.Add("fieldName", "hth1");
                //mainRootItem.Add("fieldValue", F_PYEO_ContractNo1);
                //mainRoot.Add(mainRootItem);

                //mainRootItem = new JSONObject();
                //mainRootItem.Add("fieldName", "hth2");
                //mainRootItem.Add("fieldValue", F_PYEO_ContractNo2Name);
                //mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "kh");
                mainRootItem.Add("fieldValue", custIdName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "bz");
                mainRootItem.Add("fieldValue", Note);
                mainRoot.Add(mainRootItem);

                JSONArray detailData = new JSONArray();

                JSONObject detailDataItem = new JSONObject();
                detailDataItem.Add("tableDBName", "formtable_main_74_dt1");
                JSONArray workflowRequestTableRecords = new JSONArray();

                DynamicObjectCollection BillEntrys = o["BillEntry"] as DynamicObjectCollection;
                foreach (DynamicObject BillEntry in BillEntrys)
                {
                    DynamicObject MaterialId = BillEntry["MaterialId"] as DynamicObject;
                    string MaterialNumber = MaterialId == null ? "" : Convert.ToString(MaterialId["Number"]);
                    string MaterialName = MaterialId == null ? "" : Convert.ToString(MaterialId["Name"]);
                    string Specification = MaterialId == null ? "" : Convert.ToString(MaterialId["Specification"]);
                    DynamicObject UnitId = BillEntry["UnitId"] as DynamicObject;
                    string UnitIdNumber = UnitId == null ? "" : Convert.ToString(UnitId["Name"]);
                    string Qty = Convert.ToDecimal(BillEntry["Qty"]).ToString("#0.00");
                    DynamicObject StockOrgIdEntry = BillEntry["StockOrgIdEntry"] as DynamicObject;
                    string StockOrgIdEntryNumber = StockOrgIdEntry == null ? "" : Convert.ToString(StockOrgIdEntry["F_PYEO_Text_OAID"]);
                    DynamicObject StockId = BillEntry["StockId"] as DynamicObject;
                    string StockIdNumber = StockId == null ? "" : Convert.ToString(StockId["Name"]);
                    DynamicObject stockLocId = BillEntry["StockLocId"] as DynamicObject;
                    string stockLocIdName = getStocklocString(stockLocId);
                    DynamicObject OwnerId = BillEntry["OwnerId"] as DynamicObject;
                    string OwnerIdNumber = OwnerId == null ? "" : Convert.ToString(OwnerId["Name"]);
                    string EntryNote = Convert.ToString(BillEntry["EntryNote"]);
                    string F_PYEO_Price = Convert.ToDecimal(BillEntry["F_PYEO_Price"]).ToString("#0.00");
                    string F_PYEO_Amount = Convert.ToDecimal(BillEntry["F_PYEO_Amount"]).ToString("#0.00");

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
                    workflowRequestTableFieldsItem.Add("fieldValue", UnitIdNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "sqsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", Qty);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "kczz");
                    workflowRequestTableFieldsItem.Add("fieldValue", StockOrgIdEntryNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "ck");
                    workflowRequestTableFieldsItem.Add("fieldValue", StockIdNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);
                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "cw");
                    workflowRequestTableFieldsItem.Add("fieldValue", stockLocIdName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "hz");
                    workflowRequestTableFieldsItem.Add("fieldValue", OwnerIdNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "bz");
                    workflowRequestTableFieldsItem.Add("fieldValue", EntryNote);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xsckdj");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_PYEO_Price);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "je");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_PYEO_Amount);
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
                        + "&requestName=出库申请单已到达"
                        + "&workflowId=30&detailData=" + detailData.ToString(), personId);

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

        /// <summary>
        /// 获取仓位信息
        /// </summary>
        /// <param name="stockLocId"></param>
        /// <returns></returns>
        private string getStocklocString(DynamicObject stockLocId)
        {
            if (null == stockLocId)
            {
                return "";
            }
            StringBuilder stb = new StringBuilder();
            DynamicPropertyCollection keys = stockLocId.DynamicObjectType.Properties;
            string id, stringId, key = "";
            //手动使用for循环判断对辅助属性象中是否有该key值，并且判断是否有值
            for (int x = 1; x <= keys.Count; x++)
            {
                id = string.Format(x > 9 ? "F1000{0}_Id" : "F10000{0}_Id", x);//对象类型的id
                stringId = string.Format(x > 9 ? "F1000{0}" : "F10000{0}", x);//手动输入类型的ID
                //key = string.Format(x > 9 ? "FF1000{0}" : "FF10000{0}", x);//赋值用的key
                //原有的对象类型的复制
                if (stockLocId.DynamicObjectType.Properties.Contains(id))//判断是否含有该key,如果有就说明是对象型的
                {
                    DynamicObject itemId = stockLocId[stringId] as DynamicObject;
                    if (null != itemId)
                    {
                        string itemName = Convert.ToString(itemId["Name"]);
                        stb.Append(itemName + "|");
                    }
                }
                else
                {
                    //原有的手动输入类型的复制
                    if (stockLocId.DynamicObjectType.Properties.Contains(stringId))//如果上述没有，在判断是否是手动输入类型的
                    {
                        string itemName = Convert.ToString(stockLocId[stringId]);
                        stb.Append(itemName + "|");
                    }
                }
            }
            return stb.ToString();

        }
    }
}
