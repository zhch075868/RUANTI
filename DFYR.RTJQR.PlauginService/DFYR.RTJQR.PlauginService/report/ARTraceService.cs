using Kingdee.BOS;
using Kingdee.BOS.App;
using Kingdee.BOS.App.Data;
using Kingdee.BOS.Contracts;
using Kingdee.BOS.Core.Report;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.K3.CRM.OPP.APP.Report;
using Kingdee.K3.SCM.App.Purchase.Report;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFYR.RTJQR.PlauginService.report
{
    [Kingdee.BOS.Util.HotUpdate, Description("应收跟踪表推送至OA")]
    public class ARTraceService : Kingdee.K3.FIN.AR.App.Report.TraceReport
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
            string ht = Convert.ToString(customFilter["F_PYEO_HT_Id"]);
            string td = Convert.ToString(customFilter["F_PYEO_TD_Id"]);

            //string projectNumber = projectId == null ? "" : Convert.ToString(projectId["Number"]);
            StringBuilder sql = new StringBuilder(string.Format(@"/*dialect*/select 
                                   e.F_SRT_HTH as F_PYEO_HT,
                                   e.F_SRT_TD as F_PYEO_TD,
                                   ott.FCARRYBILLNO,
	                               t.* into {0}
                            from {1} t left join t_AR_receivable e on t.FSRECEIVEBILLNO = e.FBILLNO
                            left join T_SAL_OUTSTOCK ot on ot.fbillNo = t.FOUTBILLNO
                            left join T_SAL_OUTSTOCKTRACE ott on ott.fid = ot.fid
                            where 1=1 ", tableName, tempTableName));
            if (!ht.Equals("0"))
            {
                sql.AppendFormat(" and en.e.F_SRT_HTH = '{0}'", ht);
            }
            if (!td.Equals("0"))
            {
                sql.AppendFormat(" and e.F_SRT_TD = '{0}'", td);
            }
            Utils.WriteLog(sql.ToString());
            DBUtils.Execute(this.Context, sql.ToString());
        }
    }
}
