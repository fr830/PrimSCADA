// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
    /// Логика взаимодействия для DialogWindowCreateProject.xaml
    /// </summary>
    public partial class DialogWindowCreateProject : Window
    {
        public DialogWindowCreateProject()
        {
            InitializeComponent();

            TextBoxProjectName.Focus();
        }

        private void CreateButton(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBoxProjectName.Text))
            {
                Message.IsOpen = true;
                e.Handled = true;
                return;
            }
         
            SaveFileDialog BrowseDialog = new SaveFileDialog();
            BrowseDialog.FileName = TextBoxProjectName.Text;
            BrowseDialog.OverwritePrompt = true;        
            BrowseDialog.InitialDirectory = ((AppWPF)Application.Current).ConfigProgramBin.PathBrowseProject;
            BrowseDialog.Filter = "SCADA проект|*.proj";

            if (BrowseDialog.ShowDialog() == true)
            {
                if (((AppWPF)Application.Current).ConfigProgramBin.CreateFolder) // если есть отметка, создаем папку под проект
                {
                    // Сохраняем старый проект
                    if (((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin != null)
                    {
                        using (FileStream fs = File.Create(((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject))
                        {
                            BinaryFormatter Projserializer = new BinaryFormatter();
                            Projserializer.Serialize(fs, ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin);
                        }
                    }

                    // Находим индекс имени проекта в полном пути и удаляем расширение проекта чтобы получить путь нового проекта
                    int index2 = BrowseDialog.FileName.LastIndexOf(".proj");
                    string dirFolder = BrowseDialog.FileName.Remove(index2);

                    if (((AppWPF)Application.Current).ConfigProgramBin.CreateFolder)
                    {
                        if (Directory.Exists(dirFolder))
                        {
                            MessageBox.Show("Папка " + dirFolder + " уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);

                            e.Handled = true;
                            return;
                        }
                    }
                                 
                    Directory.CreateDirectory(dirFolder);

                    using (FileStream ProjectStream = File.Create(dirFolder + "\\" + BrowseDialog.SafeFileName))
                    {
                        BinaryFormatter serializer = new BinaryFormatter();

                        if (((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin == null)
                        {
                            ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin = new SerializationProject();                                                       
                        }
                        else
                        {
                            ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionFolderScada.Clear();
                            ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionPageScada.Clear();
                            ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionControlPanelScada.Clear();                        
                        }

                        ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.ProjectName = BrowseDialog.SafeFileName;
                        ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject = ProjectStream.Name;
                        serializer.Serialize(ProjectStream, ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin);
                        ProjectStream.Close();
                    }
                }
                else
                {
                    using (FileStream ProjectStream = File.Create(BrowseDialog.FileName))
                    {
                        BinaryFormatter serializer = new BinaryFormatter();

                        if (((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin == null)
                        {
                            ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin = new SerializationProject();                                                       
                        }
                        else
                        {
                            ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionFolderScada.Clear();
                            ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionPageScada.Clear();
                            ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionControlPanelScada.Clear();                        
                        }

                        ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.ProjectName = BrowseDialog.SafeFileName;
                        ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject = ProjectStream.Name;
                        serializer.Serialize(ProjectStream, ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin);
                        ProjectStream.Close();   
                    }
                }

                Window MainWindow = ((AppWPF)Application.Current).MainWindow;

                ((AppWPF)Application.Current).CollectionPage.Clear();
                ((AppWPF)Application.Current).CollectionControlPanel.Clear();
                ((AppWPF)Application.Current).CollectionTabItemParent.Clear();
                ((AppWPF)Application.Current).CollectionSaveTabItem.Clear();
                ((MainWindow)MainWindow).BrowseProject.Items.Clear();
                ((MainWindow)MainWindow).TabControlMain.Items.Clear();

                StackPanel panel = new StackPanel();
                panel.Orientation = Orientation.Horizontal;

                Image imageProject = new Image();
                imageProject.Source = new BitmapImage(new Uri("Images/IcoScada16.png", UriKind.Relative));

                Image imageNewPage = new Image();
                imageNewPage.Source = new BitmapImage(new Uri("Images/NewPage16.png", UriKind.Relative));

                Image ImageNewFolder = new Image();
                ImageNewFolder.Source = new BitmapImage(new Uri("Images/NewFolder16.png", UriKind.Relative));

                Image imageInsertProject = new Image();
                imageInsertProject.Source = new BitmapImage(new Uri("Images/Insert16.ico", UriKind.Relative));

                Image imageNewControlPanel = new Image();
                imageNewControlPanel.Source = new BitmapImage(new Uri("Images/ControlPanel16.png", UriKind.Relative));

                MenuItem MenuItemCreateFolder = new MenuItem();
                MenuItemCreateFolder.Click += ((MainWindow)MainWindow).CreateFolder;
                MenuItemCreateFolder.Icon = ImageNewFolder;
                MenuItemCreateFolder.Header = "Создать папку";

                MenuItem MenuItemCreateControlPanelProject = new MenuItem();
                MenuItemCreateControlPanelProject.Click += ((MainWindow)MainWindow).CreateControlPanel;
                MenuItemCreateControlPanelProject.Icon = imageNewControlPanel;
                MenuItemCreateControlPanelProject.Header = "Создать щит управления";

                MenuItem MenuItemCreatePage = new MenuItem();
                MenuItemCreatePage.Click += ((MainWindow)MainWindow).CreatePage;
                MenuItemCreatePage.Icon = imageNewPage;
                MenuItemCreatePage.Header = "Создать страницу";

                Binding BindingInsertContextProject = new Binding();
                BindingInsertContextProject.Source = MainWindow;
                BindingInsertContextProject.Path = new PropertyPath("IsBindingInsert");
                BindingInsertContextProject.Mode = BindingMode.OneWay;

                MenuItem MenuItemInsertProject = new MenuItem();
                MenuItemInsertProject.Header = "Вставить";
                MenuItemInsertProject.SetBinding(MenuItem.IsEnabledProperty, BindingInsertContextProject);
                MenuItemInsertProject.Icon = imageInsertProject;
                MenuItemInsertProject.Click += ((MainWindow)MainWindow).InsertItem;

                ContextMenu ContextMenuProject = new ContextMenu();
                ContextMenuProject.Items.Add(MenuItemCreateFolder);
                ContextMenuProject.Items.Add(MenuItemCreatePage);
                ContextMenuProject.Items.Add(MenuItemCreateControlPanelProject);
                ContextMenuProject.Items.Add(MenuItemInsertProject);

                TreeViewItem ItemNameProject = new TreeViewItem();
                ItemNameProject.Tag = "1";
                ItemNameProject.ContextMenu = ContextMenuProject;
                ItemNameProject.KeyDown += ((MainWindow)MainWindow).RenameProject;

                Label lNameProject = new Label();
                lNameProject.Content = BrowseDialog.SafeFileName;

                panel.Children.Add(imageProject);
                panel.Children.Add(lNameProject);

                ItemNameProject.Header = panel;

                TextBox tbNameProject = new TextBox();
                tbNameProject.KeyDown += ((MainWindow)MainWindow).LostRename;
                tbNameProject.Text = BrowseDialog.SafeFileName;

                lNameProject.Tag = tbNameProject;
               
                ((MainWindow)MainWindow).BrowseProject.Items.Add(ItemNameProject);

                MainWindow.Title = "SCADA " + BrowseDialog.SafeFileName;

                e.Handled = true;
                this.Close();
            }
        }
    }
}
  