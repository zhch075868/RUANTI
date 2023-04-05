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
namespace DFYR.RTJQR.PlauginService.FunctionPlugin
{
    [Kingdee.BOS.Util.HotUpdate, Description("采购订单变更单生效至OA")]
    public class POXChangeEffective : AbstractOperationServicePlugIn
    {
        public override void OnPreparePropertys(PreparePropertysEventArgs e)
        {
            //e.FieldKeys.Add("");
        }

        public override void OnPrepareOperationServiceOption(OnPrepareOperationServiceEventArgs e)
        {
            e.AllowSetOperationResult = false;
        }
        public override void BeginOperationTransaction(BeginOperationTransactionArgs e)
        {
            base.BeginOperationTransaction(e);
            foreach (DynamicObject item in e.DataEntitys)
            {
                string PKIDX = Convert.ToString(item["PKIDX"]);
                FormMetadata meta = MetaDataServiceHelper.Load(this.Context, "PUR_PurchaseOrder") as FormMetadata;
                DynamicObject o = BusinessDataServiceHelper.LoadSingle(
                               this.Context,
                               PKIDX,
                               meta.BusinessInfo.GetDynamicObjectType());
                string billNo = Convert.ToString(o["BillNo"]);
                JSONObject pushjson = new JSONObject();
                JSONObject dataJson = new JSONObject();

                JSONArray data = new JSONArray();
                JSONObject dateItem = new JSONObject();
                JSONObject operationinfo = new JSONObject();
                JSONObject mainTable = new JSONObject();

                mainTable.Add("erpnumber", billNo);

                JSONArray detail1 = new JSONArray();
                DynamicObjectCollection POOrderEntrys = o["POOrderEntry"] as DynamicObjectCollection;
                foreach (DynamicObject entry in POOrderEntrys)
                {
                    JSONObject entryItem = new JSONObject();
                    JSONObject entryData = new JSONObject();
                    JSONObject operate = new JSONObject();
                    operate.Add("action", "Delete");
                    operate.Add("actionDescribe", "Delete");
                    entryItem.Add("operate", operate);

                    string entryId = Convert.ToString(entry["Id"]);
                    entryData.Add("erpfkjhid", entryId);

                    entryItem.Add("data", entryData);
                    detail1.Add(entryItem);

                }
                dateItem.Add("detail1", detail1);
                JSONArray detail2 = new JSONArray();

                DynamicObjectCollection FIinstallment = o["FIinstallment"] as DynamicObjectCollection;
                foreach (DynamicObject IinstallmentItem in FIinstallment)
                {
                    JSONObject FIinstallmentItem = new JSONObject();
                    JSONObject FIinstallmentData = new JSONObject();
                    JSONObject operate = new JSONObject();
                    operate.Add("action", "Delete");
                    operate.Add("actionDescribe", "Delete");
                    FIinstallmentItem.Add("operate", operate);

                    string entryId = Convert.ToString(IinstallmentItem["Id"]);
                    FIinstallmentData.Add("erpfkjhid", entryId);

                    FIinstallmentItem.Add("data", FIinstallmentData);
                    detail2.Add(FIinstallmentItem);
                }
                dateItem.Add("detail2", detail2);


                operationinfo.Add("operationDate", DateTime.Now.ToString("yyyy-MM-dd"));
                operationinfo.Add("operator", "1");
                operationinfo.Add("operationTime", DateTime.Now.ToString("HH:mm:ss"));

                dateItem.Add("operationinfo", operationinfo);
                dateItem.Add("mainTable", mainTable);
                data.Add(dateItem);
                dataJson.Add("data", data);


                JSONObject header = new JSONObject();
                string datetime = DateTime.Now.ToString("yyyyMMddHHmmss");
                header.Add("systemid", "ERP");
                header.Add("currentDateTime", datetime);
                header.Add("Md5", Utils.StringToMD5Hash("ERPerp" + datetime));

                dataJson.Add("header", header);

                string results = Utils.PostUrl(Utils.pushCGDDurl, "datajson=" + dataJson.ToString());

                JSONObject resultJson = JSONObject.Parse(results);
                string retCode = Convert.ToString(resultJson["status"]);

                if (retCode.Equals("1"))
                {

                }
                else
                {
                    throw new KDException("", results);
                }
            }
        }



