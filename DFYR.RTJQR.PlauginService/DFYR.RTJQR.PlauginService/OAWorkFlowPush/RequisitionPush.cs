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
    [Kingdee.BOS.Util.HotUpdate, Description("推送采购申请单至OA")]
    public class RequisitionPush : AbstractOperationServicePlugIn
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
                string BillTypeNumber = BillTypeID == null? "":Convert.ToString(BillTypeID["Number"]);
                BillTypeNumber = getOABillType(BillTypeNumber);
                string ApplicationDate = Convert.ToDateTime(o["ApplicationDate"]).ToString("yyyy-MM-dd");
                string RequestType = Convert.ToString(o["RequestType"]);
                RequestType = this.RequestType(RequestType);
                DynamicObject ApplicationOrgId = o["ApplicationOrgId"] as DynamicObject;
                string ApplicationOrgNumber = ApplicationOrgId == null ? "" : Convert.ToString(ApplicationOrgId["F_PYEO_Text_OAID"]);
                DynamicObject ApplicationDeptId = o["ApplicationDeptId"] as DynamicObject;
                string ApplicationDeptNumber = ApplicationDeptId == null ? "" : Convert.ToString(ApplicationDeptId["F_PYEO_Text_OAID"]);
                DynamicObject ApplicantId = o["ApplicantId"] as DynamicObject;
                string ApplicantNumber = ApplicantId == null ? "" : Convert.ToString(ApplicantId["Number"]);
                ApplicantNumber = Utils.getPersonOAid(this.Context, ApplicantNumber);
                DynamicObject CurrencyId = o["CurrencyId"] as DynamicObject;
                string CurrencyNumber = CurrencyId == null ? "" : Convert.ToString(CurrencyId["Number"]);
                string TotalAmount = Convert.ToDecimal(o["TotalAmount"]).ToString("#0.00");
                string Note = Convert.ToString(o["Note"]);
                string FISPRICEEXCLUDETAX = Convert.ToString(o["FISPRICEEXCLUDETAX"]).Equals("True") ? "0" : "1";
                DynamicObject F_SRT_CGLX = o["F_SRT_CGLX"] as DynamicObject;
                string F_SRT_CGLXNumber = F_SRT_CGLX == null ? "" : Convert.ToString(F_SRT_CGLX["Number"]);
                F_SRT_CGLXNumber = this.PurType(F_SRT_CGLXNumber);
                string FACCTYPE = Convert.ToString(o["FACCTYPE"]);
                FACCTYPE = this.getAcceptance(FACCTYPE);
                string F_PYEO_CheckBox_sfjjsq = Convert.ToString(o["F_PYEO_CheckBox_sfjjsq"].Equals("True")?"0":"1");

                JSONArray mainRoot = new JSONArray();
                JSONObject mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "djlx");
                mainRootItem.Add("fieldValue", BillTypeNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "erpnumber");
                mainRootItem.Add("fieldValue", billNo);
                mainRoot.Add(mainRootItem);


                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "djbh");
                mainRootItem.Add("fieldValue", billNo);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "sqrq");
                mainRootItem.Add("fieldValue", ApplicationDate);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "sqlx");
                mainRootItem.Add("fieldValue", RequestType);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "sqzz");
                mainRootItem.Add("fieldValue", ApplicationOrgNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "sqbm");
                mainRootItem.Add("fieldValue", ApplicationDeptNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "djzt");
                mainRootItem.Add("fieldValue", "审核中");
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "sqr");
                mainRootItem.Add("fieldValue", ApplicantNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "bb");
                mainRootItem.Add("fieldValue", CurrencyNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "hsjehj");
                mainRootItem.Add("fieldValue", TotalAmount);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "bz");
                mainRootItem.Add("fieldValue", Note);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "jws");
                mainRootItem.Add("fieldValue", FISPRICEEXCLUDETAX);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "cglx");
                mainRootItem.Add("fieldValue", F_SRT_CGLXNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "ysfs");
                mainRootItem.Add("fieldValue", FACCTYPE);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "sfjjsq");
                mainRootItem.Add("fieldValue", F_PYEO_CheckBox_sfjjsq);
                mainRoot.Add(mainRootItem);


                JSONArray detailData = new JSONArray();

                JSONObject detailDataItem = new JSONObject();
                detailDataItem.Add("tableDBName", "formtable_main_50_dt1");
                JSONArray workflowRequestTableRecords = new JSONArray();

                DynamicObjectCollection ReqEntrys = o["ReqEntry"] as DynamicObjectCollection;
                foreach (DynamicObject ReqEntry in ReqEntrys)
                {
                    DynamicObject RequireOrgId = ReqEntry["RequireOrgId"] as DynamicObject;
                    string RequireOrgNumber = RequireOrgId == null ? "" : Convert.ToString(RequireOrgId["F_PYEO_Text_OAID"]);
                    DynamicObject MaterialId = ReqEntry["MaterialId"] as DynamicObject;
                    string MaterialNumber = MaterialId == null ? "" : Convert.ToString(MaterialId["Number"]);
                    string MaterialName = MaterialId == null ? "" : Convert.ToString(MaterialId["Name"]);
                    string Specification = MaterialId == null ? "" : Convert.ToString(MaterialId["Specification"]);
                    DynamicObject F_PYEO_BASE_ic = MaterialId["F_PYEO_BASE_ic"] as DynamicObject;
                    string F_PYEO_BASE_icNmae = F_PYEO_BASE_ic == null ? "" : Convert.ToString(F_PYEO_BASE_ic["Name"]);
                    string stockQty = Convert.ToDecimal(ReqEntry["REQSTOCKQTY"]).ToString("#0.00");

                    DynamicObject UnitID = ReqEntry["UnitID"] as DynamicObject;
                    string UnitNumber = UnitID == null ? "" : Convert.ToString(UnitID["Number"]);
                    string ReqQty = Convert.ToDecimal(ReqEntry["ReqQty"]).ToString("#0.00");
                    string ApproveQty = Convert.ToDecimal(ReqEntry["ApproveQty"]).ToString("#0.00");
                    string ArrivalDate = Convert.ToDateTime(ReqEntry["ArrivalDate"]).ToString("yyyy-MM-dd");
                    DynamicObject PurchaseOrgId = ReqEntry["PurchaseOrgId"] as DynamicObject;
                    string PurchaseOrgNumber = PurchaseOrgId == null ? "" : Convert.ToString(PurchaseOrgId["F_PYEO_Text_OAID"]);
                    DynamicObject SuggestSupplierId = ReqEntry["SuggestSupplierId"] as DynamicObject;
                    string SuggestSupplierNumber = SuggestSupplierId == null ? "" : Convert.ToString(SuggestSupplierId["Number"]);
                    DynamicObject ReceiveOrgId = ReqEntry["ReceiveOrgId"] as DynamicObject;
                    string ReceiveOrgNumber = ReceiveOrgId == null ? "" : Convert.ToString(ReceiveOrgId["F_PYEO_Text_OAID"]);
                    DynamicObject PriceUnitId = ReqEntry["PriceUnitId"] as DynamicObject;
                    string PriceUnitNumber = PriceUnitId == null ? "" : Convert.ToString(PriceUnitId["Number"]);
                    string PriceUnitQty = Convert.ToDecimal(ReqEntry["PriceUnitQty"]).ToString("#0.00");
                    DynamicObject REQSTOCKUNITID = ReqEntry["REQSTOCKUNITID"] as DynamicObject;
                    string REQSTOCKUNITNumber = REQSTOCKUNITID == null ? "" : Convert.ToString(REQSTOCKUNITID["Number"]);
                    string EntryNote = Convert.ToString(ReqEntry["EntryNote"]);
                    string TAXPRICE = Convert.ToDecimal(ReqEntry["TAXPRICE"]).ToString("#0.00");
                    string Amount = Convert.ToDecimal(ReqEntry["Amount"]).ToString("#0.00");
                    string ReqAmount = Convert.ToDecimal(ReqEntry["ReqAmount"]).ToString("#0.00");
                    string EvaluatePrice = Convert.ToDecimal(ReqEntry["EvaluatePrice"]).ToString("#0.00");
                    string TAXRATE = Convert.ToDecimal(ReqEntry["TAXRATE"]).ToString("#0.00");
                    

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
                    workflowRequestTableFieldsItem.Add("fieldName", "ggxh");
                    workflowRequestTableFieldsItem.Add("fieldValue", Specification);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "sqdww");
                    workflowRequestTableFieldsItem.Add("fieldValue", UnitNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "sqsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", ReqQty);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "pzsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", ApproveQty);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "dhrq");
                    workflowRequestTableFieldsItem.Add("fieldValue", ArrivalDate);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "cgzz");
                    workflowRequestTableFieldsItem.Add("fieldValue", PurchaseOrgNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jygys");
                    workflowRequestTableFieldsItem.Add("fieldValue", SuggestSupplierNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "slzz");
                    workflowRequestTableFieldsItem.Add("fieldValue", ReceiveOrgNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jjdww");
                    workflowRequestTableFieldsItem.Add("fieldValue", PriceUnitNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jjsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", PriceUnitQty);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "kcdww");
                    workflowRequestTableFieldsItem.Add("fieldValue", REQSTOCKUNITNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "bz");
                    workflowRequestTableFieldsItem.Add("fieldValue", EntryNote);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "hsdj");
                    workflowRequestTableFieldsItem.Add("fieldValue", TAXPRICE);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "je");
                    workflowRequestTableFieldsItem.Add("fieldValue", Amount);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "hsje");
                    workflowRequestTableFieldsItem.Add("fieldValue", ReqAmount);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "dj");
                    workflowRequestTableFieldsItem.Add("fieldValue", EvaluatePrice);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "sqdww");
                    workflowRequestTableFieldsItem.Add("fieldValue", UnitNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jjdww");
                    workflowRequestTableFieldsItem.Add("fieldValue", PriceUnitNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "kcdww");
                    workflowRequestTableFieldsItem.Add("fieldValue", REQSTOCKUNITNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "chdl");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_PYEO_BASE_icNmae);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);
   
                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "kcsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", stockQty);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "sljs");
                    workflowRequestTableFieldsItem.Add("fieldValue", TAXRATE);
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
                    + "&requestName=采购申请单已到达"
                    + "&workflowId=9&detailData=" + detailData.ToString(), personId);

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
                string queryOAidSql = string.Format(@"select F_PYEO_OAID6 from PYEO_t_Cust_Entry100033 where F_PYEO_ERPID6 = '{0}'", erpId.Trim());
                return DBUtils.ExecuteDynamicObject(this.Context, queryOAidSql)[0]["F_PYEO_OAID6"].ToString();
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
        /// 申请 类型
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
                return "";
            }
        }
        
        
    }
}
