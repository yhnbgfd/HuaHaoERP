﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using HuaHaoERP.Model;

namespace HuaHaoERP.ViewModel.Warehouse
{
    class RawMaterialsConsole
    {
        internal bool AddByBatch(List<Model.RawMaterialsDetailModel> list)
        {
            List<string> sqlList = new List<string>();
            foreach (RawMaterialsDetailModel d in list)
            {
                string sql = "Insert into T_Warehouse_RawMaterials(Guid,RawMaterialsID,Date,Operator,Number,Remark) "
                        + "values('" + d.Guid + "','" + d.Number + "','" + d.Date + "','" + d.Operator + "','" + d.Weight + "','" + d.Remark + "')";
                sqlList.Add(sql);
               
            }
            return new Helper.SQLite.DBHelper().Transaction(sqlList);
        }

        internal bool ReadList(out List<RawMaterialsDetailModel> data)
        {
            bool flag = true;
            data = new List<RawMaterialsDetailModel>();
            string sql = "select a.RawMaterialsID as RawMaterialsID,b.Name as Name,count(1) as Amount from T_Warehouse_RawMaterials a left join T_ProductInfo_RawMaterials b on a.RawMaterialsID = b.Number group by RawMaterialsID";
            DataSet ds = new DataSet();
            flag = new Helper.SQLite.DBHelper().QueryData(sql, out ds);
            if (flag)
            {
                int id = 1;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    RawMaterialsDetailModel d = new RawMaterialsDetailModel();
                    d.Id = id;
                    id++;
                    d.RawMaterialsID = dr["RawMaterialsID"].ToString();
                    d.Name = dr["Name"].ToString();
                    d.Amount = dr["Amount"].ToString();
                    data.Add(d);
                }
            }
            return flag;
        }

        internal bool ReadRecordList(out List<RawMaterialsDetailModel> data)
        {
            bool flag = true;
            data = new List<RawMaterialsDetailModel>();
            string sql = "select a.Guid as Guid,a.RawMaterialsID as RawMaterialsID,b.Name as Name,strftime(a.Date) as Date,a.Operator as Operator,a.Number as Number,a.Remark as Remark from T_Warehouse_RawMaterials a left join T_ProductInfo_RawMaterials b on a.RawMaterialsID = b.Number order by a.Date desc";
            DataSet ds = new DataSet();
            flag = new Helper.SQLite.DBHelper().QueryData(sql, out ds);
            if (flag)
            {
                int id = 1;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    RawMaterialsDetailModel d = new RawMaterialsDetailModel();
                    d.Id = id;
                    id++;
                    d.Guid = Guid.Parse(dr["Guid"].ToString());
                    d.RawMaterialsID = dr["RawMaterialsID"].ToString();
                    d.Date = dr["Date"].ToString();
                    d.Number = dr["Number"].ToString();
                    d.Name = dr["Name"].ToString();
                    d.Remark = dr["Remark"].ToString();
                    d.Operator = dr["Operator"].ToString();
                    data.Add(d);
                }
            }
            return flag;
        }
    }
}