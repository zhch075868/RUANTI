using Kingdee.BOS.App.Data;
using Kingdee.BOS.Core.DynamicForm.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Core.Metadata.FieldElement;
using Kingdee.BOS.Core.DynamicForm.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Core.Metadata.FieldElement;
using Kingdee.BOS.Util;
using Kingdee.BOS.JSON;
using Kingdee.BOS.Orm.DataEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingdee.BOS.Core.DynamicForm;
using Kingdee.BOS;

namespace DFYR.RTJQR.PlauginService.OAWorkFlowPush
{
    [Kingdee.BOS.Util.HotUpdate, Description("推送付款申请单至OA")]
    public class PayapplyPush : AbstractOperationServicePlugIn
    {
        public override void OnPreparePropertys(PreparePropertysEventArgs e)
        {

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
                string billNo = Convert.ToString(o["FBILLNo"]);
                string documentStatus = Convert.ToString(o["FDOCUMENTSTATUS"]);

                //提交校验
                if (!documentStatus.Equals("B"))
                {
                    return;
                }
                var field = this.BusinessInfo.GetField("FCONTACTUNITTYPE") as ComboField; ;
                var enumValue = o["FCONTACTUNITTYPE"];
                string FCONTACTUNITTYPE = field.GetEnumItemName(enumValue);
                string FPAYAMOUNTFOR_H = Convert.ToDecimal(o["FPAYAMOUNTFOR_H"]).ToString("#0.00");
                DynamicObject F_PYEO_ContractNo2 = o["F_PYEO_ContractNo2"] as DynamicObject;
                string F_PYEO_ContractNo2Number = F_PYEO_ContractNo2 == null ? "" : Convert.ToString(F_PYEO_ContractNo2["Number"]);
                DynamicObject FRECTUNIT = o["FRECTUNIT"] as DynamicObject;
                string FRECTUNITName = FRECTUNIT == null ? "" : Convert.ToString(FRECTUNIT["Name"]);
                //string FEXCHANGERATE = Convert.ToDecimal("FEXCHANGERATE").ToString("#0.00");
                DynamicObject FSETTLECUR = o["FSETTLECUR"] as DynamicObject;
                string FSETTLECURName = FSETTLECUR == null ? "" : Convert.ToString(FSETTLECUR["Name"]);
                //string FSETTLERATE = Convert.ToDecimal("FSETTLERATE").ToString("#0.00");
                DynamicObject FSETTLEORGID = o["FSETTLEORGID"] as DynamicObject;
                string FSETTLEORGIDNumber = FSETTLEORGID == null ? "" : Convert.ToString(FSETTLEORGID["F_PYEO_Text_OAID"]);
                DynamicObject FPURCHASEORGID = o["FPURCHASEORGID"] as DynamicObject;
                string FPURCHASEORGIDNumber = FPURCHASEORGID == null ? "" : Convert.ToString(FPURCHASEORGID["F_PYEO_Text_OAID"]);
                DynamicObject PURCHASERGROUPID = o["PURCHASERGROUPID"] as DynamicObject;
                string PURCHASERGROUPIDNumber = PURCHASERGROUPID == null ? "" : Convert.ToString(PURCHASERGROUPID["Name"]);
                DynamicObject FCONTACTUNIT = o["FCONTACTUNIT"] as DynamicObject;
                string FCONTACTUNITNumber = FCONTACTUNIT == null ? "" : Convert.ToString(FCONTACTUNIT["Name"]);
                string F_PYEO_ContractNo1 = Convert.ToString(o["F_PYEO_ContractNo1"]);
                DynamicObject FAPPLYORGID = o["FAPPLYORGID"] as DynamicObject;
                string FAPPLYORGIDNumber = FAPPLYORGID == null ? "" : Convert.ToString(FAPPLYORGID["F_PYEO_Text_OAID"]);
                DynamicObject FPAYORGID = o["FPAYORGID"] as DynamicObject;
                string FPAYORGIDNumber = FPAYORGID == null ? "" : Convert.ToString(FPAYORGID["F_PYEO_Text_OAID"]);
                field = this.BusinessInfo.GetField("F_PYEO__SRT_CONTRACTYPE") as ComboField; ;
                enumValue = o["F_PYEO__SRT_CONTRACTYPE"];
                string F_PYEO__SRT_CONTRACTYPE = field.GetEnumItemName(enumValue);
                DynamicObject FPURCHASEDEPTID = o["FPURCHASEDEPTID"] as DynamicObject;
                string FPURCHASEDEPTIDNumber = FPURCHASEDEPTID == null ? "" : Convert.ToString(FPURCHASEDEPTID["F_PYEO_Text_OAID"]);
                DynamicObject PURCHASERID = o["FPURCHASERID"] as DynamicObject;
                string PURCHASERIDName = PURCHASERID == null ? "" : Convert.ToString(PURCHASERID["Number"]);
                PURCHASERIDName = Utils.getPersonOAid(this.Context, PURCHASERIDName);
                DynamicObject F_SRT_CGLXId = o["F_PYEO__SRT_CGLX"] as DynamicObject;
                string F_SRT_CGLX = Convert.ToString(F_SRT_CGLXId["Number"]);
                F_SRT_CGLX = this.PurType(F_SRT_CGLX);
                DynamicObject BillTypeID = o["FBILLTYPEID"] as DynamicObject;
                string BillTypeNumber = BillTypeID == null? "":Convert.ToString(BillTypeID["Name"]);
                string date = Convert.ToDateTime(o["FDATE"]).ToString("yyyy-MM-dd");
                field = this.BusinessInfo.GetField("FRECTUNITTYPE") as ComboField; ;
                enumValue = o["FRECTUNITTYPE"];
                string FRECTUNITTYPE = field.GetEnumItemName(enumValue);
                string FPAYAPPLYAMOUNTFOR_H = Convert.ToDecimal(o["FPAYAPPLYAMOUNTFOR_H"]).ToString("#0.00");
                DynamicObject FCURRENCYID = o["FCURRENCYID"] as DynamicObject;
                string FCURRENCYIDName = FCURRENCYID == null ? "" : Convert.ToString(FCURRENCYID["Name"]);

                JSONArray mainRoot = new JSONArray();
                JSONObject mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "djbh");
                mainRootItem.Add("fieldValue", billNo);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "erpnumber");
                mainRootItem.Add("fieldValue", billNo);
                mainRoot.Add(mainRootItem);
                

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "wldwlx");
                mainRootItem.Add("fieldValue", FCONTACTUNITTYPE);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "yfje");
                mainRootItem.Add("fieldValue", FPAYAMOUNTFOR_H);
                mainRoot.Add(mainRootItem);

                //mainRootItem = new JSONObject();
                //mainRootItem.Add("fieldName", "glcgddht");
                //mainRootItem.Add("fieldValue", F_PYEO_ContractNo2Number);
                //mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "skdw");
                mainRootItem.Add("fieldValue", FRECTUNITName);
                mainRoot.Add(mainRootItem);

                //mainRootItem = new JSONObject();
                //mainRootItem.Add("fieldName", "hl");
                //mainRootItem.Add("fieldValue", FEXCHANGERATE);
                //mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "jsbb");
                mainRootItem.Add("fieldValue", FSETTLECURName);
                mainRoot.Add(mainRootItem);

                //mainRootItem = new JSONObject();
                //mainRootItem.Add("fieldName", "jshl");
                //mainRootItem.Add("fieldValue", FSETTLERATE);
                //mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "jszz");
                mainRootItem.Add("fieldValue", FSETTLEORGIDNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "cgzz");
                mainRootItem.Add("fieldValue", FPURCHASEORGIDNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "cgz");
                mainRootItem.Add("fieldValue", PURCHASERGROUPIDNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "wldwgys");
                mainRootItem.Add("fieldValue", FCONTACTUNITNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "hth1");
                mainRootItem.Add("fieldValue", F_PYEO_ContractNo1);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "sqzz");
                mainRootItem.Add("fieldValue", FAPPLYORGIDNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "fkzz");
                mainRootItem.Add("fieldValue", FPAYORGIDNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "htlx");
                mainRootItem.Add("fieldValue", F_PYEO__SRT_CONTRACTYPE);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "cgbm");
                mainRootItem.Add("fieldValue", FPURCHASEDEPTIDNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "cgy");
                mainRootItem.Add("fieldValue", PURCHASERIDName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "cglx");
                mainRootItem.Add("fieldValue", F_SRT_CGLX);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "djlx");
                mainRootItem.Add("fieldValue", BillTypeNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "sqrq");
                mainRootItem.Add("fieldValue", date);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "sqfkje");
                mainRootItem.Add("fieldValue", FPAYAPPLYAMOUNTFOR_H);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "skdwlx");
                mainRootItem.Add("fieldValue", FRECTUNITTYPE);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "bb");
                mainRootItem.Add("fieldValue", FCURRENCYIDName);
                mainRoot.Add(mainRootItem);

                JSONArray detailData = new JSONArray();

                JSONObject detailDataItem = new JSONObject();
                detailDataItem.Add("tableDBName", "formtable_main_62_dt1");//formtable_main_62_dt1
                JSONArray workflowRequestTableRecords = new JSONArray();

                DynamicObjectCollection FPAYAPPLYENTRY = o["FPAYAPPLYENTRY"] as DynamicObjectCollection;
                foreach (DynamicObject entry in FPAYAPPLYENTRY)
                {
                    DynamicObject FCOSTID = entry["FCOSTID"] as DynamicObject;
                    string FCOSTIDName = FCOSTID == null ? "" : Convert.ToString(FCOSTID["Name"]);
                    DynamicObject FSETTLETYPEID = entry["FSETTLETYPEID"] as DynamicObject;
                    string FSETTLETYPEIDName = FSETTLETYPEID == null ? "" : Convert.ToString(FSETTLETYPEID["Name"]);
                    string FENDDATE = Convert.ToDateTime(entry["FENDDATE"]).ToString("yyyy-MM-dd");
                    string FPAYAMOUNTFOR = Convert.ToDecimal(entry["FPAYAMOUNTFOR"]).ToString("#0.00");
                    string FUnpaidAmount = Convert.ToDecimal(entry["FUnpaidAmount"]).ToString("#0.00");
                    string FEACHBANKACCOUNT  = Convert.ToString(entry["FEACHBANKACCOUNT"]);
                    string FEACHBANKNAME = Convert.ToString(entry["FEACHBANKNAME"]);
                    string TAXAMOUNT = Convert.ToDecimal(entry["TAXAMOUNT"]).ToString("#0.00");
                    DynamicObject FEXPENSEDEPTID = entry["FEXPENSEDEPTID"] as DynamicObject;
                    string FEXPENSEDEPTIDNumber = FEXPENSEDEPTID == null ? "" : Convert.ToString(FEXPENSEDEPTID["F_PYEO_Text_OAID"]);
                    DynamicObject FPAYPURPOSEID = entry["FPAYPURPOSEID"] as DynamicObject;
                    string FPAYPURPOSEIDName = FPAYPURPOSEID == null ? "" : Convert.ToString(FPAYPURPOSEID["Name"]);
                    string FEACHCCOUNTNAME = Convert.ToString(entry["FEACHCCOUNTNAME"]);
                    string Description = Convert.ToString(entry["Description"]);
                    string FEXPECTPAYDATE = Convert.ToDateTime(entry["FEXPECTPAYDATE"]).ToString("yyyy-MM-dd");
                    string FAPPLYAMOUNTFOR = Convert.ToDecimal(entry["FAPPLYAMOUNTFOR"]).ToString("#0.00");

                    JSONObject workflowRequestTableRecordsItem = new JSONObject();
                    workflowRequestTableRecordsItem.Add("recordOrder", "0");
                    JSONArray workflowRequestTableFields = new JSONArray();


                    JSONObject workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "fyxm");
                    workflowRequestTableFieldsItem.Add("fieldValue", FCOSTIDName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jsfs");
                    workflowRequestTableFieldsItem.Add("fieldValue", FSETTLETYPEIDName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "dqr");
                    workflowRequestTableFieldsItem.Add("fieldValue", FENDDATE);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "yfje");
                    workflowRequestTableFieldsItem.Add("fieldValue", FPAYAMOUNTFOR);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "wfkje");
                    workflowRequestTableFieldsItem.Add("fieldValue", FUnpaidAmount);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "dfyxzh");
                    workflowRequestTableFieldsItem.Add("fieldValue", FEACHBANKACCOUNT);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "dfkhx");
                    workflowRequestTableFieldsItem.Add("fieldValue", FEACHBANKNAME);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "se");
                    workflowRequestTableFieldsItem.Add("fieldValue", TAXAMOUNT);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "fybm");
                    workflowRequestTableFieldsItem.Add("fieldValue", FEXPENSEDEPTIDNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "fkyt");
                    workflowRequestTableFieldsItem.Add("fieldValue", FPAYPURPOSEIDName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "dfzhmc");
                    workflowRequestTableFieldsItem.Add("fieldValue", FEACHCCOUNTNAME);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "bz");
                    workflowRequestTableFieldsItem.Add("fieldValue", Description);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "qwfkrq");
                    workflowRequestTableFieldsItem.Add("fieldValue", FEXPECTPAYDATE);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "sqfkje");
                    workflowRequestTableFieldsItem.Add("fieldValue", FAPPLYAMOUNTFOR);
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
                    + "&requestName=付款申请单已到达"
                    + "&workflowId=21&detailData=" + detailData.ToString(), personId);

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
    }
}
