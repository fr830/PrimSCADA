using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace SCADA
{
    /// <summary>
    /// Логика взаимодействия для WindowArchive.xaml
    /// </summary>
    public partial class WindowArchive : Window
    {
        public IList<string> CollectionTables = new List<string>();

        public WindowArchive()
        {
            InitializeComponent();

            AppWPF app = (AppWPF)Application.Current;

            MainWindow window = (MainWindow)Application.Current.MainWindow;

            if (app.ConfigProgramBin.UseDatabase)
            {
                //SqlConnectionStringBuilder Sqlbuilder = new SqlConnectionStringBuilder();
                //Sqlbuilder.DataSource = app.ConfigProgramBin.SQLServerName;
                //Sqlbuilder.InitialCatalog = app.ConfigProgramBin.SQLDatabaseName;

                //if (((AppWPF)Application.Current).ConfigProgramBin.SQLSecuritySSPI)
                //{
                //    Sqlbuilder.IntegratedSecurity = true;
                //}
                //else
                //{
                //    Sqlbuilder.UserID = app.ConfigProgramBin.SQLUserName;
                //    Sqlbuilder.Password = app.ConfigProgramBin.SQLPassword;
                //}

                string connstring = String.Format("Server={0};Port={1};" +
                    "User Id={2};Password={3};Database={4};",
                    app.ConfigProgramBin.SQLServerName, 5432, app.ConfigProgramBin.SQLUserName,
                    app.ConfigProgramBin.SQLPassword, app.ConfigProgramBin.SQLDatabaseName);


                Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(connstring);

                string sql = "Select table_name FROM information_schema.tables WHERE table_schema = 'public'";

                Npgsql.NpgsqlCommand command = null;

                try
                {
                    conn.Open();

                    command = new Npgsql.NpgsqlCommand(sql, conn);

                    Npgsql.NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string table = reader.GetString(0);
                        CollectionTables.Add(table.Replace('_', ' '));
                    }

                    reader.Close();

                    LBTables.ItemsSource = CollectionTables;
                    LBTables.SelectionMode = SelectionMode.Multiple;

                    Binding bindingLoadTable = new Binding();
                    bindingLoadTable.Source = LBTables;
                    bindingLoadTable.Path = new PropertyPath("SelectedItem");
                    bindingLoadTable.Converter = new RemoveButtonConverter();

                    Binding bindingLoadTableChart = new Binding();
                    bindingLoadTableChart.Source = LBTables;
                    bindingLoadTableChart.Path = new PropertyPath("SelectedItem");
                    bindingLoadTableChart.Converter = new RemoveButtonConverter();

                    Binding bindingGetOptionalData = new Binding();
                    bindingGetOptionalData.Source = LBTables;
                    bindingGetOptionalData.Path = new PropertyPath("SelectedItem");
                    bindingGetOptionalData.Converter = new RemoveButtonConverter();

                    Binding bindingGetOptionalData2 = new Binding();
                    bindingGetOptionalData2.Source = CHBAverage;
                    bindingGetOptionalData2.Path = new PropertyPath("IsChecked");

                    Binding bindingGetOptionalData3 = new Binding();
                    bindingGetOptionalData3.Source = CHBSum;
                    bindingGetOptionalData3.Path = new PropertyPath("IsChecked");

                    Binding bindingGetOptionalData4 = new Binding();
                    bindingGetOptionalData4.Source = CHBMax;
                    bindingGetOptionalData4.Path = new PropertyPath("IsChecked");

                    Binding bindingGetOptionalData5 = new Binding();
                    bindingGetOptionalData5.Source = CHBMin;
                    bindingGetOptionalData5.Path = new PropertyPath("IsChecked");

                    Binding bindingGetOptionalData6 = new Binding();
                    bindingGetOptionalData6.Source = CHBIntegralSum;
                    bindingGetOptionalData6.Path = new PropertyPath("IsChecked");

                    MultiBinding mBindingGetOptionalData = new MultiBinding();
                    mBindingGetOptionalData.Converter = new GetOptionalData();
                    mBindingGetOptionalData.Bindings.Add(bindingGetOptionalData);
                    mBindingGetOptionalData.Bindings.Add(bindingGetOptionalData2);
                    mBindingGetOptionalData.Bindings.Add(bindingGetOptionalData3);
                    mBindingGetOptionalData.Bindings.Add(bindingGetOptionalData4);
                    mBindingGetOptionalData.Bindings.Add(bindingGetOptionalData5);
                    mBindingGetOptionalData.Bindings.Add(bindingGetOptionalData6);

                    BLoadTable.SetBinding(Button.IsEnabledProperty, bindingLoadTable);

                    BLoadTableChart.SetBinding(Button.IsEnabledProperty, bindingLoadTableChart);

                    BGetOptionalData.SetBinding(Button.IsEnabledProperty, mBindingGetOptionalData);

                    DataPickerForm.DisplayDateEnd = DateTime.Now;
                    DataPickerForm.ToolTip = "Диапазон даты от";
                    DataPickerForm.SelectedDateChanged += DataPickerForm_SelectedDateChanged;
                    DataPickerForm.SelectedDate = DateTime.Now;

                    TimePickerForm.ValueChanged += TimePickerForm_ValueChanged;
                    TimePickerForm.ToolTip = "Диапазон времени от";
                    TimePickerForm.Format = TimeFormat.Custom;
                    TimePickerForm.FormatString = "HH:mm:ss";

                    if (DateTime.Now.Hour == 0)
                    {
                        TimePickerForm.Value = DateTime.Now;
                    }
                    else
                    {
                        TimePickerForm.Value = DateTime.Now.AddHours(-1);
                    }

                    DataPickerTo.DisplayDateEnd = DateTime.Now;
                    DataPickerTo.ToolTip = "Диапазон даты до";
                    DataPickerTo.SelectedDateChanged += DataPickerTo_SelectedDateChanged;
                    DataPickerTo.SelectedDate = DateTime.Now;

                    TimePickerTo.ValueChanged += TimePickerTo_ValueChanged;
                    TimePickerTo.ToolTip = "Диапазон времени до";
                    TimePickerTo.Format = TimeFormat.Custom;
                    TimePickerTo.FormatString = "HH:mm:ss";
                    TimePickerTo.Value = DateTime.Now;
                }
                catch (SystemException ex)
                {                   
                    if (window.CollectionMessage.Count > 300)
                    {
                        window.CollectionMessage.RemoveAt(0);

                        window.CollectionMessage.Insert(298, "Сообщение " + " : " + "Ошибка в окне Архива " + ex.Message + "  " + DateTime.Now);
                    }
                    else
                    {
                        window.CollectionMessage.Add("Сообщение " + " : " + "Ошибка в окне Архива " + ex.Message + "  " + DateTime.Now);
                    }

                    //if (ex is SqlException)
                    //{
                    //    SqlException sqlex = ex as SqlException;

                    //    foreach (SqlError er in sqlex.Errors)
                    //    {
                    //        if (window.WindowErrorMessages.LBMessageError.Text.Length > 0)
                    //        {
                    //            window.CountLineTextMessage++;
                    //            window.WindowErrorMessages.LBMessageError.Text += "\n" + "Сообщение " + window.CountLineTextMessage.ToString() + " : " + "Ошибка в окне Архива " + er.Message + "  " + DateTime.Now;
                    //        }
                    //        else
                    //        {
                    //            window.CountLineTextMessage++;
                    //            window.WindowErrorMessages.LBMessageError.Text = "Сообщение " + window.CountLineTextMessage.ToString() + " : " + "Ошибка в окне Архива " + er.Message + "  " + DateTime.Now;
                    //        }
                    //    }
                    //}
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();

                    if (command != null)
                    {
                        command.Dispose();
                    }
                }
            }      
        }        

        void DataPickerForm_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePicker dp = (DatePicker)sender;

            DateTime dtForm;

            DateTime dtTo;

            if (TimePickerForm.Value == null)
            {
                if (DateTime.Now.Hour == 0)
                {
                    dtForm = new DateTime(((DateTime)e.AddedItems[0]).Year, ((DateTime)e.AddedItems[0]).Month, ((DateTime)e.AddedItems[0]).Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                }
                else
                {
                    dtForm = new DateTime(((DateTime)e.AddedItems[0]).Year, ((DateTime)e.AddedItems[0]).Month, ((DateTime)e.AddedItems[0]).Day, DateTime.Now.Hour - 1, DateTime.Now.Minute, DateTime.Now.Second);
                }
            }
            else
            {
                dtForm = new DateTime(((DateTime)e.AddedItems[0]).Year, ((DateTime)e.AddedItems[0]).Month, ((DateTime)e.AddedItems[0]).Day, TimePickerForm.Value.Value.Hour, TimePickerForm.Value.Value.Minute, TimePickerForm.Value.Value.Second);
            }

            if (DataPickerTo.SelectedDate == null && TimePickerTo.Value == null)
            {
                dtTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            }
            else
            {
                dtTo = new DateTime(DataPickerTo.SelectedDate.Value.Year, DataPickerTo.SelectedDate.Value.Month, DataPickerTo.SelectedDate.Value.Day, TimePickerTo.Value.Value.Hour, TimePickerTo.Value.Value.Minute, TimePickerTo.Value.Value.Second);
            }            

            if (dtForm > dtTo || dtForm > DateTime.Now)
            {
                dp.SelectedDate = (DateTime)e.RemovedItems[0];
            }

            e.Handled = true;
        }

        void DataPickerTo_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePicker dp = (DatePicker)sender;

            DateTime dtTo;

            DateTime dtForm = new DateTime(DataPickerTo.SelectedDate.Value.Year, DataPickerTo.SelectedDate.Value.Month, DataPickerTo.SelectedDate.Value.Day, TimePickerForm.Value.Value.Hour, TimePickerForm.Value.Value.Minute, TimePickerForm.Value.Value.Second);

            if (TimePickerTo.Value == null)
            {
                dtTo = new DateTime(((DateTime)e.AddedItems[0]).Year, ((DateTime)e.AddedItems[0]).Month, ((DateTime)e.AddedItems[0]).Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            }
            else
            {
                dtTo = new DateTime(((DateTime)e.AddedItems[0]).Year, ((DateTime)e.AddedItems[0]).Month, ((DateTime)e.AddedItems[0]).Day, TimePickerTo.Value.Value.Hour, TimePickerTo.Value.Value.Minute, TimePickerTo.Value.Value.Second);
            }

            if (dtTo < dtForm || dtTo > DateTime.Now)
            {
                dp.SelectedDate = (DateTime)e.RemovedItems[0];
            }

            e.Handled = true;
        }

        void TimePickerForm_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TimePicker tm = (TimePicker)sender;

            DateTime dtForm = new DateTime(DataPickerForm.SelectedDate.Value.Year, DataPickerForm.SelectedDate.Value.Month, DataPickerForm.SelectedDate.Value.Day, ((DateTime)e.NewValue).Hour, ((DateTime)e.NewValue).Minute, ((DateTime)e.NewValue).Second);

            DateTime dtTo;

            if (DataPickerTo.SelectedDate == null && TimePickerTo.Value == null)
            {
                dtTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            }
            else
            {
                dtTo = new DateTime(DataPickerTo.SelectedDate.Value.Year, DataPickerTo.SelectedDate.Value.Month, DataPickerTo.SelectedDate.Value.Day, TimePickerTo.Value.Value.Hour, TimePickerTo.Value.Value.Minute, TimePickerTo.Value.Value.Second);
            }

            if (dtForm > dtTo || dtForm > DateTime.Now)
            {
                tm.Value = (DateTime)e.OldValue;
            }

            e.Handled = true;
        }

        void TimePickerTo_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TimePicker tm = (TimePicker)sender;

            DateTime dtForm = new DateTime(DataPickerForm.SelectedDate.Value.Year, DataPickerForm.SelectedDate.Value.Month, DataPickerForm.SelectedDate.Value.Day, TimePickerForm.Value.Value.Hour, TimePickerForm.Value.Value.Minute, TimePickerForm.Value.Value.Second);

            DateTime dtTo = new DateTime(DataPickerTo.SelectedDate.Value.Year, DataPickerTo.SelectedDate.Value.Month, DataPickerTo.SelectedDate.Value.Day, ((DateTime)e.NewValue).Hour, ((DateTime)e.NewValue).Minute, ((DateTime)e.NewValue).Second);

            if (dtTo < dtForm || dtTo > DateTime.Now)
            {
                tm.Value = (DateTime)e.OldValue;
            }

            e.Handled = true;
        }     

        private void BLoadTable_Click(object sender, RoutedEventArgs e)
        {
            AppWPF app = (AppWPF)Application.Current;

            MainWindow window = (MainWindow)Application.Current.MainWindow;

            try
            {
                if (app.ConfigProgramBin.UseDatabase)
                {
                    DateTime form = DataPickerForm.SelectedDate.Value;
                    form = form.AddHours(TimePickerForm.Value.Value.Hour);
                    form = form.AddMinutes(TimePickerForm.Value.Value.Minute);
                    form = form.AddSeconds(TimePickerForm.Value.Value.Second);

                    DateTime to = DataPickerTo.SelectedDate.Value;
                    to = to.AddHours(TimePickerTo.Value.Value.Hour);
                    to = to.AddMinutes(TimePickerTo.Value.Value.Minute);
                    to = to.AddSeconds(TimePickerTo.Value.Value.Second);

                    string format = "yyyy/MM/dd HH:mm:ss";

                    string colSelect = null;

                    List<string> colTableName = new List<string>();

                    if (LBTables.SelectedItems.Count >= 2)
                    {
                        int count = 0;

                        while (true)
                        {
                            colSelect = " WHERE " + "\"Time\"" + " BETWEEN " + "'" + form.ToString(format) + "'" + " AND " + "'" + to.ToString(format) + "'";
                            colTableName.Add(LBTables.SelectedItems[count].ToString());

                            count++;

                            if (count == LBTables.SelectedItems.Count)
                            {
                                WindowArchiveTable wArchiveTable = new WindowArchiveTable(colSelect, colTableName);
                                wArchiveTable.Owner = Application.Current.MainWindow;
                                wArchiveTable.Show();

                                break;
                            }
                        }
                    }
                    else
                    {
                        colSelect = "select * from " + "\"" + LBTables.SelectedItem.ToString().Replace(' ', '_') + "\"" + " WHERE " + "\"Time\"" + " BETWEEN " + "'" + form.ToString(format) + "'" + " AND " + "'" + to.ToString(format) + "'";
                        colTableName.Add(LBTables.SelectedItem.ToString());

                        WindowArchiveTable wArchiveChart = new WindowArchiveTable(colSelect, colTableName);
                        wArchiveChart.Owner = Application.Current.MainWindow;
                        wArchiveChart.Show();
                    }
                }
            }            
            catch(SystemException ex)
            {
                if (window.CollectionMessage.Count > 300)
                {
                    window.CollectionMessage.RemoveAt(0);

                    window.CollectionMessage.Insert(298, "Сообщение " + " : " + "Ошибка в окне Архива " + ex.Message + "  " + DateTime.Now);
                }
                else
                {
                    window.CollectionMessage.Add("Сообщение " + " : " + "Ошибка в окне Архива " + ex.Message + "  " + DateTime.Now);
                } 

                //if (ex is SqlException)
                //{
                //    SqlException sqlex = ex as SqlException;

                //    foreach (SqlError er in sqlex.Errors)
                //    {
                //        if (window.WindowErrorMessages.LBMessageError.Text.Length > 0)
                //        {
                //            window.CountLineTextMessage++;
                //            window.WindowErrorMessages.LBMessageError.Text += "\n" + "Сообщение " + window.CountLineTextMessage.ToString() + " : " + "Ошибка в окне Архива " + er.Message + "  " + DateTime.Now;
                //        }
                //        else
                //        {
                //            window.CountLineTextMessage++;
                //            window.WindowErrorMessages.LBMessageError.Text = "Сообщение " + window.CountLineTextMessage.ToString() + " : " + "Ошибка в окне Архива " + er.Message + "  " + DateTime.Now;
                //        }
                //    }
                //}
            }
                
             //SqlConnectionStringBuilder Sqlbuilder = new SqlConnectionStringBuilder();
             //Sqlbuilder.DataSource = app.ConfigProgramBin.SQLServerName;
             //Sqlbuilder.InitialCatalog = app.ConfigProgramBin.SQLDatabaseName;

             //if (((AppWPF)Application.Current).ConfigProgramBin.SQLSecuritySSPI)
             //{
             //    Sqlbuilder.IntegratedSecurity = true;
             //}
             //else
             //{
             //    Sqlbuilder.UserID = app.ConfigProgramBin.SQLUserName;
             //    Sqlbuilder.Password = app.ConfigProgramBin.SQLPassword;
             //}

             e.Handled = true;
        }

        private void BLoadTableChart_Click(object sender, RoutedEventArgs e)
        {
            DateTime form = DataPickerForm.SelectedDate.Value;
            form = form.AddHours(TimePickerForm.Value.Value.Hour);
            form = form.AddMinutes(TimePickerForm.Value.Value.Minute);
            form = form.AddSeconds(TimePickerForm.Value.Value.Second);

            DateTime to = DataPickerTo.SelectedDate.Value;
            to = to.AddHours(TimePickerTo.Value.Value.Hour);
            to = to.AddMinutes(TimePickerTo.Value.Value.Minute);
            to = to.AddSeconds(TimePickerTo.Value.Value.Second);

            string format = "yyyy/MM/dd HH:mm:ss";

            List<string> colSelect = new List<string>();

            List<string> colTableName = new List<string>();

            if (LBTables.SelectedItems.Count >= 2)
            {
                int count = 0;
              
                while (true)
                {
                    colSelect.Add("select * from " + "\"" + LBTables.SelectedItems[count].ToString().Replace(' ', '_') + "\"" + " WHERE " + "\"Time\"" + " BETWEEN " + "'" + form.ToString(format) + "'" + " AND " + "'" + to.ToString(format) + "'");
                    colTableName.Add(LBTables.SelectedItems[count].ToString());

                    count++;

                    if (count == LBTables.SelectedItems.Count)
                    {
                        WindowArchiveChart wArchiveChart = new WindowArchiveChart(colSelect, colTableName);
                        wArchiveChart.Owner = Application.Current.MainWindow;
                        wArchiveChart.Show();

                        break;
                    }
                }              
            }
            else
            {
                colSelect.Add("select * from " + "\"" + LBTables.SelectedItems[0].ToString().Replace(' ', '_') + "\"" + " WHERE " + "\"Time\"" + " BETWEEN " + "'" + form.ToString(format) + "'" + " AND " + "'" + to.ToString(format) + "'");
                colTableName.Add(LBTables.SelectedItem.ToString());

                WindowArchiveChart wArchiveChart = new WindowArchiveChart(colSelect, colTableName);
                wArchiveChart.Owner = Application.Current.MainWindow;
                wArchiveChart.Show();
            }

            e.Handled = true;
        }

        private void BGetOptionalData_Click(object sender, RoutedEventArgs e)
        {
            
            AppWPF app = (AppWPF)Application.Current;

            MainWindow window = (MainWindow)Application.Current.MainWindow;

            try
            {
                if (app.ConfigProgramBin.UseDatabase)
                {
                    DateTime form = DataPickerForm.SelectedDate.Value;
                    form = form.AddHours(TimePickerForm.Value.Value.Hour);
                    form = form.AddMinutes(TimePickerForm.Value.Value.Minute);
                    form = form.AddSeconds(TimePickerForm.Value.Value.Second);

                    DateTime to = DataPickerTo.SelectedDate.Value;
                    to = to.AddHours(TimePickerTo.Value.Value.Hour);
                    to = to.AddMinutes(TimePickerTo.Value.Value.Minute);
                    to = to.AddSeconds(TimePickerTo.Value.Value.Second);

                    string format = "yyyy/MM/dd HH:mm:ss";

                    List<string> colSelect = new List<string>();

                    List<string> colTableName = new List<string>();

                    string[] sOptional = new string[5];

                    if ((bool)CHBAverage.IsChecked)
                    {
                        sOptional[0] = "avg";
                    }

                    if ((bool)CHBMax.IsChecked)
                    {
                        sOptional[1] = "max";
                    }

                    if ((bool)CHBMin.IsChecked)
                    {
                        sOptional[2] = "min";
                    }

                    if ((bool)CHBSum.IsChecked)
                    {
                        sOptional[3] = "sum";
                    }

                    if ((bool)CHBIntegralSum.IsChecked)
                    {
                        sOptional[4] = "integralsum";
                    }

                    if (LBTables.SelectedItems.Count >= 2)
                    {
                        int count = 0;

                        while (true)
                        {
                            colSelect.Add("select * from " + "\"" + LBTables.SelectedItems[count].ToString().Replace(' ', '_') + "\"" + " WHERE " + "\"Time\"" + " BETWEEN " + "'" + form.ToString(format) + "'" + " AND " + "'" + to.ToString(format) + "'");
                            colTableName.Add(LBTables.SelectedItems[count].ToString());

                            count++;

                            if (count == LBTables.SelectedItems.Count)
                            {
                                WindowGetOptionalData wArchiveOptional = new WindowGetOptionalData(colSelect, colTableName, sOptional, form.ToString(format), to.ToString(format));
                                wArchiveOptional.Owner = Application.Current.MainWindow;
                                wArchiveOptional.Show();

                                break;
                            }
                        }
                    }
                    else
                    {           
                        colSelect.Add("select * from " + "\"" + LBTables.SelectedItem.ToString().Replace(' ', '_') + "\"" + " Where \"Value\" = (select (\"Value\") from " + "\"" + LBTables.SelectedItem.ToString().Replace(' ', '_') + "\"" + " WHERE " + "\"Time\"" + " BETWEEN " + "'" + form.ToString(format) + "'" + " AND " + "'" + to.ToString(format) + "')");
                        colTableName.Add(LBTables.SelectedItem.ToString());

                        WindowGetOptionalData wArchiveOptional = new WindowGetOptionalData(colSelect, colTableName, sOptional, form.ToString(format), to.ToString(format));
                        wArchiveOptional.Owner = Application.Current.MainWindow;
                        wArchiveOptional.Show();
                    }
                }
            }
            catch (SystemException ex)
            {
                if (window.CollectionMessage.Count > 300)
                {
                    window.CollectionMessage.RemoveAt(0);

                    window.CollectionMessage.Insert(298, "Сообщение " + " : " + "Ошибка в окне Архива " + ex.Message + "  " + DateTime.Now);
                }
                else
                {
                    window.CollectionMessage.Add("Сообщение " + " : " + "Ошибка в окне Архива " + ex.Message + "  " + DateTime.Now);
                }                    
            }                                    

            e.Handled = true;
        }        
    }
}
