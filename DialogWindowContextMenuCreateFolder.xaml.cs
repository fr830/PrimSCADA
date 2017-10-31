// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;

namespace SCADA
{
    /// <summary>
    /// Логика взаимодействия для DialogWindowCreateFile.xaml
    /// </summary>
    public partial class DialogWindowContextMenuCreateFolder : Window
    {
        public DialogWindowContextMenuCreateFolder()
        {
            InitializeComponent();
        }

        private void CreateFolder(object sender, RoutedEventArgs e)
        {
            char[] InvalidChars = { '"', '/', '\\', '<', '>', '?', '*', '|', ':' };

            if (tbNameFolder.Text.IndexOfAny(InvalidChars) != -1)
            {
                Border border = new Border();
                border.BorderThickness = new Thickness(2);
                border.Background = new SolidColorBrush(Colors.White);
                border.BorderBrush = new SolidColorBrush(Colors.Red);

                TextBlock textBlock = new TextBlock();
                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.Text = "Имя папки не должно содержать символы: < > | \" / \\ * : ?";
                border.Child = textBlock;

                Message.Child = border;
                Message.IsOpen = true;

                e.Handled = true;
                return;

            }

            if (string.IsNullOrWhiteSpace(tbNameFolder.Text))
            {
                Border border = new Border();
                border.BorderThickness = new Thickness(2);
                border.Background = new SolidColorBrush(Colors.White);
                border.BorderBrush = new SolidColorBrush(Colors.Red);

                TextBlock textBlock = new TextBlock();
                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.Text = "Имя папки не должно содержать только пробелы или быть пустой строкой.";
                border.Child = textBlock;

                Message.Child = border;
                Message.IsOpen = true;

                e.Handled = true;
                return;
            }

            TreeViewItem parentItem = (TreeViewItem)this.Tag; // TreeViewItem куда будет помещена вложенная папка
            parentItem.IsExpanded = true;

            Window MainWindow = ((AppWPF)System.Windows.Application.Current).MainWindow;

            Dictionary<string, FolderScada> CollectionFolders = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionFolderScada;

            FolderScada fs = new FolderScada();
            fs.Name = tbNameFolder.Text;
            fs.Path = ((FolderScada)parentItem.Tag).Path + "\\" + tbNameFolder.Text;
            fs.AttachmentFolder = ((FolderScada)parentItem.Tag).Path;
            fs.Attachments = ((FolderScada)parentItem.Tag).Attachments + 1;
            fs.ParentItem = parentItem;

            //Проверка существования папки
            if (Directory.Exists(fs.Path))
            {
                MessageBox.Show("Папка " + fs.Path + " уже существует.", "Ошибка создания папки", MessageBoxButton.OK, MessageBoxImage.Error);

                e.Handled = true;
                return;
            }

            TreeViewItem ItemFolder = new TreeViewItem();
           
            FolderScada parentFolder = (FolderScada)parentItem.Tag;
            if (parentFolder.ChildItem == null) parentFolder.ChildItem = new List<TreeViewItem>();
            parentFolder.ChildItem.Add(ItemFolder);
         
            CollectionFolders.Add(fs.Path, fs);

            Directory.CreateDirectory(fs.Path);
                           
            Image imageFolder = new Image();
            imageFolder.Source = new BitmapImage(new Uri("Images/CloseFolder16.png", UriKind.Relative));

            Image imageMenuItemCreateFolder = new Image();
            imageMenuItemCreateFolder.Source = new BitmapImage(new Uri("Images/NewFolder16.png", UriKind.Relative));

            Image imageMenuItemCreatePage = new Image();
            imageMenuItemCreatePage.Source = new BitmapImage(new Uri("Images/NewPage16.png", UriKind.Relative));

            Image imageCut = new Image();
            imageCut.Source = new BitmapImage(new Uri("Images/Cut16.png", UriKind.Relative));

            Image imageInsert = new Image();
            imageInsert.Source = new BitmapImage(new Uri("Images/Insert16.ico", UriKind.Relative));

            Image ImageDelete = new Image();
            ImageDelete.Source = new BitmapImage(new Uri("Images/FolderDelete16.png", UriKind.Relative));

            Image ImageCopy = new Image();
            ImageCopy.Source = new BitmapImage(new Uri("Images/CopyFolder16.ico", UriKind.Relative));

            Image imageControlPanel = new Image();
            imageControlPanel.Source = new BitmapImage(new Uri("Images/ControlPanel16.png", UriKind.Relative));

            fs.TreeItem = ItemFolder;

            MenuItem MenuItemCreate = new MenuItem();
            MenuItemCreate.Header = "Добавить";

            MenuItem MenuItemCreateControlPanel = new MenuItem();
            MenuItemCreateControlPanel.Click += ((MainWindow)MainWindow).ContextMenuCreateControlPanel;
            MenuItemCreateControlPanel.Icon = imageControlPanel;
            MenuItemCreateControlPanel.Header = "Щит управления";
            MenuItemCreateControlPanel.Tag = ItemFolder;

            MenuItem MenuItemCreateFolder = new MenuItem();
            MenuItemCreateFolder.Icon = imageMenuItemCreateFolder;
            MenuItemCreateFolder.Header = "Папку";
            MenuItemCreateFolder.Tag = ItemFolder; // Нужен для индефикации в какую папку сохранять при создании вложенной папки
            MenuItemCreateFolder.Click += ((MainWindow)MainWindow).ContextMenuCreateFolder;

            MenuItem MenuItemCreatePage = new MenuItem();
            MenuItemCreatePage.Icon = imageMenuItemCreatePage;
            MenuItemCreatePage.Header = "Страницу";
            MenuItemCreatePage.Tag = ItemFolder;  // Нужен для индефикации в какую папку сохранять при создании вложенной страницы
            MenuItemCreatePage.Click += ((MainWindow)MainWindow).ContextMenuCreatePage;

            MenuItem menuItemCopyFolder = new MenuItem();
            menuItemCopyFolder.IsEnabled = false;
            menuItemCopyFolder.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            menuItemCopyFolder.Header = "Копировать";
            menuItemCopyFolder.Icon = ImageCopy;
            menuItemCopyFolder.Tag = fs;
            menuItemCopyFolder.Click += ((MainWindow)MainWindow).CopyItem;

            MenuItem menuItemCutFolder = new MenuItem();
            menuItemCutFolder.IsEnabled = false;
            menuItemCutFolder.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            menuItemCutFolder.Header = "Вырезать";
            menuItemCutFolder.Icon = imageCut;
            menuItemCutFolder.Tag = fs;
            menuItemCutFolder.Click += ((MainWindow)MainWindow).CutItem;

            MenuItem menuItemDeleteFolder = new MenuItem();
            menuItemDeleteFolder.Header = "Удалить";
            menuItemDeleteFolder.Icon = ImageDelete;
            menuItemDeleteFolder.Tag = fs;
            menuItemDeleteFolder.Click += ((MainWindow)MainWindow).DeleteItem;

            Binding BindingInsert = new Binding();
            BindingInsert.Source = (MainWindow)MainWindow;
            BindingInsert.Path = new PropertyPath("IsBindingInsert");
            BindingInsert.Mode = BindingMode.OneWay;

            MenuItem menuItemInsert = new MenuItem();
            menuItemInsert.Header = "Вставить";
            menuItemInsert.Tag = fs;
            menuItemInsert.SetBinding(MenuItem.IsEnabledProperty, BindingInsert);
            menuItemInsert.Icon = imageInsert;
            menuItemInsert.Click += ((MainWindow)MainWindow).InsertItem;

            MenuItemCreate.Items.Add(MenuItemCreateFolder);
            MenuItemCreate.Items.Add(MenuItemCreatePage);
            MenuItemCreate.Items.Add(MenuItemCreateControlPanel);

            ContextMenu ContextMenuFolder = new ContextMenu();
            ContextMenuFolder.Tag = "FolderScada";
            ContextMenuFolder.Items.Add(MenuItemCreate);
            ContextMenuFolder.Items.Add(menuItemCopyFolder);
            ContextMenuFolder.Items.Add(menuItemCutFolder);
            ContextMenuFolder.Items.Add(menuItemInsert);
            ContextMenuFolder.Items.Add(menuItemDeleteFolder);

            ItemFolder.ContextMenu = ContextMenuFolder;

            System.Windows.Controls.TextBox tbRenameFolder = new System.Windows.Controls.TextBox();
            tbRenameFolder.KeyDown += ((MainWindow)MainWindow).OkRenameFolder;
            tbRenameFolder.Text = fs.Name;
            System.Windows.Controls.Label lNameFolder = new System.Windows.Controls.Label();
            lNameFolder.Content = fs.Name;
            lNameFolder.Tag = tbRenameFolder;

            AlphanumComparator a = new AlphanumComparator();
            a.Name = (string)lNameFolder.Content;

            StackPanel panel = new StackPanel();
            panel.Orientation = System.Windows.Controls.Orientation.Horizontal;
            panel.Tag = a;

            panel.Children.Add(imageFolder);
            panel.Children.Add(lNameFolder);

            ItemFolder.Header = panel;
            ItemFolder.Collapsed += ((MainWindow)MainWindow).Collapsed;
            ItemFolder.Expanded += ((MainWindow)MainWindow).Expanded;
            ItemFolder.Tag = fs;
            ItemFolder.KeyDown += ((MainWindow)MainWindow).RenameFolder;

            parentItem.Items.Add(ItemFolder);

            parentItem.Items.SortDescriptions.Clear();
            parentItem.Items.SortDescriptions.Add(new SortDescription("ContextMenu.Tag", ListSortDirection.Ascending));
            parentItem.Items.SortDescriptions.Add(new SortDescription("Header.Tag", ListSortDirection.Ascending));
            
            e.Handled = true;
            this.Close();
        }
    }
}
