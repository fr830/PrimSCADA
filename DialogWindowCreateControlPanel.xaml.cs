// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SCADA
{
    /// <summary>
    /// Логика взаимодействия для DialogWindowCreateControlPanel.xaml
    /// </summary>
    public partial class DialogWindowCreateControlPanel : Window
    {
        public DialogWindowCreateControlPanel()
        {
            InitializeComponent();
        }

        private void CreateControlPanel(object sender, RoutedEventArgs e)
        {
            char[] InvalidChars = { '"', '/', '\\', '<', '>', '?', '*', '|', ':' };

            if (NameControlPanel.Text.IndexOfAny(InvalidChars) != -1)
            {
                Border border = new Border();
                border.BorderThickness = new Thickness(2);
                border.Background = new SolidColorBrush(Colors.White);
                border.BorderBrush = new SolidColorBrush(Colors.Red);

                TextBlock textBlock = new TextBlock();
                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.Text = "Имя щита управления не должно содержать символы: < > | \" / \\ * : ?";

                border.Child = textBlock;

                Message.Child = border;
                Message.IsOpen = true;

                e.Handled = true;
                return;

            }

            if (string.IsNullOrWhiteSpace(NameControlPanel.Text))
            {
                Border border = new Border();
                border.BorderThickness = new Thickness(2);
                border.Background = new SolidColorBrush(Colors.White);
                border.BorderBrush = new SolidColorBrush(Colors.Red);

                TextBlock textBlock = new TextBlock();
                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.Text = "Имя щита управления не должно содержать только пробелы или быть пустой строкой.";
                border.Child = textBlock;

                Message.Child = border;
                Message.IsOpen = true;

                e.Handled = true;
                return;
            }

            if (!NameControlPanel.Text.EndsWith(".cp"))
            {
                NameControlPanel.Text += ".cp";
            }

            //Находим индекс имени проекта, для удаления и получения пути проекта
            int index = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject.LastIndexOf(((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.ProjectName);

            Window MainWindow = ((AppWPF)System.Windows.Application.Current).MainWindow;

            ControlPanelScada cps = new ControlPanelScada();
            cps.Name = NameControlPanel.Text;
            cps.Path = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject.Remove(index) + "\\" + NameControlPanel.Text;
            cps.Attachments = 0;

            if (File.Exists(cps.Path))
            {
                MessageBox.Show("Щит управления " + cps.Path + " уже существует.", "Ошибка создания щита управления", MessageBoxButton.OK, MessageBoxImage.Error);

                e.Handled = true;
                return;
            }
           
            StackPanel panel = new StackPanel();
            panel.Orientation = System.Windows.Controls.Orientation.Horizontal;

            Image imageScada = new Image();
            imageScada.Source = new BitmapImage(new Uri("Images/ControlPanel16.png", UriKind.Relative));

            Image imageCut = new Image();
            imageCut.Source = new BitmapImage(new Uri("Images/Cut16.png", UriKind.Relative));

            Image imageDelete = new Image();
            imageDelete.Source = new BitmapImage(new Uri("Images/PageDelete16.png", UriKind.Relative));

            Image imageCopy = new Image();
            imageCopy.Source = new BitmapImage(new Uri("Images/CopyPage16.png", UriKind.Relative));

            MenuItem menuItemCopyPage = new MenuItem();
            menuItemCopyPage.IsEnabled = false;
            menuItemCopyPage.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            menuItemCopyPage.Header = "Копировать";
            menuItemCopyPage.Icon = imageCopy;
            menuItemCopyPage.Tag = cps;
            menuItemCopyPage.Click += ((MainWindow)MainWindow).CopyItem;

            MenuItem menuItemCutPage = new MenuItem();
            menuItemCutPage.IsEnabled = false;
            menuItemCutPage.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            menuItemCutPage.Header = "Вырезать";
            menuItemCutPage.Icon = imageCut;
            menuItemCutPage.Tag = cps;
            menuItemCutPage.Click += ((MainWindow)MainWindow).CutItem;

            MenuItem menuItemDeletePage = new MenuItem();
            menuItemDeletePage.Header = "Удалить";
            menuItemDeletePage.Icon = imageDelete;
            menuItemDeletePage.Tag = cps;
            menuItemDeletePage.Click += ((MainWindow)MainWindow).DeleteItem;

            ContextMenu contextMenuControlPanel = new ContextMenu();
            contextMenuControlPanel.Tag = "X";
            contextMenuControlPanel.Items.Add(menuItemCopyPage);
            contextMenuControlPanel.Items.Add(menuItemCutPage);
            contextMenuControlPanel.Items.Add(menuItemDeletePage);

            TreeViewItem ItemControlPanel = new TreeViewItem();
            ItemControlPanel.Tag = cps;
            ItemControlPanel.KeyDown += ((MainWindow)MainWindow).RenameControlPanel;
            ItemControlPanel.MouseDoubleClick += ((MainWindow)MainWindow).OpenBrowseControlPanel;
            ItemControlPanel.ContextMenu = contextMenuControlPanel;

            cps.TreeItem = ItemControlPanel;

            System.Windows.Controls.TextBox tbRenameControlPanel = new System.Windows.Controls.TextBox();
            tbRenameControlPanel.KeyDown += ((MainWindow)MainWindow).OkRenameControlPanel;
            tbRenameControlPanel.Text = cps.Name;

            System.Windows.Controls.Label lNameControlPanel = new System.Windows.Controls.Label();
            lNameControlPanel.Content = cps.Name;
            lNameControlPanel.Tag = tbRenameControlPanel;

            AlphanumComparator a = new AlphanumComparator();
            a.Name = (string)lNameControlPanel.Content;

            panel.Children.Add(imageScada);
            panel.Children.Add(lNameControlPanel);
            panel.Tag = a;

            ItemControlPanel.Header = panel;
            ((MainWindow)MainWindow).BrowseProject.Items.Add(ItemControlPanel);

            ControlPanel cp = new ControlPanel();

            ((MainWindow)MainWindow).BrowseProject.Items.SortDescriptions.Clear();
            ((MainWindow)MainWindow).BrowseProject.Items.SortDescriptions.Add(new SortDescription("ContextMenu.Tag", ListSortDirection.Ascending));
            ((MainWindow)MainWindow).BrowseProject.Items.SortDescriptions.Add(new SortDescription("Header.Tag", ListSortDirection.Ascending));

            ((AppWPF)System.Windows.Application.Current).CollectionControlPanel.Add(cps.Path, cp);
            ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionControlPanelScada.Add(cps.Path, cps);

            TabItemControlPanel tabItemPage = new TabItemControlPanel(cps);

            using (FileStream fs = File.Create((cps.Path)))
            {               
                XamlWriter.Save(cp, fs);
                fs.Close();
            }

            e.Handled = true;
            this.Close();
        }
    }
}
