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
    [Kingdee.BOS.Util.HotUpdate, Description("推送付款单至OA")]
    public class PayBillPush : AbstractOperationServicePlugIn
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

                string date = Convert.ToDateTime(o["DATE"]).ToString("yyyy-MM-dd");
                DynamicObject CONTACTUNIT = o["CONTACTUNIT"] as DynamicObject;
                string CONTACTUNITName = CONTACTUNIT == null ? "" : Convert.ToString(CONTACTUNIT["Name"]);
                DynamicObject PURCHASEORGID = o["PURCHASEORGID"] as DynamicObject;
                string PURCHASEORGIDNumber = PURCHASEORGID == null ? "" : Convert.ToString(PURCHASEORGID["F_PYEO_Text_OAID"]);
                DynamicObject PURCHASEDEPTID = o["PURCHASEDEPTID"] as DynamicObject;
                string PURCHASEDEPTIDNumber = PURCHASEDEPTID == null ? "" : Convert.ToString(PURCHASEDEPTID["F_PYEO_Text_OAID"]);
                DynamicObject PURCHASERGROUPID = o["PURCHASERGROUPID"] as DynamicObject;
                string PURCHASERGROUPIDName = PURCHASERGROUPID == null ? "" : Convert.ToString(PURCHASERGROUPID["Name"]);
                DynamicObject PURCHASERID = o["PURCHASERID"] as DynamicObject;
                string PURCHASERIDName = PURCHASERID == null ? "" : Convert.ToString(PURCHASERID["Number"]);
                PURCHASERIDName = Utils.getPersonOAid(this.Context, PURCHASERIDName);
                DynamicObject F_PYEO_ContractNo2 = o["F_PYEO_ContractNo2"] as DynamicObject;
                string F_PYEO_ContractNo2Name = F_PYEO_ContractNo2 == null ? "" : Convert.ToString(F_PYEO_ContractNo2["Number"]);
                string PAYAMOUNTFOR = Convert.ToDecimal(o["PAYAMOUNTFOR"]).ToString("#0.00");

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
                mainRootItem.Add("fieldName", "ywrq");
                mainRootItem.Add("fieldValue", date);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "wldw");
                mainRootItem.Add("fieldValue", CONTACTUNITName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "cgzz");
                mainRootItem.Add("fieldValue", PURCHASEORGIDNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "cgbm");
                mainRootItem.Add("fieldValue", PURCHASEDEPTIDNumber);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "cgz");
                mainRootItem.Add("fieldValue", PURCHASERGROUPIDName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "cgy");
                mainRootItem.Add("fieldValue", PURCHASERIDName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "ht");
                mainRootItem.Add("fieldValue", F_PYEO_ContractNo2Name);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "fkje");
                mainRootItem.Add("fieldValue", PAYAMOUNTFOR);
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
                        + "&requestName=付款单已到达"
                        + "&workflowId=34", personId);

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
