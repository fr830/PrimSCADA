// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;
using System.Windows.Markup;

namespace SCADA
{
    /// <summary>
    /// Логика взаимодействия для DialogWindowCreateFile.xaml
    /// </summary>
    public partial class DialogWindowContextMenuCreatePage : Window
    {
        public DialogWindowContextMenuCreatePage()
        {
            InitializeComponent();
        }

        private void CreatePage(object sender, RoutedEventArgs e)
        {
            char[] InvalidChars = { '"', '/', '\\', '<', '>', '?', '*', '|', ':' };

            if (tbNamePage.Text.IndexOfAny(InvalidChars) != -1)
            {
                Border border = new Border();
                border.BorderThickness = new Thickness(2);
                border.Background = new SolidColorBrush(Colors.White);
                border.BorderBrush = new SolidColorBrush(Colors.Red);

                TextBlock textBlock = new TextBlock();
                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.Text = "Имя странцы не должно содержать символы: < > | \" / \\ * : ?";

                border.Child = textBlock;

                Message.Child = border;
                Message.IsOpen = true;

                e.Handled = true;
                return;

            }

            if (string.IsNullOrWhiteSpace(tbNamePage.Text))
            {
                Border border = new Border();
                border.BorderThickness = new Thickness(2);
                border.Background = new SolidColorBrush(Colors.White);
                border.BorderBrush = new SolidColorBrush(Colors.Red);

                TextBlock textBlock = new TextBlock();
                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.Text = "Имя страницы не должно содержать только пробелы или быть пустой строкой.";
                border.Child = textBlock;

                Message.Child = border;
                Message.IsOpen = true;

                e.Handled = true;
                return;
            }

            if (!tbNamePage.Text.EndsWith(".pg"))
            {
                tbNamePage.Text += ".pg";
            }
                       
            Window MainWindow = ((AppWPF)System.Windows.Application.Current).MainWindow;

            TreeViewItem parentItem = (TreeViewItem)this.Tag; // TreeViewItem куда будет помещена вложенная страница
            parentItem.IsExpanded = true;
            FolderScada parentFolder = (FolderScada)parentItem.Tag;

            PageScada ps = new PageScada();
            ps.Name = tbNamePage.Text;
            ps.Path = parentFolder.Path + "\\" + tbNamePage.Text;
            ps.AttachmentFolder = parentFolder.Path;
            ps.Attachments = parentFolder.Attachments + 1;
            ps.ParentItem = parentItem;

            if (File.Exists(ps.Path))
            {
                MessageBox.Show("Страница " + ps.Path + " уже существует.", "Ошибка создания странцы", MessageBoxButton.OK, MessageBoxImage.Error);

                e.Handled = true;
                return;
            }

            TreeViewItem ItemPage = new TreeViewItem();

            if (parentFolder.ChildItem == null) parentFolder.ChildItem = new List<TreeViewItem>();
            parentFolder.ChildItem.Add(ItemPage);
                              
            Image imageScada = new Image();
            imageScada.Source = new BitmapImage(new Uri("Images/Page16.png", UriKind.Relative));

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
            menuItemCopyPage.Tag = ps;
            menuItemCopyPage.Click += ((MainWindow)MainWindow).CopyItem;

            MenuItem menuItemCutPage = new MenuItem();
            menuItemCutPage.IsEnabled = false;
            menuItemCutPage.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            menuItemCutPage.Header = "Вырезать";
            menuItemCutPage.Icon = imageCut;
            menuItemCutPage.Tag = ps;
            menuItemCutPage.Click += ((MainWindow)MainWindow).CutItem;

            MenuItem menuItemDeletePage = new MenuItem();
            menuItemDeletePage.Header = "Удалить";
            menuItemDeletePage.Icon = imageDelete;
            menuItemDeletePage.Tag = ps;
            menuItemDeletePage.Click += ((MainWindow)MainWindow).DeleteItem;

            ContextMenu contextMenuPage = new ContextMenu();
            contextMenuPage.Tag = "PageScada";
            contextMenuPage.Items.Add(menuItemCopyPage);
            contextMenuPage.Items.Add(menuItemCutPage);
            contextMenuPage.Items.Add(menuItemDeletePage);
           
            ItemPage.Tag = ps;
            ItemPage.MouseDoubleClick += ((MainWindow)MainWindow).OpenBrowsePage;
            ItemPage.KeyDown += ((MainWindow)MainWindow).RenamePage;
            ItemPage.ContextMenu = contextMenuPage;

            ps.TreeItem = ItemPage;

            System.Windows.Controls.TextBox tbRenamePage = new System.Windows.Controls .TextBox();
            tbRenamePage.KeyDown += ((MainWindow)MainWindow).OkRenamePage;
            tbRenamePage.Text = ps.Name;

            System.Windows.Controls.Label lNamePage = new System.Windows.Controls.Label();
            lNamePage.Content = ps.Name;
            lNamePage.Tag = tbRenamePage;

            AlphanumComparator a = new AlphanumComparator();
            a.Name = (string)lNamePage.Content;

            StackPanel panel = new StackPanel();
            panel.Orientation = System.Windows.Controls.Orientation.Horizontal;
            panel.Tag = a;

            panel.Children.Add(imageScada);
            panel.Children.Add(lNamePage);

            ItemPage.Header = panel;
            parentItem.Items.Add(ItemPage);

            Page pg = new Page();

            parentItem.Items.SortDescriptions.Clear();
            parentItem.Items.SortDescriptions.Add(new SortDescription("ContextMenu.Tag", ListSortDirection.Ascending));
            parentItem.Items.SortDescriptions.Add(new SortDescription("Header.Tag", ListSortDirection.Ascending));

            ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionPageScada.Add(ps.Path, ps);
            ((AppWPF)System.Windows.Application.Current).CollectionPage.Add(ps.Path, pg);

            TabItemPage tabItemPage = new TabItemPage(ps);

            using (FileStream fs = File.Create((ps.Path)))
            {               
                XamlWriter.Save(pg, fs);

                fs.Close();
            }
            
            e.Handled = true;
            this.Close();
        }
    }
}
