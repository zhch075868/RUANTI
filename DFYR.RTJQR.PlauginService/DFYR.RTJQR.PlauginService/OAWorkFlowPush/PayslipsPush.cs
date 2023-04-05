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
    [Kingdee.BOS.Util.HotUpdate, Description("工资发放单推送至OA")]
    public class PayslipsPush : AbstractOperationServicePlugIn
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
                string Date = Convert.ToDateTime(o["Date"]).ToString("yyyy-MM-dd");
                DynamicObject ForeOrgId = o["ForeOrgId"] as DynamicObject;
                string ForeOrgIdName = ForeOrgId == null ? "" : Convert.ToString(ForeOrgId["Number"]);
                DynamicObject F_PYEO_BM = o["F_PYEO_BM"] as DynamicObject;
                string F_PYEO_BMName = F_PYEO_BM == null ? "" : Convert.ToString(F_PYEO_BM["Number"]);
                DynamicObject F_PYEO_YGXM = o["F_PYEO_YGXM"] as DynamicObject;
                string F_PYEO_YGXMName = F_PYEO_YGXM == null ? "" : Convert.ToString(F_PYEO_YGXM["Number"]);
                DynamicObject F_PYEO_Base = o["F_PYEO_Base"] as DynamicObject;
                string F_PYEO_BaseName = F_PYEO_Base == null ? "" : Convert.ToString(F_PYEO_Base["Number"]);

                JSONArray mainRoot = new JSONArray();
                JSONObject mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "djbh");
                mainRootItem.Add("fieldValue", BillNo);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "rq");
                mainRootItem.Add("fieldValue", Date);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "ffzz");
                mainRootItem.Add("fieldValue", ForeOrgIdName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "bm");
                mainRootItem.Add("fieldValue", F_PYEO_BMName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "yg");
                mainRootItem.Add("fieldValue", F_PYEO_YGXMName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "bmfzr");
                mainRootItem.Add("fieldValue", F_PYEO_BaseName);
                mainRoot.Add(mainRootItem);

                JSONArray detailData = new JSONArray();

                JSONObject detailDataItem = new JSONObject();
                detailDataItem.Add("tableDBName", "formtable_main_82_dt1");
                JSONArray workflowRequestTableRecords = new JSONArray();

                DynamicObjectCollection materials = o["PLN_FORECASTENTRY"] as DynamicObjectCollection;
                foreach (DynamicObject entry in materials)
                {
                    string F_PYEO_SCDDLX = Convert.ToString(entry["F_PYEO_SCDDLX"]);
                    DynamicObject F_PYEO_Project1 = entry["F_PYEO_Project1"] as DynamicObject;
                    string F_PYEO_Project1Number = F_PYEO_Project1 == null ? "" : Convert.ToString(F_PYEO_Project1["Number"]);
                    string F_PYEO_JNGS = Convert.ToString(entry["F_PYEO_JNGS"]);
                    string F_PYEO_BL = Convert.ToDecimal(entry["F_PYEO_BL"]).ToString("#0.00");
                    string F_PYEO_SQGZ = Convert.ToDecimal(entry["F_PYEO_SQGZ"]).ToString("#0.00");
                    string F_PYEO_GRYLBX = Convert.ToDecimal(entry["F_PYEO_GRYLBX"]).ToString("#0.00");
                    string F_PYEO_GEYLBX1 = Convert.ToDecimal(entry["F_PYEO_GEYLBX1"]).ToString("#0.00");
                    string F_PYEO_GESYBX = Convert.ToDecimal(entry["F_PYEO_GESYBX"]).ToString("#0.00");
                    string F_PYEO_GRGJJ = Convert.ToDecimal(entry["F_PYEO_GRGJJ"]).ToString("#0.00");
                    string F_PYEO_GSYLBX = Convert.ToDecimal(entry["F_PYEO_GSYLBX"]).ToString("#0.00");
                    string F_PYEO_GSYLBX1 = Convert.ToDecimal(entry["F_PYEO_GSYLBX1"]).ToString("#0.00");

                    string F_PYEO_GSSYBX = Convert.ToDecimal(entry["F_PYEO_GSSYBX"]).ToString("#0.00");
                    string F_PYEO_GSGSBX = Convert.ToDecimal(entry["F_PYEO_GSGSBX"]).ToString("#0.00");
                    string F_PYEO_GSGJJ = Convert.ToDecimal(entry["F_PYEO_GSGJJ"]).ToString("#0.00");
                    string F_PYEO_GS = Convert.ToDecimal(entry["F_PYEO_GS"]).ToString("#0.00");
                    string F_PYEO_HBGS = Convert.ToDecimal(entry["F_PYEO_HBGS"]).ToString("#0.00");
                    string F_PYEO_ZGS = Convert.ToDecimal(entry["F_PYEO_ZGS"]).ToString("#0.00");

                    JSONObject workflowRequestTableRecordsItem = new JSONObject();
                    workflowRequestTableRecordsItem.Add("recordOrder", "0");
                    JSONArray workflowRequestTableFields = new JSONArray();


                    JSONObject workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "scddlx");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_PYEO_SCDDLX);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xm");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_PYEO_Project1Number);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jngs");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_PYEO_JNGS);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "bl");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_PYEO_BL);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "sqgz");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_PYEO_SQGZ);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "ylbxgr");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_PYEO_GRYLBX);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "ylbxgrr");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_PYEO_GEYLBX1);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "sybxgr");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_PYEO_GESYBX);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "gjjgr");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_PYEO_GRGJJ);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "ylbxgs");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_PYEO_GSYLBX);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "ylbxgss");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_PYEO_GSYLBX1);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "sybxgs");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_PYEO_GSSYBX);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "gsbxg");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_PYEO_GSGSBX);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "gjjgs");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_PYEO_GSGJJ);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "gs");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_PYEO_GS);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "hbgs");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_PYEO_HBGS);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "zgs");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_PYEO_ZGS);
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
                        + "&requestName=工资发放单已到达"
                        + "&workflowId=46&detailData=" + detailData.ToString(), personId);

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
