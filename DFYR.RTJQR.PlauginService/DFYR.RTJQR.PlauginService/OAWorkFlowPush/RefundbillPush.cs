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
    [Kingdee.BOS.Util.HotUpdate, Description("推送收款退款单至OA")]
    public class RefundbillPush : AbstractOperationServicePlugIn
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
                string billNo = Convert.ToString(o["BillNo"]);
                string documentStatus = Convert.ToString(o["DocumentStatus"]);

                //提交校验
                if (!documentStatus.Equals("B"))
                {
                    return;
                }
                DynamicObject BillTypeID = o["BillTypeID"] as DynamicObject;
                string BillTypeIDNumber = BillTypeID == null ? "" : Convert.ToString(BillTypeID["Name"]);
                var field = this.BusinessInfo.GetField("FRECTUNITTYPE") as ComboField; ;
                var enumValue = o["RECTUNITTYPE"];
                var RECTUNITTYPE = field.GetEnumItemName(enumValue);
                DynamicObject RECTUNIT = o["RECTUNIT"] as DynamicObject;
                string RECTUNITNumber = RECTUNIT == null ? "" : Convert.ToString(RECTUNIT["Name"]);
                DynamicObject F_SRT_TD = o["F_SRT_TD"] as DynamicObject;
                string F_SRT_TDNumber = F_SRT_TD == null ? "" : Convert.ToString(F_SRT_TD["Name"]);
                string date = Convert.ToDateTime(o["Date"]).ToString("yyyy-MM-dd");
                DynamicObject SaleOrgId = o["SALEORGID"] as DynamicObject;
                string SaleOrgIdNumber = SaleOrgId == null ? "" : Convert.ToString(SaleOrgId["F_PYEO_Text_OAID"]);
                DynamicObject SaleDeptID = o["SALEDEPTID"] as DynamicObject;
                string SaleDeptIDNumber = SaleDeptID == null ? "" : Convert.ToString(SaleDeptID["F_PYEO_Text_OAID"]);
                DynamicObject F_SRT_XSQD = o["F_SRT_XSQD"] as DynamicObject;
                string F_SRT_XSQDName = F_SRT_XSQD == null ? "" : Convert.ToString(F_SRT_XSQD["Name"]);
                DynamicObject F_SRT_XSLX = o["F_SRT_XSLX"] as DynamicObject;
                string F_SRT_XSLXName = F_SRT_XSLX == null ? "" : Convert.ToString(F_SRT_XSLX["Name"]);
                DynamicObject SALEGROUPID = o["SALEGROUPID"] as DynamicObject;
                string SALEGROUPIDName = SALEGROUPID == null ? "" : Convert.ToString(SALEGROUPID["Name"]);
                DynamicObject SALEERID = o["SALEERID"] as DynamicObject;
                string SALEERIDName = SALEERID == null ? "" : Convert.ToString(SALEERID["Name"]);
                DynamicObject F_SRT_DQ = o["F_SRT_DQ"] as DynamicObject;
                string F_SRT_DQName = F_SRT_DQ == null ? "" : Convert.ToString(F_SRT_DQ["FDataValue"]);
                DynamicObject F_SRT_HY = o["F_SRT_HY"] as DynamicObject;
                string F_SRT_HYName = F_SRT_HY == null ? "" : Convert.ToString(F_SRT_HY["FDataValue"]);
                DynamicObject F_SRT_HTH = o["F_SRT_HTH"] as DynamicObject;
                string F_SRT_HTHNumber = F_SRT_HTH == null ? "" : Convert.ToString(F_SRT_HTH["Number"]);
                DynamicObject F_PYEO__SRT_HTYWLX = o["F_PYEO__SRT_HTYWLX"] as DynamicObject;
                string F_PYEO__SRT_HTYWLXName = F_PYEO__SRT_HTYWLX == null ? "" : Convert.ToString(F_PYEO__SRT_HTYWLX["Name"]);
                DynamicObject F_SRT_MYLX = o["F_SRT_MYLX"] as DynamicObject;
                string F_SRT_MYLXName = F_SRT_MYLX == null ? "" : Convert.ToString(F_SRT_MYLX["Name"]);
                DynamicObject F_SRT_ZDKH = o["F_SRT_ZDKH"] as DynamicObject;
                string F_SRT_ZDKHNumber = F_SRT_ZDKH == null ? "" : Convert.ToString(F_SRT_ZDKH["Name"]);
                DynamicObject FPAYORGID = o["FPAYORGID"] as DynamicObject;
                string FPAYORGIDNumber = FPAYORGID == null ? "" : Convert.ToString(FPAYORGID["F_PYEO_Text_OAID"]);
                DynamicObject SETTLEORGID = o["SETTLEORGID"] as DynamicObject;
                string SETTLEORGIDNumber = SETTLEORGID == null ? "" : Convert.ToString(SETTLEORGID["F_PYEO_Text_OAID"]);
                DynamicObject F_SRT_CPFL = o["F_SRT_CPFL"] as DynamicObject;
                string F_SRT_CPFLName = F_SRT_CPFL == null ? "" : Convert.ToString(F_SRT_CPFL["Name"]);
                DynamicObject CURRENCYID = o["CURRENCYID"] as DynamicObject;
                string CURRENCYIDName = CURRENCYID == null ? "" : Convert.ToString(CURRENCYID["Name"]);
                field = this.BusinessInfo.GetField("FCONTACTUNITTYPE") as ComboField; ;
                enumValue = o["CONTACTUNITTYPE"];
                var CONTACTUNITTYPE = field.GetEnumItemName(enumValue);
                DynamicObject CONTACTUNIT = o["CONTACTUNIT"] as DynamicObject;
                string CONTACTUNITName = CONTACTUNIT == null ? "" : Convert.ToString(CONTACTUNIT["Name"]);
                DynamicObject F_SRT_Project = o["F_SRT_Project"] as DynamicObject;
                string F_SRT_ProjectName = F_SRT_Project == null ? "" : Convert.ToString(F_SRT_Project["Name"]);
                DynamicObject F_PYEO_Assistant_HYYJ = o["F_PYEO_Assistant_HYYJ"] as DynamicObject;
                string F_PYEO_Assistant_HYYJName = F_PYEO_Assistant_HYYJ == null ? "" : Convert.ToString(F_PYEO_Assistant_HYYJ["FDataValue"]);
                DynamicObject F_PYEO_Assistant_HYEJ = o["F_PYEO_Assistant_HYEJ"] as DynamicObject;
                string F_PYEO_Assistant_HYEJName = F_PYEO_Assistant_HYEJ == null ? "" : Convert.ToString(F_PYEO_Assistant_HYEJ["FDataValue"]);
                string FBookingDate = Convert.ToDateTime(o["FBookingDate"]).ToString("yyyy-MM-dd");
                string FREMARK = Convert.ToString(o["FREMARK"]);
                string REFUNDTOTALAMOUNTFOR = Convert.ToDecimal(o["REFUNDTOTALAMOUNTFOR"]).ToString("#0.00");
                string REALREFUNDAMOUNTFOR = Convert.ToDecimal(o["REALREFUNDAMOUNTFOR"]).ToString("#0.00");


                JSONArray mainRoot = new JSONArray();
                JSONObject mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "djlx");
                mainRootItem.Add("fieldValue", BillTypeIDNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "djbh");
                mainRootItem.Add("fieldValue", billNo);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "skdwlx");
                mainRootItem.Add("fieldValue", RECTUNITTYPE);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "skdw");
                mainRootItem.Add("fieldValue", RECTUNITNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "td");
                mainRootItem.Add("fieldValue", F_SRT_TDNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "ywrq");
                mainRootItem.Add("fieldValue", date);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xszz");
                mainRootItem.Add("fieldValue", SaleOrgIdNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xsbm");
                mainRootItem.Add("fieldValue", SaleDeptIDNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xsqd");
                mainRootItem.Add("fieldValue", F_SRT_XSQDName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xslx");
                mainRootItem.Add("fieldValue", F_SRT_XSLXName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xsz");
                mainRootItem.Add("fieldValue", SALEGROUPIDName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xsy");
                mainRootItem.Add("fieldValue", SALEERIDName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "dq");
                mainRootItem.Add("fieldValue", F_SRT_DQName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xy");
                mainRootItem.Add("fieldValue", F_SRT_HYName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "ht");
                mainRootItem.Add("fieldValue", F_SRT_HTHNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "htywlx");
                mainRootItem.Add("fieldValue", F_PYEO__SRT_HTYWLXName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "mylx");
                mainRootItem.Add("fieldValue", F_SRT_MYLXName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "zdkh");
                mainRootItem.Add("fieldValue", F_SRT_ZDKHNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "fkzz");
                mainRootItem.Add("fieldValue", FPAYORGIDNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "jszz");
                mainRootItem.Add("fieldValue", SETTLEORGIDNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "cpfl");
                mainRootItem.Add("fieldValue", F_SRT_CPFLName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "bb");
                mainRootItem.Add("fieldValue", CURRENCYIDName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "wldwlx");
                mainRootItem.Add("fieldValue", CONTACTUNITTYPE);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "wldw");
                mainRootItem.Add("fieldValue", CONTACTUNITName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xm");
                mainRootItem.Add("fieldValue", F_SRT_ProjectName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xyyj");
                mainRootItem.Add("fieldValue", F_PYEO_Assistant_HYYJName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xyej");
                mainRootItem.Add("fieldValue", F_PYEO_Assistant_HYEJName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "qwfkrq");
                mainRootItem.Add("fieldValue", FBookingDate);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "bz");
                mainRootItem.Add("fieldValue", FREMARK);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "erpnumber");
                mainRootItem.Add("fieldValue", billNo);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "ytje");
                mainRootItem.Add("fieldValue", REFUNDTOTALAMOUNTFOR);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "stje");
                mainRootItem.Add("fieldValue", REALREFUNDAMOUNTFOR);
                mainRoot.Add(mainRootItem);

                JSONArray detailData = new JSONArray();

                JSONObject detailDataItem = new JSONObject();
                detailDataItem.Add("tableDBName", "formtable_main_77_dt1");
                JSONArray workflowRequestTableRecords = new JSONArray();

                DynamicObjectCollection entrys = o["REFUNDBILLENTRY"] as DynamicObjectCollection;
                foreach (DynamicObject entry in entrys)
                {
                    DynamicObject SETTLETYPEID = entry["SETTLETYPEID"] as DynamicObject;
                    string SETTLETYPEIDName = SETTLETYPEID == null ? "" : Convert.ToString(SETTLETYPEID["Name"]);
                    DynamicObject PURPOSEID = entry["PURPOSEID"] as DynamicObject;
                    string PURPOSEIDName = PURPOSEID == null ? "" : Convert.ToString(PURPOSEID["Name"]);
                    string ENTRYREALREFUNDAMOUNTFOR = Convert.ToDecimal(entry["REALREFUNDAMOUNTFOR"]).ToString("#0.00");
                    string REFUNDAMOUNTFOR = Convert.ToDecimal(entry["REALREFUNDAMOUNTFOR"]).ToString("#0.00");
                    string DISTAMOUNTFOR = Convert.ToDecimal(entry["DISTAMOUNTFOR"]).ToString("#0.00");
                    string OUAMOUNTFOR = Convert.ToDecimal(entry["OUAMOUNTFOR"]).ToString("#0.00");
                    string HANDLINGCHARGEFOR = Convert.ToDecimal(entry["HANDLINGCHARGEFOR"]).ToString("#0.00");
                    DynamicObject ACCOUNTID = entry["ACCOUNTID"] as DynamicObject;
                    string ACCOUNTIDName = ACCOUNTID == null ? "" : Convert.ToString(ACCOUNTID["Number"]);
                    DynamicObject CashAccount = entry["CashAccount"] as DynamicObject;
                    string CashAccountName = CashAccount == null ? "" : Convert.ToString(CashAccount["Name"]);
                    string SETTLENO = Convert.ToString(entry["SETTLENO"]);
                    string NOTE = Convert.ToString(entry["NOTE"]);
                    string FSALEORDERNUMBER = Convert.ToString(entry["FSALEORDERNUMBER"]);
                    DynamicObject MaterialId = entry["FMATERIALID"] as DynamicObject;
                    string MaterialNumber = MaterialId == null ? "" : Convert.ToString(MaterialId["Number"]);
                    string MaterialName = MaterialId == null ? "" : Convert.ToString(MaterialId["Name"]);
                    string FPrice = Convert.ToDecimal(entry["FPrice"]).ToString("#0.00");
                    string FQty = Convert.ToDecimal(entry["FQty"]).ToString("#0.00");
                    int FMATERIALSEQ = Convert.ToInt32(entry["FMATERIALSEQ"]);
                    DynamicObject COSTID = entry["COSTID"] as DynamicObject;
                    string COSTIDName = COSTID == null ? "" : Convert.ToString(COSTID["Name"]);
                    DynamicObject COSTDEPARTMENTID = entry["COSTDEPARTMENTID"] as DynamicObject;
                    string COSTDEPARTMENTIDName = COSTDEPARTMENTID == null ? "" : Convert.ToString(COSTDEPARTMENTID["Name"]);


                    JSONObject workflowRequestTableRecordsItem = new JSONObject();
                    workflowRequestTableRecordsItem.Add("recordOrder", "0");
                    JSONArray workflowRequestTableFields = new JSONArray();


                    JSONObject workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jsfs");
                    workflowRequestTableFieldsItem.Add("fieldValue", SETTLETYPEIDName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "yskyt");
                    workflowRequestTableFieldsItem.Add("fieldValue", PURPOSEIDName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "stje");
                    workflowRequestTableFieldsItem.Add("fieldValue", ENTRYREALREFUNDAMOUNTFOR);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "ytje");
                    workflowRequestTableFieldsItem.Add("fieldValue", REFUNDAMOUNTFOR);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xjzk");
                    workflowRequestTableFieldsItem.Add("fieldValue", DISTAMOUNTFOR);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "cdk");
                    workflowRequestTableFieldsItem.Add("fieldValue", OUAMOUNTFOR);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "sxf");
                    workflowRequestTableFieldsItem.Add("fieldValue", HANDLINGCHARGEFOR);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "wfyxzh");
                    workflowRequestTableFieldsItem.Add("fieldValue", ACCOUNTIDName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xjzh");
                    workflowRequestTableFieldsItem.Add("fieldValue", CashAccountName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jsh");
                    workflowRequestTableFieldsItem.Add("fieldValue", SETTLENO);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "bz");
                    workflowRequestTableFieldsItem.Add("fieldValue", NOTE);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xsddh");
                    workflowRequestTableFieldsItem.Add("fieldValue", FSALEORDERNUMBER);
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
                    workflowRequestTableFieldsItem.Add("fieldName", "hsdj");
                    workflowRequestTableFieldsItem.Add("fieldValue", FPrice);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "sl");
                    workflowRequestTableFieldsItem.Add("fieldValue", FQty);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "ddxh");
                    workflowRequestTableFieldsItem.Add("fieldValue", FMATERIALSEQ);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "fyxm");
                    workflowRequestTableFieldsItem.Add("fieldValue", COSTIDName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "fycdbm");
                    workflowRequestTableFieldsItem.Add("fieldValue", COSTDEPARTMENTIDName);
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
                    + "&requestName=收款退款单已到达"
                    + "&workflowId=33&detailData=" + detailData.ToString(), personId);

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
