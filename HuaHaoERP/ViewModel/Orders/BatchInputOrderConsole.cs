﻿using HuaHaoERP.Helper.SQLite;
using HuaHaoERP.Model.Order;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace HuaHaoERP.ViewModel.Orders
{
    class BatchInputOrderConsole
    {
        internal void ReadOrder(int BacthType, out ObservableCollection<Model_BatchInputOrder> data, int OrderType)
        {
            data = new ObservableCollection<Model_BatchInputOrder>();
            Model_BatchInputOrder m;
            DataSet ds = new DataSet();
            string TableName = GetTableName(BacthType);
            string sql = "select * from " + TableName + " WHERE DeleteMark ISNULL Order By rowid Desc";
            if (BacthType == 3 && OrderType > 0)
            {
                sql = "select * from " + TableName + " WHERE DeleteMark ISNULL AND OrderType='" + (OrderType - 1) + "' Order By rowid Desc";
            }
            else if (BacthType == 2)//外加工需读取加工商名字
            {
                string OrderTypeWhereParm = "";
                if (OrderType > 0)
                {
                    OrderTypeWhereParm = " AND a.OrderType='" + (OrderType - 1) + "'";
                }
                sql = "select a.*,b.Name as PName "
                    + " from " + TableName + " a "
                    + " Left join T_UserInfo_Processors b ON a.ProcessorsID=b.Guid"
                    + " WHERE a.DeleteMark ISNULL " + OrderTypeWhereParm + " Order By a.rowid Desc";
            }
            if (new DBHelper().QueryData(sql, out ds))
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    m = new Model_BatchInputOrder();
                    m.Guid = (Guid)dr["Guid"];
                    m.Number = dr["Number"].ToString();
                    m.Name = dr["Name"].ToString();
                    m.Date = Convert.ToDateTime(dr["Date"].ToString()).ToString("yyyy-MM-dd");
                    m.Remark = dr["Remark"].ToString();
                    if (BacthType != 1)
                    {
                        m.OrderType = dr["OrderType"].ToString();
                    }
                    if (BacthType == 2)
                    {
                        m.ProcessorsID = (Guid)dr["ProcessorsID"];
                        m.ProcessorsName = dr["PName"].ToString();
                    }
                    data.Add(m);
                }
            }
        }

        internal bool DeleteOrder(int Type, Guid OrderGuid)
        {
            string TableName = GetTableName(Type);
            string DetailTableName = GetDetailTableName(Type);
            List<string> sqls = new List<string>();
            sqls.Add("Update " + TableName + " SET DeleteMark='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE Guid='" + OrderGuid + "'");
            if (Type == 1 || Type == 2)
            {
                sqls.Add("Update " + DetailTableName + " Set DeleteMark='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE Obligate1='" + OrderGuid + "'");
            }
            else if (Type == 3)
            {
                sqls.Add("Update T_Warehouse_ProductPacking Set DeleteMark='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE Obligate1='" + OrderGuid + "'");
                sqls.Add("Update T_Warehouse_Product Set DeleteMark='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE Obligate1='" + OrderGuid + "'");
            }
            return new DBHelper().Transaction(sqls);
        }

        private string GetTableName(int Type)
        {
            switch (Type)
            {
                case 1://流水线批量录入
                    return "T_PM_ProductionBatchInput";
                case 2://外加工批量录入
                    return "T_PM_ProcessBatchInput";
                case 3://仓库产品批量录入
                    return "T_Warehouse_ProductBatchInput";
            }
            return "";
        }

        private string GetDetailTableName(int Type)
        {
            switch (Type)
            {
                case 1://流水线批量录入
                    return "T_PM_ProductionSchedule";
                case 2://外加工批量录入
                    return "T_PM_ProcessSchedule";
            }
            return "";
        }
    }
}
