using Kingdee.BOS;
using Kingdee.BOS.App;
using Kingdee.BOS.App.Data;
using Kingdee.BOS.Contracts;
using Kingdee.BOS.Core.Report;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.K3.SCM.App.Purchase.Report;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFYR.RTJQR.PlauginService.report
{
    [Kingdee.BOS.Util.HotUpdate, Description("采购明细执行扩展")]
    public class PurchaseOrderDetailRptEx : PurchaseOrderExecuteRpt
    {
        /// <summary>
        /// 主要取数逻辑方法
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="tableName"></param>
        public override void BuilderReportSqlAndTempTable(IRptParams filter, string tableName)
        {
            base.BuilderReportSqlAndTempTable(filter, tableName);
            //获取到原有的临时表内容
            IDBService service = ServiceHelper.GetService<IDBService>();
            string tmpTableName = service.CreateTemporaryTableName(this.Context);
            DBUtils.Execute(this.Context, string.Format("select * into {0} from {1}", tmpTableName, tableName));

            //用原有的表联查项目
            dorpTableName(tableName);
            setTmpData(tableName, tmpTableName, filter);

        }

        /// <summary>
        /// 清除系统原有临时表
        /// </summary>
        /// <param name="tableName"></param>
        private void dorpTableName(string tableName)
        {
            string dropSql = string.Format("drop table {0}", tableName);
            DBUtils.Execute(this.Context, dropSql);
        }

        /// <summary>
        /// 增加数据,并且添加过滤
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="tempTableName"></param>
        private void setTmpData(string tableName, string tempTableName, IRptParams filter)
        {
            DynamicObject customFilter = filter.FilterParameter.CustomFilter;
            string projectId = Convert.ToString(customFilter["F_PYEO_project_Id"]);
            //string xqdh = Convert.ToString(customFilter["F_VRGO_KHXQDH"]);

            //string projectNumber = projectId == null ? "" : Convert.ToString(projectId["Number"]);
            StringBuilder sql = new StringBuilder(string.Format(@"/*dialect*/select 
                                   e.F_SRT_PROJECT1 as F_SRT_Project,
	                               t.* into {0}
                            from {1} t left join t_PUR_POOrderEntry e on t.Forderid = e.FENTRYID
                            where 1=1 ", tableName, tempTableName));
            if (!projectId.Equals("0"))
            {
                sql.AppendFormat(" and e.F_SRT_PROJECT1 = '{0}'", projectId);
            }

            //Utils.WriteLog(sql.ToString());
            DBUtils.Execute(this.Context, sql.ToString());
        }

    }
}
