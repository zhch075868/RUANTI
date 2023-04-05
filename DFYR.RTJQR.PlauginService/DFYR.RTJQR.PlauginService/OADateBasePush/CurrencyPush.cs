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
    [Kingdee.BOS.Util.HotUpdate, Description("推送币别至OA")]
    public class CurrencyPush : AbstractOperationServicePlugIn
    {
        public override void OnPreparePropertys(PreparePropertysEventArgs e)
        {
            //e.FieldKeys.Add("");将需要应用的字段Key加入
            e.FieldKeys.Add("F_PYEO_CHECKBOX_OA");

        }

        /// <summary>
        /// 操作开始前功能处理
        /// </summary>
        /// <param name="e"></param>
        public override void BeginOperationTransaction(BeginOperationTransactionArgs e)
        {
            foreach (DynamicObject o in e.DataEntitys)
            {
                string id = Convert.ToString(o["Id"]);
                string opName = this.FormOperation.Operation;

                string number = Convert.ToString(o["Number"]);
                string name = Convert.ToString(o["Name"]);

                JSONObject pushjson = new JSONObject();
                JSONObject dataJson = new JSONObject();

                JSONArray data = new JSONArray();
                JSONObject dateItem = new JSONObject();
                JSONObject operationinfo = new JSONObject();
                JSONObject mainTable = new JSONObject();

                //数据组合
                mainTable.Add("mc", name);
                mainTable.Add("bm", number);

                if (opName.Equals("PushOA"))
                {
                    string isOa = Convert.ToString(o["F_PYEO_CHECKBOX_OA"]);
                    if (isOa.Equals("1"))
                    {
                        return;
                    }                    
                }

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

                string results = Utils.PostUrl(Utils.pushBBurl, "datajson=" + dataJson.ToString());

                JSONObject resultJson = JSONObject.Parse(results);
                string retCode = Convert.ToString(resultJson["status"]);

                if (retCode.Equals("1"))
                {
                    string sql = string.Format("update T_BD_CURRENCY set F_PYEO_CHECKBOX_OA = 1 where FCURRENCYID = {0}", id);
                    DBUtils.Execute(this.Context, sql);
                }
                else
                {
                    throw new KDException("", results);
                }

            }
        }
    }
}
