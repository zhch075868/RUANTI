using Kingdee.BOS.App.Data;
using Kingdee.BOS.Core.DynamicForm;
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

namespace DFYR.RTJQR.PlauginService.OAWorkFlowPush
{
    [Kingdee.BOS.Util.HotUpdate, Description("推送采购入库到OA")]
    public class InstockPush : AbstractOperationServicePlugIn
    {
        public override void OnPreparePropertys(PreparePropertysEventArgs e)
        {
            //e.FieldKeys.Add("");将需要应用的字段Key加入
            e.FieldKeys.Add("FStockOrgId");
            e.FieldKeys.Add("FStockerId");
            e.FieldKeys.Add("FSupplierId");
            e.FieldKeys.Add("FMaterialId");
            e.FieldKeys.Add("F_SRT_PROJECT");
            e.FieldKeys.Add("FRealQty");
            e.FieldKeys.Add("FUnitID");
            e.FieldKeys.Add("FStockId");
            e.FieldKeys.Add("F_PYEO_base_D");
            e.FieldKeys.Add("FPurchaseOrgId");
            e.FieldKeys.Add("FDate");
            e.FieldKeys.Add("FSupplierId");

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
            Utils.WriteLog("进入入库消息插件");
            Utils.token = "";
            foreach (DynamicObject o in e.DataEntitys)
            {
                string id = Convert.ToString(o["Id"]);
                string billNo = Convert.ToString(o["BillNo"]);

                List<string> reqList = this.getRequestList(id);
                foreach (string xqr in reqList)
                {

                    Utils.WriteLog("进入入库循环");
                    JSONArray mainRoot = new JSONArray();
                    JSONObject mainRootItem = new JSONObject();
                    mainRootItem.Add("fieldName", "djbh");
                    mainRootItem.Add("fieldValue", billNo);
                    mainRoot.Add(mainRootItem);

                    DynamicObject stockOrgId = o["StockOrgId"] as DynamicObject;
                    string stockOrgName = Convert.ToString(stockOrgId["Name"]);
                    mainRootItem = new JSONObject();
                    mainRootItem.Add("fieldName", "slzz");
                    mainRootItem.Add("fieldValue", stockOrgName);
                    mainRoot.Add(mainRootItem);

                    DynamicObject stockerId = o["StockerId"] as DynamicObject;
                    string stockerName = stockerId == null? "":Convert.ToString(stockerId["Name"]);
                    mainRootItem = new JSONObject();
                    mainRootItem.Add("fieldName", "cgy");
                    mainRootItem.Add("fieldValue", stockerName);
                    mainRoot.Add(mainRootItem);

                    DynamicObject supplierId = o["SupplierId"] as DynamicObject;
                    string supplierName = supplierId == null?"":Convert.ToString(supplierId["Name"]);
                    mainRootItem = new JSONObject();
                    mainRootItem.Add("fieldName", "gys");
                    mainRootItem.Add("fieldValue", supplierName);
                    mainRoot.Add(mainRootItem);

                    //根据需求人拆单提交流程
                    mainRootItem = new JSONObject();
                    mainRootItem.Add("fieldName", "xqr");
                    mainRootItem.Add("fieldValue", xqr);
                    mainRoot.Add(mainRootItem);

                    JSONArray detailData = new JSONArray();

                    JSONObject detailDataItem = new JSONObject();
                    detailDataItem.Add("tableDBName", "formtable_main_54_dt1");
                    JSONArray workflowRequestTableRecords = new JSONArray();

                    DynamicObjectCollection entrys = o["InStockEntry"] as DynamicObjectCollection ;
                    string personName = "";
                    int i = 0;
                    List<string> entryIdLlist = new List<string>();
                    foreach (DynamicObject entry in entrys)
                    {
                        string entryId = Convert.ToString(entry["Id"]);
                        DynamicObject xqrrg = entry["F_PYEO_base_D"] as DynamicObject;
                        DynamicObject xqrObj = xqrrg["EmpInfoId"] as DynamicObject ;
                        string xqrid = Convert.ToString(xqrObj["Id"]);
                        DynamicObjectCollection xqridResults = DBUtils.ExecuteDynamicObject(this.Context, string.Format("select F_PYEO_TEXT_OAID from T_HR_EMPINFO where FID = {0}", xqrid));
                        string xqrNumber = Convert.ToString(xqridResults[0]["F_PYEO_TEXT_OAID"]);
                        personName = Convert.ToString(xqrObj["Name"]);
                        if (!xqrNumber.Equals(xqr))
                        {
                            continue;
                        }

                        JSONObject workflowRequestTableRecordsItem = new JSONObject();
                        workflowRequestTableRecordsItem.Add("recordOrder", i);
                        JSONArray workflowRequestTableFields = new JSONArray();

                        DynamicObject materialId = entry["MaterialId"] as DynamicObject;
                        string materialNumber = Convert.ToString(materialId["Number"]);
                        JSONObject workflowRequestTableFieldsItem = new JSONObject();
                        workflowRequestTableFieldsItem.Add("fieldName", "wlbm");
                        workflowRequestTableFieldsItem.Add("fieldValue", materialNumber);
                        workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                        string materialName = Convert.ToString(materialId["Name"]);
                        workflowRequestTableFieldsItem = new JSONObject();
                        workflowRequestTableFieldsItem.Add("fieldName", "wlmc");
                        workflowRequestTableFieldsItem.Add("fieldValue", materialName);
                        workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                        string materialSpecification = Convert.ToString(materialId["Specification"]);
                        workflowRequestTableFieldsItem = new JSONObject();
                        workflowRequestTableFieldsItem.Add("fieldName", "ggxh");
                        workflowRequestTableFieldsItem.Add("fieldValue", materialSpecification);
                        workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                        DynamicObject project = entry["F_SRT_PROJECT"] as DynamicObject;
                        string projectName = project == null?"":Convert.ToString(project["Name"]);
                        workflowRequestTableFieldsItem = new JSONObject();
                        workflowRequestTableFieldsItem.Add("fieldName", "xm");
                        workflowRequestTableFieldsItem.Add("fieldValue", projectName);
                        workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                        decimal realQty = Convert.ToDecimal(entry["RealQty"]);
                        workflowRequestTableFieldsItem = new JSONObject();
                        workflowRequestTableFieldsItem.Add("fieldName", "sssl");
                        workflowRequestTableFieldsItem.Add("fieldValue", realQty);
                        workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                        DynamicObject unit = entry["UnitID"] as DynamicObject;
                        string unitName = unit == null?"": Convert.ToString(unit["Name"]);
                        workflowRequestTableFieldsItem = new JSONObject();
                        workflowRequestTableFieldsItem.Add("fieldName", "jldw");
                        workflowRequestTableFieldsItem.Add("fieldValue", unitName);
                        workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                        DynamicObject stock = entry["StockId"] as DynamicObject;
                        string stockName = stock == null?"":Convert.ToString(stock["Name"]);
                        workflowRequestTableFieldsItem = new JSONObject();
                        workflowRequestTableFieldsItem.Add("fieldName", "ck");
                        workflowRequestTableFieldsItem.Add("fieldValue", stockName);
                        workflowRequestTableFields.Add(workflowRequestTableFieldsItem);

                        workflowRequestTableRecordsItem.Add("workflowRequestTableFields", workflowRequestTableFields);
                        workflowRequestTableRecords.Add(workflowRequestTableRecordsItem);

                        entryIdLlist.Add(entryId);
                    }

                    detailDataItem.Add("workflowRequestTableRecords", workflowRequestTableRecords);
                    detailData.Add(detailDataItem);

                    //拼接标题
                    DynamicObject orgId = o["PurchaseOrgId"] as DynamicObject;
                    string orgName = Convert.ToString(orgId["Name"]);//组织名称
                    string date = Convert.ToDateTime(o["Date"]).ToString("yyyy年MM月dd日");
                    DynamicObject purId = o["SupplierId"] as DynamicObject;
                    string purName = Convert.ToString(purId["Name"]);

                    StringBuilder requestName = new StringBuilder();
                    requestName.Append("采购入库单-");
                    requestName.Append(orgName+"-");
                    requestName.Append(date+"-");
                    requestName.Append(purName+"-");
                    requestName.Append(billNo);

                    string resultStr = Utils.wkPostUrl(Utils.pushAddWF,
                        "mainData=" + mainRoot.ToString()
                        + "&requestName=" + requestName.ToString()
                        + "&workflowId=13&detailData=" + detailData.ToString(), xqr);

                    JSONObject resultJson = JSONObject.Parse(resultStr);
                    string code = Convert.ToString(resultJson["code"]);
                    
                    if (code.Equals("SUCCESS"))
                    {
                        //更新传递标识
                        int x = 1;
                        StringBuilder stb = new StringBuilder();
                        foreach(string item in entryIdLlist)
                        {
                            if (x == entryIdLlist.Count)
                            {
                                stb.Append("'"+item+"'");
                            }
                            else
                            {
                                stb.Append("'" + item + "',");
                            }
                            x++;
                        }
                        string sql = "/*dialect*/update T_STK_INSTOCKENTRY set FISTOOA = 1 where fentryid in (" + stb.ToString() + ")";
                        DBUtils.Execute(this.Context,sql);
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
                            Message = "员工【" + personName + "】的提交流程失败：" + errMsg,
                            Name = "提交OA流程返回",
                            SuccessStatus = false,
                        });
                    }

                }
            }
        }

        /// <summary>
        /// 获取申请人集合
        /// </summary>
        /// <param name="billId"></param>
        /// <returns></returns>
        public List<string> getRequestList(string billId) 
        {
            List<string> retList = new List<string>();
            string sql = string.Format(@"SELECT em.F_PYEO_TEXT_OAID xqr 
                                            FROM t_STK_InStock t 
                                            LEFT OUTER JOIN T_STK_INSTOCKENTRY e ON t.FID = e.FID 
                                            LEFT OUTER JOIN T_BD_STAFF bs ON e.F_PYEO_base_D = bs.FSTAFFID
                                            left join T_HR_EMPINFO em on em.FID = bs.FEMPINFOID
                                         WHERE (t.FID = {0} AND e.FISTOOA = 0) GROUP BY em.F_PYEO_TEXT_OAID", billId);
            DynamicObjectCollection sqlresults = DBUtils.ExecuteDynamicObject(this.Context,sql);
            foreach(DynamicObject sqlresult in sqlresults)
            {
                string needPeople = Convert.ToString(sqlresult["xqr"]);
                retList.Add(needPeople);
            }
            return retList;
        } 

    }
}
