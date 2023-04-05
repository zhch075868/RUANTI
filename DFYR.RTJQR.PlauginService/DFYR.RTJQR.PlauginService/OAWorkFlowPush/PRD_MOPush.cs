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
    [Kingdee.BOS.Util.HotUpdate, Description("生产订单推送至OA")]
    public class PRD_MOPush : AbstractOperationServicePlugIn
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
                DynamicObject BillType = o["BillType"] as DynamicObject;
                string BillTypeName = BillType == null ? "" : Convert.ToString(BillType["Name"]);
                string Date = Convert.ToDateTime(o["Date"]).ToString("yyyy-MM-dd");
                DynamicObject PrdOrgId = o["PrdOrgId"] as DynamicObject;
                string PrdOrgIdName = PrdOrgId == null ? "" : Convert.ToString(PrdOrgId["Number"]);
                DynamicObject WorkGroupId = o["WorkGroupId"] as DynamicObject;
                string WorkGroupIdName = WorkGroupId == null ? "" : Convert.ToString(WorkGroupId["Name"]);
                DynamicObject PlannerID = o["PlannerID"] as DynamicObject;
                string PlannerIDName = PlannerID == null ? "" : Convert.ToString(PlannerID["Name"]);
                DynamicObject ENTrustOrgId = o["ENTrustOrgId"] as DynamicObject;
                string ENTrustOrgIdName = ENTrustOrgId == null ? "" : Convert.ToString(ENTrustOrgId["Number"]);
                DynamicObject F_SRT_Project = o["F_SRT_Project"] as DynamicObject;
                string F_SRT_ProjectName = F_SRT_Project == null ? "" : Convert.ToString(F_SRT_Project["Name"]);
                DynamicObject F_PYEO__SRT_XSZXM = o["F_PYEO__SRT_XSZXM"] as DynamicObject;
                string F_PYEO__SRT_XSZXMName = F_PYEO__SRT_XSZXM == null ? "" : Convert.ToString(F_PYEO__SRT_XSZXM["Name"]);
                DynamicObject F_SRT_SCZXM = o["F_SRT_SCZXM"] as DynamicObject;
                string F_SRT_SCZXMName = F_SRT_SCZXM == null ? "" : Convert.ToString(F_SRT_SCZXM["Name"]);
                DynamicObject F_SRT_CustId = o["F_SRT_CustId"] as DynamicObject;
                string F_SRT_CustIdName = F_SRT_CustId == null ? "" : Convert.ToString(F_SRT_CustId["Name"]);
                DynamicObject F_SRT_ZDKH = o["F_SRT_ZDKH"] as DynamicObject;
                string F_SRT_ZDKHName = F_SRT_ZDKH == null ? "" : Convert.ToString(F_SRT_ZDKH["Name"]);
                DynamicObject F_SRT_DQ = o["F_SRT_DQ"] as DynamicObject;
                string F_SRT_DQName = F_SRT_DQ == null ? "" : Convert.ToString(F_SRT_DQ["FDataValue"]);
                DynamicObject F_SRT_HY = o["F_SRT_HY"] as DynamicObject;
                string F_SRT_HYName = F_SRT_HY == null ? "" : Convert.ToString(F_SRT_HY["FDataValue"]);
                DynamicObject F_PYEO_SaleDeptId = o["F_PYEO_SaleDeptId"] as DynamicObject;
                string F_PYEO_SaleDeptIdName = F_PYEO_SaleDeptId == null ? "" : Convert.ToString(F_PYEO_SaleDeptId["Number"]);
                DynamicObject F_SRT_MYLX = o["F_SRT_MYLX"] as DynamicObject;
                string F_SRT_MYLXName = F_SRT_MYLX == null ? "" : Convert.ToString(F_SRT_MYLX["Name"]);
                DynamicObject F_SRT_XSLX = o["F_SRT_XSLX"] as DynamicObject;
                string F_SRT_XSLXName = F_SRT_XSLX == null ? "" : Convert.ToString(F_SRT_XSLX["Name"]);
                DynamicObject F_SRT_TD = o["F_SRT_TD"] as DynamicObject;
                string F_SRT_TDName = F_SRT_TD == null ? "" : Convert.ToString(F_SRT_TD["Name"]);
                DynamicObject F_SRT_CPFL = o["F_SRT_CPFL"] as DynamicObject;
                string F_SRT_CPFLName = F_SRT_CPFL == null ? "" : Convert.ToString(F_SRT_CPFL["Name"]);
                string F_PYEO_SourceBillType = Convert.ToString(o["F_PYEO_SourceBillType"]);
                F_PYEO_SourceBillType = getOABillType(F_PYEO_SourceBillType);
                string F_PYEO_SourceBillNo = Convert.ToString(o["F_PYEO_SourceBillNo"]);
                string Description = Convert.ToString(o["Description"]);
                
                
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
                mainRootItem.Add("fieldName", "djrq");
                mainRootItem.Add("fieldValue", Date);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "sczz");
                mainRootItem.Add("fieldValue", PrdOrgIdName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "jhz");
                mainRootItem.Add("fieldValue", WorkGroupIdName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "jhy");
                mainRootItem.Add("fieldValue", PlannerIDName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "wtzz");
                mainRootItem.Add("fieldValue", ENTrustOrgIdName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xm");
                mainRootItem.Add("fieldValue", F_SRT_ProjectName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xszxm");
                mainRootItem.Add("fieldValue", F_PYEO__SRT_XSZXMName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "yfzxm");
                mainRootItem.Add("fieldValue", F_SRT_SCZXMName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "kh");
                mainRootItem.Add("fieldValue", F_SRT_CustIdName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "zdkh");
                mainRootItem.Add("fieldValue", F_SRT_ZDKHName);
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
                mainRootItem.Add("fieldName", "xsbm");
                mainRootItem.Add("fieldValue", F_PYEO_SaleDeptIdName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "mylx");
                mainRootItem.Add("fieldValue", F_SRT_MYLXName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xslx");
                mainRootItem.Add("fieldValue", F_SRT_XSLXName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "td");
                mainRootItem.Add("fieldValue", F_SRT_TDName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "cpfl");
                mainRootItem.Add("fieldValue", F_SRT_CPFLName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "ydlx");
                mainRootItem.Add("fieldValue", F_PYEO_SourceBillType);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "ydbh");
                mainRootItem.Add("fieldValue", F_PYEO_SourceBillNo);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "bz");
                mainRootItem.Add("fieldValue", Description);
                mainRoot.Add(mainRootItem);

                JSONArray detailData = new JSONArray();

                JSONObject detailDataItem = new JSONObject();
                detailDataItem.Add("tableDBName", "formtable_main_75_dt1");
                JSONArray workflowRequestTableRecords = new JSONArray();

                DynamicObjectCollection BillEntrys = o["TreeEntity"] as DynamicObjectCollection;
                foreach (DynamicObject BillEntry in BillEntrys)
                {
                    var field = this.BusinessInfo.GetField("FProductType") as ComboField; ;
                    var enumValue = BillEntry["ProductType"];
                    var ProductType = field.GetEnumItemName(enumValue);

                    DynamicObject MaterialId = BillEntry["MaterialId"] as DynamicObject;
                    string MaterialNumber = MaterialId == null ? "" : Convert.ToString(MaterialId["Number"]);
                    string MaterialName = MaterialId == null ? "" : Convert.ToString(MaterialId["Name"]);
                    string Specification = MaterialId == null ? "" : Convert.ToString(MaterialId["Specification"]);

                    DynamicObject WorkShopID = BillEntry["WorkShopID"] as DynamicObject;
                    string WorkShopIDNumber = WorkShopID == null ? "" : Convert.ToString(WorkShopID["Name"]);
                    DynamicObject UnitId = BillEntry["UnitId"] as DynamicObject;
                    string UnitIdNumber = UnitId == null ? "" : Convert.ToString(UnitId["Name"]);
                    string Qty = Convert.ToDecimal(BillEntry["Qty"]).ToString("#0.00");
                    field = this.BusinessInfo.GetField("FStatus") as ComboField; ;
                    enumValue = BillEntry["Status"];
                    var Status = field.GetEnumItemName(enumValue);
                    string PlanStartDate = Convert.ToDateTime(BillEntry["PlanStartDate"]).ToString("yyyy-MM-dd");
                    string PlanFinishDate = Convert.ToDateTime(BillEntry["PlanFinishDate"]).ToString("yyyy-MM-dd");
                    DynamicObject BomId = BillEntry["BomId"] as DynamicObject;
                    string BomIdNumber = BomId == null ? "" : Convert.ToString(BomId["Name"]);
                    string ISBACKFLUSH = Convert.ToString(BillEntry["ISBACKFLUSH"]).Equals("True") ? "是" : "否";
                    field = this.BusinessInfo.GetField("FPickMtrlStatus") as ComboField; ;
                    enumValue = BillEntry["PickMtrlStatus"];
                    var PickMtrlStatus = field.GetEnumItemName(enumValue);
                    DynamicObject F_SRT_MXCPFL = BillEntry["F_SRT_MXCPFL"] as DynamicObject;
                    string F_SRT_MXCPFLNumber = F_SRT_MXCPFL == null ? "" : Convert.ToString(F_SRT_MXCPFL["Name"]);
                    DynamicObject F_SRT_CHDL = BillEntry["F_SRT_CHDL"] as DynamicObject;
                    string F_SRT_CHDLNumber = F_SRT_CHDL == null ? "" : Convert.ToString(F_SRT_CHDL["Name"]);
                    string ISENABLESCHEDULE = Convert.ToString(BillEntry["ISENABLESCHEDULE"]).Equals("True") ? "是" : "否";
                    string ISMRPCAL = Convert.ToString(BillEntry["ISMRPCAL"]).Equals("True") ? "是" : "否";

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
                    workflowRequestTableFieldsItem.Add("fieldName", "cplx");
                    workflowRequestTableFieldsItem.Add("fieldValue", ProductType);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "sccj");
                    workflowRequestTableFieldsItem.Add("fieldValue", WorkShopIDNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "dw");
                    workflowRequestTableFieldsItem.Add("fieldValue", UnitIdNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "sl");
                    workflowRequestTableFieldsItem.Add("fieldValue", Qty);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "ywzt");
                    workflowRequestTableFieldsItem.Add("fieldValue", Status);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jhkgsj");
                    workflowRequestTableFieldsItem.Add("fieldValue", PlanStartDate);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jhwgsj");
                    workflowRequestTableFieldsItem.Add("fieldValue", PlanFinishDate);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "bombb");
                    workflowRequestTableFieldsItem.Add("fieldValue", BomIdNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "dcll");
                    workflowRequestTableFieldsItem.Add("fieldValue", ISBACKFLUSH);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "llzt");
                    workflowRequestTableFieldsItem.Add("fieldValue", PickMtrlStatus);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "mxcpfl");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_SRT_MXCPFLNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);
                    
                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "chdl");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_SRT_CHDLNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "qyrpc");
                    workflowRequestTableFieldsItem.Add("fieldValue", ISENABLESCHEDULE);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "yyl");
                    workflowRequestTableFieldsItem.Add("fieldValue", ISMRPCAL);
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
                        + "&requestName=生产订单已到达"
                        + "&workflowId=29&detailData=" + detailData.ToString(),personId);

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
                string queryErpIdSql = string.Format(@"select FNAME from T_BAS_BILLTYPE_L where FBILLTYPEID = '{0}' ", erpId);
                return DBUtils.ExecuteDynamicObject(this.Context, queryErpIdSql)[0]["FNAME"].ToString();
                
            }
            catch (Exception e)
            {
                return "";
            }
        }
    }
}
