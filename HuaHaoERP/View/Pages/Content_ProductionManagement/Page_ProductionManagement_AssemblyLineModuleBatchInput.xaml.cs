﻿using HuaHaoERP.Model.ProductionManagement;
using HuaHaoERP.ViewModel.ProductionManagement;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HuaHaoERP.View.Pages.Content_ProductionManagement
{
    public partial class Page_ProductionManagement_AssemblyLineModuleBatchInput : Page
    {
        ObservableCollection<Model_AssemblyLineModuleBatchInput> data = new ObservableCollection<Model_AssemblyLineModuleBatchInput>();

        public Page_ProductionManagement_AssemblyLineModuleBatchInput()
        {
            InitializeComponent();
            InitializeDataGrid();
        }

        private void InitializeDataGrid()
        {
            for (int i = 0; i < 20; i++)
            {
                data.Add(new Model_AssemblyLineModuleBatchInput { Id = i + 1 });
            }
            this.DataGrid_BatchInput.ItemsSource = data;
        }

        private void Button_Commit_Click(object sender, RoutedEventArgs e)
        {
            if (new AssemblyLineModuleBatchInputConsole().InsertData(data))
            {
                Button_Cancel_Click(null, null);
                Helper.Events.UpdateEvent.AssemblyLineModuleEvent.OnUpdateDataGrid();
            }
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Helper.Events.PopUpEvent.OnHidePopUp();
        }

        private void DataGrid_BatchInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string Header = DataGrid_BatchInput.SelectedCells[0].Column.Header.ToString();
                if (Header == "产品编号")
                {
                    e.Handled = true;
                    DataGrid_BatchInput.CurrentCell = new DataGridCellInfo(DataGrid_BatchInput.SelectedCells[0].Item, DataGrid_BatchInput.Columns[2]);
                }
                else if (Header == "工序")
                {
                    e.Handled = true;
                    DataGrid_BatchInput.CurrentCell = new DataGridCellInfo(DataGrid_BatchInput.SelectedCells[0].Item, DataGrid_BatchInput.Columns[3]);
                }
                else if (Header == "员工编号")
                {
                    e.Handled = true;
                    DataGrid_BatchInput.CurrentCell = new DataGridCellInfo(DataGrid_BatchInput.SelectedCells[0].Item, DataGrid_BatchInput.Columns[5]);
                }
                else if (Header == "数量")
                {
                    e.Handled = true;
                    DataGrid_BatchInput.CurrentCell = new DataGridCellInfo(DataGrid_BatchInput.SelectedCells[0].Item, DataGrid_BatchInput.Columns[6]);
                }
                else if (Header == "损坏")
                {
                    DataGrid_BatchInput.CurrentCell = new DataGridCellInfo(DataGrid_BatchInput.SelectedCells[0].Item, DataGrid_BatchInput.Columns[0]);
                }
                else
                {
                    e.Handled = true;
                    DataGrid_BatchInput.CurrentCell = new DataGridCellInfo(DataGrid_BatchInput.SelectedCells[0].Item, DataGrid_BatchInput.Columns[0]);
                }
                DataGrid_BatchInput.SelectedCells.Clear();
                DataGrid_BatchInput.SelectedCells.Add(DataGrid_BatchInput.CurrentCell);
            }
        }

        private void DataGrid_BatchInput_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Model_AssemblyLineModuleBatchInput model = this.DataGrid_BatchInput.SelectedCells[0].Item as Model_AssemblyLineModuleBatchInput;
            string newValue = (e.EditingElement as TextBox).Text.Trim();
            string Header = e.Column.Header.ToString();
            if (Header == "产品编号")
            {
                Model_AssemblyLineModuleBatchInput m = new AssemblyLineModuleBatchInputConsole().ReadProductInfo(newValue);
                if (m.ProductGuid == new Guid())
                {
                    DataGrid_BatchInput.CurrentCell = new DataGridCellInfo(DataGrid_BatchInput.SelectedCells[0].Item, DataGrid_BatchInput.Columns[0]);
                    return;
                }
                data[data.IndexOf(model)].ProductGuid = m.ProductGuid;
                data[data.IndexOf(model)].ProductName = m.ProductName;
                data[data.IndexOf(model)].ProcessList = m.ProcessList;
                data[data.IndexOf(model)].ProcessListStr = m.ProcessListStr;
            }
            else if (Header == "工序")
            {
                int ProcessNum = 0;
                int.TryParse(newValue, out ProcessNum);
                if (ProcessNum <= 6 && ProcessNum > 0 && data[data.IndexOf(model)].ProcessList[ProcessNum - 1] != null)
                {
                    data[data.IndexOf(model)].Process = data[data.IndexOf(model)].ProcessList[ProcessNum - 1];
                }
                else
                {
                    DataGrid_BatchInput.CurrentCell = new DataGridCellInfo(DataGrid_BatchInput.SelectedCells[0].Item, DataGrid_BatchInput.Columns[2]);
                }
            }
            else if (Header == "员工编号")
            {
                Model_AssemblyLineModuleBatchInput m = new AssemblyLineModuleBatchInputConsole().ReadStaffInfo(newValue);
                if (m.StaffGuid == new Guid())
                {
                    DataGrid_BatchInput.CurrentCell = new DataGridCellInfo(DataGrid_BatchInput.SelectedCells[0].Item, DataGrid_BatchInput.Columns[3]);
                    return;
                }
                data[data.IndexOf(model)].StaffGuid = m.StaffGuid;
                data[data.IndexOf(model)].StaffName = m.StaffName;
            }
        }
    }
}
