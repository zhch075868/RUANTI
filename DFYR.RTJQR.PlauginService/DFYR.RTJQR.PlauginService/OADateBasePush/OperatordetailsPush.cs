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
    [Kingdee.BOS.Util.HotUpdate, Description("推送业务员至OA")]
    public class OperatordetailsPush : AbstractOperationServicePlugIn
    {
         public override void OnPreparePropertys(PreparePropertysEventArgs e)
         {
             //e.FieldKeys.Add("");将需要应用的字段Key加入
             e.FieldKeys.Add("F_PYEO_CHECKBOX_OA");
             e.FieldKeys.Add("FRATE");
             e.FieldKeys.Add("FALLOWREIM");
             e.FieldKeys.Add("FDescription");
             e.FieldKeys.Add("FOperatorType");

         }

        /// <summary>
        /// 操作开始前功能处理
        /// </summary>
        /// <param name="e"></param>
         public override void BeginOperationTransaction(BeginOperationTransactionArgs e)
         {
             foreach (DynamicObject o in e.DataEntitys)
             {
                 string OperatorType = Convert.ToString(o["OperatorType"]);
                 OperatorType = this.PurType(OperatorType);

                 DynamicObjectCollection operatorEntrys = o["BD_OPERATORENTRY"] as DynamicObjectCollection;
                 foreach (DynamicObject entry in operatorEntrys)
                 {
                     string entryId = Convert.ToString(entry["Id"]);
                     DynamicObject StaffId = entry["StaffId"] as DynamicObject;
                     string staffNumber = Convert.ToString(StaffId["Number"]);
                     string staffName = Convert.ToString(StaffId["Name"]);
                     string ForbiddenStatus = Convert.ToString(entry["ForbiddenStatus"]);
                     ForbiddenStatus = this.ForbiddenStatus(ForbiddenStatus);
                     DynamicObject BizOrgId = entry["BizOrgId"] as DynamicObject;
                     string BizOrgNumber = Convert.ToString(BizOrgId["Number"]);
                     string number = Convert.ToString(entry["Number"]);

                     JSONObject pushjson = new JSONObject();
                     JSONObject dataJson = new JSONObject();

                     JSONArray data = new JSONArray();
                     JSONObject dateItem = new JSONObject();
                     JSONObject operationinfo = new JSONObject();
                     JSONObject mainTable = new JSONObject();

                     mainTable.Add("ywylx", OperatorType);
                     mainTable.Add("ywybm", number);
                     mainTable.Add("ywyxm", staffName);
                     mainTable.Add("jyzt", ForbiddenStatus);
                     //mainTable.Add("ywzzbm", BizOrgNumber);
                     //mainTable.Add("ywzzmc", BizOrgNumber);
                     mainTable.Add("ygbm", staffNumber);
                     mainTable.Add("erpid", entryId);

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

                     string results = Utils.PostUrl(Utils.pushYWYurl, "datajson=" + dataJson.ToString());

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
         /// 业务员类型
         /// </summary>
         /// <param name="erpId"></param>
         /// <returns></returns>s
         private string PurType(string erpId)
         {
             try
             {
                 string queryOAidSql = string.Format(@"select F_PYEO_TEXT1 from F_PYEO_Entity_YWY where F_PYEO_TEXT = '{0}'", erpId);
                 return DBUtils.ExecuteDynamicObject(this.Context, queryOAidSql)[0]["F_PYEO_TEXT1"].ToString();
             }
             catch (Exception e)
             {
                 return "";
             }
         }

         /// <summary>
         /// 禁用状态
         /// </summary>
         /// <param name="erpId"></param>
         /// <returns></returns>
         private string ForbiddenStatus(string erpId)
         {
             try
             {
                 string queryOAidSql = string.Format(@"select F_PYEO_OAID10 from PYEO_t_Cust_Entry100037 where F_PYEO_ERPID10 = '{0}'", erpId);
                 return DBUtils.ExecuteDynamicObject(this.Context, queryOAidSql)[0]["F_PYEO_OAID10"].ToString();
             }
             catch (Exception e)
             {
                 return "";
             }
         }
    }
}
