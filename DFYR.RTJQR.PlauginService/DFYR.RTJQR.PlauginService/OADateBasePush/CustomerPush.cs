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
    [Kingdee.BOS.Util.HotUpdate, Description("推送客户至OA")]
    public class CustomerPush : AbstractOperationServicePlugIn
    {
        public override void OnPreparePropertys(PreparePropertysEventArgs e)
        {
            //e.FieldKeys.Add("");将需要应用的字段Key加入
            e.FieldKeys.Add("F_PYEO_CHECKBOX_OA");
            e.FieldKeys.Add("FShortName");
            e.FieldKeys.Add("FINVOICETITLE");
            e.FieldKeys.Add("FTAXREGISTERCODE");
            e.FieldKeys.Add("FINVOICEBANKNAME");
            e.FieldKeys.Add("FINVOICEBANKACCOUNT");
            e.FieldKeys.Add("FINVOICETEL");
            e.FieldKeys.Add("FINVOICEADDRESS");
            e.FieldKeys.Add("FContactId");
            e.FieldKeys.Add("FISDEFAULT");
            e.FieldKeys.Add("FGroup");

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
                mainTable.Add("khmc", name);
                mainTable.Add("khbm", number);

                if (opName.Equals("Forbid"))
                {
                    mainTable.Add("zt", "1");
                }
                if (opName.Equals("Enable"))
                {
                    mainTable.Add("zt", "0");
                }
                
                if (opName.Equals("PushOA"))
                {
                    string isOa = Convert.ToString(o["F_PYEO_CHECKBOX_OA"]);
                    mainTable.Add("zt", "0");
                    JSONArray detail1 = new JSONArray();
                    JSONObject invoiceItem = new JSONObject();
                    JSONObject invoiceData = new JSONObject();
                    JSONObject invoiceoperate = new JSONObject();
                    invoiceoperate.Add("action", "SaveOrUpdate");
                    if (isOa.Equals("True"))
                    {
                        invoiceoperate.Add("actionDescribe", "Update");
                    }
                    else
                    {
                        invoiceoperate.Add("actionDescribe", "Save");
                    }
                    invoiceItem.Add("operate", invoiceoperate);


                    string shortName = Convert.ToString(o["ShortName"]);
                    string invoicTitle = Convert.ToString(o["INVOICETITLE"]);
                    string taxregisterCode = Convert.ToString(o["FTAXREGISTERCODE"]);
                    string invoicebankName = Convert.ToString(o["INVOICEBANKNAME"]);
                    string invoiceBankAccount = Convert.ToString(o["INVOICEBANKACCOUNT"]);
                    string invoiceTel = Convert.ToString(o["INVOICETEL"]);
                    string invoiceAddress = Convert.ToString(o["INVOICEADDRESS"]);


                    mainTable.Add("khjc", shortName);

                    invoiceData.Add("fptt", invoicTitle);
                    invoiceData.Add("nsdjh", taxregisterCode);
                    invoiceData.Add("khyx", invoicebankName);
                    invoiceData.Add("yxzh", invoiceBankAccount);
                    invoiceData.Add("kplxdh", invoiceTel);
                    invoiceData.Add("kptxdz", invoiceAddress);
                    invoiceData.Add("erpmxid", id);

                    invoiceItem.Add("data", invoiceData);
                    detail1.Add(invoiceItem);
                    dateItem.Add("detail1", detail1);

                    JSONArray detail2 = new JSONArray();   
                    DynamicObjectCollection custlocations = o["BD_CUSTLOCATION"] as DynamicObjectCollection;
                    foreach (DynamicObject custlocation in custlocations)
                    {
                        JSONObject locationItem = new JSONObject();
                        JSONObject locationData = new JSONObject();
                        JSONObject operate = new JSONObject();
                        operate.Add("action", "SaveOrUpdate");
                        if (isOa.Equals("True"))
                        {
                            operate.Add("actionDescribe", "Update");
                        }
                        else
                        {
                            operate.Add("actionDescribe", "Save");
                        }
                        locationItem.Add("operate", operate);

                        DynamicObject contactId = custlocation["ContactId"] as DynamicObject;
                        string contactName = Convert.ToString(contactId["Name"]);
                        string contactNnumber = Convert.ToString(contactId["Number"]);
                        string mobilePhone = Convert.ToString(contactId["Mobile"]);
                        string isdefault = Convert.ToString(custlocation["ISDEFAULT"]);
                        string fax = Convert.ToString(contactId["Fax"]);
                        string bizAddress = Convert.ToString(contactId["BizAddress"]);
                        string entryid = Convert.ToString(contactId["Id"]);

                        locationData.Add("lxrbm", contactNnumber);
                        locationData.Add("lxrxm", contactName);
                        locationData.Add("yddh", mobilePhone);
                        locationData.Add("mr", isdefault.Equals("True")?"1":"0");
                        locationData.Add("czyx", fax);
                        locationData.Add("xxdz", bizAddress);
                        locationData.Add("jyzt", "0");
                        locationData.Add("erpmxid", entryid);

                        locationItem.Add("data", locationData);
                        detail2.Add(locationItem);
                    }

                    dateItem.Add("detail2", detail2);
                    DynamicObject group = o["FGroup"] as DynamicObject;
                    string groupName = Convert.ToString(group["Name"]);

                    JSONArray detail3 = new JSONArray();
                    JSONObject baseItem = new JSONObject();
                    JSONObject baseData = new JSONObject();
                    JSONObject baseoperate = new JSONObject();
                    baseoperate.Add("action", "SaveOrUpdate");
                    if (isOa.Equals("True"))
                    {
                        baseoperate.Add("actionDescribe", "Update");
                    }
                    else
                    {
                        baseoperate.Add("actionDescribe", "Save");
                    }
                    baseItem.Add("operate", baseoperate);

                    baseData.Add("khfz",groupName);
                    baseData.Add("erpmxid", id);

                    baseItem.Add("data", baseData);
                    detail3.Add(baseItem);
                    dateItem.Add("detail3", detail3);

;

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

                string results = Utils.PostUrl(Utils.pushKHurl, "datajson=" + dataJson.ToString());

                JSONObject resultJson = JSONObject.Parse(results);
                string retCode = Convert.ToString(resultJson["status"]);

                if (retCode.Equals("1"))
                {
                    string sql = string.Format("update T_BD_CUSTOMER set F_PYEO_CHECKBOX_OA = 1 where FCUSTID = {0}", id);
                    DBUtils.Execute(this.Context, sql);
                }
                else
                {
                    throw new KDException("",results);
                }

            }
        }
    }
}
