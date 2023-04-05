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
    [Kingdee.BOS.Util.HotUpdate, Description("采购订单变更单推送")]
    public class POXChangePush : AbstractOperationServicePlugIn
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
                string billNo = Convert.ToString(o["BillNo"]);
                string documentStatus = Convert.ToString(o["DocumentStatus"]);

                //提交校验
                if (!documentStatus.Equals("B"))
                {
                    return;
                }

                string billTypeIdX = Convert.ToString(o["BillTypeIdX"]);
                billTypeIdX = this.getOABillType(billTypeIdX);
                string businessType = Convert.ToString(o["BusinessType"]);
                businessType = this.getBizType(businessType);
                var field = this.BusinessInfo.GetField("F_SRT_CONTRACTYPE") as ComboField; ;
                var contractType = o["F_SRT_CONTRACTYPE"];
                string contractTypeName = field.GetEnumItemName(contractType);

                string date = Convert.ToDateTime(o["Date"]).ToString("yyyy-MM-dd");
                DynamicObject supplierId = o["SupplierId"] as DynamicObject;
                string supplierNumber = supplierId == null?"":Convert.ToString(supplierId["Number"]);
                DynamicObject purchaseOrgId = o["PurchaseOrgId"] as DynamicObject;
                string purchaseOrgNumber = purchaseOrgId == null ? "" : Convert.ToString(purchaseOrgId["F_PYEO_Text_OAID"]);
                DynamicObject purchaseDeptId = o["PurchaseDeptId"] as DynamicObject;
                string purchaseDeptNumber = purchaseDeptId == null ? "" : Convert.ToString(purchaseDeptId["F_PYEO_Text_OAID"]);
                DynamicObject purchaserGroupId = o["PurchaserGroupId"] as DynamicObject;
                string purchaserGroupNumber = purchaserGroupId==null?"":Convert.ToString(purchaserGroupId["Number"]);

                string settleCurrNumber = string.Empty;
                string PayConditionName = string.Empty;
                string acctype = Convert.ToString(o["FACCTYPE"]);
                acctype = this.getAcceptance(acctype);
                DynamicObject providerContactId = o["ProviderContactId"] as DynamicObject;
                string ProviderContactNumber = providerContactId == null ? "" : Convert.ToString(providerContactId["ContactNumber"]);
                string SettleModeName = string.Empty;
                string ExchangeTypeName = string.Empty;
                string ExchangeRate = string.Empty;
                string PriceListtName = string.Empty;
                string IsIncludedTax = string.Empty;
                string FISPRICEEXCLUDETAX = string.Empty;
                string BillTaxAmount = string.Empty;
                string BillAmount = string.Empty;
                string BillAllAmount = string.Empty;
                string LocalCurrNumebr = string.Empty;
                string F_SRT_ContractName = Convert.ToString(o["F_SRT_ContractName"]);
                string F_PYEO__SRT_Reameks_01 = Convert.ToString(o["F_PYEO__SRT_Reameks_01"]);
                string F_PYEO_Remarks_01 = Convert.ToString(o["F_PYEO_Remarks_01"]);
                string F_PYEO__SRT_REAMEKS_02 = Convert.ToString(o["F_PYEO__SRT_REAMEKS_02"]);
                string F_PYEO_Remarks_02 = Convert.ToString(o["F_PYEO_Remarks_02"]);
                string F_SRT_ChrckMode = Convert.ToString(o["F_SRT_ChrckMode"]);
                DynamicObject ProviderId = o["ProviderId"] as DynamicObject;
                string ProviderNumber =ProviderId==null?"": Convert.ToString(ProviderId["Number"]);
                string FPost = providerContactId==null?"":Convert.ToString(providerContactId["providerContactId"]);
                string FMobile = providerContactId==null?"":Convert.ToString(providerContactId["FMobile"]);
                string ProviderAddress = Convert.ToString(o["ProviderAddress"]);
                DynamicObject SettleId = o["SettleId"] as DynamicObject;
                string SettleNumber = SettleId==null?"":Convert.ToString(SettleId["Number"]);
                DynamicObject ChargeId = o["ChargeId"] as DynamicObject;
                string ChargeNumber =ChargeId == null?"": Convert.ToString(ChargeId["Number"]);
                string ProviderEMail = Convert.ToString(o["ProviderEMail"]);
                DynamicObject PurchaserId = o["PurchaserId"] as DynamicObject;
                string PurchaserNumber =PurchaserId==null?"": Convert.ToString(PurchaserId["Number"]);
                string F_PYEO__SRT_REAMEKS_03 = Convert.ToString(o["F_PYEO__SRT_REAMEKS_03"]);
                string F_PYEO_REMARKS_03 = Convert.ToString(o["F_PYEO_REMARKS_03"]);
                string F_PYEO__SRT_REAMEKS_04 = Convert.ToString(o["F_PYEO__SRT_REAMEKS_04"]);
                string F_PYEO_REMARKS_04 = Convert.ToString(o["F_PYEO_REMARKS_04"]);
                string F_PYEO__SRT_Reameks_05 = Convert.ToString(o["F_PYEO__SRT_Reameks_05"]);
                string F_PYEO_Remarks_05 = Convert.ToString(o["F_PYEO_Remarks_05"]);
                string F_PYEO__SRT_REAMEKS_06 = Convert.ToString(o["F_PYEO__SRT_REAMEKS_06"]);
                string F_PYEO_REMARKS_06 = Convert.ToString(o["F_PYEO_REMARKS_06"]);
                string ChangeReason = Convert.ToString(o["ChangeReason"]);

                DynamicObjectCollection poorderFinances = o["POOrderFinance"] as DynamicObjectCollection;
                foreach (DynamicObject poorderFinance in poorderFinances)
                {
                    DynamicObject settleCurrId = poorderFinance["SettleCurrId"] as DynamicObject;
                    settleCurrNumber = settleCurrId==null?"":Convert.ToString(settleCurrId["Number"]);
                    DynamicObject PayConditionId = poorderFinance["PayConditionId"] as DynamicObject;
                    PayConditionName =PayConditionId==null?"": Convert.ToString(PayConditionId["Number"]);
                    DynamicObject SettleModeId = poorderFinance["SettleModeId"] as DynamicObject;
                    SettleModeName = SettleModeId==null?"":Convert.ToString(SettleModeId["Number"]);
                    DynamicObject ExchangeTypeId = poorderFinance["ExchangeTypeId"] as DynamicObject;
                    ExchangeTypeName = ExchangeTypeId==null?"":Convert.ToString(ExchangeTypeId["Number"]);
                    ExchangeRate = Convert.ToDecimal(poorderFinance["ExchangeRate"]).ToString("#0.00");
                    DynamicObject PriceListtId = poorderFinance["PriceListtId"] as DynamicObject;
                    PriceListtName = PriceListtId==null?"":Convert.ToString(PriceListtId["Number"]);
                    IsIncludedTax = Convert.ToString(poorderFinance["IsIncludedTax"]);
                    FISPRICEEXCLUDETAX = Convert.ToString(poorderFinance["FISPRICEEXCLUDETAX"]);
                    BillTaxAmount = Convert.ToDecimal(poorderFinance["BillTaxAmount"]).ToString("#0.00");
                    BillAmount = Convert.ToDecimal(poorderFinance["BillAmount"]).ToString("#0.00");
                    BillAllAmount = Convert.ToDecimal(poorderFinance["BillAllAmount"]).ToString("#0.00");
                    DynamicObject LocalCurrId = poorderFinance["LocalCurrId"] as DynamicObject;
                    LocalCurrNumebr = LocalCurrId == null?"":Convert.ToString(LocalCurrId["Number"]);
                }

                //处理附件
                List<Dictionary<string, string>> urlArray = Utils.GetUrl(id, this.BusinessInfo.GetForm().Id, this.Context);
                int i = 1;
                StringBuilder attchemnt = new StringBuilder();
                foreach (Dictionary<string, string> url in urlArray)
                {

                    string fjresult = Utils.UploadRequest(url.Values.First(), url.Keys.First());
                    JSONObject jfresultJson = JSONObject.Parse(fjresult);
                    if (!jfresultJson.Keys.Contains("data"))
                    {
                        i++;
                        continue;
                    }
                    JSONObject data = jfresultJson["data"] as JSONObject;
                    string fileId = data["fileid"].ToString();
                    if (i == urlArray.Count)
                    {
                        attchemnt.Append(fileId);
                    }
                    else
                    {
                        attchemnt.Append(fileId + ",");
                    }
                    i++;
                }

                JSONArray mainRoot = new JSONArray();
                JSONObject mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "djlx");
                mainRootItem.Add("fieldValue", billTypeIdX);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "fj");
                mainRootItem.Add("fieldValue", attchemnt.ToString());
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "ywlx");
                mainRootItem.Add("fieldValue", businessType);
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
                mainRootItem.Add("fieldName", "htlx");
                mainRootItem.Add("fieldValue", contractTypeName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "cgrq");
                mainRootItem.Add("fieldValue", date);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "gys");
                mainRootItem.Add("fieldValue", supplierNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "cgzz");
                mainRootItem.Add("fieldValue", purchaseOrgNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "cgbm");
                mainRootItem.Add("fieldValue", purchaseDeptNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "cgz");
                mainRootItem.Add("fieldValue", purchaserGroupNumber);
                mainRoot.Add(mainRootItem);


                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "jsbb");
                mainRootItem.Add("fieldValue", settleCurrNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "ysfs");
                mainRootItem.Add("fieldValue", acctype);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "djzt");
                mainRootItem.Add("fieldValue", "0");
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "ghflxr");
                mainRootItem.Add("fieldValue", ProviderContactNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "fktj");
                mainRootItem.Add("fieldValue", PayConditionName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "jsfs");
                mainRootItem.Add("fieldValue", SettleModeName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "hllx");
                mainRootItem.Add("fieldValue", ExchangeTypeName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "hl");
                mainRootItem.Add("fieldValue", ExchangeRate);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "jmb");
                mainRootItem.Add("fieldValue", PriceListtName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "hs");
                mainRootItem.Add("fieldValue", IsIncludedTax == null?"1": IsIncludedTax.Equals("True")?"0":"1");
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "jws");
                mainRootItem.Add("fieldValue", FISPRICEEXCLUDETAX == null?"1":FISPRICEEXCLUDETAX.Equals("True") ? "0" : "1");
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "se");
                mainRootItem.Add("fieldValue", BillTaxAmount);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "je");
                mainRootItem.Add("fieldValue", BillAmount);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "jshj");
                mainRootItem.Add("fieldValue", BillAllAmount);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "bwb");
                mainRootItem.Add("fieldValue", LocalCurrNumebr);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "htmc");
                mainRootItem.Add("fieldValue", F_SRT_ContractName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "y");
                mainRootItem.Add("fieldValue", F_PYEO__SRT_Reameks_01);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "nry");
                mainRootItem.Add("fieldValue", F_PYEO_Remarks_01);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "e");
                mainRootItem.Add("fieldValue", F_PYEO__SRT_REAMEKS_02);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "nre");
                mainRootItem.Add("fieldValue", F_PYEO_Remarks_02);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "sfsydfmb");
                mainRootItem.Add("fieldValue", F_SRT_ChrckMode==null?"1":F_SRT_ChrckMode.Equals("True") ? "0" : "1");
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "ghf");
                mainRootItem.Add("fieldValue", ProviderNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "zw");
                mainRootItem.Add("fieldValue", FPost);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "sj");
                mainRootItem.Add("fieldValue", FMobile);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "ghfdz");
                mainRootItem.Add("fieldValue", ProviderAddress);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "jsf");
                mainRootItem.Add("fieldValue", SettleNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "skf");
                mainRootItem.Add("fieldValue", ChargeNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "yx");
                mainRootItem.Add("fieldValue", ProviderEMail);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "cgy");
                mainRootItem.Add("fieldValue", PurchaserNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "san");
                mainRootItem.Add("fieldValue", F_PYEO__SRT_REAMEKS_03);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "nrsan");
                mainRootItem.Add("fieldValue", F_PYEO_REMARKS_03);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "s");
                mainRootItem.Add("fieldValue", F_PYEO__SRT_REAMEKS_04);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "nrs");
                mainRootItem.Add("fieldValue", F_PYEO_REMARKS_04);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "w");
                mainRootItem.Add("fieldValue", F_PYEO__SRT_Reameks_05);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "nrw");
                mainRootItem.Add("fieldValue", F_PYEO_Remarks_05);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "l");
                mainRootItem.Add("fieldValue", F_PYEO__SRT_REAMEKS_06);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "nrl");
                mainRootItem.Add("fieldValue", F_PYEO_REMARKS_06);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "bgyy");
                mainRootItem.Add("fieldValue", ChangeReason);
                mainRoot.Add(mainRootItem);

                JSONArray detailData = new JSONArray();

                JSONObject detailDataItem = new JSONObject();
                detailDataItem.Add("tableDBName", "formtable_main_53_dt1");
                JSONArray workflowRequestTableRecords = new JSONArray();

                DynamicObjectCollection POOrderEntrys = o["POOrderEntry"] as DynamicObjectCollection;
                foreach (DynamicObject entry in POOrderEntrys)
                {
                    DynamicObject ChargeProjectID = entry["ChargeProjectID"] as DynamicObject;
                    string ChargeProjectNumber = ChargeProjectID==null?"":Convert.ToString(ChargeProjectID["Number"]);
                    DynamicObject ReceiveDeptId = entry["ReceiveDeptId"] as DynamicObject;
                    string ReceiveDeptNumber = ReceiveDeptId == null ? "" : Convert.ToString(ReceiveDeptId["F_PYEO_Text_OAID"]);

                    string DeliveryControl = Convert.ToString(entry["DeliveryControl"]);
                    string DeliveryMaxQty = Convert.ToDecimal(entry["DeliveryMaxQty"]).ToString("#0.00");
                    string DeliveryMinQty = Convert.ToDecimal(entry["DeliveryMinQty"]).ToString("#0.00");
                    string TimeControl = Convert.ToString(entry["TimeControl"]);
                    string DeliveryBeforeDays = Convert.ToString(entry["DeliveryBeforeDays"]);
                    string DeliveryDelayDays = Convert.ToString(entry["DeliveryDelayDays"]);
                    string DeliveryEarlyDate = Convert.ToDateTime(entry["DeliveryEarlyDate"]).ToString("yyyy-MM-dd");
                    string FDeliveryEarlyTime = Convert.ToDateTime(entry["DeliveryEarlyDate"]).ToString("HH:mm");
                    string DeliveryLastDate = Convert.ToDateTime(entry["DeliveryLastDate"]).ToString("yyyy-MM-dd");
                    string DeliveryLastTime = Convert.ToDateTime(entry["DeliveryLastDate"]).ToString("HH:mm");
                    string ProductType = Convert.ToString(entry["ProductType"]);
                    ProductType = this.ProductsType(ProductType);
                    DynamicObject MaterialId = entry["MaterialId"] as DynamicObject;
                    string MaterialNumber = MaterialId==null?"":Convert.ToString(MaterialId["Number"]);
                    string MaterialName = MaterialId == null ? "" : Convert.ToString(MaterialId["Name"]);
                    string Specification = MaterialId == null ? "" : Convert.ToString(MaterialId["Specification"]);
                    DynamicObject UnitId = entry["UnitId"] as DynamicObject;
                    string unitNumber = UnitId==null?"":Convert.ToString(UnitId["Number"]);
                    string QtyX = Convert.ToDecimal(entry["QtyX"]).ToString("#0.00");
                    DynamicObject PriceUnitId = entry["PriceUnitId"] as DynamicObject;
                    string priceUnitNumber =PriceUnitId==null?"": Convert.ToString(PriceUnitId["Number"]);
                    string PriceUnitQty = Convert.ToDecimal(entry["PriceUnitQty"]).ToString("#0.00");
                    string DeliveryDate = Convert.ToDateTime(entry["DeliveryDate"]).ToString("yyyy-MM-dd");
                    string ELocationAddress = string.Empty;
                    string Price = Convert.ToDecimal(entry["Price"]).ToString("#0.00");
                    string TaxPrice = Convert.ToDecimal(entry["TaxPrice"]).ToString("#0.00");
                    string DiscountRate = Convert.ToDecimal(entry["DiscountRate"]).ToString("#0.00");
                    string Discount = Convert.ToDecimal(entry["Discount"]).ToString("#0.00");
                    string TaxRate = (Convert.ToDecimal(entry["TaxRate"]) / 100).ToString("#0.00");
                    string TaxAmount = Convert.ToDecimal(entry["TaxAmount"]).ToString("#0.00");
                    string AllAmount = Convert.ToDecimal(entry["AllAmount"]).ToString("#0.00");
                    string Amount = Convert.ToDecimal(entry["Amount"]).ToString("#0.00");
                    DynamicObject RequireOrgId = entry["RequireOrgId"] as DynamicObject;
                    string RequireOrgNumber = RequireOrgId == null ? "" : Convert.ToString(RequireOrgId["F_PYEO_Text_OAID"]);
                    DynamicObject RequireDeptId = entry["RequireDeptId"] as DynamicObject;
                    string RequireDeptNumber = RequireDeptId == null ? "" : Convert.ToString(RequireDeptId["F_PYEO_Text_OAID"]);
                    DynamicObject RequireStaffId = entry["RequireStaffId"] as DynamicObject;
                    string RequireStaffNumber = RequireStaffId==null?"":Convert.ToString(RequireStaffId["Number"]);
                    RequireStaffNumber = Utils.getPersonOAid(this.Context, RequireStaffNumber);
                    DynamicObject ReceiveOrgId = entry["ReceiveOrgId"] as DynamicObject;
                    string ReceiveOrgNumber = ReceiveOrgId == null ? "" : Convert.ToString(ReceiveOrgId["F_PYEO_Text_OAID"]);
                    DynamicObject SettleOrgId = entry["SettleOrgId"] as DynamicObject;
                    string SettleOrgNumber = SettleOrgId == null ? "" : Convert.ToString(SettleOrgId["F_PYEO_Text_OAID"]);
                    string GiveAway = Convert.ToString(entry["GiveAway"]);
                    string Note = Convert.ToString(entry["Note"]);
                    DynamicObject StockUnitID = entry["StockUnitID"] as DynamicObject;
                    string StockUnitNumber = StockUnitID==null?"":Convert.ToString(StockUnitID["Number"]);
                    string StockQty = Convert.ToDecimal(entry["StockQty"]).ToString("#0.00");
                    DynamicObject F_SRT_PROJECT1 = entry["F_SRT_PROJECT1"] as DynamicObject;
                    string F_SRT_PROJECT1Number = F_SRT_PROJECT1==null?"":Convert.ToString(F_SRT_PROJECT1["Number"]);
                    string PlanUnitIDNumber = string.Empty;
                    string DeliCommitQty = string.Empty;
                    string DeliRemainQty = string.Empty;
                    string ELocation = string.Empty;
                    string CHANGETYPE = Convert.ToString(entry["CHANGETYPE"]);
                    CHANGETYPE = this.ChangeType(CHANGETYPE);
                    //子单据体
                    DynamicObjectCollection POOrderEntryDeliPlan = entry["POOrderEntryDeliPlan"] as DynamicObjectCollection;
                    foreach (DynamicObject EntryDeliPlan in POOrderEntryDeliPlan)
                    {
                        ELocationAddress = Convert.ToString(EntryDeliPlan["ELocationAddress"]);
                        DynamicObject PlanUnitID = EntryDeliPlan["PlanUnitID"] as DynamicObject;
                        PlanUnitIDNumber = PlanUnitID==null?"":Convert.ToString(PlanUnitID["Number"]);
                        DeliCommitQty = Convert.ToDecimal(EntryDeliPlan["DeliCommitQty"]).ToString("#0.00");
                        DeliRemainQty = Convert.ToDecimal(EntryDeliPlan["DeliRemainQty"]).ToString("#0.00");
                        ELocation = Convert.ToString(EntryDeliPlan["ELocation"]);

                    }

                    JSONObject workflowRequestTableRecordsItem = new JSONObject();
                    workflowRequestTableRecordsItem.Add("recordOrder", "0");
                    JSONArray workflowRequestTableFields = new JSONArray();


                    JSONObject workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "fyxm");
                    workflowRequestTableFieldsItem.Add("fieldValue", ChargeProjectNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "slbm");
                    workflowRequestTableFieldsItem.Add("fieldValue", ReceiveDeptNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "kzjhsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", DeliveryControl==null?"1":DeliveryControl.Equals("True") ? "0" : "1");
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jhsx");
                    workflowRequestTableFieldsItem.Add("fieldValue", DeliveryMaxQty);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jhxx");
                    workflowRequestTableFieldsItem.Add("fieldValue", DeliveryMinQty);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "kzjhsj");
                    workflowRequestTableFieldsItem.Add("fieldValue",TimeControl == null?"1": TimeControl.Equals("True")?"0":"1");
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jhtqts");
                    workflowRequestTableFieldsItem.Add("fieldValue", DeliveryBeforeDays);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jhycts");
                    workflowRequestTableFieldsItem.Add("fieldValue", DeliveryDelayDays);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "zzjhrq");
                    workflowRequestTableFieldsItem.Add("fieldValue", DeliveryEarlyDate);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "zzjhsj");
                    workflowRequestTableFieldsItem.Add("fieldValue", FDeliveryEarlyTime);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "zwjhrq");
                    workflowRequestTableFieldsItem.Add("fieldValue", DeliveryLastDate);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "zwjhsj");
                    workflowRequestTableFieldsItem.Add("fieldValue", DeliveryLastTime);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "slyc");
                    workflowRequestTableFieldsItem.Add("fieldValue", TaxRate);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "wwcplx");
                    workflowRequestTableFieldsItem.Add("fieldValue", ProductType);
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
                    workflowRequestTableFieldsItem.Add("fieldName", "ggxh");
                    workflowRequestTableFieldsItem.Add("fieldValue", Specification);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "cgdw");
                    workflowRequestTableFieldsItem.Add("fieldValue", unitNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "cgsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", QtyX);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jjdw");
                    workflowRequestTableFieldsItem.Add("fieldValue", priceUnitNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jjsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", PriceUnitQty);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jhrq");
                    workflowRequestTableFieldsItem.Add("fieldValue", DeliveryDate);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jhdz");
                    workflowRequestTableFieldsItem.Add("fieldValue", ELocationAddress);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "dj");
                    workflowRequestTableFieldsItem.Add("fieldValue", Price);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "hsdj");
                    workflowRequestTableFieldsItem.Add("fieldValue", TaxPrice);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "zkl");
                    workflowRequestTableFieldsItem.Add("fieldValue", DiscountRate);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "zke");
                    workflowRequestTableFieldsItem.Add("fieldValue", Discount);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    //workflowRequestTableFieldsItem = new JSONObject();
                    //workflowRequestTableFieldsItem.Add("fieldName", "sl");
                    //workflowRequestTableFieldsItem.Add("fieldValue", TaxRate);
                    //workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "se");
                    workflowRequestTableFieldsItem.Add("fieldValue", TaxAmount);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jshj");
                    workflowRequestTableFieldsItem.Add("fieldValue", AllAmount);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "je");
                    workflowRequestTableFieldsItem.Add("fieldValue", Amount);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xqzz");
                    workflowRequestTableFieldsItem.Add("fieldValue", RequireOrgNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xqbm");
                    workflowRequestTableFieldsItem.Add("fieldValue", RequireDeptNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xqr");
                    workflowRequestTableFieldsItem.Add("fieldValue", RequireStaffNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "slzz");
                    workflowRequestTableFieldsItem.Add("fieldValue", ReceiveOrgNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jszz");
                    workflowRequestTableFieldsItem.Add("fieldValue", SettleOrgNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "sfzp");
                    workflowRequestTableFieldsItem.Add("fieldValue", GiveAway==null?"1":GiveAway.Equals("True") ? "0" : "1");
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "bz");
                    workflowRequestTableFieldsItem.Add("fieldValue", Note);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "kcdw");
                    workflowRequestTableFieldsItem.Add("fieldValue", StockUnitNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "kcsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", StockQty);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xm");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_SRT_PROJECT1Number);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jhdw");
                    workflowRequestTableFieldsItem.Add("fieldValue", PlanUnitIDNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "yjhsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", DeliCommitQty);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "syjhsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", DeliRemainQty);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jhdd");
                    workflowRequestTableFieldsItem.Add("fieldValue", ELocation);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "bglx");
                    workflowRequestTableFieldsItem.Add("fieldValue", CHANGETYPE);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableRecordsItem.Add("workflowRequestTableFields", workflowRequestTableFields);
                    workflowRequestTableRecords.Add(workflowRequestTableRecordsItem);

                }
                detailDataItem.Add("workflowRequestTableRecords", workflowRequestTableRecords);
                detailData.Add(detailDataItem);

                detailDataItem = new JSONObject();
                detailDataItem.Add("tableDBName", "formtable_main_53_dt2");
                workflowRequestTableRecords = new JSONArray();

                DynamicObjectCollection FIinstallment = o["FIinstallment"] as DynamicObjectCollection;
                foreach (DynamicObject IinstallmentItem in FIinstallment)
                {
                    string YFDATE = Convert.ToDateTime(IinstallmentItem["YFDATE"]).ToString("yyyy-MM-dd");
                    string YFRATIO = Convert.ToDecimal(IinstallmentItem["YFRATIO"]).ToString("#0.00");
                    string YFAMOUNT = Convert.ToDecimal(IinstallmentItem["YFAMOUNT"]).ToString("#0.00");
                    string ISPREPAYMENT = Convert.ToString(IinstallmentItem["ISPREPAYMENT"]);
                    string RelBillNo = Convert.ToString(IinstallmentItem["RelBillNo"]);
                    string InsPrepaidAmount = Convert.ToDecimal(IinstallmentItem["InsPrepaidAmount"]).ToString("#0.00");
                    DynamicObject F_SRT_ProjectNode1 = IinstallmentItem["F_SRT_ProjectNode1"] as DynamicObject;
                    string F_SRT_ProjectNode1Number = F_SRT_ProjectNode1==null?"":Convert.ToString(F_SRT_ProjectNode1["Number"]);
                    string ACTUALAMOUNT = Convert.ToDecimal(IinstallmentItem["ACTUALAMOUNT"]).ToString("#0.00");
                    string PayJoinAmount = Convert.ToDecimal(IinstallmentItem["PayJoinAmount"]).ToString("#0.00");
                    string FRemarks = Convert.ToString(IinstallmentItem["FRemarks"]);
                    string F_SRT_SFFK1 = Convert.ToString(IinstallmentItem["F_SRT_SFFK1"]).Equals("True")?"0":"1";
                    string F_SRT_SFSH1 = Convert.ToString(IinstallmentItem["F_SRT_SFSH1"]);

                    JSONObject workflowRequestTableRecordsItem = new JSONObject();
                    workflowRequestTableRecordsItem.Add("recordOrder", "0");
                    JSONArray workflowRequestTableFields = new JSONArray();


                    JSONObject workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "yfrq");
                    workflowRequestTableFieldsItem.Add("fieldValue", YFDATE);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "yfbl");
                    workflowRequestTableFieldsItem.Add("fieldValue", YFRATIO);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "yfje");
                    workflowRequestTableFieldsItem.Add("fieldValue", YFAMOUNT);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "sfyf");
                    workflowRequestTableFieldsItem.Add("fieldValue", ISPREPAYMENT.Equals("True")?"0":"1");
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "yfdh");
                    workflowRequestTableFieldsItem.Add("fieldValue", RelBillNo);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "dcyfed");
                    workflowRequestTableFieldsItem.Add("fieldValue", InsPrepaidAmount);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xmjd");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_SRT_ProjectNode1Number);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "sjyfje");
                    workflowRequestTableFieldsItem.Add("fieldValue", ACTUALAMOUNT);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "fkglje");
                    workflowRequestTableFieldsItem.Add("fieldValue", PayJoinAmount);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "bz");
                    workflowRequestTableFieldsItem.Add("fieldValue", FRemarks);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "sfsk");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_SRT_SFFK1.Equals("True")?"0":"1");
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "sfsh");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_SRT_SFSH1.Equals("True")?"0":"1");
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
                                     + "&requestName=采购订单变更单已到达"
                                     + "&workflowId=12", personId);//&detailData=" + detailData.ToString()

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
        /// 获取单据类型
        /// </summary>
        /// <param name="erpId"></param>
        /// <returns></returns>
        private string getOABillType(string erpId) 
        {
            try
            {
                string queryErpIdSql = string.Format(@"select FNUMBER from T_BAS_BILLTYPE where FBILLTYPEID = '{0}' ", erpId);
                string erpNumber = DBUtils.ExecuteDynamicObject(this.Context, queryErpIdSql)[0]["FNUMBER"].ToString();

                string queryOAidSql = string.Format(@"select F_PYEO_OAID from PYEO_t_Cust_Entry100024 where F_PYEO_ERPID = '{0}'", erpNumber);
                return DBUtils.ExecuteDynamicObject(this.Context, queryOAidSql)[0]["F_PYEO_OAID"].ToString();
            }
            catch (Exception e)
            {
                return "";
            }
        }
        /// <summary>
        /// 业务类型
        /// </summary>
        /// <param name="erpId"></param>
        /// <returns></returns>
        private string getBizType(string erpId) 
        {
            try
            {
                string queryOAidSql = string.Format(@"select F_PYEO_OAID1 from PYEO_t_Cust_Entry100028 where F_PYEO_ERPID1 = '{0}'", erpId);
                return DBUtils.ExecuteDynamicObject(this.Context, queryOAidSql)[0]["F_PYEO_OAID1"].ToString();
            }
            catch (Exception e)
            {
                return "";
            }
        }
        /// <summary>
        /// 验收方式
        /// </summary>
        /// <param name="erpId"></param>
        /// <returns></returns>
        private string getAcceptance(string erpId)
        {
            try
            {
                string queryOAidSql = string.Format(@"select F_PYEO_OAID2 from PYEO_t_Cust_Entry100029 where F_PYEO_ERPID2 = '{0}'", erpId);
                return DBUtils.ExecuteDynamicObject(this.Context, queryOAidSql)[0]["F_PYEO_OAID2"].ToString();
            }
            catch (Exception e)
            {
                return "";
            }
        }
        /// <summary>
        /// 产品类型
        /// </summary>
        /// <param name="erpId"></param>
        /// <returns></returns>
        private string ProductsType(string erpId)
        {
            try
            {
                string queryOAidSql = string.Format(@"select F_PYEO_OAID4 from PYEO_t_Cust_Entry100031 where F_PYEO_ERPID4 = '{0}'", erpId);
                return DBUtils.ExecuteDynamicObject(this.Context, queryOAidSql)[0]["F_PYEO_OAID4"].ToString();
            }
            catch (Exception e)
            {
                return "";
            }
        }
        /// <summary>
        /// 变更类型
        /// </summary>
        /// <param name="erpId"></param>
        /// <returns></returns>
        private string ChangeType(string erpId)
        {
            try
            {
                string queryOAidSql = string.Format(@"select F_PYEO_OAID5 from PYEO_t_Cust_Entry100034 where F_PYEO_ERPID5 = '{0}'", erpId);
                return DBUtils.ExecuteDynamicObject(this.Context, queryOAidSql)[0]["F_PYEO_OAID5"].ToString();
            }
            catch (Exception e)
            {
                return "";
            }
        }

        private string RequestType(string erpId)
        {
            try
            {
                string queryOAidSql = string.Format(@"select F_PYEO_OAID7 from PYEO_t_Cust_Entry100032 where F_PYEO_ERPID7 = '{0}'", erpId);
                return DBUtils.ExecuteDynamicObject(this.Context, queryOAidSql)[0]["F_PYEO_OAID7"].ToString();
            }
            catch (Exception e)
            {
                return "";
            }
        }
    }
}
