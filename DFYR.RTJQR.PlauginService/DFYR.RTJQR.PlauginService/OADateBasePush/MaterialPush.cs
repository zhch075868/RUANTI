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
    [Kingdee.BOS.Util.HotUpdate, Description("推送物料至OA")]
    public class MaterialPush : AbstractOperationServicePlugIn
    {
        public override void OnPreparePropertys(PreparePropertysEventArgs e)
        {
            //e.FieldKeys.Add("");将需要应用的字段Key加入
            e.FieldKeys.Add("F_PYEO_CHECKBOX_OA");
            e.FieldKeys.Add("FSpecification");
            e.FieldKeys.Add("FMaterialGroup");
            e.FieldKeys.Add("FBaseUnitId");
            e.FieldKeys.Add("FAuxPropertyId");
            e.FieldKeys.Add("FIsEnable1");

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
                mainTable.Add("wlmc", name);
                mainTable.Add("wlbm", number);

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
                    if (isOa.Equals("1"))
                    {
                        return;
                    }
                    mainTable.Add("zt", "0");
                    string specification = Convert.ToString(o["Specification"]);
                    DynamicObject materialGroup = o["MaterialGroup"] as DynamicObject;
                    string groupName = Convert.ToString(materialGroup["Name"]);
                    DynamicObject F_PYEO_BASE_ic = o["F_PYEO_BASE_ic"] as DynamicObject;
                    string F_PYEO_BASE_icName = Convert.ToString(F_PYEO_BASE_ic["Name"]);

                    mainTable.Add("ggxh", specification);
                    mainTable.Add("wlfz", groupName);
                    mainTable.Add("chdl", F_PYEO_BASE_icName);

                    DynamicObjectCollection materialBases = o["MaterialBase"] as DynamicObjectCollection;
                    if (materialBases.Count != 0)
                    {
                        DynamicObject baseUnitId = materialBases[0]["BaseUnitId"] as DynamicObject;
                        string baseUnitNumber = Convert.ToString(baseUnitId["Number"]);
                        mainTable.Add("jldw", baseUnitNumber);
                    }

                    JSONArray detail1 = new JSONArray();
                    DynamicObjectCollection materialAuxPtys = o["MaterialAuxPty"] as DynamicObjectCollection;
                    foreach (DynamicObject materialAuxPty in materialAuxPtys)
                    {
                        JSONObject materialAuxPtyItem = new JSONObject();
                        JSONObject materialAuxPtyData = new JSONObject();
                        JSONObject operate = new JSONObject();
                        operate.Add("action", "SaveOrUpdate");
                        operate.Add("actionDescribe", "Save");
                        materialAuxPtyItem.Add("operate", operate);

                        DynamicObject AuxPropertyId = materialAuxPty["AuxPropertyId"] as DynamicObject;
                        string AuxPropertyName = Convert.ToString(AuxPropertyId["Name"]);
                        string isEnable1 = Convert.ToString(materialAuxPty["IsEnable1"]);

                        materialAuxPtyData.Add("fzzx", AuxPropertyName);
                        materialAuxPtyData.Add("sfqy", isEnable1.Equals("True")?"0":"1");

                        materialAuxPtyItem.Add("data", materialAuxPtyData);
                        detail1.Add(materialAuxPtyItem);
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

                string results = Utils.PostUrl(Utils.pushWLurl, "datajson=" + dataJson.ToString());

                JSONObject resultJson = JSONObject.Parse(results);
                string retCode = Convert.ToString(resultJson["status"]);

                if (retCode.Equals("1"))
                {
                    string sql = string.Format("update T_BD_MATERIAL set F_PYEO_CHECKBOX_OA = 1 where FMATERIALID = {0}", id);
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
