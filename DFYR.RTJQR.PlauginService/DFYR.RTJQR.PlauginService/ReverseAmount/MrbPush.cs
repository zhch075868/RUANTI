using Kingdee.BOS;
using Kingdee.BOS.App.Data;
using Kingdee.BOS.Core.DynamicForm;
using Kingdee.BOS.Core.DynamicForm.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Core.Metadata.FieldElement;
using Kingdee.BOS.JSON;
using Kingdee.BOS.Orm.DataEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFYR.RTJQR.PlauginService.ReverseAmount
{
    [Kingdee.BOS.Util.HotUpdate, Description("采购退料单反写OA采购合同")]
    public class MrbPush : AbstractOperationServicePlugIn
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
            foreach (DynamicObject o in e.DataEntitys)
            {
                string id = Convert.ToString(o["Id"]);
                string queryPurOrderNoSql = string.Format(@"select e.FORDERNO as FORDERNO from T_PUR_MRB t 
                                                left join T_PUR_MRBENTRY e on t.FID = e.FID 
                                                where t.FID = '{0}'
                                                group by e.FORDERNO", id);
                DynamicObjectCollection purOrderNoResult = DBUtils.ExecuteDynamicObject(this.Context, queryPurOrderNoSql);
                foreach (DynamicObject purOrderNo in purOrderNoResult)
                {
                    string purNo = Convert.ToString(purOrderNo["FORDERNO"]);
                    JSONObject pushjson = new JSONObject();
                    JSONObject dataJson = new JSONObject();

                    JSONArray data = new JSONArray();
                    JSONObject dateItem = new JSONObject();
                    JSONObject operationinfo = new JSONObject();
                    JSONObject mainTable = new JSONObject();

                    mainTable.Add("erpnumber", purNo);
                    string queryAmountSql = string.Format(@"select e.FPOORDERENTRYID as FPOORDERENTRYID,sum(FRMREALQTY) as qty,sum(ef.FALLAMOUNT) as amount,e.FORDERNO from T_PUR_MRB t 
                                                            left join T_PUR_MRBENTRY e on t.FID = e.FID 
															left join T_PUR_MRBENTRY_F ef on e.FENTRYID = ef.FENTRYID
                                                            where e.FORDERNO = '{0}'
                                                            group by e.FORDERNO,e.FPOORDERENTRYID", purNo);
                    DynamicObjectCollection queryAmountResult = DBUtils.ExecuteDynamicObject(this.Context, queryAmountSql);

                    JSONArray detail1 = new JSONArray();
                    foreach (DynamicObject queryAmount in queryAmountResult)
                    {
                        JSONObject queryAmountItem = new JSONObject();
                        JSONObject queryAmountData = new JSONObject();
                        JSONObject operate = new JSONObject();
                        operate.Add("action", "SaveOrUpdate");
                        operate.Add("actionDescribe", "Update");
                        queryAmountItem.Add("operate", operate);

                        decimal amount = Convert.ToDecimal(queryAmount["amount"]);
                        decimal qty = Convert.ToDecimal(queryAmount["qty"]);
                        string entryId = Convert.ToString(queryAmount["FPOORDERENTRYID"]);

                        queryAmountData.Add("gltlsl", qty);
                        queryAmountData.Add("gltlje", amount);
                        queryAmountData.Add("erpfkjhid", entryId);

                        queryAmountItem.Add("data", queryAmountData);
                        detail1.Add(queryAmountItem);
                    }

                    dateItem.Add("detail1", detail1);

                    operationinfo.Add("operationDate", DateTime.Now.ToString("yyyy-MM-dd"));
                    operationinfo.Add("operator", "1");
                    operationinfo.Add("operationTime", DateTime.Now.ToString("HH:mm:ss"));


                    dateItem.Add("operationinfo", operationinfo);
                    dateItem.Add("mainTable", mainTable);
                    data.Add(dateItem);
                    dataJson.Add("data", data);


                    JSONObject header = new JSONObject();
                    string datetime = DateTime.Now.ToString("yyyyMMddHHmmss");
                    header.Add("systemid", "ERP");
                    header.Add("currentDateTime", datetime);
                    header.Add("Md5", Utils.StringToMD5Hash("ERPerp" + datetime));

                    dataJson.Add("header", header);

                    string results = Utils.PostUrl(Utils.pushCGDDurl, "datajson=" + dataJson.ToString());

                    JSONObject resultJson = JSONObject.Parse(results);
                    string retCode = Convert.ToString(resultJson["status"]);

                    if (retCode.Equals("1"))
                    {

                    }
                    else
                    {
                        throw new KDException("", results);
                    }


                }

            }
        }
    }
}
