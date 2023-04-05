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
    [Kingdee.BOS.Util.HotUpdate, Description("推送销售出库单至OA")]
    public class SalOutStockPush : AbstractOperationServicePlugIn
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
                string date = Convert.ToDateTime(o["Date"]).ToString("yyyy-MM-dd");
                DynamicObject SaleOrgId = o["SaleOrgId"] as DynamicObject;
                string SaleOrgIdNumber = SaleOrgId == null ? "" : Convert.ToString(SaleOrgId["F_PYEO_Text_OAID"]);
                DynamicObject CustomerID = o["CustomerID"] as DynamicObject;
                string CustomerIDNumber = CustomerID == null ? "" : Convert.ToString(CustomerID["Name"]);
                DynamicObject SaleDeptID = o["SaleDeptID"] as DynamicObject;
                string SaleDeptIDNumber = SaleDeptID == null ? "" : Convert.ToString(SaleDeptID["F_PYEO_Text_OAID"]);
                DynamicObject SalesGroupID = o["SalesGroupID"] as DynamicObject;
                string SalesGroupIDNumber = SalesGroupID == null ? "" : Convert.ToString(SalesGroupID["Name"]);
                DynamicObject SalesManID = o["SalesManID"] as DynamicObject;
                string SalesManIDName = Convert.ToString(SalesManID["EmpNumber"]);
                SalesManIDName = Utils.getPersonOAid(this.Context, SalesManIDName);
                DynamicObject F_SRT_Project = o["F_SRT_Project"] as DynamicObject;
                string F_SRT_ProjectNumber = F_SRT_Project == null ? "" : Convert.ToString(F_SRT_Project["Name"]);
                DynamicObject F_SRT_HTH = o["F_SRT_HTH"] as DynamicObject;
                string F_SRT_HTHNumber = F_SRT_HTH == null ? "" : Convert.ToString(F_SRT_HTH["Number"]);
                DynamicObject F_SRT_ZDKH = o["F_SRT_ZDKH"] as DynamicObject;
                string F_SRT_ZDKHNumber = F_SRT_ZDKH == null ? "" : Convert.ToString(F_SRT_ZDKH["Name"]);

                JSONArray mainRoot = new JSONArray();
                JSONObject mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "djbh");
                mainRootItem.Add("fieldValue", billNo);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "rq");
                mainRootItem.Add("fieldValue", date);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xszz");
                mainRootItem.Add("fieldValue", SaleOrgIdNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "kh");
                mainRootItem.Add("fieldValue", CustomerIDNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xsbm");
                mainRootItem.Add("fieldValue", SaleDeptIDNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xsz");
                mainRootItem.Add("fieldValue", SalesGroupIDNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xsy");
                mainRootItem.Add("fieldValue", SalesManIDName);
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
                mainRootItem.Add("fieldName", "zdkh");
                mainRootItem.Add("fieldValue", F_SRT_ZDKHNumber);
                mainRoot.Add(mainRootItem);

                JSONArray detailData = new JSONArray();

                JSONObject detailDataItem = new JSONObject();
                detailDataItem.Add("tableDBName", "formtable_main_82_dt1");
                JSONArray workflowRequestTableRecords = new JSONArray();

                DynamicObjectCollection Entrys = o["SAL_OUTSTOCKENTRY"] as DynamicObjectCollection;
                foreach (DynamicObject Entry in Entrys)
                {
                    DynamicObject MaterialId = Entry["MaterialId"] as DynamicObject;
                    string MaterialNumber = MaterialId == null ? "" : Convert.ToString(MaterialId["Number"]);
                    string MaterialName = MaterialId == null ? "" : Convert.ToString(MaterialId["Name"]);
                    string Specification = MaterialId == null ? "" : Convert.ToString(MaterialId["Specification"]);
                    string RealQty = Convert.ToDecimal(Entry["RealQty"]).ToString("#0.00");

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
                    workflowRequestTableFieldsItem.Add("fieldName", "sfsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", RealQty);
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
                    + "&requestName=销售出库已到达"
                    + "&workflowId=36&detailData=" + detailData.ToString(), personId);

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
