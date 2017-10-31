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
    public partial class DialogWindowCreateFile : Window
    {
        public DialogWindowCreateFile()
        {
            InitializeComponent();
        }

        private void CreatePage(object sender, RoutedEventArgs e)
        {
            char[] InvalidChars = { '"', '/', '\\', '<', '>', '?', '*', '|', ':' };

            if (NamePage.Text.IndexOfAny(InvalidChars) != -1)
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

            if (string.IsNullOrWhiteSpace(NamePage.Text))
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

            if (!NamePage.Text.EndsWith(".pg"))
            {
                NamePage.Text += ".pg";
            }

            //Находим индекс имени проекта, для удаления и получения пути проекта
            int index = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject.LastIndexOf(((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.ProjectName);

            Window MainWindow = ((AppWPF)System.Windows.Application.Current).MainWindow;

            PageScada ps = new PageScada();
            ps.Name = NamePage.Text;
            ps.Path = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject.Remove(index) + "\\" + NamePage.Text;
            ps.Attachments = 0;

            if (File.Exists(ps.Path))
            {
                MessageBox.Show("Страница " + ps.Path + " уже существует.", "Ошибка создания страницы", MessageBoxButton.OK, MessageBoxImage.Error);

                e.Handled = true;
                return;
            }
           
            StackPanel panelPage = new StackPanel();
            panelPage.Orientation = System.Windows.Controls.Orientation.Horizontal;

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

            TreeViewItem ItemPage = new TreeViewItem();
            ItemPage.MouseDoubleClick += ((MainWindow)MainWindow).OpenBrowsePage;
            ItemPage.Tag = ps;
            ItemPage.KeyDown += ((MainWindow)MainWindow).RenamePage;
            ItemPage.ContextMenu = contextMenuPage;

            ps.TreeItem = ItemPage;

            System.Windows.Controls.TextBox tbRenamePage = new System.Windows.Controls.TextBox();
            tbRenamePage.KeyDown += ((MainWindow)MainWindow).OkRenamePage;
            tbRenamePage.Text = ps.Name;

            System.Windows.Controls.Label lNamePage = new System.Windows.Controls.Label();
            lNamePage.Content = ps.Name;
            lNamePage.Tag = tbRenamePage;

            AlphanumComparator a = new AlphanumComparator();
            a.Name = (string)lNamePage.Content;

            panelPage.Children.Add(imageScada);
            panelPage.Children.Add(lNamePage);
            panelPage.Tag = a;

            ItemPage.Header = panelPage;
            ((MainWindow)MainWindow).BrowseProject.Items.Add(ItemPage);

            Page pg = new Page();

            ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionPageScada.Add(ps.Path, ps);
            ((AppWPF)System.Windows.Application.Current).CollectionPage.Add(ps.Path, pg);

            TabItemPage tabItemPage = new TabItemPage(ps);
            
            using (FileStream fs = File.Create((ps.Path)))
            {
                XamlWriter.Save(pg, fs);               
                fs.Close();
            }

            ((MainWindow)MainWindow).BrowseProject.Items.SortDescriptions.Clear();
            ((MainWindow)MainWindow).BrowseProject.Items.SortDescriptions.Add(new SortDescription("ContextMenu.Tag", ListSortDirection.Ascending));
            ((MainWindow)MainWindow).BrowseProject.Items.SortDescriptions.Add(new SortDescription("Header.Tag", ListSortDirection.Ascending));
                         
            e.Handled = true;
            this.Close();
        }
    }
}
