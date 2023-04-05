using Kingdee.BOS;
using Kingdee.BOS.App.Data;
using Kingdee.BOS.Core.DynamicForm;
using Kingdee.BOS.Core.DynamicForm.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Core.Metadata;
using Kingdee.BOS.Core.Metadata.FieldElement;
using Kingdee.BOS.JSON;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.BOS.ServiceHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFYR.RTJQR.PlauginService.OAWorkFlowPush
{
    [Kingdee.BOS.Util.HotUpdate, Description("生产挪料推送至OA")]
    public class MovemtrlPush : AbstractOperationServicePlugIn
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
                DynamicObject PrdOrgId = o["PrdOrgId"] as DynamicObject;
                string PrdOrgIdName = PrdOrgId == null ? "" : Convert.ToString(PrdOrgId["F_PYEO_Text_OAID"]);
                DynamicObject WorkShopId = o["WorkShopId"] as DynamicObject;
                string WorkShopIdName = WorkShopId == null ? "" : Convert.ToString(WorkShopId["Name"]);
                DynamicObject StockId = o["StockId"] as DynamicObject;
                string StockIdName = StockId == null ? "" : Convert.ToString(StockId["Name"]);
                var field = this.BusinessInfo.GetField("FBUSINESSTYPE") as ComboField; ;
                var enumValue = o["BUSINESSTYPE"];
                var BUSINESSTYPE = field.GetEnumItemName(enumValue);
                field = this.BusinessInfo.GetField("FMOVEOUTTYPE") as ComboField; ;
                enumValue = o["MOVEOUTTYPE"];
                string MOVEOUTTYPE = field.GetEnumItemName(enumValue);
                field = this.BusinessInfo.GetField("FMOVEINTYPE") as ComboField; ;
                enumValue = o["MOVEINTYPE"];
                string MOVEINTYPE = field.GetEnumItemName(enumValue);
                string MOVESETQTY = Convert.ToDecimal(o["MOVESETQTY"]).ToString("#0.00");
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
                DynamicObject F_PYEO_SaleDeptId = o["F_SaleDeptId"] as DynamicObject;
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
                mainRootItem.Add("fieldName", "djrq");
                mainRootItem.Add("fieldValue", Date);
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
                mainRootItem.Add("fieldName", "zzck");
                mainRootItem.Add("fieldValue", StockIdName);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "ywlx");
                mainRootItem.Add("fieldValue", BUSINESSTYPE);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "zclx");
                mainRootItem.Add("fieldValue", MOVEOUTTYPE);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "zrlx");
                mainRootItem.Add("fieldValue", MOVEINTYPE);
                mainRoot.Add(mainRootItem);

                mainRootItem = new JSONObject();
                mainRootItem.Add("fieldName", "nlts");
                mainRootItem.Add("fieldValue", MOVESETQTY);
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
                detailDataItem.Add("tableDBName", "formtable_main_71_dt1");
                JSONArray workflowRequestTableRecords = new JSONArray();

                DynamicObjectCollection materials = o["Entity"] as DynamicObjectCollection;
                foreach (DynamicObject material in materials)
                {
                    field = this.BusinessInfo.GetField("FROWTYPE") as ComboField; ;
                    enumValue = material["ROWTYPE"];
                    string ROWTYPE = field.GetEnumItemName(enumValue);
                    string MOBILLNO = Convert.ToString(material["MOBILLNO"]);
                    string MOENTRYSEQ = Convert.ToString(material["MOENTRYSEQ"]);
                    DynamicObject MaterialId = material["MaterialId"] as DynamicObject;
                    string MaterialNumber = MaterialId == null ? "" : Convert.ToString(MaterialId["Number"]);
                    string MaterialName = MaterialId == null ? "" : Convert.ToString(MaterialId["Name"]);
                    string Specification = MaterialId == null ? "" : Convert.ToString(MaterialId["Specification"]);
                    DynamicObject MATERIALCHILDID = material["MATERIALCHILDID"] as DynamicObject;
                    string MaterialchildNumber = MATERIALCHILDID == null ? "" : Convert.ToString(MATERIALCHILDID["Number"]);
                    string MaterialchildName = MATERIALCHILDID == null ? "" : Convert.ToString(MATERIALCHILDID["Name"]);
                    string MaterialchildSpecification = MATERIALCHILDID == null ? "" : Convert.ToString(MATERIALCHILDID["Specification"]);

                    DynamicObject UnitId = material["UnitId"] as DynamicObject;
                    string UnitIdNumber = UnitId == null ? "" : Convert.ToString(UnitId["Name"]);
                    string MUSTQTY = Convert.ToDecimal(material["MUSTQTY"]).ToString("#0.00");
                    string PICKEDQTY = Convert.ToDecimal(material["PICKEDQTY"]).ToString("#0.00");
                    string RETURNQTY = Convert.ToDecimal(material["RETURNQTY"]).ToString("#0.00");
                    string WIPQTY = Convert.ToDecimal(material["WIPQTY"]).ToString("#0.00");
                    string CANMOVEQTY = Convert.ToDecimal(material["CANMOVEQTY"]).ToString("#0.00");
                    string MOVEQTY = Convert.ToDecimal(material["MOVEQTY"]).ToString("#0.00");
                    field = this.BusinessInfo.GetField("FOverControlMode") as ComboField; ;
                    enumValue = material["OverControlMode"];
                    string OverControlMode = field.GetEnumItemName(enumValue);
                    DynamicObject F_SRT_MXCPFL = material["F_SRT_MXCPFL"] as DynamicObject;
                    string F_SRT_MXCPFLNumber = F_SRT_MXCPFL == null ? "" : Convert.ToString(F_SRT_MXCPFL["Name"]);
                    DynamicObject F_SRT_CHDL = material["F_SRT_CHDL"] as DynamicObject;
                    string F_SRT_CHDLNumber = F_SRT_CHDL == null ? "" : Convert.ToString(F_SRT_CHDL["Name"]);


                    JSONObject workflowRequestTableRecordsItem = new JSONObject();
                    workflowRequestTableRecordsItem.Add("recordOrder", "0");
                    JSONArray workflowRequestTableFields = new JSONArray();

                    JSONObject workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xlx");
                    workflowRequestTableFieldsItem.Add("fieldValue", ROWTYPE);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "scdd");
                    workflowRequestTableFieldsItem.Add("fieldValue", MOBILLNO);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "xh");
                    workflowRequestTableFieldsItem.Add("fieldValue", MOENTRYSEQ);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "cpbm");
                    workflowRequestTableFieldsItem.Add("fieldValue", MaterialNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "cpmc");
                    workflowRequestTableFieldsItem.Add("fieldValue", MaterialName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "cpgg");
                    workflowRequestTableFieldsItem.Add("fieldValue", Specification);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "zxbm");
                    workflowRequestTableFieldsItem.Add("fieldValue", MaterialchildNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "zxmc");
                    workflowRequestTableFieldsItem.Add("fieldValue", MaterialchildName);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "zxgg");
                    workflowRequestTableFieldsItem.Add("fieldValue", MaterialchildSpecification);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "zxdw");
                    workflowRequestTableFieldsItem.Add("fieldValue", UnitIdNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "yfsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", MUSTQTY);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "ylsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", PICKEDQTY);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "tlsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", RETURNQTY);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "zzclsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", WIPQTY);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "knsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", CANMOVEQTY);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "nlsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", MOVEQTY);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "zrcfsl");
                    workflowRequestTableFieldsItem.Add("fieldValue", OverControlMode);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "mxcpfl");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_SRT_MXCPFLNumber);
                    workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                    workflowRequestTableFieldsItem = new JSONObject();
                    workflowRequestTableFieldsItem.Add("fieldName", "chdl");
                    workflowRequestTableFieldsItem.Add("fieldValue", F_SRT_CHDLNumber);
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
                        + "&requestName=生产挪料单已到达"
                        + "&workflowId=27&detailData=" + detailData.ToString(), personId);

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
