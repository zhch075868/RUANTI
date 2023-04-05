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

namespace DFYR.RTJQR.PlauginService.OADateBasePush
{
    [Kingdee.BOS.Util.HotUpdate, Description("推送采购价目表至OA")]
    public class PriceCategoryPush : AbstractOperationServicePlugIn
    {
        public override void OnPreparePropertys(PreparePropertysEventArgs e)
        {
            //e.FieldKeys.Add("");将需要应用的字段Key加入
            e.FieldKeys.Add("F_PYEO_CHECKBOX_OA");
            e.FieldKeys.Add("FCreateOrgId");
            e.FieldKeys.Add("FUseOrgId");
            e.FieldKeys.Add("FNumber");
            e.FieldKeys.Add("FName");
            e.FieldKeys.Add("FPriceObject");
            e.FieldKeys.Add("FPriceType");
            e.FieldKeys.Add("FCurrencyID");
            e.FieldKeys.Add("FSupplierID");
            e.FieldKeys.Add("FPricer");
            e.FieldKeys.Add("FIsIncludedTax");
            e.FieldKeys.Add("FDefPriceListId");
            e.FieldKeys.Add("FDescription");
            e.FieldKeys.Add("FIsPriceExcludeTax");
            e.FieldKeys.Add("FMaterialId");
            e.FieldKeys.Add("FAuxPropId");
            e.FieldKeys.Add("FMaterialTypeId");
            e.FieldKeys.Add("FUnitID");
            e.FieldKeys.Add("FFROMQTY");
            e.FieldKeys.Add("FToQty");
            e.FieldKeys.Add("FPrice");
            e.FieldKeys.Add("FTaxPrice");
            e.FieldKeys.Add("FPublicWastePrice");
            e.FieldKeys.Add("FPublicWasteTaxPrice");
            e.FieldKeys.Add("FWastePrice");
            e.FieldKeys.Add("FWasteTaxPrice");
            e.FieldKeys.Add("FTaxRate");
            e.FieldKeys.Add("FPriceCoefficient");
            e.FieldKeys.Add("FUpPrice");
            e.FieldKeys.Add("FDownPrice");
            e.FieldKeys.Add("FEntryEffectiveDate");
            e.FieldKeys.Add("FEntryExpiryDate");
            e.FieldKeys.Add("FProcessOrgId");
            e.FieldKeys.Add("FPROCESSID");
            e.FieldKeys.Add("FNote");
            e.FieldKeys.Add("FPRICEFROM");
            e.FieldKeys.Add("FFROMBILLNO");

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

                    DynamicObject createOrgId = o["CreateOrgId"] as DynamicObject;
                    string createOrgName = Convert.ToString(createOrgId["Name"]);
                    DynamicObject useOrgId = o["UseOrgId"] as DynamicObject;
                    string useOrgIdName = Convert.ToString(useOrgId["Name"]);
                    //价目表对象获取
                    var field = this.BusinessInfo.GetField("FPriceObject") as ComboField; ;
                    var enumValue = o["PriceObject"];
                    var priceObjectName = field.GetEnumItemName(enumValue);

                    field = this.BusinessInfo.GetField("FPriceType") as ComboField; ;
                    enumValue = o["PriceType"];
                    var priceTypeName = field.GetEnumItemName(enumValue);

                    DynamicObject currencyID = o["CurrencyID"] as DynamicObject;
                    string currencyName = Convert.ToString(currencyID["Name"]);

                    DynamicObject supplierID = o["SupplierID"] as DynamicObject;
                    string supplierName = Convert.ToString(supplierID["Name"]);

                    DynamicObject pricer = o["Pricer"] as DynamicObject;
                    string pricerName = Convert.ToString(pricer["Name"]);

                    string isIncludedTax = Convert.ToString(o["IsIncludedTax"]);
                    string defPriceListId = Convert.ToString(o["DefPriceListId"]);

                    string description = Convert.ToString(o["NOTE"]);
                    string isPriceExcludeTax = Convert.ToString(o["IsPriceExcludeTax"]);

                    mainTable.Add("cgzz", createOrgName);
                    mainTable.Add("syzz", useOrgIdName);
                    mainTable.Add("jmbdx", priceObjectName);
                    mainTable.Add("jglx", priceTypeName);
                    mainTable.Add("bb", currencyName);
                    mainTable.Add("gys", supplierName);
                    mainTable.Add("djy", pricerName);
                    mainTable.Add("hs", isIncludedTax.Equals("True")?1:0);
                    mainTable.Add("mrjmb", defPriceListId.Equals("True")?1:0);
                    mainTable.Add("bz", description);
                    mainTable.Add("jws", isPriceExcludeTax.Equals("True")?1:0);


                    JSONArray detail1 = new JSONArray();
                    DynamicObjectCollection materials = o["PriceListEntry"] as DynamicObjectCollection;
                    foreach (DynamicObject material in materials)
                    {
                        JSONObject materialItem = new JSONObject();
                        JSONObject materialData = new JSONObject();
                        JSONObject operate = new JSONObject();
                        operate.Add("action", "SaveOrUpdate");
                        operate.Add("actionDescribe", "Save");
                        materialItem.Add("operate", operate);

                        DynamicObject materialId = material["MaterialId"] as DynamicObject;
                        string materialNumber = Convert.ToString(materialId["Number"]);
                        string materialName = Convert.ToString(materialId["Name"]);

                        DynamicObject materialTypeId = material["MaterialTypeId"] as DynamicObject;
                        string materialTypeName = Convert.ToString(materialTypeId["Name"]);

                        DynamicObject unitId = material["UnitId"] as DynamicObject;
                        string unitName = Convert.ToString(unitId["Name"]);
                        decimal romQty = Convert.ToDecimal(material["FROMQTY"]);
                        decimal toQty = Convert.ToDecimal(material["ToQty"]);
                        decimal price = Convert.ToDecimal(material["Price"]);
                        decimal taxPrice = Convert.ToDecimal(material["TaxPrice"]);
                        decimal publicWastePrice = Convert.ToDecimal(material["PublicWastePrice"]);
                        decimal publicWasteTaxPrice = Convert.ToDecimal(material["PublicWasteTaxPrice"]);
                        decimal wastePrice = Convert.ToDecimal(material["WastePrice"]);
                        decimal wasteTaxPrice = Convert.ToDecimal(material["WasteTaxPrice"]);
                        decimal taxRate = Convert.ToDecimal(material["TaxRate"]);
                        decimal pricecoefficient = Convert.ToDecimal(material["Pricecoefficient"]);
                        decimal upPrice = Convert.ToDecimal(material["UpPrice"]);
                        decimal downPrice = Convert.ToDecimal(material["DownPrice"]);
                        string effectiveDate = Convert.ToDateTime(material["EffectiveDate"]).ToString("yyyy-MM-dd");
                        string expiryDate = Convert.ToDateTime(material["ExpiryDate"]).ToString("yyyy-MM-dd");

                        DynamicObject processOrgId = material["ProcessOrgId"] as DynamicObject;
                        string processOrgName = Convert.ToString(processOrgId["Name"]);
                        DynamicObject processId = material["PROCESSID"] as DynamicObject;
                        string processName = processId == null?"": Convert.ToString(processId["Name"]);
                        string note = Convert.ToString(material["Note"]);

                        field = this.BusinessInfo.GetField("FPriceObject") as ComboField; ;
                        enumValue = material["PRICEFROM"];
                        var priceromName = field.GetEnumItemName(enumValue);
                        string romBillNo = Convert.ToString(material["FROMBILLNO"]);


                        materialData.Add("wlbm", materialNumber);
                        materialData.Add("wlmc", materialName);
                        materialData.Add("chlb", materialTypeName);
                        materialData.Add("jjdw", unitName);
                        materialData.Add("c", romQty);
                        materialData.Add("z", toQty);
                        materialData.Add("dj", price);
                        materialData.Add("hsdj", taxPrice);
                        materialData.Add("gfdj", publicWastePrice);
                        materialData.Add("gfhsdj", publicWasteTaxPrice);
                        materialData.Add("lfdj", wastePrice);
                        materialData.Add("lfhsdj", wasteTaxPrice);
                        materialData.Add("sl", taxRate);
                        materialData.Add("jgxs", pricecoefficient);
                        materialData.Add("jgsx", upPrice);
                        materialData.Add("jgxx", downPrice);
                        materialData.Add("sxrq", effectiveDate);
                        materialData.Add("sxrqq", expiryDate);
                        materialData.Add("xqzz", processOrgName);
                        materialData.Add("zy", processName);
                        materialData.Add("bz", note);
                        materialData.Add("jgly", priceromName);
                        materialData.Add("lydh", romBillNo);

                        materialItem.Add("data", materialData);
                        detail1.Add(materialItem);
                    }

                    dateItem.Add("detail1", detail1);
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

                string results = Utils.PostUrl(Utils.pushJMBurl, "datajson=" + dataJson.ToString());

                JSONObject resultJson = JSONObject.Parse(results);
                string retCode = Convert.ToString(resultJson["status"]);

                //if (retCode.Equals("1"))
                //{
                //    string sql = string.Format("update t_BD_Supplier set F_PYEO_CHECKBOX_OA = 1 where FSupplierId = {0}", id);
                //    DBUtils.Execute(this.Context, sql);
                //}

            }
        }
    }
}
