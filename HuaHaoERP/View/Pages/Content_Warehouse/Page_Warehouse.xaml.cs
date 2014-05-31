﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HuaHaoERP.ViewModel.Warehouse;
using HuaHaoERP.Model;
using HuaHaoERP.Model.Warehouse;
using HuaHaoERP.Helper.Events;
using HuaHaoERP.Helper.Events.UpdateEvent;

namespace HuaHaoERP.View.Pages.Content_Warehouse
{
    public partial class Page_Warehouse : Page
    {
        private WarehouseProductModel ProductModel = new WarehouseProductModel();
        private ScrapConsole sc = new ScrapConsole();
        private RawMaterialsConsole rmc = new RawMaterialsConsole();
        private ScrapModel m = new ScrapModel();

        public Page_Warehouse()
        {
            InitializeComponent();
            this.Grid_Packing.Visibility = System.Windows.Visibility.Hidden;
            SubscribeToEvent();
            InitializeRawMaterialsDataGrid();
            InitPage();
        }
        private void SubscribeToEvent()
        {
            WarehouseRawMaterialsEvent.EUpdateDataGrid += (sender, e) => { InitializeRawMaterialsDataGrid(); };
        }

        private void InitPage()
        {
            InitializeProductDataGrid();
            #region 余料管理
            ComboBox_DropDownOpened(this, null);
            RefreshData_Scrap();
            ComboBox_Name.ItemsSource = sc.GetName(false);
            ComboBox_Name.SelectedIndex = 0;
            DatePicker_Date.Text = DateTime.Now.ToShortDateString();
            #endregion 
        }

        #region 产品仓库

        private void Button_Packing_Click(object sender, RoutedEventArgs e)
        {
            this.Grid_Packing.Visibility = System.Windows.Visibility.Visible;
        }
        private void Button_ClosePacking_Click(object sender, RoutedEventArgs e)
        {
            this.Grid_Packing.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void InitializeProductDataGrid()
        {
            List<WarehouseProductModel> d = new List<WarehouseProductModel>();
            if(new ViewModel.Warehouse.WarehouseProductConsole().ReadDetailsList(out d))
            {
                this.DataGrid_ProductDetails.ItemsSource = d;
            }
        }

        #endregion

        #region 原材料管理
        private void InitializeRawMaterialsDataGrid()
        {
            List<RawMaterialsDetailModel> rmm = new List<RawMaterialsDetailModel>();
            rmc.ReadList(out rmm);
            DataGrid_RawMaterialsQuantity.ItemsSource = rmm;
            rmc.ReadRecordList(out rmm);
            DataGrid_RawMaterialsRecord.ItemsSource = rmm;
        }
        private void Button_RawMaterials_Out_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid_RawMaterialsRecord.SelectedCells.Count != 0)
            {
                List<RawMaterialsDetailModel> list = new List<RawMaterialsDetailModel>();
                for (int i = 0; i < DataGrid_RawMaterialsRecord.SelectedItems.Count; i++) 
                {
                    RawMaterialsDetailModel m = DataGrid_RawMaterialsRecord.SelectedItems[i] as RawMaterialsDetailModel;
                    list.Add(m);
                }
                bool flag  = rmc.AddByBatch(list,false);
                if (flag)
                {
                    InitializeRawMaterialsDataGrid();
                }
            }
        }

        private void Button_RawMaterials_In_Click(object sender, RoutedEventArgs e)
        {
            Helper.Events.PopUpEvent.OnShowPopUp(new Page_Warehouse_RawMaterials());
        }
        #endregion

        #region 余料管理

        private void RefreshData_Scrap()
        {
            List<ScrapModel> data = new List<ScrapModel>();
            sc.ReadList(ComboBox_Scrap_Name.Text, out data);
            DataGrid_Scrap.ItemsSource = data;
            Label_Scrap_Amount.Content = "数量：" + sc.GetAmountByName(ComboBox_Scrap_Name.Text);
        }
        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            ComboBox_Scrap_Name.ItemsSource = sc.GetName(true);
            ComboBox_Scrap_Name.SelectedIndex = 0;
        }

        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            RefreshData_Scrap();
        }

        private void ComboBox_Name_DropDownOpened(object sender, EventArgs e)
        {
            ComboBox_Name.ItemsSource = sc.GetName(false);
            ComboBox_Name.SelectedIndex = 0;
        }
        private bool CheckAndGetData()
        {
            DateTime dt = new DateTime();
            DateTime.TryParse(DatePicker_Date.Text, out dt);
            TimeSpan ts = DateTime.Now - dt;
            int day = ts.Days;
            if (day > 1000) 
            {
                StatusBarMessageEvent.OnUpdateMessage("日期不能为空！");
                DatePicker_Date.Text = DateTime.Now.ToShortDateString();
                return false;
            }
            if (string.IsNullOrEmpty(TextBox_Operator.Text))
            {
                StatusBarMessageEvent.OnUpdateMessage("操作人不能空！");
                return false;
            }
            if (ComboBox_Name.SelectedIndex == 0)
            {
                StatusBarMessageEvent.OnUpdateMessage("请选择余料类型！");
                ComboBox_Name.Focus();
                return false;
            }
            if (RadioButton_添加.IsChecked == false)
            {
                decimal dTmp = 0;
                decimal.TryParse(TextBox_Number.Text, out dTmp);
                if (dTmp > sc.GetAmountByName(ComboBox_Name.Text))
                {
                    StatusBarMessageEvent.OnUpdateMessage("卖出数量/重量大于库存值！");
                    return false;
                }
            }
            m.Guid = Guid.NewGuid();
            m.Name = ComboBox_Name.Text;
            m.Number = RadioButton_添加.IsChecked == true ? TextBox_Number.Text : "-" + TextBox_Number.Text;
            m.Remark = TextBox_Remark.Text;
            m.Date = DateTime.Parse(DatePicker_Date.Text).ToString("yyyy-MM-dd") + " " + DateTime.Now.ToString("T");
            m.Operator = TextBox_Operator.Text;
            return true;
        }
        private void Button_Commit_Click(object sender, RoutedEventArgs e)
        {
            string typeMsg = RadioButton_添加.IsChecked == true ? "添加" : "卖出";
            if (CheckAndGetData())
            {
                sc.Add(m);
                RefreshData_Scrap();
                StatusBarMessageEvent.OnUpdateMessage(typeMsg + "余料名称：" + m.Name);
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back || e.Key == Key.Tab)
            {
                if (txt.Text.Contains(".") && e.Key == Key.Decimal)
                {
                    e.Handled = true;
                    return;
                }
                e.Handled = false;
            }
            else if (((e.Key >= Key.D0 && e.Key <= Key.D9) || e.Key == Key.OemPeriod) && e.KeyboardDevice.Modifiers != ModifierKeys.Shift)
            {
                if (txt.Text.Contains(".") && e.Key == Key.OemPeriod)
                {
                    e.Handled = true;
                    return;
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        #endregion

    }
}
