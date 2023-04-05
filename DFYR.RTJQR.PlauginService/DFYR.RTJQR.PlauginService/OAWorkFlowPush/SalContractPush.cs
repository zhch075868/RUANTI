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
    [Kingdee.BOS.Util.HotUpdate, Description("销售合同推送至OA")]
    public class SalContractPush : AbstractOperationServicePlugIn
    {
        public override void OnPreparePropertys(PreparePropertysEventArgs e)
        {
            //e.FieldKeys.Add("");
            e.FieldKeys.Add("F_PYEO_Remarks_05");
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
                string documentStatus = Convert.ToString(o["FDocumentStatus"]);

                //提交校验
                if (!documentStatus.Equals("B"))
                {
                    return;
                }
                string F_SRT_CheckContract = Convert.ToString(o["F_SRT_CheckContract"]).Equals("True") ? "0" : "1";
                //if (F_SRT_CheckContract.Equals("1"))
                //{
                //    return;
                //}
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
                //FContactNumber = FContactId == null ? "" : Convert.ToString(FContactId["Name"]);
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
                
                //DynamicObject F_SRT_ZDKH = o["F_SRT_ZDKH"] as DynamicObject;
                //string F_SRT_ZDKHNumber = F_SRT_ZDKH == null ? "" : Convert.ToString(F_SRT_ZDKH["Number"]);
                string F_SRT_ZDKHNumber = Convert.ToString(o["F_PYEO_Text3"]);
                
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
                mainRootItem.Add("fieldName", "djbh");
                mainRootItem.Add("fieldValue", billNo);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "erpnumber");
                mainRootItem.Add("fieldValue", billNo);
                mainRoot.Add(mainRootItem);

                if (attchemnt != null)
                {
                    mainRootItem = new JSONObject();
                    mainRootItem.Add("fieldName", "scfj");
                    mainRootItem.Add("fieldValue", attchemnt.ToString());
                    mainRoot.Add(mainRootItem);
                }

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "htmc");
                mainRootItem.Add("fieldValue", name);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xszz");
                mainRootItem.Add("fieldValue", FSALEORGNUMBER);
                mainRoot.Add(mainRootItem);


                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "crmkh");
                mainRootItem.Add("fieldValue", FCUSTNUMBER);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "kh");
                mainRootItem.Add("fieldValue", FBDCUSTNUMBER);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xsbm");
                mainRootItem.Add("fieldValue", FSALEDEPTNUMBER);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "lxrmc");
                mainRootItem.Add("fieldValue", FContactNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "rq");
                mainRootItem.Add("fieldValue", FDate);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xsz");
                mainRootItem.Add("fieldValue", FSALEGROUPNUMBER);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "yxqsrq");
                mainRootItem.Add("fieldValue", FValiStartDate);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "yxjzrq");
                mainRootItem.Add("fieldValue", FValiEndDate);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xsy");
                mainRootItem.Add("fieldValue", FSalerNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "htyxqt");
                mainRootItem.Add("fieldValue", FCONTRVALIDITY);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "lxrmcc");
                mainRootItem.Add("fieldValue", FContactNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "bgbz");
                mainRootItem.Add("fieldValue", ChangeStatus);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "lrmx");
                mainRootItem.Add("fieldValue", FHAVEENTRY);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "htlx");
                mainRootItem.Add("fieldValue", ContractType);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "gbyy");
                mainRootItem.Add("fieldValue", CloseReason);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xm");
                mainRootItem.Add("fieldValue", F_PYEO_ProjectNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "htywlx");
                mainRootItem.Add("fieldValue", F_SRT_HTYWLXNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "sfsydfmb");
                mainRootItem.Add("fieldValue", F_SRT_CheckContract);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "nr1");
                mainRootItem.Add("fieldValue", F_PYEO_Remarks_01);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "y");
                mainRootItem.Add("fieldValue", F_SRT_Reameks_03);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "nr2");
                mainRootItem.Add("fieldValue", F_PYEO_Remarks_04);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "e");
                mainRootItem.Add("fieldValue", F_SRT_Reameks_04);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "nr3");
                mainRootItem.Add("fieldValue", F_PYEO_Remarks_05);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "s");
                mainRootItem.Add("fieldValue", F_SRT_Reameks_05);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "nr4");
                mainRootItem.Add("fieldValue", F_PYEO_Remarks_06);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "si");
                mainRootItem.Add("fieldValue", F_SRT_Reameks_06);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "nr5");
                mainRootItem.Add("fieldValue", F_PYEO_Remarks_07);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "w");
                mainRootItem.Add("fieldValue", F_SRT_Reameks_07);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "nr6");
                mainRootItem.Add("fieldValue", F_PYEO_Remarks_08);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "l");
                mainRootItem.Add("fieldValue", F_SRT_Reameks_08);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "nr7");
                mainRootItem.Add("fieldValue", F_PYEO_Remarks_09);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "q");
                mainRootItem.Add("fieldValue", F_SRT_Reameks_09);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "nr8");
                mainRootItem.Add("fieldValue", F_PYEO_Remarks_10);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "b");
                mainRootItem.Add("fieldValue", F_SRT_Reameks_10);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "nr9");
                mainRootItem.Add("fieldValue", F_PYEO_Remarks_11);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "j");
                mainRootItem.Add("fieldValue", F_SRT_Reameks_11);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "nr10");
                mainRootItem.Add("fieldValue", F_PYEO_Remarks_12);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "shi");
                mainRootItem.Add("fieldValue", F_SRT_Reameks_12);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "nr11");
                mainRootItem.Add("fieldValue", F_PYEO_Remarks_13);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "sy");
                mainRootItem.Add("fieldValue", F_SRT_Reameks_13);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "nr12");
                mainRootItem.Add("fieldValue", F_PYEO_Remarks_14);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "se");
                mainRootItem.Add("fieldValue", F_SRT_Reameks_14);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xy");
                mainRootItem.Add("fieldValue", F_SRT_HYName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xslx");
                mainRootItem.Add("fieldValue", F_SRT_XSLXName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "mylx");
                mainRootItem.Add("fieldValue", F_SRT_MYLXName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "dq");
                mainRootItem.Add("fieldValue", F_SRT_DQName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "td");
                mainRootItem.Add("fieldValue", F_SRT_TDName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "zdkh");
                mainRootItem.Add("fieldValue", F_SRT_ZDKHNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "cpfl");
                mainRootItem.Add("fieldValue", F_SRT_CPFLName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xsqd");
                mainRootItem.Add("fieldValue", F_SRT_XSQDName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "bzjbl");
                mainRootItem.Add("fieldValue", MarginLevel);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "zje");
                mainRootItem.Add("fieldValue", FBILLALLAMOUNT);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "jsfs");
                mainRootItem.Add("fieldValue", FSETTLEMODEName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "sktj");
                mainRootItem.Add("fieldValue", FRECCONDITIONName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "sfhs");
                mainRootItem.Add("fieldValue", FISINCLUDEDTAX);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "jws");
                mainRootItem.Add("fieldValue", IsPriceExcludeTax);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "jmb");
                mainRootItem.Add("fieldValue", FPRICELISTName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "zkb");
                mainRootItem.Add("fieldValue", FDISCOUNTLISTIDTName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "jsbb");
                mainRootItem.Add("fieldValue", FSETTLECURRIDName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "zjebwb");
                mainRootItem.Add("fieldValue", FBILLALLAMOUNT_LC);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "bzj");
                mainRootItem.Add("fieldValue", Margin);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "bwb");
                mainRootItem.Add("fieldValue", FLOCALCURRIDName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "hllx");
                mainRootItem.Add("fieldValue", FEXCHANGETYPEName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "hl");
                mainRootItem.Add("fieldValue", FEXCHANGERATE);
                mainRoot.Add(mainRootItem);

                JSONArray detailData = new JSONArray();

                JSONObject detailDataItem = new JSONObject();
                detailDataItem.Add("tableDBName", "formtable_main_58_dt1");
                JSONArray workflowRequestTableRecords = new JSONArray();

                DynamicObjectCollection materials = o["FCRMContractEntry"] as DynamicObjectCollection;
                foreach (DynamicObject material in materials)
                {
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
                    workflowRequestTableFieldsItem.Add("fieldName", "chdl");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_SRT_CHDLName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "bombb");
                    workflowRequestTableFieldsItem.Add("fieldValue", BomName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "fxcp");
                    workflowRequestTableFieldsItem.Add("fieldValue", ParentMatIdName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xsdw");
                    workflowRequestTableFieldsItem.Add("fieldValue", UnitIdName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xssl");
                    workflowRequestTableFieldsItem.Add("fieldValue", Qty);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jjdw");
                    workflowRequestTableFieldsItem.Add("fieldValue", FPRICEUNITIDName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jjsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", FPRICEUNITQTY);
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
                    workflowRequestTableFieldsItem.Add("fieldName", "hl");
                    workflowRequestTableFieldsItem.Add("fieldValue", FTAXRATE);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "je");
                    workflowRequestTableFieldsItem.Add("fieldValue", FAMOUNT);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "se");
                    workflowRequestTableFieldsItem.Add("fieldValue", FTAXAMOUNT);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jshj");
                    workflowRequestTableFieldsItem.Add("fieldValue", AllAmount);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "yhrq");
                    workflowRequestTableFieldsItem.Add("fieldValue", FDELIVERYDATE);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jszz");
                    workflowRequestTableFieldsItem.Add("fieldValue", FSETTLEORGNUMBER);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "bz");
                    workflowRequestTableFieldsItem.Add("fieldValue", Note);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "sfzp");
                    workflowRequestTableFieldsItem.Add("fieldValue", FISFREE);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "kcdw");
                    workflowRequestTableFieldsItem.Add("fieldValue", FSTOCKUNITIDNAME);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "kcsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", FSTOCKQTY);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableRecordsItem.Add("workflowRequestTableFields", workflowRequestTableFields);
                    workflowRequestTableRecords.Add(workflowRequestTableRecordsItem);
                }
                    detailDataItem.Add("workflowRequestTableRecords", workflowRequestTableRecords);
                    detailData.Add(detailDataItem);

                detailDataItem = new JSONObject();
                detailDataItem.Add("tableDBName", "formtable_main_58_dt2");
                workflowRequestTableRecords = new JSONArray();

                DynamicObjectCollection FIinstallment = o["FContractPlan"] as DynamicObjectCollection;
                foreach (DynamicObject IinstallmentItem in FIinstallment)
                {
                    string NeedRecAdvance = Convert.ToString(IinstallmentItem["NeedRecAdvance"]).Equals("True") ? "0" : "1";
                    string FRECADVANCERATE = Convert.ToDecimal(IinstallmentItem["FRECADVANCERATE"]).ToString("#0.00");
                    string FRECADVANCEAMOUNT = Convert.ToDecimal(IinstallmentItem["FRECADVANCEAMOUNT"]).ToString("#0.00");
                    string FMUSTDATE = Convert.ToDateTime(IinstallmentItem["FMUSTDATE"]).ToString("yyyy-MM-dd");
                    string FRECJOINAMOUNT = Convert.ToDecimal(IinstallmentItem["FRECJOINAMOUNT"]).ToString("#0.00");
                    DynamicObject F_SRT_ProjectNode = IinstallmentItem["F_SRT_ProjectNode"] as DynamicObject;
                    string F_SRT_ProjectNodeNumber = F_SRT_ProjectNode == null ? "" : Convert.ToString(F_SRT_ProjectNode["Number"]);
                    string F_SRT_ProjectNodeName = F_SRT_ProjectNode == null ? "" : Convert.ToString(F_SRT_ProjectNode["Name"]);
                    string F_SRT_CheckBox = Convert.ToString(IinstallmentItem["F_SRT_CheckBox"]).Equals("True") ? "0" : "1";
                    string F_SRT_NodeCheckBox = F_SRT_ProjectNode == null ? "" : Convert.ToString(F_SRT_ProjectNode["F_SRT_NodeCheckBox"]).Equals("True")?"0":"1";
                    string erpId = Convert.ToString(IinstallmentItem["Id"]);


                    JSONObject workflowRequestTableRecordsItem = new JSONObject();
                    workflowRequestTableRecordsItem.Add("recordOrder", "0");
                    JSONArray workflowRequestTableFields = new JSONArray();

                    JSONObject workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "sfys");
                    workflowRequestTableFieldsItem.Add("fieldValue", NeedRecAdvance);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "ysbl");
                    workflowRequestTableFieldsItem.Add("fieldValue", FRECADVANCERATE);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "ysje");
                    workflowRequestTableFieldsItem.Add("fieldValue", FRECADVANCEAMOUNT);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "dqr");
                    workflowRequestTableFieldsItem.Add("fieldValue", FMUSTDATE);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "glskje");
                    workflowRequestTableFieldsItem.Add("fieldValue", FRECJOINAMOUNT);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xmjdbm");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_SRT_ProjectNodeNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xmjdmc");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_SRT_ProjectNodeName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xmsfxyglqr");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_SRT_NodeCheckBox);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xmsfglqr");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_SRT_CheckBox);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "skjhid");
                    workflowRequestTableFieldsItem.Add("fieldValue", erpId);
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
                        + "&requestName=销售合同已到达"
                        + "&workflowId=17&detailData=" + detailData.ToString(),personId);

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
