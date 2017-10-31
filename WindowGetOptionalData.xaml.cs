using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SCADA
{
    /// <summary>
    /// Логика взаимодействия для WindowGetOptionalData.xaml
    /// </summary>
    public partial class WindowGetOptionalData : Window
    {
        public WindowGetOptionalData(List<string> colSelect, List<string> colTableName, string[] sOptionals, string formTime, string toTime)
        {
            InitializeComponent();

            DataTable DataTable = new DataTable();

            AppWPF app = (AppWPF)Application.Current;

            MainWindow window = (MainWindow)Application.Current.MainWindow;

            if (app.ConfigProgramBin.UseDatabase)
            {
                string connstring = String.Format("Server={0};Port={1};" +
                    "User Id={2};Password={3};Database={4};",
                    app.ConfigProgramBin.SQLServerName, 5432, app.ConfigProgramBin.SQLUserName,
                    app.ConfigProgramBin.SQLPassword, app.ConfigProgramBin.SQLDatabaseName);


                Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(connstring);

                int count = 0;

                while (true)
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

                    DataTable dt = new System.Data.DataTable();
                    dt.Columns.Add("Value", typeof(decimal));
                    dt.Columns.Add("Time", typeof(DateTime));
                    dt.Columns.Add("Опция", typeof(string));

                    DataTable.Columns.Add("Value", typeof(decimal));
                    DataTable.Columns.Add("Time", typeof(DateTime));
                    DataTable.Columns.Add("Опция", typeof(string));

                    object[] obj = new object[3];

                    int countOptional = 0;

                    DateTime dtForm = DateTime.Parse(formTime);
                    DateTime dtTo = DateTime.Parse(toTime);

                    int intCount = 1;

                    foreach (string str in sOptionals)
                    {
                        if (countOptional == 0)
                        {
                            if (str == "max")
                            {
                                int i = colSelect[0].IndexOf("(select ");
                                string s = colSelect[0].Insert(i + 8, "max");
                                Npgsql.NpgsqlDataAdapter adapter = new Npgsql.NpgsqlDataAdapter(s, conn);
                                adapter.FillSchema(DataTable, SchemaType.Source);
                                adapter.Fill(DataTable);
                                adapter.Dispose();

                                foreach (DataRow dr in DataTable.Rows)
                                {
                                    obj[0] = dr.ItemArray[0];
                                    obj[1] = dr.ItemArray[1];
                                    obj[2] = "Максимальное значение";
                                    dr.ItemArray = obj;
                                }

                                int index = 0;

                                while (true)
                                {
                                    DataRow dr = DataTable.Rows[index];

                                    if ((DateTime)dr.ItemArray[1] < dtForm || (DateTime)dr.ItemArray[1] > dtTo)
                                    {
                                        DataTable.Rows.Remove(dr);

                                        index--;
                                    }

                                    index++;

                                    if (DataTable.Rows.Count == index)
                                    {
                                        break;
                                    }
                                }

                                countOptional++;
                            }
                            else if (str == "min")
                            {
                                int i = colSelect[0].IndexOf("(select ");
                                string s = colSelect[0].Insert(i + 8, "min");
                                Npgsql.NpgsqlDataAdapter adapter = new Npgsql.NpgsqlDataAdapter(s, conn);
                                adapter.FillSchema(DataTable, SchemaType.Source);
                                adapter.Fill(DataTable);
                                adapter.Dispose();

                                foreach (DataRow dr in DataTable.Rows)
                                {
                                    obj[0] = dr.ItemArray[0];
                                    obj[1] = dr.ItemArray[1];
                                    obj[2] = "Минимальное значение";
                                    dr.ItemArray = obj;
                                }

                                int index = 0;

                                while (true)
                                {
                                    DataRow dr = DataTable.Rows[index];

                                    if ((DateTime)dr.ItemArray[1] < dtForm || (DateTime)dr.ItemArray[1] > dtTo)
                                    {
                                        DataTable.Rows.Remove(dr);

                                        index--;
                                    }

                                    index++;

                                    if (DataTable.Rows.Count == index)
                                    {
                                        break;
                                    }
                                }

                                countOptional++;
                            }
                            else if (str == "avg")
                            {
                                int i = colSelect[0].IndexOf("*");
                                string s = colSelect[0].Remove(i, 1);
                                s = s.Insert(7, "avg(\"Value\") ");
                                i = s.IndexOf("Where");
                                s = s.Remove(i + 6);
                                s = s + " \"Time\"" + " BETWEEN " + "'" + formTime + "'" + " AND " + "'" + toTime + "'";
                                Npgsql.NpgsqlDataAdapter adapter = new Npgsql.NpgsqlDataAdapter(s, conn);
                                adapter.FillSchema(DataTable, SchemaType.Source);
                                adapter.Fill(DataTable);
                                adapter.Dispose();

                                if (DataTable.Rows.Count != 0)
                                {
                                    foreach (DataRow dr in DataTable.Rows)
                                    {
                                        obj[0] = dr.ItemArray[3];
                                        obj[1] = DateTime.Now;
                                        obj[2] = "Среднее значение";
                                        dr.ItemArray = obj;
                                    }

                                    DataTable.Columns.RemoveAt(3);
                                }
                                else
                                {

                                }

                                countOptional++;
                            }
                            else if (str == "sum")
                            {
                                int i = colSelect[0].IndexOf("*");
                                string s = colSelect[0].Remove(i, 1);
                                s = s.Insert(7, "sum(\"Value\") ");
                                i = s.IndexOf("Where");
                                s = s.Remove(i + 6);
                                s = s + " \"Time\"" + " BETWEEN " + "'" + formTime + "'" + " AND " + "'" + toTime + "'";
                                Npgsql.NpgsqlDataAdapter adapter = new Npgsql.NpgsqlDataAdapter(s, conn);
                                adapter.Fill(DataTable);
                                adapter.Dispose();

                                if (DataTable.Rows.Count != 0)
                                {
                                    foreach (DataRow dr in DataTable.Rows)
                                    {
                                        obj[0] = dr.ItemArray[3];
                                        obj[1] = DateTime.Now;
                                        obj[2] = "Арифметическая сумма";
                                        dr.ItemArray = obj;
                                    }

                                    DataTable.Columns.RemoveAt(3);
                                }
                                else
                                {
                                    DataRow datarow = DataTable.NewRow();
                                    DataTable.Clear();

                                    obj[0] = 0M;
                                    obj[1] = DateTime.Now;
                                    obj[2] = "Арифметическая сумма. Пустая таблица.";
                                    datarow.ItemArray = obj;

                                    DataTable.Rows.Add(datarow);
                                }

                                countOptional++;
                            }
                            else if (str == "integralsum")
                            {
                                int i = colSelect[0].IndexOf("Where");
                                string s = colSelect[0].Remove(i + 6);
                                s = s + " \"Time\"" + " BETWEEN " + "'" + formTime + "'" + " AND " + "'" + toTime + "'";
                                Npgsql.NpgsqlDataAdapter adapter = new Npgsql.NpgsqlDataAdapter(s, conn);
                                adapter.Fill(DataTable);
                                adapter.Dispose();

                                DateTime form = new DateTime();
                                DateTime to = new DateTime();

                                decimal d = 0;

                                int init = 0;

                                decimal prevItem = 0;

                                TimeSpan ts;

                                if (DataTable.Rows.Count != 0)
                                {
                                    foreach (DataRow dr in DataTable.Rows)
                                    {
                                        if (intCount == 1)
                                        {
                                            form = (DateTime)dr.ItemArray[1];

                                            prevItem = (decimal)dr.ItemArray[0];

                                            intCount++;
                                        }
                                        else if (intCount == 2)
                                        {
                                            if (init == 0)
                                            {
                                                to = (DateTime)dr.ItemArray[1];
                                                ts = to.Subtract(form);

                                                form = to;

                                                d = ((decimal)ts.TotalSeconds / 3600) * prevItem;

                                                prevItem = (decimal)dr.ItemArray[0];

                                                init = 1;
                                            }
                                            else
                                            {
                                                to = (DateTime)dr.ItemArray[1];
                                                ts = to.Subtract(form);

                                                form = to;

                                                d += ((decimal)ts.TotalSeconds / 3600) * prevItem;

                                                prevItem = (decimal)dr.ItemArray[0];
                                            }
                                        }
                                    }

                                    DataRow datarow = DataTable.NewRow();
                                    DataTable.Clear();

                                    obj[0] = d;
                                    obj[1] = DateTime.Now;
                                    obj[2] = "Интегральная сумма";
                                    datarow.ItemArray = obj;

                                    DataTable.Rows.Add(datarow);
                                }
                                else
                                {
                                    DataRow datarow = DataTable.NewRow();
                                    DataTable.Clear();

                                    obj[1] = DateTime.Now;
                                    obj[2] = "Интегральная сумма. Пустая таблица.";
                                    datarow.ItemArray = obj;

                                    DataTable.Rows.Add(datarow);
                                }

                                countOptional++;
                            }
                        }
                        else
                        {
                            if (str == "max")
                            {
                                int i = colSelect[0].IndexOf("(select ");
                                string s = colSelect[0].Insert(i + 8, "max");
                                Npgsql.NpgsqlDataAdapter adapter = new Npgsql.NpgsqlDataAdapter(s, conn);
                                adapter.FillSchema(dt, SchemaType.Source);
                                dt.Rows.Clear();
                                adapter.Fill(dt);
                                adapter.Dispose();

                                foreach (DataRow dr in dt.Rows)
                                {
                                    obj[0] = dr.ItemArray[0];
                                    obj[1] = dr.ItemArray[1];
                                    obj[2] = "Максимальное значение";
                                    dr.ItemArray = obj;
                                }

                                int index = 0;

                                while (true)
                                {
                                    DataRow dr = dt.Rows[index];

                                    if ((DateTime)dr.ItemArray[1] < dtForm || (DateTime)dr.ItemArray[1] > dtTo)
                                    {
                                        dt.Rows.Remove(dr);

                                        index--;
                                    }

                                    index++;

                                    if (dt.Rows.Count == index)
                                    {
                                        break;
                                    }
                                }

                                DataTable.Merge(dt);

                                countOptional++;
                            }
                            else if (str == "min")
                            {
                                int i = colSelect[0].IndexOf("(select ");
                                string s = colSelect[0].Insert(i + 8, "min");
                                Npgsql.NpgsqlDataAdapter adapter = new Npgsql.NpgsqlDataAdapter(s, conn);
                                adapter.FillSchema(dt, SchemaType.Source);
                                dt.Rows.Clear();
                                adapter.Fill(dt);
                                adapter.Dispose();

                                foreach (DataRow dr in dt.Rows)
                                {
                                    obj[0] = dr.ItemArray[0];
                                    obj[1] = dr.ItemArray[1];
                                    obj[2] = "Минимальное значение";
                                    dr.ItemArray = obj;
                                }

                                int index = 0;

                                while (true)
                                {
                                    DataRow dr = dt.Rows[index];

                                    if ((DateTime)dr.ItemArray[1] < dtForm || (DateTime)dr.ItemArray[1] > dtTo)
                                    {
                                        dt.Rows.Remove(dr);

                                        index--;
                                    }

                                    index++;

                                    if (dt.Rows.Count == index)
                                    {
                                        break;
                                    }
                                }

                                DataTable.Merge(dt);

                                countOptional++;
                            }
                            else if (str == "avg")
                            {
                                int i = colSelect[0].IndexOf("*");
                                string s = colSelect[0].Remove(i, 1);
                                s = s.Insert(7, "avg(\"Value\") ");
                                i = s.IndexOf("Where");
                                s = s.Remove(i + 6);
                                s = s + " \"Time\"" + " BETWEEN " + "'" + formTime + "'" + " AND " + "'" + toTime + "'";
                                Npgsql.NpgsqlDataAdapter adapter = new Npgsql.NpgsqlDataAdapter(s, conn);
                                adapter.FillSchema(dt, SchemaType.Source);
                                dt.Rows.Clear();
                                adapter.Fill(dt);
                                adapter.Dispose();

                                dt.Columns[0].ColumnName = "Value";

                                foreach (DataRow dr in dt.Rows)
                                {
                                    obj[0] = dr.ItemArray[0];
                                    obj[1] = DateTime.Now;
                                    obj[2] = "Среднее значение";
                                    dr.ItemArray = obj;
                                }

                                dt.Columns.RemoveAt(3);

                                DataTable.Merge(dt);

                                countOptional++;
                            }
                            else if (str == "sum")
                            {
                                int i = colSelect[0].IndexOf("*");
                                string s = colSelect[0].Remove(i, 1);
                                s = s.Insert(7, "sum(\"Value\") ");
                                i = s.IndexOf("Where");
                                s = s.Remove(i + 6);
                                s = s + " \"Time\"" + " BETWEEN " + "'" + formTime + "'" + " AND " + "'" + toTime + "'";
                                Npgsql.NpgsqlDataAdapter adapter = new Npgsql.NpgsqlDataAdapter(s, conn);
                                dt.Rows.Clear();
                                adapter.Fill(dt);
                                adapter.Dispose();

                                foreach (DataRow dr in dt.Rows)
                                {
                                    obj[0] = dr.ItemArray[3];
                                    obj[1] = DateTime.Now;
                                    obj[2] = "Сумма";
                                    dr.ItemArray = obj;
                                }

                                dt.Columns.RemoveAt(3);

                                DataTable.Merge(dt);

                                countOptional++;
                            }

                        }
                    }

                    DataTable.Columns["Value"].ColumnName = colTableName[0].Replace('_', ' ');
                    DataTable.Columns["Time"].ColumnName = "Время";

                    DGTable.IsReadOnly = true;
                    DGTable.ItemsSource = DataTable.DefaultView;
                    DGTable.AutoGenerateColumns = true;
                    DGTable.MinHeight = 400;
                    DGTable.MinWidth = 300;
                }
                catch (SystemException ex)
                {
                    conn.Close();

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
                finally
                {
                    conn.Close();
                }
            }           
        }
    }
}
