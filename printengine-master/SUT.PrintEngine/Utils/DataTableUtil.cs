﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Data;
using System.Linq;

namespace SUT.PrintEngine.Utils
{
    internal class DataTableUtil
    {
        public static void Validate(DataTable dataTable)
        {
            var result = true;
            foreach (DataColumn column in dataTable.Columns)
            {
                if(column.ExtendedProperties.ContainsKey("Width"))
                {
                    throw new FormatException(string.Format("Column Width not Defined for column : '{0}'", column.ColumnName));
                }
            }
        }
    }
}
