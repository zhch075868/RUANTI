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
    [Kingdee.BOS.Util.HotUpdate, Description("预测单推送至OA")]
    public class ForecastPush : AbstractOperationServicePlugIn
    {public override void OnPreparePropertys(PreparePropertysEventArgs e)
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
                DynamicObject BillType = o["BillTypeID"] as DynamicObject;
                string BillTypeName = BillType == null ? "" : Convert.ToString(BillType["Name"]);
                string Date = Convert.ToDateTime(o["Date"]).ToString("yyyy-MM-dd");
                DynamicObject ForeOrgId = o["ForeOrgId"] as DynamicObject;
                string ForeOrgIdNumber = ForeOrgId == null ? "" : Convert.ToString(ForeOrgId["F_PYEO_Text_OAID"]);
                var field = this.BusinessInfo.GetField("F_SRT_YCLX") as ComboField; ;
                var enumValue = o["F_SRT_YCLX"];
                var F_SRT_YCLX = field.GetEnumItemName(enumValue);
                DynamicObject F_PYEO__SRT_Project = o["F_PYEO__SRT_Project"] as DynamicObject;
                string F_PYEO__SRT_ProjectName = F_PYEO__SRT_Project == null ? "" : Convert.ToString(F_PYEO__SRT_Project["Name"]);
                DynamicObject F_PYEO__SRT_SCZXM = o["F_PYEO__SRT_SCZXM"] as DynamicObject;
                string F_PYEO__SRT_SCZXMName = F_PYEO__SRT_SCZXM == null ? "" : Convert.ToString(F_PYEO__SRT_SCZXM["Name"]);
                DynamicObject F_SRT_XSZXM = o["F_SRT_XSZXM"] as DynamicObject;
                string F_SRT_XSZXMName = F_SRT_XSZXM == null ? "" : Convert.ToString(F_SRT_XSZXM["Name"]);
                DynamicObject F_SRT_CustId = o["F_SRT_CustId"] as DynamicObject;
                string F_SRT_CustIdName = F_SRT_CustId == null ? "" : Convert.ToString(F_SRT_CustId["Name"]);
                DynamicObject F_SRT_ZDKH = o["F_SRT_ZDKH"] as DynamicObject;
                string F_SRT_ZDKHName = F_SRT_ZDKH == null ? "" : Convert.ToString(F_SRT_ZDKH["Name"]);
                DynamicObject F_SRT_DQ = o["F_SRT_DQ"] as DynamicObject;
                string F_SRT_DQName = F_SRT_DQ == null ? "" : Convert.ToString(F_SRT_DQ["FDataValue"]);
                DynamicObject F_SRT_HY = o["F_SRT_HY"] as DynamicObject;
                string F_SRT_HYName = F_SRT_HY == null ? "" : Convert.ToString(F_SRT_HY["FDataValue"]);
                DynamicObject F_SaleDeptId = o["F_SaleDeptId"] as DynamicObject;
                string F_SaleDeptIdName = F_SaleDeptId == null ? "" : Convert.ToString(F_SaleDeptId["F_PYEO_Text_OAID"]);
                DynamicObject F_SRT_MYLX = o["F_SRT_MYLX"] as DynamicObject;
                string F_SRT_MYLXName = F_SRT_MYLX == null ? "" : Convert.ToString(F_SRT_MYLX["Name"]);
                DynamicObject F_SRT_XSLX = o["F_SRT_XSLX"] as DynamicObject;
                string F_SRT_XSLXName = F_SRT_XSLX == null ? "" : Convert.ToString(F_SRT_XSLX["Name"]);
                DynamicObject F_SRT_TD = o["F_SRT_TD"] as DynamicObject;
                string F_SRT_TDName = F_SRT_TD == null ? "" : Convert.ToString(F_SRT_TD["Name"]);
                DynamicObject F_SRT_CPFL = o["F_SRT_CPFL"] as DynamicObject;
                string F_SRT_CPFLName = F_SRT_CPFL == null ? "" : Convert.ToString(F_SRT_CPFL["Name"]);
                DynamicObject F_SRT_XSQD = o["F_SRT_XSQD"] as DynamicObject;
                string F_SRT_XSQDName = F_SRT_XSQD == null ? "" : Convert.ToString(F_SRT_XSQD["Name"]);

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
                mainRootItem.Add("fieldName", "yczz");
                mainRootItem.Add("fieldValue", ForeOrgIdNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "yclx");
                mainRootItem.Add("fieldValue", F_SRT_YCLX);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xm");
                mainRootItem.Add("fieldValue", F_PYEO__SRT_ProjectName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "yfzxm");
                mainRootItem.Add("fieldValue", F_PYEO__SRT_SCZXMName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xszxm");
                mainRootItem.Add("fieldValue", F_SRT_XSZXMName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "rq");
                mainRootItem.Add("fieldValue", Date);
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
                mainRootItem.Add("fieldValue", F_SaleDeptIdName);
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
                mainRootItem.Add("fieldName", "xsqd");
                mainRootItem.Add("fieldValue", F_SRT_XSQDName);
                mainRoot.Add(mainRootItem);

                JSONArray detailData = new JSONArray();

                JSONObject detailDataItem = new JSONObject();
                detailDataItem.Add("tableDBName", "formtable_main_72_dt1");
                JSONArray workflowRequestTableRecords = new JSONArray();

                DynamicObjectCollection materials = o["PLN_FORECASTENTRY"] as DynamicObjectCollection;
                foreach (DynamicObject BillEntry in materials)
                {
                    DynamicObject SupplyOrgId = BillEntry["SupplyOrgId"] as DynamicObject;
                    string SupplyOrgIdNumber = SupplyOrgId == null ? "" : Convert.ToString(SupplyOrgId["F_PYEO_Text_OAID"]);
                    DynamicObject CustID = BillEntry["CustID"] as DynamicObject;
                    string CustIDName = CustID == null ? "" : Convert.ToString(CustID["Name"]);
                    field = this.BusinessInfo.GetField("FProductType") as ComboField; ;
                    enumValue = BillEntry["ProductType"];
                    var ProductType = field.GetEnumItemName(enumValue);

                    DynamicObject MaterialId = BillEntry["MaterialId"] as DynamicObject;
                    string MaterialNumber = MaterialId == null ? "" : Convert.ToString(MaterialId["Number"]);
                    string MaterialName = MaterialId == null ? "" : Convert.ToString(MaterialId["Name"]);
                    string Specification = MaterialId == null ? "" : Convert.ToString(MaterialId["Specification"]);
                    DynamicObject MapId = BillEntry["MapId"] as DynamicObject;
                    string MapIdNumber = MapId == null ? "" : Convert.ToString(MapId["Number"]);
                    string MapIdName = MapId == null ? "" : Convert.ToString(MapId["Name"]);
                    DynamicObject BomID = BillEntry["BomID"] as DynamicObject;
                    string BomIDName = BomID == null ? "" : Convert.ToString(BomID["Name"]);
                    DynamicObject UnitId = BillEntry["UnitId"] as DynamicObject;
                    string UnitIdNumber = UnitId == null ? "" : Convert.ToString(UnitId["Name"]);
                    string Qty = Convert.ToDecimal(BillEntry["Qty"]).ToString("#0.00");
                    DynamicObject ParentMtrlId = BillEntry["ParentMtrlId"] as DynamicObject;
                    string ParentMtrlIdName = ParentMtrlId == null ? "" : Convert.ToString(ParentMtrlId["Name"]);
                    string StartDate = Convert.ToDateTime(BillEntry["StartDate"]).ToString("yyyy-MM-dd");
                    string EndDate = Convert.ToDateTime(BillEntry["EndDate"]).ToString("yyyy-MM-dd");
                    field = this.BusinessInfo.GetField("FAveraType") as ComboField; ;
                    enumValue = BillEntry["AveraType"];
                    var AveraType = field.GetEnumItemName(enumValue);
                    string AveraCycle = Convert.ToString(BillEntry["AveraCycle"]);
                    DynamicObject StockOrgId = BillEntry["StockOrgId"] as DynamicObject;
                    string StockOrgIdNumber = StockOrgId == null ? "" : Convert.ToString(StockOrgId["F_PYEO_Text_OAID"]);
                    field = this.BusinessInfo.GetField("FOwnerTypeId") as ComboField; ;
                    enumValue = BillEntry["OwnerTypeId"];
                    string OwnerTypeId = field.GetEnumItemName(enumValue);
                    DynamicObject OwnerId = BillEntry["OwnerId"] as DynamicObject;
                    string OwnerIdName = OwnerId == null ? "" : Convert.ToString(OwnerId["Name"]);
                    field = this.BusinessInfo.GetField("FReserveType") as ComboField; ;
                    enumValue = BillEntry["ReserveType"];
                    var ReserveType = field.GetEnumItemName(enumValue);
                    string Priority = Convert.ToString(BillEntry["Priority"]);
                    DynamicObject F_SRT_MXCPFL = BillEntry["F_SRT_MXCPFL"] as DynamicObject;
                    string F_SRT_MXCPFLName = F_SRT_MXCPFL == null ? "" : Convert.ToString(F_SRT_MXCPFL["Name"]);
                    DynamicObject F_SRT_CHDL = BillEntry["F_SRT_CHDL"] as DynamicObject;
                    string F_SRT_CHDLName = F_SRT_CHDL == null ? "" : Convert.ToString(F_SRT_CHDL["Name"]);
                    string Description = Convert.ToString(BillEntry["Remark"]);

                    JSONObject workflowRequestTableRecordsItem = new JSONObject();
                    workflowRequestTableRecordsItem.Add("recordOrder", "0");
                    JSONArray workflowRequestTableFields = new JSONArray();


                    JSONObject workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "wlbm");
                    workflowRequestTableFieldsItem.Add("fieldValue", MaterialNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "gyzz");
                    workflowRequestTableFieldsItem.Add("fieldValue", SupplyOrgIdNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "khmc");
                    workflowRequestTableFieldsItem.Add("fieldValue", CustIDName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "cplx");
                    workflowRequestTableFieldsItem.Add("fieldValue", ProductType);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "wlbm");
                    workflowRequestTableFieldsItem.Add("fieldValue", MaterialNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "khwlbm");
                    workflowRequestTableFieldsItem.Add("fieldValue", MapIdNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "wlmc");
                    workflowRequestTableFieldsItem.Add("fieldValue", MaterialName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "khwlmc");
                    workflowRequestTableFieldsItem.Add("fieldValue", MapIdName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "ggxh");
                    workflowRequestTableFieldsItem.Add("fieldValue", Specification);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "bombb");
                    workflowRequestTableFieldsItem.Add("fieldValue", BomIDName);
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
                    workflowRequestTableFieldsItem.Add("fieldName", "fxwl");
                    workflowRequestTableFieldsItem.Add("fieldValue", ParentMtrlIdName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "ycksrq");
                    workflowRequestTableFieldsItem.Add("fieldValue", StartDate);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "ycjsrq");
                    workflowRequestTableFieldsItem.Add("fieldValue", EndDate);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jhlx");
                    workflowRequestTableFieldsItem.Add("fieldValue", AveraType);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jhzq");
                    workflowRequestTableFieldsItem.Add("fieldValue", AveraCycle);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "kczz");
                    workflowRequestTableFieldsItem.Add("fieldValue", StockOrgIdNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "hzlx");
                    workflowRequestTableFieldsItem.Add("fieldValue", OwnerTypeId);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "hz");
                    workflowRequestTableFieldsItem.Add("fieldValue", OwnerIdName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "yllx");
                    workflowRequestTableFieldsItem.Add("fieldValue", ReserveType);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xqyxj");
                    workflowRequestTableFieldsItem.Add("fieldValue", Priority);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "mxcpfl");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_SRT_MXCPFLName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "chdl");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_SRT_CHDLName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "bz");
                    workflowRequestTableFieldsItem.Add("fieldValue", Description);
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
                        + "&requestName=预测单已到达"
                        + "&workflowId=22&detailData=" + detailData.ToString(), personId);

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
