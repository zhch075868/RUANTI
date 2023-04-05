﻿using Kingdee.BOS.App.Data;
using Kingdee.BOS.Contracts;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.K3.FIN.AP.ServiceHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingdee.BOS.Orm.DataEntity;

namespace DFYR.RTJQR.PlauginService.report
{
    public class supStatementEx : Kingdee.K3.FIN.AP.Business.PlugIn.Statement
    {
        string customerName = ""; //过滤条件往来单位
        string newtempTable = ""; //新的临时表
        bool NewSearch = false; //是否是新查询的数据

        public override void BarItemClick(BarItemClickEventArgs e)
        {
            if (e.BarItemKey == "tbFilter")
            {
                this.customerName = "";
                NewSearch = true;
            }
            if (e.BarItemKey == "tbRefresh")
            {
                NewSearch = true;
            }
            base.BarItemClick(e);
        }

        public override void DataChanged(DataChangedEventArgs e)
        {
            if (e.Field.Key == "FCombo")
            {
                NewSearch = false;
            }
            base.DataChanged(e);
        }

        protected override void Refresh()
        {
            if (this.filterPara == null)
            {
                return;
            }
            string oldSelectedValue = Convert.ToString(this.View.Model.GetValue("FCombo"));
            if (Convert.ToBoolean(this.filterPara.CustomFilter["FByContact"]))
            {
                customerName = this.GetSupplierData(oldSelectedValue);
            }
            dropTempTable();
            base.Refresh();
        }

        public override void BeforeClosed(BeforeClosedEventArgs e)
        {
            dropTempTable();
            base.BeforeClosed(e);
        }

        ///
        /// 将标准的对账单生成的临时表数据复制到带有扩展字段的临时表里面去
        ///

        public void CopyDataToNewTempTable()
        {
            this.newtempTable = GetTempTable();
            string sql = string.Format("select t1.*,t2.F_PYEO_BASE as F_SRT_HT into {1} from {0} t1 left join T_AP_PAYABLE t2 on t1.FID=t2.FID and t1.FFORMID='AR_receivable'", this.tempTable, this.newtempTable);
            DBUtils.Execute(this.Context, sql);
        }

        ///

        /// 获取新的带有扩展字段的数据
        ///

        ///
        public DynamicObjectCollection GetNewData()
        {
            CopyDataToNewTempTable();
            DynamicObjectCollection dataByContactUnit = StatementServiceHelper.GetDataByContactUnit(base.Context, this.newtempTable, customerName);
            return dataByContactUnit;
        }

        ///

        /// 获取新的带扩展字段的汇总数据
        ///

        ///
        public DynamicObjectCollection GetNewSumDate()
        {
            DynamicObjectCollection sumDataByContactUnit = StatementServiceHelper.GetSumDataByContactUnit(base.Context, this.newtempTable, this.agingKeys.Keys.ToList(), customerName);
            return sumDataByContactUnit;
        }

        public override void SetListData(DynamicObjectCollection data)
        {
            if (NewSearch)
            {
                DynamicObjectCollection dt = GetNewData();
                string oldTempTable = this.tempTable;
                this.tempTable = this.newtempTable;
                this.newtempTable = oldTempTable;
                base.SetListData(dt);
            }
            else
            {
                base.SetListData(data);
            }
        }

        public override void SetListSumData(DynamicObjectCollection data)
        {
            if (NewSearch)
            {
                DynamicObjectCollection dt = GetNewSumDate();
                base.SetListSumData(dt);
            }
            else
            {
                base.SetListSumData(data);
            }
        }

        ///

        /// 删除临时表
        ///

        private void dropTempTable()
        {
            if (!string.IsNullOrWhiteSpace(this.newtempTable))
            {
                IDBService dbservice = Kingdee.BOS.App.ServiceHelper.GetService<IDBService>();
                dbservice.DeleteTemporaryTableName(this.Context, new string[] { this.newtempTable });
                this.newtempTable = "";
            }
        }

        ///

        /// 获取临时表名称
        ///

        ///
        private string GetTempTable()
        {
            IDBService dbservice = Kingdee.BOS.App.ServiceHelper.GetService<IDBService>();
            string[] temptables = dbservice.CreateTemporaryTableName(this.Context, 1);
            return temptables[0];
        }
    }
}
