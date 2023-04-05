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
    [Kingdee.BOS.Util.HotUpdate, Description("销售退货通知单推送")]
    public class ReturnNoticePush : AbstractOperationServicePlugIn
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

                string SettleCurrNumber = string.Empty;
                string SettleOrgNumber = string.Empty;
                string ChageConditionNumber = string.Empty;
                string SettleTypeIdNumber = string.Empty;
                string LocalCurrIdName = string.Empty;
                string ExchangeTypeIdName = string.Empty;
                string ExchangeRate = string.Empty;
                DynamicObject BillTypeID = o["BillTypeID"] as DynamicObject;
                string BillTypeNumber = BillTypeID==null?"":Convert.ToString(BillTypeID["Name"]);
                string BillNo = Convert.ToString(o["BillNo"]);
                string Date = Convert.ToDateTime(o["Date"]).ToString("yyyy-MM-dd");
                DynamicObject SaleOrgId = o["SaleOrgId"] as DynamicObject;
                string SaleOrgIdNumber = SaleOrgId == null ? "" : Convert.ToString(SaleOrgId["F_PYEO_Text_OAID"]);
                DynamicObject RetcustId = o["RetcustId"] as DynamicObject;
                string RetcustIdNumber = RetcustId == null ? "" : Convert.ToString(RetcustId["Name"]);
                DynamicObject Sledeptid = o["Sledeptid"] as DynamicObject;
                string SledeptidNumber = Sledeptid == null ? "" : Convert.ToString(Sledeptid["F_PYEO_Text_OAID"]);
                DynamicObject ReturnReason = o["ReturnReason"] as DynamicObject;
                string ReturnReasonNumber = ReturnReason == null ? "" : Convert.ToString(ReturnReason["FDataValue"]);
                DynamicObject HeadLocId = o["HeadLocId"] as DynamicObject;
                string HeadLocIdNumber = HeadLocId == null ? "" : Convert.ToString(HeadLocId["Name"]);
                DynamicObject CorrespondOrgId = o["CorrespondOrgId"] as DynamicObject;
                string CorrespondOrgIdNumber = CorrespondOrgId == null ? "" : Convert.ToString(CorrespondOrgId["F_PYEO_Text_OAID"]);
                DynamicObject SaleGroupId = o["SaleGroupId"] as DynamicObject;
                string SaleGroupIdNumber = SaleGroupId == null ? "" : Convert.ToString(SaleGroupId["Number"]);
                DynamicObject SalesManId = o["SalesManId"] as DynamicObject;
                string SalesManIdNumber = SalesManId == null ? "" : Convert.ToString(SalesManId["EmpNumber"]);
                DynamicObject RetorgId = o["RetorgId"] as DynamicObject;
                string RetorgIdNumber = RetorgId == null ? "" : Convert.ToString(RetorgId["F_PYEO_Text_OAID"]);
                DynamicObject RetDeptId = o["RetDeptId"] as DynamicObject;
                string RetDeptIdNumber = RetDeptId == null ? "" : Convert.ToString(RetDeptId["F_PYEO_Text_OAID"]);
                DynamicObject StockerGroupId = o["StockerGroupId"] as DynamicObject;
                string StockerGroupIdNumber = StockerGroupId == null ? "" : Convert.ToString(StockerGroupId["Number"]);
                DynamicObject StockerId = o["StockerId"] as DynamicObject;
                string StockerIdNumber = StockerId == null ? "" : Convert.ToString(StockerId["Number"]);
                StockerIdNumber = Utils.getPersonOAid(this.Context, StockerIdNumber);
                string Description = Convert.ToString(o["Description"]);
                DynamicObject ReceiveCusId = o["ReceiveCusId"] as DynamicObject;
                string ReceiveCusIdNumber = ReceiveCusId == null ? "" : Convert.ToString(ReceiveCusId["Name"]);
                string ReceiveAddress = Convert.ToString(o["ReceiveAddress"]);
                DynamicObject SettleCusId = o["SettleCusId"] as DynamicObject;
                string SettleCusIdNumber = SettleCusId == null ? "" : Convert.ToString(SettleCusId["Name"]);
                DynamicObject ReceiveCusContact = o["ReceiveCusContact"] as DynamicObject;
                string ReceiveCusContactNumber = ReceiveCusContact == null ? "" : Convert.ToString(ReceiveCusContact["Name"]);
                DynamicObject PayCusId = o["PayCusId"] as DynamicObject;
                string PayCusIdNumber = PayCusId == null ? "" : Convert.ToString(PayCusId["Name"]);

                var field = this.BusinessInfo.GetField("FOwnerTypeIdHead") as ComboField; ;
                var enumValue = o["OwnerTypeIdHead"];
                var OwnerTypeIdHead = field.GetEnumItemName(enumValue);

                DynamicObject OwnerIdHead = o["OwnerIdHead"] as DynamicObject;
                string OwnerIdHeadNumber = OwnerIdHead == null ? "" : Convert.ToString(OwnerIdHead["Name"]);
                string FLinkMan = Convert.ToString(o["FLinkMan"]);
                string FLinkPhone = Convert.ToString(o["FLinkPhone"]);
                DynamicObject F_SRT_Project = o["F_SRT_Project"] as DynamicObject;
                string F_SRT_ProjectNumber = F_SRT_Project == null ? "" : Convert.ToString(F_SRT_Project["Name"]);
                DynamicObject F_SRT_HTH = o["F_SRT_HTH"] as DynamicObject;
                string F_SRT_HTHNumber = F_SRT_HTH == null ? "" : Convert.ToString(F_SRT_HTH["Name"]);
                DynamicObject F_SRT_XSLX = o["F_SRT_XSLX"] as DynamicObject;
                string F_SRT_XSLXNumber = F_SRT_XSLX == null ? "" : Convert.ToString(F_SRT_XSLX["Name"]);
                DynamicObject F_SRT_HY = o["F_SRT_HY"] as DynamicObject;
                string F_SRT_HYNumber = F_SRT_HY == null ? "" : Convert.ToString(F_SRT_HY["FDataValue"]);
                DynamicObject F_SRT_DQ = o["F_SRT_DQ"] as DynamicObject;
                string F_SRT_DQNumber = F_SRT_DQ == null ? "" : Convert.ToString(F_SRT_DQ["FDataValue"]);
                DynamicObject F_SRT_HTYWLX = o["F_SRT_HTYWLX"] as DynamicObject;
                string F_SRT_HTYWLXNumber = F_SRT_HTH == null ? "" : Convert.ToString(F_SRT_HTYWLX["Name"]);
                DynamicObject F_SRT_MYLX = o["F_SRT_MYLX"] as DynamicObject;
                string F_SRT_MYLXNumber = F_SRT_MYLX == null ? "" : Convert.ToString(F_SRT_MYLX["Name"]);
                DynamicObject F_SRT_TD = o["F_SRT_TD"] as DynamicObject;
                string F_SRT_TDNumber = F_SRT_TD == null ? "" : Convert.ToString(F_SRT_TD["Name"]);
                DynamicObject F_SRT_ZDKH = o["F_SRT_ZDKH"] as DynamicObject;
                string F_SRT_ZDKHNumber = F_SRT_ZDKH == null ? "" : Convert.ToString(F_SRT_ZDKH["Name"]);
                DynamicObject F_SRT_CPFL = o["F_SRT_CPFL"] as DynamicObject;
                string F_SRT_CPFLNumber = F_SRT_CPFL == null ? "" : Convert.ToString(F_SRT_CPFL["Name"]);
                DynamicObject F_SRT_XSQD = o["F_SRT_XSQD"] as DynamicObject;
                string F_SRT_XSQDNumber = F_SRT_XSQD == null ? "" : Convert.ToString(F_SRT_XSQD["Name"]);
                
                
                DynamicObjectCollection RETURNNOTICEFIN = o["SAL_RETURNNOTICEFIN"] as DynamicObjectCollection;
                foreach (DynamicObject finEntity in RETURNNOTICEFIN)
                {
                    DynamicObject SettleCurrId = finEntity["SettleCurrId"] as DynamicObject;
                    SettleCurrNumber = SettleCurrId == null?"":Convert.ToString(SettleCurrId["Number"]);
                    DynamicObject SettleOrgId = finEntity["SettleOrgId"] as DynamicObject;
                    SettleOrgNumber = SettleOrgId == null ? "" : Convert.ToString(SettleOrgId["F_PYEO_Text_OAID"]);
                    DynamicObject ChageCondition = finEntity["ChageCondition"] as DynamicObject;
                    ChageConditionNumber = ChageCondition == null ? "" : Convert.ToString(ChageCondition["Name"]);
                    DynamicObject SettleTypeId = finEntity["SettleTypeId"] as DynamicObject;
                    SettleTypeIdNumber = SettleTypeId == null ? "" : Convert.ToString(SettleTypeId["Name"]);
                    DynamicObject LocalCurrId = finEntity["LocalCurrId"] as DynamicObject;
                    LocalCurrIdName = LocalCurrId == null ? "" : Convert.ToString(LocalCurrId["Name"]);
                    DynamicObject ExchangeTypeId = finEntity["ExchangeTypeId"] as DynamicObject;
                    ExchangeTypeIdName = ExchangeTypeId == null ? "" : Convert.ToString(ExchangeTypeId["Name"]);
                    ExchangeRate = Convert.ToDecimal(finEntity["ExchangeRate"]).ToString("#0.00");


                }

                JSONArray mainRoot = new JSONArray();
                JSONObject mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "jsbb");
                mainRootItem.Add("fieldValue", SettleCurrNumber);
                mainRoot.Add(mainRootItem);


                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "jszz");
                mainRootItem.Add("fieldValue", SettleOrgNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "sktj");
                mainRootItem.Add("fieldValue", ChageConditionNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "jsfs");
                mainRootItem.Add("fieldValue", SettleTypeIdNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "bwb");
                mainRootItem.Add("fieldValue", LocalCurrIdName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "hllx");
                mainRootItem.Add("fieldValue", ExchangeTypeIdName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "hl");
                mainRootItem.Add("fieldValue", ExchangeRate);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "djlx");
                mainRootItem.Add("fieldValue", BillTypeNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "djbh");
                mainRootItem.Add("fieldValue", BillNo);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "erpnumber");
                mainRootItem.Add("fieldValue", BillNo);
                mainRoot.Add(mainRootItem);
                

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "rq");
                mainRootItem.Add("fieldValue", Date);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xszz");
                mainRootItem.Add("fieldValue", SaleOrgIdNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "thkh");
                mainRootItem.Add("fieldValue", RetcustIdNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xsbm");
                mainRootItem.Add("fieldValue", SledeptidNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "thyy");
                mainRootItem.Add("fieldValue", ReturnReasonNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "jhdd");
                mainRootItem.Add("fieldValue", HeadLocIdNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "dyzz");
                mainRootItem.Add("fieldValue", CorrespondOrgIdNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xsz");
                mainRootItem.Add("fieldValue", SaleGroupIdNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xsy");
                mainRootItem.Add("fieldValue", SalesManIdNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "kczz");
                mainRootItem.Add("fieldValue", RetorgIdNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "kcbm");
                mainRootItem.Add("fieldValue", RetDeptIdNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "kcz");
                mainRootItem.Add("fieldValue", StockerGroupIdNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "cgy");
                mainRootItem.Add("fieldValue", StockerIdNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "bz");
                mainRootItem.Add("fieldValue", Description);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "shf");
                mainRootItem.Add("fieldValue", ReceiveCusIdNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "shfdz");
                mainRootItem.Add("fieldValue", ReceiveAddress);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "jsf");
                mainRootItem.Add("fieldValue", SettleCusIdNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "shflxr");
                mainRootItem.Add("fieldValue", ReceiveCusContactNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "fkf");
                mainRootItem.Add("fieldValue", PayCusIdNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "hzlx");
                mainRootItem.Add("fieldValue", OwnerTypeIdHead);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "hz");
                mainRootItem.Add("fieldValue", OwnerIdHeadNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "shrxm");
                mainRootItem.Add("fieldValue", FLinkMan);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "lxdh");
                mainRootItem.Add("fieldValue", FLinkPhone);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xm");
                mainRootItem.Add("fieldValue", F_SRT_ProjectNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "ht");
                mainRootItem.Add("fieldValue", F_SRT_HTHNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xslx");
                mainRootItem.Add("fieldValue", F_SRT_XSLXNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xy");
                mainRootItem.Add("fieldValue", F_SRT_HYNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "dq");
                mainRootItem.Add("fieldValue", F_SRT_DQNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "htywlx");
                mainRootItem.Add("fieldValue", F_SRT_HTYWLXNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "mylx");
                mainRootItem.Add("fieldValue", F_SRT_MYLXNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "td");
                mainRootItem.Add("fieldValue", F_SRT_TDNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "zdkh");
                mainRootItem.Add("fieldValue", F_SRT_ZDKHNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "cpfl");
                mainRootItem.Add("fieldValue", F_SRT_CPFLNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xsqd");
                mainRootItem.Add("fieldValue", F_SRT_XSQDNumber);
                mainRoot.Add(mainRootItem);

                JSONArray detailData = new JSONArray();

                JSONObject detailDataItem = new JSONObject();
                detailDataItem.Add("tableDBName", "formtable_main_60_dt1");
                JSONArray workflowRequestTableRecords = new JSONArray();

                DynamicObjectCollection Entrys = o["SAL_RETURNNOTICEENTRY"] as DynamicObjectCollection;
                foreach (DynamicObject entry in Entrys)
                {
                    field = this.BusinessInfo.GetField("FRowType") as ComboField; ;
                    enumValue = entry["RowType"];
                    string RowType = field.GetEnumItemName(enumValue);

                    DynamicObject MaterialId = entry["MaterialId"] as DynamicObject;
                    string MaterialNumber = MaterialId == null ? "" : Convert.ToString(MaterialId["Number"]);
                    string MaterialName = MaterialId == null ? "" : Convert.ToString(MaterialId["Name"]);
                    DynamicObject F_SRT_CHDL = entry["F_SRT_CHDL"] as DynamicObject;
                    string F_SRT_CHDLNumber = F_SRT_CHDL == null ? "" : Convert.ToString(F_SRT_CHDL["Name"]);
                    DynamicObject UnitID = entry["UnitID"] as DynamicObject;
                    string UnitIDNumber = UnitID == null ? "" : Convert.ToString(UnitID["Name"]);
                    string Qty = Convert.ToDecimal(entry["Qty"]).ToString("#0.00");
                    string ExpiryDate = Convert.ToDateTime(entry["ExpiryDate"]).ToString("yyyy-MM-dd");
                    DynamicObject Lot = entry["Lot"] as DynamicObject;
                    string LotNumber = Lot == null ? "" : Convert.ToString(Lot["Number"]);
                    string PriceBaseQty = Convert.ToDecimal(entry["PriceBaseQty"]).ToString("#0.00");
                    string Deliverydate = Convert.ToDateTime(entry["Deliverydate"]).ToString("yyyy-MM-dd");
                    DynamicObject StockId = entry["StockId"] as DynamicObject;
                    string StockIdNumber = StockId == null ? "" : Convert.ToString(StockId["Name"]);
                    DynamicObject BOMId = entry["BOMId"] as DynamicObject;
                    string BOMIdNumber = BOMId == null ? "" : Convert.ToString(BOMId["Name"]);
                    string entryDescription = Convert.ToString(entry["Description"]);

                    field = this.BusinessInfo.GetField("FOwnerTypeId") as ComboField; ;
                    enumValue = entry["OwnerTypeId"];
                    string entryOwnerTypeId = field.GetEnumItemName(enumValue);

                    DynamicObject OwnerId = entry["OwnerId"] as DynamicObject;
                    string OwnerName = OwnerId==null?"":Convert.ToString(OwnerId["Name"]);
                    
                    DynamicObject rmTypeId = entry["RmType"] as DynamicObject;
                    string RmType = rmTypeId==null?"": Convert.ToString(rmTypeId["FDataValue"]);
                    DynamicObject StockUnitID = entry["StockUnitID"] as DynamicObject;
                    string StockUnitIDNumber = StockUnitID == null ? "" : Convert.ToString(StockUnitID["Name"]);
                    string StockQty = Convert.ToDecimal(entry["StockQty"]).ToString("#0.00");
                    string RefuseFlag = Convert.ToString(entry["RefuseFlag"]).Equals("True") ? "是" : "否";
                    DynamicObject SNUnitID = entry["SNUnitID"] as DynamicObject;
                    string SNUnitIDNumber = SNUnitID == null ? "" : Convert.ToString(SNUnitID["Name"]);
                    string SNQty = Convert.ToDecimal(entry["SNQty"]).ToString("#0.00");



                    JSONObject workflowRequestTableRecordsItem = new JSONObject();
                    workflowRequestTableRecordsItem.Add("recordOrder", "0");
                    JSONArray workflowRequestTableFields = new JSONArray();


                    JSONObject workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "cplx");
                    workflowRequestTableFieldsItem.Add("fieldValue", RowType);
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
                    workflowRequestTableFieldsItem.Add("fieldName", "chdl");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_SRT_CHDLNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xsdw");
                    workflowRequestTableFieldsItem.Add("fieldValue", UnitIDNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xssl");
                    workflowRequestTableFieldsItem.Add("fieldValue", Qty);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "yxqz");
                    workflowRequestTableFieldsItem.Add("fieldValue", ExpiryDate);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "ph");
                    workflowRequestTableFieldsItem.Add("fieldValue", LotNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jjjbsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", PriceBaseQty);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "thrq");
                    workflowRequestTableFieldsItem.Add("fieldValue", Deliverydate);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "ck");
                    workflowRequestTableFieldsItem.Add("fieldValue", StockIdNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "bombb");
                    workflowRequestTableFieldsItem.Add("fieldValue", BOMIdNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "bz");
                    workflowRequestTableFieldsItem.Add("fieldValue", entryDescription);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "thlx");
                    workflowRequestTableFieldsItem.Add("fieldValue", RmType);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "kcdw");
                    workflowRequestTableFieldsItem.Add("fieldValue", StockUnitIDNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "kcsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", StockQty);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jsbz");
                    workflowRequestTableFieldsItem.Add("fieldValue", RefuseFlag);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xlhdw");
                    workflowRequestTableFieldsItem.Add("fieldValue", SNUnitIDNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xlhdwsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", SNQty);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "hzlx");
                    workflowRequestTableFieldsItem.Add("fieldValue", entryOwnerTypeId);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "hz");
                    workflowRequestTableFieldsItem.Add("fieldValue", OwnerName);
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
                                     + "&requestName=退货通知单单已到达"
                                     + "&workflowId=19&detailData=" + detailData.ToString(),personId);

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
