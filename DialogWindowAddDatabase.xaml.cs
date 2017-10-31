// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SCADA
{
    /// <summary>
    /// Логика взаимодействия для DialogWindowAddDatabase.xaml
    /// </summary>
    public partial class DialogWindowAddDatabase : Window
    {
        public Popup PopupMessage = new Popup();
        public Label LPopupMessage = new Label();

        public ComboBox CBDatabases;

        public string ServerName = "";
        public bool SSPI;
        public string Password = "";
        public string UserName = "";

        public DialogWindowAddDatabase()
        {
            InitializeComponent();

            LPopupMessage = new Label();
            LPopupMessage.BorderThickness = new Thickness(1);
            LPopupMessage.BorderBrush = Brushes.Red;
            LPopupMessage.Background = Brushes.White;

            PopupMessage.AllowsTransparency = true;
            PopupMessage.Child = LPopupMessage;
            PopupMessage.PopupAnimation = PopupAnimation.Fade;
            PopupMessage.StaysOpen = false;

            MenuItem menuItemPasteDatabaseName = new MenuItem();
            menuItemPasteDatabaseName.Command = ApplicationCommands.Paste;

            MenuItem menuItemCopyDatabaseName = new MenuItem();
            menuItemCopyDatabaseName.Command = ApplicationCommands.Copy;

            ContextMenu ContextMenuDatabaseName = new System.Windows.Controls.ContextMenu();
            ContextMenuDatabaseName.Items.Add(menuItemPasteDatabaseName);
            ContextMenuDatabaseName.Items.Add(menuItemCopyDatabaseName);

            TBNameDatabase.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
            TBNameDatabase.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, TextBoxPaste));
            TBNameDatabase.ContextMenu = ContextMenuDatabaseName;
            TBNameDatabase.PreviewKeyDown += TextBox_PreviewKeyDown;
            TBNameDatabase.PreviewTextInput += TextBox_PreviewTextInput;            
        }

        void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (tb.Text.Length + 1 > 40)
            {
                if (tb.SelectionLength > 0)
                {
                    if ((tb.Text.Length + 1) - tb.SelectionLength <= 40)
                    {
                        return;
                    }
                }

                if (tb == TBNameDatabase)
                {
                    LPopupMessage.Content = "Имя базы данных не может быть длинее 40 символов.";
                }               

                PopupMessage.PlacementTarget = tb;
                PopupMessage.IsOpen = true;

                e.Handled = true;
            }
        }

        private void IgnoreCut(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void TextBoxPaste(object sender, ExecutedRoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            string s;

            if (tb.SelectionLength > 0)
            {
                s = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                s = s.Insert(tb.SelectionStart, Clipboard.GetText());
            }
            else
            {
                s = tb.Text.Insert(tb.Text.Length, Clipboard.GetText());
            }

            if (s.Length > 40)
            {
                if (tb == TBNameDatabase)
                {
                    LPopupMessage.Content = "Имя базы данных не может быть длинее 40 символов.";
                }              

                PopupMessage.PlacementTarget = tb;
                PopupMessage.IsOpen = true;

                e.Handled = true;
            }
            else
            {
                tb.Paste();
            }
        }

        void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (tb.Text.Length > 40 && e.Key == Key.Enter)
            {
                if (tb == TBNameDatabase)
                {
                    LPopupMessage.Content = "Имя базы данных не может быть длинее 40 символов.";
                }               

                PopupMessage.PlacementTarget = tb;
                PopupMessage.IsOpen = true;

                e.Handled = true;
            }
            else
            {
                PopupMessage.IsOpen = false;
            }
        }

        private void BNewDatabase_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            AppWPF app = (AppWPF)Application.Current;

            if (TBNameDatabase.Text.Length > 40)
            {                
                LPopupMessage.Content = "Имя базы данных не может быть длинее 40 символов.";

                PopupMessage.PlacementTarget = TBNameDatabase;
                PopupMessage.IsOpen = true;

                return;
            }

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = ServerName;

            if (SSPI)
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.UserID = app.ConfigProgramBin.SQLUserName;
                builder.Password = app.ConfigProgramBin.SQLPassword;
            }

            string str = null;
            string newSQLDatabaseName = TBNameDatabase.Text;

            if (TBNameDatabase.Text == null || TBNameDatabase.Text.Length == 0)
            {
                LPopupMessage.Content = "Имя базы данных не может быть пустой.";

                PopupMessage.PlacementTarget = TBNameDatabase;
                PopupMessage.IsOpen = true;

                return;
            }
            else
            {
                str = "CREATE DATABASE [" + TBNameDatabase.Text + "]";               
            }

            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = builder.ConnectionString;

                try
                {
                    SqlCommand myCommand = new SqlCommand(str, cn);

                    cn.Open();

                    myCommand.ExecuteNonQuery();

                    CBDatabases.Items.Add(TBNameDatabase.Text);
                    CBDatabases.SelectedItem = TBNameDatabase.Text;

                }
                catch (SystemException ex)
                {
                    LPopupMessage.Content = ex.Message;

                    PopupMessage.PlacementTarget = TBNameDatabase;
                    PopupMessage.IsOpen = true;

                    return;
                }
            }                

            this.Close();
        }
    }
}
