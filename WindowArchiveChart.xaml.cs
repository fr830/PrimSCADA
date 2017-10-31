using System;
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
using System.Windows.Shapes;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Wpf;
using System.Data.SqlClient;
using System.Data;
using SUT.PrintEngine.Utils;

namespace SCADA
{
    /// <summary>
    /// Логика взаимодействия для WindowArchiveChart.xaml
    /// </summary>
    public partial class WindowArchiveChart : Window
    {
        public Plot Plot;

        public WindowArchiveChart(List<string> colSelect, List<string> colTableName)
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

                try
                {
                    conn.Open();

                    PlotModel plotModel = new PlotModel();

                    if (colSelect.Count >= 2)
                    {
                        int count = 0;

                        while (true)
                        {
                            DataTable dataTable = new DataTable();

                            Npgsql.NpgsqlDataAdapter adapter = new Npgsql.NpgsqlDataAdapter(colSelect[count], conn);
                            adapter.Fill(dataTable);

                            adapter.Dispose();

                            OxyPlot.Series.LineSeries line = new OxyPlot.Series.LineSeries()
                            {
                                CanTrackerInterpolatePoints = false,
                                Title = string.Format(colTableName[count]),
                                Smooth = false,
                                TrackerFormatString = "{0}" + Environment.NewLine + "{3} {4}" + Environment.NewLine + "{1} {2:dd/MM/yyyy HH:mm:ss}"
                            };

                            DataView data = dataTable.DefaultView;

                            foreach (DataRowView rowView in data)
                            {
                                DataRow row = rowView.Row;

                                DateTime dt = (DateTime)row.ItemArray[1];

                                double d = Convert.ToDouble(row.ItemArray[0]);

                                line.Points.Add(new DataPoint(OxyPlot.Axes.DateTimeAxis.ToDouble(dt), d));
                            }

                            plotModel.Series.Add(line);

                            count++;

                            if (count == colSelect.Count)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        DataTable dataTable = new DataTable();

                        Npgsql.NpgsqlDataAdapter adapter = new Npgsql.NpgsqlDataAdapter(colSelect[0], conn);
                        adapter.Fill(dataTable);

                        adapter.Dispose();

                        OxyPlot.Series.LineSeries line = new OxyPlot.Series.LineSeries()
                        {
                            CanTrackerInterpolatePoints = false,
                            Title = string.Format(colTableName[0]),
                            Smooth = false,
                            TrackerFormatString = "{0}" + Environment.NewLine + "{3} {4}" + Environment.NewLine + "{1} {2:dd/MM/yyyy HH:mm:ss}"
                        };

                        DataView data = dataTable.DefaultView;

                        foreach (DataRowView rowView in data)
                        {
                            DataRow row = rowView.Row;

                            DateTime dt = (DateTime)row.ItemArray[1];

                            double d = Convert.ToDouble(row.ItemArray[0]);

                            line.Points.Add(new DataPoint(OxyPlot.Axes.DateTimeAxis.ToDouble(dt), d));
                        }

                        plotModel.Series.Add(line);
                    }

                    plotModel.LegendTitle = "Легенда";
                    plotModel.LegendOrientation = LegendOrientation.Horizontal;
                    plotModel.LegendPlacement = LegendPlacement.Outside;
                    plotModel.LegendPosition = LegendPosition.TopRight;
                    plotModel.LegendBackground = OxyColor.FromAColor(200, OxyColors.White);
                    plotModel.LegendBorder = OxyColors.Black;

                    var dateAxis = new OxyPlot.Axes.DateTimeAxis(OxyPlot.Axes.AxisPosition.Bottom, "Дата", "dd/MM HH:mm") { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Solid, IntervalLength = 65 };
                    plotModel.Axes.Add(dateAxis);
                    var valueAxis = new OxyPlot.Axes.LinearAxis(AxisPosition.Left) { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Solid, Title = "Значение" };
                    valueAxis.MaximumPadding = 0.3;
                    valueAxis.MinimumPadding = 0.3;
                    plotModel.Axes.Add(valueAxis);

                    Plot = new Plot();
                    Plot.SetValue(Grid.RowProperty, 1);
                    Plot.Model = plotModel;
                    Plot.MinHeight = 100;
                    Plot.MinWidth = 100;

                    GridMain.Children.Add(Plot);
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
            var visualSize = new Size(Plot.ActualWidth, Plot.ActualHeight);
            var printControl = PrintControlFactory.Create(visualSize, Plot);
            printControl.ShowPrintPreview();

            e.Handled = true;
        }       
    }
}
