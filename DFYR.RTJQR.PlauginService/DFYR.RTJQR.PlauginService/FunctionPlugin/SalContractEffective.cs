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
    [Kingdee.BOS.Util.HotUpdate, Description("销售合同变更单生效至OA")]
    public class SalContractEffective : AbstractOperationServicePlugIn
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
                FormMetadata meta = MetaDataServiceHelper.Load(this.Context, "CRM_Contract") as FormMetadata;
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
                DynamicObjectCollection POOrderEntrys = o["FCRMContractEntry"] as DynamicObjectCollection;
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

                DynamicObjectCollection FIinstallment = o["FContractPlan"] as DynamicObjectCollection;
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
                FormMetadata meta = MetaDataServiceHelper.Load(this.Context, "CRM_Contract") as FormMetadata;
                DynamicObject o = BusinessDataServiceHelper.LoadSingle(
                               this.Context,
                               PKIDX,
                               meta.BusinessInfo.GetDynamicObjectType());

                string billNo = Convert.ToString(o["BillNo"]);
                string name = Convert.ToString(o["FName"]);
                DynamicObject FSALEORGID = o["FSALEORGID"] as DynamicObject;
                string FSALEORGNUMBER = FSALEORGID == null ? "" : Convert.ToString(FSALEORGID["F_PYEO_Text_OAID"]);
                DynamicObject FCUSTID = o["FCUSTID"] as DynamicObject;
                string FCUSTNUMBER = FCUSTID == null ? "" : Convert.ToString(FCUSTID["Name"]);
                DynamicObject FBDCUSTID = o["FBDCUSTID"] as DynamicObject;
                string FBDCUSTNUMBER = FBDCUSTID == null ? "" : Convert.ToString(FBDCUSTID["Number"]);
                DynamicObject FSALEDEPTID = o["FSALEDEPTID"] as DynamicObject;
                string FSALEDEPTNUMBER = FSALEDEPTID == null ? "" : Convert.ToString(FSALEDEPTID["F_PYEO_Text_OAID"]);
                DynamicObject FContactId = o["FContactId"] as DynamicObject;
                string FContactNumber = FContactId == null ? "" : Convert.ToString(FContactId["Name"]);
                string FDate = Convert.ToDateTime(o["FDate"]).ToString("yyyy-MM-dd");
                DynamicObject FSALEGROUPID = o["FSALEGROUPID"] as DynamicObject;
                string FSALEGROUPNUMBER = FSALEGROUPID == null ? "" : Convert.ToString(FSALEGROUPID["Number"]);
                string FValiStartDate = Convert.ToDateTime(o["FValiStartDate"]).ToString("yyyy-MM-dd");
                string FValiEndDate = Convert.ToDateTime(o["FValiEndDate"]).ToString("yyyy-MM-dd");
                DynamicObject FSalerId = o["FSALERID"] as DynamicObject;
                string FSalerNumber = FSalerId == null ? "" : Convert.ToString(FSalerId["Number"]);
                string FCONTRVALIDITY = Convert.ToString(o["FCONTRVALIDITY"]);
                FContactNumber = FContactId == null ? "" : Convert.ToString(FContactId["Name"]);
                var field = this.BusinessInfo.GetField("FChangeStatus") as ComboField; ;
                var enumValue = o["ChangeStatus"];
                string ChangeStatus = field.GetEnumItemName(enumValue);
                string FHAVEENTRY = Convert.ToString(o["FHAVEENTRY"]).Equals("True") ? "是" : "否";
                field = this.BusinessInfo.GetField("FContractType") as ComboField; ;
                enumValue = o["ContractType"];
                string ContractType = field.GetEnumItemName(enumValue);
                string CloseReason = Convert.ToString(o["CloseReason"]);
                DynamicObject F_PYEO_Project = o["F_PYEO_Project"] as DynamicObject;
                string F_PYEO_ProjectNumber = F_PYEO_Project == null ? "" : Convert.ToString(F_PYEO_Project["Number"]);
                DynamicObject F_SRT_HTYWLX = o["F_SRT_HTYWLX"] as DynamicObject;
                string F_SRT_HTYWLXNumber = F_SRT_HTYWLX == null ? "" : Convert.ToString(F_SRT_HTYWLX["Number"]);

                string F_PYEO_Remarks_01 = Convert.ToString(o["F_PYEO_Remarks_01"]);
                string F_SRT_Reameks_03 = Convert.ToString(o["F_SRT_Reameks_03"]);
                string F_PYEO_Remarks_04 = Convert.ToString(o["F_PYEO_Remarks_04"]);
                string F_SRT_Reameks_04 = Convert.ToString(o["F_SRT_Reameks_04"]);
                string F_PYEO_Remarks_05 = Convert.ToString(o["F_PYEO_Remarks_05"]);
                string F_SRT_Reameks_05 = Convert.ToString(o["F_SRT_Reameks_05"]);
                string F_PYEO_Remarks_06 = Convert.ToString(o["F_PYEO_Remarks_06"]);
                string F_SRT_Reameks_06 = Convert.ToString(o["F_SRT_Reameks_06"]);
                string F_PYEO_Remarks_07 = Convert.ToString(o["F_PYEO_Remarks_07"]);
                string F_SRT_Reameks_07 = Convert.ToString(o["F_SRT_Reameks_07"]);
                string F_PYEO_Remarks_08 = Convert.ToString(o["F_PYEO_Remarks_08"]);
                string F_SRT_Reameks_08 = Convert.ToString(o["F_SRT_Reameks_08"]);
                string F_PYEO_Remarks_09 = Convert.ToString(o["F_PYEO_Remarks_09"]);
                string F_SRT_Reameks_09 = Convert.ToString(o["F_SRT_Reameks_09"]);
                string F_PYEO_Remarks_10 = Convert.ToString(o["F_PYEO_Remarks_10"]);
                string F_SRT_Reameks_10 = Convert.ToString(o["F_SRT_Reameks_10"]);
                string F_PYEO_Remarks_11 = Convert.ToString(o["F_PYEO_Remarks_11"]);
                string F_SRT_Reameks_11 = Convert.ToString(o["F_SRT_Reameks_11"]);
                string F_PYEO_Remarks_12 = Convert.ToString(o["F_PYEO_Remarks_12"]);
                string F_SRT_Reameks_12 = Convert.ToString(o["F_SRT_Reameks_12"]);
                string F_PYEO_Remarks_13 = Convert.ToString(o["F_PYEO_Remarks_13"]);
                string F_SRT_Reameks_13 = Convert.ToString(o["F_SRT_Reameks_13"]);
                string F_PYEO_Remarks_14 = Convert.ToString(o["F_PYEO_Remarks_14"]);
                string F_SRT_Reameks_14 = Convert.ToString(o["F_SRT_Reameks_14"]);
                DynamicObject F_SRT_HY = o["F_SRT_HY"] as DynamicObject;
                string F_SRT_HYName = F_SRT_HY == null ? "" : Convert.ToString(F_SRT_HY["FDataValue"]);

                DynamicObject F_SRT_XSLX = o["F_SRT_XSLX"] as DynamicObject;
                string F_SRT_XSLXName = F_SRT_XSLX == null ? "" : Convert.ToString(F_SRT_XSLX["Name"]);
                DynamicObject F_SRT_MYLX = o["F_SRT_MYLX"] as DynamicObject;
                string F_SRT_MYLXName = F_SRT_MYLX == null ? "" : Convert.ToString(F_SRT_MYLX["Name"]);
                DynamicObject F_SRT_DQ = o["F_SRT_DQ"] as DynamicObject;
                string F_SRT_DQName = F_SRT_DQ == null ? "" : Convert.ToString(F_SRT_DQ["FDataValue"]);
                DynamicObject F_SRT_TD = o["F_SRT_TD"] as DynamicObject;
                string F_SRT_TDName = F_SRT_TD == null ? "" : Convert.ToString(F_SRT_TD["Name"]);

                DynamicObject F_SRT_ZDKH = o["F_SRT_ZDKH"] as DynamicObject;
                string F_SRT_ZDKHNumber = F_SRT_ZDKH == null ? "" : Convert.ToString(F_SRT_ZDKH["Number"]);
                DynamicObject F_SRT_CPFL = o["F_SRT_CPFL"] as DynamicObject;
                string F_SRT_CPFLName = F_SRT_CPFL == null ? "" : Convert.ToString(F_SRT_CPFL["Name"]);
                DynamicObject F_SRT_XSQD = o["F_SRT_XSQD"] as DynamicObject;
                string F_SRT_XSQDName = F_SRT_XSQD == null ? "" : Convert.ToString(F_SRT_XSQD["Name"]);
                string MarginLevel = string.Empty;
                string FBILLALLAMOUNT = string.Empty;
                string FSETTLEMODEName = string.Empty;
                string FRECCONDITIONName = string.Empty;
                string FISINCLUDEDTAX = string.Empty;
                string IsPriceExcludeTax = string.Empty;
                string FPRICELISTName = string.Empty;
                string FDISCOUNTLISTIDTName = string.Empty;
                string FSETTLECURRIDName = string.Empty;
                string FBILLALLAMOUNT_LC = string.Empty;
                string Margin = string.Empty;
                string FLOCALCURRIDName = string.Empty;
                string FEXCHANGETYPEName = string.Empty;
                string FEXCHANGERATE = string.Empty;

                DynamicObjectCollection CRM_ContractFINs = o["CRM_ContractFIN"] as DynamicObjectCollection;
                foreach (DynamicObject CRM_ContractFIN in CRM_ContractFINs)
                {
                    MarginLevel = Convert.ToDecimal(CRM_ContractFIN["MarginLevel"]).ToString("#0.00");
                    FBILLALLAMOUNT = Convert.ToDecimal(CRM_ContractFIN["FBILLALLAMOUNT"]).ToString("#0.00");
                    DynamicObject FSETTLEMODEID = CRM_ContractFIN["FSETTLEMODEID"] as DynamicObject;
                    FSETTLEMODEName = FSETTLEMODEID == null ? "" : Convert.ToString(FSETTLEMODEID["Name"]);
                    DynamicObject FRECCONDITIONID = CRM_ContractFIN["FRECCONDITIONID"] as DynamicObject;
                    FRECCONDITIONName = FRECCONDITIONID == null ? "" : Convert.ToString(FRECCONDITIONID["Name"]);
                    FISINCLUDEDTAX = Convert.ToString(CRM_ContractFIN["FISINCLUDEDTAX"]).Equals("True") ? "是" : "否";
                    IsPriceExcludeTax = Convert.ToString(CRM_ContractFIN["IsPriceExcludeTax"]).Equals("True") ? "是" : "否";
                    DynamicObject FPRICELISTID = CRM_ContractFIN["FPRICELISTID"] as DynamicObject;
                    FPRICELISTName = FPRICELISTID == null ? "" : Convert.ToString(FPRICELISTID["Name"]);
                    DynamicObject FDISCOUNTLISTID = CRM_ContractFIN["FDISCOUNTLISTID"] as DynamicObject;
                    FDISCOUNTLISTIDTName = FDISCOUNTLISTID == null ? "" : Convert.ToString(FDISCOUNTLISTID["Name"]);
                    DynamicObject FSETTLECURRID = CRM_ContractFIN["FSETTLECURRID"] as DynamicObject;
                    FSETTLECURRIDName = FSETTLECURRID == null ? "" : Convert.ToString(FSETTLECURRID["Name"]);
                    FBILLALLAMOUNT_LC = Convert.ToDecimal(CRM_ContractFIN["FBILLALLAMOUNT_LC"]).ToString("#0.00");
                    Margin = Convert.ToDecimal(CRM_ContractFIN["Margin"]).ToString("#0.00");
                    DynamicObject FLOCALCURRID = CRM_ContractFIN["FLOCALCURRID"] as DynamicObject;
                    FLOCALCURRIDName = FLOCALCURRID == null ? "" : Convert.ToString(FLOCALCURRID["Name"]);
                    DynamicObject FEXCHANGETYPEID = CRM_ContractFIN["FEXCHANGETYPEID"] as DynamicObject;
                    FEXCHANGETYPEName = FEXCHANGETYPEID == null ? "" : Convert.ToString(FEXCHANGETYPEID["Name"]);
                    FEXCHANGERATE = Convert.ToDecimal(CRM_ContractFIN["FEXCHANGERATE"]).ToString("#0.00");

                }
                JSONObject pushjson = new JSONObject();
                JSONObject dataJson = new JSONObject();

                JSONArray data = new JSONArray();
                JSONObject dateItem = new JSONObject();
                JSONObject operationinfo = new JSONObject();
                JSONObject mainTable = new JSONObject();

                mainTable.Add("erpnumber", billNo);
                mainTable.Add("htmc", name);
                mainTable.Add("xszz", FSALEORGNUMBER);
                mainTable.Add("crmkh", FCUSTNUMBER);
                mainTable.Add("kh", FBDCUSTNUMBER);
                mainTable.Add("xsbm", FSALEDEPTNUMBER);
                mainTable.Add("lxrmc", FContactNumber);
                mainTable.Add("rq", FDate);
                mainTable.Add("xsz", FSALEGROUPNUMBER);
                mainTable.Add("yxqsrq", FValiStartDate);
                mainTable.Add("yxjzrq", FValiEndDate);
                mainTable.Add("xsy", FSalerNumber);
                mainTable.Add("htyxqt", FCONTRVALIDITY);
                mainTable.Add("lxrmcc", FContactNumber);
                mainTable.Add("bgbz", ChangeStatus);
                mainTable.Add("lrmx", FHAVEENTRY);
                mainTable.Add("htlx", ContractType);
                mainTable.Add("gbyy", CloseReason);
                mainTable.Add("xm", F_PYEO_ProjectNumber);
                mainTable.Add("htywlx", F_SRT_HTYWLXNumber);
                mainTable.Add("nr1", F_PYEO_Remarks_01);
                mainTable.Add("y", F_SRT_Reameks_03);
                mainTable.Add("nr2", F_PYEO_Remarks_04);
                mainTable.Add("e", F_SRT_Reameks_04);
                mainTable.Add("nr3", F_PYEO_Remarks_05);
                mainTable.Add("s", F_SRT_Reameks_05);
                mainTable.Add("nr4", F_PYEO_Remarks_06);
                mainTable.Add("w", F_SRT_Reameks_07);
                mainTable.Add("nr6", F_PYEO_Remarks_08);
                mainTable.Add("l", F_SRT_Reameks_08);
                mainTable.Add("nr7", F_PYEO_Remarks_09);
                mainTable.Add("q", F_SRT_Reameks_09);
                mainTable.Add("nr8", F_PYEO_Remarks_10);
                mainTable.Add("b", F_SRT_Reameks_10);
                mainTable.Add("nr9", F_PYEO_Remarks_11);
                mainTable.Add("j", F_SRT_Reameks_11);
                mainTable.Add("nr10", F_PYEO_Remarks_12);
                mainTable.Add("shi", F_SRT_Reameks_12);
                mainTable.Add("nr11", F_PYEO_Remarks_13);
                mainTable.Add("sy", F_SRT_Reameks_13);
                mainTable.Add("nr12", F_PYEO_Remarks_14);
                mainTable.Add("se", F_SRT_Reameks_14);
                mainTable.Add("xy", F_SRT_HYName);
                mainTable.Add("xslx", F_SRT_XSLXName);
                mainTable.Add("mylx", F_SRT_MYLXName);
                mainTable.Add("dq", F_SRT_DQName);
                mainTable.Add("td" ,F_SRT_TDName);
                mainTable.Add("zdkh", F_SRT_ZDKHNumber);
                mainTable.Add("cpfl", F_SRT_CPFLName);
                mainTable.Add("xsqd", F_SRT_XSQDName);
                mainTable.Add("bzjbl", MarginLevel);
                mainTable.Add("zje", FBILLALLAMOUNT);
                mainTable.Add("jsfs", FSETTLEMODEName);
                mainTable.Add("sktj", FRECCONDITIONName);
                mainTable.Add("jws", IsPriceExcludeTax);
                mainTable.Add("jmb", FPRICELISTName);
                mainTable.Add("zkb", FDISCOUNTLISTIDTName);
                mainTable.Add("jsbb", FSETTLECURRIDName);
                mainTable.Add("zjebwb", FBILLALLAMOUNT_LC);
                mainTable.Add("bzj", Margin);
                mainTable.Add("bwb", FLOCALCURRIDName);
                mainTable.Add("hllx", FEXCHANGETYPEName);
                mainTable.Add("hl", FEXCHANGERATE);

                JSONArray detail1 = new JSONArray();
                DynamicObjectCollection materials = o["FCRMContractEntry"] as DynamicObjectCollection;
                foreach (DynamicObject material in materials)
                {
                    JSONObject entryItem = new JSONObject();
                    JSONObject entryData = new JSONObject();
                    JSONObject operate = new JSONObject();
                    operate.Add("action", "SaveOrUpdate");
                    operate.Add("actionDescribe", "SaveOrUpdate");
                    entryItem.Add("operate", operate);

                    DynamicObject MaterialId = material["MaterialId"] as DynamicObject;
                    string MaterialNumber = MaterialId == null ? "" : Convert.ToString(MaterialId["Number"]);
                    string MaterialName = MaterialId == null ? "" : Convert.ToString(MaterialId["Name"]);
                    string Specification = MaterialId == null ? "" : Convert.ToString(MaterialId["Specification"]);
                    DynamicObject F_SRT_CHDL = material["F_SRT_CHDL"] as DynamicObject;
                    string F_SRT_CHDLName = F_SRT_CHDL == null ? "" : Convert.ToString(F_SRT_CHDL["Name"]);
                    DynamicObject BomId = material["BomId"] as DynamicObject;
                    string BomName = BomId == null ? "" : Convert.ToString(BomId["Name"]);
                    DynamicObject ParentMatId = material["ParentMatId"] as DynamicObject;
                    string ParentMatIdName = ParentMatId == null ? "" : Convert.ToString(ParentMatId["Name"]);
                    DynamicObject UnitId = material["UnitId"] as DynamicObject;
                    string UnitIdName = UnitId == null ? "" : Convert.ToString(UnitId["Name"]);
                    string Qty = Convert.ToDecimal(material["Qty"]).ToString("#0.00");
                    DynamicObject FPRICEUNITID = material["FPRICEUNITID"] as DynamicObject;
                    string FPRICEUNITIDName = FPRICEUNITID == null ? "" : Convert.ToString(FPRICEUNITID["Name"]);
                    string FPRICEUNITQTY = Convert.ToDecimal(material["FPRICEUNITQTY"]).ToString("#0.00");
                    string Price = Convert.ToDecimal(material["Price"]).ToString("#0.00");
                    string TaxPrice = Convert.ToDecimal(material["TaxPrice"]).ToString("#0.00");
                    string FTAXRATE = Convert.ToDecimal(material["FTAXRATE"]).ToString("#0.00");
                    string FAMOUNT = Convert.ToDecimal(material["FAMOUNT"]).ToString("#0.00");
                    string FTAXAMOUNT = Convert.ToDecimal(material["FTAXAMOUNT"]).ToString("#0.00");
                    string AllAmount = Convert.ToDecimal(material["AllAmount"]).ToString("#0.00");
                    string FDELIVERYDATE = Convert.ToDateTime(material["FDELIVERYDATE"]).ToString("yyyy-MM-dd");
                    DynamicObject FSETTLEORGID = material["FSETTLEORGID"] as DynamicObject;
                    string FSETTLEORGNUMBER = FSETTLEORGID == null ? "" : Convert.ToString(FSETTLEORGID["F_PYEO_Text_OAID"]);
                    string Note = Convert.ToString(material["Note"]);
                    string FISFREE = Convert.ToString(material["FISFREE"]).Equals("True") ? "0" : "1";
                    DynamicObject FSTOCKUNITID = material["FSTOCKUNITID"] as DynamicObject;
                    string FSTOCKUNITIDNAME = FSTOCKUNITID == null ? "" : Convert.ToString(FSTOCKUNITID["Number"]);
                    string FSTOCKQTY = Convert.ToDecimal(material["FSTOCKQTY"]).ToString("#0.00");

                    entryData.Add("wlbm", MaterialNumber);
                    entryData.Add("wlmc", MaterialName);
                    entryData.Add("ggxh", Specification);
                    entryData.Add("chdl", F_SRT_CHDLName);
                    entryData.Add("bombb", BomName);
                    entryData.Add("fxcp", ParentMatIdName);
                    entryData.Add("xsdw", UnitIdName);
                    entryData.Add("xssl", Qty);
                    entryData.Add("jjdw", FPRICEUNITIDName);
                    entryData.Add("jjsl", FPRICEUNITQTY);
                    entryData.Add("dj", Price);
                    entryData.Add("hsdj", TaxPrice);
                    entryData.Add("hl", FTAXRATE);
                    entryData.Add("je", FAMOUNT);
                    entryData.Add("se", FTAXAMOUNT);
                    entryData.Add("jshj", AllAmount);
                    entryData.Add("jszz", FSETTLEORGNUMBER);
                    entryData.Add("bz", Note);
                    entryData.Add("sfzp", FISFREE);
                    entryData.Add("kcdw", FSTOCKUNITIDNAME);
                    entryData.Add("kcsl", FSTOCKQTY);

                    entryItem.Add("data", entryData);
                    detail1.Add(entryItem);
                }
                dateItem.Add("detail1", detail1);

                JSONArray detail2 = new JSONArray();
                DynamicObjectCollection FIinstallment = o["FContractPlan"] as DynamicObjectCollection;
                foreach (DynamicObject IinstallmentItem in FIinstallment)
                {
                    JSONObject FIinstallmentItem = new JSONObject();
                    JSONObject FIinstallmentData = new JSONObject();
                    JSONObject operate = new JSONObject();
                    operate.Add("action", "SaveOrUpdate");
                    operate.Add("actionDescribe", "SaveOrUpdate");

                    FIinstallmentItem.Add("operate", operate);

                    string NeedRecAdvance = Convert.ToString(IinstallmentItem["NeedRecAdvance"]).Equals("True") ? "0" : "1";
                    string FRECADVANCERATE = Convert.ToDecimal(IinstallmentItem["FRECADVANCERATE"]).ToString("#0.00");
                    string FRECADVANCEAMOUNT = Convert.ToDecimal(IinstallmentItem["FRECADVANCEAMOUNT"]).ToString("#0.00");
                    string FMUSTDATE = Convert.ToDateTime(IinstallmentItem["FMUSTDATE"]).ToString("yyyy-MM-dd");
                    string FRECJOINAMOUNT = Convert.ToDecimal(IinstallmentItem["FRECJOINAMOUNT"]).ToString("#0.00");
                    DynamicObject F_SRT_ProjectNode = IinstallmentItem["F_SRT_ProjectNode"] as DynamicObject;
                    string F_SRT_ProjectNodeNumber = F_SRT_ProjectNode == null ? "" : Convert.ToString(F_SRT_ProjectNode["Number"]);
                    string F_SRT_ProjectNodeName = F_SRT_ProjectNode == null ? "" : Convert.ToString(F_SRT_ProjectNode["Name"]);
                    string F_SRT_CheckBox = Convert.ToString(IinstallmentItem["F_SRT_CheckBox"]).Equals("True") ? "0" : "1";
                    string F_SRT_NodeCheckBox = F_SRT_ProjectNode == null ? "" : Convert.ToString(F_SRT_ProjectNode["F_SRT_NodeCheckBox"]).Equals("True") ? "0" : "1";
                    string erpId = Convert.ToString(IinstallmentItem["Id"]);

                    FIinstallmentData.Add("sfys", NeedRecAdvance);
                    FIinstallmentData.Add("ysbl", FRECADVANCERATE);
                    FIinstallmentData.Add("ysje", FRECADVANCEAMOUNT);
                    FIinstallmentData.Add("dqr", FMUSTDATE);
                    FIinstallmentData.Add("glskje", FRECJOINAMOUNT);
                    FIinstallmentData.Add("xmjdbm", F_SRT_ProjectNodeNumber);
                    FIinstallmentData.Add("xmjdmc", F_SRT_ProjectNodeName);
                    FIinstallmentData.Add("xmsfxyglqr", F_SRT_NodeCheckBox);
                    FIinstallmentData.Add("xmsfglqr", F_SRT_CheckBox);
                    FIinstallmentData.Add("skjhid", erpId);

                    FIinstallmentItem.Add("data", FIinstallmentData);
                    detail2.Add(FIinstallmentItem);
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
        }
    }
}
