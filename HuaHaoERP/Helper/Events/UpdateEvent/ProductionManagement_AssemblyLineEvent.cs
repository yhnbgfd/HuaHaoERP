﻿using System;

namespace HuaHaoERP.Helper.Events
{
    class ProductionManagement_AssemblyLineEvent
    {
        public static EventHandler EUpdateDataGrid;

        internal static void OnUpdateDataGrid()
        {
            if (EUpdateDataGrid != null)
            {
                EUpdateDataGrid(null, null);
            }
        }
    }
}
