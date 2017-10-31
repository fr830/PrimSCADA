using Npgsql;
using SUT.PrintEngine.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SCADA
{
    /// <summary>
    /// Логика взаимодействия для WindowArchiveTable.xaml
    /// </summary>
    public partial class WindowArchiveTable : Window
    {
        DataTable DataTable = new DataTable();

        public WindowArchiveTable(string colSelect, List<string> colTableName)
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

                int count = 0;

                while(true)
                {
                    colTableName[count] = colTableName[count].Replace(' ', '_');

                    count++;

                    if (count == colTableName.Count)
                    {
                        break;
                    }
                }

                try
                {
                    conn.Open();

                    if (colTableName.Count >= 2)
                    {
                        count = 0;
                        string tables = "SELECT ";

                        while (true)
                        {
                            tables += colTableName[count] + @".""Value"", ";

                            count++;

                            if (count == colTableName.Count)
                            {
                                break;
                            }
                        }

                        count = 0;

                        tables += "COALESCE(";

                        while (true)
                        {
                            if (count + 1 == colTableName.Count)
                            {
                                tables += colTableName[count] + @".""Time"") AS Время FROM " + colTableName[0];
                            }
                            else
                            {
                                tables += colTableName[count] + @".""Time"", ";
                            }

                            count++;

                            if (count == colTableName.Count)
                            {
                                break;
                            }
                        }

                        count = 1;

                        while (true)
                        {
                            if (colTableName.Count == 2)
                            {
                                tables += " FULL OUTER JOIN " + colTableName[count] + @" USING(""Time"")";
                                break;
                            }
                            else
                            {
                                tables += " FULL OUTER JOIN " + colTableName[count] + @" USING(""Time"")";
                            }

                            count++;

                            if (count == colTableName.Count)
                            {
                                break;
                            }
                        }

                        tables += colSelect;

                        Npgsql.NpgsqlDataAdapter adapter = new Npgsql.NpgsqlDataAdapter(tables, conn);
                        adapter.Fill(DataTable);

                        adapter.Dispose();

                        int countR = 0;

                        //List<int> colTimes = new List<int>();

                        //while (true)
                        //{
                        //    if (DataTable.Columns[countR].ColumnName.LastIndexOf("Time") > 0)
                        //    {
                        //        DataTable.Columns.RemoveAt(countR);
                        //    }

                        //    countR++;

                        //    if (countR == DataTable.Columns.Count)
                        //    {
                        //        break;
                        //    }
                        //}

                        countR = 0;
                        int countInRow = 0;
                        DataRow rowNext;
                        DataRow row;
                        DataTable.DefaultView.Sort = "Время asc";
                        DataTable = DataTable.DefaultView.ToTable();
                        if (DataTable.Rows.Count > 0)
                        {
                            while (true)
                            {
                                row = DataTable.Rows[countR];

                                if (countR + 1 == DataTable.Rows.Count)
                                {
                                    break;
                                }

                                rowNext = DataTable.Rows[countR + 1];
                                DateTime dt1 = (DateTime)row[row.ItemArray.Length - 1];
                                DateTime dt2 = (DateTime)rowNext[row.ItemArray.Length - 1]; ;
                                if (dt1.Year == dt2.Year && dt1.Month == dt2.Month && dt1.Day == dt2.Day && dt1.Hour == dt2.Hour && dt1.Minute == dt2.Minute && dt1.Second == dt2.Second)
                                {
                                    while (true)
                                    {
                                        if (row[countInRow] is DBNull && !(rowNext[countInRow] is DBNull))
                                        {
                                            row[countInRow] = rowNext[countInRow];
                                        }

                                        countInRow++;

                                        if (countInRow == row.ItemArray.Length)
                                        {
                                            countInRow = 0;
                                            DataTable.Rows.RemoveAt(countR + 1);
                                            break;
                                        }
                                    }
                                }

                                countR++;

                                if (countR == DataTable.Rows.Count)
                                {
                                    break;
                                }
                            }
                        }

                        count = 0;

                        while (true)
                        {
                            if (count == DataTable.Columns.Count - 1)
                            {
                                break;
                            }

                            colTableName[count] = colTableName[count].Replace('_', ' ');

                            DataTable.Columns[count].ColumnName = colTableName[count];

                            count++;

                            if (count == DataTable.Rows.Count)
                            {
                                break;
                            }
                        }

                        DGTable.IsReadOnly = true;
                        DGTable.ItemsSource = DataTable.DefaultView;
                        DGTable.AutoGenerateColumns = true;
                        DGTable.MinHeight = 400;
                        DGTable.MinWidth = 300;
                    }
                    else
                    {
                        Npgsql.NpgsqlDataAdapter adapter = new Npgsql.NpgsqlDataAdapter(colSelect, conn);
                        adapter.Fill(DataTable);

                        DataTable.Columns["Value"].ColumnName = colTableName[0].Replace('_', ' ');
                        DataTable.Columns["Time"].ColumnName = "Время";

                        adapter.Dispose();

                        DGTable.IsReadOnly = true;
                        DGTable.ItemsSource = DataTable.DefaultView;
                        DGTable.AutoGenerateColumns = true;
                        DGTable.MinHeight = 400;
                        DGTable.MinWidth = 300;
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
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var columnWidths = new List<double>();

            foreach (DataGridColumn column in DGTable.Columns)
            {
                columnWidths.Add(((DataGridLength)column.Width).DesiredValue);
            }

            var ht = new HeaderTemplate();

            var headerTemplate = XamlWriter.Save(ht);
            var printControl = PrintControlFactory.Create(DataTable, columnWidths, headerTemplate);
            printControl.ShowPrintPreview();

            e.Handled = true;
        }
    }
}
