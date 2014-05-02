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
using HuaHaoERP.Helper.Events;

namespace HuaHaoERP.View.Pages.Content2
{
    public partial class Page_MainContent2_Popup_RawMaterials : Page
    {
        public Page_MainContent2_Popup_RawMaterials()
        {
            InitializeComponent();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            PopUpEvent.OnHidePopUp(this, new PopUpEventArgs());
        }

        private void Button_Commit_Click(object sender, RoutedEventArgs e)
        {

            Button_Cancel_Click(null, null);
        }
    }
}
