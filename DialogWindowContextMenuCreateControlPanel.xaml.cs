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
    /// Логика взаимодействия для DialogWindowContextMenuCreateControlPanel.xaml
    /// </summary>
    public partial class DialogWindowContextMenuCreateControlPanel : Window
    {
        public DialogWindowContextMenuCreateControlPanel()
        {
            InitializeComponent();
        }

        private void CreateControlPanel(object sender, RoutedEventArgs e)
        {
            char[] InvalidChars = { '"', '/', '\\', '<', '>', '?', '*', '|', ':' };

            if (tbControlPanel.Text.IndexOfAny(InvalidChars) != -1)
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

            if (string.IsNullOrWhiteSpace(tbControlPanel.Text))
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

            if (!tbControlPanel.Text.EndsWith(".cp"))
            {
                tbControlPanel.Text += ".cp";
            }

            Window MainWindow = ((AppWPF)System.Windows.Application.Current).MainWindow;

            TreeViewItem parentItem = (TreeViewItem)this.Tag; // TreeViewItem куда будет помещена вложенная страница
            parentItem.IsExpanded = true;
            FolderScada parentFolder = (FolderScada)parentItem.Tag;

            ControlPanelScada cps = new ControlPanelScada();
            cps.Name = tbControlPanel.Text;
            cps.Path = parentFolder.Path + "\\" + tbControlPanel.Text;
            cps.AttachmentFolder = parentFolder.Path;
            cps.Attachments = parentFolder.Attachments + 1;
            cps.ParentItem = parentItem;

            if (File.Exists(cps.Path))
            {
                MessageBox.Show("Щит управления " + cps.Path + " уже существует.", "Ошибка создания щита управления", MessageBoxButton.OK, MessageBoxImage.Error);

                e.Handled = true;
                return;
            }

            TreeViewItem ItemControlPanel = new TreeViewItem();

            if (parentFolder.ChildItem == null) parentFolder.ChildItem = new List<TreeViewItem>();
            parentFolder.ChildItem.Add(ItemControlPanel);
          
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

            ContextMenu contextMenuPage = new ContextMenu();
            contextMenuPage.Tag = "X";
            contextMenuPage.Items.Add(menuItemCopyPage);
            contextMenuPage.Items.Add(menuItemCutPage);
            contextMenuPage.Items.Add(menuItemDeletePage);

            ItemControlPanel.Tag = cps;
            ItemControlPanel.KeyDown += ((MainWindow)MainWindow).RenameControlPanel;
            ItemControlPanel.MouseDoubleClick += ((MainWindow)MainWindow).OpenBrowseControlPanel;
            ItemControlPanel.ContextMenu = contextMenuPage;

            cps.TreeItem = ItemControlPanel;

            System.Windows.Controls.TextBox tbRenameControlPanel = new System.Windows.Controls.TextBox();
            tbRenameControlPanel.KeyDown += ((MainWindow)MainWindow).OkRenameControlPanel;
            tbRenameControlPanel.Text = cps.Name;

            System.Windows.Controls.Label lNameControlPanel = new System.Windows.Controls.Label();
            lNameControlPanel.Content = cps.Name;
            lNameControlPanel.Tag = tbRenameControlPanel;

            AlphanumComparator a = new AlphanumComparator();
            a.Name = (string)lNameControlPanel.Content;

            StackPanel panel = new StackPanel();
            panel.Orientation = System.Windows.Controls.Orientation.Horizontal;
            panel.Tag = a;

            panel.Children.Add(imageScada);
            panel.Children.Add(lNameControlPanel);

            ItemControlPanel.Header = panel;
            parentItem.Items.Add(ItemControlPanel);

            ControlPanel cp = new ControlPanel();

            parentItem.Items.SortDescriptions.Clear();
            parentItem.Items.SortDescriptions.Add(new SortDescription("ContextMenu.Tag", ListSortDirection.Ascending));
            parentItem.Items.SortDescriptions.Add(new SortDescription("Header.Tag", ListSortDirection.Ascending));

            ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionControlPanelScada.Add(cps.Path, cps);
            ((AppWPF)System.Windows.Application.Current).CollectionControlPanel.Add(cps.Path, cp);

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
