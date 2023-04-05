using Kingdee.BOS;
using Kingdee.BOS.App.Data;
using Kingdee.BOS.Core.DynamicForm;
using Kingdee.BOS.Core.DynamicForm.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Core.Metadata.FieldElement;
using Kingdee.BOS.JSON;
using Kingdee.BOS.Orm.DataEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFYR.RTJQR.PlauginService.OAWorkFlowPush
{
    [Kingdee.BOS.Util.HotUpdate, Description("比价单推送")]
    public class PriceListPush : AbstractOperationServicePlugIn
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
            foreach (DynamicObject o in e.DataEntitys)
            {
                string id = Convert.ToString(o["Id"]);
                string documentStatus = Convert.ToString(o["DocumentStatus"]);

                //提交校验
                if (!documentStatus.Equals("B"))
                {
                    return;
                }

                DynamicObject BillTypeID = o["BillTypeID"] as DynamicObject;
                string BillTypeNumber = BillTypeID == null?"":Convert.ToString(BillTypeID["Name"]);
                string billNo = Convert.ToString(o["BillNo"]);
                var field = this.BusinessInfo.GetField("FRequestType") as ComboField; ;
                var enumValue = o["RequestType"];
                var RequestType = field.GetEnumItemName(enumValue);

                DynamicObject ApplicationOrgId = o["ApplicationOrgId"] as DynamicObject;
                string ApplicationOrgNumber = ApplicationOrgId == null ? "" : Convert.ToString(ApplicationOrgId["Name"]);
                DynamicObject ApplicationDeptId = o["ApplicationDeptId"] as DynamicObject;
                string ApplicationDeptNumber = ApplicationDeptId == null ? "" : Convert.ToString(ApplicationDeptId["Name"]);
                DynamicObject ApplicantId = o["ApplicantId"] as DynamicObject;
                string ApplicantNumber = ApplicantId == null ? "" : Convert.ToString(ApplicantId["Name"]);
                DynamicObject CurrencyId = o["CurrencyId"] as DynamicObject;
                string CurrencyNumber = CurrencyId == null ? "" : Convert.ToString(CurrencyId["Name"]);
                string Note = Convert.ToString(o["Note"]);
                string IsConvert = Convert.ToString(o["IsConvert"]).Equals("True") ? "是" : "否";
                string FISPRICEEXCLUDETAX = Convert.ToString(o["FISPRICEEXCLUDETAX"]).Equals("True") ? "是" : "否";
                DynamicObject ExchangeTypeId = o["ExchangeTypeId"] as DynamicObject;
                string ExchangeTypeNumber = ExchangeTypeId == null ? "" : Convert.ToString(ExchangeTypeId["Name"]);
                string SrcType = Convert.ToString(o["SrcType"]);

                field = this.BusinessInfo.GetField("FACCTYPE") as ComboField; ;
                enumValue = o["FACCTYPE"];
                var FACCTYPE = field.GetEnumItemName(enumValue);

                string CloseReason = Convert.ToString(o["CloseReason"]);
                string TotalAmount = Convert.ToDecimal(o["TotalAmount"]).ToString("#0.00");


                JSONArray mainRoot = new JSONArray();
                JSONObject mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "djlx");
                mainRootItem.Add("fieldValue", BillTypeNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "djbh");
                mainRootItem.Add("fieldValue", billNo);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "erpnumber");
                mainRootItem.Add("fieldValue", billNo);
                mainRoot.Add(mainRootItem);

                
                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "bjlx");
                mainRootItem.Add("fieldValue", RequestType);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "bjzz");
                mainRootItem.Add("fieldValue", ApplicationOrgNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "bjbm");
                mainRootItem.Add("fieldValue", ApplicationDeptNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "bjr");
                mainRootItem.Add("fieldValue", ApplicantNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "bb");
                mainRootItem.Add("fieldValue", CurrencyNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "bz");
                mainRootItem.Add("fieldValue", Note);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "sfdjzh");
                mainRootItem.Add("fieldValue", IsConvert);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "jws");
                mainRootItem.Add("fieldValue", FISPRICEEXCLUDETAX);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "hllx");
                mainRootItem.Add("fieldValue", ExchangeTypeNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "lylx");
                mainRootItem.Add("fieldValue", SrcType);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "ysfs");
                mainRootItem.Add("fieldValue", FACCTYPE);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "gbyy");
                mainRootItem.Add("fieldValue", CloseReason);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "hsjehj");
                mainRootItem.Add("fieldValue", TotalAmount);
                mainRoot.Add(mainRootItem);

                JSONArray detailData = new JSONArray();

                JSONObject detailDataItem = new JSONObject();
                detailDataItem.Add("tableDBName", "formtable_main_51_dt1");
                JSONArray workflowRequestTableRecords = new JSONArray();

                DynamicObjectCollection ReqEntrys = o["ReqEntry"] as DynamicObjectCollection;
                foreach (DynamicObject ReqEntry in ReqEntrys)
                {
                    DynamicObject RequireOrgId = ReqEntry["RequireOrgId"] as DynamicObject;
                    string RequireOrgNumber = RequireOrgId == null ? "" : Convert.ToString(RequireOrgId["Name"]);
                    DynamicObject MaterialId = ReqEntry["MaterialId"] as DynamicObject;
                    string MaterialNumber = MaterialId == null ? "" : Convert.ToString(MaterialId["Number"]);
                    string MaterialName = MaterialId == null ? "" : Convert.ToString(MaterialId["Name"]);
                    DynamicObject UnitID = ReqEntry["UnitID"] as DynamicObject;
                    string UnitNumber = UnitID == null ? "" : Convert.ToString(UnitID["Name"]);
                    string ReqQty = Convert.ToDecimal(ReqEntry["ReqQty"]).ToString("#0.00");
                    string ApproveQty = Convert.ToDecimal(ReqEntry["ApproveQty"]).ToString("#0.00");
                    DynamicObject PurchaseOrgId = ReqEntry["PurchaseOrgId"] as DynamicObject;
                    string PurchaseOrgNumebr = PurchaseOrgId == null ? "" : Convert.ToString(PurchaseOrgId["Name"]);
                    DynamicObject SuggestSupplierId = ReqEntry["SuggestSupplierId"] as DynamicObject;
                    string SuggestSupplierNumber = SuggestSupplierId == null ? "" : Convert.ToString(SuggestSupplierId["Name"]);
                    DynamicObject ReceiveOrgId = ReqEntry["ReceiveOrgId"] as DynamicObject;
                    string ReceiveOrgNumber = ReceiveOrgId == null ? "" : Convert.ToString(ReceiveOrgId["Name"]);
                    string EvaluatePrice = Convert.ToDecimal(ReqEntry["EvaluatePrice"]).ToString("#0.00");
                    string TAXPRICE = Convert.ToDecimal(ReqEntry["TAXPRICE"]).ToString("#0.00");
                    string TAXRATE = Convert.ToDecimal(ReqEntry["TAXRATE"]).ToString("#0.00");
                    string PriceUnitQty = Convert.ToDecimal(ReqEntry["PriceUnitQty"]).ToString("#0.00");
                    DynamicObject REQSTOCKUNITID = ReqEntry["REQSTOCKUNITID"] as DynamicObject;
                    string REQSTOCKUNITNUMBER = REQSTOCKUNITID == null ? "" : Convert.ToString(REQSTOCKUNITID["Name"]);
                    string REQSTOCKQTY = Convert.ToDecimal(ReqEntry["REQSTOCKQTY"]).ToString("#0.00");
                    string LeadTime = Convert.ToString(ReqEntry["LeadTime"]);
                    DynamicObject ChargeProjectID = ReqEntry["ChargeProjectID"] as DynamicObject;
                    string ChargeProjectNumber = ChargeProjectID == null ? "" : Convert.ToString(ChargeProjectID["Name"]);
                    DynamicObject PurchaseDeptId = ReqEntry["PurchaseDeptId"] as DynamicObject;
                    string PurchaseDeptNumber = PurchaseDeptId == null ? "" : Convert.ToString(PurchaseDeptId["Name"]);
                    string ReceiveAddress = Convert.ToString(ReqEntry["ReceiveAddress"]);
                    string EntryNote = Convert.ToString(ReqEntry["EntryNote"]);
                    DynamicObject RequireStaffId = ReqEntry["RequireStaffId"] as DynamicObject;
                    string RequireStaffNumber = RequireStaffId == null ? "" : Convert.ToString(RequireStaffId["Number"]);
                    DynamicObject PurchaseGroupId = ReqEntry["PurchaseGroupId"] as DynamicObject;
                    string PurchaseGroupNumber = PurchaseGroupId == null ? "" : Convert.ToString(PurchaseGroupId["PurchaseGroupId"]);
                    DynamicObject purchaserId = ReqEntry["purchaserId"] as DynamicObject;
                    string purchaserName =purchaserId==null?"": Convert.ToString(purchaserId["Name"]);
                    DynamicObject BOMNoId = ReqEntry["BOMNoId"] as DynamicObject;
                    string BOMNoNumber = BOMNoId == null ? "" : Convert.ToString(BOMNoId["Name"]);
                    DynamicObject StockId = ReqEntry["StockId"] as DynamicObject;
                    string StockNumber = StockId == null ? "" : Convert.ToString(StockId["Name"]);
                    DynamicObject ProviderId = ReqEntry["ProviderId"] as DynamicObject;
                    string ProviderNumber = ProviderId == null ? "" : Convert.ToString(ProviderId["Name"]);
                    string MtoNo = Convert.ToString(ReqEntry["MtoNo"]);
                    string BaseReqQty = Convert.ToDecimal(ReqEntry["BaseReqQty"]).ToString("#0.00");
                    DynamicObject ReceiveDeptId = ReqEntry["ReceiveDeptId"] as DynamicObject;
                    string ReceiveDeptNumber = ReceiveDeptId == null ? "" : Convert.ToString(ReceiveDeptId["Name"]);
                    DynamicObject RequireDeptId = ReqEntry["RequireDeptId"] as DynamicObject;
                    string RequireDeptNumber = RequireDeptId == null ? "" : Convert.ToString(RequireDeptId["Name"]);
                    DynamicObject SalUnitID = ReqEntry["SalUnitID"] as DynamicObject;
                    string SalUnitNumber = SalUnitID == null ? "" : Convert.ToString(SalUnitID["Name"]);
                    string SalQty = Convert.ToDecimal(ReqEntry["SalQty"]).ToString("#0.00");
                    string SalBaseQty = Convert.ToDecimal(ReqEntry["SalBaseQty"]).ToString("#0.00");
                    string IsVmiBusiness = Convert.ToString(ReqEntry["IsVmiBusiness"]).Equals("True")?"是":"否";
                    string DEMANDTYPE = Convert.ToString(ReqEntry["DEMANDTYPE"]);
                    string DEMANDBILLNO = Convert.ToString(ReqEntry["DEMANDBILLNO"]);
                    string DEMANDBILLENTRYSEQ = Convert.ToString(ReqEntry["DEMANDBILLENTRYSEQ"]);
                    string DEMANDBILLENTRYID = Convert.ToString(ReqEntry["DEMANDBILLENTRYID"]);
                    string F_SRT_CheckBox = Convert.ToString(ReqEntry["F_SRT_CheckBox"]).Equals("True") ? "是" : "否";
                    string F_SRT_BJYJ = Convert.ToString(ReqEntry["F_SRT_BJYJ"]);
                    string SrcReqSplitEntryId = Convert.ToString(ReqEntry["SrcReqSplitEntryId"]);
                    string AssortBillNo = Convert.ToString(ReqEntry["AssortBillNo"]);


                    JSONObject workflowRequestTableRecordsItem = new JSONObject();
                    workflowRequestTableRecordsItem.Add("recordOrder", "0");
                    JSONArray workflowRequestTableFields = new JSONArray();


                    JSONObject workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xqzz");
                    workflowRequestTableFieldsItem.Add("fieldValue", RequireOrgNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "wlbm");
                    workflowRequestTableFieldsItem.Add("fieldValue", MaterialNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "wlmc");
                    workflowRequestTableFieldsItem.Add("fieldValue", MaterialName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "bjdw");
                    workflowRequestTableFieldsItem.Add("fieldValue", UnitNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "bjqrsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", ReqQty);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "pzsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", ApproveQty);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "cgzz");
                    workflowRequestTableFieldsItem.Add("fieldValue", ApplicationOrgNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "bjgys");
                    workflowRequestTableFieldsItem.Add("fieldValue", SuggestSupplierNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "slzz");
                    workflowRequestTableFieldsItem.Add("fieldValue", ApplicationOrgNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "bjqrdj");
                    workflowRequestTableFieldsItem.Add("fieldValue", EvaluatePrice);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "bjqrhsdj");
                    workflowRequestTableFieldsItem.Add("fieldValue", TAXPRICE);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "sl");
                    workflowRequestTableFieldsItem.Add("fieldValue", TAXRATE);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "bjqrjjsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", PriceUnitQty);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "kcdw");
                    workflowRequestTableFieldsItem.Add("fieldValue", REQSTOCKUNITNUMBER);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "kcdwsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", REQSTOCKQTY);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "tqq");
                    workflowRequestTableFieldsItem.Add("fieldValue", LeadTime);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "fyxm");
                    workflowRequestTableFieldsItem.Add("fieldValue", ChargeProjectNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "cgbm");
                    workflowRequestTableFieldsItem.Add("fieldValue", ApplicationDeptNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jhdz");
                    workflowRequestTableFieldsItem.Add("fieldValue", ReceiveAddress);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "bz");
                    workflowRequestTableFieldsItem.Add("fieldValue", EntryNote);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "cgy");
                    workflowRequestTableFieldsItem.Add("fieldValue", purchaserName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "cgz");
                    workflowRequestTableFieldsItem.Add("fieldValue", PurchaseGroupNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "bombb");
                    workflowRequestTableFieldsItem.Add("fieldValue", BOMNoNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "ck");
                    workflowRequestTableFieldsItem.Add("fieldValue", StockNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "ghdd");
                    workflowRequestTableFieldsItem.Add("fieldValue", ProviderNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jhgzh");
                    workflowRequestTableFieldsItem.Add("fieldValue", MtoNo);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "sqsljbdw");
                    workflowRequestTableFieldsItem.Add("fieldValue", BaseReqQty);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "slbm");
                    workflowRequestTableFieldsItem.Add("fieldValue", ReceiveDeptNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xqbm");
                    workflowRequestTableFieldsItem.Add("fieldValue", ReceiveDeptNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xsdw");
                    workflowRequestTableFieldsItem.Add("fieldValue", SalUnitNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xssl");
                    workflowRequestTableFieldsItem.Add("fieldValue", SalQty);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xsjbsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", SalBaseQty);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "vmiyw");
                    workflowRequestTableFieldsItem.Add("fieldValue", IsVmiBusiness);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xqly");
                    workflowRequestTableFieldsItem.Add("fieldValue", DEMANDTYPE);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xqdjbh");
                    workflowRequestTableFieldsItem.Add("fieldValue", DEMANDBILLNO);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xqdjxh");
                    workflowRequestTableFieldsItem.Add("fieldValue", DEMANDBILLENTRYSEQ);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xqdjflnm");
                    workflowRequestTableFieldsItem.Add("fieldValue", DEMANDBILLENTRYID);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "sfybj");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_SRT_CheckBox);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "bjqryj");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_SRT_BJYJ);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "sqdhbqflnm");
                    workflowRequestTableFieldsItem.Add("fieldValue", SrcReqSplitEntryId);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "ptdjbh");
                    workflowRequestTableFieldsItem.Add("fieldValue", AssortBillNo);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xqr");
                    workflowRequestTableFieldsItem.Add("fieldValue", "59");//RequireStaffNumber);
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
                    + "&requestName=采购比价单已到达"
                    + "&workflowId=10&detailData=" + detailData.ToString(), personId);

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
