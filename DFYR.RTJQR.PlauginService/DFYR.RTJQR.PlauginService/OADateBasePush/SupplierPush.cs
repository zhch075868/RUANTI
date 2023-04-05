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
    [Kingdee.BOS.Util.HotUpdate, Description("推送供应商至OA")]
    public class SupplierPush : AbstractOperationServicePlugIn
    {
        public override void OnPreparePropertys(PreparePropertysEventArgs e)
        {
            //e.FieldKeys.Add("");将需要应用的字段Key加入
            e.FieldKeys.Add("F_PYEO_CHECKBOX_OA");
            e.FieldKeys.Add("FGender");
            e.FieldKeys.Add("FContact");
            e.FieldKeys.Add("FPost");
            e.FieldKeys.Add("FCompanyClassify");
            e.FieldKeys.Add("FTel");
            e.FieldKeys.Add("FMobile");
            e.FieldKeys.Add("FFax");
            e.FieldKeys.Add("FEMail");
            e.FieldKeys.Add("FContactIsDefault");
            e.FieldKeys.Add("FContactDescription");
            e.FieldKeys.Add("FBankCountry");
            e.FieldKeys.Add("FBankCode");
            e.FieldKeys.Add("FBankHolder");
            e.FieldKeys.Add("FTextBankDetail");
            e.FieldKeys.Add("FBankTypeRec");
            e.FieldKeys.Add("FRegisterCode");
            e.FieldKeys.Add("FTendPermit");
            e.FieldKeys.Add("FSOCIALCRECODE");
            e.FieldKeys.Add("FRegisterAddress");
            e.FieldKeys.Add("FGroup");
            e.FieldKeys.Add("FBaseInfo");

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
                    mainTable.Add("zt", "0");
                    string isOa = Convert.ToString(o["F_PYEO_CHECKBOX_OA"]);
                    //明细1
                    JSONArray detail1 = new JSONArray();
                    DynamicObjectCollection SupplierContacts = o["SupplierContact"] as DynamicObjectCollection;
                    if (SupplierContacts != null)
                    {
                        foreach (DynamicObject supplierContact in SupplierContacts)
                        {
                            JSONObject contractItem = new JSONObject();
                            JSONObject contractData = new JSONObject();
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
                            contractItem.Add("operate", operate);

                            string strGender = string.Empty;
                            DynamicObject gender = supplierContact["Gender"] as DynamicObject;
                            if (gender != null) { strGender = Convert.ToString(gender["FDataValue"]); }

                            string isDefault = Convert.ToString(supplierContact["IsDefault"]).Equals("1") ? "0" : "1";
                            string entryId = Convert.ToString(supplierContact["id"]);

                            contractData.Add("gddh", supplierContact["Tel"]);
                            contractData.Add("lxr", supplierContact["Contact"]);
                            contractData.Add("yddh", supplierContact["Mobile"]);
                            contractData.Add("cz", supplierContact["Fax"]);
                            contractData.Add("bz", supplierContact["Description"]);
                            contractData.Add("xb", strGender.Equals("男") ? "0" : "1");
                            contractData.Add("dzyx", supplierContact["EMail"]);
                            contractData.Add("bm", supplierContact["ContactNumber"]);
                            contractData.Add("lx", "0");
                            contractData.Add("smrlxr", isDefault);
                            contractData.Add("zw", supplierContact["FPost"]);
                            contractData.Add("erpmxid", entryId);

                            contractItem.Add("data", contractData);
                            detail1.Add(contractItem);
                        }
                    }

                    JSONArray detail2 = new JSONArray();
                    DynamicObjectCollection SupplierBanks = o["SupplierBank"] as DynamicObjectCollection;
                    foreach (DynamicObject SupplierBank in SupplierBanks)
                    {
                        JSONObject bankItem = new JSONObject();
                        JSONObject bankData = new JSONObject();
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
                        bankItem.Add("operate", operate);

                        string strCountry = string.Empty;
                        DynamicObject country = SupplierBank["Country"] as DynamicObject;
                        if (country != null) { strCountry = Convert.ToString(country["FDataValue"]); }
                        DynamicObject BankDetailId = SupplierBank["FTextBankDetail"] as DynamicObject;
                        string BankDetailName = BankDetailId==null?"": Convert.ToString(BankDetailId["Name"]);
                        string entryId = Convert.ToString(SupplierBank["Id"]);

                        bankData.Add("gj", strCountry);
                        bankData.Add("yxwd", BankDetailName);
                        bankData.Add("yxzh", SupplierBank["BankCode"]);
                        bankData.Add("zhmc", SupplierBank["BankHolder"]);
                        bankData.Add("khyx", Convert.ToString(SupplierBank["OpenBankName"]));
                        bankData.Add("erpmxid", entryId);

                        bankItem.Add("data", bankData);
                        detail2.Add(bankItem);
                    }
                    dateItem.Add("detail1", detail1);
                    dateItem.Add("detail2", detail2);

                    DynamicObjectCollection SupplierBase = o["SupplierBase"] as DynamicObjectCollection;
                    
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
                    if (SupplierBase != null && SupplierBase.Count != 0)
                    {
                        string strGroop = string.Empty;
                        DynamicObject group = o["FGroup"] as DynamicObject;
                        if (group != null) { strGroop = Convert.ToString(group["Name"]); }
                        string entryId = Convert.ToString(SupplierBase[0]["Id"]);

                        baseData.Add("gysfz", strGroop);
                        baseData.Add("scjyxkz", SupplierBase[0]["TendPermit"]);
                        baseData.Add("gsdjh", SupplierBase[0]["RegisterCode"]);
                        baseData.Add("zcdz", SupplierBase[0]["RegisterAddress"]);
                        baseData.Add("tyshxydm", SupplierBase[0]["SOCIALCRECODE"]);
                        baseData.Add("erpmxid", entryId);

                    }
                    baseItem.Add("data", baseData);
                    detail3.Add(baseItem);
                    dateItem.Add("detail3", detail3);



                    
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

                string results = Utils.PostUrl(Utils.pushOAGYSurl, "datajson=" + dataJson.ToString());

                JSONObject resultJson = JSONObject.Parse(results);
                string retCode = Convert.ToString(resultJson["status"]);

                if (retCode.Equals("1"))
                {
                    string sql = string.Format("update t_BD_Supplier set F_PYEO_CHECKBOX_OA = 1 where FSupplierId = {0}", id);
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
