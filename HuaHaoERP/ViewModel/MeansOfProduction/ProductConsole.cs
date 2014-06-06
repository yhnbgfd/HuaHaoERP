﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HuaHaoERP.Model;
using System.Data;

namespace HuaHaoERP.ViewModel.MeansOfProduction
{
    class ProductConsole
    {
        internal bool Add(ProductModel d)
        {
            object oTemp;
            string sql_Repeat = "select 1 from T_ProductInfo_Product where (Number='" + d.Number + "' OR Name='" + d.Name + "') AND DeleteMark IS NULL";
            if (new Helper.SQLite.DBHelper().QuerySingleResult(sql_Repeat, out oTemp))
            {
                return false;
            }
            bool flag = true;
            string sql = "Insert Into T_ProductInfo_Product (GUID,Number,Name,Material,Type,Specification,P1,P2,P3,P4,P5,P6,PackageNumber,Remark,AddTime) "
                       + " values('" + d.Guid + "','" + d.Number + "','" + d.Name + "','" + d.Material + "','" + d.Type + "','" + d.Specification + "','" + d.P1 + "','" + d.P2 + "','" + d.P3 + "','" + d.P4 + "','" + d.P5 + "','" + d.P6 + "','" + d.PackageNumber + "','" + d.Remark + "','" + d.AddTime.ToString("yyyy-MM-dd HH:mm:ss") + "')";
            flag = new Helper.SQLite.DBHelper().SingleExecution(sql);
            return flag;
        }
        internal bool Update(ProductModel d)
        {
            bool flag = true;
            List<string> sqls = new List<string>();
            string sql_Delete = "Delete From T_ProductInfo_Product Where GUID='" + d.Guid + "'";
            string sql_Update = "Insert Into T_ProductInfo_Product (GUID,Number,Name,Material,Type,Specification,P1,P2,P3,P4,P5,P6,PackageNumber,Remark,AddTime) "
                                + " values('" + d.Guid + "','" + d.Number + "','" + d.Name + "','" + d.Material + "','" + d.Type + "','" + d.Specification + "','" + d.P1 + "','" + d.P2 + "','" + d.P3 + "','" + d.P4 + "','" + d.P5 + "','" + d.P6 + "','" + d.PackageNumber + "','" + d.Remark + "','" + d.AddTime.ToString("yyyy-MM-dd HH:mm:ss") + "')";
            sqls.Add(sql_Delete);
            sqls.Add(sql_Update);
            flag = new Helper.SQLite.DBHelper().Transaction(sqls);
            return flag;
        }
        internal bool MarkDelete(ProductModel d)
        {
            bool flag = true;
            string sql = "Update T_ProductInfo_Product Set DeleteMark='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' Where GUID='" + d.Guid + "'";
            flag = new Helper.SQLite.DBHelper().SingleExecution(sql);
            return flag;
        }
        internal bool ReadList(string ProductType, out List<ProductModel> data)
        {
            string Sql_Where = "";
            if(!ProductType.StartsWith("全部"))
            {
                Sql_Where += " AND Type='" + ProductType + "' ";
            }
            bool flag = true;
            data = new List<ProductModel>();
            string sql = " select * "
                       + " from T_ProductInfo_Product "
                       + " Where DeleteMark is null "
                       + Sql_Where
                       + " order by AddTime";
            DataSet ds = new DataSet();
            flag = new Helper.SQLite.DBHelper().QueryData(sql, out ds);
            if (flag)
            {
                int id = 1;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    ProductModel d = new ProductModel();
                    d.Guid = (Guid)dr["GUID"];
                    d.Id = id++;
                    d.Number = dr["Number"].ToString();
                    d.Name = dr["Name"].ToString();
                    d.Material = dr["Material"].ToString();
                    d.Type = dr["Type"].ToString();
                    d.Specification = dr["Specification"].ToString();
                    d.P1 = dr["P1"].ToString();
                    d.P2 = dr["P2"].ToString();
                    d.P3 = dr["P3"].ToString();
                    d.P4 = dr["P4"].ToString();
                    d.P5 = dr["P5"].ToString();
                    d.P6 = dr["P6"].ToString();
                    GenerateProcess(ref d);
                    d.PackageNumber = int.Parse(dr["PackageNumber"].ToString());
                    d.Remark = dr["Remark"].ToString();
                    d.AddTime = Convert.ToDateTime(dr["AddTime"]);
                    data.Add(d);
                }
            }
            return flag;
        }
        private void GenerateProcess(ref ProductModel d)
        {
            if(d.P1 != "无")
            {
                d.Process = d.P1;
            }
            if (d.P2 != "无")
            {
                d.Process += " - " + d.P2;
            }
            if (d.P3 != "无")
            {
                d.Process += " - " + d.P3;
            }
            if (d.P4 != "无")
            {
                d.Process += " - " + d.P4;
            }
            if (d.P5 != "无")
            {
                d.Process += " - " + d.P5;
            }
            if (d.P6 != "无")
            {
                d.Process += " - " + d.P6;
            }
        }

        internal bool GetNameList(out DataSet ds)
        {
            ds = new DataSet();
            string sql = "select Guid,Number,Name From T_ProductInfo_Product Where DeleteMark is null order by AddTime";
            return new Helper.SQLite.DBHelper().QueryData(sql, out ds);
        }
        internal bool GetNameList(string Parm, out DataSet ds)
        {
            ds = new DataSet();
            string sql = "select Guid,Number,Name From T_ProductInfo_Product "
                       + " Where (Number LIKE '%" + Parm + "%' OR Name LIKE '%" + Parm + "%') "
                       + " AND DeleteMark is null order by AddTime";
            return new Helper.SQLite.DBHelper().QueryData(sql, out ds);
        }

        internal bool GetTypeList(out DataSet ds)
        {
            ds = new DataSet();
            string sql = "select Type From T_ProductInfo_Product Group by Type";
            return new Helper.SQLite.DBHelper().QueryData(sql, out ds);
        }
    }
}
