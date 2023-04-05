using Kingdee.BOS;
using Kingdee.BOS.App.Data;
using Kingdee.BOS.Core.DynamicForm.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.JSON;
using Kingdee.BOS.Orm.DataEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFYR.RTJQR.PlauginService.OADateBasePush
{
    [Kingdee.BOS.Util.HotUpdate, Description("推送业务组至OA")]
    public class BizGroupPush : AbstractOperationServicePlugIn
    {
        public override void OnPreparePropertys(PreparePropertysEventArgs e)
        {
            //e.FieldKeys.Add("");将需要应用的字段Key加入
            e.FieldKeys.Add("OperatorGroupType");
            e.FieldKeys.Add("FRATE");
            e.FieldKeys.Add("FALLOWREIM");
            e.FieldKeys.Add("FDescription");

        }

        /// <summary>
        /// 操作开始前功能处理
        /// </summary>
        /// <param name="e"></param>
        public override void BeginOperationTransaction(BeginOperationTransactionArgs e)
        {
            foreach (DynamicObject o in e.DataEntitys)
            {
                string OperatorType = Convert.ToString(o["OperatorGroupType"]);
                OperatorType = this.bizGroupType(OperatorType);

                DynamicObjectCollection operatorEntrys = o["BD_OPERATORGROUPENTRY"] as DynamicObjectCollection;
                foreach (DynamicObject entry in operatorEntrys)
                {
                    string number = Convert.ToString(entry["Number"]);
                    string name = Convert.ToString(entry["Name"]);
                    string IsUse = Convert.ToString(entry["IsUse"]).Equals("True")?"0":"1";
                    string Description = Convert.ToString(entry["Description"]);
                    JSONObject pushjson = new JSONObject();
                    JSONObject dataJson = new JSONObject();

                    JSONArray data = new JSONArray();
                    JSONObject dateItem = new JSONObject();
                    JSONObject operationinfo = new JSONObject();
                    JSONObject mainTable = new JSONObject();

                    mainTable.Add("ywzlx", OperatorType);
                    mainTable.Add("ywzbm", number);
                    mainTable.Add("ywzmc", name);
                    mainTable.Add("qy", IsUse);
                    mainTable.Add("ms", Description);


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

                    string results = Utils.PostUrl(Utils.pushYWZurl, "datajson=" + dataJson.ToString());

                    JSONObject resultJson = JSONObject.Parse(results);
                    string retCode = Convert.ToString(resultJson["status"]);

                    if (retCode.Equals("1"))
                    {
                        //string sql = string.Format("update T_BD_EXPENSE set F_PYEO_CHECKBOX_OA = 1 where FEXPID = {0}", id);
                        //DBUtils.Execute(this.Context, sql);
                    }
                    else
                    {
                        throw new KDException("", results);
                    }
                }
            }
        }


        /// <summary>
        /// 业务组类型
        /// </summary>
        /// <param name="erpId"></param>
        /// <returns></returns>s
        private string bizGroupType(string erpId)
        {
            try
            {
                string queryOAidSql = string.Format(@"select F_PYEO_OAID9 from PYEO_t_Cust_Entry100036 where F_PYEO_ERPID9 = '{0}'", erpId);
                return DBUtils.ExecuteDynamicObject(this.Context, queryOAidSql)[0]["F_PYEO_OAID9"].ToString();
            }
            catch (Exception e)
            {
                return "";
            }
        }
    }
}
