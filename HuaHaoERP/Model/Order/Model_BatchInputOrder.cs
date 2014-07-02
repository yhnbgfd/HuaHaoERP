﻿using System;
using System.ComponentModel;

namespace HuaHaoERP.Model.Order
{
    class Model_BatchInputOrder : INotifyPropertyChanged
    {
        private Guid guid;

        public Guid Guid
        {
            get { return guid; }
            set { guid = value; }
        }
        private string number;

        public string Number
        {
            get { return number; }
            set { number = value; NotifyPropertyChanged("Number"); }
        }
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; NotifyPropertyChanged("Name"); }
        }
        private string date;

        public string Date
        {
            get { return date; }
            set { date = value; NotifyPropertyChanged("Date"); }
        }
        private string remark;

        public string Remark
        {
            get { return remark; }
            set { remark = value; NotifyPropertyChanged("Remark"); }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}