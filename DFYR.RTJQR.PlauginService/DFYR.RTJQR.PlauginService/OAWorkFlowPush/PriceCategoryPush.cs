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
                string billNo = Convert.ToString(o["Number"]);
                string billName = Convert.ToString(o["Name"]);
                string documentStatus = Convert.ToString(o["DocumentStatus"]);

                //提交校验
                if (!documentStatus.Equals("B"))
                {
                    return;
                }

                DynamicObject createOrgId = o["CreateOrgId"] as DynamicObject;
                string createOrgName = createOrgId == null?"":Convert.ToString(createOrgId["Name"]);
                DynamicObject useOrgId = o["UseOrgId"] as DynamicObject;
                string useOrgIdName = useOrgId == null ? "" : Convert.ToString(useOrgId["Name"]);
                //价目表对象获取
                var field = this.BusinessInfo.GetField("FPriceObject") as ComboField; ;
                var enumValue = o["PriceObject"];
                var priceObjectName = field.GetEnumItemName(enumValue);

                field = this.BusinessInfo.GetField("FPriceType") as ComboField; ;
                enumValue = o["PriceType"];
                var priceTypeName = field.GetEnumItemName(enumValue);

                DynamicObject currencyID = o["CurrencyID"] as DynamicObject;
                string currencyName = currencyID == null ? "" : Convert.ToString(currencyID["Name"]);

                DynamicObject supplierID = o["SupplierID"] as DynamicObject;
                string supplierName = supplierID == null ? "" : Convert.ToString(supplierID["Name"]);

                DynamicObject pricer = o["Pricer"] as DynamicObject;
                string pricerName = pricer == null ? "" : Convert.ToString(pricer["Name"]);

                string isIncludedTax = Convert.ToString(o["IsIncludedTax"]).Equals("True")?"是":"否";
                string defPriceListId = Convert.ToString(o["DefPriceListId"]).Equals("True")?"是":"否";

                string description = Convert.ToString(o["NOTE"]);
                string isPriceExcludeTax = Convert.ToString(o["IsPriceExcludeTax"]).Equals("True")?"是":"否";

                JSONArray mainRoot = new JSONArray();
                JSONObject mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "cgzz");
                mainRootItem.Add("fieldValue", createOrgName);
                mainRoot.Add(mainRootItem);


                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "bm");
                mainRootItem.Add("fieldValue", billNo);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "erpnumber");
                mainRootItem.Add("fieldValue", billNo);
                mainRoot.Add(mainRootItem);


                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "mc");
                mainRootItem.Add("fieldValue", billName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "syzz");
                mainRootItem.Add("fieldValue", useOrgIdName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "jmbdx");
                mainRootItem.Add("fieldValue", priceObjectName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "jglx");
                mainRootItem.Add("fieldValue", priceTypeName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "bb");
                mainRootItem.Add("fieldValue", currencyName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "gys");
                mainRootItem.Add("fieldValue", supplierName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "djy");
                mainRootItem.Add("fieldValue", pricerName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "hs");
                mainRootItem.Add("fieldValue", isIncludedTax);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "mrjmb");
                mainRootItem.Add("fieldValue", defPriceListId);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "bz");
                mainRootItem.Add("fieldValue", description);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "jws");
                mainRootItem.Add("fieldValue", isPriceExcludeTax);
                mainRoot.Add(mainRootItem);

                JSONArray detailData = new JSONArray();

                JSONObject detailDataItem = new JSONObject();
                detailDataItem.Add("tableDBName", "formtable_main_56_dt1");
                JSONArray workflowRequestTableRecords = new JSONArray();

                DynamicObjectCollection materials = o["PriceListEntry"] as DynamicObjectCollection;
                foreach (DynamicObject material in materials)
                {
                    DynamicObject materialId = material["MaterialId"] as DynamicObject;
                    string materialNumber = materialId == null ? "" : Convert.ToString(materialId["Number"]);
                    string materialName = materialId == null ? "" : Convert.ToString(materialId["Name"]);

                    DynamicObject materialTypeId = material["MaterialTypeId"] as DynamicObject;
                    string materialTypeName = materialTypeId == null ? "" : Convert.ToString(materialTypeId["Name"]);

                    DynamicObject unitId = material["UnitId"] as DynamicObject;
                    string unitName = unitId == null ? "" : Convert.ToString(unitId["Name"]);
                    string romQty = Convert.ToDecimal(material["FROMQTY"]).ToString("#0.00");
                    string toQty = Convert.ToDecimal(material["ToQty"]).ToString("#0.00");
                    string price = Convert.ToDecimal(material["Price"]).ToString("#0.00");
                    string taxPrice = Convert.ToDecimal(material["TaxPrice"]).ToString("#0.00");
                    string publicWastePrice = Convert.ToDecimal(material["PublicWastePrice"]).ToString("#0.00");
                    string publicWasteTaxPrice = Convert.ToDecimal(material["PublicWasteTaxPrice"]).ToString("#0.00");
                    string wastePrice = Convert.ToDecimal(material["WastePrice"]).ToString("#0.00");
                    string wasteTaxPrice = Convert.ToDecimal(material["WasteTaxPrice"]).ToString("#0.00");
                    string taxRate = Convert.ToDecimal(material["TaxRate"]).ToString("#0.00");
                    string pricecoefficient = Convert.ToDecimal(material["Pricecoefficient"]).ToString("#0.00");
                    string upPrice = Convert.ToDecimal(material["UpPrice"]).ToString("#0.00");
                    string downPrice = Convert.ToDecimal(material["DownPrice"]).ToString("#0.00");
                    string effectiveDate = Convert.ToDateTime(material["EffectiveDate"]).ToString("yyyy-MM-dd");
                    string expiryDate = Convert.ToDateTime(material["ExpiryDate"]).ToString("yyyy-MM-dd");

                    DynamicObject processOrgId = material["ProcessOrgId"] as DynamicObject;
                    string processOrgName = processOrgId == null ? "" : Convert.ToString(processOrgId["Name"]);
                    DynamicObject processId = material["PROCESSID"] as DynamicObject;
                    string processName = processId == null ? "" : Convert.ToString(processId["Name"]);
                    string note = Convert.ToString(material["Note"]);

                    field = this.BusinessInfo.GetField("FPriceObject") as ComboField; ;
                    enumValue = material["PRICEFROM"];
                    var priceromName = field.GetEnumItemName(enumValue);
                    string romBillNo = Convert.ToString(material["FROMBILLNO"]);

                    JSONObject workflowRequestTableRecordsItem = new JSONObject();
                    workflowRequestTableRecordsItem.Add("recordOrder", "0");
                    JSONArray workflowRequestTableFields = new JSONArray();


                    JSONObject workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "wlbm");
                    workflowRequestTableFieldsItem.Add("fieldValue", materialNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "wlmc");
                    workflowRequestTableFieldsItem.Add("fieldValue", materialName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "chlb");
                    workflowRequestTableFieldsItem.Add("fieldValue", materialTypeName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jjdw");
                    workflowRequestTableFieldsItem.Add("fieldValue", unitName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "c");
                    workflowRequestTableFieldsItem.Add("fieldValue", romQty);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "z");
                    workflowRequestTableFieldsItem.Add("fieldValue", toQty);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "dj");
                    workflowRequestTableFieldsItem.Add("fieldValue", price);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "hsdj");
                    workflowRequestTableFieldsItem.Add("fieldValue", taxPrice);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "gfdj");
                    workflowRequestTableFieldsItem.Add("fieldValue", publicWastePrice);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "gfhsdj");
                    workflowRequestTableFieldsItem.Add("fieldValue", publicWasteTaxPrice);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "lfdj");
                    workflowRequestTableFieldsItem.Add("fieldValue", wastePrice);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "lfhsdj");
                    workflowRequestTableFieldsItem.Add("fieldValue", wasteTaxPrice);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "sl");
                    workflowRequestTableFieldsItem.Add("fieldValue", taxRate);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jgxs");
                    workflowRequestTableFieldsItem.Add("fieldValue", pricecoefficient);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jgsx");
                    workflowRequestTableFieldsItem.Add("fieldValue", upPrice);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jgxx");
                    workflowRequestTableFieldsItem.Add("fieldValue", downPrice);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "sxrq");
                    workflowRequestTableFieldsItem.Add("fieldValue", effectiveDate);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "sxrqq");
                    workflowRequestTableFieldsItem.Add("fieldValue", expiryDate);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xqzz");
                    workflowRequestTableFieldsItem.Add("fieldValue", processOrgName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "zy");
                    workflowRequestTableFieldsItem.Add("fieldValue", processName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "bz");
                    workflowRequestTableFieldsItem.Add("fieldValue", note);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "jgly");
                    workflowRequestTableFieldsItem.Add("fieldValue", priceromName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "lydh");
                    workflowRequestTableFieldsItem.Add("fieldValue", romBillNo);
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
                    + "&requestName=价目表已到达"
                    + "&workflowId=15&detailData=" + detailData.ToString(), personId);

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