        /// <summary>
        /// 操作开始前功能处理
        /// </summary>
        /// <param name="e"></param>
        public override void EndOperationTransaction(EndOperationTransactionArgs e)
        {
            foreach (DynamicObject item in e.DataEntitys)
            {
                string PKIDX = Convert.ToString(item["PKIDX"]);
                FormMetadata meta = MetaDataServiceHelper.Load(this.Context, "PUR_PurchaseOrder") as FormMetadata;
                DynamicObject o = BusinessDataServiceHelper.LoadSingle(
                               this.Context,
                               PKIDX,
                               meta.BusinessInfo.GetDynamicObjectType());

                string billNo = Convert.ToString(o["BillNo"]);
                string documentStatus = Convert.ToString(o["DocumentStatus"]);
                DynamicObject F_SRT_CGLXId = o["F_SRT_CGLX"] as DynamicObject;
                string F_SRT_CGLX = Convert.ToString(F_SRT_CGLXId["Number"]);
                if (F_SRT_CGLX.Equals("CDFL0001") == false && F_SRT_CGLX.Equals("CDFL0006") == false)
                {
                    return;
                }
                F_SRT_CGLX = this.PurType(F_SRT_CGLX);


                DynamicObject billTypeId = o["BillTypeId"] as DynamicObject; ;
                string billTypeIdX = Convert.ToString(billTypeId["Number"]);
                billTypeIdX = this.getOABillType(billTypeIdX);
                string businessType = Convert.ToString(o["BusinessType"]);
                businessType = this.getBizType(businessType);

                string erpContractType = Convert.ToString(o["F_SRT_CONTRACTYPE"]);
                string contractTypeName = this.contractType(erpContractType);

                string date = Convert.ToDateTime(o["Date"]).ToString("yyyy-MM-dd");
                DynamicObject supplierId = o["SupplierId"] as DynamicObject;
                string supplierNumber = supplierId == null ? "" : Convert.ToString(supplierId["Number"]);
                DynamicObject purchaseOrgId = o["PurchaseOrgId"] as DynamicObject;
                string purchaseOrgNumber = purchaseOrgId == null ? "" : Convert.ToString(purchaseOrgId["F_PYEO_Text_OAID"]);
                DynamicObject purchaseDeptId = o["PurchaseDeptId"] as DynamicObject;
                string purchaseDeptNumber = purchaseDeptId == null ? "" : Convert.ToString(purchaseDeptId["F_PYEO_Text_OAID"]);
                DynamicObject purchaserGroupId = o["PurchaserGroupId"] as DynamicObject;
                string purchaserGroupNumber = purchaserGroupId == null ? "" : Convert.ToString(purchaserGroupId["Number"]);
                string settleCurrNumber = string.Empty;
                string PayConditionName = string.Empty;
                string acctype = Convert.ToString(o["FACCTYPE"]);
                acctype = this.getAcceptance(acctype);
                DynamicObject providerContactId = o["ProviderContactId"] as DynamicObject;
                string ProviderContactNumber = providerContactId == null ? "" : Convert.ToString(providerContactId["Number"]);
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
                string ProviderNumber = ProviderId == null ? "" : Convert.ToString(ProviderId["Number"]);
                string FPost = providerContactId == null ? "" : Convert.ToString(providerContactId["providerContactId"]);
                string FMobile = providerContactId == null ? "" : Convert.ToString(providerContactId["FMobile"]);
                string ProviderAddress = Convert.ToString(o["ProviderAddress"]);
                DynamicObject SettleId = o["SettleId"] as DynamicObject;
                string SettleNumber = SettleId == null ? "" : Convert.ToString(SettleId["Number"]);
                DynamicObject ChargeId = o["ChargeId"] as DynamicObject;
                string ChargeNumber = ChargeId == null ? "" : Convert.ToString(ChargeId["Number"]);
                string ProviderEMail = Convert.ToString(o["ProviderEMail"]);
                DynamicObject PurchaserId = o["PurchaserId"] as DynamicObject;
                string PurchaserNumber = PurchaserId == null ? "" : Convert.ToString(PurchaserId["Number"]);
                string F_PYEO__SRT_REAMEKS_03 = Convert.ToString(o["F_PYEO__SRT_REAMEKS_03"]);
                string F_PYEO_REMARKS_03 = Convert.ToString(o["F_PYEO_REMARKS_03"]);
                string F_PYEO__SRT_REAMEKS_04 = Convert.ToString(o["F_PYEO__SRT_REAMEKS_04"]);
                string F_PYEO_REMARKS_04 = Convert.ToString(o["F_PYEO_REMARKS_04"]);
                string F_PYEO__SRT_Reameks_05 = Convert.ToString(o["F_PYEO__SRT_Reameks_05"]);
                string F_PYEO_Remarks_05 = Convert.ToString(o["F_PYEO_Remarks_05"]);
                string F_PYEO__SRT_REAMEKS_06 = Convert.ToString(o["F_PYEO__SRT_REAMEKS_06"]);
                string F_PYEO_REMARKS_06 = Convert.ToString(o["F_PYEO_REMARKS_06"]);
                //string ChangeReason = Convert.ToString(o["ChangeReason"]);

                string F_SRT_CheckBox = Convert.ToString(o["F_SRT_CheckBox"]).Equals("True") ? "0" : "1";
                DynamicObject F_SRT_LSHTH2 = o["F_PYEO_Base"] as DynamicObject;
                string F_SRT_LSHTH2Number = F_SRT_LSHTH2 == null ? "" : Convert.ToString(F_SRT_LSHTH2["Number"]);
                //string F_SRT_LSHTH1 = Convert.ToString(o["F_SRT_LSHTH1"]);

                DynamicObjectCollection poorderFinances = o["POOrderFinance"] as DynamicObjectCollection;
                foreach (DynamicObject poorderFinance in poorderFinances)
                {
                    DynamicObject settleCurrId = poorderFinance["SettleCurrId"] as DynamicObject;
                    settleCurrNumber = settleCurrId == null ? "" : Convert.ToString(settleCurrId["Number"]);
                    DynamicObject PayConditionId = poorderFinance["PayConditionId"] as DynamicObject;
                    PayConditionName = PayConditionId == null ? "" : Convert.ToString(PayConditionId["Number"]);
                    DynamicObject SettleModeId = poorderFinance["SettleModeId"] as DynamicObject;
                    SettleModeName = SettleModeId == null ? "" : Convert.ToString(SettleModeId["Name"]);
                    DynamicObject ExchangeTypeId = poorderFinance["ExchangeTypeId"] as DynamicObject;
                    ExchangeTypeName = ExchangeTypeId == null ? "" : Convert.ToString(ExchangeTypeId["Name"]);
                    ExchangeRate = Convert.ToDecimal(poorderFinance["ExchangeRate"]).ToString("#0.00");
                    DynamicObject PriceListtId = poorderFinance["PriceListtId"] as DynamicObject;
                    PriceListtName = PriceListtId == null ? "" : Convert.ToString(PriceListtId["Number"]);
                    IsIncludedTax = Convert.ToString(poorderFinance["IsIncludedTax"]);
                    FISPRICEEXCLUDETAX = Convert.ToString(poorderFinance["FISPRICEEXCLUDETAX"]);
                    BillTaxAmount = Convert.ToDecimal(poorderFinance["BillTaxAmount"]).ToString("#0.00");
                    BillAmount = Convert.ToDecimal(poorderFinance["BillAmount"]).ToString("#0.00");
                    BillAllAmount = Convert.ToDecimal(poorderFinance["BillAllAmount"]).ToString("#0.00");
                    DynamicObject LocalCurrId = poorderFinance["LocalCurrId"] as DynamicObject;
                    LocalCurrNumebr = LocalCurrId == null ? "" : Convert.ToString(LocalCurrId["Name"]);
                }

                JSONObject pushjson = new JSONObject();
                JSONObject dataJson = new JSONObject();

                JSONArray data = new JSONArray();
                JSONObject dateItem = new JSONObject();
                JSONObject operationinfo = new JSONObject();
                JSONObject mainTable = new JSONObject();

                mainTable.Add("djlx", billTypeIdX);
                mainTable.Add("ywlx", businessType);
                mainTable.Add("djbh", billNo);
                mainTable.Add("erpnumber", billNo);
                mainTable.Add("htlx", contractTypeName);
                mainTable.Add("cgrq", date);
                mainTable.Add("gys", supplierNumber);
                mainTable.Add("cgzz", purchaseOrgNumber);
                mainTable.Add("cgbm", purchaseDeptNumber);
                mainTable.Add("cgz", purchaserGroupNumber);
                mainTable.Add("jsbb", settleCurrNumber);
                mainTable.Add("ysfs", acctype);
                mainTable.Add("ghflxr", ProviderContactNumber);
                mainTable.Add("fktj", PayConditionName);
                mainTable.Add("jsfs", SettleModeName);
                mainTable.Add("hllx", ExchangeTypeName);
                mainTable.Add("hl", ExchangeRate);
                mainTable.Add("jmb", PriceListtName);
                mainTable.Add("hs", IsIncludedTax == null ? "1" : IsIncludedTax.Equals("True") ? "0" : "1");
                mainTable.Add("jws", FISPRICEEXCLUDETAX == null ? "1" : FISPRICEEXCLUDETAX.Equals("True") ? "0" : "1");
                mainTable.Add("se", BillTaxAmount);
                mainTable.Add("je", BillAmount);
                mainTable.Add("jshj", BillAllAmount);
                mainTable.Add("bwb", LocalCurrNumebr);
                mainTable.Add("htmc", F_SRT_ContractName);
                mainTable.Add("y", F_PYEO__SRT_Reameks_01);
                mainTable.Add("nry", F_PYEO_Remarks_01);
                mainTable.Add("e", F_PYEO__SRT_REAMEKS_02);
                mainTable.Add("nre", F_PYEO_Remarks_02);
                mainTable.Add("sfsydfmb", F_SRT_ChrckMode == null ? "1" : F_SRT_ChrckMode.Equals("True") ? "0" : "1");
                mainTable.Add("ghf", ProviderNumber);
                mainTable.Add("zw", FPost);
                mainTable.Add("sj", FMobile);
                mainTable.Add("ghfdz", ProviderAddress);
                mainTable.Add("jsf", SettleNumber);
                mainTable.Add("skf", ChargeNumber);
                mainTable.Add("yx", ProviderEMail);
                mainTable.Add("cgy", PurchaserNumber);
                mainTable.Add("san", F_PYEO__SRT_REAMEKS_03);
                mainTable.Add("nrsan", F_PYEO_REMARKS_03);
                mainTable.Add("s", F_PYEO__SRT_REAMEKS_04);
                mainTable.Add("nrs", F_PYEO_REMARKS_04);
                mainTable.Add("w", F_PYEO__SRT_Reameks_05);
                mainTable.Add("nrw", F_PYEO_Remarks_05);
                mainTable.Add("l", F_PYEO__SRT_REAMEKS_06);
                mainTable.Add("nrl", F_PYEO_REMARKS_06);

                JSONArray detail1 = new JSONArray();
                DynamicObjectCollection POOrderEntrys = o["POOrderEntry"] as DynamicObjectCollection;
                foreach (DynamicObject entry in POOrderEntrys)
                {
                    JSONObject entryItem = new JSONObject();
                    JSONObject entryData = new JSONObject();
                    JSONObject operate = new JSONObject();
                    operate.Add("action", "SaveOrUpdate");

                    operate.Add("actionDescribe", "SaveOrUpdate");

                    entryItem.Add("operate", operate);

                    string entryId = Convert.ToString(entry["Id"]);
                    DynamicObject ChargeProjectID = entry["ChargeProjectID"] as DynamicObject;
                    string ChargeProjectNumber = ChargeProjectID == null ? "" : Convert.ToString(ChargeProjectID["Number"]);
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
                    string MaterialNumber = MaterialId == null ? "" : Convert.ToString(MaterialId["Number"]);
                    string MaterialName = MaterialId == null ? "" : Convert.ToString(MaterialId["Name"]);
                    string Specification = MaterialId == null ? "" : Convert.ToString(MaterialId["Specification"]);
                    DynamicObject UnitId = entry["UnitId"] as DynamicObject;
                    string unitNumber = UnitId == null ? "" : Convert.ToString(UnitId["Number"]);
                    string QtyX = Convert.ToDecimal(entry["Qty"]).ToString("#0.00");
                    DynamicObject PriceUnitId = entry["PriceUnitId"] as DynamicObject;
                    string priceUnitNumber = PriceUnitId == null ? "" : Convert.ToString(PriceUnitId["Number"]);
                    string PriceUnitQty = Convert.ToDecimal(entry["PriceUnitQty"]).ToString("#0.00");
                    string DeliveryDate = Convert.ToDateTime(entry["DeliveryDate"]).ToString("yyyy-MM-dd");
                    string ELocationAddress = string.Empty;
                    string Price = Convert.ToDecimal(entry["Price"]).ToString("#0.00");
                    string TaxPrice = Convert.ToDecimal(entry["TaxPrice"]).ToString("#0.00");
                    string DiscountRate = Convert.ToDecimal(entry["DiscountRate"]).ToString("#0.00");
                    string Discount = Convert.ToDecimal(entry["Discount"]).ToString("#0.00");
                    string TaxRate = Convert.ToDecimal(entry["TaxRate"]).ToString("#0.00");
                    string TaxAmount = Convert.ToDecimal(entry["TaxAmount"]).ToString("#0.00");
                    string AllAmount = Convert.ToDecimal(entry["AllAmount"]).ToString("#0.00");
                    string Amount = Convert.ToDecimal(entry["Amount"]).ToString("#0.00");
                    DynamicObject RequireOrgId = entry["RequireOrgId"] as DynamicObject;
                    string RequireOrgNumber = RequireOrgId == null ? "" : Convert.ToString(RequireOrgId["F_PYEO_Text_OAID"]);
                    DynamicObject RequireDeptId = entry["RequireDeptId"] as DynamicObject;
                    string RequireDeptNumber = RequireDeptId == null ? "" : Convert.ToString(RequireDeptId["F_PYEO_Text_OAID"]);
                    DynamicObject RequireStaffId = entry["RequireStaffId"] as DynamicObject;
                    string RequireStaffNumber = RequireStaffId == null ? "" : Convert.ToString(RequireStaffId["Number"]);
                    RequireStaffNumber = Utils.getPersonOAid(this.Context, RequireStaffNumber);
                    DynamicObject ReceiveOrgId = entry["ReceiveOrgId"] as DynamicObject;
                    string ReceiveOrgNumber = ReceiveOrgId == null ? "" : Convert.ToString(ReceiveOrgId["F_PYEO_Text_OAID"]);
                    DynamicObject SettleOrgId = entry["SettleOrgId"] as DynamicObject;
                    string SettleOrgNumber = SettleOrgId == null ? "" : Convert.ToString(SettleOrgId["F_PYEO_Text_OAID"]);
                    string GiveAway = Convert.ToString(entry["GiveAway"]);
                    string Note = Convert.ToString(entry["Note"]);
                    DynamicObject StockUnitID = entry["StockUnitID"] as DynamicObject;
                    string StockUnitNumber = StockUnitID == null ? "" : Convert.ToString(StockUnitID["Number"]);
                    string StockQty = Convert.ToDecimal(entry["StockQty"]).ToString("#0.00");
                    DynamicObject F_SRT_PROJECT1 = entry["F_SRT_PROJECT1"] as DynamicObject;
                    string F_SRT_PROJECT1Number = F_SRT_PROJECT1 == null ? "" : Convert.ToString(F_SRT_PROJECT1["Number"]);
                    string PlanUnitIDNumber = string.Empty;
                    string DeliCommitQty = string.Empty;
                    string DeliRemainQty = string.Empty;
                    string ELocation = string.Empty;
                    //string CHANGETYPE = Convert.ToString(entry["CHANGETYPE"]);

                    //子单据体
                    DynamicObjectCollection POOrderEntryDeliPlan = entry["POOrderEntryDeliPlan"] as DynamicObjectCollection;
                    foreach (DynamicObject EntryDeliPlan in POOrderEntryDeliPlan)
                    {
                        ELocationAddress = Convert.ToString(EntryDeliPlan["ELocationAddress"]);
                        DynamicObject PlanUnitID = EntryDeliPlan["PlanUnitID"] as DynamicObject;
                        PlanUnitIDNumber = PlanUnitID == null ? "" : Convert.ToString(PlanUnitID["Number"]);
                        DeliCommitQty = Convert.ToDecimal(EntryDeliPlan["DeliCommitQty"]).ToString("#0.00");
                        DeliRemainQty = Convert.ToDecimal(EntryDeliPlan["DeliRemainQty"]).ToString("#0.00");
                        ELocation = Convert.ToString(EntryDeliPlan["ELocation"]);

                    }

                    entryData.Add("fyxm", ChargeProjectNumber);
                    entryData.Add("slbm", ReceiveDeptNumber);
                    entryData.Add("kzjhsl", DeliveryControl == null ? "1" : DeliveryControl.Equals("True") ? "0" : "1");
                    entryData.Add("jhsx", DeliveryMaxQty);
                    entryData.Add("jhxx", DeliveryMinQty);
                    entryData.Add("kzjhsj", TimeControl == null ? "1" : TimeControl.Equals("True") ? "0" : "1");
                    entryData.Add("jhtqts", DeliveryBeforeDays);
                    entryData.Add("jhycts", DeliveryDelayDays);
                    entryData.Add("zzjhrq", DeliveryEarlyDate);
                    entryData.Add("zzjhsj", FDeliveryEarlyTime);
                    entryData.Add("zwjhrq", DeliveryLastDate);
                    entryData.Add("zwjhsj", DeliveryLastTime);
                    entryData.Add("slyc", TaxRate);
                    entryData.Add("wwcplx", ProductType);
                    entryData.Add("wlbm", MaterialNumber);
                    entryData.Add("wlmc", MaterialName);
                    entryData.Add("ggxh", Specification);
                    entryData.Add("cgdw", unitNumber);
                    entryData.Add("cgsl", QtyX);
                    entryData.Add("jjdw", priceUnitNumber);
                    entryData.Add("jjsl", PriceUnitQty);
                    entryData.Add("jhrq", DeliveryDate);
                    entryData.Add("jhdz", ELocationAddress);
                    entryData.Add("dj", Price);
                    entryData.Add("hsdj", TaxPrice);
                    entryData.Add("zkl", DiscountRate);
                    entryData.Add("zke", Discount);
                    entryData.Add("se", TaxAmount);
                    entryData.Add("jshj", AllAmount);
                    entryData.Add("je", Amount);
                    entryData.Add("xqzz", RequireOrgNumber);
                    entryData.Add("xqbm", RequireDeptNumber);
                    entryData.Add("xqr", RequireStaffNumber);
                    entryData.Add("slzz", ReceiveOrgNumber);
                    entryData.Add("jszz", SettleOrgNumber);
                    entryData.Add("sfzp", GiveAway == null ? "1" : GiveAway.Equals("True") ? "0" : "1");
                    entryData.Add("bz", Note);
                    entryData.Add("kcdw", StockUnitNumber);
                    entryData.Add("kcsl", StockQty);
                    entryData.Add("xm", F_SRT_PROJECT1Number);
                    entryData.Add("jhdw", PlanUnitIDNumber);
                    entryData.Add("yjhsl", DeliCommitQty);
                    entryData.Add("syjhsl", DeliRemainQty);
                    entryData.Add("jhdd", ELocation);
                    entryData.Add("erpfkjhid", entryId);



                    entryItem.Add("data", entryData);
                    detail1.Add(entryItem);

                }
                dateItem.Add("detail1", detail1);

                JSONArray detail2 = new JSONArray();

                DynamicObjectCollection FIinstallment = o["FIinstallment"] as DynamicObjectCollection;
                foreach (DynamicObject IinstallmentItem in FIinstallment)
                {
                    JSONObject FIinstallmentItem = new JSONObject();
                    JSONObject FIinstallmentData = new JSONObject();
                    JSONObject operate = new JSONObject();
                    operate.Add("action", "SaveOrUpdate");
                    operate.Add("actionDescribe", "SaveOrUpdate");

                    FIinstallmentItem.Add("operate", operate);

                    string YFDATE = Convert.ToDateTime(IinstallmentItem["YFDATE"]).ToString("yyyy-MM-dd");
                    string YFRATIO = Convert.ToDecimal(IinstallmentItem["YFRATIO"]).ToString("#0.00");
                    string YFAMOUNT = Convert.ToDecimal(IinstallmentItem["YFAMOUNT"]).ToString("#0.00");
                    string ISPREPAYMENT = Convert.ToString(IinstallmentItem["ISPREPAYMENT"]);
                    string RelBillNo = Convert.ToString(IinstallmentItem["RelBillNo"]);
                    string InsPrepaidAmount = Convert.ToDecimal(IinstallmentItem["InsPrepaidAmount"]).ToString("#0.00");
                    DynamicObject F_SRT_ProjectNode1 = IinstallmentItem["F_SRT_ProjectNode1"] as DynamicObject;
                    string F_SRT_ProjectNode1Number = F_SRT_ProjectNode1 == null ? "" : Convert.ToString(F_SRT_ProjectNode1["Number"]);
                    string ACTUALAMOUNT = Convert.ToDecimal(IinstallmentItem["ACTUALAMOUNT"]).ToString("#0.00");
                    string PayJoinAmount = Convert.ToDecimal(IinstallmentItem["PayJoinAmount"]).ToString("#0.00");
                    string FRemarks = Convert.ToString(IinstallmentItem["FRemarks"]);
                    string F_SRT_SFFK1 = Convert.ToString(IinstallmentItem["F_SRT_SFFK1"]).Equals("True") ? "0" : "1";
                    string F_SRT_SFSH1 = Convert.ToString(IinstallmentItem["F_SRT_SFSH1"]);
                    string entryId = Convert.ToString(IinstallmentItem["Id"]);

                    FIinstallmentData.Add("yfrq", YFDATE);
                    FIinstallmentData.Add("yfbl", YFRATIO);
                    FIinstallmentData.Add("yfje", YFAMOUNT);
                    FIinstallmentData.Add("sfyf", ISPREPAYMENT.Equals("True") ? "0" : "1");
                    FIinstallmentData.Add("yfdh", RelBillNo);
                    FIinstallmentData.Add("dcyfed", InsPrepaidAmount);
                    FIinstallmentData.Add("xmjd", F_SRT_ProjectNode1Number);
                    FIinstallmentData.Add("sjyfje", ACTUALAMOUNT);
                    FIinstallmentData.Add("fkglje", PayJoinAmount);
                    FIinstallmentData.Add("bz", FRemarks);
                    FIinstallmentData.Add("sfsk", F_SRT_SFFK1.Equals("True") ? "0" : "1");
                    FIinstallmentData.Add("sfsh", F_SRT_SFSH1.Equals("True") ? "0" : "1");
                    FIinstallmentData.Add("erpfkjhid", entryId);

                    FIinstallmentItem.Add("data", FIinstallmentData);
                    detail2.Add(FIinstallmentItem);
                }
                dateItem.Add("detail2", detail2);


                operationinfo.Add("operationDate", DateTime.Now.ToString("yyyy-MM-dd"));
                operationinfo.Add("operator", "1");
                operationinfo.Add("operationTime", DateTime.Now.ToString("HH:mm:ss"));

                dateItem.Add("operationinfo", operationinfo);
                dateItem.Add("mainTable", mainTable);
                data.Add(dateItem);
                dataJson.Add("data", data);


                JSONObject header = new JSONObject();
                string datetime = DateTime.Now.ToString("yyyyMMddHHmmss");
                header.Add("systemid", "ERP");
                header.Add("currentDateTime", datetime);
                header.Add("Md5", Utils.StringToMD5Hash("ERPerp" + datetime));

                dataJson.Add("header", header);

                string results = Utils.PostUrl(Utils.pushCGDDurl, "datajson=" + dataJson.ToString());

                JSONObject resultJson = JSONObject.Parse(results);
                string retCode = Convert.ToString(resultJson["status"]);

                if (retCode.Equals("1"))
                {

                }
                else
                {
                    throw new KDException("", results);
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
        /// <summary>
        /// 申请类型
        /// </summary>
        /// <param name="erpId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 采购类型
        /// </summary>
        /// <param name="erpId"></param>
        /// <returns></returns>
        private string PurType(string erpId)
        {
            try
            {
                string queryOAidSql = string.Format(@"select F_PYEO_OAID8 from PYEO_t_Cust_Entry100035 where F_PYEO_ERPID8 = '{0}'", erpId);
                return DBUtils.ExecuteDynamicObject(this.Context, queryOAidSql)[0]["F_PYEO_OAID8"].ToString();
            }
            catch (Exception e)
            {
                return "6";
            }
        }

        /// <summary>
        /// 合同类型
        /// </summary>
        /// <param name="erpId"></param>
        /// <returns></returns>
        private string contractType(string erpId)
        {
            try
            {
                string queryOAidSql = string.Format(@"select F_PYEO_CONTRACTTYPEOA from PYEO_t_Cust_Entry100004 where  F_PYEO_CONTRACTTYPEErp='{0}'", erpId);
                return DBUtils.ExecuteDynamicObject(this.Context, queryOAidSql)[0]["F_PYEO_CONTRACTTYPEOA"].ToString();
            }
            catch (Exception e)
            {
                return "7";
            }
        }

        private void addQtyAndAmount(JSONObject mainTable,string entryid)
        {
            decimal amount = 0; 
            decimal qty = 0;
            //入库
            string sql = string.Format(@"select sum(FREALQTY) as qty,sum(ef.FALLAMOUNT) as amount ,e.FPOORDERNO,e.FPOORDERENTRYID as FPOORDERENTRYID from T_STK_INSTOCK t 
                                                            left join T_STK_INSTOCKENTRY e on t.FID = e.FID 
															left join T_STK_INSTOCKENTRY_F ef on e.FENTRYID = ef.FENTRYID
                                                            where e.FPOORDERENTRYID = '{0}'
                                                            group by e.FPOORDERNO,e.FPOORDERENTRYID", entryid);
            DynamicObjectCollection queryAmountResult = DBUtils.ExecuteDynamicObject(this.Context, sql);
            if (queryAmountResult.Count != 0)
            {
                amount = Convert.ToDecimal(queryAmountResult[0]["amount"]);
                qty = Convert.ToDecimal(queryAmountResult[0]["qty"]);
                mainTable.Add("glrkje", amount);
                mainTable.Add("glrksl", qty);
            }
            //退料
            sql = string.Format(@"select e.FPOORDERENTRYID as FPOORDERENTRYID,sum(FRMREALQTY) as qty,sum(ef.FALLAMOUNT) as amount,e.FORDERNO from T_PUR_MRB t 
                                                            left join T_PUR_MRBENTRY e on t.FID = e.FID 
															left join T_PUR_MRBENTRY_F ef on e.FENTRYID = ef.FENTRYID
                                                            where e.FPOORDERENTRYID = '{0}'
                                                            group by e.FORDERNO,e.FPOORDERENTRYID", entryid);
            queryAmountResult = DBUtils.ExecuteDynamicObject(this.Context, sql);
            if (queryAmountResult.Count != 0)
            {
                amount = Convert.ToDecimal(queryAmountResult[0]["amount"]);
                qty = Convert.ToDecimal(queryAmountResult[0]["qty"]);
                mainTable.Add("gltlsl", qty);
                mainTable.Add("gltlje", amount);
            }
            //应付
            sql = string.Format(@"select e.FORDERENTRYID,sum(e.FPriceQty) as qty,sum(e.FALLAMOUNTFOR) as amount ,e.FORDERNUMBER from T_AP_PAYABLE t 
                                                            left join T_AP_PAYABLEENTRY e on t.FID = e.FID 
                                                            where e.FORDERENTRYID = '{0}'
                                                            group by e.FORDERNUMBER,e.FORDERENTRYID", entryid);
            queryAmountResult = DBUtils.ExecuteDynamicObject(this.Context, sql);
            if (queryAmountResult.Count != 0)
            {
                amount = Convert.ToDecimal(queryAmountResult[0]["amount"]);
                qty = Convert.ToDecimal(queryAmountResult[0]["qty"]);
                mainTable.Add("glyfsl", qty);
                mainTable.Add("glyfje", amount);
            }
            //付款
            sql = string.Format(@"select FORDERENTRYID,sum(e.FREALPAYAMOUNT) as amount,e.FPURORDERNO from T_AP_PAYBILL t 
                                                            left join T_AP_PAYBILLSRCENTRY e on t.FID = e.FID 
                                                            where e.FORDERENTRYID = '{0}'
                                                            group by e.FPURORDERNO,FORDERENTRYID", entryid);
            queryAmountResult = DBUtils.ExecuteDynamicObject(this.Context, sql);
            if (queryAmountResult.Count != 0)
            {
                amount = Convert.ToDecimal(queryAmountResult[0]["amount"]);
                mainTable.Add("glfkje", amount);
            }
            sql = string.Format(@"select FORDERENTRYID,sum(e.FREALREFUNDAMOUNTFOR) as amount,e.FPURCHASEORDERNUMBER from T_AP_REFUNDBILL t 
                                                        left join T_AP_REFUNDBILLENTRY e on t.FID = e.FID 
                                                        where e.FORDERENTRYID = '{0}'
                                                        group by e.FPURCHASEORDERNUMBER,FORDERENTRYID", entryid);
            queryAmountResult = DBUtils.ExecuteDynamicObject(this.Context, sql);
            if (queryAmountResult.Count != 0)
            {
                amount = Convert.ToDecimal(queryAmountResult[0]["amount"]);
                mainTable.Add("glfktkje", amount);
            }

        }
    }
}
