// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SCADA
{
    public class ValueItemModbusConverter : IValueConverter
    {
        public ItemModbus Item;

        public ValueItemModbusConverter(ItemModbus item)
        {
            Item = item;
        }
       
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                value = "###";
            }

            if (Item.Text.Length != 0)
            {
                if (Item.TypeValue == "float")
                {
                    if (!(value is string))
                    {
                        return string.Format("{0:f}", value) + " " + Item.Text;
                    }
                    else
                    {
                        return value;
                    }                   
                }
                else
                {                   
                    return value.ToString() + " " + Item.Text;
                }
            }
            else
            {
                if (Item.TypeValue == "float")
                {
                    if (!(value is string))
                    {
                        return string.Format("{0:f}", value);
                    }
                    else
                    {
                        return value;
                    } 
                }
                else
                {
                    return value;
                }
            }           
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class ValueItemNetConverter : IValueConverter
    {
        public ItemNet Item;

        public ValueItemNetConverter(ItemNet item)
        {
            Item = item;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                value = "###";
            }

            if (Item.Text.Length != 0)
            {
                if (Item.TypeValue == "float")
                {
                    if (!(value is string))
                    {
                        return string.Format("{0:f}", value) + " " + Item.Text;
                    }
                    else
                    {
                        return value;
                    }
                }
                else
                {
                    return value.ToString() + " " + Item.Text;
                }
            }
            else
            {
                if (Item.TypeValue == "float")
                {
                    if (!(value is string))
                    {
                        return string.Format("{0:f}", value);
                    }
                    else
                    {
                        return value;
                    }
                }
                else
                {
                    return value;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(int), typeof(bool))]
    public class DGItemsToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int count = (int)value;

            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }    
   
    public class EthernetOperationDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "Описание: " + value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
         
    [ValueConversion(typeof(bool), typeof(bool))]
    public class EnableButtonsStartValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool enable = (bool)value;

            return !enable;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
  
    public class StartDGEnableValueConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (((bool)value[0] || (bool)value[1]) && !(bool)value[2])
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class DGTBEmergencyEnabledValueConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (((bool)value[0] || (bool)value[1]) && (bool)value[2])
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class EnableOpenCreateFileValueConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            bool enable = (bool)value[0];

            if (!enable && value[1] != null)
            {
                return true;
            }
            else
            {
                return false;
            }           
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class EnableSaveValueConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            bool enable = (bool)value[0];

            if (!enable && value[1] != null && value[2] != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class AddDatabaseValueConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            bool enable = (bool)value[0];

            if (enable && (bool)value[1])
            {
                return true;
            }
            else
            {
                if ((bool)value[2])
                {
                    return true;
                }
                else
                {
                    return false;
                }              
            }
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class TBPeriodEmergencySaveDBValueConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value[0] && (bool)value[1])
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class CheckBoxEmergencySaveDBValueConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value[0] || (bool)value[1])
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class AddTableValueConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            bool enable = (bool)value[0];

            if (enable && (bool)value[1] && (bool)value[2])
            {
                return true;
            }
            else
            {
                if ((bool)value[3] && (bool)value[4])
                {
                    return true;
                }
                else
                {
                    return false;
                }               
            }
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
   
    public class ButtonStartValueConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            int count = (int)value[0];

            int count2 = (int)value[1];

            if ((count > 0 || count2 > 0) && !(bool)value[2])
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
   
    public class ButtonStartInterfaceValueConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            bool itemsCount = (bool)value[0];

            bool disable = (bool)value[1];

            if (disable)
            {
                return false;
            }
            else if (itemsCount)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class GetOptionalData : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value[0] != null && (bool)value[0]) && ((bool)value[1] || (bool)value[2] || (bool)value[3] || (bool)value[4] || (bool)value[5]))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(object), typeof(bool))]
    public class RemoveButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(object), typeof(bool))]
    public class EnablePolicyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null )
            {
                if (((Group)value).GroupName == "Администратор")
                {
                    return false;
                }
                else
                {
                    return true;
                }               
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(object), typeof(bool))]
    public class AddTableServerName : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (string.IsNullOrWhiteSpace((string)value))
                {
                    return false;
                }
                else
                {
                    return true;
                }               
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(object), typeof(bool))]
    public class AddTableDatabaseName : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (string.IsNullOrWhiteSpace((string)value) || (string.Compare((string)value, "master", true) == 0) || (string.Compare((string)value, "model", true) == 0)
                   || (string.Compare((string)value, "msdb", true) == 0) || (string.Compare((string)value, "ReportServer", true) == 0)
                   || (string.Compare((string)value, "ReportServerTempDB", true) == 0) || (string.Compare((string)value, "tempdb", true) == 0))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class ModbusDisplaySerConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            string IPPort = value[0] + "\n" + "Slave Address: " + value[1] + "\n" + "Com-порт: " + value[2];

            return IPPort;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class EthernetSerSlaveAddressPortConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            string IPPort = value[0] + "\n" + "Slave address: " + value[1] + "\n" + "Порт сервера: " + value[2];

            return IPPort;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class EthernetSerIPPortConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {           
            string IPPort = value[0] + "\n" + "IP сервер: " + value[1] + "\n" + "Порт сервера: " + value[2];

            return IPPort;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
    public class EthernetSerIPConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string[] arrayIP = (string[])value;

            string IP = arrayIP[0] + "." + arrayIP[1] + "." + arrayIP[2] + "." + arrayIP[3];

            return IP;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
