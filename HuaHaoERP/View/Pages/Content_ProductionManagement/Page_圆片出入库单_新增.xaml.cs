﻿using HuaHaoERP.Helper.Events.UpdateEvent.Warehouse;
using HuaHaoERP.Model.MeansOfProduction;
using HuaHaoERP.Model.Order;
using HuaHaoERP.Model.ProductionManagement;
using HuaHaoERP.Model.Warehouse;
using HuaHaoERP.ViewModel.ProductionManagement;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HuaHaoERP.View.Pages.Content_ProductionManagement
{
    public partial class Page_圆片出入库单_新增 : Page
    {
        private Model_圆片订单 _data = new Model_圆片订单();
        private int _inOut = 0;//1 in 0 out
        private int _orderNum;

        public Page_圆片出入库单_新增(int inOut)
        {
            InitializeComponent();
            _inOut = inOut;
            InitData();
        }

        private void InitData()
        {
            if (_inOut == 1)
            {
                this.DataGridTextColumn_入库半成品编号.Visibility = System.Windows.Visibility.Collapsed;
                this.DataGridTextColumn_半成品品名.Visibility = System.Windows.Visibility.Collapsed;
                this.DataGridTextColumn_半成品数量.Visibility = System.Windows.Visibility.Collapsed;
                this.Label_Title.Content = "圆片入库单";
                int.TryParse(new Helper.SettingFile.INIHelper().IniReadValue("Other", "圆片入库单"), out _orderNum);
                this.TextBox_Number.Text = "SYP";
            }
            else
            {
                this.DataGridTextColumn_损耗数量.Visibility = System.Windows.Visibility.Collapsed;
                this.Label_Title.Content = "圆片出库单";
                int.TryParse(new Helper.SettingFile.INIHelper().IniReadValue("Other", "圆片出库单"), out _orderNum);
                this.TextBox_Number.Text = "CYP";
            }
            _orderNum++;
            for (int i = _orderNum.ToString().Length; i < 6; i++)
            {
                this.TextBox_Number.Text += "0";
            }
            this.TextBox_Number.Text += _orderNum;

            this.DatePicker_InsertDate.SelectedDate = DateTime.Now;
            for (int i = 0; i < 20; i++)
            {
                _data.明细.Add(new Model_圆片仓库());
            }
            this.DataGrid_List.ItemsSource = _data.明细;
        }

        private bool Commit()
        {
            _data.Guid = Guid.NewGuid();
            _data.OrderType = _inOut;
            _data.单号 = this.TextBox_Number.Text;
            _data.备注 = this.TextBox_Remark.Text;
            _data.下单日期 = (((DateTime)this.DatePicker_InsertDate.SelectedDate).Date + DateTime.Now.TimeOfDay).ToString("yyyy-MM-dd HH:mm:ss");
            _data.明细 = this.DataGrid_List.ItemsSource as List<Model_圆片仓库>;
            bool flag = new ViewModel.Orders.Vm_Order_圆片().Add(_data);
            if (flag)
            {
                if (_inOut == 1)
                {
                    new Helper.SettingFile.INIHelper().IniWriteValue("Other", "圆片入库单", _orderNum.ToString());
                }
                else
                {
                    new Helper.SettingFile.INIHelper().IniWriteValue("Other", "圆片出库单", _orderNum.ToString());
                    HalfProductEvent.OnUpdateDataGrid();
                }
                Helper.Events.MeansOfProduction.Event_圆片.OnUpdate圆片订单();
                Helper.Events.MeansOfProduction.Event_圆片.OnUpdate圆片库存();
            }
            return flag;
        }

        private void Button_Cancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Helper.Events.PopUpEvent.OnHidePopUp();
        }

        private void Button_CommitNew_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Commit())
            {
                Helper.Events.PopUpEvent.OnShowPopUp(new Page_圆片出入库单_新增(_inOut));
            }
        }

        private void Button_Commit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Commit())
            {
                Helper.Events.PopUpEvent.OnHidePopUp();
            }
        }

        private void DataGrid_List_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            string newValue = (e.EditingElement as TextBox).Text.Trim();
            if (string.IsNullOrWhiteSpace(newValue))
            {
                return;
            }
            string header = e.Column.Header.ToString();
            Model_圆片仓库 m = this.DataGrid_List.SelectedCells[0].Item as Model_圆片仓库;
            if (header == this.DataGridTextColumn_编号.Header.ToString())
            {
                Model_圆片资料 w = new ViewModel.MeansOfProduction.Vm_圆片().ReadInfo(newValue);
                if (w != null)
                {
                    m.Guid = w.Guid;
                    m.编号 = w.编号;
                    m.直径 = w.直径;
                    m.厚度 = w.厚度;
                }
                else
                {
                    MessageBox.Show("找不到编号对应的品名");
                    m.编号 = "";
                    this.DataGrid_List.CurrentCell = new DataGridCellInfo(this.DataGrid_List.SelectedCells[0].Item, this.DataGrid_List.Columns[0]);
                }
            }
            else if (header == this.DataGridTextColumn_数量.Header.ToString())
            {
                Int64 quantity = 0;
                Int64.TryParse(newValue, out quantity);
                m.数量 = quantity;
            }
            else if (header == this.DataGridTextColumn_入库半成品编号.Header.ToString())
            {
                Model_ProductionManagement_OutsideProcessBatch pm = new OutsideProcessBatchInputConsole().ReadProductInfo(newValue);
                if (pm.ProductGuid != new Guid())
                {
                    m.半成品Guid = pm.ProductGuid;
                    m.半成品品名 = pm.ProductName;
                }
                else
                {
                    MessageBox.Show("找不到编号对应的品名");
                    m.入库半成品编号 = "";
                    this.DataGrid_List.CurrentCell = new DataGridCellInfo(this.DataGrid_List.SelectedCells[0].Item, this.DataGrid_List.Columns[5]);
                }
            }
            else if (header == this.DataGridTextColumn_半成品数量.Header.ToString())
            {
                Int64 quantity = 0;
                Int64.TryParse(newValue, out quantity);
                m.半成品数量 = quantity;
            }
        }

        private void DataGrid_List_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string header = this.DataGrid_List.SelectedCells[0].Column.Header.ToString();
                if (header == this.DataGridTextColumn_编号.Header.ToString())
                {
                    e.Handled = true;
                    this.DataGrid_List.CurrentCell = new DataGridCellInfo(this.DataGrid_List.SelectedCells[0].Item, this.DataGrid_List.Columns[3]);
                }
                else if (header == this.DataGridTextColumn_数量.Header.ToString())
                {
                    e.Handled = true;
                    if (_inOut == 1)
                    {
                        this.DataGrid_List.CurrentCell = new DataGridCellInfo(this.DataGrid_List.SelectedCells[0].Item, this.DataGrid_List.Columns[4]);
                    }
                    else
                    {
                        this.DataGrid_List.CurrentCell = new DataGridCellInfo(this.DataGrid_List.SelectedCells[0].Item, this.DataGrid_List.Columns[5]);
                    }
                }
                else if (header == this.DataGridTextColumn_损耗数量.Header.ToString())
                {
                    this.DataGrid_List.CurrentCell = new DataGridCellInfo(this.DataGrid_List.SelectedCells[0].Item, this.DataGrid_List.Columns[0]);
                }
                else if (header == this.DataGridTextColumn_入库半成品编号.Header.ToString())
                {
                    e.Handled = true;
                    this.DataGrid_List.CurrentCell = new DataGridCellInfo(this.DataGrid_List.SelectedCells[0].Item, this.DataGrid_List.Columns[7]);
                }
                else if (header == this.DataGridTextColumn_半成品数量.Header.ToString())
                {
                    this.DataGrid_List.CurrentCell = new DataGridCellInfo(this.DataGrid_List.SelectedCells[0].Item, this.DataGrid_List.Columns[0]);
                }
                else
                {
                    e.Handled = true;
                    this.DataGrid_List.CurrentCell = new DataGridCellInfo(this.DataGrid_List.SelectedCells[0].Item, this.DataGrid_List.Columns[0]);
                }
                this.DataGrid_List.SelectedCells.Clear();
                this.DataGrid_List.SelectedCells.Add(this.DataGrid_List.CurrentCell);
            }
        }
    }
}
