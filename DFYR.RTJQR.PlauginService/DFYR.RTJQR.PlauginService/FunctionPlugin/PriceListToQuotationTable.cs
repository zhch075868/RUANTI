using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kingdee.BOS;
using Kingdee.BOS.Util;
using Kingdee.BOS.Core;
using Kingdee.BOS.Core.Metadata;
using Kingdee.BOS.Core.Metadata.ConvertElement;
using Kingdee.BOS.Core.Metadata.ConvertElement.PlugIn;
using Kingdee.BOS.Core.Metadata.ConvertElement.PlugIn.Args;
using Kingdee.BOS.Core.Metadata.EntityElement;
using Kingdee.BOS.Core.Metadata.FormElement;
using Kingdee.BOS.Core.SqlBuilder;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.BOS.Contracts;
using Kingdee.BOS.App;
using System.ComponentModel;
using Kingdee.BOS.ServiceHelper;

namespace DFYR.RTJQR.PlauginService.FunctionPlugin
{
    public class PriceListToQuotationTable : AbstractConvertPlugIn
    {
        /// <summary>        
        /// 主单据体的字段携带完毕，与源单的关联关系创建好之后，触发此事件    
        /// /// </summary>     
        /// /// <param name="e"></param>   
        public override void OnAfterCreateLink(CreateLinkEventArgs e)
        {
            Entity ProductionEntry = e.TargetBusinessInfo.GetEntity("FPriceListEntry");

             // 获取生成的全部下游单据 
            ExtendedDataEntity[] billDataEntitys = e.TargetExtendedDataEntities.FindByEntityKey("FBillHead");

            // 目标单关联子单据体        
            Entity linkEntity = null;
            Form form = e.TargetBusinessInfo.GetForm();
            if (form.LinkSet != null
                && form.LinkSet.LinkEntitys != null
                && form.LinkSet.LinkEntitys.Count != 0)
            {
                linkEntity = e.TargetBusinessInfo.GetEntity(
                    form.LinkSet.LinkEntitys[0].Key);
            }
            if (linkEntity == null)
            {
                return;
            }        
            // 对下游单据，逐张单据进行处理         
            foreach (var item in billDataEntitys)
            {
                decimal upper = Convert.ToDecimal(SystemParameterServiceHelper.GetParamter(this.Context, this.Context.CurrentOrganizationInfo.ID, 0, "PUR_SystemParameter", "F_PYEO_Integer_upper"));
                decimal lower = Convert.ToDecimal(SystemParameterServiceHelper.GetParamter(this.Context, this.Context.CurrentOrganizationInfo.ID, 0, "PUR_SystemParameter", "F_PYEO_Integer_lower"));

                DynamicObject dataObject = item.DataEntity;
                HashSet<long> srcBillIds = new HashSet<long>();
                // 开始到主单据体中，读取关联的源单内码         
                DynamicObjectCollection mainEntryRows = ProductionEntry.DynamicProperty.GetValue(dataObject) as DynamicObjectCollection;
                foreach (var mainEntityRow in mainEntryRows)
                {
                    DynamicObjectCollection linkRows = linkEntity.DynamicProperty.GetValue(mainEntityRow) as DynamicObjectCollection;
                    foreach (var linkRow in linkRows)
                    {
                        long srcBillId = Convert.ToInt64(linkRow["SBillId"]);
                        if (srcBillId != 0
                            && srcBillIds.Contains(srcBillId) == false)
                        {
                            srcBillIds.Add(srcBillId);
                        }
                    }
                }
                if (srcBillIds.Count == 0)
                {
                    continue;
                }      
                List<SelectorItemInfo> selector = new List<SelectorItemInfo>();
                selector.Add(new SelectorItemInfo("FBillTypeID"));

                string filter = string.Format(" {0} IN ({1}) ", e.SourceBusinessInfo.GetForm().PkFieldName, string.Join(",", srcBillIds));
                OQLFilter filterObj = OQLFilter.CreateHeadEntityFilter(filter);

                //原单数据
                IViewService viewService = ServiceHelper.GetService<IViewService>();
                DynamicObject srcBillObj = viewService.Load(this.Context,
                    e.SourceBusinessInfo.GetForm().Id,
                    selector,
                    filterObj)[0];   

                //根据单据类型判断采购类型
                DynamicObject BillTypeId = srcBillObj["BillTypeId"] as DynamicObject;
                string billTypeNumber = Convert.ToString(BillTypeId["Number"]);
                string purType = "1";
                if (billTypeNumber.Equals("BJD（ZDY）01"))
                {
                    purType = "2";
                }

                DynamicObjectCollection Entrys = ProductionEntry.DynamicProperty.GetValue(dataObject) as DynamicObjectCollection;
                foreach(DynamicObject Entry in Entrys )
                {
                    decimal price = Convert.ToDecimal(Entry["TaxPrice"]);
                    Entry["DownPrice"] = (1 - lower / 100) * price;
                    Entry["UpPrice"] = (1 + upper / 100) * price;
                }
            }
        }
    }
}
