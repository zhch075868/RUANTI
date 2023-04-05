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
    [Kingdee.BOS.Util.HotUpdate, Description("计划订单推送至OA")]
    public class PlanorderPush : AbstractOperationServicePlugIn
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
                DynamicObject DemandOrgId = o["DemandOrgId"] as DynamicObject;
                string DemandOrgIdName = DemandOrgId == null ? "" : Convert.ToString(DemandOrgId["F_PYEO_Text_OAID"]);
                DynamicObject MaterialId = o["MaterialId"] as DynamicObject;
                string MaterialNumber = MaterialId == null ? "" : Convert.ToString(MaterialId["Number"]);
                string MaterialName = MaterialId == null ? "" : Convert.ToString(MaterialId["Name"]);
                string Specification = MaterialId == null ? "" : Convert.ToString(MaterialId["Specification"]);
                var field = this.BusinessInfo.GetField("FReleaseType") as ComboField; ;
                var enumValue = o["ReleaseType"];
                var ReleaseType = field.GetEnumItemName(enumValue);
                DynamicObject BomId = o["BomId"] as DynamicObject;
                string BomIdName = BomId == null ? "" : Convert.ToString(BomId["Name"]);
                string SugQty = Convert.ToDecimal(o["SugQty"]).ToString("#0.00");
                string PlanStartDate = Convert.ToDateTime(o["PlanStartDate"]).ToString("yyyy-MM-dd");
                string PlanFinishDate = Convert.ToDateTime(o["PlanFinishDate"]).ToString("yyyy-MM-dd");
                string FirmQty = Convert.ToDecimal(o["FirmQty"]).ToString("#0.00");
                string FirmStartDate = Convert.ToDateTime(o["FirmStartDate"]).ToString("yyyy-MM-dd");
                string FirmFinishDate = Convert.ToDateTime(o["FirmFinishDate"]).ToString("yyyy-MM-dd");
                DynamicObject PlanerId = o["PlanerId"] as DynamicObject;
                string PlanerIdNumber = PlanerId == null ? "" : Convert.ToString(PlanerId["Number"]);
                PlanerIdNumber = Utils.getPersonOAid(this.Context,PlanerIdNumber);
                DynamicObject F_SRT_CustId = o["F_SRT_CustId"] as DynamicObject;
                string F_SRT_CustIdName = F_SRT_CustId == null ? "" : Convert.ToString(F_SRT_CustId["Name"]);
                DynamicObject F_SRT_HY = o["F_SRT_HY"] as DynamicObject;
                string F_SRT_HYName = F_SRT_HY == null ? "" : Convert.ToString(F_SRT_HY["FDataValue"]);
                DynamicObject F_SRT_Project = o["F_SRT_Project"] as DynamicObject;
                string F_SRT_ProjectName = F_SRT_Project == null ? "" : Convert.ToString(F_SRT_Project["Name"]);
                DynamicObject F_PYEO_SaleDeptId = o["F_PYEO_SaleDeptId"] as DynamicObject;
                string F_PYEO_SaleDeptIdNumebr = F_PYEO_SaleDeptId == null ? "" : Convert.ToString(F_PYEO_SaleDeptId["F_PYEO_Text_OAID"]);
                DynamicObject F_SRT_DQ = o["F_SRT_DQ"] as DynamicObject;
                string F_SRT_DQName = F_SRT_DQ == null ? "" : Convert.ToString(F_SRT_DQ["FDataValue"]);
                DynamicObject F_SRT_TD = o["F_SRT_TD"] as DynamicObject;
                string F_SRT_TDName = F_SRT_TD == null ? "" : Convert.ToString(F_SRT_TD["Name"]);
                DynamicObject F_SRT_XSLX = o["F_SRT_XSLX"] as DynamicObject;
                string F_SRT_XSLXName = F_SRT_XSLX == null ? "" : Convert.ToString(F_SRT_XSLX["Name"]);
                DynamicObject F_SRT_MYLX = o["F_SRT_MYLX"] as DynamicObject;
                string F_SRT_MYLXName = F_SRT_MYLX == null ? "" : Convert.ToString(F_SRT_MYLX["Name"]);
                DynamicObject F_SRT_XSQD = o["F_SRT_XSQD"] as DynamicObject;
                string F_SRT_XSQDName = F_SRT_XSQD == null ? "" : Convert.ToString(F_SRT_XSQD["Name"]);
                DynamicObject F_PYEO_Assistant_HYYJ = o["F_PYEO_Assistant_HYYJ"] as DynamicObject;
                string F_PYEO_Assistant_HYYJName = F_PYEO_Assistant_HYYJ == null ? "" : Convert.ToString(F_PYEO_Assistant_HYYJ["FDataValue"]);
                DynamicObject F_PYEO_Assistant_HYEJ = o["F_PYEO_Assistant_HYEJ"] as DynamicObject;
                string F_PYEO_Assistant_HYEJName = F_PYEO_Assistant_HYEJ == null ? "" : Convert.ToString(F_PYEO_Assistant_HYEJ["FDataValue"]);

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
                mainRootItem.Add("fieldName", "xqzz");
                mainRootItem.Add("fieldValue", DemandOrgIdName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "wlbm");
                mainRootItem.Add("fieldValue", MaterialNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "wlmc");
                mainRootItem.Add("fieldValue", MaterialName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "ggxh");
                mainRootItem.Add("fieldValue", Specification);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "tflx");
                mainRootItem.Add("fieldValue", ReleaseType);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "bombb");
                mainRootItem.Add("fieldValue", BomIdName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "jyddl");
                mainRootItem.Add("fieldValue", SugQty);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "jycgscrq");
                mainRootItem.Add("fieldValue", PlanStartDate);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "jydhwgrq");
                mainRootItem.Add("fieldValue", PlanFinishDate);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "qrddl");
                mainRootItem.Add("fieldValue", FirmQty);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "qrcgscrq");
                mainRootItem.Add("fieldValue", FirmStartDate);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "qrdhwgrq");
                mainRootItem.Add("fieldValue", FirmFinishDate);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "jhy");
                mainRootItem.Add("fieldValue", PlanerIdNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "kh");
                mainRootItem.Add("fieldValue", F_SRT_CustIdName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xy");
                mainRootItem.Add("fieldValue", F_SRT_HYName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xm");
                mainRootItem.Add("fieldValue", F_SRT_ProjectName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xsbm");
                mainRootItem.Add("fieldValue", F_PYEO_SaleDeptIdNumebr);
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
                mainRootItem.Add("fieldName", "xslx");
                mainRootItem.Add("fieldValue", F_SRT_XSLXName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "mylx");
                mainRootItem.Add("fieldValue", F_SRT_MYLXName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xsqd");
                mainRootItem.Add("fieldValue", F_SRT_XSQDName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xyyj");
                mainRootItem.Add("fieldValue", F_PYEO_Assistant_HYYJName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xyej");
                mainRootItem.Add("fieldValue", F_PYEO_Assistant_HYEJName);
                mainRoot.Add(mainRootItem);

                DynamicObject userObject = Utils.GetUser(this.Context, Convert.ToString(this.Context.UserId));//当前用户信息
                if (userObject == null)
                {
                    throw new KDException("", "当前用户未绑定员工，无法推送OA");
                }
                string personId = Convert.ToString((userObject["FLinkObject"] as DynamicObject)["Number"]);
                personId = Utils.getPersonOAid(this.Context, personId);
                string resultStr = Utils.wkPostUrl(Utils.pushAddWF,
                        "mainData=" + mainRoot.ToString()
                        + "&requestName=计划订单已到达"
                        + "&workflowId=37", personId);

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

