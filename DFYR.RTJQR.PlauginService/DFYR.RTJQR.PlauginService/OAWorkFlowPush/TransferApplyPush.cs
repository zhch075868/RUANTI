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
    [Kingdee.BOS.Util.HotUpdate, Description("推送调拨申请单至OA")]
    public class TransferApplyPush : AbstractOperationServicePlugIn
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
                DynamicObject BillType = o["FBillTypeID"] as DynamicObject;
                string BillTypeName = BillType == null ? "" : Convert.ToString(BillType["Name"]);
                var field = this.BusinessInfo.GetField("FBusinessType") as ComboField; ;
                var enumValue = o["BusinessType"];
                var BusinessType = field.GetEnumItemName(enumValue);
                field = this.BusinessInfo.GetField("FTransferDirect") as ComboField; ;
                enumValue = o["FTransferDirect"];
                var FTransferDirect = field.GetEnumItemName(enumValue);
                string FRemarks = Convert.ToString(o["FRemarks"]);
                DynamicObject APPORGID = o["APPORGID"] as DynamicObject;
                string APPORGIDNumber = APPORGID == null ? "" : Convert.ToString(APPORGID["F_PYEO_Text_OAID"]);
                DynamicObject F_PYEO_Assistant = o["F_PYEO_Assistant"] as DynamicObject;
                string F_PYEO_AssistantName = F_PYEO_Assistant == null ? "" : Convert.ToString(F_PYEO_Assistant["FDataValue"]);
                string FDate = Convert.ToDateTime(o["FDate"]).ToString("yyyy-MM-dd");
                field = this.BusinessInfo.GetField("FOwnerTypeIdHead") as ComboField; ;
                enumValue = o["OwnerTypeIdHead"];
                string OwnerTypeIdHead = field.GetEnumItemName(enumValue);
                field = this.BusinessInfo.GetField("FOwnerTypeInIdHead") as ComboField; ;
                enumValue = o["OwnerTypeInIdHead"];
                string OwnerTypeInIdHead = field.GetEnumItemName(enumValue);
                //新增字段
                DynamicObject F_PYEO_SaleDeptId = o["F_PYEO_SaleDeptId"] as DynamicObject;
                string F_PYEO_SaleDeptIdNumber = F_PYEO_SaleDeptId == null? "": Convert.ToString(F_PYEO_SaleDeptId["F_PYEO_Text_OAID"]);
                DynamicObject F_PYEO_SaleGroupId = o["F_PYEO_SaleGroupId"] as DynamicObject;
                string F_PYEO_SaleGroupIdName = F_PYEO_SaleGroupId == null ? "" : Convert.ToString(F_PYEO_SaleGroupId["Name"]);
                DynamicObject F_PYEO_SalerId = o["F_PYEO_SalerId"] as DynamicObject;
                string F_PYEO_SalerIdName = F_PYEO_SalerId == null ? "" : Convert.ToString(F_PYEO_SalerId["EmpNumber"]);
                F_PYEO_SalerIdName = Utils.getPersonOAid(this.Context, F_PYEO_SalerIdName);
                string F_PYEO_Amount1 = Convert.ToDecimal(o["F_PYEO_Amount1"]).ToString("#0.00");


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
                mainRootItem.Add("fieldName", "ywlx");
                mainRootItem.Add("fieldValue", BusinessType);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "dbfx");
                mainRootItem.Add("fieldValue", FTransferDirect);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "bz");
                mainRootItem.Add("fieldValue", FRemarks);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "sqzz");
                mainRootItem.Add("fieldValue", APPORGIDNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "dblx");
                mainRootItem.Add("fieldValue", F_PYEO_AssistantName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "sqrq");
                mainRootItem.Add("fieldValue", FDate);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "dchzlx");
                mainRootItem.Add("fieldValue", OwnerTypeIdHead);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "drhzlx");
                mainRootItem.Add("fieldValue", OwnerTypeInIdHead);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xsbm");
                mainRootItem.Add("fieldValue", F_PYEO_SaleDeptIdNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xsz");
                mainRootItem.Add("fieldValue", F_PYEO_SaleGroupIdName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xsy");
                mainRootItem.Add("fieldValue", F_PYEO_SalerIdName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "je");
                mainRootItem.Add("fieldValue", F_PYEO_Amount1);
                mainRoot.Add(mainRootItem);

                JSONArray detailData = new JSONArray();

                JSONObject detailDataItem = new JSONObject();
                detailDataItem.Add("tableDBName", "formtable_main_65_dt1");
                JSONArray workflowRequestTableRecords = new JSONArray();

                DynamicObjectCollection materials = o["STK_STKTRANSFERAPPENTRY"] as DynamicObjectCollection;
                foreach (DynamicObject material in materials)
                {
                    DynamicObject MaterialId = material["MATERIALID"] as DynamicObject;
                    string MaterialNumber = MaterialId == null ? "" : Convert.ToString(MaterialId["Number"]);
                    string MaterialName = MaterialId == null ? "" : Convert.ToString(MaterialId["Name"]);
                    string Specification = MaterialId == null ? "" : Convert.ToString(MaterialId["Specification"]);
                    DynamicObject UnitId = material["UnitId"] as DynamicObject;
                    string UnitIdNumber = UnitId == null ? "" : Convert.ToString(UnitId["Name"]);
                    string Qty = Convert.ToDecimal(material["Qty"]).ToString("#0.00");
                    DynamicObject STOCKORGID = material["STOCKORGID"] as DynamicObject;
                    string STOCKORGIDNumber = STOCKORGID == null ? "" : Convert.ToString(STOCKORGID["F_PYEO_Text_OAID"]);
                    DynamicObject StockId = material["StockId"] as DynamicObject;
                    string StockIdName = StockId == null ? "" : Convert.ToString(StockId["Name"]);
                    DynamicObject STOCKORGINID = material["STOCKORGINID"] as DynamicObject;
                    string STOCKORGINIDNumber = STOCKORGINID == null ? "" : Convert.ToString(STOCKORGINID["F_PYEO_Text_OAID"]);
                    DynamicObject StockInId = material["StockInId"] as DynamicObject;
                    string StockInIdName = StockInId == null ? "" : Convert.ToString(StockInId["Name"]);
                    DynamicObject OwnerId = material["OwnerId"] as DynamicObject;
                    string OwnerIdName = OwnerId == null ? "" : Convert.ToString(OwnerId["Name"]);
                    DynamicObject FOwnerInId = material["OwnerInId"] as DynamicObject;
                    string FOwnerInIdName = FOwnerInId == null ? "" : Convert.ToString(FOwnerInId["Name"]);

                    string Note = Convert.ToString(material["Note"]);
                    DynamicObject StockLocId = material["StockLocId"] as DynamicObject;
                    string stockLocName = StockLocId == null?"": getStocklocString(StockLocId);

                    DynamicObject StockLocInId = material["StockLocInId"] as DynamicObject;
                    string StockLocInName = StockLocInId == null ? "" : getStocklocString(StockLocInId);

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
                    workflowRequestTableFieldsItem.Add("fieldName", "dczz");
                    workflowRequestTableFieldsItem.Add("fieldValue", STOCKORGIDNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "dcck");
                    workflowRequestTableFieldsItem.Add("fieldValue", StockIdName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "drzz");
                    workflowRequestTableFieldsItem.Add("fieldValue", STOCKORGINIDNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "drck");
                    workflowRequestTableFieldsItem.Add("fieldValue", StockInIdName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "dchz");
                    workflowRequestTableFieldsItem.Add("fieldValue", OwnerIdName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "drhz");
                    workflowRequestTableFieldsItem.Add("fieldValue", FOwnerInIdName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "bz");
                    workflowRequestTableFieldsItem.Add("fieldValue", Note);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "dccw");
                    workflowRequestTableFieldsItem.Add("fieldValue", stockLocName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "drcw");
                    workflowRequestTableFieldsItem.Add("fieldValue", StockLocInName);
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
                        + "&requestName=调拨申请单已到达"
                        + "&workflowId=28&detailData=" + detailData.ToString(), personId);

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
