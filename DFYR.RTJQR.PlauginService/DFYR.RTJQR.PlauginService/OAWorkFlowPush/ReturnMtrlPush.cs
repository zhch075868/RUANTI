using Kingdee.BOS;
using Kingdee.BOS.App.Data;
using Kingdee.BOS.Core.DynamicForm;
using Kingdee.BOS.Core.DynamicForm.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Core.Metadata;
using Kingdee.BOS.Core.Metadata.FieldElement;
using Kingdee.BOS.JSON;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.BOS.Orm.Metadata.DataEntity;
using Kingdee.BOS.ServiceHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFYR.RTJQR.PlauginService.OAWorkFlowPush
{
    [Kingdee.BOS.Util.HotUpdate, Description("生产退料推送至OA")]
    public class ReturnMtrlPush : AbstractOperationServicePlugIn
    {public override void OnPreparePropertys(PreparePropertysEventArgs e)
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
            Utils.token = "";
            foreach (DynamicObject item in e.DataEntitys)
            {
                DynamicObject o = BusinessDataServiceHelper.LoadSingle(
                               this.Context,
                               item["Id"].ToString(),
                               this.BusinessInfo.GetDynamicObjectType());

                string id = Convert.ToString(o["Id"]);
                string documentStatus = Convert.ToString(o["DocumentStatus"]);

                //提交校验
                if (!documentStatus.Equals("B"))
                {
                    return;
                }

                string BillNo = Convert.ToString(o["BillNo"]);
                DynamicObject BillType = o["BillType"] as DynamicObject;
                string BillTypeName = BillType == null ? "" : Convert.ToString(BillType["Name"]);
                string Date = Convert.ToDateTime(o["Date"]).ToString("yyyy-MM-dd");
                DynamicObject StockOrgId = o["StockOrgId"] as DynamicObject;
                string StockOrgIdName = StockOrgId == null ? "" : Convert.ToString(StockOrgId["F_PYEO_Text_OAID"]);
                DynamicObject StockId = o["StockId"] as DynamicObject;
                string StockIdName = StockId == null ? "" : Convert.ToString(StockId["Name"]);
                DynamicObject PrdOrgId = o["PrdOrgId"] as DynamicObject;
                string PrdOrgIdName = PrdOrgId == null ? "" : Convert.ToString(PrdOrgId["F_PYEO_Text_OAID"]);
                DynamicObject WorkShopId = o["WorkShopId"] as DynamicObject;
                string WorkShopIdName = WorkShopId == null ? "" : Convert.ToString(WorkShopId["Name"]);
                DynamicObject ReturnerId = o["ReturnerId"] as DynamicObject;
                string ReturnerIdName = ReturnerId == null ? "" : Convert.ToString(ReturnerId["Number"]);
                ReturnerIdName = Utils.getPersonOAid(this.Context,ReturnerIdName);
                DynamicObject FSTOCKERID = o["FSTOCKERID"] as DynamicObject;
                string FSTOCKERIDName = FSTOCKERID == null ? "" : Convert.ToString(FSTOCKERID["Number"]);
                FSTOCKERIDName = Utils.getPersonOAid(this.Context, FSTOCKERIDName);
                DynamicObject F_SRT_Project = o["F_SRT_Project"] as DynamicObject;

                string F_SRT_ProjectName = F_SRT_Project == null ? "" : Convert.ToString(F_SRT_Project["Name"]);
                DynamicObject F_SRT_SCZXM = o["F_SRT_SCZXM"] as DynamicObject;
                string F_SRT_SCZXMName = F_SRT_SCZXM == null ? "" : Convert.ToString(F_SRT_SCZXM["Name"]);
                DynamicObject F_SRT_CustId = o["F_SRT_CustId"] as DynamicObject;
                string F_SRT_CustIdName = F_SRT_CustId == null ? "" : Convert.ToString(F_SRT_CustId["Name"]);
                DynamicObject F_SRT_ZDKH = o["F_SRT_ZDKH"] as DynamicObject;
                string F_SRT_ZDKHName = F_SRT_ZDKH == null ? "" : Convert.ToString(F_SRT_ZDKH["Name"]);
                DynamicObject F_SRT_DQ = o["F_SRT_DQ"] as DynamicObject;
                string F_SRT_DQName = F_SRT_DQ == null ? "" : Convert.ToString(F_SRT_DQ["FDataValue"]);
                DynamicObject F_SRT_HY = o["F_SRT_HY"] as DynamicObject;
                string F_SRT_HYName = F_SRT_HY == null ? "" : Convert.ToString(F_SRT_HY["FDataValue"]);
                DynamicObject F_PYEO_SaleDeptId = o["F_PYEO_SaleDeptId"] as DynamicObject;
                string F_PYEO_SaleDeptIdName = F_PYEO_SaleDeptId == null ? "" : Convert.ToString(F_PYEO_SaleDeptId["F_PYEO_Text_OAID"]);
                DynamicObject F_SRT_MYLX = o["F_SRT_MYLX"] as DynamicObject;
                string F_SRT_MYLXName = F_SRT_MYLX == null ? "" : Convert.ToString(F_SRT_MYLX["Name"]);
                DynamicObject F_SRT_XSLX = o["F_SRT_XSLX"] as DynamicObject;
                string F_SRT_XSLXName = F_SRT_XSLX == null ? "" : Convert.ToString(F_SRT_XSLX["Name"]);
                DynamicObject F_SRT_TD = o["F_SRT_TD"] as DynamicObject;
                string F_SRT_TDName = F_SRT_TD == null ? "" : Convert.ToString(F_SRT_TD["Name"]);
                DynamicObject F_SRT_CPFL = o["F_SRT_CPFL"] as DynamicObject;
                string F_SRT_CPFLName = F_SRT_CPFL == null ? "" : Convert.ToString(F_SRT_CPFL["Name"]);
                DynamicObject F_SRT_XSQD = o["F_SRT_XSQD"] as DynamicObject;
                string F_SRT_XSQDName = F_SRT_XSQD == null ? "" : Convert.ToString(F_SRT_XSQD["Name"]);

                JSONArray mainRoot = new JSONArray();
                JSONObject mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "djbh");
                mainRootItem.Add("fieldValue", BillNo);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "erpnumber");
                mainRootItem.Add("fieldValue", BillNo);
                mainRoot.Add(mainRootItem);
                

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "djlx");
                mainRootItem.Add("fieldValue", BillTypeName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "rq");
                mainRootItem.Add("fieldValue", Date);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "slzz");
                mainRootItem.Add("fieldValue", StockOrgIdName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "ck");
                mainRootItem.Add("fieldValue", StockIdName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "sczz");
                mainRootItem.Add("fieldValue", PrdOrgIdName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "sccj");
                mainRootItem.Add("fieldValue", WorkShopIdName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "tlr");
                mainRootItem.Add("fieldValue", ReturnerIdName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "cgy");
                mainRootItem.Add("fieldValue", FSTOCKERIDName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xm");
                mainRootItem.Add("fieldValue", F_SRT_ProjectName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "yfzxm");
                mainRootItem.Add("fieldValue", F_SRT_SCZXMName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "kh");
                mainRootItem.Add("fieldValue", F_SRT_CustIdName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "zdkh");
                mainRootItem.Add("fieldValue", F_SRT_ZDKHName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "dq");
                mainRootItem.Add("fieldValue", F_SRT_DQName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xy");
                mainRootItem.Add("fieldValue", F_SRT_HYName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xsbm");
                mainRootItem.Add("fieldValue", F_PYEO_SaleDeptIdName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "mylx");
                mainRootItem.Add("fieldValue", F_SRT_MYLXName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xslx");
                mainRootItem.Add("fieldValue", F_SRT_XSLXName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "td");
                mainRootItem.Add("fieldValue", F_SRT_TDName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "cpfl");
                mainRootItem.Add("fieldValue", F_SRT_CPFLName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "xsqd");
                mainRootItem.Add("fieldValue", F_SRT_XSQDName);
                mainRoot.Add(mainRootItem);

                JSONArray detailData = new JSONArray();

                JSONObject detailDataItem = new JSONObject();
                detailDataItem.Add("tableDBName", "formtable_main_68_dt1");
                JSONArray workflowRequestTableRecords = new JSONArray();

                DynamicObjectCollection materials = o["Entity"] as DynamicObjectCollection;
                foreach (DynamicObject material in materials)
                {
                    DynamicObject MaterialId = material["MaterialId"] as DynamicObject;
                    string MaterialNumber = MaterialId == null ? "" : Convert.ToString(MaterialId["Number"]);
                    string MaterialName = MaterialId == null ? "" : Convert.ToString(MaterialId["Name"]);
                    string Specification = MaterialId == null ? "" : Convert.ToString(MaterialId["Specification"]);
                    DynamicObject UnitId = material["UnitId"] as DynamicObject;
                    string UnitIdNumber = UnitId == null ? "" : Convert.ToString(UnitId["Name"]);
                    string AppQty = Convert.ToDecimal(material["AppQty"]).ToString("#0.00");
                    string Qty = Convert.ToDecimal(material["Qty"]).ToString("#0.00");
                    var field = this.BusinessInfo.GetField("FReturnType") as ComboField; ;
                    var enumValue = material["ReturnType"];
                    var ReturnType = field.GetEnumItemName(enumValue);
                    
                    DynamicObject ReturnReason = material["ReturnReason"] as DynamicObject;
                    string ReturnReasonName = Convert.ToString(ReturnReason["FDataValue"]);
                    DynamicObject entryStockId = material["StockId"] as DynamicObject;
                    string entryStockIdName = entryStockId == null ? "" : Convert.ToString(entryStockId["Name"]);
                    DynamicObject stockLocId = material["StockLocId"] as DynamicObject;
                    string stockLocIdName = getStocklocString(stockLocId);

                    DynamicObject Lot = material["Lot"] as DynamicObject;
                    string LotText = Lot == null ? "" : Convert.ToString(Lot["Number"]);
                    string MoBillNo = Convert.ToString(material["MoBillNo"]);
                    DynamicObject EquipmentId = material["EquipmentId"] as DynamicObject;
                    string EquipmentIdName = EquipmentId == null ? "" : Convert.ToString(EquipmentId["Name"]);
                    string IsUpdateQty = Convert.ToString(material["IsUpdateQty"]).Equals("True") ? "是" : "否";
                    DynamicObject WorkShopId1 = material["WorkShopId1"] as DynamicObject;
                    string WorkShopId1Name = WorkShopId1 == null ? "" : Convert.ToString(WorkShopId1["Name"]);
                    DynamicObject F_SRT_MXCPFL = material["F_SRT_MXCPFL"] as DynamicObject;
                    string F_SRT_MXCPFLName = F_SRT_MXCPFL == null ? "" : Convert.ToString(F_SRT_MXCPFL["Name"]);
                    DynamicObject F_SRT_CHDL = material["F_SRT_CHDL"] as DynamicObject;
                    string F_SRT_CHDLName = F_SRT_CHDL == null ? "" : Convert.ToString(F_SRT_CHDL["Name"]);


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
                    workflowRequestTableFieldsItem.Add("fieldName", "dw");
                    workflowRequestTableFieldsItem.Add("fieldValue", UnitIdNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "sqsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", AppQty);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "stsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", Qty);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "tllx");
                    workflowRequestTableFieldsItem.Add("fieldValue", ReturnType);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "tlyy");
                    workflowRequestTableFieldsItem.Add("fieldValue", ReturnReasonName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "ck");
                    workflowRequestTableFieldsItem.Add("fieldValue", entryStockIdName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "cw");
                    workflowRequestTableFieldsItem.Add("fieldValue", stockLocIdName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "ph");
                    workflowRequestTableFieldsItem.Add("fieldValue", LotText);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "scddbh");
                    workflowRequestTableFieldsItem.Add("fieldValue", MoBillNo);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "sb");
                    workflowRequestTableFieldsItem.Add("fieldValue", EquipmentIdName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "bgxwl");
                    workflowRequestTableFieldsItem.Add("fieldValue", IsUpdateQty);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "cj");
                    workflowRequestTableFieldsItem.Add("fieldValue", WorkShopId1Name);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "mxcpfl");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_SRT_MXCPFLName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "chdl");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_SRT_CHDLName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

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
                        + "&requestName=生产退料单已到达"
                        + "&workflowId=24&detailData=" + detailData.ToString(), personId);

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

        /// <summary>
        /// 获取仓位信息
        /// </summary>
        /// <param name="stockLocId"></param>
        /// <returns></returns>
        private string getStocklocString(DynamicObject stockLocId)
        {
            if (null == stockLocId)
            {
                return "";
            }
            StringBuilder stb = new StringBuilder();
            DynamicPropertyCollection keys = stockLocId.DynamicObjectType.Properties;
            string id, stringId, key = "";
            //手动使用for循环判断对辅助属性象中是否有该key值，并且判断是否有值
            for (int x = 1; x <= keys.Count; x++)
            {
                id = string.Format(x > 9 ? "F1000{0}_Id" : "F10000{0}_Id", x);//对象类型的id
                stringId = string.Format(x > 9 ? "F1000{0}" : "F10000{0}", x);//手动输入类型的ID
                //key = string.Format(x > 9 ? "FF1000{0}" : "FF10000{0}", x);//赋值用的key
                //原有的对象类型的复制
                if (stockLocId.DynamicObjectType.Properties.Contains(id))//判断是否含有该key,如果有就说明是对象型的
                {
                    DynamicObject itemId = stockLocId[stringId] as DynamicObject;
                    if (null != itemId)
                    {
                        string itemName = Convert.ToString(itemId["Name"]);
                        stb.Append(itemName + "|");
                    }
                }
                else
                {
                    //原有的手动输入类型的复制
                    if (stockLocId.DynamicObjectType.Properties.Contains(stringId))//如果上述没有，在判断是否是手动输入类型的
                    {
                        string itemName = Convert.ToString(stockLocId[stringId]);
                        stb.Append(itemName + "|");
                    }
                }
            }
            return stb.ToString();

        }
    }
}
