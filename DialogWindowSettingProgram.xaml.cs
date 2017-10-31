using System;
using System.Collections.Generic;
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
using Microsoft.Win32;

namespace SCADA
{
    /// <summary>
    /// Логика взаимодействия для DialogWindowSettingProject.xaml
    /// </summary>
    public partial class DialogWindowSettingProgram : Window
    {
        GridOptionsGeneralProgramm OptionsGeneralProgramm;
        GridOptionsDatabaseProgramm OptionsDatabaseProgramm;

        bool OldCreateFolder;
        string OldPathCreateProject;
        bool OldSQLList;
        bool OldSQLSecuritySSPI;
        bool OldUseDatabase;
        string OldSQLServerName;
        string OldSQLDatabaseName;
        string OldSQLPassword;
        string OldSQLUserName;

        public DialogWindowSettingProgram()
        {
            AppWPF application = ((AppWPF)Application.Current);

            InitializeComponent();

            OptionsGeneralProgramm = new GridOptionsGeneralProgramm();
            OptionsDatabaseProgramm = new GridOptionsDatabaseProgramm();

            OldCreateFolder = application.ConfigProgramBin.CreateFolder;
            OldPathCreateProject = application.ConfigProgramBin.PathBrowseProject;

            OldSQLSecuritySSPI = application.ConfigProgramBin.SQLSecuritySSPI;
            OldUseDatabase = application.ConfigProgramBin.UseDatabase;
            OldSQLServerName = application.ConfigProgramBin.SQLServerName;
            OldSQLDatabaseName = application.ConfigProgramBin.SQLDatabaseName;
            OldSQLPassword = application.ConfigProgramBin.SQLPassword;
            OldSQLUserName = application.ConfigProgramBin.SQLUserName;
        }

        private void TreeViewProperties_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {           
            TreeViewItem Selected = (TreeViewItem)e.NewValue;
            if ((string)Selected.Header == "Общие")
            {               
                OptionsGeneralProgramm.SetValue(Grid.ColumnProperty, 1);

                if (PropertiesGrid.Children.Contains(OptionsDatabaseProgramm))
                {
                    PropertiesGrid.Children.Remove(OptionsDatabaseProgramm);
                }
               
                PropertiesGrid.Children.Add(OptionsGeneralProgramm);
            }
            else if ((string)Selected.Header == "База данных")
            {
                OptionsDatabaseProgramm.SetValue(Grid.ColumnProperty, 1);

                if (PropertiesGrid.Children.Contains(OptionsGeneralProgramm))
                {
                    PropertiesGrid.Children.Remove(OptionsGeneralProgramm);
                }

                PropertiesGrid.Children.Add(OptionsDatabaseProgramm);
            }
        }
     
        void WindowLoaded(object sender, RoutedEventArgs e)
        {               
            TreeViewItemGeneral.IsSelected = true;
            e.Handled = true;
        }
      
        private void Close(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            this.Close();
        }

        private void Apply(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            AppWPF application = ((AppWPF)Application.Current);
           
            if (OptionsDatabaseProgramm.TBDatabaseName.Text.Length > 40)
            {
                if (TreeViewProperties.SelectedItem != TreeViewItemDatabase)
                {
                    TreeViewItemDatabase.IsSelected = true;
                }

                OptionsDatabaseProgramm.LPopupMessage.Content = "Имя базы данных не может быть длинее 40 символов.";

                OptionsDatabaseProgramm.PopupMessage.PlacementTarget = OptionsDatabaseProgramm.TBDatabaseName;
                OptionsDatabaseProgramm.PopupMessage.IsOpen = true;
                
                return;                   
            }
            else if (OptionsDatabaseProgramm.TBServerName.Text.Length > 40)
            {
                if (TreeViewProperties.SelectedItem != TreeViewItemDatabase)
                {
                    TreeViewItemDatabase.IsSelected = true;
                }

                OptionsDatabaseProgramm.LPopupMessage.Content = "Имя сервера не может быть длинее 40 символов.";

                OptionsDatabaseProgramm.PopupMessage.PlacementTarget = OptionsDatabaseProgramm.TBServerName;
                OptionsDatabaseProgramm.PopupMessage.IsOpen = true;

                return;  
            }
            
            if (!(bool)OptionsDatabaseProgramm.RBSecuritySSPI.IsChecked)
            {
                if (OptionsDatabaseProgramm.TBUserName.Text.Length > 40)
                {
                    if (TreeViewProperties.SelectedItem != TreeViewItemDatabase)
                    {
                        TreeViewItemDatabase.IsSelected = true;
                    }

                    OptionsDatabaseProgramm.LPopupMessage.Content = "Имя пользователя не может быть длинее 40 символов.";

                    OptionsDatabaseProgramm.PopupMessage.PlacementTarget = OptionsDatabaseProgramm.TBUserName;
                    OptionsDatabaseProgramm.PopupMessage.IsOpen = true;

                    return;
                }
                else if (OptionsDatabaseProgramm.TBUserPassword.Text.Length > 40)
                {
                    if (TreeViewProperties.SelectedItem != TreeViewItemDatabase)
                    {
                        TreeViewItemDatabase.IsSelected = true;
                    }

                    OptionsDatabaseProgramm.LPopupMessage.Content = "Пароль не может быть длинее 40 символов.";

                    OptionsDatabaseProgramm.PopupMessage.PlacementTarget = OptionsDatabaseProgramm.TBUserPassword;
                    OptionsDatabaseProgramm.PopupMessage.IsOpen = true;

                    return;
                }
            }

            if (OldUseDatabase != (bool)OptionsDatabaseProgramm.chbUseDatabase.IsChecked)
            {
                application.ConfigProgramBin.UseDatabase = (bool)OptionsDatabaseProgramm.chbUseDatabase.IsChecked;
            }

            if (OldCreateFolder != OptionsGeneralProgramm.CheckBoxCreateFolder.IsChecked || OldPathCreateProject != OptionsGeneralProgramm.TextBoxBrowseProject.Text)
            {
                application.ConfigProgramBin.PathBrowseProject = OptionsGeneralProgramm.TextBoxBrowseProject.Text;
                application.ConfigProgramBin.CreateFolder = (bool)OptionsGeneralProgramm.CheckBoxCreateFolder.IsChecked;
            }
            
            if (OldSQLServerName != OptionsDatabaseProgramm.TBServerName.Text || OldSQLDatabaseName != OptionsDatabaseProgramm.TBDatabaseName.Text)
            {
                application.ConfigProgramBin.SQLServerName = OptionsDatabaseProgramm.TBServerName.Text;

                application.ConfigProgramBin.SQLDatabaseName = OptionsDatabaseProgramm.TBDatabaseName.Text;
            }            

            if(OldSQLSecuritySSPI != OptionsDatabaseProgramm.RBSecuritySSPI.IsChecked)
            {
                application.ConfigProgramBin.SQLSecuritySSPI = (bool)OptionsDatabaseProgramm.RBSecuritySSPI.IsChecked;                
            }

            if (OptionsDatabaseProgramm.RBSecuritySSPI.IsChecked == false)
            {
                if (OldSQLUserName != OptionsDatabaseProgramm.TBUserName.Text || OldSQLPassword != OptionsDatabaseProgramm.TBUserPassword.Text)
                {
                    application.ConfigProgramBin.SQLUserName = OptionsDatabaseProgramm.TBUserName.Text;

                    application.ConfigProgramBin.SQLPassword = OptionsDatabaseProgramm.TBUserPassword.Text;
                }
            }

            this.Close();
        }        
    }
}
