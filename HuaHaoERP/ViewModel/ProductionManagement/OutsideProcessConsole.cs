﻿using HuaHaoERP.Model;
using HuaHaoERP.Model.ProductionManagement;
using System;
using System.Collections.Generic;
using System.Data;

namespace HuaHaoERP.ViewModel.ProductionManagement
{
    class OutsideProcessConsole
    {
        internal bool Add(ProductionManagement_OutsideProcessModel d)
        {
            string sql = "Insert into T_PM_ProcessSchedule(Guid,Date,ProductID,ProcessorsID,Quantity,MinorInjuries,Injuries,Lose,OrderType,Remark) "
                       + " values('" + d.Guid + "','" + d.OrderDate + "','" + d.ProductGuid + "','" + d.ProcessorsGuid + "','" + d.Quantity + "','" + d.MinorInjuries + "','" + d.Injuries + "','" + d.Lose + "','" + d.OrderType + "','" + d.Remark + "')";
            return new Helper.SQLite.DBHelper().SingleExecution(sql);
        }

        internal bool Delete(Guid guid)
        {
            bool flag = false;
            string sql = "Delete From T_PM_ProcessSchedule Where GUID='" + guid + "'";
            flag = new Helper.SQLite.DBHelper().SingleExecution(sql);
            return flag;
        }

        internal bool ReadList(string OrderType, DateTime Start, DateTime End, Guid ProductID, Guid ProcessorsID, out List<ProductionManagement_OutsideProcessModel> data, out string strCount)
        {
            strCount = "";
            string sql_WhereParm = "";
            if (ProductID != new Guid())
            {
                sql_WhereParm += " AND a.ProductID='" + ProductID + "' ";
            }
            if (ProcessorsID != new Guid())
            {
                sql_WhereParm += " AND a.ProcessorsID='" + ProcessorsID + "' ";
            }
            sql_WhereParm += " AND a.Date between '" + Start.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + End.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            bool flag = false;
            data = new List<ProductionManagement_OutsideProcessModel>();
            int Count = 0;
            int CountMinorInjuries = 0, CountInjuries = 0, CountLose = 0;
            string sql = " SELECT                                                                    "
                       + "	a.*,                                                                     "
                       + "   b.Name as ProductName,                                                  "
                       + "   c.Name as ProcessorsName                                                "
                       + " FROM                                                                      "
                       + "	T_PM_ProcessSchedule a                                                   "
                       + " LEFT JOIN T_ProductInfo_Product b ON a.ProductID=b.GUID                   "
                       + " LEFT JOIN T_UserInfo_Processors c ON a.ProcessorsID=c.GUID                "
                       + " WHERE                                                                     "
                       + "	OrderType = '" + OrderType + "'                                          "
                       + "  AND a.DeleteMark ISNULL"
                       + sql_WhereParm
                       + " Order By a.Date";
            DataSet ds = new DataSet();
            flag = new Helper.SQLite.DBHelper().QueryData(sql, out ds);
            if (flag)
            {
                int id = 1;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    ProductionManagement_OutsideProcessModel d = new ProductionManagement_OutsideProcessModel();
                    d.Guid = (Guid)dr["GUID"];
                    d.Id = id++;
                    d.OrderDate = Convert.ToDateTime(dr["Date"].ToString()).ToString("yyyy-MM-dd");
                    d.ProductGuid = (Guid)dr["ProductID"];
                    d.ProductName = dr["ProductName"].ToString();
                    d.ProcessorsGuid = (Guid)dr["ProcessorsID"];
                    d.ProcessorsName = dr["ProcessorsName"].ToString();
                    d.Quantity = int.Parse(dr["Quantity"].ToString());
                    Count += d.Quantity;
                    d.MinorInjuries = int.Parse(dr["MinorInjuries"].ToString());
                    CountMinorInjuries += d.MinorInjuries;
                    d.Injuries = int.Parse(dr["Injuries"].ToString());
                    CountInjuries += d.Injuries;
                    d.Lose = int.Parse(dr["Lose"].ToString());
                    CountLose += d.Lose;
                    d.OrderType = OrderType;
                    d.Remark = dr["Remark"].ToString();
                    data.Add(d);
                }
            }
            strCount = (Count + CountMinorInjuries + CountInjuries + CountLose).ToString();
            if (OrderType == "入单")
            {
                strCount += "（" + Count.ToString() + "," + CountMinorInjuries.ToString() + "," + CountInjuries.ToString() + "," + CountLose.ToString() + "）";
            }
            return flag;
        }
    }
}
