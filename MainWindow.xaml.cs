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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Win32;
using System.Windows.Controls.Primitives;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Collections;
using System.ComponentModel;
using System.Windows.Markup;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Threading;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Data;
using System.Data.SqlTypes;
using Modbus.Device;
using System.IO.Ports;
using NpgsqlTypes;
using System.Collections.ObjectModel;
using System.Globalization;

namespace SCADA
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public class AlphanumComparator : IComparable
    {
        public string Name;
                
        private enum ChunkType { Alphanumeric, Numeric };
        private bool InChunk(char ch, char otherCh)
        {
            ChunkType type = ChunkType.Alphanumeric;

            if (char.IsDigit(otherCh))
            {
                type = ChunkType.Numeric;
            }

            if ((type == ChunkType.Alphanumeric && char.IsDigit(ch))
                || (type == ChunkType.Numeric && !char.IsDigit(ch)))
            {
                return false;
            }

            return true;
        }

        public int CompareTo(object x)
        {
            AlphanumComparator a1 = x as AlphanumComparator;

            string s1 = a1.Name;

            if (s1 == null || Name == null)
            {
                return 0;
            }

            int thisMarker = 0, thisNumericChunk = 0;
            int thatMarker = 0, thatNumericChunk = 0;

            while ((thisMarker < s1.Length) || (thatMarker < Name.Length))
            {
                if (thisMarker >= s1.Length)
                {
                    return 1;
                }
                else if (thatMarker >= Name.Length)
                {
                    return -1;
                }
                char thisCh = s1[thisMarker];
                char thatCh = Name[thatMarker];

                StringBuilder thisChunk = new StringBuilder();
                StringBuilder thatChunk = new StringBuilder();

                while ((thisMarker < s1.Length) && (thisChunk.Length == 0 || InChunk(thisCh, thisChunk[0])))
                {
                    thisChunk.Append(thisCh);
                    thisMarker++;

                    if (thisMarker < s1.Length)
                    {
                        thisCh = s1[thisMarker];
                    }
                }

                while ((thatMarker < Name.Length) && (thatChunk.Length == 0 || InChunk(thatCh, thatChunk[0])))
                {
                    thatChunk.Append(thatCh);
                    thatMarker++;

                    if (thatMarker < Name.Length)
                    {
                        thatCh = Name[thatMarker];
                    }
                }

                int result = 0;
                // If both chunks contain numeric characters, sort them numerically
                if (char.IsDigit(thisChunk[0]) && char.IsDigit(thatChunk[0]))
                {
                    thisNumericChunk = Convert.ToInt32(thisChunk.ToString());
                    thatNumericChunk = Convert.ToInt32(thatChunk.ToString());

                    if (thisNumericChunk < thatNumericChunk)
                    {
                        result = 1;
                    }

                    if (thisNumericChunk > thatNumericChunk)
                    {
                        result = -1;
                    }
                }
                else
                {
                    result = thisChunk.ToString().CompareTo(thatChunk.ToString()) * -1;
                }

                if (result != 0)
                {
                    return result;
                }
            }

            return 0;
        }
    }
    
    public partial class MainWindow : Window
    {
        public double Version = 0.3;

        public List<EthernetObject> CollectionTCPEthernetObject = new List<EthernetObject>();

        public List<EthernetObject> CollectionUDPEthernetObject = new List<EthernetObject>();

        public List<SerialPort> CollectionSerialPortThread = new List<SerialPort>();

        public List<SQLObject> CollectionSQLObject = new List<SQLObject>();

        public ObservableCollection<string> CollectionMessage = new ObservableCollection<string>();

        public List<ColorButton> RecentColorCollection = new List<ColorButton>(7);

        private bool IsCopy;
        public bool IsTabFocus { get; set; }// При переходе через Tab по BottomPanel, если значение не верно, устанавливается старое значение и фокус передается на следующий текстбокс

        private bool isCopyObject;
        public bool IsCopyObject
        {
            get { return isCopyObject; }
            set { isCopyObject = value; }
        }

        private bool isCopyObjects;
        public bool IsCopyObjects
        {
            get { return isCopyObjects; }
            set { isCopyObjects = value; }
        }

        private bool isCutObjects;
        public bool IsCutObjects
        {
            get { return isCutObjects; }
            set { isCutObjects = value; }
        }
        
        public static readonly DependencyProperty IsBindingStartProjectProperty =
          DependencyProperty.Register(
          "IsBindingStartProject",
          typeof(bool),
          typeof(MainWindow),
           new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsBindingInsertControlPanelProperty =
          DependencyProperty.Register(
          "IsBindingInsertControlPanel",
          typeof(bool),
          typeof(MainWindow),
           new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsBindingInsertProperty =
          DependencyProperty.Register(
          "IsBindingInsert",
          typeof(bool),
          typeof(MainWindow),
           new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsBindingInsertTextProperty =
          DependencyProperty.Register(
          "IsBindingInsertText",
          typeof(bool),
          typeof(MainWindow),
           new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsBindingInsertObjectProperty =
         DependencyProperty.Register(
         "IsBindingInsertObject",
         typeof(bool),
         typeof(MainWindow),
          new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty BindingSerializationProjectProperty =
         DependencyProperty.Register(
         "ProjectBin",
         typeof(SerializationProject),
         typeof(MainWindow),
          new FrameworkPropertyMetadata(null));

        public SerializationProject ProjectBin
        {
            get { return (SerializationProject)GetValue(MainWindow.BindingSerializationProjectProperty); }
            set { SetValue(MainWindow.BindingSerializationProjectProperty, value); }
        }

        public bool IsBindingStartProject
        {
            get { return (bool)GetValue(MainWindow.IsBindingStartProjectProperty); }
            set { SetValue(MainWindow.IsBindingStartProjectProperty, value); }
        }

        public bool IsBindingInsert
        {
            get { return (bool)GetValue(MainWindow.IsBindingInsertProperty); }
            set { SetValue(MainWindow.IsBindingInsertProperty, value); }
        }

        public bool IsBindingInsertControlPanel
        {
            get { return (bool)GetValue(MainWindow.IsBindingInsertControlPanelProperty); }
            set { SetValue(MainWindow.IsBindingInsertControlPanelProperty, value); }
        }

        public bool IsBindingInsertText
        {
            get { return (bool)GetValue(MainWindow.IsBindingInsertTextProperty); }
            set { SetValue(MainWindow.IsBindingInsertTextProperty, value); }
        }

        public bool IsBindingInsertObject
        {
            get { return (bool)GetValue(MainWindow.IsBindingInsertObjectProperty); }
            set { SetValue(MainWindow.IsBindingInsertObjectProperty, value); }
        }

        private TreeViewItem currentItem;
        public TreeViewItem CurrentItem
        {
            get { return currentItem; }
            set { currentItem = value; }
        }

        private List<ControlOnCanvas> currentObjects = new List<ControlOnCanvas>();
        public List<ControlOnCanvas> CurrentObjects
        {
            get { return currentObjects; }
            set { currentObjects = value; }
        }

        private HwndSource source = null;
        private IntPtr nextClipboardViewer;
        private IntPtr handle
        {
            get
            {
                return new WindowInteropHelper(this).Handle;
            }
        }

        #region "Win32 API"
        private const int WM_DRAWCLIPBOARD = 0x308;
        private const int WM_CHANGECBCHAIN = 0x030D;
        [DllImport("User32.dll")]
        private static extern int SetClipboardViewer(int hWndNewViewer);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);
        #endregion

        #region "overrides"
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            nextClipboardViewer = (IntPtr)SetClipboardViewer((int)this.handle);
            source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            ChangeClipboardChain(this.handle, nextClipboardViewer);
            if (null != source)
                source.RemoveHook(WndProc);
        }
        #endregion

        #region "Clipboard data"
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_DRAWCLIPBOARD:
                    clipboardData();
                    SendMessage(nextClipboardViewer, msg, wParam, lParam);
                    break;
                case WM_CHANGECBCHAIN:
                    if (wParam == nextClipboardViewer)
                        nextClipboardViewer = lParam;
                    else
                        SendMessage(nextClipboardViewer, msg, wParam, lParam);
                    break;
            }
            return IntPtr.Zero;
        }

        private void clipboardData()
        {
            IDataObject iData = Clipboard.GetDataObject();

            if (iData.GetDataPresent("SCADA.FolderScada") || iData.GetDataPresent("SCADA.PageScada") || iData.GetDataPresent("SCADA.ControlPanelScada"))
            {
                IsBindingInsert = true;

                IsBindingInsertText = false;

                IsBindingInsertObject = false;

                IsBindingInsertControlPanel = false;

                if (currentObjects.Count != 0)
                {
                    CurrentObjects.Clear();
                }
            }            
            else if (iData.GetDataPresent("SCADA.ClipboardManipulation"))
            {
                ClipboardManipulation clipboardManipulation = (ClipboardManipulation)iData.GetData("SCADA.ClipboardManipulation");

                if (clipboardManipulation.Manipulation == 3 || clipboardManipulation.Manipulation == 4)
                {
                    IsBindingInsertControlPanel = true;

                    IsBindingInsertObject = false;
                }
                else if (clipboardManipulation.Manipulation == 1 || clipboardManipulation.Manipulation == 2)
                {
                    IsBindingInsertObject = true;

                    IsBindingInsertControlPanel = false;
                }
                
                IsBindingInsertText = false;

                IsBindingInsert = false;

                if (CurrentItem != null)
                {
                    CurrentItem.Background = new SolidColorBrush(Colors.White);
                    CurrentItem = null;
                }
            }
            else if (iData.GetDataPresent(DataFormats.StringFormat))
            {
                IsBindingInsertText = true;

                IsBindingInsert = false;

                IsBindingInsertObject = false;

                IsBindingInsertControlPanel = false;

                if (currentObjects.Count != 0)
                {
                    CurrentObjects.Clear();
                }

                if (CurrentItem != null)
                {
                    CurrentItem.Background = new SolidColorBrush(Colors.White);
                    CurrentItem = null;
                }
            }
            else
            {
                if (CurrentItem != null)
                {
                    IsBindingInsert = false;

                    CurrentItem.Background = new SolidColorBrush(Colors.White);
                    CurrentItem = null;
                }
              
                if(currentObjects.Count != 0)
                {
                    IsBindingInsertObject = false;

                    IsBindingInsertControlPanel = false;

                    CurrentObjects.Clear();
                }
               
                IsBindingInsertText = false;                            
            }
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SortedBrowseProject(TreeViewItem item)
        {
            foreach (TreeViewItem childItem in item.Items)
            {
                item.Items.SortDescriptions.Clear();
                item.Items.SortDescriptions.Add(new SortDescription("ContextMenu.Tag", ListSortDirection.Ascending));
                item.Items.SortDescriptions.Add(new SortDescription("Header.Tag", ListSortDirection.Ascending));

                SortedBrowseProject(childItem);
            }
        }

        //Открываем проект через ассоциацию файла
        private void OpenProjectFile(SerializationProject serProj)
        {            
            //// Сохраняем старый проект
            //if (((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin != null)
            //{
            //    using (FileStream fs = File.Create(((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject))
            //    {
            //        BinaryFormatter Projserializer = new BinaryFormatter();
            //        Projserializer.Serialize(fs, ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin);
            //    }
            //}
           
            SerializationProject serializationProject = serProj;

            //if (((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin == null)
            //{
                ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin = serializationProject;
            //}
            //else
            //{
            //    // Если старый проект не пустой, коллекции освобождаются
            //    ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionPageScada.Clear();
            //    ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionFolderScada.Clear();
            //    ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionControlPanelScada.Clear();
            //    ((AppWPF)Application.Current).CollectionEthernetSers.Clear();
            //    ((AppWPF)Application.Current).CollectionComSers.Clear();
            //    ((AppWPF)Application.Current).CollectionModbusSers.Clear();

            //    ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin = serializationProject;
            //}

            //((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject = serializationProject.PathProject;
            
            //((AppWPF)Application.Current).CollectionPage.Clear();
            //((AppWPF)Application.Current).CollectionControlPanel.Clear();
            //((AppWPF)System.Windows.Application.Current).CollectionTabItemParent.Clear();
            //((AppWPF)Application.Current).CollectionSaveTabItem.Clear();
            //TabControlMain.Items.Clear();
            //BrowseProject.Items.Clear();

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

            MenuItem MenuItemCreateFolderProject = new MenuItem();
            MenuItemCreateFolderProject.Click += CreateFolder;
            MenuItemCreateFolderProject.Icon = ImageNewFolder;
            MenuItemCreateFolderProject.Header = "Создать папку";

            MenuItem MenuItemCreateControlPanelProject = new MenuItem();
            MenuItemCreateControlPanelProject.Click += CreateControlPanel;
            MenuItemCreateControlPanelProject.Icon = imageNewControlPanel;
            MenuItemCreateControlPanelProject.Header = "Создать щит управления";

            MenuItem MenuItemCreatePageProject = new MenuItem();
            MenuItemCreatePageProject.Click += CreatePage;
            MenuItemCreatePageProject.Icon = imageNewPage;
            MenuItemCreatePageProject.Header = "Создать страницу";

            Binding BindingInsertContextProject = new Binding();
            BindingInsertContextProject.Source = this;
            BindingInsertContextProject.Path = new PropertyPath("IsBindingInsert");
            BindingInsertContextProject.Mode = BindingMode.OneWay;

            MenuItem MenuItemInsertProject = new MenuItem();
            MenuItemInsertProject.Header = "Вставить";
            MenuItemInsertProject.SetBinding(MenuItem.IsEnabledProperty, BindingInsertContextProject);
            MenuItemInsertProject.Icon = imageInsertProject;
            MenuItemInsertProject.Click += InsertItem;

            ContextMenu ContextMenuProject = new ContextMenu();
            ContextMenuProject.Items.Add(MenuItemCreateFolderProject);
            ContextMenuProject.Items.Add(MenuItemCreatePageProject);
            ContextMenuProject.Items.Add(MenuItemCreateControlPanelProject);
            ContextMenuProject.Items.Add(MenuItemInsertProject);

            TreeViewItem ItemNameProject = new TreeViewItem();
            ItemNameProject.Tag = "1";
            ItemNameProject.ContextMenu = ContextMenuProject;
            ItemNameProject.KeyDown += RenameProject;

            Label NameProject = new Label();
            NameProject.Content = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.ProjectName;
            panel.Children.Add(imageProject);
            panel.Children.Add(NameProject);

            ItemNameProject.Header = panel;

            TextBox tbNameProject = new TextBox();
            tbNameProject.KeyDown += LostRename;
            tbNameProject.Text = (string)NameProject.Content;

            NameProject.Tag = tbNameProject;

            BrowseProject.Items.Add(ItemNameProject);

            this.Title = "SCADA " + NameProject.Content;

            FolderScada[] CollectionFolder = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionFolderScada.Values.ToArray();

            Array.Sort(CollectionFolder);

            foreach (FolderScada fs in CollectionFolder)
            {
                StackPanel panelFolder = new StackPanel();
                panelFolder.Orientation = System.Windows.Controls.Orientation.Horizontal;

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

                TextBox tbFolder = new TextBox();
                tbFolder.KeyDown += OkRenameFolder;
                tbFolder.Text = fs.Name;

                Label lNameFolder = new Label();
                lNameFolder.Content = tbFolder.Text;
                lNameFolder.Tag = tbFolder;

                AlphanumComparator a = new AlphanumComparator();
                a.Name = (string)lNameFolder.Content;

                panelFolder.Children.Add(imageFolder);
                panelFolder.Children.Add(lNameFolder);
                panelFolder.Tag = a;

                TreeViewItem ItemFolder = new TreeViewItem();
                ItemFolder.Expanded += Expanded;
                ItemFolder.Collapsed += Collapsed;
                ItemFolder.Tag = fs;
                ItemFolder.KeyDown += RenameFolder;
                ItemFolder.Header = panelFolder;

                if (fs.IsExpand) ItemFolder.IsExpanded = true;

                fs.TreeItem = ItemFolder;

                MenuItem MenuItemCreate = new MenuItem();
                MenuItemCreate.Header = "Добавить";

                MenuItem MenuItemCreateFolder = new MenuItem();
                MenuItemCreateFolder.Icon = imageMenuItemCreateFolder;
                MenuItemCreateFolder.Header = "Папку";
                MenuItemCreateFolder.Tag = ItemFolder; // Нужен для индефикации в какую папку сохранять при создании вложенной папки
                MenuItemCreateFolder.Click += ContextMenuCreateFolder;

                MenuItem MenuItemCreateControlPanel = new MenuItem();
                MenuItemCreateControlPanel.Click += ContextMenuCreateControlPanel;
                MenuItemCreateControlPanel.Icon = imageControlPanel;
                MenuItemCreateControlPanel.Header = "Щит управления";
                MenuItemCreateControlPanel.Tag = ItemFolder;

                MenuItem MenuItemCreatePage = new MenuItem();
                MenuItemCreatePage.Icon = imageMenuItemCreatePage;
                MenuItemCreatePage.Header = "Страницу";
                MenuItemCreatePage.Tag = ItemFolder;  // Нужен для индефикации в какую папку сохранять при создании вложенной страницы
                MenuItemCreatePage.Click += ContextMenuCreatePage;

                MenuItem menuItemDeleteFolder = new MenuItem();
                menuItemDeleteFolder.Header = "Удалить";
                menuItemDeleteFolder.Icon = ImageDelete;
                menuItemDeleteFolder.Tag = fs;
                menuItemDeleteFolder.Click += DeleteItem;

                MenuItem menuItemCopyFolder = new MenuItem();
                menuItemCopyFolder.IsEnabled = false;
                menuItemCopyFolder.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
                menuItemCopyFolder.Header = "Копировать";
                menuItemCopyFolder.Icon = ImageCopy;
                menuItemCopyFolder.Tag = fs;
                menuItemCopyFolder.Click += CopyItem;

                MenuItem menuItemCutFolder = new MenuItem();
                menuItemCutFolder.IsEnabled = false;
                menuItemCutFolder.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
                menuItemCutFolder.Header = "Вырезать";
                menuItemCutFolder.Icon = imageCut;
                menuItemCutFolder.Tag = fs;
                menuItemCutFolder.Click += CutItem;

                Binding BindingInsert = new Binding();
                BindingInsert.Source = this;
                BindingInsert.Path = new PropertyPath("IsBindingInsert");
                BindingInsert.Mode = BindingMode.OneWay;

                MenuItem menuItemInsert = new MenuItem();
                menuItemInsert.Header = "Вставить";
                menuItemInsert.Tag = fs;
                menuItemInsert.SetBinding(MenuItem.IsEnabledProperty, BindingInsert);
                menuItemInsert.Icon = imageInsert;
                menuItemInsert.Click += InsertItem;

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

                if (fs.Attachments != 0)
                {
                    FolderScada parentFolder = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionFolderScada[fs.AttachmentFolder];

                    if (parentFolder.ChildItem == null) parentFolder.ChildItem = new List<TreeViewItem>();

                    parentFolder.ChildItem.Add(ItemFolder);

                    TreeViewItem parentItem = parentFolder.TreeItem;

                    fs.ParentItem = parentItem;

                    parentItem.Items.Add(ItemFolder);
                }
                else BrowseProject.Items.Add(ItemFolder);

            }

            ControlPanelScada[] CollectionControlPanel = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionControlPanelScada.Values.ToArray();

            Array.Sort(CollectionControlPanel);

            foreach (ControlPanelScada cps in CollectionControlPanel)
            {
                StackPanel panelControlPanel = new StackPanel();
                panelControlPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;

                Image imagePage = new Image();
                imagePage.Source = new BitmapImage(new Uri("Images/ControlPanel16.png", UriKind.Relative));

                Image imageCut = new Image();
                imageCut.Source = new BitmapImage(new Uri("Images/Cut16.png", UriKind.Relative));

                Image imageDelete = new Image();
                imageDelete.Source = new BitmapImage(new Uri("Images/PageDelete16.png", UriKind.Relative));

                Image imageCopy = new Image();
                imageCopy.Source = new BitmapImage(new Uri("Images/CopyPage16.png", UriKind.Relative));

                MenuItem menuItemCopyControlPanel = new MenuItem();
                menuItemCopyControlPanel.IsEnabled = false;
                menuItemCopyControlPanel.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
                menuItemCopyControlPanel.Header = "Копировать";
                menuItemCopyControlPanel.Icon = imageCopy;
                menuItemCopyControlPanel.Tag = cps;
                menuItemCopyControlPanel.Click += CopyItem;

                MenuItem menuItemCutControlPanel = new MenuItem();
                menuItemCutControlPanel.IsEnabled = false;
                menuItemCutControlPanel.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
                menuItemCutControlPanel.Header = "Вырезать";
                menuItemCutControlPanel.Icon = imageCut;
                menuItemCutControlPanel.Tag = cps;
                menuItemCutControlPanel.Click += CutItem;

                MenuItem menuItemDeleteControlPanel = new MenuItem();
                menuItemDeleteControlPanel.Header = "Удалить";
                menuItemDeleteControlPanel.Icon = imageDelete;
                menuItemDeleteControlPanel.Tag = cps;
                menuItemDeleteControlPanel.Click += DeleteItem;

                TextBox tbControlPanel = new TextBox();
                tbControlPanel.KeyDown += OkRenameControlPanel;
                tbControlPanel.Text = cps.Name;

                Label lNameControlPanel = new Label();
                lNameControlPanel.Content = tbControlPanel.Text;
                lNameControlPanel.Tag = tbControlPanel;

                ContextMenu contextMenuControlPanel = new ContextMenu();
                contextMenuControlPanel.Items.Add(menuItemCopyControlPanel);
                contextMenuControlPanel.Items.Add(menuItemCutControlPanel);
                contextMenuControlPanel.Items.Add(menuItemDeleteControlPanel);
                contextMenuControlPanel.Tag = "X";

                AlphanumComparator a = new AlphanumComparator();
                a.Name = (string)lNameControlPanel.Content;

                panelControlPanel.Children.Add(imagePage);
                panelControlPanel.Children.Add(lNameControlPanel);
                panelControlPanel.Tag = a;

                TreeViewItem ItemControlPanel = new TreeViewItem();
                ItemControlPanel.Tag = cps;
                ItemControlPanel.MouseDoubleClick += OpenBrowseControlPanel;
                ItemControlPanel.KeyDown += RenameControlPanel;
                ItemControlPanel.Header = panelControlPanel;
                ItemControlPanel.ContextMenu = contextMenuControlPanel;

                cps.TreeItem = ItemControlPanel;

                if (cps.Attachments != 0)
                {
                    using (FileStream ProjectStream = File.Open(cps.Path, FileMode.Open, FileAccess.ReadWrite))
                    {
                        ((AppWPF)Application.Current).CollectionControlPanel.Add(cps.Path, (ControlPanel)XamlReader.Load(ProjectStream));
                        ProjectStream.Close();

                        FolderScada parentFolder = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionFolderScada[cps.AttachmentFolder];

                        TreeViewItem parentItem = parentFolder.TreeItem;
                        parentItem.Items.Add(ItemControlPanel);

                        cps.ParentItem = parentItem;

                        if (parentFolder.ChildItem == null) parentFolder.ChildItem = new List<TreeViewItem>();
                        parentFolder.ChildItem.Add(ItemControlPanel);

                        TabItemControlPanel tabItemControlPanel = new TabItemControlPanel(cps);

                        if (cps.IsOpen) TabControlMain.Items.Add(tabItemControlPanel);
                        if (cps.IsFocus) TabControlMain.SelectedItem = tabItemControlPanel;
                    }
                }
                else
                {
                    using (FileStream ProjectStream = File.Open(cps.Path, FileMode.Open, FileAccess.ReadWrite))
                    {
                        ((AppWPF)Application.Current).CollectionControlPanel.Add(cps.Path, (ControlPanel)XamlReader.Load(ProjectStream));
                        ProjectStream.Close();

                        BrowseProject.Items.Add(ItemControlPanel);

                        TabItemControlPanel tabItemControlPanel = new TabItemControlPanel(cps);

                        if (cps.IsOpen) TabControlMain.Items.Add(tabItemControlPanel);
                        if (cps.IsFocus) TabControlMain.SelectedItem = tabItemControlPanel;
                    }
                }               
            }

            PageScada[] CollectionPage = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionPageScada.Values.ToArray();

            Array.Sort(CollectionPage);

            foreach (PageScada ps in CollectionPage)
            {
                StackPanel panelPage = new StackPanel();
                panelPage.Orientation = System.Windows.Controls.Orientation.Horizontal;

                Image imagePage = new Image();
                imagePage.Source = new BitmapImage(new Uri("Images/Page16.png", UriKind.Relative));

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
                menuItemCopyPage.Click += CopyItem;

                MenuItem menuItemCutPage = new MenuItem();
                menuItemCutPage.IsEnabled = false;
                menuItemCutPage.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
                menuItemCutPage.Header = "Вырезать";
                menuItemCutPage.Icon = imageCut;
                menuItemCutPage.Tag = ps;
                menuItemCutPage.Click += CutItem;

                MenuItem menuItemDeletePage = new MenuItem();
                menuItemDeletePage.Header = "Удалить";
                menuItemDeletePage.Icon = imageDelete;
                menuItemDeletePage.Tag = ps;
                menuItemDeletePage.Click += DeleteItem;

                TextBox tbPage = new TextBox();
                tbPage.KeyDown += OkRenamePage;
                tbPage.Text = ps.Name;

                Label lNamePage = new Label();
                lNamePage.Content = tbPage.Text;
                lNamePage.Tag = tbPage;

                ContextMenu contextMenuPage = new ContextMenu();
                contextMenuPage.Items.Add(menuItemCopyPage);
                contextMenuPage.Items.Add(menuItemCutPage);
                contextMenuPage.Items.Add(menuItemDeletePage);
                contextMenuPage.Tag = "PageScada";

                AlphanumComparator a = new AlphanumComparator();
                a.Name = (string)lNamePage.Content;

                panelPage.Children.Add(imagePage);
                panelPage.Children.Add(lNamePage);
                panelPage.Tag = a;

                TreeViewItem ItemPage = new TreeViewItem();
                ItemPage.MouseDoubleClick += OpenBrowsePage;
                ItemPage.Tag = ps;
                ItemPage.KeyDown += RenamePage;
                ItemPage.Header = panelPage;
                ItemPage.ContextMenu = contextMenuPage;

                ps.TreeItem = ItemPage;

                if (ps.Attachments != 0)
                {
                    FolderScada parentFolder = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionFolderScada[ps.AttachmentFolder];

                    TreeViewItem parentItem = parentFolder.TreeItem;
                    parentItem.Items.Add(ItemPage);

                    ps.ParentItem = parentItem;

                    if (parentFolder.ChildItem == null) parentFolder.ChildItem = new List<TreeViewItem>();
                    parentFolder.ChildItem.Add(ItemPage);

                    using (FileStream ProjectStream = File.Open(ps.Path, FileMode.Open, FileAccess.ReadWrite))
                    {
                        ((AppWPF)Application.Current).CollectionPage.Add(ps.Path, (Page)XamlReader.Load(ProjectStream));
                        ProjectStream.Close();
                    }
                }
                else
                {
                    BrowseProject.Items.Add(ItemPage);

                    using (FileStream ProjectStream = File.Open(ps.Path, FileMode.Open, FileAccess.ReadWrite))
                    {
                        ((AppWPF)Application.Current).CollectionPage.Add(ps.Path, (Page)XamlReader.Load(ProjectStream));
                        ProjectStream.Close();
                    }
                }

                TabItemPage tabItemPage = new TabItemPage(ps);

                if (ps.IsOpen) TabControlMain.Items.Add(tabItemPage);
                if (ps.IsFocus) TabControlMain.SelectedItem = tabItemPage;
            }
            

            BrowseProject.Items.SortDescriptions.Clear();
            BrowseProject.Items.SortDescriptions.Add(new SortDescription("ContextMenu.Tag", ListSortDirection.Ascending));
            BrowseProject.Items.SortDescriptions.Add(new SortDescription("Header.Tag", ListSortDirection.Ascending));

            foreach (TreeViewItem item in BrowseProject.Items)
            {
                SortedBrowseProject(item);
            }
        }

        private void OpenProject(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OpenProjectDialog = new OpenFileDialog();
            OpenProjectDialog.Filter = "SCADA проект(*.proj)|*.proj";
            OpenProjectDialog.InitialDirectory = ((AppWPF)Application.Current).ConfigProgramBin.PathBrowseProject;
            if (OpenProjectDialog.ShowDialog() == true) 
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

                using (FileStream ProjectStream = File.Open(OpenProjectDialog.FileName, FileMode.Open, FileAccess.ReadWrite))
                {
                    BinaryFormatter Deserializer = new BinaryFormatter();

                    SerializationProject serializationProject = (SerializationProject)Deserializer.Deserialize(ProjectStream);

                    if (((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin == null)
                    {
                        ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin = serializationProject;                                       
                    }
                    else
                    {
                        // Если старый проект не пустой, коллекции освобождаются
                        ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionPageScada.Clear();
                        ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionFolderScada.Clear();
                        ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionControlPanelScada.Clear();
                        ((AppWPF)Application.Current).CollectionEthernetSers.Clear();
                        ((AppWPF)Application.Current).CollectionComSers.Clear();
                        ((AppWPF)Application.Current).CollectionModbusSers.Clear();

                        ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin = serializationProject;
                    }

                    ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject = OpenProjectDialog.FileName;
                    ProjectStream.Close();
                }

                ((AppWPF)Application.Current).CollectionPage.Clear();
                ((AppWPF)Application.Current).CollectionControlPanel.Clear();
                ((AppWPF)System.Windows.Application.Current).CollectionTabItemParent.Clear();
                ((AppWPF)Application.Current).CollectionSaveTabItem.Clear();
                TabControlMain.Items.Clear();
                BrowseProject.Items.Clear();

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

                MenuItem MenuItemCreateFolderProject = new MenuItem();
                MenuItemCreateFolderProject.Click += CreateFolder;
                MenuItemCreateFolderProject.Icon = ImageNewFolder;
                MenuItemCreateFolderProject.Header = "Создать папку";

                MenuItem MenuItemCreateControlPanelProject = new MenuItem();
                MenuItemCreateControlPanelProject.Click += CreateControlPanel;
                MenuItemCreateControlPanelProject.Icon = imageNewControlPanel;
                MenuItemCreateControlPanelProject.Header = "Создать щит управления";

                MenuItem MenuItemCreatePageProject = new MenuItem();
                MenuItemCreatePageProject.Click += CreatePage;
                MenuItemCreatePageProject.Icon = imageNewPage;
                MenuItemCreatePageProject.Header = "Создать страницу";

                Binding BindingInsertContextProject = new Binding();
                BindingInsertContextProject.Source = this;
                BindingInsertContextProject.Path = new PropertyPath("IsBindingInsert");
                BindingInsertContextProject.Mode = BindingMode.OneWay;

                MenuItem MenuItemInsertProject = new MenuItem();
                MenuItemInsertProject.Header = "Вставить";
                MenuItemInsertProject.SetBinding(MenuItem.IsEnabledProperty, BindingInsertContextProject);
                MenuItemInsertProject.Icon = imageInsertProject;
                MenuItemInsertProject.Click += InsertItem;

                ContextMenu ContextMenuProject = new ContextMenu();
                ContextMenuProject.Items.Add(MenuItemCreateFolderProject);
                ContextMenuProject.Items.Add(MenuItemCreatePageProject);
                ContextMenuProject.Items.Add(MenuItemCreateControlPanelProject);
                ContextMenuProject.Items.Add(MenuItemInsertProject);

                TreeViewItem ItemNameProject = new TreeViewItem();
                ItemNameProject.Tag = "1";
                ItemNameProject.ContextMenu = ContextMenuProject;
                ItemNameProject.KeyDown += RenameProject;

                Label NameProject = new Label();
                NameProject.Content = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.ProjectName;
                panel.Children.Add(imageProject);
                panel.Children.Add(NameProject);

                ItemNameProject.Header = panel;

                TextBox tbNameProject = new TextBox();
                tbNameProject.KeyDown += LostRename;               
                tbNameProject.Text = (string)NameProject.Content;

                NameProject.Tag = tbNameProject;
                
                BrowseProject.Items.Add(ItemNameProject);

                this.Title = "SCADA " + NameProject.Content;

                FolderScada[] CollectionFolder = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionFolderScada.Values.ToArray();

                Array.Sort(CollectionFolder);

                foreach (FolderScada fs in CollectionFolder)
                {
                    StackPanel panelFolder = new StackPanel();
                    panelFolder.Orientation = System.Windows.Controls.Orientation.Horizontal;

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

                    TextBox tbFolder = new TextBox();
                    tbFolder.KeyDown += OkRenameFolder;
                    tbFolder.Text = fs.Name;

                    Label lNameFolder = new Label();
                    lNameFolder.Content = tbFolder.Text;
                    lNameFolder.Tag = tbFolder;

                    AlphanumComparator a = new AlphanumComparator();
                    a.Name = (string)lNameFolder.Content;

                    panelFolder.Children.Add(imageFolder);
                    panelFolder.Children.Add(lNameFolder);
                    panelFolder.Tag = a;
                        
                    TreeViewItem ItemFolder = new TreeViewItem();
                    ItemFolder.Expanded += Expanded;
                    ItemFolder.Collapsed += Collapsed;
                    ItemFolder.Tag = fs;
                    ItemFolder.KeyDown += RenameFolder;
                    ItemFolder.Header = panelFolder;

                    if (fs.IsExpand) ItemFolder.IsExpanded = true;

                    fs.TreeItem = ItemFolder;

                    MenuItem MenuItemCreate = new MenuItem();
                    MenuItemCreate.Header = "Добавить";

                    MenuItem MenuItemCreateFolder = new MenuItem();
                    MenuItemCreateFolder.Icon = imageMenuItemCreateFolder;
                    MenuItemCreateFolder.Header = "Папку";
                    MenuItemCreateFolder.Tag = ItemFolder; // Нужен для индефикации в какую папку сохранять при создании вложенной папки
                    MenuItemCreateFolder.Click += ContextMenuCreateFolder;

                    MenuItem MenuItemCreateControlPanel = new MenuItem();
                    MenuItemCreateControlPanel.Click += ContextMenuCreateControlPanel;
                    MenuItemCreateControlPanel.Icon = imageControlPanel;
                    MenuItemCreateControlPanel.Header = "Щит управления";
                    MenuItemCreateControlPanel.Tag = ItemFolder;

                    MenuItem MenuItemCreatePage = new MenuItem();
                    MenuItemCreatePage.Icon = imageMenuItemCreatePage;
                    MenuItemCreatePage.Header = "Страницу";
                    MenuItemCreatePage.Tag = ItemFolder;  // Нужен для индефикации в какую папку сохранять при создании вложенной страницы
                    MenuItemCreatePage.Click += ContextMenuCreatePage;

                    MenuItem menuItemDeleteFolder = new MenuItem();
                    menuItemDeleteFolder.Header = "Удалить";
                    menuItemDeleteFolder.Icon = ImageDelete;
                    menuItemDeleteFolder.Tag = fs;
                    menuItemDeleteFolder.Click += DeleteItem;

                    MenuItem menuItemCopyFolder = new MenuItem();
                    menuItemCopyFolder.IsEnabled = false;
                    menuItemCopyFolder.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
                    menuItemCopyFolder.Header = "Копировать";
                    menuItemCopyFolder.Icon = ImageCopy;
                    menuItemCopyFolder.Tag = fs;
                    menuItemCopyFolder.Click += CopyItem;
                                       
                    MenuItem menuItemCutFolder = new MenuItem();
                    menuItemCutFolder.IsEnabled = false;
                    menuItemCutFolder.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
                    menuItemCutFolder.Header = "Вырезать";                    
                    menuItemCutFolder.Icon = imageCut;
                    menuItemCutFolder.Tag = fs; 
                    menuItemCutFolder.Click += CutItem;

                    Binding BindingInsert = new Binding();
                    BindingInsert.Source = this;
                    BindingInsert.Path = new PropertyPath("IsBindingInsert");
                    BindingInsert.Mode = BindingMode.OneWay;

                    MenuItem menuItemInsert = new MenuItem();
                    menuItemInsert.Header = "Вставить";
                    menuItemInsert.Tag = fs;
                    menuItemInsert.SetBinding(MenuItem.IsEnabledProperty, BindingInsert);
                    menuItemInsert.Icon = imageInsert;
                    menuItemInsert.Click += InsertItem;

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

                    if(fs.Attachments != 0)
                    {
                        FolderScada parentFolder = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionFolderScada[fs.AttachmentFolder];

                        if (parentFolder.ChildItem == null) parentFolder.ChildItem = new List<TreeViewItem>();

                        parentFolder.ChildItem.Add(ItemFolder);                       

                        TreeViewItem parentItem = parentFolder.TreeItem;

                        fs.ParentItem = parentItem;

                        parentItem.Items.Add(ItemFolder);
                    }
                    else BrowseProject.Items.Add(ItemFolder);
                }
                
                ControlPanelScada[] CollectionControlPanel = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionControlPanelScada.Values.ToArray();
                Array.Sort(CollectionControlPanel);

                foreach (ControlPanelScada cps in CollectionControlPanel)
                {
                    StackPanel panelControlPanel = new StackPanel();
                    panelControlPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;

                    Image imagePage = new Image();
                    imagePage.Source = new BitmapImage(new Uri("Images/ControlPanel16.png", UriKind.Relative));

                    Image imageCut = new Image();
                    imageCut.Source = new BitmapImage(new Uri("Images/Cut16.png", UriKind.Relative));

                    Image imageDelete = new Image();
                    imageDelete.Source = new BitmapImage(new Uri("Images/PageDelete16.png", UriKind.Relative));

                    Image imageCopy = new Image();
                    imageCopy.Source = new BitmapImage(new Uri("Images/CopyPage16.png", UriKind.Relative));

                    MenuItem menuItemCopyControlPanel = new MenuItem();
                    menuItemCopyControlPanel.IsEnabled = false;
                    menuItemCopyControlPanel.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
                    menuItemCopyControlPanel.Header = "Копировать";
                    menuItemCopyControlPanel.Icon = imageCopy;
                    menuItemCopyControlPanel.Tag = cps;
                    menuItemCopyControlPanel.Click += CopyItem;

                    MenuItem menuItemCutControlPanel = new MenuItem();
                    menuItemCutControlPanel.IsEnabled = false;
                    menuItemCutControlPanel.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
                    menuItemCutControlPanel.Header = "Вырезать";
                    menuItemCutControlPanel.Icon = imageCut;
                    menuItemCutControlPanel.Tag = cps;
                    menuItemCutControlPanel.Click += CutItem;

                    MenuItem menuItemDeleteControlPanel = new MenuItem();
                    menuItemDeleteControlPanel.Header = "Удалить";
                    menuItemDeleteControlPanel.Icon = imageDelete;
                    menuItemDeleteControlPanel.Tag = cps;
                    menuItemDeleteControlPanel.Click += DeleteItem;

                    TextBox tbControlPanel = new TextBox();
                    tbControlPanel.KeyDown += OkRenameControlPanel;
                    tbControlPanel.Text = cps.Name;

                    Label lNameControlPanel = new Label();
                    lNameControlPanel.Content = tbControlPanel.Text;
                    lNameControlPanel.Tag = tbControlPanel;

                    ContextMenu contextMenuControlPanel = new ContextMenu();
                    contextMenuControlPanel.Items.Add(menuItemCopyControlPanel);
                    contextMenuControlPanel.Items.Add(menuItemCutControlPanel);
                    contextMenuControlPanel.Items.Add(menuItemDeleteControlPanel);
                    contextMenuControlPanel.Tag = "X";

                    AlphanumComparator a = new AlphanumComparator();
                    a.Name = (string)lNameControlPanel.Content;

                    panelControlPanel.Children.Add(imagePage);
                    panelControlPanel.Children.Add(lNameControlPanel);
                    panelControlPanel.Tag = a;

                    TreeViewItem ItemControlPanel = new TreeViewItem();
                    ItemControlPanel.Tag = cps;
                    ItemControlPanel.MouseDoubleClick += OpenBrowseControlPanel;
                    ItemControlPanel.KeyDown += RenameControlPanel;
                    ItemControlPanel.Header = panelControlPanel;
                    ItemControlPanel.ContextMenu = contextMenuControlPanel;

                    cps.TreeItem = ItemControlPanel;

                    if (cps.Attachments != 0)
                    {
                        if (File.Exists(cps.Path))
                        {
                            using (FileStream ProjectStream = File.Open(cps.Path, FileMode.Open, FileAccess.ReadWrite))
                            {
                                ((AppWPF)Application.Current).CollectionControlPanel.Add(cps.Path, (ControlPanel)XamlReader.Load(ProjectStream));
                                ProjectStream.Close();

                                FolderScada parentFolder = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionFolderScada[cps.AttachmentFolder];

                                TreeViewItem parentItem = parentFolder.TreeItem;
                                parentItem.Items.Add(ItemControlPanel);

                                cps.ParentItem = parentItem;

                                if (parentFolder.ChildItem == null) parentFolder.ChildItem = new List<TreeViewItem>();
                                parentFolder.ChildItem.Add(ItemControlPanel);

                                TabItemControlPanel tabItemControlPanel = new TabItemControlPanel(cps);

                                if (cps.IsOpen) TabControlMain.Items.Add(tabItemControlPanel);
                                if (cps.IsFocus) TabControlMain.SelectedItem = tabItemControlPanel;
                            }
                        }
                        else
                        {
                            OpenFileDialog NotLoad = new OpenFileDialog();
                            NotLoad.Title = "Не найден файл " + cps.Path; 
                            NotLoad.Filter = "SCADA щит управления(*.cp)|*.cp";
                            NotLoad.InitialDirectory = ((AppWPF)Application.Current).ConfigProgramBin.PathBrowseProject;
                            if (NotLoad.ShowDialog() == true)
                            {
                                ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionControlPanelScada.Remove(cps.Path);

                                using (FileStream ProjectStream = File.Open(cps.Path = NotLoad.FileName, FileMode.Open, FileAccess.ReadWrite))
                                {
                                    ((AppWPF)Application.Current).CollectionControlPanel.Add(cps.Path, (ControlPanel)XamlReader.Load(ProjectStream));
                                    ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionControlPanelScada.Add(cps.Path, cps);
                                    ProjectStream.Close();

                                    FolderScada parentFolder = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionFolderScada[cps.AttachmentFolder];

                                    TreeViewItem parentItem = parentFolder.TreeItem;
                                    parentItem.Items.Add(ItemControlPanel);

                                    cps.ParentItem = parentItem;

                                    if (parentFolder.ChildItem == null) parentFolder.ChildItem = new List<TreeViewItem>();
                                    parentFolder.ChildItem.Add(ItemControlPanel);

                                    TabItemControlPanel tabItemControlPanel = new TabItemControlPanel(cps);

                                    if (cps.IsOpen) TabControlMain.Items.Add(tabItemControlPanel);
                                    if (cps.IsFocus) TabControlMain.SelectedItem = tabItemControlPanel;
                                }
                            }
                            else
                            {
                                ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionControlPanelScada.Remove(cps.Path);
                            }
                        }
                    }
                    else
                    {
                        if (File.Exists(cps.Path))
                        {
                            using (FileStream ProjectStream = File.Open(cps.Path, FileMode.Open, FileAccess.ReadWrite))
                            {
                                ((AppWPF)Application.Current).CollectionControlPanel.Add(cps.Path, (ControlPanel)XamlReader.Load(ProjectStream));
                                ProjectStream.Close();

                                BrowseProject.Items.Add(ItemControlPanel);

                                TabItemControlPanel tabItemControlPanel = new TabItemControlPanel(cps);

                                if (cps.IsOpen) TabControlMain.Items.Add(tabItemControlPanel);
                                if (cps.IsFocus) TabControlMain.SelectedItem = tabItemControlPanel;
                            }
                        }
                        else
                        {
                            OpenFileDialog NotLoad = new OpenFileDialog();
                            NotLoad.Title = "Не найден файл " + cps.Path; 
                            NotLoad.Filter = "SCADA щит управления(*.cp)|*.cp";
                            NotLoad.InitialDirectory = ((AppWPF)Application.Current).ConfigProgramBin.PathBrowseProject;
                            if (NotLoad.ShowDialog() == true)
                            {
                                ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionControlPanelScada.Remove(cps.Path);

                                using (FileStream ProjectStream = File.Open(cps.Path = NotLoad.FileName, FileMode.Open, FileAccess.ReadWrite))
                                {
                                    ((AppWPF)Application.Current).CollectionControlPanel.Add(cps.Path, (ControlPanel)XamlReader.Load(ProjectStream));
                                    ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionControlPanelScada.Add(cps.Path, cps);
                                    ProjectStream.Close();

                                    BrowseProject.Items.Add(ItemControlPanel);

                                    TabItemControlPanel tabItemControlPanel = new TabItemControlPanel(cps);

                                    if (cps.IsOpen) TabControlMain.Items.Add(tabItemControlPanel);
                                    if (cps.IsFocus) TabControlMain.SelectedItem = tabItemControlPanel;
                                }
                            }
                            else
                            {
                                ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionControlPanelScada.Remove(cps.Path);
                            }
                        }
                    }                   
                }

                PageScada[] CollectionPage = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionPageScada.Values.ToArray();
                Array.Sort(CollectionPage);

                foreach (PageScada ps in CollectionPage)
                {
                    StackPanel panelPage = new StackPanel();
                    panelPage.Orientation = System.Windows.Controls.Orientation.Horizontal;

                    Image imagePage = new Image();
                    imagePage.Source = new BitmapImage(new Uri("Images/Page16.png", UriKind.Relative));

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
                    menuItemCopyPage.Click += CopyItem;

                    MenuItem menuItemCutPage = new MenuItem();
                    menuItemCutPage.IsEnabled = false;
                    menuItemCutPage.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
                    menuItemCutPage.Header = "Вырезать";
                    menuItemCutPage.Icon = imageCut;
                    menuItemCutPage.Tag = ps;
                    menuItemCutPage.Click += CutItem;

                    MenuItem menuItemDeletePage = new MenuItem();
                    menuItemDeletePage.Header = "Удалить";
                    menuItemDeletePage.Icon = imageDelete;
                    menuItemDeletePage.Tag = ps;
                    menuItemDeletePage.Click += DeleteItem;
                 
                    TextBox tbPage = new TextBox();
                    tbPage.KeyDown += OkRenamePage;
                    tbPage.Text = ps.Name;

                    Label lNamePage = new Label();
                    lNamePage.Content = tbPage.Text;
                    lNamePage.Tag = tbPage;

                    ContextMenu contextMenuPage = new ContextMenu();
                    contextMenuPage.Items.Add(menuItemCopyPage);
                    contextMenuPage.Items.Add(menuItemCutPage);
                    contextMenuPage.Items.Add(menuItemDeletePage);
                    contextMenuPage.Tag = "PageScada";

                    AlphanumComparator a = new AlphanumComparator();
                    a.Name = (string)lNamePage.Content;

                    panelPage.Children.Add(imagePage);
                    panelPage.Children.Add(lNamePage);
                    panelPage.Tag = a;

                    TreeViewItem ItemPage = new TreeViewItem();
                    ItemPage.MouseDoubleClick += OpenBrowsePage;
                    ItemPage.Tag = ps;
                    ItemPage.KeyDown += RenamePage;
                    ItemPage.Header = panelPage;
                    ItemPage.ContextMenu = contextMenuPage;

                    ps.TreeItem = ItemPage;
                   
                    if (ps.Attachments != 0)
                    {
                        if (File.Exists(ps.Path))
                        {
                            using (FileStream ProjectStream = File.Open(ps.Path, FileMode.Open, FileAccess.ReadWrite))
                            {
                                ((AppWPF)Application.Current).CollectionPage.Add(ps.Path, (Page)XamlReader.Load(ProjectStream));
                                ProjectStream.Close();

                                FolderScada parentFolder = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionFolderScada[ps.AttachmentFolder];

                                TreeViewItem parentItem = parentFolder.TreeItem;
                                parentItem.Items.Add(ItemPage);

                                ps.ParentItem = parentItem;

                                if (parentFolder.ChildItem == null) parentFolder.ChildItem = new List<TreeViewItem>();
                                parentFolder.ChildItem.Add(ItemPage);

                                TabItemPage tabItemPage = new TabItemPage(ps);

                                if (ps.IsOpen) TabControlMain.Items.Add(tabItemPage);
                                if (ps.IsFocus) TabControlMain.SelectedItem = tabItemPage;
                            }
                        }
                        else
                        {
                            OpenFileDialog NotLoad = new OpenFileDialog();
                            NotLoad.Filter = "SCADA страница(*.pg)|*.pg";
                            NotLoad.Title = "Не найден файл " + ps.Path; 
                            NotLoad.InitialDirectory = ((AppWPF)Application.Current).ConfigProgramBin.PathBrowseProject;
                            if (NotLoad.ShowDialog() == true)
                            {
                                ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionPageScada.Remove(ps.Path);

                                using (FileStream ProjectStream = File.Open(ps.Path = NotLoad.FileName, FileMode.Open, FileAccess.ReadWrite))
                                {
                                    ((AppWPF)Application.Current).CollectionPage.Add(ps.Path, (Page)XamlReader.Load(ProjectStream));
                                    ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionPageScada.Add(ps.Path, ps);
                                    ProjectStream.Close();

                                    FolderScada parentFolder = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionFolderScada[ps.AttachmentFolder];

                                    TreeViewItem parentItem = parentFolder.TreeItem;
                                    parentItem.Items.Add(ItemPage);

                                    ps.ParentItem = parentItem;

                                    if (parentFolder.ChildItem == null) parentFolder.ChildItem = new List<TreeViewItem>();
                                    parentFolder.ChildItem.Add(ItemPage);

                                    TabItemPage tabItemPage = new TabItemPage(ps);

                                    if (ps.IsOpen) TabControlMain.Items.Add(tabItemPage);
                                    if (ps.IsFocus) TabControlMain.SelectedItem = tabItemPage;
                                }
                            }
                            else
                            {
                                ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionPageScada.Remove(ps.Path);
                            }
                        }
                    }
                    else 
                    {
                        if (File.Exists(ps.Path))
                        {
                            using (FileStream ProjectStream = File.Open(ps.Path, FileMode.Open, FileAccess.ReadWrite))
                            {
                                ((AppWPF)Application.Current).CollectionPage.Add(ps.Path, (Page)XamlReader.Load(ProjectStream));
                                ProjectStream.Close();

                                BrowseProject.Items.Add(ItemPage);

                                TabItemPage tabItemPage = new TabItemPage(ps);

                                if (ps.IsOpen) TabControlMain.Items.Add(tabItemPage);
                                if (ps.IsFocus) TabControlMain.SelectedItem = tabItemPage;
                            }
                        }
                        else
                        {
                            OpenFileDialog NotLoad = new OpenFileDialog();
                            NotLoad.Title = "Не найден файл " + ps.Path; 
                            NotLoad.Filter = "SCADA страница(*.pg)|*.pg";
                            NotLoad.InitialDirectory = ((AppWPF)Application.Current).ConfigProgramBin.PathBrowseProject;
                            if (NotLoad.ShowDialog() == true)
                            {
                                ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionPageScada.Remove(ps.Path);

                                using (FileStream ProjectStream = File.Open(ps.Path = NotLoad.FileName, FileMode.Open, FileAccess.ReadWrite))
                                {
                                    ((AppWPF)Application.Current).CollectionPage.Add(ps.Path, (Page)XamlReader.Load(ProjectStream));
                                    ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionPageScada.Add(ps.Path, ps);
                                    ProjectStream.Close();

                                    BrowseProject.Items.Add(ItemPage);

                                    TabItemPage tabItemPage = new TabItemPage(ps);

                                    if (ps.IsOpen) TabControlMain.Items.Add(tabItemPage);
                                    if (ps.IsFocus) TabControlMain.SelectedItem = tabItemPage;
                                }
                            }
                            else
                            {
                                ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionPageScada.Remove(ps.Path);
                            }
                        }  
                    }                    
                }              
            }

            BrowseProject.Items.SortDescriptions.Clear();
            BrowseProject.Items.SortDescriptions.Add(new SortDescription("ContextMenu.Tag", ListSortDirection.Ascending));
            BrowseProject.Items.SortDescriptions.Add(new SortDescription("Header.Tag", ListSortDirection.Ascending));

            foreach (TreeViewItem item in BrowseProject.Items)
            {
                SortedBrowseProject(item);
            }
        
            e.Handled = true;
        }

        public void OpenBrowseControlPanel(object sender, MouseButtonEventArgs e)
        {
            ControlPanelScada cps = (ControlPanelScada)((TreeViewItem)sender).Tag;

            TabItemParent tabItemParent = ((AppWPF)System.Windows.Application.Current).CollectionTabItemParent[cps.Path];

            if (TabControlMain.Items.IndexOf(tabItemParent) == -1)
            {
                TabControlMain.Items.Add(tabItemParent);
            }

            tabItemParent.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                tabItemParent.Focus();
            }));

            e.Handled = true;
        }

        public void OpenBrowsePage(object sender, MouseButtonEventArgs e)
        {
            PageScada ps = (PageScada)((TreeViewItem)sender).Tag;

            TabItemParent tabItemParent = ((AppWPF)System.Windows.Application.Current).CollectionTabItemParent[ps.Path];

            if (TabControlMain.Items.IndexOf(tabItemParent) == -1)
            {
                TabControlMain.Items.Add(tabItemParent);
            }

            tabItemParent.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                tabItemParent.Focus();
            }));
           
            e.Handled = true;
        }

        public void Collapsed(object sender, RoutedEventArgs e)
        {
            FolderScada fs = (FolderScada)((TreeViewItem)sender).Tag;
            fs.IsExpand = false;

            e.Handled = true;
        }

        public void Expanded(object sender, RoutedEventArgs e)
        {
            FolderScada fs = (FolderScada)((TreeViewItem)sender).Tag;
            fs.IsExpand = true;

            e.Handled = true;
        }

        public void ContextMenuCreateControlPanel(object sender, RoutedEventArgs e)
        {
            DialogWindowContextMenuCreateControlPanel DialogWindow = new DialogWindowContextMenuCreateControlPanel();
            DialogWindow.Owner = this;
            DialogWindow.Tag = ((MenuItem)sender).Tag;
            DialogWindow.ShowDialog();

            e.Handled = true;
        }

        public void OkRenameControlPanel(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox t = (TextBox)sender;
                StackPanel panel = (StackPanel)t.Parent;

                Label l = (Label)t.Tag;

                TreeViewItem i = (TreeViewItem)panel.Parent;

                ControlPanelScada cps = (ControlPanelScada)i.Tag;

                char[] InvalidChars = { '"', '/', '\\', '<', '>', '?', '*', '|', ':' };

                if (t.Text.IndexOfAny(InvalidChars) != -1)
                {
                    Popup ErrorPopup = new Popup();
                    ErrorPopup.PlacementTarget = i;
                    ErrorPopup.Placement = PlacementMode.Bottom;
                    ErrorPopup.AllowsTransparency = true;
                    ErrorPopup.PopupAnimation = PopupAnimation.Slide;
                    ErrorPopup.StaysOpen = false;

                    Border border = new Border();
                    border.BorderThickness = new Thickness(2);
                    border.Background = new SolidColorBrush(Colors.White);
                    border.BorderBrush = new SolidColorBrush(Colors.Red);

                    TextBlock text = new TextBlock();
                    text.TextWrapping = TextWrapping.Wrap;
                    text.Foreground = new SolidColorBrush(Colors.Black);
                    text.FontSize = 16;
                    text.Text = "Имя щита управления не должно содержать символы: < > | \" / \\ * : ?";

                    Binding BindWidth = new Binding();
                    BindWidth.Source = BrowseProject;
                    BindWidth.Path = new PropertyPath("ActualWidth");
                    BindWidth.Mode = BindingMode.OneTime;

                    ErrorPopup.SetBinding(Popup.WidthProperty, BindWidth);

                    border.Child = text;

                    ErrorPopup.Child = border;
                    ErrorPopup.IsOpen = true;

                    e.Handled = true;

                    return;
                }

                if (string.IsNullOrWhiteSpace(t.Text))
                {
                    Popup ErrorPopup = new Popup();
                    ErrorPopup.PlacementTarget = i;
                    ErrorPopup.Placement = PlacementMode.Bottom;
                    ErrorPopup.AllowsTransparency = true;
                    ErrorPopup.PopupAnimation = PopupAnimation.Slide;
                    ErrorPopup.StaysOpen = false;

                    Border border = new Border();
                    border.BorderThickness = new Thickness(2);
                    border.Background = new SolidColorBrush(Colors.White);
                    border.BorderBrush = new SolidColorBrush(Colors.Red);

                    TextBlock text = new TextBlock();
                    text.TextWrapping = TextWrapping.Wrap;
                    text.Foreground = new SolidColorBrush(Colors.Black);
                    text.FontSize = 16;
                    text.Text = "Имя щита управления не должно содержать только пробелы или быть пустой строкой.";

                    Binding BindWidth = new Binding();
                    BindWidth.Source = BrowseProject;
                    BindWidth.Path = new PropertyPath("ActualWidth");
                    BindWidth.Mode = BindingMode.OneTime;

                    ErrorPopup.SetBinding(Popup.WidthProperty, BindWidth);

                    border.Child = text;

                    ErrorPopup.Child = border;
                    ErrorPopup.IsOpen = true;

                    e.Handled = true;

                    return;
                }

                if (!t.Text.EndsWith(".cp"))
                {
                    t.Text = t.Text + ".cp";
                }

                ControlPanel controlPanel = ((AppWPF)Application.Current).CollectionControlPanel[cps.Path];

                int index = cps.Path.LastIndexOf(cps.Name);
                string path = cps.Path.Remove(index);
                path = path + t.Text;

                if (cps.Path == path)
                {
                    panel.Children.Remove(t);
                    panel.Children.Add(l);

                    e.Handled = true;

                    return;
                }

                //Если файл страницы с таким именем существует, переименование не происходит.  
                if (File.Exists(path))
                {
                    MessageBox.Show("Внимание! Щит управления " + path + " уже существует.", "Ошибка переименования", MessageBoxButton.OK, MessageBoxImage.Error);

                    e.Handled = true;

                    return;
                }
                else File.Move(cps.Path, path); // Переименование

                l.Content = t.Text;

                AlphanumComparator a = new AlphanumComparator();
                a.Name = (string)l.Content;

                panel.Children.Remove(t);
                panel.Children.Add(l);
                panel.Tag = a;

                TabItemParent tabParent = ((AppWPF)Application.Current).CollectionTabItemParent[cps.Path];

                this.RemoveCollectionControlPanel(cps);

                cps.Name = t.Text;
                cps.Path = path;

                tabParent.RenameTab();

                if (cps.Attachments > 0)
                {
                    TreeViewItem parentFolder = cps.ParentItem;

                    parentFolder.Items.SortDescriptions.Clear();
                    parentFolder.Items.SortDescriptions.Add(new SortDescription("ContextMenu.Tag", ListSortDirection.Ascending));
                    parentFolder.Items.SortDescriptions.Add(new SortDescription("Header.Tag", ListSortDirection.Ascending));
                }
                else
                {
                    BrowseProject.Items.SortDescriptions.Clear();
                    BrowseProject.Items.SortDescriptions.Add(new SortDescription("ContextMenu.Tag", ListSortDirection.Ascending));
                    BrowseProject.Items.SortDescriptions.Add(new SortDescription("Header.Tag", ListSortDirection.Ascending));
                }

                this.AddCollectionControlPanel(cps, tabParent, controlPanel);

                e.Handled = true;
            }
        }

        public void RenameControlPanel(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2)
            {
                TreeViewItem i = (TreeViewItem)sender;
                StackPanel panel = (StackPanel)i.Header;
                Label l = (Label)panel.Children[1];
                TextBox t = (TextBox)l.Tag;
                t.Tag = l;
                panel.Children.Remove(l);
                panel.Children.Add(t);
                t.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    t.Focus();
                }));
                t.SelectAll();

                e.Handled = true;
            }
        }

        public void CreateControlPanel(object sender, RoutedEventArgs e)
        {
            DialogWindowCreateControlPanel DialogCreateFile = new DialogWindowCreateControlPanel();
            DialogCreateFile.Owner = this;
            DialogCreateFile.ShowDialog();
            e.Handled = true;
        }

        private void CreateProject(object sender, RoutedEventArgs e)
        {
            DialogWindowCreateProject DialogProject = new DialogWindowCreateProject();
            DialogProject.Owner = this;
            DialogProject.ShowDialog();
            e.Handled = true;
        }

        private void Options(object sender, RoutedEventArgs e)
        {
            DialogWindowSettingProgram DialogSetting = new DialogWindowSettingProgram();
            DialogSetting.Owner = this;
            DialogSetting.ShowDialog();
            e.Handled = true;
        }

        private void ShowElements(object sender, RoutedEventArgs e)
        {
            Elements.Visibility = System.Windows.Visibility.Visible;
            e.Handled = true;       
        }

        private void HideElements(object sender, RoutedEventArgs e)
        {
            Elements.Visibility = System.Windows.Visibility.Collapsed;
            e.Handled = true;
        }

        private void ShowBrowseProject(object sender, RoutedEventArgs e)
        {
            BrowseProject.Visibility = System.Windows.Visibility.Visible;
        }

        private void HideBrowseProject(object sender, RoutedEventArgs e)
        {
            BrowseProject.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void RenameProject(object sender, KeyEventArgs e) 
        {
            if (e.Key == Key.F2)
            {
                TreeViewItem i = (TreeViewItem)sender;
                StackPanel panel = (StackPanel)i.Header;
                Label l = (Label)panel.Children[1];
                TextBox t = (TextBox)l.Tag;
                t.Tag = l;
                panel.Children.Remove(l);
                panel.Children.Add(t);
                t.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    t.Focus();
                }));
                t.SelectAll();

                e.Handled = true;
            }          
        }

        public void LostRename(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {              
                TextBox t = (TextBox)sender;
                StackPanel panel = (StackPanel)t.Parent;
                Label l = (Label)t.Tag;

                char[] InvalidChars = {'"', '/', '\\', '<', '>', '?', '*', '|', ':'}; 

                if (t.Text.IndexOfAny(InvalidChars) != -1)
                {
                    TreeViewItem i = (TreeViewItem)panel.Parent;
                   
                    Popup ErrorPopup = new Popup();
                    ErrorPopup.PlacementTarget = i;
                    ErrorPopup.Placement = PlacementMode.Bottom;
                    ErrorPopup.AllowsTransparency = true;
                    ErrorPopup.PopupAnimation = PopupAnimation.Slide;
                    ErrorPopup.StaysOpen = false;

                    Border border = new Border();
                    border.BorderThickness = new Thickness(2);
                    border.Background = new SolidColorBrush(Colors.White);
                    border.BorderBrush = new SolidColorBrush(Colors.Red);                  

                    TextBlock text = new TextBlock();
                    text.TextWrapping = TextWrapping.Wrap;
                    text.Foreground = new SolidColorBrush(Colors.Black);
                    text.FontSize = 16;
                    text.Text = "Имя проекта не должно содержать символы: < > | \" / \\ * : ?";

                    Binding BindWidth = new Binding();
                    BindWidth.Source = BrowseProject;
                    BindWidth.Path = new PropertyPath("ActualWidth");
                    BindWidth.Mode = BindingMode.OneTime;

                    ErrorPopup.SetBinding(Popup.WidthProperty, BindWidth);

                    border.Child = text;
                                        
                    ErrorPopup.Child = border;
                    ErrorPopup.IsOpen = true;

                    e.Handled = true;
                
                    return;
                }

                if (string.IsNullOrWhiteSpace(t.Text))
                {
                    TreeViewItem i = (TreeViewItem)panel.Parent;

                    Popup ErrorPopup = new Popup();
                    ErrorPopup.PlacementTarget = i;
                    ErrorPopup.Placement = PlacementMode.Bottom;
                    ErrorPopup.AllowsTransparency = true;
                    ErrorPopup.PopupAnimation = PopupAnimation.Slide;
                    ErrorPopup.StaysOpen = false;

                    Border border = new Border();
                    border.BorderThickness = new Thickness(2);
                    border.Background = new SolidColorBrush(Colors.White);
                    border.BorderBrush = new SolidColorBrush(Colors.Red);

                    TextBlock text = new TextBlock();
                    text.TextWrapping = TextWrapping.Wrap;
                    text.Foreground = new SolidColorBrush(Colors.Black);
                    text.FontSize = 16;
                    text.Text = "Имя проекта не должно содержать только пробелы или быть пустой строкой.";

                    Binding BindWidth = new Binding();
                    BindWidth.Source = BrowseProject;
                    BindWidth.Path = new PropertyPath("ActualWidth");
                    BindWidth.Mode = BindingMode.OneTime;

                    ErrorPopup.SetBinding(Popup.WidthProperty, BindWidth);

                    border.Child = text;

                    ErrorPopup.Child = border;
                    ErrorPopup.IsOpen = true;

                    e.Handled = true;

                    return;
                }

                SerializationProject SerProj = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin;

                if (!t.Text.EndsWith(".proj"))
                {
                    t.Text = t.Text + ".proj";
                }

                int index = SerProj.PathProject.LastIndexOf(SerProj.ProjectName);
                string path = SerProj.PathProject.Remove(index);
                path = path + t.Text;

                if (SerProj.ProjectName == t.Text)
                {
                    panel.Children.Remove(t);
                    panel.Children.Add(l);

                    e.Handled = true;

                    return;
                }


                //Если файл проекта с таким именем существует, переименование не происходит.  
                if (File.Exists(path))
                {
                     MessageBox.Show("Внимание! Проект " + path + " уже существует.", "Ошибка переименования", MessageBoxButton.OK, MessageBoxImage.Error);
                 
                    e.Handled = true;

                    return;           
                }
                else File.Move(SerProj.PathProject, path); // Переименование


                l.Content = t.Text;
                panel.Children.Remove(t);
                panel.Children.Add(l);
                this.Title = "SCADA " + l.Content;

                SerProj.ProjectName = t.Text;
                SerProj.PathProject = path;

                e.Handled = true;
            }
        }

        public void CreatePage(object sender, RoutedEventArgs e)
        {
            DialogWindowCreateFile DialogCreateFile = new DialogWindowCreateFile();
            DialogCreateFile.Owner = this;
            DialogCreateFile.ShowDialog();
            e.Handled = true;

        }
      
        private void OpenFile(object sender, RoutedEventArgs e)
        {          
            OpenFileDialog OpenFile = new OpenFileDialog();
            OpenFile.InitialDirectory = ((AppWPF)Application.Current).ConfigProgramBin.PathBrowseProject;
            OpenFile.Filter = "PrimScada файлы|*.cp;*.pg;";
            if (OpenFile.ShowDialog() == true)
            {
                using (FileStream ProjectStream = File.Open(OpenFile.FileName, FileMode.Open, FileAccess.ReadWrite))
                {
                    if (OpenFile.FileName.IndexOf(".cp") != -1)
                    {
                        ControlPanelScada[] CollectionControlPanel = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionControlPanelScada.Values.ToArray();

                        foreach (ControlPanelScada controlPanelScada in CollectionControlPanel)
                        {
                            if (controlPanelScada.Path == OpenFile.FileName)
                            {
                                MessageBox.Show("Такая панель управления уже существует", "Ошибка добавления панели управления", MessageBoxButton.OK, MessageBoxImage.Warning);

                                return;
                            }
                        }

                        ControlPanelScada cps = new ControlPanelScada();
                        cps.Name = OpenFile.SafeFileName;
                        cps.Path = OpenFile.FileName;

                        StackPanel panelControlPanel = new StackPanel();
                        panelControlPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;

                        Image imagePage = new Image();
                        imagePage.Source = new BitmapImage(new Uri("Images/ControlPanel16.png", UriKind.Relative));

                        Image imageCut = new Image();
                        imageCut.Source = new BitmapImage(new Uri("Images/Cut16.png", UriKind.Relative));

                        Image imageDelete = new Image();
                        imageDelete.Source = new BitmapImage(new Uri("Images/PageDelete16.png", UriKind.Relative));

                        Image imageCopy = new Image();
                        imageCopy.Source = new BitmapImage(new Uri("Images/CopyPage16.png", UriKind.Relative));

                        MenuItem menuItemCopyControlPanel = new MenuItem();
                        menuItemCopyControlPanel.IsEnabled = false;
                        menuItemCopyControlPanel.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
                        menuItemCopyControlPanel.Header = "Копировать";
                        menuItemCopyControlPanel.Icon = imageCopy;
                        menuItemCopyControlPanel.Tag = cps;
                        menuItemCopyControlPanel.Click += CopyItem;

                        MenuItem menuItemCutControlPanel = new MenuItem();
                        menuItemCutControlPanel.IsEnabled = false;
                        menuItemCutControlPanel.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
                        menuItemCutControlPanel.Header = "Вырезать";
                        menuItemCutControlPanel.Icon = imageCut;
                        menuItemCutControlPanel.Tag = cps;
                        menuItemCutControlPanel.Click += CutItem;

                        MenuItem menuItemDeleteControlPanel = new MenuItem();
                        menuItemDeleteControlPanel.Header = "Удалить";
                        menuItemDeleteControlPanel.Icon = imageDelete;
                        menuItemDeleteControlPanel.Tag = cps;
                        menuItemDeleteControlPanel.Click += DeleteItem;

                        TextBox tbControlPanel = new TextBox();
                        tbControlPanel.KeyDown += OkRenameControlPanel;
                        tbControlPanel.Text = cps.Name;

                        Label lNameControlPanel = new Label();
                        lNameControlPanel.Content = tbControlPanel.Text;
                        lNameControlPanel.Tag = tbControlPanel;

                        ContextMenu contextMenuControlPanel = new ContextMenu();
                        contextMenuControlPanel.Items.Add(menuItemCopyControlPanel);
                        contextMenuControlPanel.Items.Add(menuItemCutControlPanel);
                        contextMenuControlPanel.Items.Add(menuItemDeleteControlPanel);
                        contextMenuControlPanel.Tag = "X";

                        AlphanumComparator a = new AlphanumComparator();
                        a.Name = (string)lNameControlPanel.Content;

                        panelControlPanel.Children.Add(imagePage);
                        panelControlPanel.Children.Add(lNameControlPanel);
                        panelControlPanel.Tag = a;

                        TreeViewItem ItemControlPanel = new TreeViewItem();
                        ItemControlPanel.Tag = cps;
                        ItemControlPanel.MouseDoubleClick += OpenBrowseControlPanel;
                        ItemControlPanel.KeyDown += RenameControlPanel;
                        ItemControlPanel.Header = panelControlPanel;
                        ItemControlPanel.ContextMenu = contextMenuControlPanel;

                        cps.TreeItem = ItemControlPanel;

                        ControlPanel controlPanel = (ControlPanel)XamlReader.Load(ProjectStream);

                        ((AppWPF)Application.Current).CollectionControlPanel.Add(cps.Path, controlPanel);
                        ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionControlPanelScada.Add(cps.Path, cps);
                        ProjectStream.Close();

                        BrowseProject.Items.Add(ItemControlPanel);

                        TabItemControlPanel tabItemControlPanel = new TabItemControlPanel(cps);

                        if (cps.IsOpen) TabControlMain.Items.Add(tabItemControlPanel);
                        if (cps.IsFocus) TabControlMain.SelectedItem = tabItemControlPanel;

                        foreach (TreeViewItem item in BrowseProject.Items)
                        {
                            SortedBrowseProject(item);
                        }
                    }
                    else if (OpenFile.FileName.IndexOf(".pg") != -1)
                    {
                        PageScada[] CollectionPage = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionPageScada.Values.ToArray();

                        foreach (PageScada pageScada in CollectionPage)
                        {
                            if (pageScada.Path == OpenFile.FileName)
                            {
                                MessageBox.Show("Такая страница уже существует", "Ошибка добавления страницы", MessageBoxButton.OK, MessageBoxImage.Warning);

                                return;
                            }
                        }

                        PageScada ps = new PageScada();
                        ps.Name = OpenFile.SafeFileName;
                        ps.Path = OpenFile.FileName;

                        StackPanel panelPage = new StackPanel();
                        panelPage.Orientation = System.Windows.Controls.Orientation.Horizontal;

                        Image imagePage = new Image();
                        imagePage.Source = new BitmapImage(new Uri("Images/Page16.png", UriKind.Relative));

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
                        menuItemCopyPage.Click += CopyItem;

                        MenuItem menuItemCutPage = new MenuItem();
                        menuItemCutPage.IsEnabled = false;
                        menuItemCutPage.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
                        menuItemCutPage.Header = "Вырезать";
                        menuItemCutPage.Icon = imageCut;
                        menuItemCutPage.Tag = ps;
                        menuItemCutPage.Click += CutItem;

                        MenuItem menuItemDeletePage = new MenuItem();
                        menuItemDeletePage.Header = "Удалить";
                        menuItemDeletePage.Icon = imageDelete;
                        menuItemDeletePage.Tag = ps;
                        menuItemDeletePage.Click += DeleteItem;

                        TextBox tbPage = new TextBox();
                        tbPage.KeyDown += OkRenamePage;
                        tbPage.Text = ps.Name;

                        Label lNamePage = new Label();
                        lNamePage.Content = tbPage.Text;
                        lNamePage.Tag = tbPage;

                        ContextMenu contextMenuPage = new ContextMenu();
                        contextMenuPage.Items.Add(menuItemCopyPage);
                        contextMenuPage.Items.Add(menuItemCutPage);
                        contextMenuPage.Items.Add(menuItemDeletePage);
                        contextMenuPage.Tag = "PageScada";

                        AlphanumComparator a = new AlphanumComparator();
                        a.Name = (string)lNamePage.Content;

                        panelPage.Children.Add(imagePage);
                        panelPage.Children.Add(lNamePage);
                        panelPage.Tag = a;

                        TreeViewItem ItemPage = new TreeViewItem();
                        ItemPage.MouseDoubleClick += OpenBrowsePage;
                        ItemPage.Tag = ps;
                        ItemPage.KeyDown += RenamePage;
                        ItemPage.Header = panelPage;
                        ItemPage.ContextMenu = contextMenuPage;

                        ps.TreeItem = ItemPage;

                        Page page = (Page)XamlReader.Load(ProjectStream);

                        ((AppWPF)Application.Current).CollectionPage.Add(ps.Path, page);
                        ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionPageScada.Add(ps.Path, ps);
                        ProjectStream.Close();

                        BrowseProject.Items.Add(ItemPage);

                        TabItemPage tabItemPage = new TabItemPage(ps);

                        if (ps.IsOpen) TabControlMain.Items.Add(tabItemPage);
                        if (ps.IsFocus) TabControlMain.SelectedItem = tabItemPage;

                        foreach (TreeViewItem item in BrowseProject.Items)
                        {
                            SortedBrowseProject(item);
                        }
                    }   
                }                             
            }
            e.Handled = true;
        }

        public void CreateFolder(object sender, RoutedEventArgs e)
        {
            DialogWindowCreateFolder DialogCreateFolder = new DialogWindowCreateFolder();
            DialogCreateFolder.Owner = this;
            DialogCreateFolder.ShowDialog();

            e.Handled = true;
        }

        public void RenameFolder(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2)
            {
                TreeViewItem i = (TreeViewItem)sender;
                StackPanel panel = (StackPanel)i.Header;
                Label l = (Label)panel.Children[1];
                TextBox t = (TextBox)l.Tag;
                t.Tag = l;
                panel.Children.Remove(l);
                panel.Children.Add(t);
                t.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    t.Focus();
                }));
                t.SelectAll();

                e.Handled = true;
            }
        }

        public void RenameFolderCollection(TreeViewItem parentItem)
        {
            ItemCollection collectionItem = parentItem.Items;

            FolderScada parentFolder;

             if (parentItem.Tag is FolderScada)
             {
                 parentFolder = (FolderScada)parentItem.Tag;
             }
             else parentFolder = null;
           
            foreach (TreeViewItem child in collectionItem)
            {
                object objectChild = child.Tag;

                if(objectChild is FolderScada)
                {                  
                    FolderScada childFolderScada = (FolderScada)objectChild;

                    ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionFolderScada.Remove(childFolderScada.Path);

                    childFolderScada.AttachmentFolder = parentFolder.Path;
                    childFolderScada.Path = parentFolder.Path + "\\" + childFolderScada.Name;

                    ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionFolderScada.Add(childFolderScada.Path, childFolderScada);
                }

                if (objectChild is PageScada)
                {
                    PageScada childPageScada = (PageScada)objectChild;
                    Page pageChild = ((AppWPF)Application.Current).CollectionPage[childPageScada.Path];

                    TabItemParent tabParent =  ((AppWPF)Application.Current).CollectionTabItemParent[childPageScada.Path];

                    this.RemoveCollectionPage(childPageScada);

                    childPageScada.Path = parentFolder.Path + "\\" + childPageScada.Name;
                    childPageScada.AttachmentFolder = parentFolder.Path;                  

                    tabParent.RenameTab();

                    this.AddCollectionPage(childPageScada, tabParent, pageChild);
                }

                if (objectChild is ControlPanelScada)
                {
                    ControlPanelScada childControlPanelScada = (ControlPanelScada)objectChild;
                    ControlPanel cPanelChild = ((AppWPF)Application.Current).CollectionControlPanel[childControlPanelScada.Path];

                    TabItemParent tabParent = ((AppWPF)Application.Current).CollectionTabItemParent[childControlPanelScada.Path];

                    this.RemoveCollectionControlPanel(childControlPanelScada);

                    childControlPanelScada.Path = parentFolder.Path + "\\" + childControlPanelScada.Name;
                    childControlPanelScada.AttachmentFolder = parentFolder.Path;

                    tabParent.RenameTab();

                    this.AddCollectionControlPanel(childControlPanelScada, tabParent, cPanelChild);                    
                }

                RenameFolderCollection(child);
            }

        }

        public void OkRenameFolder(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox t = (TextBox)sender;
                StackPanel panel = (StackPanel)t.Parent;

                Label l = (Label)t.Tag;

                TreeViewItem i = (TreeViewItem)panel.Parent;

                FolderScada fs = (FolderScada)i.Tag; // Ссылка на FolderScada в Tag присваивается в файле DialogWindowCreateFolder.xaml.cs при создании новой папки

                char[] InvalidChars = { '"', '/', '\\', '<', '>', '?', '*', '|', ':' };

                if (t.Text.IndexOfAny(InvalidChars) != -1)
                {                   
                    Popup ErrorPopup = new Popup();
                    ErrorPopup.PlacementTarget = i;
                    ErrorPopup.Placement = PlacementMode.Bottom;
                    ErrorPopup.AllowsTransparency = true;
                    ErrorPopup.PopupAnimation = PopupAnimation.Slide;
                    ErrorPopup.StaysOpen = false;

                    Border border = new Border();
                    border.BorderThickness = new Thickness(2);
                    border.Background = new SolidColorBrush(Colors.White);
                    border.BorderBrush = new SolidColorBrush(Colors.Red);

                    TextBlock text = new TextBlock();
                    text.TextWrapping = TextWrapping.Wrap;
                    text.Foreground = new SolidColorBrush(Colors.Black);
                    text.FontSize = 16;
                    text.Text = "Имя папки не должно содержать символы: < > | \" / \\ * : ?";

                    Binding BindWidth = new Binding();
                    BindWidth.Source = BrowseProject;
                    BindWidth.Path = new PropertyPath("ActualWidth");
                    BindWidth.Mode = BindingMode.OneTime;

                    ErrorPopup.SetBinding(Popup.WidthProperty, BindWidth);

                    border.Child = text;

                    ErrorPopup.Child = border;
                    ErrorPopup.IsOpen = true;

                    e.Handled = true;

                    return;
                }

                if (string.IsNullOrWhiteSpace(t.Text))
                {                    
                    Popup ErrorPopup = new Popup();
                    ErrorPopup.PlacementTarget = i;
                    ErrorPopup.Placement = PlacementMode.Bottom;
                    ErrorPopup.AllowsTransparency = true;
                    ErrorPopup.PopupAnimation = PopupAnimation.Slide;
                    ErrorPopup.StaysOpen = false;

                    Border border = new Border();
                    border.BorderThickness = new Thickness(2);
                    border.Background = new SolidColorBrush(Colors.White);
                    border.BorderBrush = new SolidColorBrush(Colors.Red);

                    TextBlock text = new TextBlock();
                    text.TextWrapping = TextWrapping.Wrap;
                    text.Foreground = new SolidColorBrush(Colors.Black);
                    text.FontSize = 16;
                    text.Text = "Имя папки не должно содержать только пробелы или быть пустой строкой.";

                    Binding BindWidth = new Binding();
                    BindWidth.Source = BrowseProject;
                    BindWidth.Path = new PropertyPath("ActualWidth");
                    BindWidth.Mode = BindingMode.OneTime;

                    ErrorPopup.SetBinding(Popup.WidthProperty, BindWidth);

                    border.Child = text;

                    ErrorPopup.Child = border;
                    ErrorPopup.IsOpen = true;

                    e.Handled = true;

                    return;
                }
              
                int index = fs.Path.LastIndexOf(fs.Name);
                string path = fs.Path.Remove(index);
                path = path + t.Text;

                if(fs.Path == path)
                {
                    panel.Children.Remove(t);
                    panel.Children.Add(l);

                    e.Handled = true;

                    return;
                }

                //Если папка с таким именем существует, переименование не происходит.  
                if (Directory.Exists(path))
                {
                    MessageBox.Show("Внимание! папка " + path + " уже существует.", "Ошибка переименования", MessageBoxButton.OK, MessageBoxImage.Error);
              
                    e.Handled = true;
                    return;
                }
                else Directory.Move(fs.Path, path); // Переименование

                l.Content = t.Text;
                panel.Children.Remove(t);
                panel.Children.Add(l);

                ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionFolderScada.Remove(fs.Path);

                fs.Name = t.Text;
                fs.Path = path;

                ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionFolderScada.Add(fs.Path, fs);

                AlphanumComparator a = new AlphanumComparator();
                a.Name = (string)l.Content;

                RenameFolderCollection(i);
                panel.Tag = a;

                if (fs.Attachments > 0)
                {
                    TreeViewItem parentFolder = fs.ParentItem;

                    parentFolder.Items.SortDescriptions.Clear();
                    parentFolder.Items.SortDescriptions.Add(new SortDescription("ContextMenu.Tag", ListSortDirection.Ascending));
                    parentFolder.Items.SortDescriptions.Add(new SortDescription("Header.Tag", ListSortDirection.Ascending));
                }
                else
                {
                    BrowseProject.Items.SortDescriptions.Clear();
                    BrowseProject.Items.SortDescriptions.Add(new SortDescription("ContextMenu.Tag", ListSortDirection.Ascending));
                    BrowseProject.Items.SortDescriptions.Add(new SortDescription("Header.Tag", ListSortDirection.Ascending));
                }
               
                e.Handled = true;
            }
        }

        public void RenamePage(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2)
            {
                TreeViewItem i = (TreeViewItem)sender;
                StackPanel panel = (StackPanel)i.Header;
                Label l = (Label)panel.Children[1];
                TextBox t = (TextBox)l.Tag;
                t.Tag = l;
                panel.Children.Remove(l);
                panel.Children.Add(t);
                t.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    t.Focus();
                }));
                t.SelectAll();

                e.Handled = true;
            }
        }

        public void OkRenamePage(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox t = (TextBox)sender;
                StackPanel panel = (StackPanel)t.Parent;

                Label l = (Label)t.Tag;

                TreeViewItem i = (TreeViewItem)panel.Parent;

                PageScada ps = (PageScada)i.Tag;

                char[] InvalidChars = { '"', '/', '\\', '<', '>', '?', '*', '|', ':' };

                if (t.Text.IndexOfAny(InvalidChars) != -1)
                {
                    Popup ErrorPopup = new Popup();
                    ErrorPopup.PlacementTarget = i;
                    ErrorPopup.Placement = PlacementMode.Bottom;
                    ErrorPopup.AllowsTransparency = true;
                    ErrorPopup.PopupAnimation = PopupAnimation.Slide;
                    ErrorPopup.StaysOpen = false;

                    Border border = new Border();
                    border.BorderThickness = new Thickness(2);
                    border.Background = new SolidColorBrush(Colors.White);
                    border.BorderBrush = new SolidColorBrush(Colors.Red);

                    TextBlock text = new TextBlock();
                    text.TextWrapping = TextWrapping.Wrap;
                    text.Foreground = new SolidColorBrush(Colors.Black);
                    text.FontSize = 16;
                    text.Text = "Имя страницы не должно содержать символы: < > | \" / \\ * : ?";

                    Binding BindWidth = new Binding();
                    BindWidth.Source = BrowseProject;
                    BindWidth.Path = new PropertyPath("ActualWidth");
                    BindWidth.Mode = BindingMode.OneTime;

                    ErrorPopup.SetBinding(Popup.WidthProperty, BindWidth);

                    border.Child = text;

                    ErrorPopup.Child = border;
                    ErrorPopup.IsOpen = true;

                    e.Handled = true;

                    return;
                }

                if (string.IsNullOrWhiteSpace(t.Text))
                {                   
                    Popup ErrorPopup = new Popup();
                    ErrorPopup.PlacementTarget = i;
                    ErrorPopup.Placement = PlacementMode.Bottom;
                    ErrorPopup.AllowsTransparency = true;
                    ErrorPopup.PopupAnimation = PopupAnimation.Slide;
                    ErrorPopup.StaysOpen = false;

                    Border border = new Border();
                    border.BorderThickness = new Thickness(2);
                    border.Background = new SolidColorBrush(Colors.White);
                    border.BorderBrush = new SolidColorBrush(Colors.Red);

                    TextBlock text = new TextBlock();
                    text.TextWrapping = TextWrapping.Wrap;
                    text.Foreground = new SolidColorBrush(Colors.Black);
                    text.FontSize = 16;
                    text.Text = "Имя страницы не должно содержать только пробелы или быть пустой строкой.";

                    Binding BindWidth = new Binding();
                    BindWidth.Source = BrowseProject;
                    BindWidth.Path = new PropertyPath("ActualWidth");
                    BindWidth.Mode = BindingMode.OneTime;

                    ErrorPopup.SetBinding(Popup.WidthProperty, BindWidth);

                    border.Child = text;

                    ErrorPopup.Child = border;
                    ErrorPopup.IsOpen = true;

                    e.Handled = true;

                    return;
                }

                if (!t.Text.EndsWith(".pg"))
                {
                    t.Text = t.Text + ".pg";
                }

                Page page = ((AppWPF)Application.Current).CollectionPage[ps.Path];

                int index = ps.Path.LastIndexOf(ps.Name);
                string path = ps.Path.Remove(index);
                path = path + t.Text;

                if (ps.Path == path)
                {
                    panel.Children.Remove(t);
                    panel.Children.Add(l);

                    e.Handled = true;

                    return;
                }

                //Если файл страницы с таким именем существует, переименование не происходит.  
                if (File.Exists(path))
                {
                    MessageBox.Show("Внимание! Страница " + path + " уже существует.", "Ошибка переименования", MessageBoxButton.OK, MessageBoxImage.Error);      

                    e.Handled = true;

                    return;          
                }
                else File.Move(ps.Path, path); // Переименование

                l.Content = t.Text;

                AlphanumComparator a = new AlphanumComparator();
                a.Name = (string)l.Content;
              
                panel.Children.Remove(t);
                panel.Children.Add(l);
                panel.Tag = a;

                TabItemParent tabParent = ((AppWPF)Application.Current).CollectionTabItemParent[ps.Path];

                this.RemoveCollectionPage(ps);
                                            
                ps.Name = t.Text;
                ps.Path = path;
                
                tabParent.RenameTab();

                if (ps.Attachments > 0)
                {
                    TreeViewItem parentFolder = ps.ParentItem;

                    parentFolder.Items.SortDescriptions.Clear();
                    parentFolder.Items.SortDescriptions.Add(new SortDescription("ContextMenu.Tag", ListSortDirection.Ascending));
                    parentFolder.Items.SortDescriptions.Add(new SortDescription("Header.Tag", ListSortDirection.Ascending));
                }
                else 
                {
                    BrowseProject.Items.SortDescriptions.Clear();
                    BrowseProject.Items.SortDescriptions.Add(new SortDescription("ContextMenu.Tag", ListSortDirection.Ascending));
                    BrowseProject.Items.SortDescriptions.Add(new SortDescription("Header.Tag", ListSortDirection.Ascending));
                }

                this.AddCollectionPage(ps, tabParent, page);
                
                e.Handled = true;
            }
        }

        public void ContextMenuCreatePage(object sender, RoutedEventArgs e)
        {
            DialogWindowContextMenuCreatePage DialogWindow = new DialogWindowContextMenuCreatePage();
            DialogWindow.Owner = this;
            DialogWindow.Tag = ((MenuItem)sender).Tag; 
            DialogWindow.ShowDialog();

            e.Handled = true;
        }

        public void ContextMenuCreateFolder(object sender, RoutedEventArgs e)
        {
            DialogWindowContextMenuCreateFolder DialogWindow = new DialogWindowContextMenuCreateFolder();
            DialogWindow.Owner = this;
            DialogWindow.Tag = ((MenuItem)sender).Tag; // Передаем объект  TreeViewItem.Tag.FolderScada для вложенненой gfgrb
            DialogWindow.ShowDialog();

            e.Handled = true;
        }

        public void RenameFolderItemInsert(TreeViewItem parentItem, bool InItem = true)
        {
            if (!InItem)
            {
                if (CurrentItem.Tag is FolderScada)
                {
                    FolderScada CutFolderScada = CurrentItem.Tag as FolderScada;
                    int index = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject.LastIndexOf(((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.ProjectName);
                    string pathProject = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject.Remove(index);
                   
                    TreeViewItem parentItemFolder = CutFolderScada.ParentItem;
                    parentItemFolder.Items.Remove(CurrentItem);

                    ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionFolderScada.Remove(CutFolderScada.Path);

                    CutFolderScada.Path = pathProject + CutFolderScada.Name;
                    CutFolderScada.Attachments = 0;
                    CutFolderScada.AttachmentFolder = null;
                    CutFolderScada.ParentItem = null;
                    
                    FolderScada parentFolderScada = parentItemFolder.Tag as FolderScada;
                    parentFolderScada.ChildItem.Remove(CurrentItem);

                    ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionFolderScada.Add(CutFolderScada.Path, CutFolderScada);

                    BrowseProject.Items.Add(CurrentItem);
                    RenameFolderItemInsert(CurrentItem);

                    return;
                }              
            }

            ItemCollection collectionItem = parentItem.Items;

            FolderScada parentFolder;

            if (parentItem.Tag is FolderScada)
            {
                parentFolder = (FolderScada)parentItem.Tag;
            }
            else parentFolder = null;

            foreach (TreeViewItem child in collectionItem)
            {
                object objectChild = child.Tag;

                if (objectChild is FolderScada)
                {
                    FolderScada childFolderScada = (FolderScada)objectChild;

                    ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionFolderScada.Remove(childFolderScada.Path);

                    childFolderScada.AttachmentFolder = parentFolder.Path;
                    childFolderScada.Path = parentFolder.Path + "\\" + childFolderScada.Name;
                    childFolderScada.Attachments = parentFolder.Attachments + 1;

                    ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionFolderScada.Add(childFolderScada.Path, childFolderScada);
                }

                if (objectChild is PageScada)
                {
                    PageScada childPageScada = (PageScada)objectChild;
                    Page pageChild = ((AppWPF)Application.Current).CollectionPage[childPageScada.Path];

                    TabItemParent tabParent = ((AppWPF)Application.Current).CollectionTabItemParent[childPageScada.Path];

                    this.RemoveCollectionPage(childPageScada);

                    childPageScada.Path = parentFolder.Path + "\\" + childPageScada.Name;
                    childPageScada.Attachments = parentFolder.Attachments + 1;
                    childPageScada.AttachmentFolder = parentFolder.Path;

                    tabParent.RenameTab();

                    this.AddCollectionPage(childPageScada, tabParent, pageChild);
                }

                if (objectChild is ControlPanelScada)
                {
                    ControlPanelScada childControlPanelScada = (ControlPanelScada)objectChild;
                    ControlPanel childPanelControl = ((AppWPF)Application.Current).CollectionControlPanel[childControlPanelScada.Path];

                    TabItemParent tabParent = ((AppWPF)Application.Current).CollectionTabItemParent[childControlPanelScada.Path];

                    this.RemoveCollectionControlPanel(childControlPanelScada);

                    childControlPanelScada.Path = parentFolder.Path + "\\" + childControlPanelScada.Name;
                    childControlPanelScada.Attachments = parentFolder.Attachments + 1;
                    childControlPanelScada.AttachmentFolder = parentFolder.Path;

                    tabParent.RenameTab();

                    this.AddCollectionControlPanel(childControlPanelScada, tabParent, childPanelControl);
                }

                RenameFolderItemInsert(child);
            }

        }

        public void InsertItem(object sender, RoutedEventArgs e)
        {
            ItemScada InItem = ((MenuItem)sender).Tag as ItemScada;
            ItemScada CutItem = CurrentItem.Tag as ItemScada;

            if (InItem == null)
            {
                if (IsCopy)
                {
                    if (CurrentItem.Tag is FolderScada)
                    {
                        FolderScada fs = CurrentItem.Tag as FolderScada;

                        int index = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject.LastIndexOf(((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.ProjectName);
                        string nameFolder = "Копия - " + fs.Name;
                        string path = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject.Remove(index) + nameFolder;

                        if (Directory.Exists(path)) nameFolder = RenameCopy(1);

                        path = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject.Remove(index) + nameFolder;

                        CopyDir(((FolderScada)currentItem.Tag).Path, path);

                        FolderScada copyfs = new FolderScada();
                        copyfs.AttachmentFolder = null;
                        copyfs.Attachments = 0;
                        copyfs.Name = nameFolder;
                        copyfs.ParentItem = null;
                        copyfs.Path = path;
                        copyfs.IsExpand = fs.IsExpand;

                        StackPanel panelFolder = new StackPanel();
                        panelFolder.Orientation = System.Windows.Controls.Orientation.Horizontal;

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

                        TextBox tbFolder = new TextBox();
                        tbFolder.KeyDown += OkRenameFolder;
                        tbFolder.Text = copyfs.Name;

                        Label lNameFolder = new Label();
                        lNameFolder.Content = tbFolder.Text;
                        lNameFolder.Tag = tbFolder;

                        AlphanumComparator a = new AlphanumComparator();
                        a.Name = (string)lNameFolder.Content;

                        panelFolder.Children.Add(imageFolder);
                        panelFolder.Children.Add(lNameFolder);
                        panelFolder.Tag = a;

                        TreeViewItem copyItem = new TreeViewItem();
                        copyItem.Collapsed += Collapsed;
                        copyItem.Expanded += Expanded;
                        copyItem.Tag = copyfs;
                        copyItem.KeyDown += RenameFolder;
                        copyItem.Header = panelFolder;

                        copyfs.TreeItem = copyItem;

                        MenuItem MenuItemCreate = new MenuItem();
                        MenuItemCreate.Header = "Добавить";

                        MenuItem MenuItemCreateControlPanel = new MenuItem();
                        MenuItemCreateControlPanel.Click += ContextMenuCreateControlPanel;
                        MenuItemCreateControlPanel.Icon = imageControlPanel;
                        MenuItemCreateControlPanel.Header = "Щит управления";
                        MenuItemCreateControlPanel.Tag = copyItem;

                        MenuItem MenuItemCreateFolder = new MenuItem();
                        MenuItemCreateFolder.Icon = imageMenuItemCreateFolder;
                        MenuItemCreateFolder.Header = "Папку";
                        MenuItemCreateFolder.Tag = copyItem; // Нужен для индефикации в какую папку сохранять при создании вложенной папки
                        MenuItemCreateFolder.Click += ContextMenuCreateFolder;

                        MenuItem MenuItemCreatePage = new MenuItem();
                        MenuItemCreatePage.Icon = imageMenuItemCreatePage;
                        MenuItemCreatePage.Header = "Страницу";
                        MenuItemCreatePage.Tag = copyItem;  // Нужен для индефикации в какую папку сохранять при создании вложенной страницы
                        MenuItemCreatePage.Click += ContextMenuCreatePage;

                        MenuItem menuItemDeleteFolder = new MenuItem();
                        menuItemDeleteFolder.Header = "Удалить";
                        menuItemDeleteFolder.Icon = ImageDelete;
                        menuItemDeleteFolder.Tag = copyfs;
                        menuItemDeleteFolder.Click += DeleteItem;

                        MenuItem menuItemCopyFolder = new MenuItem();
                        menuItemCopyFolder.Header = "Копировать";
                        menuItemCopyFolder.Icon = ImageCopy;
                        menuItemCopyFolder.Tag = copyfs;
                        menuItemCopyFolder.Click += CopyItem;

                        Window MainWindow = ((AppWPF)System.Windows.Application.Current).MainWindow;

                        MenuItem menuItemCutFolder = new MenuItem();
                        menuItemCutFolder.Header = "Вырезать";
                        menuItemCutFolder.Icon = imageCut;
                        menuItemCutFolder.Tag = copyfs;
                        menuItemCutFolder.Click += ((MainWindow)MainWindow).CutItem;

                        Binding BindingInsert = new Binding();
                        BindingInsert.Source = this;
                        BindingInsert.Path = new PropertyPath("IsBindingInsert");
                        BindingInsert.Mode = BindingMode.OneWay;

                        MenuItem menuItemInsert = new MenuItem();
                        menuItemInsert.Header = "Вставить";
                        menuItemInsert.Tag = copyfs;
                        menuItemInsert.SetBinding(MenuItem.IsEnabledProperty, BindingInsert);
                        menuItemInsert.Icon = imageInsert;
                        menuItemInsert.Click += InsertItem;

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

                        copyItem.ContextMenu = ContextMenuFolder;

                        ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionFolderScada.Add(copyfs.Path, copyfs);

                        BrowseProject.Items.Add(copyItem);

                        CopyItemBrowse(currentItem, copyItem);

                        BrowseProject.Items.SortDescriptions.Clear();
                        BrowseProject.Items.SortDescriptions.Add(new SortDescription("ContextMenu.Tag", ListSortDirection.Ascending));
                        BrowseProject.Items.SortDescriptions.Add(new SortDescription("Header.Tag", ListSortDirection.Ascending));                      
                    }
                    else if (CurrentItem.Tag is PageScada)
                    {
                        PageScada ps = CurrentItem.Tag as PageScada;

                        int index = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject.LastIndexOf(((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.ProjectName);
                        string namePage = "Копия - " + ps.Name;
                        string path = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject.Remove(index) + namePage;

                        if (File.Exists(path)) namePage = RenameCopyPage(1);

                        path = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject.Remove(index) + namePage;
                       
                        PageScada copyps = new PageScada();
                        copyps.AttachmentFolder = null;
                        copyps.Attachments = 0;
                        copyps.Name = namePage;
                        copyps.ParentItem = null;
                        copyps.Path = path;

                        StackPanel panelPage = new StackPanel();
                        panelPage.Orientation = System.Windows.Controls.Orientation.Horizontal;

                        Image imagePage = new Image();
                        imagePage.Source = new BitmapImage(new Uri("Images/Page16.png", UriKind.Relative));

                        Image imageCut = new Image();
                        imageCut.Source = new BitmapImage(new Uri("Images/Cut16.png", UriKind.Relative));

                        Image imageDelete = new Image();
                        imageDelete.Source = new BitmapImage(new Uri("Images/PageDelete16.png", UriKind.Relative));

                        Image imageCopy = new Image();
                        imageCopy.Source = new BitmapImage(new Uri("Images/CopyPage16.png", UriKind.Relative));

                        MenuItem menuItemCopyPage = new MenuItem();
                        menuItemCopyPage.Header = "Копировать";
                        menuItemCopyPage.Icon = imageCopy;
                        menuItemCopyPage.Tag = copyps;
                        menuItemCopyPage.Click += CopyItem;

                        Window MainWindow = ((AppWPF)System.Windows.Application.Current).MainWindow;

                        MenuItem menuItemCutPage = new MenuItem();
                        menuItemCutPage.Header = "Вырезать";
                        menuItemCutPage.Icon = imageCut;
                        menuItemCutPage.Tag = copyps;
                        menuItemCutPage.Click += ((MainWindow)MainWindow).CutItem;

                        MenuItem menuItemDeletePage = new MenuItem();
                        menuItemDeletePage.Header = "Удалить";
                        menuItemDeletePage.Icon = imageDelete;
                        menuItemDeletePage.Tag = copyps;
                        menuItemDeletePage.Click += DeleteItem;

                        TextBox tbPage = new TextBox();
                        tbPage.KeyDown += OkRenamePage;
                        tbPage.Text = copyps.Name;

                        Label lNamePage = new Label();
                        lNamePage.Content = tbPage.Text;
                        lNamePage.Tag = tbPage;

                        ContextMenu contextMenuPage = new ContextMenu();
                        contextMenuPage.Items.Add(menuItemCopyPage);
                        contextMenuPage.Items.Add(menuItemCutPage);
                        contextMenuPage.Items.Add(menuItemDeletePage);
                        contextMenuPage.Tag = "PageScada";

                        AlphanumComparator a = new AlphanumComparator();
                        a.Name = (string)lNamePage.Content;

                        panelPage.Children.Add(imagePage);
                        panelPage.Children.Add(lNamePage);
                        panelPage.Tag = a;

                        TreeViewItem copyItemPage = new TreeViewItem();
                        copyItemPage.MouseDoubleClick += OpenBrowsePage;
                        copyItemPage.Tag = copyps;
                        copyItemPage.KeyDown += RenamePage;
                        copyItemPage.Header = panelPage;
                        copyItemPage.ContextMenu = contextMenuPage;

                        copyps.TreeItem = copyItemPage;

                        Page curPage = ((AppWPF)Application.Current).CollectionPage[ps.Path];
                        Page copyPage = new Page();

                        foreach (PipeSer pipeSer in curPage.CollectionPipe)
                        {
                            PipeSer copyPipeSer;

                            using (MemoryStream TempStream = new MemoryStream())
                            {
                                BinaryFormatter serializer = new BinaryFormatter();

                                serializer.Serialize(TempStream, pipeSer);

                                using (MemoryStream TempStreamRead = new MemoryStream(TempStream.ToArray()))
                                {
                                    BinaryFormatter deserializer = new BinaryFormatter();

                                    copyPipeSer = (PipeSer)deserializer.Deserialize(TempStreamRead);

                                    copyPage.CollectionPipe.Add(copyPipeSer);
                                }
                            }
                        }

                        this.AddCollectionPageCopy(copyps, copyPage);

                        TabItemPage tabItemPage = new TabItemPage(copyps);

                        BrowseProject.Items.Add(copyItemPage);

                        BrowseProject.Items.SortDescriptions.Clear();
                        BrowseProject.Items.SortDescriptions.Add(new SortDescription("ContextMenu.Tag", ListSortDirection.Ascending));
                        BrowseProject.Items.SortDescriptions.Add(new SortDescription("Header.Tag", ListSortDirection.Ascending));

                        using (FileStream fs = File.Create((path)))
                        {
                            BinaryFormatter serializer = new BinaryFormatter();
                            serializer.Serialize(fs, copyPage);
                        }
                    }
                    else if (CurrentItem.Tag is ControlPanelScada)
                    {
                        ControlPanelScada cps = CurrentItem.Tag as ControlPanelScada;

                        int index = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject.LastIndexOf(((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.ProjectName);
                        string nameControlPanel = "Копия - " + cps.Name;
                        string path = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject.Remove(index) + nameControlPanel;

                        if (File.Exists(path)) nameControlPanel = RenameCopyControlPanel(1);

                        path = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject.Remove(index) + nameControlPanel;

                        ControlPanelScada copycps = new ControlPanelScada();
                        copycps.AttachmentFolder = null;
                        copycps.Attachments = 0;
                        copycps.Name = nameControlPanel;
                        copycps.ParentItem = null;
                        copycps.Path = path;

                        StackPanel panel = new StackPanel();
                        panel.Orientation = System.Windows.Controls.Orientation.Horizontal;

                        Image imageControlPanel = new Image();
                        imageControlPanel.Source = new BitmapImage(new Uri("Images/ControlPanel16.png", UriKind.Relative));

                        Image imageCut = new Image();
                        imageCut.Source = new BitmapImage(new Uri("Images/Cut16.png", UriKind.Relative));

                        Image imageDelete = new Image();
                        imageDelete.Source = new BitmapImage(new Uri("Images/PageDelete16.png", UriKind.Relative));

                        Image imageCopy = new Image();
                        imageCopy.Source = new BitmapImage(new Uri("Images/CopyPage16.png", UriKind.Relative));

                        MenuItem menuItemCopyControlPanel = new MenuItem();
                        menuItemCopyControlPanel.Header = "Копировать";
                        menuItemCopyControlPanel.Icon = imageCopy;
                        menuItemCopyControlPanel.Tag = copycps;
                        menuItemCopyControlPanel.Click += CopyItem;

                        Window MainWindow = ((AppWPF)System.Windows.Application.Current).MainWindow;

                        MenuItem menuItemCutControlPanel = new MenuItem();
                        menuItemCutControlPanel.Header = "Вырезать";
                        menuItemCutControlPanel.Icon = imageCut;
                        menuItemCutControlPanel.Tag = copycps;
                        menuItemCutControlPanel.Click += ((MainWindow)MainWindow).CutItem;

                        MenuItem menuItemDeleteControlPanel = new MenuItem();
                        menuItemDeleteControlPanel.Header = "Удалить";
                        menuItemDeleteControlPanel.Icon = imageDelete;
                        menuItemDeleteControlPanel.Tag = copycps;
                        menuItemDeleteControlPanel.Click += DeleteItem;

                        TextBox tbControlPanel = new TextBox();
                        tbControlPanel.KeyDown += OkRenameControlPanel;
                        tbControlPanel.Text = copycps.Name;

                        Label lNameControlPanel = new Label();
                        lNameControlPanel.Content = tbControlPanel.Text;
                        lNameControlPanel.Tag = tbControlPanel;

                        ContextMenu contextMenuControlPanel = new ContextMenu();
                        contextMenuControlPanel.Items.Add(menuItemCopyControlPanel);
                        contextMenuControlPanel.Items.Add(menuItemCutControlPanel);
                        contextMenuControlPanel.Items.Add(menuItemDeleteControlPanel);
                        contextMenuControlPanel.Tag = "X";

                        AlphanumComparator a = new AlphanumComparator();
                        a.Name = (string)lNameControlPanel.Content;

                        panel.Children.Add(imageControlPanel);
                        panel.Children.Add(lNameControlPanel);
                        panel.Tag = a;

                        TreeViewItem copyItemControlPanel = new TreeViewItem();
                        copyItemControlPanel.Tag = copycps;
                        copyItemControlPanel.KeyDown += RenameControlPanel;
                        copyItemControlPanel.Header = panel;
                        copyItemControlPanel.ContextMenu = contextMenuControlPanel;

                        copycps.TreeItem = copyItemControlPanel;

                        ControlPanel curControlPanel = ((AppWPF)Application.Current).CollectionControlPanel[cps.Path];
                        ControlPanel copyControlPanel = new ControlPanel();

                        this.AddCollectionControlPanelCopy(copycps, copyControlPanel);

                        TabItemControlPanel tabItemControlPanel = new TabItemControlPanel(copycps);

                        BrowseProject.Items.Add(copyItemControlPanel);
                                                                  
                        BrowseProject.Items.SortDescriptions.Clear();
                        BrowseProject.Items.SortDescriptions.Add(new SortDescription("ContextMenu.Tag", ListSortDirection.Ascending));
                        BrowseProject.Items.SortDescriptions.Add(new SortDescription("Header.Tag", ListSortDirection.Ascending));

                        using (FileStream fs = File.Create((path)))
                        {
                            BinaryFormatter serializer = new BinaryFormatter();
                            serializer.Serialize(fs, copyControlPanel);
                        }                        
                    }

                    e.Handled = true;
                    return;
                }
            }

            if (InItem is FolderScada)
            {
                if(IsCopy)
                {
                    if (CutItem is FolderScada)
                    {
                        FolderScada fs = CurrentItem.Tag as FolderScada;
                        FolderScada infs = InItem as FolderScada;

                        if (infs.TreeItem == currentItem)
                        {
                            e.Handled = true;
                            return;
                        }

                        string nameFolder = "Копия - " + fs.Name;
                        string path = infs.Path + "\\" + nameFolder;

                        if (Directory.Exists(path)) nameFolder = RenameCopyInItem(1, infs.Path);

                        path = infs.Path + "\\" + nameFolder;

                        CopyDir(((FolderScada)currentItem.Tag).Path, path);

                        FolderScada copyfs = new FolderScada();
                        copyfs.AttachmentFolder = infs.Path;
                        copyfs.Attachments = infs.Attachments + 1;
                        copyfs.Name = nameFolder;
                        copyfs.ParentItem = infs.TreeItem;
                        copyfs.Path = path;
                        copyfs.IsExpand = fs.IsExpand;

                        StackPanel panelFolder = new StackPanel();
                        panelFolder.Orientation = System.Windows.Controls.Orientation.Horizontal;

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

                        TextBox tbFolder = new TextBox();
                        tbFolder.KeyDown += OkRenameFolder;
                        tbFolder.Text = copyfs.Name;

                        Label lNameFolder = new Label();
                        lNameFolder.Content = tbFolder.Text;
                        lNameFolder.Tag = tbFolder;

                        AlphanumComparator a = new AlphanumComparator();
                        a.Name = (string)lNameFolder.Content;

                        panelFolder.Children.Add(imageFolder);
                        panelFolder.Children.Add(lNameFolder);
                        panelFolder.Tag = a;

                        TreeViewItem copyItem = new TreeViewItem();
                        copyItem.Collapsed += Collapsed;
                        copyItem.Expanded += Expanded;
                        copyItem.Tag = copyfs;
                        copyItem.KeyDown += RenameFolder;
                        copyItem.Header = panelFolder;

                        copyfs.TreeItem = copyItem;

                        MenuItem MenuItemCreate = new MenuItem();
                        MenuItemCreate.Header = "Добавить";

                        MenuItem MenuItemCreateControlPanel = new MenuItem();
                        MenuItemCreateControlPanel.Click += ContextMenuCreateControlPanel;
                        MenuItemCreateControlPanel.Icon = imageControlPanel;
                        MenuItemCreateControlPanel.Header = "Щит управления";
                        MenuItemCreateControlPanel.Tag = copyItem;

                        MenuItem MenuItemCreateFolder = new MenuItem();
                        MenuItemCreateFolder.Icon = imageMenuItemCreateFolder;
                        MenuItemCreateFolder.Header = "Папку";
                        MenuItemCreateFolder.Tag = copyItem; // Нужен для индефикации в какую папку сохранять при создании вложенной папки
                        MenuItemCreateFolder.Click += ContextMenuCreateFolder;

                        MenuItem MenuItemCreatePage = new MenuItem();
                        MenuItemCreatePage.Icon = imageMenuItemCreatePage;
                        MenuItemCreatePage.Header = "Страницу";
                        MenuItemCreatePage.Tag = copyItem;  // Нужен для индефикации в какую папку сохранять при создании вложенной страницы
                        MenuItemCreatePage.Click += ContextMenuCreatePage;

                        MenuItem menuItemDeleteFolder = new MenuItem();
                        menuItemDeleteFolder.Header = "Удалить";
                        menuItemDeleteFolder.Icon = ImageDelete;
                        menuItemDeleteFolder.Tag = copyfs;
                        menuItemDeleteFolder.Click += DeleteItem;

                        MenuItem menuItemCopyFolder = new MenuItem();
                        menuItemCopyFolder.Header = "Копировать";
                        menuItemCopyFolder.Icon = ImageCopy;
                        menuItemCopyFolder.Tag = copyfs;
                        menuItemCopyFolder.Click += CopyItem;

                        Window MainWindow = ((AppWPF)System.Windows.Application.Current).MainWindow;

                        MenuItem menuItemCutFolder = new MenuItem();
                        menuItemCutFolder.Header = "Вырезать";
                        menuItemCutFolder.Icon = imageCut;
                        menuItemCutFolder.Tag = copyfs;
                        menuItemCutFolder.Click += ((MainWindow)MainWindow).CutItem;

                        Binding BindingInsert = new Binding();
                        BindingInsert.Source = this;
                        BindingInsert.Path = new PropertyPath("IsBindingInsert");
                        BindingInsert.Mode = BindingMode.OneWay;

                        MenuItem menuItemInsert = new MenuItem();
                        menuItemInsert.Header = "Вставить";
                        menuItemInsert.Tag = copyfs;
                        menuItemInsert.SetBinding(MenuItem.IsEnabledProperty, BindingInsert);
                        menuItemInsert.Icon = imageInsert;
                        menuItemInsert.Click += InsertItem;

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

                        copyItem.ContextMenu = ContextMenuFolder;

                        ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionFolderScada.Add(copyfs.Path, copyfs);
                       
                        infs.TreeItem.Items.Add(copyItem);
                        if (infs.ChildItem == null) infs.ChildItem = new List<TreeViewItem>();
                        infs.ChildItem.Add(copyItem);

                        CopyItemBrowse(currentItem, copyItem);

                        infs.TreeItem.Items.SortDescriptions.Clear();
                        infs.TreeItem.Items.SortDescriptions.Add(new SortDescription("ContextMenu.Tag", ListSortDirection.Ascending));
                        infs.TreeItem.Items.SortDescriptions.Add(new SortDescription("Header.Tag", ListSortDirection.Ascending));                       
                    }
                    else if (CutItem is PageScada)
                    {
                        PageScada ps = CurrentItem.Tag as PageScada;
                        FolderScada infs = InItem as FolderScada;

                        string namePage = "Копия - " + ps.Name;
                        string path = infs.Path + "\\" + namePage;

                        if (File.Exists(path)) namePage = RenameCopyInItemPage(1, infs.Path);

                        path = infs.Path + "\\" + namePage;

                        File.Copy(ps.Path, path);

                        PageScada copyps = new PageScada();
                        copyps.AttachmentFolder = infs.Path;
                        copyps.Attachments = infs.Attachments + 1;
                        copyps.Name = namePage;
                        copyps.ParentItem = infs.TreeItem;
                        copyps.Path = path;

                        StackPanel panelPage = new StackPanel();
                        panelPage.Orientation = System.Windows.Controls.Orientation.Horizontal;

                        Image imagePage = new Image();
                        imagePage.Source = new BitmapImage(new Uri("Images/Page16.png", UriKind.Relative));

                        Image imageCut = new Image();
                        imageCut.Source = new BitmapImage(new Uri("Images/Cut16.png", UriKind.Relative));

                        Image imageDelete = new Image();
                        imageDelete.Source = new BitmapImage(new Uri("Images/PageDelete16.png", UriKind.Relative));

                        Image imageCopy = new Image();
                        imageCopy.Source = new BitmapImage(new Uri("Images/CopyPage16.png", UriKind.Relative));

                        MenuItem menuItemCopyPage = new MenuItem();
                        menuItemCopyPage.Header = "Копировать";
                        menuItemCopyPage.Icon = imageCopy;
                        menuItemCopyPage.Tag = copyps;
                        menuItemCopyPage.Click += CopyItem;

                        Window MainWindow = ((AppWPF)System.Windows.Application.Current).MainWindow;

                        MenuItem menuItemCutPage = new MenuItem();
                        menuItemCutPage.Header = "Вырезать";
                        menuItemCutPage.Icon = imageCut;
                        menuItemCutPage.Tag = copyps;
                        menuItemCutPage.Click += ((MainWindow)MainWindow).CutItem;

                        MenuItem menuItemDeletePage = new MenuItem();
                        menuItemDeletePage.Header = "Удалить";
                        menuItemDeletePage.Icon = imageDelete;
                        menuItemDeletePage.Tag = copyps;
                        menuItemDeletePage.Click += DeleteItem;

                        TextBox tbPage = new TextBox();
                        tbPage.KeyDown += OkRenamePage;
                        tbPage.Text = copyps.Name;

                        Label lNamePage = new Label();
                        lNamePage.Content = tbPage.Text;
                        lNamePage.Tag = tbPage;

                        ContextMenu contextMenuPage = new ContextMenu();
                        contextMenuPage.Items.Add(menuItemCopyPage);
                        contextMenuPage.Items.Add(menuItemCutPage);
                        contextMenuPage.Items.Add(menuItemDeletePage);
                        contextMenuPage.Tag = "PageScada";

                        AlphanumComparator a = new AlphanumComparator();
                        a.Name = (string)lNamePage.Content;

                        panelPage.Children.Add(imagePage);
                        panelPage.Children.Add(lNamePage);
                        panelPage.Tag = a;

                        TreeViewItem copyItemPage = new TreeViewItem();
                        copyItemPage.MouseDoubleClick += OpenBrowsePage;
                        copyItemPage.Tag = copyps;
                        copyItemPage.KeyDown += RenamePage;
                        copyItemPage.Header = panelPage;
                        copyItemPage.ContextMenu = contextMenuPage;

                        copyps.TreeItem = copyItemPage;
                      
                        Page copyPage = new Page();

                        Page curPage = ((AppWPF)Application.Current).CollectionPage[ps.Path];

                        foreach (PipeSer pipeSer in curPage.CollectionPipe)
                        {
                            PipeSer copyPipeSer;

                            using (MemoryStream TempStream = new MemoryStream())
                            {
                                BinaryFormatter serializer = new BinaryFormatter();

                                serializer.Serialize(TempStream, pipeSer);

                                using (MemoryStream TempStreamRead = new MemoryStream(TempStream.ToArray()))
                                {
                                    BinaryFormatter deserializer = new BinaryFormatter();

                                    copyPipeSer = (PipeSer)deserializer.Deserialize(TempStreamRead);

                                    copyPage.CollectionPipe.Add(copyPipeSer);
                                }
                            }
                        }

                        this.AddCollectionPageCopy(copyps, copyPage);

                        TabItemPage tabItemPage = new TabItemPage(copyps);

                        infs.TreeItem.Items.Add(copyItemPage);
                        if (infs.ChildItem == null) infs.ChildItem = new List<TreeViewItem>();
                        infs.ChildItem.Add(copyItemPage);

                        infs.TreeItem.Items.SortDescriptions.Clear();
                        infs.TreeItem.Items.SortDescriptions.Add(new SortDescription("ContextMenu.Tag", ListSortDirection.Ascending));
                        infs.TreeItem.Items.SortDescriptions.Add(new SortDescription("Header.Tag", ListSortDirection.Ascending));
                    }
                    else if (CutItem is ControlPanelScada)
                    {
                        ControlPanelScada cps = CurrentItem.Tag as ControlPanelScada;
                        FolderScada infs = InItem as FolderScada;

                        string nameControlPanel = "Копия - " + cps.Name;
                        string path = infs.Path + "\\" + nameControlPanel;

                        if (File.Exists(path)) nameControlPanel = RenameCopyInItemControlPanel(1, infs.Path);

                        path = infs.Path + "\\" + nameControlPanel;

                        File.Copy(cps.Path, path);

                        ControlPanelScada copycps = new ControlPanelScada();
                        copycps.AttachmentFolder = infs.Path;
                        copycps.Attachments = infs.Attachments + 1;
                        copycps.Name = nameControlPanel;
                        copycps.ParentItem = infs.TreeItem;
                        copycps.Path = path;

                        StackPanel panel = new StackPanel();
                        panel.Orientation = System.Windows.Controls.Orientation.Horizontal;

                        Image imageControlPanel = new Image();
                        imageControlPanel.Source = new BitmapImage(new Uri("Images/ControlPanel16.png", UriKind.Relative));

                        Image imageCut = new Image();
                        imageCut.Source = new BitmapImage(new Uri("Images/Cut16.png", UriKind.Relative));

                        Image imageDelete = new Image();
                        imageDelete.Source = new BitmapImage(new Uri("Images/PageDelete16.png", UriKind.Relative));

                        Image imageCopy = new Image();
                        imageCopy.Source = new BitmapImage(new Uri("Images/CopyPage16.png", UriKind.Relative));

                        MenuItem menuItemCopyControlPanel = new MenuItem();
                        menuItemCopyControlPanel.Header = "Копировать";
                        menuItemCopyControlPanel.Icon = imageCopy;
                        menuItemCopyControlPanel.Tag = copycps;
                        menuItemCopyControlPanel.Click += CopyItem;

                        Window MainWindow = ((AppWPF)System.Windows.Application.Current).MainWindow;

                        MenuItem menuItemCutControlPanel = new MenuItem();
                        menuItemCutControlPanel.Header = "Вырезать";
                        menuItemCutControlPanel.Icon = imageCut;
                        menuItemCutControlPanel.Tag = copycps;
                        menuItemCutControlPanel.Click += ((MainWindow)MainWindow).CutItem;

                        MenuItem menuItemDeleteControlPanel = new MenuItem();
                        menuItemDeleteControlPanel.Header = "Удалить";
                        menuItemDeleteControlPanel.Icon = imageDelete;
                        menuItemDeleteControlPanel.Tag = copycps;
                        menuItemDeleteControlPanel.Click += DeleteItem;

                        TextBox tbControlPanel = new TextBox();
                        tbControlPanel.KeyDown += OkRenameControlPanel;
                        tbControlPanel.Text = copycps.Name;

                        Label lNameControlPanel = new Label();
                        lNameControlPanel.Content = tbControlPanel.Text;
                        lNameControlPanel.Tag = tbControlPanel;

                        ContextMenu contextMenuControlPanel = new ContextMenu();
                        contextMenuControlPanel.Items.Add(menuItemCopyControlPanel);
                        contextMenuControlPanel.Items.Add(menuItemCutControlPanel);
                        contextMenuControlPanel.Items.Add(menuItemDeleteControlPanel);
                        contextMenuControlPanel.Tag = "X";

                        AlphanumComparator a = new AlphanumComparator();
                        a.Name = (string)lNameControlPanel.Content;

                        panel.Children.Add(imageControlPanel);
                        panel.Children.Add(lNameControlPanel);
                        panel.Tag = a;

                        TreeViewItem copyItemControlPanel = new TreeViewItem();
                        copyItemControlPanel.Tag = copycps;
                        copyItemControlPanel.KeyDown += RenameControlPanel;
                        copyItemControlPanel.Header = panel;
                        copyItemControlPanel.ContextMenu = contextMenuControlPanel;

                        copycps.TreeItem = copyItemControlPanel;

                        ControlPanel curControlPanel = ((AppWPF)Application.Current).CollectionControlPanel[cps.Path];

                        ControlPanel copyControlPanel = new ControlPanel();

                        this.AddCollectionControlPanelCopy(copycps, copyControlPanel);

                        TabItemControlPanel tabItemControlPanel = new TabItemControlPanel(copycps);
                       
                        infs.TreeItem.Items.Add(copyItemControlPanel);
                        if (infs.ChildItem == null) infs.ChildItem = new List<TreeViewItem>();
                        infs.ChildItem.Add(copyItemControlPanel);

                        infs.TreeItem.Items.SortDescriptions.Clear();
                        infs.TreeItem.Items.SortDescriptions.Add(new SortDescription("ContextMenu.Tag", ListSortDirection.Ascending));
                        infs.TreeItem.Items.SortDescriptions.Add(new SortDescription("Header.Tag", ListSortDirection.Ascending));
                    }

                    e.Handled = true;
                    return;
                }              
            }

            if (InItem == null & CutItem.Attachments == 0)
            {              
                Clipboard.Clear();
                e.Handled = true;
                return;
            }

            if (InItem == null)
            {
                if (CutItem is FolderScada)
                {
                    FolderScada CutFolderScada = CutItem as FolderScada;
                    int index = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject.LastIndexOf(((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.ProjectName);
                    string newPathCut = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject.Remove(index) + CutFolderScada.Name;

                    if (Directory.Exists(newPathCut))
                    {
                        MessageBox.Show("Внимание! папка " + newPathCut + " уже существует.", "Ошибка вставки", MessageBoxButton.OK, MessageBoxImage.Error);

                        e.Handled = true;
                        return;
                    }

                    Directory.Move(CutFolderScada.Path, newPathCut);

                    RenameFolderItemInsert(null, false);

                    BrowseProject.Items.SortDescriptions.Clear();
                    BrowseProject.Items.SortDescriptions.Add(new SortDescription("ContextMenu.Tag", ListSortDirection.Ascending));
                    BrowseProject.Items.SortDescriptions.Add(new SortDescription("Header.Tag", ListSortDirection.Ascending));

                }

                if (CutItem is PageScada)
                {
                    PageScada CutPageScada = CutItem as PageScada;
                    int index = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject.LastIndexOf(((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.ProjectName);
                    string newPathCut = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject.Remove(index) + CutPageScada.Name;

                    if (File.Exists(newPathCut))
                    {
                        MessageBox.Show("Внимание! страница " + newPathCut + " уже существует.", "Ошибка вставки", MessageBoxButton.OK, MessageBoxImage.Error);

                        e.Handled = true;
                        return;
                    }

                    File.Move(CutPageScada.Path, newPathCut);

                    Page pageCut = ((AppWPF)Application.Current).CollectionPage[CutPageScada.Path];

                    TabItemParent tabParent = ((AppWPF)Application.Current).CollectionTabItemParent[CutPageScada.Path];

                    this.RemoveCollectionPage(CutPageScada);

                    TreeViewItem itemPageScada = CutPageScada.TreeItem;
                    TreeViewItem parentItemFolder = CutPageScada.ParentItem;
                    parentItemFolder.Items.Remove(itemPageScada);

                    FolderScada parentFolderScada = parentItemFolder.Tag as FolderScada;

                    parentFolderScada.ChildItem.Remove(CurrentItem);

                    CutPageScada.Path = newPathCut;
                    CutPageScada.Attachments = 0;
                    CutPageScada.AttachmentFolder = null;
                    CutPageScada.ParentItem = null;

                    tabParent.RenameTab();
                    
                    BrowseProject.Items.Add(itemPageScada);

                    BrowseProject.Items.SortDescriptions.Clear();
                    BrowseProject.Items.SortDescriptions.Add(new SortDescription("ContextMenu.Tag", ListSortDirection.Ascending));
                    BrowseProject.Items.SortDescriptions.Add(new SortDescription("Header.Tag", ListSortDirection.Ascending));

                    this.AddCollectionPage(CutPageScada, tabParent, pageCut);
                }

                if (CutItem is ControlPanelScada)
                {
                    ControlPanelScada CutControlPanelScada = CutItem as ControlPanelScada;
                    int index = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject.LastIndexOf(((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.ProjectName);
                    string newPathCut = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject.Remove(index) + CutControlPanelScada.Name;

                    if (File.Exists(newPathCut))
                    {
                        MessageBox.Show("Внимание! Щит управления " + newPathCut + " уже существует.", "Ошибка вставки", MessageBoxButton.OK, MessageBoxImage.Error);

                        e.Handled = true;
                        return;
                    }

                    File.Move(CutControlPanelScada.Path, newPathCut);

                    ControlPanel controlPanelCut = ((AppWPF)Application.Current).CollectionControlPanel[CutControlPanelScada.Path];

                    TabItemParent tabParent = ((AppWPF)Application.Current).CollectionTabItemParent[CutControlPanelScada.Path];

                    this.RemoveCollectionControlPanel(CutControlPanelScada);

                    TreeViewItem itemControlPanelScada = CutControlPanelScada.TreeItem;
                    TreeViewItem parentItemFolder = CutControlPanelScada.ParentItem;
                    parentItemFolder.Items.Remove(itemControlPanelScada);

                    FolderScada parentFolderScada = parentItemFolder.Tag as FolderScada;

                    parentFolderScada.ChildItem.Remove(CurrentItem);

                    CutControlPanelScada.Path = newPathCut;
                    CutControlPanelScada.Attachments = 0;
                    CutControlPanelScada.AttachmentFolder = null;
                    CutControlPanelScada.ParentItem = null;

                    tabParent.RenameTab();

                    BrowseProject.Items.Add(itemControlPanelScada);

                    BrowseProject.Items.SortDescriptions.Clear();
                    BrowseProject.Items.SortDescriptions.Add(new SortDescription("ContextMenu.Tag", ListSortDirection.Ascending));
                    BrowseProject.Items.SortDescriptions.Add(new SortDescription("Header.Tag", ListSortDirection.Ascending));

                    this.AddCollectionControlPanel(CutControlPanelScada, tabParent, controlPanelCut);        
                }

                Clipboard.Clear();
                e.Handled = true;
                return;
            }

            //Проверяем если вырезанный объект вставляется в тот же уровень вложений, очищаем буфер и не вставляем.           
            if (CutItem.ParentItem == InItem.TreeItem)
            {
                Clipboard.Clear();
                e.Handled = true;
                return;
            }

            if (InItem is FolderScada)
            {
                if (CutItem is FolderScada)
                {
                    FolderScada inItemFolderScada = InItem as FolderScada;
                    FolderScada CutFolderScada = CutItem as FolderScada;

                    string newPathCut = inItemFolderScada.Path + "\\" + CutFolderScada.Name;

                    if (Directory.Exists(newPathCut))
                    {
                        MessageBox.Show("Внимание! папка " + newPathCut + " уже существует.", "Ошибка вставки", MessageBoxButton.OK, MessageBoxImage.Error);

                        e.Handled = true;
                        return;
                    }

                    Directory.Move(CutFolderScada.Path, newPathCut);

                    TreeViewItem InItemFolder = inItemFolderScada.TreeItem;
                    TreeViewItem parentItemFolder = CutFolderScada.ParentItem;

                    if (parentItemFolder != null)
                    {
                        parentItemFolder.Items.Remove(CurrentItem);

                        FolderScada parentFolderScada = parentItemFolder.Tag as FolderScada;
                        parentFolderScada.ChildItem.Remove(CurrentItem);
                    }
                    else
                    {
                        BrowseProject.Items.Remove(CurrentItem);
                    }

                    ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionFolderScada.Remove(CutFolderScada.Path);
                    
                    CutFolderScada.Attachments = inItemFolderScada.Attachments + 1;
                    CutFolderScada.AttachmentFolder = inItemFolderScada.Path;
                    CutFolderScada.Path = newPathCut;
                    CutFolderScada.ParentItem = InItemFolder;

                    ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionFolderScada.Add(CutFolderScada.Path, CutFolderScada);

                    if (inItemFolderScada.ChildItem == null)
                    {
                        inItemFolderScada.ChildItem = new List<TreeViewItem>();
                    }
                    
                    inItemFolderScada.ChildItem.Add(CurrentItem);

                    InItemFolder.Items.Add(CurrentItem);
                   
                    RenameFolderItemInsert(CurrentItem);

                    InItemFolder.Items.SortDescriptions.Clear();
                    InItemFolder.Items.SortDescriptions.Add(new SortDescription("ContextMenu.Tag", ListSortDirection.Ascending));
                    InItemFolder.Items.SortDescriptions.Add(new SortDescription("Header.Tag", ListSortDirection.Ascending));                   
                }

                if (CutItem is PageScada)
                {
                    FolderScada inItemFolderScada = InItem as FolderScada;
                    PageScada CutPageScada = CutItem as PageScada;

                    string newPathCut = inItemFolderScada.Path + "\\" + CutPageScada.Name;

                    if (File.Exists(newPathCut))
                    {
                        MessageBox.Show("Внимание! страница " + newPathCut + " уже существует.", "Ошибка вставки", MessageBoxButton.OK, MessageBoxImage.Error);

                        e.Handled = true;
                        return;
                    }

                    File.Move(CutPageScada.Path, newPathCut);

                    Page pageCut = ((AppWPF)Application.Current).CollectionPage[CutPageScada.Path];

                    TabItemParent tabParent = ((AppWPF)Application.Current).CollectionTabItemParent[CutPageScada.Path];

                    this.RemoveCollectionPage(CutPageScada);

                    TreeViewItem parentItemFolder = CutPageScada.ParentItem;
                    TreeViewItem inItemFolder = inItemFolderScada.TreeItem;

                    if (parentItemFolder != null)
                    {
                        parentItemFolder.Items.Remove(CurrentItem);

                        FolderScada parentFolderScada = parentItemFolder.Tag as FolderScada;
                        parentFolderScada.ChildItem.Remove(CurrentItem);
                    }
                    else 
                    {
                        BrowseProject.Items.Remove(CurrentItem);
                    }
                                     
                    CutPageScada.Path = newPathCut;
                    CutPageScada.Attachments = inItemFolderScada.Attachments + 1;
                    CutPageScada.AttachmentFolder = inItemFolderScada.Path;
                    CutPageScada.ParentItem = inItemFolderScada.TreeItem;

                    if (inItemFolderScada.ChildItem == null)
                    {
                        inItemFolderScada.ChildItem = new List<TreeViewItem>();
                    }
                     
                    inItemFolderScada.ChildItem.Add(CurrentItem);
                    
                    inItemFolder.Items.Add(CurrentItem);

                    inItemFolder.Items.SortDescriptions.Clear();
                    inItemFolder.Items.SortDescriptions.Add(new SortDescription("ContextMenu.Tag", ListSortDirection.Ascending));
                    inItemFolder.Items.SortDescriptions.Add(new SortDescription("Header.Tag", ListSortDirection.Ascending));

                    tabParent.RenameTab();

                    this.AddCollectionPage(CutPageScada, tabParent, pageCut);   
                }

                if (CutItem is ControlPanelScada)
                {
                    FolderScada inItemFolderScada = InItem as FolderScada;
                    ControlPanelScada CutControlPanelScada = CutItem as ControlPanelScada;

                    string newPathCut = inItemFolderScada.Path + "\\" + CutControlPanelScada.Name;

                    if (File.Exists(newPathCut))
                    {
                        MessageBox.Show("Внимание! Щит управления " + newPathCut + " уже существует.", "Ошибка вставки", MessageBoxButton.OK, MessageBoxImage.Error);

                        e.Handled = true;
                        return;
                    }

                    File.Move(CutControlPanelScada.Path, newPathCut);

                    ControlPanel ControlPanelCut = ((AppWPF)Application.Current).CollectionControlPanel[CutControlPanelScada.Path];

                    TabItemParent tabParent = ((AppWPF)Application.Current).CollectionTabItemParent[CutControlPanelScada.Path];

                    this.RemoveCollectionControlPanel(CutControlPanelScada);

                    TreeViewItem parentItemFolder = CutControlPanelScada.ParentItem;
                    TreeViewItem inItemFolder = inItemFolderScada.TreeItem;

                    if (parentItemFolder != null)
                    {
                        parentItemFolder.Items.Remove(CurrentItem);

                        FolderScada parentFolderScada = parentItemFolder.Tag as FolderScada;
                        parentFolderScada.ChildItem.Remove(CurrentItem);
                    }
                    else
                    {
                        BrowseProject.Items.Remove(CurrentItem);
                    }

                    CutControlPanelScada.Path = newPathCut;
                    CutControlPanelScada.Attachments = inItemFolderScada.Attachments + 1;
                    CutControlPanelScada.AttachmentFolder = inItemFolderScada.Path;
                    CutControlPanelScada.ParentItem = inItemFolderScada.TreeItem;

                    if (inItemFolderScada.ChildItem == null)
                    {
                        inItemFolderScada.ChildItem = new List<TreeViewItem>();
                    }

                    inItemFolderScada.ChildItem.Add(CurrentItem);

                    inItemFolder.Items.Add(CurrentItem);

                    inItemFolder.Items.SortDescriptions.Clear();
                    inItemFolder.Items.SortDescriptions.Add(new SortDescription("ContextMenu.Tag", ListSortDirection.Ascending));
                    inItemFolder.Items.SortDescriptions.Add(new SortDescription("Header.Tag", ListSortDirection.Ascending));

                    tabParent.RenameTab();

                    this.AddCollectionControlPanel(CutControlPanelScada, tabParent, ControlPanelCut);                
                }

                Clipboard.Clear();
                e.Handled = true;
                return;
            }

            e.Handled = true;
        }

        public void CutItem(object sender, RoutedEventArgs e)
        {          
            if (((MenuItem)sender).Tag is FolderScada)
            {
                FolderScada fs = (FolderScada)((MenuItem)sender).Tag;

                TreeViewItem ItemFolder = fs.TreeItem;

                if (ItemFolder == CurrentItem)
                {
                    if (IsCopy)
                    {
                        ItemFolder.Background = new SolidColorBrush(Colors.Gray);
                        IsCopy = false;
                    }
                    e.Handled = true;
                    return;
                }

                ItemFolder.Background = new SolidColorBrush(Colors.Gray);

                if (CurrentItem != null) CurrentItem.Background = new SolidColorBrush(Colors.White);
                CurrentItem = ItemFolder;
            }
            else if (((MenuItem)sender).Tag is PageScada)
            {
                PageScada pg = (PageScada)((MenuItem)sender).Tag;

                TreeViewItem ItemPage = pg.TreeItem;

                if (ItemPage == CurrentItem)
                {
                    if (IsCopy)
                    {
                        ItemPage.Background = new SolidColorBrush(Colors.Gray);
                        IsCopy = false;
                    }
                    e.Handled = true;
                    return;
                }

                ItemPage.Background = new SolidColorBrush(Colors.Gray);

                if (CurrentItem != null) CurrentItem.Background = new SolidColorBrush(Colors.White);
                CurrentItem = ItemPage;
            }
            else if (((MenuItem)sender).Tag is ControlPanelScada)
            {
                ControlPanelScada cps = (ControlPanelScada)((MenuItem)sender).Tag;

                TreeViewItem ItemControlPanel = cps.TreeItem;

                if (ItemControlPanel == CurrentItem)
                {
                    if (IsCopy)
                    {
                        ItemControlPanel.Background = new SolidColorBrush(Colors.Gray);
                        IsCopy = false;
                    }
                    e.Handled = true;
                    return;
                }

                ItemControlPanel.Background = new SolidColorBrush(Colors.Gray);

                if (CurrentItem != null) CurrentItem.Background = new SolidColorBrush(Colors.White);
                CurrentItem = ItemControlPanel;
            }

            IsCopy = false;
            Clipboard.SetDataObject(((MenuItem)sender).Tag); // Передаем FolderScada или PageScada или ControlPanelScada
            e.Handled = true;
        }

        public void DeleteFolderScada(TreeViewItem item)
        {              
            foreach (TreeViewItem child in item.Items)
            {
                DeleteFolderScada(child);
            }

            if (item.Tag is PageScada)
            {
                PageScada ps = item.Tag as PageScada;

                ((AppWPF)Application.Current).CollectionTabItemParent[ps.Path].DeleteTabItem();

                return;
            }

            if (item.Tag is ControlPanelScada)
            {
                ControlPanelScada cps = item.Tag as ControlPanelScada;

                ((AppWPF)Application.Current).CollectionControlPanel.Remove(cps.Path);
                ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionControlPanelScada.Remove(cps.Path);

                return;
            }

            if(item.Tag is FolderScada)
            {
                FolderScada fs = item.Tag as FolderScada;

                if (fs.ParentItem == null) BrowseProject.Items.Remove(fs.TreeItem);

                ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionFolderScada.Remove(fs.Path);

                return;
            }
        }

        public void DeleteItem(object sender, RoutedEventArgs e)
        {
            if (((MenuItem)sender).Tag is FolderScada)
            {
                FolderScada fs = ((MenuItem)sender).Tag as FolderScada;

                if (fs.ParentItem == null)
                {                  
                    DeleteFolderScada(fs.TreeItem);
                    Directory.Delete(fs.Path, true);    
                }
                else
                {
                    TreeViewItem parentItem = fs.ParentItem;
                    parentItem.Items.Remove(fs.TreeItem);

                    FolderScada parentFolderScada = parentItem.Tag as FolderScada;
                    parentFolderScada.ChildItem.Remove(fs.TreeItem);

                    DeleteFolderScada(fs.TreeItem);

                    Directory.Delete(fs.Path, true);
                }
            }
         
            if (((MenuItem)sender).Tag is PageScada)
            {
                PageScada ps = ((MenuItem)sender).Tag as PageScada;

                if (ps.ParentItem == null)
                {
                    ((AppWPF)Application.Current).CollectionTabItemParent[ps.Path].DeleteTabItem();

                    BrowseProject.Items.Remove(ps.TreeItem);

                    File.Delete(ps.Path);
                }
                else 
                {
                    TreeViewItem parentItem = ps.ParentItem;
                    parentItem.Items.Remove(ps.TreeItem);

                    FolderScada parentFolderScada = parentItem.Tag as FolderScada;
                    parentFolderScada.ChildItem.Remove(ps.TreeItem);

                    ((AppWPF)Application.Current).CollectionTabItemParent[ps.Path].DeleteTabItem();

                    File.Delete(ps.Path);
                }
            }

            if (((MenuItem)sender).Tag is ControlPanelScada)
            {
                ControlPanelScada cps = ((MenuItem)sender).Tag as ControlPanelScada;

                if (cps.ParentItem == null)
                {
                    ((AppWPF)Application.Current).CollectionTabItemParent[cps.Path].DeleteTabItem();

                    BrowseProject.Items.Remove(cps.TreeItem);

                    File.Delete(cps.Path);
                }
                else 
                {
                    TreeViewItem parentItem = cps.ParentItem;
                    parentItem.Items.Remove(cps.TreeItem);

                    FolderScada parentFolderScada = parentItem.Tag as FolderScada;
                    parentFolderScada.ChildItem.Remove(cps.TreeItem);

                    ((AppWPF)Application.Current).CollectionTabItemParent[cps.Path].DeleteTabItem();

                    File.Delete(cps.Path);
                }
            }

            e.Handled = true;
        }

        public void CopyDir(string FromDir, string ToDir)
        {
            Directory.CreateDirectory(ToDir);
            foreach (string s1 in Directory.GetFiles(FromDir))
            {
                string s2 = ToDir + "\\" + System.IO.Path.GetFileName(s1);
                File.Copy(s1, s2, true);
            }
            foreach (string s in Directory.GetDirectories(FromDir))
            {
                if (s == ToDir) return;
                CopyDir(s, ToDir + "\\" + System.IO.Path.GetFileName(s));
            }
        }

        public string RenameCopyControlPanel(int count)
        {
            int index = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject.LastIndexOf(((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.ProjectName);
            string path = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject.Remove(index) + "Копия" + count + " - " + ((ControlPanelScada)currentItem.Tag).Name;
            string nameControlPanel = "Копия" + count + " - " + ((ControlPanelScada)currentItem.Tag).Name;

            if (File.Exists(path)) nameControlPanel = RenameCopyControlPanel(count + 1);
            return nameControlPanel;
        }
        
        public string RenameCopyPage(int count)
        {
            int index = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject.LastIndexOf(((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.ProjectName);
            string path = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject.Remove(index) + "Копия" + count + " - " + ((PageScada)currentItem.Tag).Name;
            string namePage = "Копия" + count + " - " + ((PageScada)currentItem.Tag).Name;

            if (File.Exists(path)) namePage = RenameCopyPage(count + 1);
            return namePage;
        }

        public string RenameCopyInItemControlPanel(int count, string inPath)
        {
            string nameControlPanel = "Копия" + count + " - " + ((ControlPanelScada)currentItem.Tag).Name;
            string path = inPath + "\\" + nameControlPanel;

            if (File.Exists(path)) nameControlPanel = RenameCopyInItemControlPanel(count + 1, inPath);
            return nameControlPanel;
        }

        public string RenameCopyInItemPage(int count, string inPath)
        {
            string namePage = "Копия" + count + " - " + ((PageScada)currentItem.Tag).Name;
            string path = inPath + "\\" + namePage;

            if (File.Exists(path)) namePage = RenameCopyInItemPage(count + 1, inPath);
            return namePage;
        }

        public string RenameCopyInItem(int count, string inPath)
        {         
            string nameFolder = "Копия" + count + " - " + ((FolderScada)currentItem.Tag).Name;
            string path = inPath + "\\" + nameFolder;

            if (Directory.Exists(path)) nameFolder = RenameCopyInItem(count + 1, inPath);
            return nameFolder;
        }

        public string RenameCopy(int count)
        {
            int index = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject.LastIndexOf(((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.ProjectName);
            string path = ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.PathProject.Remove(index) + "Копия" + count + " - " + ((FolderScada)currentItem.Tag).Name;
            string nameFolder = "Копия" + count + " - " + ((FolderScada)currentItem.Tag).Name;

            if (Directory.Exists(path)) nameFolder = RenameCopy(count + 1);
            return nameFolder;
        }

        public void CopyItemBrowse(TreeViewItem parentCurItem, TreeViewItem parentCopyItem)
        {
            ItemCollection collectionItem = parentCurItem.Items;

            foreach (TreeViewItem child in collectionItem)
            {
                TreeViewItem nextCopyItem = null;

                object objectChild = child.Tag;

                if (objectChild is FolderScada)
                {
                    FolderScada childCurFolderScada = (FolderScada)objectChild;

                    FolderScada childCopyFolderScada = new FolderScada();
                    childCopyFolderScada.AttachmentFolder = ((FolderScada)parentCopyItem.Tag).Path;
                    childCopyFolderScada.Attachments = childCurFolderScada.Attachments;
                    childCopyFolderScada.Name = childCurFolderScada.Name;
                    childCopyFolderScada.ParentItem = parentCopyItem;
                    childCopyFolderScada.Path = ((FolderScada)parentCopyItem.Tag).Path + "\\" + childCopyFolderScada.Name;
                    childCopyFolderScada.IsExpand = childCurFolderScada.IsExpand;

                    StackPanel panelFolder = new StackPanel();
                    panelFolder.Orientation = System.Windows.Controls.Orientation.Horizontal;

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

                    TextBox tbFolder = new TextBox();
                    tbFolder.KeyDown += OkRenameFolder;
                    tbFolder.Text = childCopyFolderScada.Name;

                    Label lNameFolder = new Label();
                    lNameFolder.Content = tbFolder.Text;
                    lNameFolder.Tag = tbFolder;

                    AlphanumComparator a = new AlphanumComparator();
                    a.Name = (string)lNameFolder.Content;

                    panelFolder.Children.Add(imageFolder);
                    panelFolder.Children.Add(lNameFolder);
                    panelFolder.Tag = a;

                    TreeViewItem copyItem = new TreeViewItem();
                    copyItem.Collapsed += Collapsed;
                    copyItem.Expanded += Expanded;
                    copyItem.Tag = childCopyFolderScada;
                    copyItem.KeyDown += RenameFolder;
                    copyItem.Header = panelFolder;

                    nextCopyItem = copyItem;

                    childCopyFolderScada.TreeItem = copyItem;

                    if (((FolderScada)parentCopyItem.Tag).ChildItem == null) ((FolderScada)parentCopyItem.Tag).ChildItem = new List<TreeViewItem>();
                    ((FolderScada)parentCopyItem.Tag).ChildItem.Add(copyItem);

                    MenuItem MenuItemCreate = new MenuItem();
                    MenuItemCreate.Header = "Добавить";

                    MenuItem MenuItemCreateControlPanel = new MenuItem();
                    MenuItemCreateControlPanel.Click += ContextMenuCreateControlPanel;
                    MenuItemCreateControlPanel.Icon = imageControlPanel;
                    MenuItemCreateControlPanel.Header = "Щит управления";
                    MenuItemCreateControlPanel.Tag = copyItem;

                    MenuItem MenuItemCreateFolder = new MenuItem();
                    MenuItemCreateFolder.Icon = imageMenuItemCreateFolder;
                    MenuItemCreateFolder.Header = "Папку";
                    MenuItemCreateFolder.Tag = copyItem; // Нужен для индефикации в какую папку сохранять при создании вложенной папки
                    MenuItemCreateFolder.Click += ContextMenuCreateFolder;

                    MenuItem MenuItemCreatePage = new MenuItem();
                    MenuItemCreatePage.Icon = imageMenuItemCreatePage;
                    MenuItemCreatePage.Header = "Страницу";
                    MenuItemCreatePage.Tag = copyItem;  // Нужен для индефикации в какую папку сохранять при создании вложенной страницы
                    MenuItemCreatePage.Click += ContextMenuCreatePage;

                    MenuItem menuItemDeleteFolder = new MenuItem();
                    menuItemDeleteFolder.Header = "Удалить";
                    menuItemDeleteFolder.Icon = ImageDelete;
                    menuItemDeleteFolder.Tag = childCopyFolderScada;
                    menuItemDeleteFolder.Click += DeleteItem;

                    MenuItem menuItemCopyFolder = new MenuItem();
                    menuItemCopyFolder.Header = "Копировать";
                    menuItemCopyFolder.Icon = ImageCopy;
                    menuItemCopyFolder.Tag = childCopyFolderScada;
                    menuItemCopyFolder.Click += CopyItem;
               
                    MenuItem menuItemCutFolder = new MenuItem();
                    menuItemCutFolder.Header = "Вырезать";
                    menuItemCutFolder.Icon = imageCut;
                    menuItemCutFolder.Tag = childCopyFolderScada;
                    menuItemCutFolder.Click += CutItem;

                    Binding BindingInsert = new Binding();
                    BindingInsert.Source = this;
                    BindingInsert.Path = new PropertyPath("IsBindingInsert");
                    BindingInsert.Mode = BindingMode.OneWay;

                    MenuItem menuItemInsert = new MenuItem();
                    menuItemInsert.Header = "Вставить";
                    menuItemInsert.Tag = childCopyFolderScada;
                    menuItemInsert.SetBinding(MenuItem.IsEnabledProperty, BindingInsert);
                    menuItemInsert.Icon = imageInsert;
                    menuItemInsert.Click += InsertItem;

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

                    copyItem.ContextMenu = ContextMenuFolder;

                    ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionFolderScada.Add(childCopyFolderScada.Path, childCopyFolderScada);                 
                }

                if (objectChild is PageScada)
                {
                    PageScada childCurPageScada = objectChild as PageScada;

                    PageScada childCopyPageScada = new PageScada();
                    childCopyPageScada.AttachmentFolder = ((FolderScada)parentCopyItem.Tag).Path;
                    childCopyPageScada.Attachments = childCurPageScada.Attachments;
                    childCopyPageScada.Name = childCurPageScada.Name;
                    childCopyPageScada.ParentItem = parentCopyItem;
                    childCopyPageScada.Path = ((FolderScada)parentCopyItem.Tag).Path + "\\" + childCopyPageScada.Name;

                    StackPanel panelPage = new StackPanel();
                    panelPage.Orientation = System.Windows.Controls.Orientation.Horizontal;

                    Image imagePage = new Image();
                    imagePage.Source = new BitmapImage(new Uri("Images/Page16.png", UriKind.Relative));

                    Image imageCut = new Image();
                    imageCut.Source = new BitmapImage(new Uri("Images/Cut16.png", UriKind.Relative));

                    Image imageDelete = new Image();
                    imageDelete.Source = new BitmapImage(new Uri("Images/PageDelete16.png", UriKind.Relative));

                    Image imageCopy = new Image();
                    imageCopy.Source = new BitmapImage(new Uri("Images/CopyPage16.png", UriKind.Relative));

                    MenuItem menuItemCopyPage = new MenuItem();
                    menuItemCopyPage.Header = "Копировать";
                    menuItemCopyPage.Icon = imageCopy;
                    menuItemCopyPage.Tag = childCopyPageScada;
                    menuItemCopyPage.Click += CopyItem;

                    MenuItem menuItemCutPage = new MenuItem();
                    menuItemCutPage.Header = "Вырезать";
                    menuItemCutPage.Icon = imageCut;
                    menuItemCutPage.Tag = childCopyPageScada;
                    menuItemCutPage.Click += CutItem;

                    MenuItem menuItemDeletePage = new MenuItem();
                    menuItemDeletePage.Header = "Удалить";
                    menuItemDeletePage.Icon = imageDelete;
                    menuItemDeletePage.Tag = childCopyPageScada;
                    menuItemDeletePage.Click += DeleteItem;

                    TextBox tbPage = new TextBox();
                    tbPage.KeyDown += OkRenamePage;
                    tbPage.Text = childCopyPageScada.Name;

                    Label lNamePage = new Label();
                    lNamePage.Content = tbPage.Text;
                    lNamePage.Tag = tbPage;

                    ContextMenu contextMenuPage = new ContextMenu();
                    contextMenuPage.Items.Add(menuItemCopyPage);
                    contextMenuPage.Items.Add(menuItemCutPage);
                    contextMenuPage.Items.Add(menuItemDeletePage);
                    contextMenuPage.Tag = "PageScada";

                    AlphanumComparator a = new AlphanumComparator();
                    a.Name = (string)lNamePage.Content;

                    panelPage.Children.Add(imagePage);
                    panelPage.Children.Add(lNamePage);
                    panelPage.Tag = a;

                    TreeViewItem copyItemPage = new TreeViewItem();
                    copyItemPage.MouseDoubleClick += OpenBrowsePage;
                    copyItemPage.Tag = childCopyPageScada;
                    copyItemPage.KeyDown += RenamePage;
                    copyItemPage.Header = panelPage;
                    copyItemPage.ContextMenu = contextMenuPage;

                    nextCopyItem = copyItemPage;

                    childCopyPageScada.TreeItem = copyItemPage;

                    if (((FolderScada)parentCopyItem.Tag).ChildItem == null) ((FolderScada)parentCopyItem.Tag).ChildItem = new List<TreeViewItem>();
                    ((FolderScada)parentCopyItem.Tag).ChildItem.Add(copyItemPage);

                    Page copyPage = new Page();

                    Page curPage = ((AppWPF)Application.Current).CollectionPage[childCurPageScada.Path];

                    foreach (PipeSer pipeSer in curPage.CollectionPipe)
                    {
                        PipeSer copyPipeSer;

                        using (MemoryStream TempStream = new MemoryStream())
                        {
                            BinaryFormatter serializer = new BinaryFormatter();

                            serializer.Serialize(TempStream, pipeSer);

                            using (MemoryStream TempStreamRead = new MemoryStream(TempStream.ToArray()))
                            {
                                BinaryFormatter deserializer = new BinaryFormatter();

                                copyPipeSer = (PipeSer)deserializer.Deserialize(TempStreamRead);

                                copyPage.CollectionPipe.Add(copyPipeSer);
                            }
                        }
                    }

                    this.AddCollectionPageCopy(childCopyPageScada, copyPage);

                    TabItemPage tabItemPage = new TabItemPage(childCopyPageScada);

                }

                if (objectChild is ControlPanelScada)
                {
                    ControlPanelScada childCurControlPanelScada = objectChild as ControlPanelScada;

                    ControlPanelScada childCopyControlPanelScada = new ControlPanelScada();
                    childCopyControlPanelScada.AttachmentFolder = ((FolderScada)parentCopyItem.Tag).Path;
                    childCopyControlPanelScada.Attachments = childCurControlPanelScada.Attachments;
                    childCopyControlPanelScada.Name = childCurControlPanelScada.Name;
                    childCopyControlPanelScada.ParentItem = parentCopyItem;
                    childCopyControlPanelScada.Path = ((FolderScada)parentCopyItem.Tag).Path + "\\" + childCopyControlPanelScada.Name;

                    StackPanel panel = new StackPanel();
                    panel.Orientation = System.Windows.Controls.Orientation.Horizontal;

                    Image imageControlPanel = new Image();
                    imageControlPanel.Source = new BitmapImage(new Uri("Images/ControlPanel16.png", UriKind.Relative));

                    Image imageCut = new Image();
                    imageCut.Source = new BitmapImage(new Uri("Images/Cut16.png", UriKind.Relative));

                    Image imageDelete = new Image();
                    imageDelete.Source = new BitmapImage(new Uri("Images/PageDelete16.png", UriKind.Relative));

                    Image imageCopy = new Image();
                    imageCopy.Source = new BitmapImage(new Uri("Images/CopyPage16.png", UriKind.Relative));

                    MenuItem menuItemCopyControlPanel = new MenuItem();
                    menuItemCopyControlPanel.Header = "Копировать";
                    menuItemCopyControlPanel.Icon = imageCopy;
                    menuItemCopyControlPanel.Tag = childCopyControlPanelScada;
                    menuItemCopyControlPanel.Click += CopyItem;

                    MenuItem menuItemCutControlPanel = new MenuItem();
                    menuItemCutControlPanel.Header = "Вырезать";
                    menuItemCutControlPanel.Icon = imageCut;
                    menuItemCutControlPanel.Tag = childCopyControlPanelScada;
                    menuItemCutControlPanel.Click += CutItem;

                    MenuItem menuItemDeleteControlPanel = new MenuItem();
                    menuItemDeleteControlPanel.Header = "Удалить";
                    menuItemDeleteControlPanel.Icon = imageDelete;
                    menuItemDeleteControlPanel.Tag = childCopyControlPanelScada;
                    menuItemDeleteControlPanel.Click += DeleteItem;

                    TextBox tbControlPanel = new TextBox();
                    tbControlPanel.KeyDown += OkRenameControlPanel;
                    tbControlPanel.Text = childCopyControlPanelScada.Name;

                    Label lNameControlPanel = new Label();
                    lNameControlPanel.Content = tbControlPanel.Text;
                    lNameControlPanel.Tag = tbControlPanel;

                    ContextMenu contextMenuControlPanel = new ContextMenu();
                    contextMenuControlPanel.Items.Add(menuItemCopyControlPanel);
                    contextMenuControlPanel.Items.Add(menuItemCutControlPanel);
                    contextMenuControlPanel.Items.Add(menuItemDeleteControlPanel);
                    contextMenuControlPanel.Tag = "X";

                    AlphanumComparator a = new AlphanumComparator();
                    a.Name = (string)lNameControlPanel.Content;

                    panel.Children.Add(imageControlPanel);
                    panel.Children.Add(lNameControlPanel);
                    panel.Tag = a;

                    TreeViewItem copyItemControlPanel = new TreeViewItem();
                    copyItemControlPanel.Tag = childCopyControlPanelScada;
                    copyItemControlPanel.KeyDown += RenameControlPanel;
                    copyItemControlPanel.Header = panel;
                    copyItemControlPanel.ContextMenu = contextMenuControlPanel;

                    nextCopyItem = copyItemControlPanel;

                    childCopyControlPanelScada.TreeItem = copyItemControlPanel;

                    if (((FolderScada)parentCopyItem.Tag).ChildItem == null) ((FolderScada)parentCopyItem.Tag).ChildItem = new List<TreeViewItem>();
                    ((FolderScada)parentCopyItem.Tag).ChildItem.Add(copyItemControlPanel);

                    ControlPanel copyControlPanel = new ControlPanel();

                    ControlPanel curControlPanel = ((AppWPF)Application.Current).CollectionControlPanel[childCurControlPanelScada.Path];

                    this.AddCollectionControlPanelCopy(childCurControlPanelScada, copyControlPanel);

                    TabItemControlPanel tabItemControlPanel = new TabItemControlPanel(childCurControlPanelScada);
                }
                parentCopyItem.Items.Add(nextCopyItem);

                CopyItemBrowse(child, nextCopyItem);
            }

        }

        public void CopyItem(object sender, RoutedEventArgs e)
        {
            if (((MenuItem)sender).Tag is FolderScada)
            {
                FolderScada fs = (FolderScada)((MenuItem)sender).Tag;

                TreeViewItem ItemFolder = fs.TreeItem;

                if (ItemFolder == CurrentItem)
                {
                    if (!IsCopy)
                    {
                        CurrentItem.Background = new SolidColorBrush(Colors.White);
                        IsCopy = true;
                    }
                    e.Handled = true;
                    return;
                }

                if (CurrentItem != null) CurrentItem.Background = new SolidColorBrush(Colors.White);
                CurrentItem = ItemFolder;
            }
            else if (((MenuItem)sender).Tag is PageScada)
            {
                PageScada pg = (PageScada)((MenuItem)sender).Tag;

                TreeViewItem ItemPage = pg.TreeItem;

                if (ItemPage == CurrentItem)
                {
                    if (!IsCopy)
                    {
                        CurrentItem.Background = new SolidColorBrush(Colors.White);
                        IsCopy = true;
                    }
                    e.Handled = true;
                    return;
                }

                if (CurrentItem != null) CurrentItem.Background = new SolidColorBrush(Colors.White);
                CurrentItem = ItemPage;
            }
            else if (((MenuItem)sender).Tag is ControlPanelScada)
            {
                ControlPanelScada cps = (ControlPanelScada)((MenuItem)sender).Tag;

                TreeViewItem ItemPage = cps.TreeItem;

                if (ItemPage == CurrentItem)
                {
                    if (!IsCopy)
                    {
                        CurrentItem.Background = new SolidColorBrush(Colors.White);
                        IsCopy = true;
                    }
                    e.Handled = true;
                    return;
                }

                if (CurrentItem != null) CurrentItem.Background = new SolidColorBrush(Colors.White);
                CurrentItem = ItemPage;
            }

            IsCopy = true;
            Clipboard.SetDataObject(((MenuItem)sender).Tag); // Передаем FolderScada или PageScada
            e.Handled = true;
        }

        private void Drag(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;

            item.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                item.Focus();
            }));
           
            DragDrop.DoDragDrop(item, item.Tag, DragDropEffects.Copy);

            Mouse.OverrideCursor = null;

            e.Handled = true;            
        }

        private void DragImage(object sender, GiveFeedbackEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;

            DragAndDropCanvas drag = (DragAndDropCanvas)item.Tag;
                       
            if (e.Effects == DragDropEffects.Copy)
            {
                if (drag.IsPipe) Mouse.OverrideCursor = ((AppWPF)Application.Current).CursorPipe;
                else if (drag.IsPipe90) Mouse.OverrideCursor = ((AppWPF)Application.Current).CursorPipe90;
                else if (drag.IsText) Mouse.OverrideCursor = ((AppWPF)Application.Current).CursorText;
                else if (drag.IsEthernet) Mouse.OverrideCursor = ((AppWPF)Application.Current).CursorEthernet;
                else if (drag.IsCom) Mouse.OverrideCursor = ((AppWPF)Application.Current).CursorCom;
                else if (drag.IsDisplay) Mouse.OverrideCursor = ((AppWPF)Application.Current).CursorDisplay;
                else if (drag.IsImage) Mouse.OverrideCursor = ((AppWPF)Application.Current).CursorImage;
                else if (drag.IsModbus) Mouse.OverrideCursor = ((AppWPF)Application.Current).CursorModbus;
            }
            else if (e.Effects == DragDropEffects.None)
            {
                if (drag.IsPipe) Mouse.OverrideCursor = ((AppWPF)Application.Current).CursorPipeInvalid;
                else if (drag.IsPipe90) Mouse.OverrideCursor = ((AppWPF)Application.Current).CursorPipe90Invalid;
                else if (drag.IsText) Mouse.OverrideCursor = ((AppWPF)Application.Current).CursorTextInvalid;
                else if (drag.IsEthernet) Mouse.OverrideCursor = ((AppWPF)Application.Current).CursorEthernetInvalid;
                else if (drag.IsCom) Mouse.OverrideCursor = ((AppWPF)Application.Current).CursorComInvalid;
                else if (drag.IsDisplay) Mouse.OverrideCursor = ((AppWPF)Application.Current).CursorDisplayInvalid;
                else if (drag.IsImage) Mouse.OverrideCursor = ((AppWPF)Application.Current).CursorImageInvalid;
                else if (drag.IsModbus) Mouse.OverrideCursor = ((AppWPF)Application.Current).CursorModbusInvalid;
            }
            else Mouse.OverrideCursor = null;
           
            e.Handled = true;
        }

        public void Save(object sender, RoutedEventArgs e)
        {
            try
            {
                TabItemParent tabItemSelcted = (TabItemParent)TabControlMain.SelectedItem;

                if (tabItemSelcted == null)
                {
                    e.Handled = true;
                    return;
                }
                else
                {
                    ItemScada IS = tabItemSelcted.IS;

                    if (tabItemSelcted.isSave)
                    {
                        ((AppWPF)Application.Current).CollectionSaveTabItem.Remove(tabItemSelcted);

                        object obj = null;

                        if (tabItemSelcted is TabItemPage)
                        {
                            obj = ((AppWPF)Application.Current).CollectionPage[IS.Path];
                        }
                        else if (tabItemSelcted is TabItemControlPanel)
                        {
                            obj = ((AppWPF)Application.Current).CollectionControlPanel[IS.Path];
                        }

                        using (FileStream fs = File.Create((IS.Path)))
                        {
                            XamlWriter.Save(obj, fs);
                        }

                        tabItemSelcted.isSave = false;

                        StackPanel panel = (StackPanel)tabItemSelcted.Header;
                        Label l = (Label)panel.Children[0];

                        string name = (string)l.Content;

                        if (name.IndexOf('*') != -1)
                        {
                            l.Content = IS.Name;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                if (CollectionMessage.Count > 300)
                {
                    CollectionMessage.RemoveAt(0);

                    CollectionMessage.Insert(298, "Сообщение " + " : " + ex.Message + " " + " " + DateTime.Now);
                }
                else
                {
                    CollectionMessage.Add("Сообщение " + " : " + ex.Message + " " + " " + DateTime.Now);
                }
            }
                       
            e.Handled = true;
        }

        private void TabControlMain_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            IList list = e.AddedItems;

            foreach (TabItemParent tabItemParent in list)
            {
                tabItemParent.IS.IsFocus = true;
                tabItemParent.IS.IsOpen = true;              
            }

            IList list2 = e.RemovedItems;

            bool isCompare = false;

            foreach (TabItemParent tabItemParent in list2)
            {                
                foreach(Control control in TabControlMain.Items)
                {
                    if (tabItemParent == control)
                    {
                        isCompare = true;
                    }
                }

                if (isCompare)
                {
                    tabItemParent.IS.IsOpen = true;
                }
                else
                {
                    tabItemParent.IS.IsOpen = false;
                }

                isCompare = false;

                tabItemParent.IS.IsFocus = false;                            
            }

            if (list.Count != 0)
            {
                TabItem selectedTabItem = (TabItem)list[0];
                ScrollViewer scroll = (ScrollViewer)selectedTabItem.Content;

                if (IsBindingStartProject)
                {
                    if (scroll.Content is CanvasTab)
                    {
                        CanvasTab selectedCanvas = scroll.Content as CanvasTab;

                        if (selectedCanvas.SelectedControlOnCanvas.Count != 0)
                        {
                            foreach (ControlOnCanvas objectOnCanvas in selectedCanvas.SelectedControlOnCanvas)
                            {
                                objectOnCanvas.IsSelected = false;
                                objectOnCanvas.border.Pen.Brush.Opacity = 0;
                            }

                            selectedCanvas.CountSelect = 0;
                            selectedCanvas.SelectedControlOnCanvas.Clear();

                            this.LabelSelected.Content = "Выделенно объектов: " + 0;
                            this.TextBoxDiameter.Text = null;
                            this.CoordinateObjectX.Text = null;
                            this.CoordinateObjectY.Text = null;
                            this.ComboBoxEnvironment.SelectedIndex = -1;

                            this.TextBoxDiameter.IsReadOnly = true;
                            this.CoordinateObjectX.IsReadOnly = true;
                            this.CoordinateObjectY.IsReadOnly = true;
                            this.ComboBoxEnvironment.IsEnabled = false;
                        }
                    }
                }
               
                if (scroll.Content is CanvasPage)
                {
                    CanvasPage canvasAS = scroll.Content as CanvasPage;

                    if (canvasAS.CountSelect > 1)
                    {
                        CoordinateObjectX.IsReadOnly = true;
                        CoordinateObjectY.IsReadOnly = true;
                        CoordinateObjectX.Text = null;
                        CoordinateObjectY.Text = null;

                        LabelSelected.Content = "Выделенно объектов: " + canvasAS.CountSelect;

                        PipeOnCanvas pipeOld = null;
                        PipeOnCanvas pipe = null;
                        bool falseComparer = false;
                        int countPipe = 0;

                        foreach (ControlOnCanvasPage controlOnCanvas in canvasAS.Children)
                        {
                            if (controlOnCanvas is PipeOnCanvas)
                            {
                                if (controlOnCanvas.IsSelected)
                                {
                                    countPipe += 1;
                                    pipe = controlOnCanvas as PipeOnCanvas;

                                    if (!falseComparer)
                                    {
                                        if (countPipe > 1)
                                        {
                                            if (Math.Round(pipeOld.Diameter, 2, MidpointRounding.AwayFromZero) != Math.Round(pipe.Diameter, 2, MidpointRounding.AwayFromZero))
                                            {
                                                pipe.TextBoxDiameter.Text = "-";
                                            }
                                            else
                                            {
                                                pipe.TextBoxDiameter.Text = string.Format("{0:F2}", pipe.Diameter);
                                            }
                                            if (pipeOld.IntEnvironment != pipe.IntEnvironment)
                                            {
                                                pipe.ComboBoxEnvironment.SelectedIndex = -1;
                                            }
                                            else
                                            {
                                                pipe.ComboBoxEnvironment.SelectedIndex = pipe.IntEnvironment;
                                            }

                                            if (Math.Round(pipeOld.Diameter, 2, MidpointRounding.AwayFromZero) != Math.Round(pipe.Diameter, 2, MidpointRounding.AwayFromZero))
                                            {
                                                falseComparer = true;
                                            }
                                            else if (pipeOld.IntEnvironment != pipe.IntEnvironment)
                                            {
                                                falseComparer = true;
                                            }
                                        }
                                    }

                                    pipeOld = (PipeOnCanvas)controlOnCanvas;
                                }
                            }
                        }

                        if (countPipe > 0)
                        {
                            TextBoxDiameter.IsReadOnly = false;
                            ComboBoxEnvironment.IsEnabled = true;
                        }
                        if (countPipe == 1)
                        {
                            TextBoxDiameter.IsReadOnly = false;
                            ComboBoxEnvironment.IsEnabled = true;
                            TextBoxDiameter.Text = string.Format("{0:F2}", pipe.Diameter);
                            ComboBoxEnvironment.SelectedIndex = pipe.IntEnvironment;
                        }
                        else if (countPipe == 0)
                        {
                            TextBoxDiameter.IsReadOnly = true;
                            ComboBoxEnvironment.IsEnabled = false;
                            TextBoxDiameter.Text = null;
                            ComboBoxEnvironment.SelectedIndex = -1;
                        }
                    }
                    else if (canvasAS.CountSelect == 1)
                    {
                        PipeOnCanvas pipeOnCanvas = null;

                        LabelSelected.Content = "Выделенно объектов: " + canvasAS.CountSelect;

                        foreach (ControlOnCanvasPage controlOnCanvas in canvasAS.Children)
                        {
                            if (controlOnCanvas.IsSelected)
                            {
                                CoordinateObjectX.IsReadOnly = false;
                                CoordinateObjectY.IsReadOnly = false;
                                if (controlOnCanvas.controlOnCanvasSer.Transform == 0)
                                {
                                    controlOnCanvas.CoordinateX.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.LeftProperty));
                                    controlOnCanvas.CoordinateY.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.TopProperty));
                                }
                                else if (controlOnCanvas.controlOnCanvasSer.Transform == -90 || controlOnCanvas.controlOnCanvasSer.Transform == 270)
                                {
                                    controlOnCanvas.CoordinateY.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.TopProperty) - controlOnCanvas.ActualWidth);
                                    controlOnCanvas.CoordinateX.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.LeftProperty));
                                }
                                else if (controlOnCanvas.controlOnCanvasSer.Transform == -180 || controlOnCanvas.controlOnCanvasSer.Transform == 180)
                                {
                                    controlOnCanvas.CoordinateY.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.TopProperty) - controlOnCanvas.ActualHeight);
                                    controlOnCanvas.CoordinateX.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.LeftProperty) - controlOnCanvas.ActualWidth);
                                }
                                else if (controlOnCanvas.controlOnCanvasSer.Transform == -270 || controlOnCanvas.controlOnCanvasSer.Transform == 90)
                                {
                                    controlOnCanvas.CoordinateY.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.TopProperty));
                                    controlOnCanvas.CoordinateX.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.LeftProperty) - controlOnCanvas.ActualHeight);
                                }

                                if (controlOnCanvas is PipeOnCanvas)
                                {
                                    pipeOnCanvas = controlOnCanvas as PipeOnCanvas;

                                    TextBoxDiameter.IsReadOnly = false;
                                    ComboBoxEnvironment.IsEnabled = true;
                                    TextBoxDiameter.Text = string.Format("{0:F2}", pipeOnCanvas.Diameter);
                                    ComboBoxEnvironment.SelectedIndex = pipeOnCanvas.IntEnvironment;
                                }
                            }
                        }
                    }
                    else if (canvasAS.CountSelect == 0)
                    {
                        LabelSelected.Content = "Выделенно объектов: " + canvasAS.CountSelect;

                        CoordinateObjectX.IsReadOnly = true;
                        CoordinateObjectY.IsReadOnly = true;
                        CoordinateObjectX.Text = null;
                        CoordinateObjectY.Text = null;

                        TextBoxDiameter.IsReadOnly = true;
                        ComboBoxEnvironment.IsEnabled = false;
                        TextBoxDiameter.Text = null;
                        ComboBoxEnvironment.SelectedIndex = -1;
                    }
                }
                else if (scroll.Content is CanvasControlPanel)
                {
                    CanvasControlPanel canvasAS = scroll.Content as CanvasControlPanel;

                    TextBoxDiameter.IsReadOnly = false;
                    ComboBoxEnvironment.IsEnabled = false;
                    TextBoxDiameter.Text = null;
                    ComboBoxEnvironment.SelectedIndex = -1;
                    TextBoxDiameter.IsReadOnly = true;

                    if (canvasAS.CountSelect > 1)
                    {
                        CoordinateObjectX.IsReadOnly = true;
                        CoordinateObjectY.IsReadOnly = true;
                        CoordinateObjectX.Text = null;
                        CoordinateObjectY.Text = null;

                        LabelSelected.Content = "Выделенно объектов: " + canvasAS.CountSelect;                        
                    }
                    else if (canvasAS.CountSelect == 1)
                    {                       
                        LabelSelected.Content = "Выделенно объектов: " + canvasAS.CountSelect;

                        foreach (ControlOnCanvas controlOnCanvas in canvasAS.Children)
                        {
                            if (controlOnCanvas.IsSelected)
                            {
                                CoordinateObjectX.IsReadOnly = false;
                                CoordinateObjectY.IsReadOnly = false;
                                if (controlOnCanvas.controlOnCanvasSer.Transform == 0)
                                {
                                    controlOnCanvas.CoordinateX.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.LeftProperty));
                                    controlOnCanvas.CoordinateY.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.TopProperty));
                                }
                                else if (controlOnCanvas.controlOnCanvasSer.Transform == -90 || controlOnCanvas.controlOnCanvasSer.Transform == 270)
                                {
                                    controlOnCanvas.CoordinateY.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.TopProperty) - controlOnCanvas.ActualWidth);
                                    controlOnCanvas.CoordinateX.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.LeftProperty));
                                }
                                else if (controlOnCanvas.controlOnCanvasSer.Transform == -180 || controlOnCanvas.controlOnCanvasSer.Transform == 180)
                                {
                                    controlOnCanvas.CoordinateY.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.TopProperty) - controlOnCanvas.ActualHeight);
                                    controlOnCanvas.CoordinateX.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.LeftProperty) - controlOnCanvas.ActualWidth);
                                }
                                else if (controlOnCanvas.controlOnCanvasSer.Transform == -270 || controlOnCanvas.controlOnCanvasSer.Transform == 90)
                                {
                                    controlOnCanvas.CoordinateY.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.TopProperty));
                                    controlOnCanvas.CoordinateX.Text = string.Format("{0:F2}", (double)controlOnCanvas.GetValue(Canvas.LeftProperty) - controlOnCanvas.ActualHeight);
                                }                                
                            }
                        }
                    }
                    else if (canvasAS.CountSelect == 0)
                    {
                        LabelSelected.Content = "Выделенно объектов: " + canvasAS.CountSelect;

                        CoordinateObjectX.IsReadOnly = true;
                        CoordinateObjectY.IsReadOnly = true;
                        CoordinateObjectX.Text = null;
                        CoordinateObjectY.Text = null;                        
                    }
                }
            }
                      
            e.Handled = true;
        }

        // при переименовании ключа, сначала удаляем из словаря
        public void RemoveCollectionPage(PageScada ps)
        {
            ((AppWPF)Application.Current).CollectionTabItemParent.Remove(ps.Path);
            ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionPageScada.Remove(ps.Path);
            ((AppWPF)Application.Current).CollectionPage.Remove(ps.Path);
        }

        // а сдесь добавляем, уже с новым значением ключа
        public void AddCollectionPage(PageScada ps, TabItemParent tabItemParent, Page page)
        {
            ((AppWPF)Application.Current).CollectionTabItemParent.Add(ps.Path, tabItemParent);
            ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionPageScada.Add(ps.Path, ps);
            ((AppWPF)Application.Current).CollectionPage.Add(ps.Path, page);
        }

        public void AddCollectionPageCopy(PageScada ps, Page page)
        {
            ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionPageScada.Add(ps.Path, ps);
            ((AppWPF)Application.Current).CollectionPage.Add(ps.Path, page);
        }

        // при переименовании ключа, сначала удаляем из словаря
        public void RemoveCollectionControlPanel(ControlPanelScada cps)
        {
            ((AppWPF)Application.Current).CollectionTabItemParent.Remove(cps.Path);
            ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionControlPanelScada.Remove(cps.Path);
            ((AppWPF)Application.Current).CollectionControlPanel.Remove(cps.Path);
        }

        // а сдесь добавляем, уже с новым значением ключа
        public void AddCollectionControlPanel(ControlPanelScada cps, TabItemParent tabItemParent, ControlPanel cpanel)
        {
            ((AppWPF)Application.Current).CollectionTabItemParent.Add(cps.Path, tabItemParent);
            ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionControlPanelScada.Add(cps.Path, cps);
            ((AppWPF)Application.Current).CollectionControlPanel.Add(cps.Path, cpanel);
        }

        public void AddCollectionControlPanelCopy(ControlPanelScada cps, ControlPanel cpanel)
        {
            ((MainWindow)((AppWPF)Application.Current).MainWindow).ProjectBin.CollectionControlPanelScada.Add(cps.Path, cps);
            ((AppWPF)Application.Current).CollectionControlPanel.Add(cps.Path, cpanel);
        }

        public void UpdateThread()
        {
            if (((AppWPF)Application.Current).ConfigProgramBin.IsWindowUpdate)
            {

            }
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {            
            if (((AppWPF)Application.Current).ConfigProgramBin.IsWindowErrorMessage)
            {
                WindowErrorMessages.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                WindowErrorMessages.Visibility = System.Windows.Visibility.Collapsed;
            }

            Thread updateThread = new Thread(UpdateThread);
            updateThread.IsBackground = true;
            updateThread.Priority = ThreadPriority.Lowest;
            updateThread.Start();

            WindowErrorMessages.LBMessageError.ItemsSource = CollectionMessage;

            Binding bindingBArchive = new Binding();          
            bindingBArchive.Source = ((AppWPF)Application.Current).ConfigProgramBin;
            bindingBArchive.Path = new PropertyPath("UseDatabase");

            BArchive.SetBinding(Button.IsEnabledProperty, bindingBArchive);

            Binding bindingBOpenProject = new Binding();
            bindingBOpenProject.Source = this;
            bindingBOpenProject.Path = new PropertyPath("IsBindingStartProject");
            bindingBOpenProject.Converter = new EnableButtonsStartValueConverter();

            BOpenProject.SetBinding(Button.IsEnabledProperty, bindingBOpenProject);

            Binding bindingBCreateProject = new Binding();
            bindingBCreateProject.Source = this;
            bindingBCreateProject.Path = new PropertyPath("IsBindingStartProject");
            bindingBCreateProject.Converter = new EnableButtonsStartValueConverter();

            BCreateProject.SetBinding(Button.IsEnabledProperty, bindingBCreateProject);

            Binding bindingElementsEnabled = new Binding();
            bindingElementsEnabled.Converter = new EnableButtonsStartValueConverter();
            bindingElementsEnabled.Source = this;
            bindingElementsEnabled.Path = new PropertyPath("IsBindingStartProject");

            Elements.SetBinding(TreeView.IsEnabledProperty, bindingElementsEnabled);

            Binding bindingBOpenFile = new Binding();
            bindingBOpenFile.Source = this;
            bindingBOpenFile.Path = new PropertyPath("IsBindingStartProject");

            Binding bindingBOpenFile2 = new Binding();
            bindingBOpenFile2.Source = this;
            bindingBOpenFile2.Path = new PropertyPath("ProjectBin");

            MultiBinding multiBindingBCreatePage = new MultiBinding();
            multiBindingBCreatePage.Converter = new EnableOpenCreateFileValueConverter();
            multiBindingBCreatePage.Bindings.Add(bindingBOpenFile);
            multiBindingBCreatePage.Bindings.Add(bindingBOpenFile2);

            BCreatePage.SetBinding(Button.IsEnabledProperty, multiBindingBCreatePage);

            Binding bindingBCreateFile = new Binding();
            bindingBCreateFile.Source = this;
            bindingBCreateFile.Path = new PropertyPath("IsBindingStartProject");

            Binding bindingBCreateFile2 = new Binding();
            bindingBCreateFile2.Source = this;
            bindingBCreateFile2.Path = new PropertyPath("ProjectBin");

            MultiBinding multiBindingBOpenFile = new MultiBinding();
            multiBindingBOpenFile.Converter = new EnableOpenCreateFileValueConverter();
            multiBindingBOpenFile.Bindings.Add(bindingBCreateFile);
            multiBindingBOpenFile.Bindings.Add(bindingBCreateFile2);

            BOpenFile.SetBinding(Button.IsEnabledProperty, multiBindingBOpenFile);

            Binding bindingBSaveAll = new Binding();
            bindingBSaveAll.Source = this;
            bindingBSaveAll.Path = new PropertyPath("IsBindingStartProject");

            Binding bindingBSaveAll2 = new Binding();
            bindingBSaveAll2.Source = this;
            bindingBSaveAll2.Path = new PropertyPath("ProjectBin");

            MultiBinding multiBindingBSaveAll = new MultiBinding();
            multiBindingBSaveAll.Converter = new EnableOpenCreateFileValueConverter();
            multiBindingBSaveAll.Bindings.Add(bindingBSaveAll);
            multiBindingBSaveAll.Bindings.Add(bindingBSaveAll2);

            BSaveAll.SetBinding(Button.IsEnabledProperty, multiBindingBSaveAll);

            Binding bindingBSave = new Binding();
            bindingBSave.Source = this;
            bindingBSave.Path = new PropertyPath("IsBindingStartProject");

            Binding bindingBSave2 = new Binding();
            bindingBSave2.Source = this;
            bindingBSave2.Path = new PropertyPath("ProjectBin");

            Binding bindingBSave3 = new Binding();
            bindingBSave3.Source = TabControlMain;
            bindingBSave3.Path = new PropertyPath("SelectedItem");

            MultiBinding multiBindingBSave = new MultiBinding();
            multiBindingBSave.Converter = new EnableSaveValueConverter();
            multiBindingBSave.Bindings.Add(bindingBSave);
            multiBindingBSave.Bindings.Add(bindingBSave2);
            multiBindingBSave.Bindings.Add(bindingBSave3);

            BSave.SetBinding(Button.IsEnabledProperty, multiBindingBSave);

            Binding bindingMISave = new Binding();
            bindingMISave.Source = this;
            bindingMISave.Path = new PropertyPath("IsBindingStartProject");

            Binding bindingMISave2 = new Binding();
            bindingMISave2.Source = this;
            bindingMISave2.Path = new PropertyPath("ProjectBin");

            Binding bindingMISave3 = new Binding();
            bindingMISave3.Source = TabControlMain;
            bindingMISave3.Path = new PropertyPath("SelectedItem");

            MultiBinding multiBindingMISave = new MultiBinding();
            multiBindingMISave.Converter = new EnableSaveValueConverter();
            multiBindingMISave.Bindings.Add(bindingMISave);
            multiBindingMISave.Bindings.Add(bindingMISave2);
            multiBindingMISave.Bindings.Add(bindingMISave3);

            MISave.SetBinding(Button.IsEnabledProperty, multiBindingMISave);

            Binding bindingMISaveAs = new Binding();
            bindingMISaveAs.Source = this;
            bindingMISaveAs.Path = new PropertyPath("IsBindingStartProject");

            Binding bindingMISaveAs2 = new Binding();
            bindingMISaveAs2.Source = this;
            bindingMISaveAs2.Path = new PropertyPath("ProjectBin");

            Binding bindingMISaveAs3 = new Binding();
            bindingMISaveAs3.Source = TabControlMain;
            bindingMISaveAs3.Path = new PropertyPath("SelectedItem");

            MultiBinding multiBindingMISaveAs = new MultiBinding();
            multiBindingMISaveAs.Converter = new EnableSaveValueConverter();
            multiBindingMISaveAs.Bindings.Add(bindingMISaveAs);
            multiBindingMISaveAs.Bindings.Add(bindingMISaveAs2);
            multiBindingMISaveAs.Bindings.Add(bindingMISaveAs3);

            MISaveAs.SetBinding(Button.IsEnabledProperty, multiBindingMISaveAs);

            Binding bindingMISaveAll = new Binding();
            bindingMISaveAll.Source = this;
            bindingMISaveAll.Path = new PropertyPath("IsBindingStartProject");

            Binding bindingMISaveAll2 = new Binding();
            bindingMISaveAll2.Source = this;
            bindingMISaveAll2.Path = new PropertyPath("ProjectBin");

            MultiBinding multiBindingMISaveAll = new MultiBinding();
            multiBindingMISaveAll.Converter = new EnableOpenCreateFileValueConverter();
            multiBindingMISaveAll.Bindings.Add(bindingMISaveAll);
            multiBindingMISaveAll.Bindings.Add(bindingMISaveAll2);

            MISaveAll.SetBinding(Button.IsEnabledProperty, multiBindingMISaveAll);

            Binding bindingMIOpenProject = new Binding();
            bindingMIOpenProject.Source = this;
            bindingMIOpenProject.Path = new PropertyPath("IsBindingStartProject");
            bindingMIOpenProject.Converter = new EnableButtonsStartValueConverter();

            MIOpenProject.SetBinding(Button.IsEnabledProperty, bindingMIOpenProject);

            Binding bindingMICreateProject = new Binding();
            bindingMICreateProject.Source = this;
            bindingMICreateProject.Path = new PropertyPath("IsBindingStartProject");
            bindingMICreateProject.Converter = new EnableButtonsStartValueConverter();

            MICreateProject.SetBinding(Button.IsEnabledProperty, bindingMICreateProject);

            Binding bindingMICreateFile = new Binding();
            bindingMICreateFile.Source = this;
            bindingMICreateFile.Path = new PropertyPath("IsBindingStartProject");

            Binding bindingMICreateFile2 = new Binding();
            bindingMICreateFile2.Source = this;
            bindingMICreateFile2.Path = new PropertyPath("ProjectBin");

            MultiBinding multiBindingMICreateFile = new MultiBinding();
            multiBindingMICreateFile.Converter = new EnableOpenCreateFileValueConverter();
            multiBindingMICreateFile.Bindings.Add(bindingMICreateFile);
            multiBindingMICreateFile.Bindings.Add(bindingMICreateFile2);

            MICreatePage.SetBinding(Button.IsEnabledProperty, multiBindingMICreateFile);

            Binding bindingMIOpenFile = new Binding();
            bindingMIOpenFile.Source = this;
            bindingMIOpenFile.Path = new PropertyPath("IsBindingStartProject");

            Binding bindingMIOpenFile2 = new Binding();
            bindingMIOpenFile2.Source = this;
            bindingMIOpenFile2.Path = new PropertyPath("ProjectBin");

            MultiBinding multiBindingMIOpenFile = new MultiBinding();
            multiBindingMIOpenFile.Converter = new EnableOpenCreateFileValueConverter();
            multiBindingMIOpenFile.Bindings.Add(bindingMIOpenFile);
            multiBindingMIOpenFile.Bindings.Add(bindingMIOpenFile2);

            MIOpenFile.SetBinding(Button.IsEnabledProperty, multiBindingMIOpenFile);

            Binding bindingStartCount = new Binding();
            bindingStartCount.Source = ((AppWPF)Application.Current).CollectionEthernetSers;
            bindingStartCount.Path = new PropertyPath("Count");

            Binding bindingStartCountModbus = new Binding();
            bindingStartCountModbus.Source = ((AppWPF)Application.Current).CollectionModbusSers;
            bindingStartCountModbus.Path = new PropertyPath("Count");

            Binding bindingIsBindingStartProject = new Binding();
            bindingIsBindingStartProject.Source = this;
            bindingIsBindingStartProject.Path = new PropertyPath("IsBindingStartProject");

            MultiBinding multiBindingStart = new MultiBinding();
            multiBindingStart.Converter = new ButtonStartValueConverter();
            multiBindingStart.Bindings.Add(bindingStartCount);
            multiBindingStart.Bindings.Add(bindingStartCountModbus);
            multiBindingStart.Bindings.Add(bindingIsBindingStartProject);

            BStartProject.SetBinding(Button.IsEnabledProperty, multiBindingStart);
            
            Binding bindingStop = new Binding();
            bindingStop.Source = this;
            bindingStop.Path = new PropertyPath("IsBindingStartProject");

            BStopProject.SetBinding(Button.IsEnabledProperty, bindingStop);

            DragAndDropCanvas pipe = new DragAndDropCanvas();
            pipe.IsPipe = true;

            DragAndDropCanvas pipe90 = new DragAndDropCanvas();
            pipe90.IsPipe90 = true;

            DragAndDropCanvas text = new DragAndDropCanvas();
            text.IsText = true;

            DragAndDropCanvas ethernet = new DragAndDropCanvas();
            ethernet.IsEthernet = true;

            DragAndDropCanvas display = new DragAndDropCanvas();
            display.IsDisplay = true;

            DragAndDropCanvas com = new DragAndDropCanvas();
            com.IsCom = true;

            DragAndDropCanvas image = new DragAndDropCanvas();
            image.IsImage = true;

            DragAndDropCanvas modbus = new DragAndDropCanvas();
            modbus.IsModbus = true;

            DragPipe.Tag = pipe;
            DragPipe90.Tag = pipe90;
            DragText.Tag = text;
            DragEthernet.Tag = ethernet;
            DragCom.Tag = com;
            DragDisplay.Tag = display;
            DragImageControl.Tag = image;
            DragModbus.Tag = modbus;

            IDataObject iData = Clipboard.GetDataObject();

            if (iData.GetDataPresent("SCADA.FolderScada"))
            {
                IsBindingInsert = true;
            }
            else if (iData.GetDataPresent("SCADA.PageScada"))
            {
                IsBindingInsert = true;
            }
            else if (iData.GetDataPresent("SCADA.ControlPanelScada"))
            {
                IsBindingInsert = true;
            }
            else if (iData.GetDataPresent("SCADA.ClipboardManipulation"))
            {
                IsBindingInsertObject = true;
            }
            else if (iData.GetDataPresent(DataFormats.StringFormat))
            {
                IsBindingInsertText = true;
            }
                        
            this.FontSize = 16;

            ComboBoxItem itemExhaus = new ComboBoxItem();
            itemExhaus.Content = "Уходящие газы";

            ComboBoxItem itemSteam = new ComboBoxItem();
            itemSteam.Content = "Пар";

            ComboBoxItem itemWater = new ComboBoxItem();
            itemWater.Content = "Вода";

            ComboBoxItem itemMasut = new ComboBoxItem();
            itemMasut.Content = "Мазут";

            ComboBoxItem itemAir = new ComboBoxItem();
            itemAir.Content = "Воздух";

            ComboBoxEnvironment.Items.Add(itemExhaus);
            ComboBoxEnvironment.Items.Add(itemMasut);
            ComboBoxEnvironment.Items.Add(itemWater);
            ComboBoxEnvironment.Items.Add(itemSteam);
            ComboBoxEnvironment.Items.Add(itemAir);

            // Перехват левый щелчек мыши вниз
            Mouse.AddPreviewMouseDownHandler(this, InterceptionMouseDown);

            // Заменяем стандартное контекстное меню текстбокса и удаляем меню вырезать и вставить
            MenuItem menuItemCopy = new MenuItem();
            menuItemCopy.Command = ApplicationCommands.Copy;

            ContextMenu contextMenuIgnorePast = new System.Windows.Controls.ContextMenu();
            contextMenuIgnorePast.Items.Add(menuItemCopy);

            CoordinateObjectX.ContextMenu = contextMenuIgnorePast;
            CoordinateObjectY.ContextMenu = contextMenuIgnorePast;
            TextBoxDiameter.ContextMenu = contextMenuIgnorePast;
                     
            CoordinateObjectX.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
            CoordinateObjectY.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
            TextBoxDiameter.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
            CoordinateObjectX.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, IgnorePaste));
            CoordinateObjectY.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, IgnorePaste));
            TextBoxDiameter.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, IgnorePaste));

            string[] arguments = Environment.GetCommandLineArgs();
            if (arguments.Length == 2)
            {
                string filePath = arguments[1];

                try
                {
                    using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
                    {
                        BinaryFormatter Projserializer = new BinaryFormatter();
                        OpenProjectFile((SerializationProject)Projserializer.Deserialize(fs));                      
                    }
                }
                catch (SystemException ex)
                {
                    MessageBox.Show(ex.ToString(), "Ошибка открытия проекта", MessageBoxButton.OK);

                    return;
                }
            }
            else if (arguments.Length > 2)
            {
                try
                {
                    string s = arguments[2];

                    if (s == "start")
                    {
                        string filePath = arguments[1];

                        using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
                        {
                            BinaryFormatter Projserializer = new BinaryFormatter();
                            OpenProjectFile((SerializationProject)Projserializer.Deserialize(fs));
                            BStartProject_Click(null, null);
                        }                     
                    }
                }
                catch (SystemException ex)
                {
                    MessageBox.Show(ex.ToString(), "Ошибка открытия проекта", MessageBoxButton.OK);

                    return;
                }               
            }
        }

        private void IgnoreCut(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void IgnorePaste(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        public void InterceptionMouseDown(object sender, MouseButtonEventArgs e)
        {              
            if (CoordinateObjectX.IsKeyboardFocused && !CoordinateObjectX.IsMouseOver && !CoordinateObjectX.IsReadOnly)
            {                                           
                Keyboard.Focus(null);
                FocusManager.SetFocusedElement(BottomPanel, BottomPanel);
                
                // Первый клик левой кнопкой мышки после потери фокуса CoordinateObjectX игнорируется
                e.Handled = true;
                return;                      
            }
            else if (CoordinateObjectY.IsKeyboardFocused && !CoordinateObjectY.IsMouseOver && !CoordinateObjectY.IsReadOnly)
            {               
                Keyboard.Focus(null);
                FocusManager.SetFocusedElement(BottomPanel, BottomPanel);

                e.Handled = true;
                return;            
            }
            else if (TextBoxDiameter.IsKeyboardFocused && !TextBoxDiameter.IsMouseOver && !TextBoxDiameter.IsReadOnly)
            {                
                Keyboard.Focus(null);
                FocusManager.SetFocusedElement(BottomPanel, BottomPanel);

                e.Handled = true;
                return;                                    
            }          
        }

        // При изменени среды трубопровода в ComboBox в главном окне
        private void EnvironmentChange(object sender, SelectionChangedEventArgs e)
        {
            // При сбросе выделения с контрола или при не совпадении сред контролов при выделении, ничего не происходит
            if (ComboBoxEnvironment.SelectedIndex == -1)
            {
                e.Handled = true;
                return;
            }

            CanvasTab selectedCanvas = null;
            ScrollViewer selectedScrollViewer = null;
            
            selectedScrollViewer = TabControlMain.SelectedContent as ScrollViewer;

            if (selectedScrollViewer.Content is CanvasPage)
            {
                selectedCanvas = selectedScrollViewer.Content as CanvasPage;
            }
            else if (selectedScrollViewer.Content is CanvasControlPanel)
            {
                e.Handled = true;
                return;
            }
            
            foreach (ControlOnCanvas controlOnCanvas in selectedCanvas.SelectedControlOnCanvas)
            {
                if (controlOnCanvas is Pipe)
                {
                    Pipe pipe = controlOnCanvas as Pipe;

                    if (pipe.PipeSer.Environment != ComboBoxEnvironment.SelectedIndex)
                    {                        
                        pipe.IntEnvironment = ComboBoxEnvironment.SelectedIndex;
                    }
                }
                else if (controlOnCanvas is Pipe90)
                {
                    Pipe90 pipe90 = controlOnCanvas as Pipe90;

                    if (pipe90.Pipe90Ser.Environment != ComboBoxEnvironment.SelectedIndex)
                    {                       
                        pipe90.IntEnvironment = ComboBoxEnvironment.SelectedIndex;
                    }
                }
            }

            ((AppWPF)Application.Current).SaveTabItem(selectedCanvas.TabItemParent);
 
            e.Handled = true;
        }  

        // При получении фокуса textbox, весь текст выделяется
        private void SelectionAllClick(object sender, RoutedEventArgs e)
        {
            TextBox focusableTextBox = (TextBox)sender;
            double d;

            if (!focusableTextBox.IsReadOnly)
            {
                if (double.TryParse(focusableTextBox.Text, out d) || focusableTextBox.Text == "-")
                {
                    if (focusableTextBox == CoordinateObjectX || focusableTextBox == CoordinateObjectY)
                    {
                        if (!(d > 10000)) focusableTextBox.Tag = focusableTextBox.Text;
                    }
                    else if (focusableTextBox == TextBoxDiameter)
                    {
                        if (!(d > 1000) && !(d < 20) || focusableTextBox.Text == "-") focusableTextBox.Tag = focusableTextBox.Text;
                    }                   
                }               
            }

            e.Handled = true;
        }

        private void CheckKey(object sender, KeyEventArgs e)
        {          
            TextBox tb = (TextBox)sender;

            if (tb.IsReadOnly)
            {
                e.Handled = true;
                return;
            }
            else if (e.Key == Key.Tab)
            {
                IsTabFocus = true;
            }

            if (e.Key == Key.Escape)
            {               
                tb.Text = string.Format("{0:F2}", tb.Tag);

                Keyboard.Focus(null);
                FocusManager.SetFocusedElement(this, BottomPanel);
            }
            else if (e.Key == Key.Enter)
            {
                CanvasTab selectedCanvas = null;
                ControlOnCanvas selectedControlOncanvas = null;
                ControlOnCanvasSer selectedControlOnCanvasSer = null;
                ScrollViewer selectedScrollViewer = null;

                selectedScrollViewer = TabControlMain.SelectedContent as ScrollViewer;

                if (selectedScrollViewer.Content is CanvasPage)
                {
                    selectedCanvas = selectedScrollViewer.Content as CanvasPage;
                    selectedControlOncanvas = selectedCanvas.SelectedControlOnCanvas[0];

                    selectedControlOnCanvasSer = selectedControlOncanvas.controlOnCanvasSer;
                }

                if (tb == CoordinateObjectX)
                {
                    double d;
                    if (double.TryParse(CoordinateObjectX.Text, out d))
                    {
                        if (d > 50000)
                        {
                            MessageBox.Show("Координата X не может быть больше 50000", "Ошибка диапазона", MessageBoxButton.OK, MessageBoxImage.Warning);
                            e.Handled = true;
                            return;
                        }
                        else
                        {
                            d = Math.Floor(d * 100) / 100;

                            // Если новоя координата равна старой ничего не происходит
                            if (string.Format("{0:F2}", d) != (string)CoordinateObjectX.Tag)
                            {
                                Canvas.SetLeft(selectedControlOncanvas, d);

                                selectedCanvas.RepositionAllObjects(selectedCanvas);
                                selectedCanvas.InvalidateMeasure();

                                selectedControlOnCanvasSer.Сoordinates = new Point(Canvas.GetLeft(selectedControlOncanvas), Canvas.GetTop(selectedControlOncanvas));

                                ((AppWPF)Application.Current).SaveTabItem(selectedCanvas.TabItemParent);

                                if (selectedControlOnCanvasSer.Transform == 0)
                                {
                                    CoordinateObjectX.Text = string.Format("{0:F2}", d);
                                }
                                else if (selectedControlOnCanvasSer.Transform == -90 || selectedControlOnCanvasSer.Transform == 270)
                                {
                                    CoordinateObjectX.Text = string.Format("{0:F2}", d);
                                }
                                else if (selectedControlOnCanvasSer.Transform == -180 || selectedControlOnCanvasSer.Transform == 180)
                                {
                                    CoordinateObjectX.Text = string.Format("{0:F2}", d - selectedControlOncanvas.ActualWidth);
                                }
                                else if (selectedControlOnCanvasSer.Transform == -270 || selectedControlOnCanvasSer.Transform == 90)
                                {
                                    CoordinateObjectX.Text = string.Format("{0:F2}", d - selectedControlOncanvas.ActualHeight);
                                }

                                ((AppWPF)Application.Current).SaveTabItem(selectedCanvas.TabItemParent);

                                tb.Tag = string.Format("{0:F2}", d);
                                tb.Text = string.Format("{0:F2}", d);
                            }
                            
                            tb.SelectAll();

                            e.Handled = true;
                            return;
                        }
                    }
                    else
                    {                     
                        MessageBox.Show("Не верный формат координаты", "Ошибка формата", MessageBoxButton.OK, MessageBoxImage.Warning);
                        e.Handled = true;
                        return;
                    }
                }
                else if (tb == CoordinateObjectY)
                {
                    double d;
                    if (double.TryParse(CoordinateObjectY.Text, out d))
                    {
                        if (d > 50000)
                        {
                            MessageBox.Show("Координата Y не может быть больше 50000", "Ошибка диапазона", MessageBoxButton.OK, MessageBoxImage.Warning);
                            e.Handled = true;
                            return;
                        }
                        else
                        {
                            d = Math.Floor(d * 100) / 100;

                            if (string.Format("{0:F2}", d) != (string)CoordinateObjectY.Tag)
                            {
                                Canvas.SetTop(selectedControlOncanvas, d);

                                selectedCanvas.RepositionAllObjects(selectedCanvas);
                                selectedCanvas.InvalidateMeasure();

                                selectedControlOnCanvasSer.Сoordinates = new Point(Canvas.GetLeft(selectedControlOncanvas), Canvas.GetTop(selectedControlOncanvas));

                                ((AppWPF)Application.Current).SaveTabItem(selectedCanvas.TabItemParent);

                                if (selectedControlOnCanvasSer.Transform == 0)
                                {
                                    CoordinateObjectY.Text = string.Format("{0:F2}", d);
                                }
                                else if (selectedControlOnCanvasSer.Transform == -90 || selectedControlOnCanvasSer.Transform == 270)
                                {
                                    CoordinateObjectY.Text = string.Format("{0:F2}", d);
                                }
                                else if (selectedControlOnCanvasSer.Transform == -180 || selectedControlOnCanvasSer.Transform == 180)
                                {
                                    CoordinateObjectY.Text = string.Format("{0:F2}", d - selectedControlOncanvas.ActualWidth);
                                }
                                else if (selectedControlOnCanvasSer.Transform == -270 || selectedControlOnCanvasSer.Transform == 90)
                                {
                                    CoordinateObjectY.Text = string.Format("{0:F2}", d - selectedControlOncanvas.ActualHeight);
                                }

                                ((AppWPF)Application.Current).SaveTabItem(selectedCanvas.TabItemParent);

                                tb.Tag = string.Format("{0:F2}", d);
                                tb.Text = string.Format("{0:F2}", d);
                            }
                                                     
                            tb.SelectAll();

                            e.Handled = true;
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Не верный формат координаты", "Ошибка формата", MessageBoxButton.OK, MessageBoxImage.Warning);
                        e.Handled = true;
                        return;
                    }
                }
                else if (tb == TextBoxDiameter)
                {
                    double d;
                    if (double.TryParse(TextBoxDiameter.Text, out d))
                    {
                        if (d > 500)
                        {
                            MessageBox.Show("Диаметр не может быть больше 500", "Ошибка диапазона", MessageBoxButton.OK, MessageBoxImage.Warning);
                            e.Handled = true;
                            return;
                        }
                        else if (d < 10)
                        {
                            MessageBox.Show("Диаметр не может быть меньше 10", "Ошибка диапазона", MessageBoxButton.OK, MessageBoxImage.Warning);
                            e.Handled = true;
                            return;
                        }
                        else
                        {
                            d = Math.Floor(d * 100) / 100;

                            if (string.Format("{0:F2}", d) != (string)TextBoxDiameter.Tag)
                            {
                                foreach (ControlOnCanvas controlOnCanvasPage in selectedCanvas.SelectedControlOnCanvas)
                                {
                                    if (controlOnCanvasPage is PipeOnCanvas)
                                    {
                                        PipeOnCanvas pipeOnCanvas = controlOnCanvasPage as PipeOnCanvas;

                                        if (Math.Floor(pipeOnCanvas.Diameter * 100) / 100 != d)
                                        {
                                            if (controlOnCanvasPage is Pipe)
                                            {
                                                #region Pipe
                                                Pipe pipe = controlOnCanvasPage as Pipe;

                                                double delta = d - pipe.Diameter;

                                                Point topSizePoint = pipe.PathFigureTopSize.StartPoint;

                                                Point downSizePoint = pipe.PathFigureDownSize.StartPoint;
                                                downSizePoint.Y += delta;
                                                pipe.PathFigureDownSize.StartPoint = downSizePoint;
                                                Point downSizePoint2 = pipe.LineSegmentDownSize.Point;
                                                downSizePoint2.Y += delta;
                                                pipe.LineSegmentDownSize.Point = downSizePoint2;

                                                Point leftSizePoint = pipe.LineSegmentLeftSize.Point;
                                                leftSizePoint.Y += delta;
                                                pipe.LineSegmentLeftSize.Point = leftSizePoint;

                                                Point rightSizePoint = pipe.PathFigureRightSize.StartPoint;
                                                rightSizePoint.Y += delta;
                                                pipe.PathFigureRightSize.StartPoint = rightSizePoint;

                                                Point rightFlangePoint2 = pipe.PolyLineSegmentRightFlange.Points[1];
                                                Point rightFlangePoint3 = pipe.PolyLineSegmentRightFlange.Points[2];
                                                rightFlangePoint2.Y += delta;
                                                rightFlangePoint3.Y += delta;
                                                pipe.PolyLineSegmentRightFlange.Points[1] = rightFlangePoint2;
                                                pipe.PolyLineSegmentRightFlange.Points[2] = rightFlangePoint3;

                                                Point leftFlangePoint2 = pipe.PolyLineSegmentLeftFlange.Points[1];
                                                Point leftFlangePoint3 = pipe.PolyLineSegmentLeftFlange.Points[2];
                                                leftFlangePoint2.Y += delta;
                                                leftFlangePoint3.Y += delta;
                                                pipe.PolyLineSegmentLeftFlange.Points[1] = leftFlangePoint2;
                                                pipe.PolyLineSegmentLeftFlange.Points[2] = leftFlangePoint3;

                                                Point borderPipePoint2 = pipe.PolyLineSegmentBorder.Points[1];
                                                Point borderPipePoint3 = pipe.PolyLineSegmentBorder.Points[2];
                                                borderPipePoint2.Y += delta;
                                                pipe.PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                                                borderPipePoint3.Y += delta;
                                                pipe.PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                                                Point pointPipe = pipe.PathFigurePipe.StartPoint;
                                                pointPipe.Y += delta;
                                                pipe.PathFigurePipe.StartPoint = pointPipe;
                                                Point pointPipe2 = pipe.PolyLineSegmentPipe.Points[2];
                                                Point pointPipe3 = pipe.PolyLineSegmentPipe.Points[3];
                                                pointPipe2.Y += delta;
                                                pointPipe3.Y += delta;
                                                pipe.PolyLineSegmentPipe.Points[2] = pointPipe2;
                                                pipe.PolyLineSegmentPipe.Points[3] = pointPipe3;

                                                pipe.Diameter = d;
                                                #endregion

                                                #region SavePipeSer
                                                pipe.PipeSer.LeftSize.point[0] = pipe.PathFigureLeftSize.StartPoint;
                                                pipe.PipeSer.LeftSize.point[1] = pipe.LineSegmentLeftSize.Point;

                                                pipe.PipeSer.RightSize.point[0] = pipe.PathFigureRightSize.StartPoint;
                                                pipe.PipeSer.RightSize.point[1] = pipe.LineSegmentRightSize.Point;

                                                pipe.PipeSer.TopSize.point[0] = pipe.PathFigureTopSize.StartPoint;
                                                pipe.PipeSer.TopSize.point[1] = pipe.LineSegmentTopSize.Point;

                                                pipe.PipeSer.DownSize.point[0] = pipe.PathFigureDownSize.StartPoint;
                                                pipe.PipeSer.DownSize.point[1] = pipe.LineSegmentDownSize.Point;

                                                pipe.PipeSer.LeftFlange.point[0] = pipe.PathFigureLeftFlange.StartPoint;
                                                pipe.PipeSer.LeftFlange.point[1] = pipe.PolyLineSegmentLeftFlange.Points[0];
                                                pipe.PipeSer.LeftFlange.point[2] = pipe.PolyLineSegmentLeftFlange.Points[1];
                                                pipe.PipeSer.LeftFlange.point[3] = pipe.PolyLineSegmentLeftFlange.Points[2];
                                                pipe.PipeSer.LeftFlange.point[4] = pipe.PolyLineSegmentLeftFlange.Points[3];

                                                pipe.PipeSer.RightFlange.point[0] = pipe.PathFigureRightFlange.StartPoint;
                                                pipe.PipeSer.RightFlange.point[1] = pipe.PolyLineSegmentRightFlange.Points[0];
                                                pipe.PipeSer.RightFlange.point[2] = pipe.PolyLineSegmentRightFlange.Points[1];
                                                pipe.PipeSer.RightFlange.point[3] = pipe.PolyLineSegmentRightFlange.Points[2];
                                                pipe.PipeSer.RightFlange.point[4] = pipe.PolyLineSegmentRightFlange.Points[3];

                                                pipe.PipeSer.Pipe.point[0] = pipe.PathFigurePipe.StartPoint;
                                                pipe.PipeSer.Pipe.point[1] = pipe.PolyLineSegmentPipe.Points[0];
                                                pipe.PipeSer.Pipe.point[2] = pipe.PolyLineSegmentPipe.Points[1];
                                                pipe.PipeSer.Pipe.point[3] = pipe.PolyLineSegmentPipe.Points[2];
                                                pipe.PipeSer.Pipe.point[4] = pipe.PolyLineSegmentPipe.Points[3];

                                                pipe.PipeSer.BorderPipe.point[0] = pipe.PathFigureBorder.StartPoint;
                                                pipe.PipeSer.BorderPipe.point[1] = pipe.PolyLineSegmentBorder.Points[0];
                                                pipe.PipeSer.BorderPipe.point[2] = pipe.PolyLineSegmentBorder.Points[1];
                                                pipe.PipeSer.BorderPipe.point[3] = pipe.PolyLineSegmentBorder.Points[2];
                                                pipe.PipeSer.BorderPipe.point[4] = pipe.PolyLineSegmentBorder.Points[3];
                                                #endregion

                                                if (selectedCanvas.SelectedControlOnCanvas.Count == 1)
                                                {
                                                    if (selectedControlOnCanvasSer.Transform == 0)
                                                    {
                                                        CoordinateObjectX.Text = string.Format("{0:F2}", (double)selectedControlOncanvas.GetValue(Canvas.LeftProperty));
                                                        CoordinateObjectY.Text = string.Format("{0:F2}", (double)selectedControlOncanvas.GetValue(Canvas.TopProperty));
                                                    }
                                                    else if (selectedControlOnCanvasSer.Transform == -90 || selectedControlOnCanvasSer.Transform == 270)
                                                    {
                                                        CoordinateObjectY.Text = string.Format("{0:F2}", (double)selectedControlOncanvas.GetValue(Canvas.TopProperty) - selectedControlOncanvas.ActualWidth);
                                                        CoordinateObjectX.Text = string.Format("{0:F2}", (double)selectedControlOncanvas.GetValue(Canvas.LeftProperty));
                                                    }
                                                    else if (selectedControlOnCanvasSer.Transform == -180 || selectedControlOnCanvasSer.Transform == 180)
                                                    {
                                                        CoordinateObjectY.Text = string.Format("{0:F2}", (double)selectedControlOncanvas.GetValue(Canvas.TopProperty) - selectedControlOncanvas.ActualHeight);
                                                        CoordinateObjectX.Text = string.Format("{0:F2}", (double)selectedControlOncanvas.GetValue(Canvas.LeftProperty) - selectedControlOncanvas.ActualWidth);
                                                    }
                                                    else if (selectedControlOnCanvasSer.Transform == -270 || selectedControlOncanvas.controlOnCanvasSer.Transform == 90)
                                                    {
                                                        CoordinateObjectY.Text = string.Format("{0:F2}", (double)selectedControlOncanvas.GetValue(Canvas.TopProperty));
                                                        CoordinateObjectX.Text = string.Format("{0:F2}", (double)selectedControlOncanvas.GetValue(Canvas.LeftProperty) - selectedControlOncanvas.ActualHeight);
                                                    }
                                                }
                                            }
                                            else if (controlOnCanvasPage is Pipe90)
                                            {
                                                #region Pipe90
                                                Pipe90 pipe90 = controlOnCanvasPage as Pipe90;

                                                double delta = d - pipe90.Diameter;

                                                Point topSizePoint = pipe90.PathFigureTopSize.StartPoint;
                                                topSizePoint.X += delta;
                                                pipe90.PathFigureTopSize.StartPoint = topSizePoint;

                                                Point downSizePoint = pipe90.PathFigureDownSize.StartPoint;
                                                downSizePoint.Y += delta;
                                                downSizePoint.X += delta;
                                                pipe90.PathFigureDownSize.StartPoint = downSizePoint;
                                                Point downSizePoint2 = pipe90.LineSegmentDownSize.Point;
                                                downSizePoint2.Y += delta;
                                                downSizePoint2.X += delta;
                                                pipe90.LineSegmentDownSize.Point = downSizePoint2;

                                                Point leftSizePoint2 = pipe90.PathFigureTopLenghtSize.StartPoint;
                                                leftSizePoint2.X += delta;
                                                pipe90.PathFigureTopLenghtSize.StartPoint = leftSizePoint2;
                                                Point leftSizePoint = pipe90.LineSegmentTopLenghtSize.Point;
                                                leftSizePoint.Y += delta;
                                                leftSizePoint.X += delta;
                                                pipe90.LineSegmentTopLenghtSize.Point = leftSizePoint;

                                                Point rightFlangePoint = pipe90.PathFigureRightFlange.StartPoint;
                                                rightFlangePoint.X += delta;
                                                rightFlangePoint.Y += delta;
                                                pipe90.PathFigureRightFlange.StartPoint = rightFlangePoint;
                                                Point rightFlangePoint2 = pipe90.PolyLineSegmentRightFlange.Points[0];
                                                Point rightFlangePoint3 = pipe90.PolyLineSegmentRightFlange.Points[3];
                                                Point rightFlangePoint4 = pipe90.PolyLineSegmentRightFlange.Points[1];
                                                Point rightFlangePoint5 = pipe90.PolyLineSegmentRightFlange.Points[2];
                                                rightFlangePoint2.Y += delta;
                                                rightFlangePoint2.X += delta;
                                                rightFlangePoint3.Y += delta;
                                                rightFlangePoint3.X += delta;
                                                rightFlangePoint4.X += delta;
                                                rightFlangePoint5.X += delta;
                                                pipe90.PolyLineSegmentRightFlange.Points[0] = rightFlangePoint2;
                                                pipe90.PolyLineSegmentRightFlange.Points[3] = rightFlangePoint3;
                                                pipe90.PolyLineSegmentRightFlange.Points[1] = rightFlangePoint4;
                                                pipe90.PolyLineSegmentRightFlange.Points[2] = rightFlangePoint5;

                                                Point borderPipePoint1 = pipe90.PolyLineSegmentBorder.Points[0];
                                                Point borderPipePoint2 = pipe90.PolyLineSegmentBorder.Points[1];
                                                Point borderPipePoint3 = pipe90.PolyLineSegmentBorder.Points[2];
                                                borderPipePoint1.X += delta;
                                                pipe90.PolyLineSegmentBorder.Points[0] = borderPipePoint1;
                                                borderPipePoint2.Y += delta;
                                                borderPipePoint2.X += delta;
                                                pipe90.PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                                                borderPipePoint3.Y += delta;
                                                pipe90.PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                                                Point pointPipe = pipe90.PathFigureTopImage.StartPoint;
                                                pointPipe.Y += delta;
                                                pointPipe.X += delta;
                                                pipe90.PathFigureTopImage.StartPoint = pointPipe;
                                                Point pointPipe4 = pipe90.PolyLineSegmentTopImage.Points[1];
                                                Point pointPipe2 = pipe90.PolyLineSegmentTopImage.Points[2];
                                                Point pointPipe3 = pipe90.PolyLineSegmentTopImage.Points[3];
                                                pointPipe2.Y += delta;
                                                pointPipe3.Y += delta;
                                                pointPipe2.X += delta;
                                                pointPipe3.X += delta;
                                                pointPipe4.X += delta;
                                                pipe90.PolyLineSegmentTopImage.Points[1] = pointPipe4;
                                                pipe90.PolyLineSegmentTopImage.Points[2] = pointPipe2;
                                                pipe90.PolyLineSegmentTopImage.Points[3] = pointPipe3;

                                                Point topSizePoint3 = pipe90.PathFigureLeftDownSize.StartPoint;
                                                topSizePoint3.Y += delta;
                                                pipe90.PathFigureLeftDownSize.StartPoint = topSizePoint3;

                                                Point downSizePoint3 = pipe90.PathFigureRightDownSize.StartPoint;
                                                downSizePoint3.Y += delta;
                                                downSizePoint3.X += delta;
                                                pipe90.PathFigureRightDownSize.StartPoint = downSizePoint3;
                                                Point downSizePoint4 = pipe90.LineSegmentRightDownSize.Point;
                                                downSizePoint4.X += delta;
                                                downSizePoint4.Y += delta;
                                                pipe90.LineSegmentRightDownSize.Point = downSizePoint4;

                                                Point leftFlangePoint = pipe90.PathFigureLeftFlange.StartPoint;
                                                leftFlangePoint.Y += delta;
                                                pipe90.PathFigureLeftFlange.StartPoint = leftFlangePoint;
                                                Point leftFlangePoint2 = pipe90.PolyLineSegmentLeftFlange.Points[0];
                                                Point leftFlangePoint3 = pipe90.PolyLineSegmentLeftFlange.Points[1];
                                                Point leftFlangePoint4 = pipe90.PolyLineSegmentLeftFlange.Points[2];
                                                Point leftFlangePoint5 = pipe90.PolyLineSegmentLeftFlange.Points[3];
                                                leftFlangePoint2.Y += delta;
                                                leftFlangePoint3.Y += delta;
                                                leftFlangePoint3.X += delta;
                                                leftFlangePoint4.Y += delta;
                                                leftFlangePoint4.X += delta;
                                                leftFlangePoint5.Y += delta;
                                                pipe90.PolyLineSegmentLeftFlange.Points[0] = leftFlangePoint2;
                                                pipe90.PolyLineSegmentLeftFlange.Points[1] = leftFlangePoint3;
                                                pipe90.PolyLineSegmentLeftFlange.Points[2] = leftFlangePoint4;
                                                pipe90.PolyLineSegmentLeftFlange.Points[3] = leftFlangePoint5;

                                                Point pipe3 = pipe90.PolyLineSegmentDownImage.Points[0];
                                                Point pipe4 = pipe90.PolyLineSegmentDownImage.Points[1];
                                                Point pipe5 = pipe90.PolyLineSegmentDownImage.Points[2];
                                                pipe3.Y += delta;
                                                pipe3.X += delta;
                                                pipe4.Y += delta;
                                                pipe4.X += delta;
                                                pipe5.Y += delta;
                                                pipe90.PolyLineSegmentDownImage.Points[0] = pipe3;
                                                pipe90.PolyLineSegmentDownImage.Points[1] = pipe4;
                                                pipe90.PolyLineSegmentDownImage.Points[2] = pipe5;

                                                Point downLenghtPoint = pipe90.PathFigureDownLenghtSize.StartPoint;
                                                downLenghtPoint.Y += delta;
                                                pipe90.PathFigureDownLenghtSize.StartPoint = downLenghtPoint;
                                                Point downLenghtPoint2 = pipe90.LineSegmentDownLenghtSize.Point;
                                                downLenghtPoint2.Y += delta;
                                                downLenghtPoint2.X += delta;
                                                pipe90.LineSegmentDownLenghtSize.Point = downLenghtPoint2;

                                                pipe90.Diameter = d;
                                                #endregion

                                                #region SavePipe90Ser
                                                pipe90.Pipe90Ser.TopLenghtSize.point[0] = pipe90.PathFigureTopLenghtSize.StartPoint;
                                                pipe90.Pipe90Ser.TopLenghtSize.point[1] = pipe90.LineSegmentTopLenghtSize.Point;

                                                pipe90.Pipe90Ser.DownLenghtSize.point[0] = pipe90.PathFigureDownLenghtSize.StartPoint;
                                                pipe90.Pipe90Ser.DownLenghtSize.point[1] = pipe90.LineSegmentDownLenghtSize.Point;

                                                pipe90.Pipe90Ser.TopSize.point[0] = pipe90.PathFigureTopSize.StartPoint;
                                                pipe90.Pipe90Ser.TopSize.point[1] = pipe90.LineSegmentTopSize.Point;

                                                pipe90.Pipe90Ser.DownSize.point[0] = pipe90.PathFigureDownSize.StartPoint;
                                                pipe90.Pipe90Ser.DownSize.point[1] = pipe90.LineSegmentDownSize.Point;

                                                pipe90.Pipe90Ser.LeftFlange.point[0] = pipe90.PathFigureLeftFlange.StartPoint;
                                                pipe90.Pipe90Ser.LeftFlange.point[1] = pipe90.PolyLineSegmentLeftFlange.Points[0];
                                                pipe90.Pipe90Ser.LeftFlange.point[2] = pipe90.PolyLineSegmentLeftFlange.Points[1];
                                                pipe90.Pipe90Ser.LeftFlange.point[3] = pipe90.PolyLineSegmentLeftFlange.Points[2];
                                                pipe90.Pipe90Ser.LeftFlange.point[4] = pipe90.PolyLineSegmentLeftFlange.Points[3];

                                                pipe90.Pipe90Ser.RightFlange.point[0] = pipe90.PathFigureRightFlange.StartPoint;
                                                pipe90.Pipe90Ser.RightFlange.point[1] = pipe90.PolyLineSegmentRightFlange.Points[0];
                                                pipe90.Pipe90Ser.RightFlange.point[2] = pipe90.PolyLineSegmentRightFlange.Points[1];
                                                pipe90.Pipe90Ser.RightFlange.point[3] = pipe90.PolyLineSegmentRightFlange.Points[2];
                                                pipe90.Pipe90Ser.RightFlange.point[4] = pipe90.PolyLineSegmentRightFlange.Points[3];

                                                pipe90.Pipe90Ser.TopImage.point[0] = pipe90.PathFigureTopImage.StartPoint;
                                                pipe90.Pipe90Ser.TopImage.point[1] = pipe90.PolyLineSegmentTopImage.Points[0];
                                                pipe90.Pipe90Ser.TopImage.point[2] = pipe90.PolyLineSegmentTopImage.Points[1];
                                                pipe90.Pipe90Ser.TopImage.point[3] = pipe90.PolyLineSegmentTopImage.Points[2];
                                                pipe90.Pipe90Ser.TopImage.point[4] = pipe90.PolyLineSegmentTopImage.Points[3];

                                                pipe90.Pipe90Ser.DownImage.point[0] = pipe90.PathFigureDownImage.StartPoint;
                                                pipe90.Pipe90Ser.DownImage.point[1] = pipe90.PolyLineSegmentDownImage.Points[0];
                                                pipe90.Pipe90Ser.DownImage.point[2] = pipe90.PolyLineSegmentDownImage.Points[1];
                                                pipe90.Pipe90Ser.DownImage.point[3] = pipe90.PolyLineSegmentDownImage.Points[2];
                                                pipe90.Pipe90Ser.DownImage.point[4] = pipe90.PolyLineSegmentDownImage.Points[3];

                                                pipe90.Pipe90Ser.LeftDownSize.point[0] = pipe90.PathFigureLeftDownSize.StartPoint;
                                                pipe90.Pipe90Ser.LeftDownSize.point[1] = pipe90.LineSegmentLeftDownSize.Point;

                                                pipe90.Pipe90Ser.RightDownSize.point[0] = pipe90.PathFigureRightDownSize.StartPoint;
                                                pipe90.Pipe90Ser.RightDownSize.point[1] = pipe90.LineSegmentRightDownSize.Point;

                                                pipe90.Pipe90Ser.BorderPipe90.point[0] = pipe90.PathFigureBorder.StartPoint;
                                                pipe90.Pipe90Ser.BorderPipe90.point[1] = pipe90.PolyLineSegmentBorder.Points[0];
                                                pipe90.Pipe90Ser.BorderPipe90.point[2] = pipe90.PolyLineSegmentBorder.Points[1];
                                                pipe90.Pipe90Ser.BorderPipe90.point[3] = pipe90.PolyLineSegmentBorder.Points[2];
                                                pipe90.Pipe90Ser.BorderPipe90.point[4] = pipe90.PolyLineSegmentBorder.Points[3];
                                                #endregion

                                                if (selectedCanvas.SelectedControlOnCanvas.Count == 1)
                                                {
                                                    if (pipe90.controlOnCanvasSer.Transform == 0)
                                                    {
                                                        pipe90.CoordinateX.Text = string.Format("{0:F2}", (double)pipe90.GetValue(Canvas.LeftProperty));
                                                        pipe90.CoordinateY.Text = string.Format("{0:F2}", (double)pipe90.GetValue(Canvas.TopProperty));
                                                    }
                                                    else if (pipe90.controlOnCanvasSer.Transform == -90 || pipe90.controlOnCanvasSer.Transform == 270)
                                                    {
                                                        pipe90.CoordinateY.Text = string.Format("{0:F2}", (double)pipe90.GetValue(Canvas.TopProperty) - pipe90.ActualWidth);
                                                        pipe90.CoordinateX.Text = string.Format("{0:F2}", (double)pipe90.GetValue(Canvas.LeftProperty));
                                                    }
                                                    else if (pipe90.controlOnCanvasSer.Transform == -180 || pipe90.controlOnCanvasSer.Transform == 180)
                                                    {
                                                        pipe90.CoordinateY.Text = string.Format("{0:F2}", (double)pipe90.GetValue(Canvas.TopProperty) - pipe90.ActualHeight);
                                                        pipe90.CoordinateX.Text = string.Format("{0:F2}", (double)pipe90.GetValue(Canvas.LeftProperty) - pipe90.ActualWidth);
                                                    }
                                                    else if (pipe90.controlOnCanvasSer.Transform == -270 || pipe90.controlOnCanvasSer.Transform == 90)
                                                    {
                                                        pipe90.CoordinateY.Text = string.Format("{0:F2}", (double)pipe90.GetValue(Canvas.TopProperty));
                                                        pipe90.CoordinateX.Text = string.Format("{0:F2}", (double)pipe90.GetValue(Canvas.LeftProperty) - pipe90.ActualHeight);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                ((AppWPF)Application.Current).SaveTabItem(selectedCanvas.TabItemParent);

                                Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                {
                                    selectedCanvas.RepositionAllObjects(selectedCanvas);
                                    selectedCanvas.InvalidateMeasure();
                                }));
                                
                                tb.Tag = string.Format("{0:F2}", d);
                                tb.Text = string.Format("{0:F2}", d);
                                tb.SelectAll();
                            }

                            e.Handled = true;
                            return;
                        }
                    }
                    else
                    {
                        if (tb.Text == "-")
                        {
                            e.Handled = true;
                            return;
                        }

                        MessageBox.Show("Не верный формат диаметра", "Ошибка формата", MessageBoxButton.OK, MessageBoxImage.Warning);
                        e.Handled = true;
                        return;
                    }
                }          
            }
            else if (e.Key == Key.Space)
                e.Handled = true;           
        }

        private void CheckDigits(object sender, TextCompositionEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            
            if (tb == CoordinateObjectX)
            {
                string pattern = @"^\d{1,8}(?:\.\d{0,2})?$";
                string s;

                if (CoordinateObjectX.SelectionLength > 0)
                {
                    s = CoordinateObjectX.Text.Remove(CoordinateObjectX.SelectionStart, CoordinateObjectX.SelectionLength);
                    s = s.Insert(CoordinateObjectX.SelectionStart, e.Text);
                }
                else
                {
                    s = CoordinateObjectX.Text.Insert(CoordinateObjectX.Text.Length, e.Text);
                }

                if (!Regex.IsMatch(s, pattern))
                {
                    e.Handled = true;
                }
            }
            else if (tb == CoordinateObjectY)
            {
                string pattern = @"^\d{1,8}(?:\.\d{0,2})?$";
                string s;

                if (CoordinateObjectY.SelectionLength > 0)
                {
                    s = CoordinateObjectY.Text.Remove(CoordinateObjectY.SelectionStart, CoordinateObjectY.SelectionLength);
                    s = s.Insert(CoordinateObjectY.SelectionStart, e.Text);
                }
                else
                {
                    s = CoordinateObjectY.Text.Insert(CoordinateObjectY.Text.Length, e.Text);
                }

                if (!Regex.IsMatch(s, pattern))
                {
                    e.Handled = true;
                }
            }
            else if (tb == TextBoxDiameter)
            {
                string pattern = @"^\d{1,6}(?:\.\d{0,2})?$";
                string s;

                if (TextBoxDiameter.SelectionLength > 0)
                {
                    s = TextBoxDiameter.Text.Remove(TextBoxDiameter.SelectionStart, TextBoxDiameter.SelectionLength);
                    s = s.Insert(TextBoxDiameter.SelectionStart, e.Text);
                }
                else
                {
                    s = TextBoxDiameter.Text.Insert(TextBoxDiameter.Text.Length, e.Text);
                }

                if (!Regex.IsMatch(s, pattern))
                {
                    e.Handled = true;
                }
            }                       
        }

        private void SelectAddress(object sender, RoutedEventArgs e)
        {
            TextBox tb = (sender as TextBox);

            if (tb != null)
            {
                tb.SelectAll();
            }
        }

        private void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = (sender as TextBox);

            if (tb != null)
            {
                if (!tb.IsKeyboardFocusWithin)
                {
                    e.Handled = true;

                    tb.Focus();
                }
            }
        }

        private void lostFocusTextBox(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            CanvasTab selectedCanvas = null;
            ControlOnCanvas selectedControlOncanvas = null;
            ControlOnCanvasSer selectedControlOnCanvasSer = null;
            ScrollViewer selectedScrollViewer = null;

            if (TabControlMain.Items.Count != 0)
            {
                selectedScrollViewer = TabControlMain.SelectedContent as ScrollViewer;

                if (selectedScrollViewer.Content is CanvasPage)
                {
                    selectedCanvas = selectedScrollViewer.Content as CanvasPage;

                    if (selectedCanvas.SelectedControlOnCanvas.Count != 0)
                    {
                        selectedControlOncanvas = selectedCanvas.SelectedControlOnCanvas[0];

                        selectedControlOnCanvasSer = selectedControlOncanvas.controlOnCanvasSer;
                    }
                }
            }

            if (tb == CoordinateObjectX && !tb.IsReadOnly)
            {
                double d;
                if (double.TryParse(CoordinateObjectX.Text, out d))
                {
                    if (d > 50000)
                    {
                        MessageBox.Show("Координата Y не может быть больше 50000", "Ошибка диапазона", MessageBoxButton.OK, MessageBoxImage.Warning);
                        e.Handled = true;

                        if (IsTabFocus)
                        {
                            TextBoxDiameter.Text = (string)TextBoxDiameter.Tag;
                            IsTabFocus = false;
                        }
                        else
                        {
                            e.Handled = true;
                            Keyboard.Focus(tb);
                            FocusManager.SetFocusedElement(this, tb);
                        }

                        return;    
                    }
                    else
                    {
                        d = Math.Floor(d * 100) / 100;

                        // Если новоя координата равна старой ничего не происходит
                        if (string.Format("{0:F2}", d) != (string)CoordinateObjectX.Tag)
                        {
                            Canvas.SetLeft(selectedControlOncanvas, d);

                            selectedCanvas.RepositionAllObjects(selectedCanvas);
                            selectedCanvas.InvalidateMeasure();

                            selectedControlOnCanvasSer.Сoordinates = new Point(Canvas.GetLeft(selectedControlOncanvas), Canvas.GetTop(selectedControlOncanvas));              

                            ((AppWPF)Application.Current).SaveTabItem(selectedCanvas.TabItemParent);

                            if (selectedControlOnCanvasSer.Transform == 0)
                            {
                                CoordinateObjectX.Text = string.Format("{0:F2}", d);
                            }
                            else if (selectedControlOnCanvasSer.Transform == -90 || selectedControlOnCanvasSer.Transform == 270)
                            {
                                CoordinateObjectX.Text = string.Format("{0:F2}", d);
                            }
                            else if (selectedControlOnCanvasSer.Transform == -180 || selectedControlOnCanvasSer.Transform == 180)
                            {
                                CoordinateObjectX.Text = string.Format("{0:F2}", d - selectedControlOncanvas.ActualWidth);
                            }
                            else if (selectedControlOnCanvasSer.Transform == -270 || selectedControlOnCanvasSer.Transform == 90)
                            {
                                CoordinateObjectX.Text = string.Format("{0:F2}", d - selectedControlOncanvas.ActualHeight);
                            }

                            tb.Tag = string.Format("{0:F2}", d);
                            tb.Text = string.Format("{0:F2}", d);

                            ((AppWPF)Application.Current).SaveTabItem(selectedCanvas.TabItemParent);
                        }
                       
                        Keyboard.Focus(null);

                        e.Handled = true;
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Не верный формат координаты", "Ошибка формата", MessageBoxButton.OK, MessageBoxImage.Warning);
                    e.Handled = true;

                    if (IsTabFocus)
                    {
                        TextBoxDiameter.Text = (string)TextBoxDiameter.Tag;
                        IsTabFocus = false;
                    }
                    else
                    {
                        e.Handled = true;
                        Keyboard.Focus(tb);
                        FocusManager.SetFocusedElement(this, tb);
                    }

                    return;   
                }
            }
            else if (tb == CoordinateObjectY && !tb.IsReadOnly)
            {
                double d;
                if (double.TryParse(CoordinateObjectY.Text, out d))
                {
                    if (d > 50000)
                    {
                        MessageBox.Show("Координата Y не может быть больше 50000", "Ошибка диапазона", MessageBoxButton.OK, MessageBoxImage.Warning);
                        e.Handled = true;

                        if (IsTabFocus)
                        {
                            TextBoxDiameter.Text = (string)TextBoxDiameter.Tag;
                            IsTabFocus = false;
                        }
                        else
                        {
                            e.Handled = true;
                            Keyboard.Focus(tb);
                            FocusManager.SetFocusedElement(this, tb);
                        }

                        return;    
                    }
                    else
                    {
                        d = Math.Floor(d * 100) / 100;

                        if (string.Format("{0:F2}", d) != (string)CoordinateObjectY.Tag)
                        {
                            Canvas.SetTop(selectedControlOncanvas, d);

                            selectedCanvas.RepositionAllObjects(selectedCanvas);
                            selectedCanvas.InvalidateMeasure();

                            selectedControlOnCanvasSer.Сoordinates = new Point(Canvas.GetLeft(selectedControlOncanvas), Canvas.GetTop(selectedControlOncanvas));

                            ((AppWPF)Application.Current).SaveTabItem(selectedCanvas.TabItemParent);

                            if (selectedControlOnCanvasSer.Transform == 0)
                            {
                                CoordinateObjectY.Text = string.Format("{0:F2}", d);
                            }
                            else if (selectedControlOnCanvasSer.Transform == -90 || selectedControlOnCanvasSer.Transform == 270)
                            {
                                CoordinateObjectY.Text = string.Format("{0:F2}", d);
                            }
                            else if (selectedControlOnCanvasSer.Transform == -180 || selectedControlOnCanvasSer.Transform == 180)
                            {
                                CoordinateObjectY.Text = string.Format("{0:F2}", d - selectedControlOncanvas.ActualWidth);
                            }
                            else if (selectedControlOnCanvasSer.Transform == -270 || selectedControlOnCanvasSer.Transform == 90)
                            {
                                CoordinateObjectY.Text = string.Format("{0:F2}", d - selectedControlOncanvas.ActualHeight);
                            }
                          
                            tb.Tag = string.Format("{0:F2}", d);
                            tb.Text = string.Format("{0:F2}", d);

                            ((AppWPF)Application.Current).SaveTabItem(selectedCanvas.TabItemParent);
                        }

                        Keyboard.Focus(null);
                        e.Handled = true;
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Не верный формат координаты", "Ошибка формата", MessageBoxButton.OK, MessageBoxImage.Warning);
                    e.Handled = true;

                    if (IsTabFocus)
                    {
                        TextBoxDiameter.Text = (string)TextBoxDiameter.Tag;
                        IsTabFocus = false;
                    }
                    else
                    {
                        e.Handled = true;
                        Keyboard.Focus(tb);
                        FocusManager.SetFocusedElement(this, tb);
                    }

                    return;    
                }
            }
            else if (tb == TextBoxDiameter && !tb.IsReadOnly)
            {
                double d;
                if (double.TryParse(TextBoxDiameter.Text, out d))
                {
                    if (d > 500)
                    {
                        MessageBox.Show("Диаметр не может быть больше 500", "Ошибка диапазона", MessageBoxButton.OK, MessageBoxImage.Warning);
                        e.Handled = true;

                        if (IsTabFocus)
                        {                           
                            TextBoxDiameter.Text = (string)TextBoxDiameter.Tag;
                            IsTabFocus = false;                          
                        }
                        else
                        {
                            e.Handled = true;
                            Keyboard.Focus(tb);
                            FocusManager.SetFocusedElement(this, tb);
                        }
                        
                        return;             
                    }
                    else if (d < 10)
                    {
                        MessageBox.Show("Диаметр не может быть меньше 10", "Ошибка диапазона", MessageBoxButton.OK, MessageBoxImage.Warning);
                        e.Handled = true;

                        if (IsTabFocus)
                        {
                            TextBoxDiameter.Text = (string)TextBoxDiameter.Tag;
                            IsTabFocus = false;
                        }
                        else
                        {
                            e.Handled = true;
                            Keyboard.Focus(tb);
                            FocusManager.SetFocusedElement(this, tb);
                        }

                        return;    
                    }
                    else
                    {
                        d = Math.Floor(d * 100) / 100;

                        if (string.Format("{0:F2}", d) != (string)TextBoxDiameter.Tag)
                        {
                            foreach (ControlOnCanvas controlOnCanvasPage in selectedCanvas.SelectedControlOnCanvas)
                            {
                                if (controlOnCanvasPage is PipeOnCanvas)
                                {
                                    PipeOnCanvas pipeOnCanvas = controlOnCanvasPage as PipeOnCanvas;

                                    if (Math.Floor(pipeOnCanvas.Diameter * 100) / 100 != d)
                                    {
                                        if (controlOnCanvasPage is Pipe)
                                        {
                                            #region Pipe
                                            Pipe pipe = controlOnCanvasPage as Pipe;

                                            double delta = d - pipe.Diameter;

                                            Point topSizePoint = pipe.PathFigureTopSize.StartPoint;

                                            Point downSizePoint = pipe.PathFigureDownSize.StartPoint;
                                            downSizePoint.Y += delta;
                                            pipe.PathFigureDownSize.StartPoint = downSizePoint;
                                            Point downSizePoint2 = pipe.LineSegmentDownSize.Point;
                                            downSizePoint2.Y += delta;
                                            pipe.LineSegmentDownSize.Point = downSizePoint2;

                                            Point leftSizePoint = pipe.LineSegmentLeftSize.Point;
                                            leftSizePoint.Y += delta;
                                            pipe.LineSegmentLeftSize.Point = leftSizePoint;

                                            Point rightSizePoint = pipe.PathFigureRightSize.StartPoint;
                                            rightSizePoint.Y += delta;
                                            pipe.PathFigureRightSize.StartPoint = rightSizePoint;

                                            Point rightFlangePoint2 = pipe.PolyLineSegmentRightFlange.Points[1];
                                            Point rightFlangePoint3 = pipe.PolyLineSegmentRightFlange.Points[2];
                                            rightFlangePoint2.Y += delta;
                                            rightFlangePoint3.Y += delta;
                                            pipe.PolyLineSegmentRightFlange.Points[1] = rightFlangePoint2;
                                            pipe.PolyLineSegmentRightFlange.Points[2] = rightFlangePoint3;

                                            Point leftFlangePoint2 = pipe.PolyLineSegmentLeftFlange.Points[1];
                                            Point leftFlangePoint3 = pipe.PolyLineSegmentLeftFlange.Points[2];
                                            leftFlangePoint2.Y += delta;
                                            leftFlangePoint3.Y += delta;
                                            pipe.PolyLineSegmentLeftFlange.Points[1] = leftFlangePoint2;
                                            pipe.PolyLineSegmentLeftFlange.Points[2] = leftFlangePoint3;

                                            Point borderPipePoint2 = pipe.PolyLineSegmentBorder.Points[1];
                                            Point borderPipePoint3 = pipe.PolyLineSegmentBorder.Points[2];
                                            borderPipePoint2.Y += delta;
                                            pipe.PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                                            borderPipePoint3.Y += delta;
                                            pipe.PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                                            Point pointPipe = pipe.PathFigurePipe.StartPoint;
                                            pointPipe.Y += delta;
                                            pipe.PathFigurePipe.StartPoint = pointPipe;
                                            Point pointPipe2 = pipe.PolyLineSegmentPipe.Points[2];
                                            Point pointPipe3 = pipe.PolyLineSegmentPipe.Points[3];
                                            pointPipe2.Y += delta;
                                            pointPipe3.Y += delta;
                                            pipe.PolyLineSegmentPipe.Points[2] = pointPipe2;
                                            pipe.PolyLineSegmentPipe.Points[3] = pointPipe3;

                                            pipe.Diameter = d;
                                            #endregion

                                            #region SavePipeSer
                                            pipe.PipeSer.LeftSize.point[0] = pipe.PathFigureLeftSize.StartPoint;
                                            pipe.PipeSer.LeftSize.point[1] = pipe.LineSegmentLeftSize.Point;

                                            pipe.PipeSer.RightSize.point[0] = pipe.PathFigureRightSize.StartPoint;
                                            pipe.PipeSer.RightSize.point[1] = pipe.LineSegmentRightSize.Point;

                                            pipe.PipeSer.TopSize.point[0] = pipe.PathFigureTopSize.StartPoint;
                                            pipe.PipeSer.TopSize.point[1] = pipe.LineSegmentTopSize.Point;

                                            pipe.PipeSer.DownSize.point[0] = pipe.PathFigureDownSize.StartPoint;
                                            pipe.PipeSer.DownSize.point[1] = pipe.LineSegmentDownSize.Point;

                                            pipe.PipeSer.LeftFlange.point[0] = pipe.PathFigureLeftFlange.StartPoint;
                                            pipe.PipeSer.LeftFlange.point[1] = pipe.PolyLineSegmentLeftFlange.Points[0];
                                            pipe.PipeSer.LeftFlange.point[2] = pipe.PolyLineSegmentLeftFlange.Points[1];
                                            pipe.PipeSer.LeftFlange.point[3] = pipe.PolyLineSegmentLeftFlange.Points[2];
                                            pipe.PipeSer.LeftFlange.point[4] = pipe.PolyLineSegmentLeftFlange.Points[3];

                                            pipe.PipeSer.RightFlange.point[0] = pipe.PathFigureRightFlange.StartPoint;
                                            pipe.PipeSer.RightFlange.point[1] = pipe.PolyLineSegmentRightFlange.Points[0];
                                            pipe.PipeSer.RightFlange.point[2] = pipe.PolyLineSegmentRightFlange.Points[1];
                                            pipe.PipeSer.RightFlange.point[3] = pipe.PolyLineSegmentRightFlange.Points[2];
                                            pipe.PipeSer.RightFlange.point[4] = pipe.PolyLineSegmentRightFlange.Points[3];

                                            pipe.PipeSer.Pipe.point[0] = pipe.PathFigurePipe.StartPoint;
                                            pipe.PipeSer.Pipe.point[1] = pipe.PolyLineSegmentPipe.Points[0];
                                            pipe.PipeSer.Pipe.point[2] = pipe.PolyLineSegmentPipe.Points[1];
                                            pipe.PipeSer.Pipe.point[3] = pipe.PolyLineSegmentPipe.Points[2];
                                            pipe.PipeSer.Pipe.point[4] = pipe.PolyLineSegmentPipe.Points[3];

                                            pipe.PipeSer.BorderPipe.point[0] = pipe.PathFigureBorder.StartPoint;
                                            pipe.PipeSer.BorderPipe.point[1] = pipe.PolyLineSegmentBorder.Points[0];
                                            pipe.PipeSer.BorderPipe.point[2] = pipe.PolyLineSegmentBorder.Points[1];
                                            pipe.PipeSer.BorderPipe.point[3] = pipe.PolyLineSegmentBorder.Points[2];
                                            pipe.PipeSer.BorderPipe.point[4] = pipe.PolyLineSegmentBorder.Points[3];
                                            #endregion

                                            if (selectedCanvas.SelectedControlOnCanvas.Count == 1)
                                            {
                                                if (selectedControlOnCanvasSer.Transform == 0)
                                                {
                                                    CoordinateObjectX.Text = string.Format("{0:F2}", (double)selectedControlOncanvas.GetValue(Canvas.LeftProperty));
                                                    CoordinateObjectY.Text = string.Format("{0:F2}", (double)selectedControlOncanvas.GetValue(Canvas.TopProperty));
                                                }
                                                else if (selectedControlOnCanvasSer.Transform == -90 || selectedControlOnCanvasSer.Transform == 270)
                                                {
                                                    CoordinateObjectY.Text = string.Format("{0:F2}", (double)selectedControlOncanvas.GetValue(Canvas.TopProperty) - selectedControlOncanvas.ActualWidth);
                                                    CoordinateObjectX.Text = string.Format("{0:F2}", (double)selectedControlOncanvas.GetValue(Canvas.LeftProperty));
                                                }
                                                else if (selectedControlOnCanvasSer.Transform == -180 || selectedControlOnCanvasSer.Transform == 180)
                                                {
                                                    CoordinateObjectY.Text = string.Format("{0:F2}", (double)selectedControlOncanvas.GetValue(Canvas.TopProperty) - selectedControlOncanvas.ActualHeight);
                                                    CoordinateObjectX.Text = string.Format("{0:F2}", (double)selectedControlOncanvas.GetValue(Canvas.LeftProperty) - selectedControlOncanvas.ActualWidth);
                                                }
                                                else if (selectedControlOnCanvasSer.Transform == -270 || selectedControlOncanvas.controlOnCanvasSer.Transform == 90)
                                                {
                                                    CoordinateObjectY.Text = string.Format("{0:F2}", (double)selectedControlOncanvas.GetValue(Canvas.TopProperty));
                                                    CoordinateObjectX.Text = string.Format("{0:F2}", (double)selectedControlOncanvas.GetValue(Canvas.LeftProperty) - selectedControlOncanvas.ActualHeight);
                                                }
                                            }
                                        }
                                        else if (controlOnCanvasPage is Pipe90)
                                        {
                                            #region Pipe90
                                            Pipe90 pipe90 = controlOnCanvasPage as Pipe90;

                                            double delta = d - pipe90.Diameter;

                                            Point topSizePoint = pipe90.PathFigureTopSize.StartPoint;
                                            topSizePoint.X += delta;
                                            pipe90.PathFigureTopSize.StartPoint = topSizePoint;

                                            Point downSizePoint = pipe90.PathFigureDownSize.StartPoint;
                                            downSizePoint.Y += delta;
                                            downSizePoint.X += delta;
                                            pipe90.PathFigureDownSize.StartPoint = downSizePoint;
                                            Point downSizePoint2 = pipe90.LineSegmentDownSize.Point;
                                            downSizePoint2.Y += delta;
                                            downSizePoint2.X += delta;
                                            pipe90.LineSegmentDownSize.Point = downSizePoint2;

                                            Point leftSizePoint2 = pipe90.PathFigureTopLenghtSize.StartPoint;
                                            leftSizePoint2.X += delta;
                                            pipe90.PathFigureTopLenghtSize.StartPoint = leftSizePoint2;
                                            Point leftSizePoint = pipe90.LineSegmentTopLenghtSize.Point;
                                            leftSizePoint.Y += delta;
                                            leftSizePoint.X += delta;
                                            pipe90.LineSegmentTopLenghtSize.Point = leftSizePoint;

                                            Point rightFlangePoint = pipe90.PathFigureRightFlange.StartPoint;
                                            rightFlangePoint.X += delta;
                                            rightFlangePoint.Y += delta;
                                            pipe90.PathFigureRightFlange.StartPoint = rightFlangePoint;
                                            Point rightFlangePoint2 = pipe90.PolyLineSegmentRightFlange.Points[0];
                                            Point rightFlangePoint3 = pipe90.PolyLineSegmentRightFlange.Points[3];
                                            Point rightFlangePoint4 = pipe90.PolyLineSegmentRightFlange.Points[1];
                                            Point rightFlangePoint5 = pipe90.PolyLineSegmentRightFlange.Points[2];
                                            rightFlangePoint2.Y += delta;
                                            rightFlangePoint2.X += delta;
                                            rightFlangePoint3.Y += delta;
                                            rightFlangePoint3.X += delta;
                                            rightFlangePoint4.X += delta;
                                            rightFlangePoint5.X += delta;
                                            pipe90.PolyLineSegmentRightFlange.Points[0] = rightFlangePoint2;
                                            pipe90.PolyLineSegmentRightFlange.Points[3] = rightFlangePoint3;
                                            pipe90.PolyLineSegmentRightFlange.Points[1] = rightFlangePoint4;
                                            pipe90.PolyLineSegmentRightFlange.Points[2] = rightFlangePoint5;

                                            Point borderPipePoint1 = pipe90.PolyLineSegmentBorder.Points[0];
                                            Point borderPipePoint2 = pipe90.PolyLineSegmentBorder.Points[1];
                                            Point borderPipePoint3 = pipe90.PolyLineSegmentBorder.Points[2];
                                            borderPipePoint1.X += delta;
                                            pipe90.PolyLineSegmentBorder.Points[0] = borderPipePoint1;
                                            borderPipePoint2.Y += delta;
                                            borderPipePoint2.X += delta;
                                            pipe90.PolyLineSegmentBorder.Points[1] = borderPipePoint2;
                                            borderPipePoint3.Y += delta;
                                            pipe90.PolyLineSegmentBorder.Points[2] = borderPipePoint3;

                                            Point pointPipe = pipe90.PathFigureTopImage.StartPoint;
                                            pointPipe.Y += delta;
                                            pointPipe.X += delta;
                                            pipe90.PathFigureTopImage.StartPoint = pointPipe;
                                            Point pointPipe4 = pipe90.PolyLineSegmentTopImage.Points[1];
                                            Point pointPipe2 = pipe90.PolyLineSegmentTopImage.Points[2];
                                            Point pointPipe3 = pipe90.PolyLineSegmentTopImage.Points[3];
                                            pointPipe2.Y += delta;
                                            pointPipe3.Y += delta;
                                            pointPipe2.X += delta;
                                            pointPipe3.X += delta;
                                            pointPipe4.X += delta;
                                            pipe90.PolyLineSegmentTopImage.Points[1] = pointPipe4;
                                            pipe90.PolyLineSegmentTopImage.Points[2] = pointPipe2;
                                            pipe90.PolyLineSegmentTopImage.Points[3] = pointPipe3;

                                            Point topSizePoint3 = pipe90.PathFigureLeftDownSize.StartPoint;
                                            topSizePoint3.Y += delta;
                                            pipe90.PathFigureLeftDownSize.StartPoint = topSizePoint3;

                                            Point downSizePoint3 = pipe90.PathFigureRightDownSize.StartPoint;
                                            downSizePoint3.Y += delta;
                                            downSizePoint3.X += delta;
                                            pipe90.PathFigureRightDownSize.StartPoint = downSizePoint3;
                                            Point downSizePoint4 = pipe90.LineSegmentRightDownSize.Point;
                                            downSizePoint4.X += delta;
                                            downSizePoint4.Y += delta;
                                            pipe90.LineSegmentRightDownSize.Point = downSizePoint4;

                                            Point leftFlangePoint = pipe90.PathFigureLeftFlange.StartPoint;
                                            leftFlangePoint.Y += delta;
                                            pipe90.PathFigureLeftFlange.StartPoint = leftFlangePoint;
                                            Point leftFlangePoint2 = pipe90.PolyLineSegmentLeftFlange.Points[0];
                                            Point leftFlangePoint3 = pipe90.PolyLineSegmentLeftFlange.Points[1];
                                            Point leftFlangePoint4 = pipe90.PolyLineSegmentLeftFlange.Points[2];
                                            Point leftFlangePoint5 = pipe90.PolyLineSegmentLeftFlange.Points[3];
                                            leftFlangePoint2.Y += delta;
                                            leftFlangePoint3.Y += delta;
                                            leftFlangePoint3.X += delta;
                                            leftFlangePoint4.Y += delta;
                                            leftFlangePoint4.X += delta;
                                            leftFlangePoint5.Y += delta;
                                            pipe90.PolyLineSegmentLeftFlange.Points[0] = leftFlangePoint2;
                                            pipe90.PolyLineSegmentLeftFlange.Points[1] = leftFlangePoint3;
                                            pipe90.PolyLineSegmentLeftFlange.Points[2] = leftFlangePoint4;
                                            pipe90.PolyLineSegmentLeftFlange.Points[3] = leftFlangePoint5;

                                            Point pipe3 = pipe90.PolyLineSegmentDownImage.Points[0];
                                            Point pipe4 = pipe90.PolyLineSegmentDownImage.Points[1];
                                            Point pipe5 = pipe90.PolyLineSegmentDownImage.Points[2];
                                            pipe3.Y += delta;
                                            pipe3.X += delta;
                                            pipe4.Y += delta;
                                            pipe4.X += delta;
                                            pipe5.Y += delta;
                                            pipe90.PolyLineSegmentDownImage.Points[0] = pipe3;
                                            pipe90.PolyLineSegmentDownImage.Points[1] = pipe4;
                                            pipe90.PolyLineSegmentDownImage.Points[2] = pipe5;

                                            Point downLenghtPoint = pipe90.PathFigureDownLenghtSize.StartPoint;
                                            downLenghtPoint.Y += delta;
                                            pipe90.PathFigureDownLenghtSize.StartPoint = downLenghtPoint;
                                            Point downLenghtPoint2 = pipe90.LineSegmentDownLenghtSize.Point;
                                            downLenghtPoint2.Y += delta;
                                            downLenghtPoint2.X += delta;
                                            pipe90.LineSegmentDownLenghtSize.Point = downLenghtPoint2;

                                            pipe90.Diameter = d;
                                            #endregion

                                            #region SavePipe90Ser
                                            pipe90.Pipe90Ser.TopLenghtSize.point[0] = pipe90.PathFigureTopLenghtSize.StartPoint;
                                            pipe90.Pipe90Ser.TopLenghtSize.point[1] = pipe90.LineSegmentTopLenghtSize.Point;

                                            pipe90.Pipe90Ser.DownLenghtSize.point[0] = pipe90.PathFigureDownLenghtSize.StartPoint;
                                            pipe90.Pipe90Ser.DownLenghtSize.point[1] = pipe90.LineSegmentDownLenghtSize.Point;

                                            pipe90.Pipe90Ser.TopSize.point[0] = pipe90.PathFigureTopSize.StartPoint;
                                            pipe90.Pipe90Ser.TopSize.point[1] = pipe90.LineSegmentTopSize.Point;

                                            pipe90.Pipe90Ser.DownSize.point[0] = pipe90.PathFigureDownSize.StartPoint;
                                            pipe90.Pipe90Ser.DownSize.point[1] = pipe90.LineSegmentDownSize.Point;

                                            pipe90.Pipe90Ser.LeftFlange.point[0] = pipe90.PathFigureLeftFlange.StartPoint;
                                            pipe90.Pipe90Ser.LeftFlange.point[1] = pipe90.PolyLineSegmentLeftFlange.Points[0];
                                            pipe90.Pipe90Ser.LeftFlange.point[2] = pipe90.PolyLineSegmentLeftFlange.Points[1];
                                            pipe90.Pipe90Ser.LeftFlange.point[3] = pipe90.PolyLineSegmentLeftFlange.Points[2];
                                            pipe90.Pipe90Ser.LeftFlange.point[4] = pipe90.PolyLineSegmentLeftFlange.Points[3];

                                            pipe90.Pipe90Ser.RightFlange.point[0] = pipe90.PathFigureRightFlange.StartPoint;
                                            pipe90.Pipe90Ser.RightFlange.point[1] = pipe90.PolyLineSegmentRightFlange.Points[0];
                                            pipe90.Pipe90Ser.RightFlange.point[2] = pipe90.PolyLineSegmentRightFlange.Points[1];
                                            pipe90.Pipe90Ser.RightFlange.point[3] = pipe90.PolyLineSegmentRightFlange.Points[2];
                                            pipe90.Pipe90Ser.RightFlange.point[4] = pipe90.PolyLineSegmentRightFlange.Points[3];

                                            pipe90.Pipe90Ser.TopImage.point[0] = pipe90.PathFigureTopImage.StartPoint;
                                            pipe90.Pipe90Ser.TopImage.point[1] = pipe90.PolyLineSegmentTopImage.Points[0];
                                            pipe90.Pipe90Ser.TopImage.point[2] = pipe90.PolyLineSegmentTopImage.Points[1];
                                            pipe90.Pipe90Ser.TopImage.point[3] = pipe90.PolyLineSegmentTopImage.Points[2];
                                            pipe90.Pipe90Ser.TopImage.point[4] = pipe90.PolyLineSegmentTopImage.Points[3];

                                            pipe90.Pipe90Ser.DownImage.point[0] = pipe90.PathFigureDownImage.StartPoint;
                                            pipe90.Pipe90Ser.DownImage.point[1] = pipe90.PolyLineSegmentDownImage.Points[0];
                                            pipe90.Pipe90Ser.DownImage.point[2] = pipe90.PolyLineSegmentDownImage.Points[1];
                                            pipe90.Pipe90Ser.DownImage.point[3] = pipe90.PolyLineSegmentDownImage.Points[2];
                                            pipe90.Pipe90Ser.DownImage.point[4] = pipe90.PolyLineSegmentDownImage.Points[3];

                                            pipe90.Pipe90Ser.LeftDownSize.point[0] = pipe90.PathFigureLeftDownSize.StartPoint;
                                            pipe90.Pipe90Ser.LeftDownSize.point[1] = pipe90.LineSegmentLeftDownSize.Point;

                                            pipe90.Pipe90Ser.RightDownSize.point[0] = pipe90.PathFigureRightDownSize.StartPoint;
                                            pipe90.Pipe90Ser.RightDownSize.point[1] = pipe90.LineSegmentRightDownSize.Point;

                                            pipe90.Pipe90Ser.BorderPipe90.point[0] = pipe90.PathFigureBorder.StartPoint;
                                            pipe90.Pipe90Ser.BorderPipe90.point[1] = pipe90.PolyLineSegmentBorder.Points[0];
                                            pipe90.Pipe90Ser.BorderPipe90.point[2] = pipe90.PolyLineSegmentBorder.Points[1];
                                            pipe90.Pipe90Ser.BorderPipe90.point[3] = pipe90.PolyLineSegmentBorder.Points[2];
                                            pipe90.Pipe90Ser.BorderPipe90.point[4] = pipe90.PolyLineSegmentBorder.Points[3];
                                            #endregion

                                            if (selectedCanvas.SelectedControlOnCanvas.Count == 1)
                                            {
                                                if (pipe90.controlOnCanvasSer.Transform == 0)
                                                {
                                                    pipe90.CoordinateX.Text = string.Format("{0:F2}", (double)pipe90.GetValue(Canvas.LeftProperty));
                                                    pipe90.CoordinateY.Text = string.Format("{0:F2}", (double)pipe90.GetValue(Canvas.TopProperty));
                                                }
                                                else if (pipe90.controlOnCanvasSer.Transform == -90 || pipe90.controlOnCanvasSer.Transform == 270)
                                                {
                                                    pipe90.CoordinateY.Text = string.Format("{0:F2}", (double)pipe90.GetValue(Canvas.TopProperty) - pipe90.ActualWidth);
                                                    pipe90.CoordinateX.Text = string.Format("{0:F2}", (double)pipe90.GetValue(Canvas.LeftProperty));
                                                }
                                                else if (pipe90.controlOnCanvasSer.Transform == -180 || pipe90.controlOnCanvasSer.Transform == 180)
                                                {
                                                    pipe90.CoordinateY.Text = string.Format("{0:F2}", (double)pipe90.GetValue(Canvas.TopProperty) - pipe90.ActualHeight);
                                                    pipe90.CoordinateX.Text = string.Format("{0:F2}", (double)pipe90.GetValue(Canvas.LeftProperty) - pipe90.ActualWidth);
                                                }
                                                else if (pipe90.controlOnCanvasSer.Transform == -270 || pipe90.controlOnCanvasSer.Transform == 90)
                                                {
                                                    pipe90.CoordinateY.Text = string.Format("{0:F2}", (double)pipe90.GetValue(Canvas.TopProperty));
                                                    pipe90.CoordinateX.Text = string.Format("{0:F2}", (double)pipe90.GetValue(Canvas.LeftProperty) - pipe90.ActualHeight);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            {
                                selectedCanvas.RepositionAllObjects(selectedCanvas);
                                selectedCanvas.InvalidateMeasure();
                            }));

                            tb.Tag = string.Format("{0:F2}", d);
                            tb.Text = string.Format("{0:F2}", d);

                            ((AppWPF)Application.Current).SaveTabItem(selectedCanvas.TabItemParent);                         
                        }

                        Keyboard.Focus(null);
                        e.Handled = true;
                        return;
                    }
                }
                else
                {
                    if (TextBoxDiameter.Text == "-")
                    {
                        e.Handled = true;
                        return;
                    }

                    MessageBox.Show("Не верный формат диаметра", "Ошибка формата", MessageBoxButton.OK, MessageBoxImage.Warning);
                    e.Handled = true;

                    if (IsTabFocus)
                    {
                        TextBoxDiameter.Text = (string)TextBoxDiameter.Tag;
                        IsTabFocus = false;
                    }
                    else
                    {
                        e.Handled = true;
                        Keyboard.Focus(tb);
                        FocusManager.SetFocusedElement(this, tb);
                    }

                    return;    
                }
            }          
        }

        private void BStartProject_Click(object sender, RoutedEventArgs e)
        {
            if (e != null)
            {
                e.Handled = true;
            }

            bool isTCPObjectNotClose = false;
            bool isUDPObjectNotClose = false;
            bool isModbusObjectNotClose = false;
            bool isConnectDB = false;

            string tcpClientNotClose = null;
            string udpClientNotClose = null;
            string serialPortNotClose = null;
            string sqlConnectionNotClose = null;            

            foreach (EthernetObject TCPObject in CollectionTCPEthernetObject)
            {
                if (TCPObject != null)
                {
                    if (TCPObject.TcpClient.Connected)
                    {
                        tcpClientNotClose = TCPObject.TcpClient.Client.RemoteEndPoint.ToString();

                        isTCPObjectNotClose = true;                                            
                    }
                }                
            }

            foreach (EthernetObject UDPObject in CollectionUDPEthernetObject)
            {
                if (UDPObject != null)
                {
                    if (UDPObject.IsWork)
                    {
                        udpClientNotClose = UDPObject.UdpClient.Client.RemoteEndPoint.ToString();

                        isUDPObjectNotClose = true;
                    }
                }
            }

            foreach (SerialPortObject serialPortObject in CollectionSerialPortThread)
            {
                if (serialPortObject != null)
                {
                    if (serialPortObject.IsWork)
                    {
                        isModbusObjectNotClose = true;

                        serialPortNotClose = serialPortObject.PortName;
                    }
                }
            }

            foreach (SQLObject SqlCon in CollectionSQLObject)
            {
                if (SqlCon.SQL != null)
                {
                    if (SqlCon.IsWork)
                    {
                        isConnectDB = true;

                        sqlConnectionNotClose = SqlCon.SQL.Database;
                    }   
                }                            
            }

            if (isTCPObjectNotClose)
            {
                if (CollectionMessage.Count > 300)
                {
                    CollectionMessage.RemoveAt(0);

                    CollectionMessage.Insert(298, "Сообщение " + " : " + "Клиент " + tcpClientNotClose + " еще не закрыл соединение, повторите запуск через несколько секунд" + " " + DateTime.Now);
                }
                else
                {
                    CollectionMessage.Add("Сообщение " + " : " + "Клиент " + tcpClientNotClose + " еще не закрыл соединение, повторите запуск через несколько секунд" + " " + DateTime.Now);
                }
                           
                return;
            }

            if (isUDPObjectNotClose)
            {
                if (CollectionMessage.Count > 300)
                {
                    CollectionMessage.RemoveAt(0);

                    CollectionMessage.Insert(298, "Сообщение " + " : " + "Поток " + isUDPObjectNotClose + " еще не завершил выполнение, повторите запуск через несколько секунд" + " " + DateTime.Now);
                }
                else
                {
                    CollectionMessage.Add("Сообщение " + " : " + "Поток " + isUDPObjectNotClose + " еще не завершил выполнение, повторите запуск через несколько секунд" + " " + DateTime.Now);
                }

                return;
            }

            if (isModbusObjectNotClose)
            {
                if (CollectionMessage.Count > 300)
                {
                    CollectionMessage.RemoveAt(0);

                    CollectionMessage.Insert(298, "Сообщение " + " : " + "Com-порт " + serialPortNotClose + " еще не закрыл соединение, повторите запуск через несколько секунд" + " " + DateTime.Now);
                }
                else
                {
                    CollectionMessage.Add("Сообщение " + " : " + "Com-порт " + serialPortNotClose + " еще не закрыл соединение, повторите запуск через несколько секунд" + " " + DateTime.Now);
                }
                
                return;
            }

            if (isConnectDB)
            {
                if (CollectionMessage.Count > 300)
                {
                    CollectionMessage.RemoveAt(0);

                    CollectionMessage.Insert(298, "Сообщение " + " : " + "Не все соединения закрылись с базой данных " + sqlConnectionNotClose + " , повторите запуск через несколько секунд" + " " + DateTime.Now);
                }
                else
                {
                    CollectionMessage.Add("Сообщение " + " : " + "Не все соединения закрылись с базой данных " + sqlConnectionNotClose + " , повторите запуск через несколько секунд" + " " + DateTime.Now);
                }
                
                return;
            }

            Interlocked.Exchange(ref IsStop, 0);

            CollectionTCPEthernetObject.Clear();
            CollectionUDPEthernetObject.Clear();
            CollectionSerialPortThread.Clear();
            CollectionSQLObject.Clear();

            AppWPF app = (AppWPF)Application.Current;

            if (CollectionMessage.Count > 300)
            {
                CollectionMessage.RemoveAt(0);

                CollectionMessage.Insert(298, "Сообщение " + " : " + "Старт " + ((MainWindow)app.MainWindow).ProjectBin.ProjectName + " " + DateTime.Now);
            }
            else
            {
                CollectionMessage.Add("Сообщение " + " : " + "Старт " + ((MainWindow)app.MainWindow).ProjectBin.ProjectName + " " + DateTime.Now);
            }
                                            
            IsBindingStartProject = true;

            if (TabControlMain.SelectedContent != null)
            {
                ScrollViewer selectedScrollViewer = TabControlMain.SelectedContent as ScrollViewer;

                if (selectedScrollViewer.Content is CanvasTab)
                {
                    CanvasTab selectedCanvas = selectedScrollViewer.Content as CanvasTab;

                    if (selectedCanvas.SelectedControlOnCanvas.Count != 0)
                    {
                        foreach (ControlOnCanvas objectOnCanvas in selectedCanvas.SelectedControlOnCanvas)
                        {
                            objectOnCanvas.IsSelected = false;
                            objectOnCanvas.border.Pen.Brush.Opacity = 0;
                        }

                        selectedCanvas.CountSelect = 0;
                        selectedCanvas.SelectedControlOnCanvas.Clear();

                        this.LabelSelected.Content = "Выделенно объектов: " + 0;
                        this.TextBoxDiameter.Text = null;
                        this.CoordinateObjectX.Text = null;
                        this.CoordinateObjectY.Text = null;
                        this.ComboBoxEnvironment.SelectedIndex = -1;

                        this.TextBoxDiameter.IsReadOnly = true;
                        this.CoordinateObjectX.IsReadOnly = true;
                        this.CoordinateObjectY.IsReadOnly = true;
                        this.ComboBoxEnvironment.IsEnabled = false;
                    }                                    
                }
            }

            List<EthernetControl> collectionEthernet = new List<EthernetControl>();

            List<ModbusControl> collectionModbus = new List<ModbusControl>();

            foreach (ControlPanel cp in app.CollectionControlPanel.Values)
            {
                foreach (EthernetSer es in cp.CollectionEthernet)
                {
                    foreach(ItemNet item in es.CollectionItemNetSend)
                    {
                        if (item.ItemModbus != null)
                        {
                            item.ItemModbus = null;
                        }
                    }

                    collectionEthernet.Add((EthernetControl)es.ControlItem);
                }

                foreach (ModbusSer ms in cp.CollectionModbus)
                {
                    collectionModbus.Add((ModbusControl)ms.ControlItem);
                }
            }

            //if (app.ConfigProgramBin.UseDatabase)
            //{
            //    SqlConnectionStringBuilder Sqlbuilder = new SqlConnectionStringBuilder();
            //    Sqlbuilder.DataSource = app.ConfigProgramBin.SQLServerName;
            //    Sqlbuilder.InitialCatalog = app.ConfigProgramBin.SQLDatabaseName;

            //    if (((AppWPF)Application.Current).ConfigProgramBin.SQLSecuritySSPI)
            //    {
            //        Sqlbuilder.IntegratedSecurity = true;
            //    }
            //    else
            //    {
            //        Sqlbuilder.UserID = app.ConfigProgramBin.SQLUserName;
            //        Sqlbuilder.Password = app.ConfigProgramBin.SQLPassword;
            //    }

            //    SqlConn = new SqlConnection();

            //    SqlConn.ConnectionString = Sqlbuilder.ConnectionString;

            //    try
            //    {
            //        SqlConn.Open();
            //    }
            //    catch (SystemException ex)
            //    {
            //        SqlConn.Close();

            //        if (ex is SqlException)
            //        {
            //            SqlException sqlex = ex as SqlException;

            //            foreach (SqlError er in sqlex.Errors)
            //            {
            //                WindowErrorMessages.LBMessageError.Items.Add(er.Message + "  " + DateTime.Now);
            //            }
            //        }
            //    }               
            //}      

            try
            {                
                foreach (EthernetControl ethernetControl in collectionEthernet)
                {
                    if (Interlocked.CompareExchange(ref IsStop, 0, 0) == 0)
                    {
                        if (ethernetControl.EthernetSer.EthernetProtocol == "TCP")
                        {
                            if (CollectionMessage.Count > 300)
                            {
                                CollectionMessage.RemoveAt(0);

                                CollectionMessage.Insert(298, "Сообщение " + " : " + " подключение к " + ethernetControl.EthernetSer.IPAddressServer[0] + "." + ethernetControl.EthernetSer.IPAddressServer[1] + "." + ethernetControl.EthernetSer.IPAddressServer[2] + "." + ethernetControl.EthernetSer.IPAddressServer[3] + " Порт " + ethernetControl.EthernetSer.PortServer + " " + DateTime.Now);
                            }
                            else
                            {
                                CollectionMessage.Add("Сообщение " + " : " + " подключение к " + ethernetControl.EthernetSer.IPAddressServer[0] + "." + ethernetControl.EthernetSer.IPAddressServer[1] + "." + ethernetControl.EthernetSer.IPAddressServer[2] + "." + ethernetControl.EthernetSer.IPAddressServer[3] + " Порт " + ethernetControl.EthernetSer.PortServer + " " + DateTime.Now);
                            }

                            EthernetObject Ethernet = new EthernetObject();
                            Ethernet.EthernetSer = ethernetControl.EthernetSer;

                            CollectionTCPEthernetObject.Add(Ethernet);

                            Thread threadConnect = new Thread(ConnectedTCP);
                            threadConnect.Start(Ethernet);

                            if (app.ConfigProgramBin.UseDatabase)
                            {
                                Thread threadDataBase = new Thread(ConnectedDataBaseEthernet);
                                threadDataBase.Start(Ethernet);
                            }
                        }
                        else if (ethernetControl.EthernetSer.EthernetProtocol == "UDP")
                        {
                            EthernetObject Ethernet = new EthernetObject();
                            Ethernet.EthernetSer = ethernetControl.EthernetSer;

                            CollectionUDPEthernetObject.Add(Ethernet);

                            Thread threadConnect = new Thread(ConnectedUDP);
                            threadConnect.Start(Ethernet);

                            if (app.ConfigProgramBin.UseDatabase)
                            {
                                Thread threadDataBase = new Thread(ConnectedDataBaseEthernet);
                                threadDataBase.Start(Ethernet);
                            }
                        }

                        //foreach (EthernetOperational eo in ethernetControl.EthernetSer.CollectionEthernetOperational)
                        //{
                        //    if (CollectionMessage.Count > 300)
                        //    {
                        //        CollectionMessage.RemoveAt(0);

                        //        CollectionMessage.Insert(298, "Сообщение " + " : " + " подключение к " + ethernetControl.EthernetSer.IPAddressServer[0] + "." + ethernetControl.EthernetSer.IPAddressServer[1] + "." + ethernetControl.EthernetSer.IPAddressServer[2] + "." + ethernetControl.EthernetSer.IPAddressServer[3] + " Порт " + ethernetControl.EthernetSer.PortServer + " " + DateTime.Now);
                        //    }
                        //    else
                        //    {
                        //        CollectionMessage.Add("Сообщение " + " : " + " подключение к " + ethernetControl.EthernetSer.IPAddressServer[0] + "." + ethernetControl.EthernetSer.IPAddressServer[1] + "." + ethernetControl.EthernetSer.IPAddressServer[2] + "." + ethernetControl.EthernetSer.IPAddressServer[3] + " Порт " + ethernetControl.EthernetSer.PortServer + " " + DateTime.Now);
                        //    }  

                        //    EthernetOperationalThread EthernetOperation = new EthernetOperationalThread();
                        //    EthernetOperation.EthernetSer = ethernetControl.EthernetSer;
                        //    EthernetOperation.EthernetOperational = eo;
                        //    EthernetOperation.TcpClient = new TcpClient();

                        //    CollectionTCPEthernetObject.Add(EthernetOperation);

                        //    Thread threadConnectOperational = new Thread(ConnectingEthernetOperational);
                        //    threadConnectOperational.Start(EthernetOperation);

                        //    if (app.ConfigProgramBin.UseDatabase)
                        //    {
                        //        Thread threadDataBase = new Thread(ConnectDataBaseEthernetOperational);
                        //        threadDataBase.Start(EthernetOperation);
                        //    }
                        //}
                    }
                }
              
                foreach (ComSer comSer in ((AppWPF)Application.Current).CollectionComSers)
                {
                    if (Interlocked.CompareExchange(ref IsStop, 0, 0) == 0)
                    {
                        List<ModbusObject> collectionModbusThread = new List<ModbusObject>();

                        SerialPortObject SerialPort = new SerialPortObject();                                               
                        SerialPort.PortName = comSer.ComPort;
                        SerialPort.BaudRate = comSer.BaudRate;
                        SerialPort.DataBits = comSer.DataBits;
                        SerialPort.ReadTimeout = comSer.ReadTimeout;
                        SerialPort.WriteTimeout = comSer.WriteTimeout;

                        if (comSer.StopBits == "None")
                        {
                            SerialPort.StopBits = StopBits.None;
                        }
                        else if (comSer.StopBits == "One")
                        {
                            SerialPort.StopBits = StopBits.One;
                        }
                        else if (comSer.StopBits == "OnePointFive")
                        {
                            SerialPort.StopBits = StopBits.OnePointFive;
                        }
                        else if (comSer.StopBits == "Two")
                        {
                            SerialPort.StopBits = StopBits.Two;
                        }

                        if (comSer.Parity == "Even")
                        {
                            SerialPort.Parity = Parity.Even;
                        }
                        else if (comSer.Parity == "Mark")
                        {
                            SerialPort.Parity = Parity.Mark;
                        }
                        else if (comSer.Parity == "None")
                        {
                            SerialPort.Parity = Parity.None;
                        }
                        else if (comSer.Parity == "Odd")
                        {
                            SerialPort.Parity = Parity.Odd;
                        }
                        else if (comSer.Parity == "Space")
                        {
                            SerialPort.Parity = Parity.Space;
                        }

                        try
                        {
                            SerialPort.Open();
                        }
                        catch (Exception ex)
                        {
                            break;
                        }
                        
                        CollectionSerialPortThread.Add(SerialPort);

                        ModbusSerialMaster modbus = null;

                        foreach (ModbusControl modbusControl in collectionModbus)
                        {
                            if (modbusControl.ModbusSer.ComPort == comSer.ComPort)
                            {
                                if (modbusControl.ModbusSer.Protocol == "RTU")
                                {
                                    modbus = ModbusSerialMaster.CreateRtu(SerialPort);
                                }
                                else
                                {
                                    modbus = ModbusSerialMaster.CreateAscii(SerialPort);
                                }
                                
                                modbus.Transport.Retries = 2;

                                ModbusObject ModbusSend = new ModbusObject();
                                ModbusSend.SerialPort = SerialPort;
                                ModbusSend.ModbusSerialMaster = modbus;
                                ModbusSend.ModbusControl = modbusControl;

                                collectionModbusThread.Add(ModbusSend);                              
                            }
                        }

                        if (app.ConfigProgramBin.UseDatabase)
                        {
                            Thread threadDataBase = new Thread(ConnectDataBaseModbus);
                            threadDataBase.Start(collectionModbusThread);
                        } 

                        Thread threadConnect = new Thread(ConnectingModbus);
                        threadConnect.Start(collectionModbusThread);                        
                    }                    
                }               
            }
            catch (SystemException ex)
            {
                Interlocked.Exchange(ref IsStop, 1);

                if (CollectionMessage.Count > 300)
                {
                    CollectionMessage.RemoveAt(0);

                    CollectionMessage.Insert(298, "Сообщение " + " : " + "Остановка опросов из-за ошибки: " + ex.Message + " " + DateTime.Now);
                }
                else
                {
                    CollectionMessage.Add("Сообщение " + " : " + "Остановка опросов из-за ошибки: " + ex.Message + " " + DateTime.Now);
                }                
            }
            finally
            {
                if (Interlocked.CompareExchange(ref IsStop, 1, 1) == 1)
                {                    
                    IsBindingStartProject = false;

                    if (CollectionMessage.Count > 300)
                    {
                        CollectionMessage.RemoveAt(0);

                        CollectionMessage.Insert(298, "Сообщение " + " : " + "Опрос остановлен" + " " + DateTime.Now);
                    }
                    else
                    {
                        CollectionMessage.Add("Сообщение " + " : " + "Опрос остановлен" + " " + DateTime.Now);
                    }                    
                }
            }
        }

        int Digital(ItemModbus Item, int countDigital)
        {
            if ((countDigital - Item.FormulaText.Length) != 0)
            {
                if (char.IsDigit(Item.FormulaText, countDigital) || Item.FormulaText[countDigital] == '.' || Item.FormulaText[countDigital] == ',')
                {
                    countDigital++;

                    countDigital = Digital(Item, countDigital);
                }
            }

            return countDigital;
        }

        int DigitalItemNet(ItemNet Item, int countDigital)
        {
            if ((countDigital - Item.FormulaText.Length) != 0)
            {
                if (char.IsDigit(Item.FormulaText, countDigital) || Item.FormulaText[countDigital] == '.' || Item.FormulaText[countDigital] == ',')
                {
                    countDigital++;

                    countDigital = DigitalItemNet(Item, countDigital);
                }
            }

            return countDigital;
        }

        private void ConnectingModbus(object obj)
        {
            List<ModbusObject> collectionModbusObject = (List<ModbusObject>)obj;

            try
            {
                while (Interlocked.CompareExchange(ref IsStop, 0, 0) == 0)
                {
                    foreach (ModbusObject modbusSendObject in collectionModbusObject)
                    {
                        Thread.Sleep(StaticValues.TimeSleep);

                        if (Interlocked.CompareExchange(ref IsStop, 1, 1) == 1)
                        {
                            return;
                        }

                        if (modbusSendObject.IsReconnect)
                        {
                            modbusSendObject.TimerReconnect.Start();

                            if (modbusSendObject.TimerReconnect.ElapsedMilliseconds >= 60 * 1000)
                            {
                                modbusSendObject.IsReconnect = false;

                                modbusSendObject.TimerReconnect.Reset();
                            }                            
                        }
                        else
                        {
                            modbusSendObject.TimerPeriod.Start();

                            if (modbusSendObject.TimerPeriod.ElapsedMilliseconds >= modbusSendObject.ModbusControl.ModbusSer.Time * 1000)
                            {
                                modbusSendObject.TimerPeriod.Reset();

                                ushort[] data;
                                byte[] buffer = new byte[8];
                                ushort temp;

                                foreach (ItemModbus item in modbusSendObject.ModbusControl.ModbusSer.CollectionItemModbus)
                                {
                                    if (Interlocked.CompareExchange(ref IsStop, 0, 0) == 0)
                                    {
                                        return;
                                    }

                                    try
                                    {                                        
                                        if (item.TypeValue == "float")
                                        {
                                            if (item.Function == 3)
                                            {
                                                data = modbusSendObject.ModbusSerialMaster.ReadHoldingRegisters(modbusSendObject.ModbusControl.ModbusSer.SlaveAddress, item.Address, 2);
                                            }
                                            else
                                            {
                                                data = modbusSendObject.ModbusSerialMaster.ReadInputRegisters(modbusSendObject.ModbusControl.ModbusSer.SlaveAddress, item.Address, 2);
                                            }
                                            
                                            if (data != null)
                                            {
                                                temp = data[0];

                                                data[0] = data[1];
                                                data[1] = temp;

                                                buffer[0] = BitConverter.GetBytes(data[0])[0];
                                                buffer[1] = BitConverter.GetBytes(data[0])[1];
                                                buffer[2] = BitConverter.GetBytes(data[1])[0];
                                                buffer[3] = BitConverter.GetBytes(data[1])[1];

                                                if (item.FormulaText.Length != 0)
                                                {
                                                    List<float> collectionDigital = new List<float>();

                                                    int countDigital = 0;

                                                    if (item.FormulaText[0] == '/')
                                                    {
                                                        countDigital++;

                                                        if ((countDigital - item.FormulaText.Length) != 0)
                                                        {
                                                            countDigital = Digital(item, countDigital);

                                                            collectionDigital.Add(float.Parse(item.FormulaText.Substring(1, countDigital - 1)));

                                                            lock(modbusSendObject.LockValue)
                                                            {                                                               
                                                                item.Value = BitConverter.ToSingle(buffer, 0) / collectionDigital[0];                                                               
                                                            }                                                            
                                                        }
                                                        else
                                                        {
                                                            lock (modbusSendObject.LockValue)
                                                            {
                                                                item.Value = BitConverter.ToSingle(buffer, 0);
                                                            }                                                                                                                        
                                                        }
                                                    }
                                                    else if (item.FormulaText.IndexOf("CSF") != -1)
                                                    {
                                                        if (item.FormulaText[3] == '/')
                                                        {
                                                            countDigital = 3;

                                                            countDigital++;

                                                            if ((countDigital - item.FormulaText.Length) != 0)
                                                            {
                                                                countDigital = Digital(item, countDigital);

                                                                collectionDigital.Add(float.Parse(item.FormulaText.Substring(4, countDigital - 4)));

                                                                lock (modbusSendObject.LockValue)
                                                                {
                                                                    item.Value = BitConverter.ToInt16(buffer, 2) / collectionDigital[0];
                                                                }                                                                                                                                  
                                                            }
                                                            else
                                                            {
                                                                lock (modbusSendObject.LockValue)
                                                                {
                                                                    item.Value = BitConverter.ToInt16(buffer, 2);
                                                                }                                                                                                                                  
                                                            }
                                                        }
                                                        else
                                                        {
                                                            lock (modbusSendObject.LockValue)
                                                            {
                                                                item.Value = BitConverter.ToInt16(buffer, 2);
                                                            }                                                                                                                          
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    lock (modbusSendObject.LockValue)
                                                    {
                                                        item.Value = BitConverter.ToSingle(buffer, 0);
                                                    }                                                                                                            
                                                }
                                            }
                                        }
                                        else if (item.TypeValue == "short")
                                        {
                                            if (item.Function == 3)
                                            {
                                                data = modbusSendObject.ModbusSerialMaster.ReadHoldingRegisters(modbusSendObject.ModbusControl.ModbusSer.SlaveAddress, item.Address, 1);
                                            }
                                            else
                                            {
                                                data = modbusSendObject.ModbusSerialMaster.ReadInputRegisters(modbusSendObject.ModbusControl.ModbusSer.SlaveAddress, item.Address, 1);
                                            }

                                            if (data != null)
                                            {
                                                buffer[0] = BitConverter.GetBytes(data[0])[0];
                                                buffer[1] = BitConverter.GetBytes(data[0])[1];

                                                if (item.FormulaText.Length != 0)
                                                {
                                                    List<short> collectionDigital = new List<short>();

                                                    int countDigital = 0;

                                                    if (item.FormulaText[0] == '/')
                                                    {
                                                        countDigital++;

                                                        if ((countDigital - item.FormulaText.Length) != 0)
                                                        {
                                                            countDigital = Digital(item, countDigital);

                                                            collectionDigital.Add(short.Parse(item.FormulaText.Substring(1, countDigital - 1)));

                                                            lock (modbusSendObject.LockValue)
                                                            {
                                                                item.Value = BitConverter.ToInt16(buffer, 0) / collectionDigital[0];
                                                            }                                                                                                                          
                                                        }
                                                        else
                                                        {
                                                            lock (modbusSendObject.LockValue)
                                                            {
                                                                item.Value = BitConverter.ToInt16(buffer, 0);
                                                            }                                                                                                                          
                                                        }
                                                    }
                                                    else
                                                    {
                                                        lock (modbusSendObject.LockValue)
                                                        {
                                                            item.Value = BitConverter.ToInt16(buffer, 0);
                                                        }                                                                                                                  
                                                    }
                                                }
                                                else
                                                {
                                                    lock (modbusSendObject.LockValue)
                                                    {
                                                        item.Value = BitConverter.ToInt16(buffer, 0);
                                                    }                                                                                                          
                                                }
                                            }
                                        }
                                        else if (item.TypeValue == "ushort")
                                        {
                                            if (item.Function == 3)
                                            {
                                                data = modbusSendObject.ModbusSerialMaster.ReadHoldingRegisters(modbusSendObject.ModbusControl.ModbusSer.SlaveAddress, item.Address, 1);
                                            }
                                            else
                                            {
                                                data = modbusSendObject.ModbusSerialMaster.ReadInputRegisters(modbusSendObject.ModbusControl.ModbusSer.SlaveAddress, item.Address, 1);
                                            }

                                            if (data != null)
                                            {
                                                buffer[0] = BitConverter.GetBytes(data[0])[0];
                                                buffer[1] = BitConverter.GetBytes(data[0])[1];

                                                if (item.FormulaText.Length != 0)
                                                {
                                                    List<ushort> collectionDigital = new List<ushort>();

                                                    int countDigital = 0;

                                                    if (item.FormulaText[0] == '/')
                                                    {
                                                        countDigital++;

                                                        if ((countDigital - item.FormulaText.Length) != 0)
                                                        {
                                                            countDigital = Digital(item, countDigital);

                                                            collectionDigital.Add(ushort.Parse(item.FormulaText.Substring(1, countDigital - 1)));

                                                            lock (modbusSendObject.LockValue)
                                                            {
                                                                item.Value = BitConverter.ToUInt16(buffer, 0) / collectionDigital[0];
                                                            }                                                                                                                         
                                                        }
                                                        else
                                                        {
                                                            lock (modbusSendObject.LockValue)
                                                            {
                                                                item.Value = BitConverter.ToUInt16(buffer, 0);
                                                            }                                                                                                                          
                                                        }
                                                    }
                                                    else
                                                    {
                                                        lock (modbusSendObject.LockValue)
                                                        {
                                                            item.Value = BitConverter.ToUInt16(buffer, 0);
                                                        }                                                                                                                  
                                                    }
                                                }
                                                else
                                                {
                                                    lock (modbusSendObject.LockValue)
                                                    {
                                                        item.Value = BitConverter.ToUInt16(buffer, 0);
                                                    }                                                                                                           
                                                }
                                            }
                                        }
                                        else if (item.TypeValue == "int")
                                        {
                                            if (item.Function == 3)
                                            {
                                                data = modbusSendObject.ModbusSerialMaster.ReadHoldingRegisters(modbusSendObject.ModbusControl.ModbusSer.SlaveAddress, item.Address, 2);
                                            }
                                            else
                                            {
                                                data = modbusSendObject.ModbusSerialMaster.ReadInputRegisters(modbusSendObject.ModbusControl.ModbusSer.SlaveAddress, item.Address, 2);
                                            }

                                            if (data != null)
                                            {
                                                temp = data[0];

                                                data[0] = data[1];
                                                data[1] = temp;

                                                buffer[0] = BitConverter.GetBytes(data[0])[0];
                                                buffer[1] = BitConverter.GetBytes(data[0])[1];
                                                buffer[2] = BitConverter.GetBytes(data[1])[0];
                                                buffer[3] = BitConverter.GetBytes(data[1])[1];

                                                if (item.FormulaText.Length != 0)
                                                {
                                                    List<int> collectionDigital = new List<int>();

                                                    int countDigital = 0;

                                                    if (item.FormulaText[0] == '/')
                                                    {
                                                        countDigital++;

                                                        if ((countDigital - item.FormulaText.Length) != 0)
                                                        {
                                                            countDigital = Digital(item, countDigital);

                                                            collectionDigital.Add(int.Parse(item.FormulaText.Substring(1, countDigital - 1)));

                                                            lock (modbusSendObject.LockValue)
                                                            {
                                                                item.Value = BitConverter.ToInt32(buffer, 0) / collectionDigital[0];
                                                            }                                                                                                                         
                                                        }
                                                        else
                                                        {
                                                            lock (modbusSendObject.LockValue)
                                                            {
                                                                item.Value = BitConverter.ToInt32(buffer, 0);
                                                            }                                                                                                                         
                                                        }
                                                    }
                                                    else
                                                    {
                                                        lock (modbusSendObject.LockValue)
                                                        {
                                                            item.Value = BitConverter.ToInt32(buffer, 0);
                                                        }                                                                                                                 
                                                    }
                                                }
                                                else
                                                {
                                                    lock (modbusSendObject.LockValue)
                                                    {
                                                        item.Value = BitConverter.ToInt32(buffer, 0);
                                                    }                                                                                                           
                                                }
                                            }
                                        }
                                        else if (item.TypeValue == "uint")
                                        {
                                            if (item.Function == 3)
                                            {
                                                data = modbusSendObject.ModbusSerialMaster.ReadHoldingRegisters(modbusSendObject.ModbusControl.ModbusSer.SlaveAddress, item.Address, 2);
                                            }
                                            else
                                            {
                                                data = modbusSendObject.ModbusSerialMaster.ReadInputRegisters(modbusSendObject.ModbusControl.ModbusSer.SlaveAddress, item.Address, 2);
                                            }

                                            if (data != null)
                                            {
                                                temp = data[0];

                                                data[0] = data[1];
                                                data[1] = temp;

                                                buffer[0] = BitConverter.GetBytes(data[0])[0];
                                                buffer[1] = BitConverter.GetBytes(data[0])[1];
                                                buffer[2] = BitConverter.GetBytes(data[1])[0];
                                                buffer[3] = BitConverter.GetBytes(data[1])[1];

                                                if (item.FormulaText.Length != 0)
                                                {
                                                    List<uint> collectionDigital = new List<uint>();

                                                    int countDigital = 0;

                                                    if (item.FormulaText[0] == '/')
                                                    {
                                                        countDigital++;

                                                        if ((countDigital - item.FormulaText.Length) != 0)
                                                        {
                                                            countDigital = Digital(item, countDigital);

                                                            collectionDigital.Add(uint.Parse(item.FormulaText.Substring(1, countDigital - 1)));

                                                            lock (modbusSendObject.LockValue)
                                                            {
                                                                item.Value = BitConverter.ToUInt32(buffer, 0) / collectionDigital[0];
                                                            }                                                                                                                         
                                                        }
                                                        else
                                                        {
                                                            lock (modbusSendObject.LockValue)
                                                            {
                                                                item.Value = BitConverter.ToUInt32(buffer, 0);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        lock (modbusSendObject.LockValue)
                                                        {
                                                            item.Value = BitConverter.ToUInt32(buffer, 0);
                                                        }                                                                                                                    
                                                    }
                                                }
                                                else
                                                {
                                                    lock (modbusSendObject.LockValue)
                                                    {
                                                        item.Value = BitConverter.ToUInt32(buffer, 0);
                                                    }                                                                                                         
                                                }
                                            }
                                        }

                                        lock (modbusSendObject.LockValue)
                                        {
                                            modbusSendObject.IsAvailableData = true;
                                        }
                                    }
                                    catch
                                    {                                        
                                        modbusSendObject.IsReconnect = true;
                                        lock (modbusSendObject.LockValue)
                                        {
                                            modbusSendObject.IsAvailableData = false;
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (SystemException ex)
            {
                if (Interlocked.CompareExchange(ref IsStop, 0, 0) == 0)
                {                    
                    if (CollectionMessage.Count > 300)
                    {
                        CollectionMessage.RemoveAt(0);

                        CollectionMessage.Insert(298, "Сообщение " + " : " + "Опрос " + collectionModbusObject[0].ModbusControl.ModbusSer.ComPort + "Остановлен из-за ошибки " + ex.Message + " " + DateTime.Now);
                    }
                    else
                    {
                        CollectionMessage.Add("Сообщение " + " : " + "Опрос " + collectionModbusObject[0].ModbusControl.ModbusSer.ComPort + "Остановлен из-за ошибки " + ex.Message + " " + DateTime.Now);
                    }
                }
            }
            finally
            {
                foreach (ModbusObject modbusObject in collectionModbusObject)
                {
                    modbusObject.TimerPeriod.Reset();
                    modbusObject.TimerReconnect.Reset();

                    modbusObject.IsReconnect = false;

                    lock (modbusObject.LockAvailableData)
                    {
                        modbusObject.IsAvailableData = false;
                    }

                    lock(modbusObject.LockSerialPort)
                    {
                        if (modbusObject.SerialPort != null)
                        {
                            modbusObject.SerialPort.Close();
                        }
                    }                                                                  
                }
            }
        }
               
        private void ConnectDataBaseModbus(object obj)
        {
            List<ModbusObject> collectionModbusObject = (List<ModbusObject>)obj;

            Npgsql.NpgsqlConnection SqlConn = new Npgsql.NpgsqlConnection();
            SQLObject SQLObject = new SQLObject();

            try
            {
                object value = null;
                object prevValue = null;

                Npgsql.NpgsqlParameter parametrDateTime = new Npgsql.NpgsqlParameter();
                parametrDateTime.ParameterName = "Time";
                parametrDateTime.NpgsqlDbType = NpgsqlDbType.Timestamp;

                Npgsql.NpgsqlParameter parametrValue = new Npgsql.NpgsqlParameter();
                parametrValue.ParameterName = "Value";
                parametrValue.NpgsqlDbType = NpgsqlDbType.Real;

                foreach (ModbusObject modbusObject in collectionModbusObject)
                {
                    modbusObject.CollectionTimer.Clear();

                    foreach (ItemModbus itemModbus in modbusObject.ModbusControl.ModbusSer.CollectionItemModbus)
                    {
                        if (itemModbus.IsSaveDatabase || itemModbus.IsEmergencySaveDB)
                        {
                            StopwatchItemModbus timer = new StopwatchItemModbus();
                            timer.ItemModbus = itemModbus;
                            timer.EmergencyTimerUp = new Stopwatch();
                            timer.EmergencyTimerDown = new Stopwatch();

                            modbusObject.CollectionTimer.Add(timer);
                        }
                    }
                }
            
                AppWPF app = (AppWPF)Application.Current;

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

                SqlConn.ConnectionString = connstring;

                SqlConn.Open();

                Npgsql.NpgsqlCommand cmd = new Npgsql.NpgsqlCommand();
                cmd.Parameters.Add(parametrValue);
                cmd.Parameters.Add(parametrDateTime);
                cmd.Connection = SqlConn;

                SQLObject.SQL = SqlConn;
                SQLObject.IsWork = true;

                CollectionSQLObject.Add(SQLObject);

                bool isAvailableData;

                while (true)
                {
                    foreach (ModbusObject modbusObject in collectionModbusObject)
                    {
                        Thread.Sleep(StaticValues.TimeSleep);

                        if (Interlocked.CompareExchange(ref IsStop, 1, 1) == 1)
                        {                                                      
                            return;
                        }

                        lock(modbusObject.LockAvailableData)
                        {
                            isAvailableData = modbusObject.IsAvailableData;
                        }

                        if (isAvailableData)
                        {
                            foreach (StopwatchItemModbus timer in modbusObject.CollectionTimer)
                            {
                                timer.Start();

                                if (timer.ItemModbus.IsEmergencySaveDB)
                                {
                                    if (timer.ItemModbus.IsEmergencyUp)
                                    {
                                        timer.EmergencyTimerUp.Start();

                                        if (timer.EmergencyTimerUp.ElapsedMilliseconds >= (timer.ItemModbus.PeriodEmergencySaveDB * 1000))
                                        {
                                            lock(modbusObject.LockValue)
                                            {
                                                value = timer.ItemModbus.Value;
                                                prevValue = timer.PrevValue;
                                            }                                          

                                            timer.EmergencyTimerUp.Reset();

                                            if (value is float)
                                            {
                                                if (!float.IsInfinity((float)value) && !float.IsNaN((float)value))
                                                {
                                                    if (Convert.ToSingle(value) > Convert.ToSingle(timer.ItemModbus.EmergencyUp))
                                                    {                                                    
                                                        if (timer.ItemModbus.TypeValue == "int")
                                                        {
                                                            FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Integer, modbusObject);
                                                        }
                                                        else if (timer.ItemModbus.TypeValue == "uint")
                                                        {
                                                            FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Bigint, modbusObject);
                                                        }
                                                        else if (timer.ItemModbus.TypeValue == "short")
                                                        {
                                                            FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint, modbusObject);
                                                        }
                                                        else if (timer.ItemModbus.TypeValue == "ushort")
                                                        {
                                                            FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Integer, modbusObject);
                                                        }
                                                        else if (timer.ItemModbus.TypeValue == "byte")
                                                        {
                                                            FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint, modbusObject);
                                                        }
                                                        else if (timer.ItemModbus.TypeValue == "sbyte")
                                                        {
                                                            FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint, modbusObject);
                                                        }
                                                        else if (timer.ItemModbus.TypeValue == "bool")
                                                        {
                                                            parametrValue.ParameterName = "Value";
                                                            parametrValue.NpgsqlValue = value;
                                                            parametrValue.NpgsqlDbType = NpgsqlDbType.Boolean;

                                                            parametrDateTime.NpgsqlValue = DateTime.Now;

                                                            cmd.CommandText = "Insert into " + "\"" + timer.ItemModbus.TableName + "\"" + @"(""Value"",""Time"") Values (:Value, :Time)";
                                                            cmd.ExecuteNonQuery();
                                                        }
                                                        else if (timer.ItemModbus.TypeValue == "float")
                                                        {
                                                            FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Real, modbusObject);
                                                        }
                                                    }                                                    
                                                }                                               
                                            }
                                            else
                                            {
                                                if (Convert.ToDecimal(value) > Convert.ToDecimal(timer.ItemModbus.EmergencyUp))
                                                {                                                    
                                                    if (timer.ItemModbus.TypeValue == "int")
                                                    {
                                                        FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Integer, modbusObject);
                                                    }
                                                    else if (timer.ItemModbus.TypeValue == "uint")
                                                    {
                                                        FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Bigint, modbusObject);
                                                    }
                                                    else if (timer.ItemModbus.TypeValue == "short")
                                                    {
                                                        FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint, modbusObject);
                                                    }
                                                    else if (timer.ItemModbus.TypeValue == "ushort")
                                                    {
                                                        FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Integer, modbusObject);
                                                    }
                                                    else if (timer.ItemModbus.TypeValue == "byte")
                                                    {
                                                        FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint, modbusObject);
                                                    }
                                                    else if (timer.ItemModbus.TypeValue == "sbyte")
                                                    {
                                                        FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint, modbusObject);
                                                    }
                                                    else if (timer.ItemModbus.TypeValue == "bool")
                                                    {
                                                        parametrValue.ParameterName = "Value";
                                                        parametrValue.NpgsqlValue = timer.ItemModbus.Value;
                                                        parametrValue.NpgsqlDbType = NpgsqlDbType.Boolean;

                                                        parametrDateTime.NpgsqlValue = DateTime.Now;

                                                        cmd.CommandText = "Insert into " + "\"" + timer.ItemModbus.TableName + "\"" + @"(""Value"",""Time"") Values (:Value, :Time)";
                                                        cmd.ExecuteNonQuery();
                                                    }
                                                    else if (timer.ItemModbus.TypeValue == "float")
                                                    {
                                                        FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Real, modbusObject);
                                                    }
                                                }                                               
                                            }
                                        }                                        
                                    }

                                    if (timer.ItemModbus.IsEmergencyDown)
                                    {
                                        timer.EmergencyTimerDown.Start();

                                        if (timer.EmergencyTimerDown.ElapsedMilliseconds >= (timer.ItemModbus.PeriodEmergencySaveDB * 1000))
                                        {
                                            lock (modbusObject.LockValue)
                                            {
                                                value = timer.ItemModbus.Value;
                                                prevValue = timer.PrevValue;
                                            }                                         

                                            if (value is float)
                                            {
                                                if (!float.IsInfinity((float)value) && !float.IsNaN((float)value))
                                                {
                                                    if (Convert.ToSingle(value) < Convert.ToSingle(timer.ItemModbus.EmergencyDown))
                                                    {
                                                        timer.EmergencyTimerDown.Reset();

                                                        if (timer.ItemModbus.TypeValue == "int")
                                                        {
                                                            FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Integer, modbusObject);
                                                        }
                                                        else if (timer.ItemModbus.TypeValue == "uint")
                                                        {
                                                            FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Bigint, modbusObject);
                                                        }
                                                        else if (timer.ItemModbus.TypeValue == "short")
                                                        {
                                                            FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint, modbusObject);
                                                        }
                                                        else if (timer.ItemModbus.TypeValue == "ushort")
                                                        {
                                                            FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Integer, modbusObject);
                                                        }
                                                        else if (timer.ItemModbus.TypeValue == "byte")
                                                        {
                                                            FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint, modbusObject);
                                                        }
                                                        else if (timer.ItemModbus.TypeValue == "sbyte")
                                                        {
                                                            FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint, modbusObject);
                                                        }
                                                        else if (timer.ItemModbus.TypeValue == "bool")
                                                        {
                                                            parametrValue.ParameterName = "Value";
                                                            parametrValue.NpgsqlValue = value;
                                                            parametrValue.NpgsqlDbType = NpgsqlDbType.Boolean;

                                                            parametrDateTime.NpgsqlValue = DateTime.Now;

                                                            cmd.CommandText = "Insert into " + "\"" + timer.ItemModbus.TableName + "\"" + @"(""Value"",""Time"") Values (:Value, :Time)";
                                                            cmd.ExecuteNonQuery();
                                                        }
                                                        else if (timer.ItemModbus.TypeValue == "float")
                                                        {
                                                            FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Real, modbusObject);
                                                        }
                                                    }
                                                    
                                                }                                               
                                            }
                                            else
                                            {
                                                if (Convert.ToDecimal(value) < Convert.ToDecimal(timer.ItemModbus.EmergencyDown))
                                                {                                                   
                                                    timer.EmergencyTimerDown.Reset();

                                                    if (timer.ItemModbus.TypeValue == "int")
                                                    {
                                                        FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Integer, modbusObject);
                                                    }
                                                    else if (timer.ItemModbus.TypeValue == "uint")
                                                    {
                                                        FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Bigint, modbusObject);
                                                    }
                                                    else if (timer.ItemModbus.TypeValue == "short")
                                                    {
                                                        FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint, modbusObject);
                                                    }
                                                    else if (timer.ItemModbus.TypeValue == "ushort")
                                                    {
                                                        FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Integer, modbusObject);
                                                    }
                                                    else if (timer.ItemModbus.TypeValue == "byte")
                                                    {
                                                        FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint, modbusObject);
                                                    }
                                                    else if (timer.ItemModbus.TypeValue == "sbyte")
                                                    {
                                                        FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint, modbusObject);
                                                    }
                                                    else if (timer.ItemModbus.TypeValue == "bool")
                                                    {
                                                        parametrValue.ParameterName = "Value";
                                                        parametrValue.NpgsqlValue = value;
                                                        parametrValue.NpgsqlDbType = NpgsqlDbType.Boolean;

                                                        parametrDateTime.NpgsqlValue = DateTime.Now;

                                                        cmd.CommandText = "Insert into " + "\"" + timer.ItemModbus.TableName + "\"" + @"(""Value"",""Time"") Values (:Value, :Time)";
                                                        cmd.ExecuteNonQuery();
                                                    }
                                                    else if (timer.ItemModbus.TypeValue == "float")
                                                    {
                                                        FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Real, modbusObject);
                                                    }
                                                }                                               
                                            }
                                        }                                       
                                    }                                   
                                }

                                if (timer.ItemModbus.IsSaveDatabase)
                                {
                                    if (timer.ElapsedMilliseconds >= (timer.ItemModbus.PeridTimeSaveDB * 1000))
                                    {
                                        timer.Reset();

                                        lock (modbusObject.LockValue)
                                        {
                                            value = timer.ItemModbus.Value;
                                            prevValue = timer.PrevValue;
                                        }

                                        if (timer.ItemModbus.TypeValue == "int")
                                        {
                                            FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Integer, modbusObject);
                                        }
                                        else if (timer.ItemModbus.TypeValue == "uint")
                                        {
                                            FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Bigint, modbusObject);
                                        }
                                        else if (timer.ItemModbus.TypeValue == "short")
                                        {
                                            FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint, modbusObject);
                                        }
                                        else if (timer.ItemModbus.TypeValue == "ushort")
                                        {
                                            FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Integer, modbusObject);
                                        }
                                        else if (timer.ItemModbus.TypeValue == "byte")
                                        {
                                            FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint, modbusObject);
                                        }
                                        else if (timer.ItemModbus.TypeValue == "sbyte")
                                        {
                                            FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint, modbusObject);
                                        }
                                        else if (timer.ItemModbus.TypeValue == "bool")
                                        {
                                            parametrValue.ParameterName = "Value";
                                            parametrValue.NpgsqlValue = value;
                                            parametrValue.NpgsqlDbType = NpgsqlDbType.Boolean;

                                            parametrDateTime.NpgsqlValue = DateTime.Now;

                                            cmd.CommandText = "Insert into " + "\"" + timer.ItemModbus.TableName + "\"" + @"(""Value"",""Time"") Values (:Value, :Time)";
                                            cmd.ExecuteNonQuery();
                                        }
                                        else if (timer.ItemModbus.TypeValue == "float")
                                        {
                                            FormulaSaveDatabaseModbus(prevValue, value, timer, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Real, modbusObject);
                                        }
                                    }
                                }
                            }                            
                        }
                    }
                }
            }
            catch (SystemException ex)
            {
                if (ex is SqlException)
                {
                    SqlException sqlex = ex as SqlException;

                    foreach (SqlError er in sqlex.Errors)
                    {
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            if (CollectionMessage.Count > 300)
                            {
                                CollectionMessage.RemoveAt(0);

                                CollectionMessage.Insert(298, "Сообщение " + " : " + "Ошибка SQL: " + er.Message + ". Данные не будут сохраняться в БД: " + collectionModbusObject[0].ModbusControl.ModbusSer.ComPort + " " + DateTime.Now);
                            }
                            else
                            {
                                CollectionMessage.Add("Сообщение " + " : " + "Ошибка SQL: " + er.Message + ". Данные не будут сохраняться в БД: " + collectionModbusObject[0].ModbusControl.ModbusSer.ComPort + " " + DateTime.Now);
                            }
                        }));
                    }
                }
                else
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        if (CollectionMessage.Count > 300)
                        {
                            CollectionMessage.RemoveAt(0);

                            CollectionMessage.Insert(298, "Сообщение " + " : " + "Ошибка SQL: " + ex.Message + ". Данные не будут сохраняться в БД: " + collectionModbusObject[0].ModbusControl.ModbusSer.ComPort + " " + DateTime.Now);
                        }
                        else
                        {
                            CollectionMessage.Add("Сообщение " + " : " + "Ошибка SQL: " + ex.Message + ". Данные не будут сохраняться в БД: " + collectionModbusObject[0].ModbusControl.ModbusSer.ComPort + " " + DateTime.Now);
                        }
                    }));
                }
            }
            finally
            {               
                SqlConn.Close();
                SqlConn.Dispose();

                SQLObject.IsWork = false;
            }
        }

        private void FormulaSaveDatabaseModbus(object PrevValue, object Value, StopwatchItemModbus timer, Npgsql.NpgsqlParameter parametrValue, Npgsql.NpgsqlParameter parametrDateTime, Npgsql.NpgsqlCommand cmd, NpgsqlDbType dbType, ModbusObject mst)
        {
            if (Value is float)
            {
                if (float.IsNaN((float)Value) || float.IsInfinity((float)Value))
                {
                    return;
                }
            }
            else if (Value is double)
            {
                if (double.IsNaN((double)Value) || double.IsInfinity((double)Value))
                {
                    return;
                }
            }

            if (timer.ItemModbus.FormulaText.Length > 0)
            {
                int i2 = timer.ItemModbus.FormulaText.LastIndexOf("NDB");

                if (i2 != -1)
                {
                    List<decimal> collectionDigital = new List<decimal>();

                    int firstDigital = i2 + 3;
                    int endDigital = 0;

                    if (char.IsDigit(timer.ItemModbus.FormulaText, firstDigital))
                    {
                        if ((firstDigital - timer.ItemModbus.FormulaText.Length) != 0)
                        {
                            endDigital = Digital(timer.ItemModbus, firstDigital);
                            int diff = endDigital - firstDigital;
                            string tt = timer.ItemModbus.FormulaText.Substring(firstDigital, diff);
                            tt = tt.Replace(',', '.');

                            collectionDigital.Add(decimal.Parse(tt, CultureInfo.InvariantCulture));

                            if (Convert.ToDecimal(Value) > collectionDigital[0])
                            {
                                return;
                            }
                        }
                    }
                }

                int i = timer.ItemModbus.FormulaText.LastIndexOf("SDB");

                if (i != -1)
                {
                    List<decimal> collectionDigital = new List<decimal>();

                    int firstDigital = i + 3;
                    int endDigital = 0;

                    if (char.IsDigit(timer.ItemModbus.FormulaText, firstDigital))
                    {                       
                        endDigital = Digital(timer.ItemModbus, firstDigital);
                        int diff = endDigital - firstDigital;
                        string tt = timer.ItemModbus.FormulaText.Substring(firstDigital, diff);
                        tt = tt.Replace(',', '.');

                        collectionDigital.Add(decimal.Parse(tt, CultureInfo.InvariantCulture));

                        if (PrevValue != null)
                        {
                            if (PrevValue is float)
                            {
                                if (float.IsNaN((float)PrevValue) || float.IsInfinity((float)PrevValue))
                                {
                                    return;
                                }
                            }
                            else if (PrevValue is double)
                            {
                                if (double.IsNaN((double)PrevValue) || double.IsInfinity((double)PrevValue))
                                {
                                    return;
                                }
                            }                                                            

                            if (Math.Abs((Convert.ToDecimal(Value) - Convert.ToDecimal(PrevValue))) >= collectionDigital[0])
                            {
                                parametrValue.NpgsqlValue = Value;
                                parametrValue.NpgsqlDbType = dbType;

                                PrevValue = Value;

                                parametrDateTime.NpgsqlValue = DateTime.Now;

                                cmd.CommandText = "Insert into " + "\"" + timer.ItemModbus.TableName + "\"" + @"(""Value"",""Time"") Values(:Value, :Time)";
                                cmd.ExecuteNonQuery();

                                return;
                            }
                            else
                            {
                                return;
                            }                                                             
                        }                                          
                    }
                    else
                    {
                        parametrValue.NpgsqlValue = Value;
                        parametrValue.NpgsqlDbType = dbType;

                        PrevValue = Value;

                        parametrDateTime.NpgsqlValue = DateTime.Now;

                        cmd.CommandText = "Insert into " + "\"" + timer.ItemModbus.TableName + "\"" + @"(""Value"",""Time"") Values(:Value, :Time)";
                        cmd.ExecuteNonQuery();

                        return;
                    } 
                }
                else
                {
                    parametrValue.NpgsqlValue = Value;
                    parametrValue.NpgsqlDbType = dbType;

                    parametrDateTime.NpgsqlValue = DateTime.Now;

                    cmd.CommandText = "Insert into " + "\"" + timer.ItemModbus.TableName + "\"" + @"(""Value"",""Time"") Values(:Value, :Time)";
                    cmd.ExecuteNonQuery();

                    return;
                }
            }
            else
            {
                parametrValue.NpgsqlValue = Value;
                parametrValue.NpgsqlDbType = dbType;

                parametrDateTime.NpgsqlValue = DateTime.Now;

                cmd.CommandText = "Insert into " + "\"" + timer.ItemModbus.TableName + "\"" + @"(""Value"",""Time"") Values(:Value, :Time)";
                cmd.ExecuteNonQuery();
            }
        }

        private void FormulaSaveDatabaseEthernet(object PrevValue, StopwatchItemNet timer, object value, Npgsql.NpgsqlParameter parametrValue, Npgsql.NpgsqlParameter parametrDateTime, Npgsql.NpgsqlCommand cmd, NpgsqlDbType dbType)
        {           
            if (value is float)
            {
                if(float.IsNaN((float)value) || float.IsInfinity((float)value))
                {
                    return;
                }
            }
            else if(value is double)
            {
                if(double.IsNaN((double)value) || double.IsInfinity((double)value))
                {
                    return;
                }
            }

            if (timer.ItemNet.FormulaText.Length > 0)
            {
                int i2 = timer.ItemNet.FormulaText.LastIndexOf("NDB");

                if (i2 != -1)
                {
                    List<decimal> collectionDigital = new List<decimal>();

                    int firstDigital = i2 + 3;
                    int endDigital = 0;

                    if (char.IsDigit(timer.ItemNet.FormulaText, firstDigital))
                    {
                        endDigital = DigitalItemNet(timer.ItemNet, firstDigital);
                        int diff = endDigital - firstDigital;
                        string tt = timer.ItemNet.FormulaText.Substring(firstDigital, diff);
                        tt = tt.Replace(',', '.');

                        collectionDigital.Add(decimal.Parse(tt, NumberStyles.Any, CultureInfo.InvariantCulture));

                        if (Convert.ToDecimal(value) > collectionDigital[0])
                        {
                            return;
                        }
                    }
                }

                int i = timer.ItemNet.FormulaText.LastIndexOf("SDB");

                if (i != -1)
                {
                    List<decimal> collectionDigital = new List<decimal>();

                    int firstDigital = i + 3;
                    int endDigital = 0;

                    if (char.IsDigit(timer.ItemNet.FormulaText, firstDigital))
                    {
                        endDigital = DigitalItemNet(timer.ItemNet, firstDigital);
                        int diff = endDigital - firstDigital;
                        string tt = timer.ItemNet.FormulaText.Substring(firstDigital, diff);
                        tt = tt.Replace(',', '.');

                        collectionDigital.Add(decimal.Parse(tt, NumberStyles.Any, CultureInfo.InvariantCulture));

                        if (PrevValue != null)
                        {
                            if (PrevValue is float)
                            {
                                if (float.IsNaN((float)timer.PrevValue) || float.IsInfinity((float)timer.PrevValue))
                                {
                                    return;
                                }
                            }
                            else if (timer.PrevValue is double)
                            {
                                if (double.IsNaN((double)timer.PrevValue) || double.IsInfinity((double)timer.PrevValue))
                                {
                                    return;
                                }
                            }

                            if (Math.Abs((Convert.ToDecimal(value) - Convert.ToDecimal(timer.PrevValue))) >= collectionDigital[0])
                            {
                                parametrValue.NpgsqlValue = value;
                                parametrValue.NpgsqlDbType = dbType;

                                timer.PrevValue = value;

                                parametrDateTime.NpgsqlValue = DateTime.Now;

                                cmd.CommandText = "Insert into " + "\"" + timer.ItemNet.TableName + "\"" + @"(""Value"",""Time"") Values(:Value, :Time)";
                                cmd.ExecuteNonQuery();

                                return;
                            }
                            else
                            {
                                return;
                            }

                        }
                    }
                    else
                    {
                        parametrValue.NpgsqlValue = value;
                        parametrValue.NpgsqlDbType = dbType;

                        timer.PrevValue = value;

                        parametrDateTime.NpgsqlValue = DateTime.Now;

                        cmd.CommandText = "Insert into " + "\"" + timer.ItemNet.TableName + "\"" + @"(""Value"",""Time"") Values(:Value, :Time)";
                        cmd.ExecuteNonQuery();

                        return;                   
                    }
                }
                else
                {                    
                    parametrValue.NpgsqlValue = value;
                    parametrValue.NpgsqlDbType = dbType;

                    timer.PrevValue = value;

                    parametrDateTime.NpgsqlValue = DateTime.Now;

                    cmd.CommandText = "Insert into " + "\"" + timer.ItemNet.TableName + "\"" + @"(""Value"",""Time"") Values(:Value, :Time)";
                    cmd.ExecuteNonQuery();

                    return;                   
                }
            }
            else
            {
                parametrValue.NpgsqlValue = value;
                parametrValue.NpgsqlDbType = dbType;

                timer.PrevValue = value;

                parametrDateTime.NpgsqlValue = DateTime.Now;

                cmd.CommandText = "Insert into " + "\"" + timer.ItemNet.TableName + "\"" + @"(""Value"",""Time"") Values(:Value, :Time)";
                cmd.ExecuteNonQuery();

                return;                    
            }           
        }

        private void ConnectedDataBaseEthernet(object obj)
        {
            EthernetObject ethernetObject = (EthernetObject)obj;

            Npgsql.NpgsqlConnection SqlConn = new Npgsql.NpgsqlConnection();

            try
            {
                object value = null;
                object prevValue = null;

                List<StopwatchItemNet> collectionTimer = new List<StopwatchItemNet>();
                              
                Npgsql.NpgsqlParameter parametrDateTime = new Npgsql.NpgsqlParameter();
                parametrDateTime.ParameterName = "Time";
                parametrDateTime.NpgsqlDbType = NpgsqlDbType.Timestamp;

                Npgsql.NpgsqlParameter parametrValue = new Npgsql.NpgsqlParameter();
                parametrValue.ParameterName = "Value";
                parametrValue.NpgsqlDbType = NpgsqlDbType.Real;

                foreach (ItemNet itemNet in ethernetObject.EthernetSer.CollectionItemNetRec)
                {
                    if (itemNet.IsSaveDatabase || itemNet.IsEmergencySaveDB)
                    {
                        StopwatchItemNet timer = new StopwatchItemNet();
                        timer.ItemNet = itemNet;
                        timer.EmergencyTimerUp = new Stopwatch();
                        timer.EmergencyTimerDown = new Stopwatch();

                        collectionTimer.Add(timer);
                    }
                }

                foreach (ItemNet itemNet in ethernetObject.EthernetSer.CollectionItemNetSend)
                {
                    if (itemNet.IsSaveDatabase || itemNet.IsEmergencySaveDB)
                    {
                        StopwatchItemNet timer = new StopwatchItemNet();
                        timer.ItemNet = itemNet;
                        timer.EmergencyTimerUp = new Stopwatch();
                        timer.EmergencyTimerDown = new Stopwatch();

                        collectionTimer.Add(timer);
                    }
                }

                AppWPF app = (AppWPF)Application.Current;

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

                SqlConn.ConnectionString = connstring;

                SqlConn.Open();

                Npgsql.NpgsqlCommand cmd = new Npgsql.NpgsqlCommand();
                cmd.Parameters.Add(parametrValue);
                cmd.Parameters.Add(parametrDateTime);
                cmd.Connection = SqlConn;

                SQLObject sqlObject = new SQLObject();
                sqlObject.SQL = SqlConn;

                CollectionSQLObject.Add(sqlObject);

                bool isAvailableData;

                while (true)
                {
                    if (Interlocked.CompareExchange(ref IsStop, 1, 1) == 1)
                    {
                        SqlConn.Close();
                        SqlConn.Dispose();

                        return;
                    }

                    lock(ethernetObject.LockBool)
                    {
                        isAvailableData = ethernetObject.IsAvailableData;
                    }

                    if (isAvailableData)
                    {
                        foreach (StopwatchItemNet timer in collectionTimer)
                        {
                            Thread.Sleep(StaticValues.TimeSleep);

                            timer.Start();

                            if (timer.ItemNet.IsEmergencySaveDB)
                            {                                
                                if (timer.ItemNet.IsEmergencyUp)
                                {
                                    timer.EmergencyTimerUp.Start();

                                    if (timer.EmergencyTimerUp.ElapsedMilliseconds >= (timer.ItemNet.PeriodEmergencySaveDB * 1000))
                                    {
                                        lock(ethernetObject.LockValue)
                                        {
                                            value = timer.ItemNet.Value;
                                            prevValue = value;
                                        }
                                        
                                        timer.EmergencyTimerUp.Reset();

                                        if (value is float)
                                        {
                                            if (!float.IsInfinity((float)value) && !float.IsNaN((float)value))
                                            {
                                                if (Convert.ToSingle(value) > Convert.ToSingle(timer.ItemNet.EmergencyUp))
                                                {                                                                                                        
                                                    if (timer.ItemNet.TypeValue == "long")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Numeric);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "ulong")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Bigint);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "int")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Integer);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "uint")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Bigint);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "short")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "ushort")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Integer);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "byte")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "sbyte")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "bool")
                                                    {
                                                        parametrValue.NpgsqlValue = value;
                                                        parametrValue.NpgsqlDbType = NpgsqlDbType.Boolean;

                                                        parametrDateTime.NpgsqlValue = DateTime.Now;

                                                        cmd.CommandText = "Insert into " + "\"" + timer.ItemNet.TableName + "\"" + @"(""Value"",""Time"") Values (:Value, :Time)";
                                                        cmd.ExecuteNonQuery();
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "decimal")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Numeric);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "float")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Real);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "double")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Double);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "char")
                                                    {
                                                        parametrValue.NpgsqlValue = value;
                                                        parametrValue.NpgsqlDbType = NpgsqlDbType.Char;

                                                        parametrDateTime.NpgsqlValue = DateTime.Now;

                                                        cmd.CommandText = "Insert into " + "\"" + timer.ItemNet.TableName + "\"" + @"(""Value"",""Time"") Values (:Value, :Time)";
                                                        cmd.ExecuteNonQuery();
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "string")
                                                    {
                                                        parametrValue.NpgsqlValue = value;
                                                        parametrValue.NpgsqlDbType = NpgsqlDbType.Text;

                                                        parametrDateTime.NpgsqlValue = DateTime.Now;

                                                        cmd.CommandText = "Insert into " + "\"" + timer.ItemNet.TableName + "\"" + @"(""Value"",""Time"") Values (:Value, :Time)";
                                                        cmd.ExecuteNonQuery();
                                                    }                                                                                                                                                                                                    
                                                }                                               
                                            }                                            
                                        }
                                        else if (value is double)
                                        {
                                            if (!double.IsInfinity((double)value) && !double.IsNaN((double)value))
                                            {
                                                if (Convert.ToDouble(value) > Convert.ToDouble(timer.ItemNet.EmergencyUp))
                                                {                                                    
                                                    if (timer.ItemNet.TypeValue == "long")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Numeric);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "ulong")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Bigint);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "int")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Integer);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "uint")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Bigint);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "short")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "ushort")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Integer);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "byte")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "sbyte")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "bool")
                                                    {
                                                        parametrValue.NpgsqlValue = value;
                                                        parametrValue.NpgsqlDbType = NpgsqlDbType.Boolean;

                                                        parametrDateTime.NpgsqlValue = DateTime.Now;

                                                        cmd.CommandText = "Insert into " + "\"" + timer.ItemNet.TableName + "\"" + @"(""Value"",""Time"") Values (:Value, :Time)";
                                                        cmd.ExecuteNonQuery();
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "decimal")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Numeric);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "float")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Real);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "double")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Double);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "char")
                                                    {
                                                        parametrValue.NpgsqlValue = value;
                                                        parametrValue.NpgsqlDbType = NpgsqlDbType.Char;

                                                        parametrDateTime.NpgsqlValue = DateTime.Now;

                                                        cmd.CommandText = "Insert into " + "\"" + timer.ItemNet.TableName + "\"" + @"(""Value"",""Time"") Values (:Value, :Time)";
                                                        cmd.ExecuteNonQuery();
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "string")
                                                    {
                                                        parametrValue.NpgsqlValue = value;
                                                        parametrValue.NpgsqlDbType = NpgsqlDbType.Text;

                                                        parametrDateTime.NpgsqlValue = DateTime.Now;

                                                        cmd.CommandText = "Insert into " + "\"" + timer.ItemNet.TableName + "\"" + @"(""Value"",""Time"") Values (:Value, :Time)";
                                                        cmd.ExecuteNonQuery();
                                                    }
                                                }                                               
                                            }                                           
                                        }
                                        else
                                        {
                                            if (Convert.ToDecimal(value) > Convert.ToDecimal(timer.ItemNet.EmergencyUp))
                                            {                                                                                               
                                                if (timer.ItemNet.TypeValue == "long")
                                                {
                                                    FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Numeric);
                                                }
                                                else if (timer.ItemNet.TypeValue == "ulong")
                                                {
                                                    FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Bigint);
                                                }
                                                else if (timer.ItemNet.TypeValue == "int")
                                                {
                                                    FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Integer);
                                                }
                                                else if (timer.ItemNet.TypeValue == "uint")
                                                {
                                                    FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Bigint);
                                                }
                                                else if (timer.ItemNet.TypeValue == "short")
                                                {
                                                    FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint);
                                                }
                                                else if (timer.ItemNet.TypeValue == "ushort")
                                                {
                                                    FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Integer);
                                                }
                                                else if (timer.ItemNet.TypeValue == "byte")
                                                {
                                                    FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint);
                                                }
                                                else if (timer.ItemNet.TypeValue == "sbyte")
                                                {
                                                    FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint);
                                                }
                                                else if (timer.ItemNet.TypeValue == "bool")
                                                {
                                                    parametrValue.NpgsqlValue = value;
                                                    parametrValue.NpgsqlDbType = NpgsqlDbType.Boolean;

                                                    parametrDateTime.NpgsqlValue = DateTime.Now;

                                                    cmd.CommandText = "Insert into " + "\"" + timer.ItemNet.TableName + "\"" + @"(""Value"",""Time"") Values (:Value, :Time)";
                                                    cmd.ExecuteNonQuery();
                                                }
                                                else if (timer.ItemNet.TypeValue == "decimal")
                                                {
                                                    FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Numeric);
                                                }
                                                else if (timer.ItemNet.TypeValue == "float")
                                                {
                                                    FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Real);
                                                }
                                                else if (timer.ItemNet.TypeValue == "double")
                                                {
                                                    FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Double);
                                                }
                                                else if (timer.ItemNet.TypeValue == "char")
                                                {
                                                    parametrValue.NpgsqlValue = value;
                                                    parametrValue.NpgsqlDbType = NpgsqlDbType.Char;

                                                    parametrDateTime.NpgsqlValue = DateTime.Now;

                                                    cmd.CommandText = "Insert into " + "\"" + timer.ItemNet.TableName + "\"" + @"(""Value"",""Time"") Values (:Value, :Time)";
                                                    cmd.ExecuteNonQuery();
                                                }
                                                else if (timer.ItemNet.TypeValue == "string")
                                                {
                                                    parametrValue.NpgsqlValue = value;
                                                    parametrValue.NpgsqlDbType = NpgsqlDbType.Text;

                                                    parametrDateTime.NpgsqlValue = DateTime.Now;

                                                    cmd.CommandText = "Insert into " + "\"" + timer.ItemNet.TableName + "\"" + @"(""Value"",""Time"") Values (:Value, :Time)";
                                                    cmd.ExecuteNonQuery();
                                                }                                                                                                                                             
                                            }                                           
                                        }                                        
                                    }                                  
                                }

                                if (timer.ItemNet.IsEmergencyDown)
                                {
                                    timer.EmergencyTimerDown.Start();

                                    if (timer.EmergencyTimerDown.ElapsedMilliseconds >= (timer.ItemNet.PeriodEmergencySaveDB * 1000))
                                    {
                                        timer.EmergencyTimerDown.Reset();

                                        lock (ethernetObject.LockValue)
                                        {
                                            value = timer.ItemNet.Value;
                                            prevValue = value;
                                        }

                                        if (timer.ItemNet.Value is float)
                                        {
                                            if (!float.IsInfinity((float)timer.ItemNet.Value) && !float.IsNaN((float)timer.ItemNet.Value))
                                            {
                                                if (Convert.ToSingle(timer.ItemNet.Value) < Convert.ToSingle(timer.ItemNet.EmergencyDown))
                                                {                                                 
                                                    if (timer.ItemNet.TypeValue == "long")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Numeric);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "ulong")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Bigint);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "int")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Integer);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "uint")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Bigint);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "short")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "ushort")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Integer);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "byte")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "sbyte")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "bool")
                                                    {
                                                        parametrValue.NpgsqlValue = timer.ItemNet.Value;
                                                        parametrValue.NpgsqlDbType = NpgsqlDbType.Boolean;

                                                        parametrDateTime.NpgsqlValue = DateTime.Now;

                                                        cmd.CommandText = "Insert into " + "\"" + timer.ItemNet.TableName + "\"" + @"(""Value"",""Time"") Values (:Value, :Time)";
                                                        cmd.ExecuteNonQuery();
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "decimal")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Numeric);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "float")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Real);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "double")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Double);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "char")
                                                    {
                                                        parametrValue.NpgsqlValue = timer.ItemNet.Value;
                                                        parametrValue.NpgsqlDbType = NpgsqlDbType.Char;

                                                        parametrDateTime.NpgsqlValue = DateTime.Now;

                                                        cmd.CommandText = "Insert into " + "\"" + timer.ItemNet.TableName + "\"" + @"(""Value"",""Time"") Values (:Value, :Time)";
                                                        cmd.ExecuteNonQuery();
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "string")
                                                    {
                                                        parametrValue.NpgsqlValue = timer.ItemNet.Value;
                                                        parametrValue.NpgsqlDbType = NpgsqlDbType.Text;

                                                        parametrDateTime.NpgsqlValue = DateTime.Now;

                                                        cmd.CommandText = "Insert into " + "\"" + timer.ItemNet.TableName + "\"" + @"(""Value"",""Time"") Values (:Value, :Time)";
                                                        cmd.ExecuteNonQuery();
                                                    }                                                                                                                                                                                                         
                                                }                                               
                                            }                                                                                     
                                        }
                                        else if (timer.ItemNet.Value is double)
                                        {
                                            if (!double.IsInfinity((double)timer.ItemNet.Value) && !double.IsNaN((double)timer.ItemNet.Value))
                                            {
                                                if (Convert.ToDouble(timer.ItemNet.Value) < Convert.ToDouble(timer.ItemNet.EmergencyDown))
                                                {                                                   
                                                    if (timer.ItemNet.TypeValue == "long")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Numeric);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "ulong")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Bigint);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "int")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Integer);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "uint")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Bigint);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "short")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "ushort")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Integer);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "byte")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "sbyte")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "bool")
                                                    {
                                                        parametrValue.NpgsqlValue = timer.ItemNet.Value;
                                                        parametrValue.NpgsqlDbType = NpgsqlDbType.Boolean;

                                                        parametrDateTime.NpgsqlValue = DateTime.Now;

                                                        cmd.CommandText = "Insert into " + "\"" + timer.ItemNet.TableName + "\"" + @"(""Value"",""Time"") Values (:Value, :Time)";
                                                        cmd.ExecuteNonQuery();
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "decimal")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Numeric);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "float")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Real);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "double")
                                                    {
                                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Double);
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "char")
                                                    {
                                                        parametrValue.NpgsqlValue = timer.ItemNet.Value;
                                                        parametrValue.NpgsqlDbType = NpgsqlDbType.Char;

                                                        parametrDateTime.NpgsqlValue = DateTime.Now;

                                                        cmd.CommandText = "Insert into " + "\"" + timer.ItemNet.TableName + "\"" + @"(""Value"",""Time"") Values (:Value, :Time)";
                                                        cmd.ExecuteNonQuery();
                                                    }
                                                    else if (timer.ItemNet.TypeValue == "string")
                                                    {
                                                        parametrValue.NpgsqlValue = timer.ItemNet.Value;
                                                        parametrValue.NpgsqlDbType = NpgsqlDbType.Text;

                                                        parametrDateTime.NpgsqlValue = DateTime.Now;

                                                        cmd.CommandText = "Insert into " + "\"" + timer.ItemNet.TableName + "\"" + @"(""Value"",""Time"") Values (:Value, :Time)";
                                                        cmd.ExecuteNonQuery();
                                                    }                                                                                                                                                                                                               
                                                }                                               
                                            }                                            
                                        }
                                        else
                                        {
                                            if (Convert.ToDecimal(timer.ItemNet.Value) < Convert.ToDecimal(timer.ItemNet.EmergencyDown))
                                            {                                                                                              
                                                if (timer.ItemNet.TypeValue == "long")
                                                {
                                                    FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Numeric);
                                                }
                                                else if (timer.ItemNet.TypeValue == "ulong")
                                                {
                                                    FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Bigint);
                                                }
                                                else if (timer.ItemNet.TypeValue == "int")
                                                {
                                                    FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Integer);
                                                }
                                                else if (timer.ItemNet.TypeValue == "uint")
                                                {
                                                    FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Bigint);
                                                }
                                                else if (timer.ItemNet.TypeValue == "short")
                                                {
                                                    FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint);
                                                }
                                                else if (timer.ItemNet.TypeValue == "ushort")
                                                {
                                                    FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Integer);
                                                }
                                                else if (timer.ItemNet.TypeValue == "byte")
                                                {
                                                    FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint);
                                                }
                                                else if (timer.ItemNet.TypeValue == "sbyte")
                                                {
                                                    FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint);
                                                }
                                                else if (timer.ItemNet.TypeValue == "bool")
                                                {
                                                    parametrValue.NpgsqlValue = timer.ItemNet.Value;
                                                    parametrValue.NpgsqlDbType = NpgsqlDbType.Boolean;

                                                    parametrDateTime.NpgsqlValue = DateTime.Now;

                                                    cmd.CommandText = "Insert into " + "\"" + timer.ItemNet.TableName + "\"" + @"(""Value"",""Time"") Values (:Value, :Time)";
                                                    cmd.ExecuteNonQuery();
                                                }
                                                else if (timer.ItemNet.TypeValue == "decimal")
                                                {
                                                    FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Numeric);
                                                }
                                                else if (timer.ItemNet.TypeValue == "float")
                                                {
                                                    FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Real);
                                                }
                                                else if (timer.ItemNet.TypeValue == "double")
                                                {
                                                    FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Double);
                                                }
                                                else if (timer.ItemNet.TypeValue == "char")
                                                {
                                                    parametrValue.NpgsqlValue = timer.ItemNet.Value;
                                                    parametrValue.NpgsqlDbType = NpgsqlDbType.Char;

                                                    parametrDateTime.NpgsqlValue = DateTime.Now;

                                                    cmd.CommandText = "Insert into " + "\"" + timer.ItemNet.TableName + "\"" + @"(""Value"",""Time"") Values (:Value, :Time)";
                                                    cmd.ExecuteNonQuery();
                                                }
                                                else if (timer.ItemNet.TypeValue == "string")
                                                {
                                                    parametrValue.NpgsqlValue = timer.ItemNet.Value;
                                                    parametrValue.NpgsqlDbType = NpgsqlDbType.Text;

                                                    parametrDateTime.NpgsqlValue = DateTime.Now;

                                                    cmd.CommandText = "Insert into " + "\"" + timer.ItemNet.TableName + "\"" + @"(""Value"",""Time"") Values (:Value, :Time)";
                                                    cmd.ExecuteNonQuery();
                                                }                                                                                                                                                                                               
                                            }                                           
                                        }
                                    }                                   
                                }                                
                            }

                            if (timer.ItemNet.IsSaveDatabase)
                            {
                                if (timer.ElapsedMilliseconds >= (timer.ItemNet.PeridTimeSaveDB * 1000))
                                {
                                    lock(ethernetObject.LockValue)
                                    {
                                        value = timer.ItemNet.Value;
                                        prevValue = value;
                                    }

                                    timer.Reset();

                                    if (timer.ItemNet.TypeValue == "long")
                                    {
                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Numeric);
                                    }
                                    else if (timer.ItemNet.TypeValue == "ulong")
                                    {
                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Bigint);
                                    }
                                    else if (timer.ItemNet.TypeValue == "int")
                                    {
                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Integer);
                                    }
                                    else if (timer.ItemNet.TypeValue == "uint")
                                    {
                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Bigint);
                                    }
                                    else if (timer.ItemNet.TypeValue == "short")
                                    {
                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint);
                                    }
                                    else if (timer.ItemNet.TypeValue == "ushort")
                                    {
                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Integer);
                                    }
                                    else if (timer.ItemNet.TypeValue == "byte")
                                    {
                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint);
                                    }
                                    else if (timer.ItemNet.TypeValue == "sbyte")
                                    {
                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Smallint);
                                    }
                                    else if (timer.ItemNet.TypeValue == "bool")
                                    {
                                        parametrValue.NpgsqlValue = timer.ItemNet.Value;
                                        parametrValue.NpgsqlDbType = NpgsqlDbType.Boolean;

                                        parametrDateTime.NpgsqlValue = DateTime.Now;

                                        cmd.CommandText = "Insert into " + "\"" + timer.ItemNet.TableName + "\"" + @"(""Value"",""Time"") Values (:Value, :Time)";
                                        cmd.ExecuteNonQuery();
                                    }
                                    else if (timer.ItemNet.TypeValue == "decimal")
                                    {
                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Numeric);
                                    }
                                    else if (timer.ItemNet.TypeValue == "float")
                                    {
                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Real);
                                    }
                                    else if (timer.ItemNet.TypeValue == "double")
                                    {
                                        FormulaSaveDatabaseEthernet(prevValue, timer, value, parametrValue, parametrDateTime, cmd, NpgsqlDbType.Double);
                                    }
                                    else if (timer.ItemNet.TypeValue == "char")
                                    {
                                        parametrValue.ParameterName = "Value";
                                        parametrValue.NpgsqlValue = timer.ItemNet.Value;
                                        parametrValue.NpgsqlDbType = NpgsqlDbType.Char;

                                        parametrDateTime.NpgsqlValue = DateTime.Now;

                                        cmd.CommandText = "Insert into " + "\"" + timer.ItemNet.TableName + "\"" + @"(""Value"",""Time"") Values (:Value, :Time)";
                                        cmd.ExecuteNonQuery();
                                    }
                                    else if (timer.ItemNet.TypeValue == "string")
                                    {
                                        parametrValue.NpgsqlValue = timer.ItemNet.Value;
                                        parametrValue.NpgsqlDbType = NpgsqlDbType.Text;

                                        parametrDateTime.NpgsqlValue = DateTime.Now;

                                        cmd.CommandText = "Insert into " + "\"" + timer.ItemNet.TableName + "\"" + @"(""Value"",""Time"") Values (:Value, :Time)";
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }
                        }                        
                    }
                }
            }
            catch (SystemException ex)
            {                
                if (ex is SqlException)
                {
                    SqlException sqlex = ex as SqlException;

                    foreach (SqlError er in sqlex.Errors)
                    {
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            if (CollectionMessage.Count > 300)
                            {
                                CollectionMessage.RemoveAt(0);

                                CollectionMessage.Insert(298, "Сообщение " + " : " + "Ошибка SQL: " + er.Message + ". Данные не будут сохраняться в БД с IP: " + ethernetObject.EthernetSer.IPAddressServer[0] + "." + ethernetObject.EthernetSer.IPAddressServer[1]
                                   + "." + ethernetObject.EthernetSer.IPAddressServer[2] +
                                                "." + ethernetObject.EthernetSer.IPAddressServer[3] + " " + DateTime.Now);
                            }
                            else
                            {
                                CollectionMessage.Add("Сообщение " + " : " + "Ошибка SQL: " + er.Message + ". Данные не будут сохраняться в БД с IP: " + ethernetObject.EthernetSer.IPAddressServer[0] + "." + ethernetObject.EthernetSer.IPAddressServer[1]
                                   + "." + ethernetObject.EthernetSer.IPAddressServer[2] +
                                                "." + ethernetObject.EthernetSer.IPAddressServer[3] + " " + DateTime.Now);
                            }
                        }));
                    }
                }
                else
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        if (CollectionMessage.Count > 300)
                        {
                            CollectionMessage.RemoveAt(0);

                            CollectionMessage.Insert(298, "Сообщение " + " : " + "Ошибка SQL: " + ex.Message + ". Данные не будут сохраняться в БД с IP: " + ethernetObject.EthernetSer.IPAddressServer[0] + "." + ethernetObject.EthernetSer.IPAddressServer[1]
                               + "." + ethernetObject.EthernetSer.IPAddressServer[2] +
                                            "." + ethernetObject.EthernetSer.IPAddressServer[3] + " " + DateTime.Now);
                        }
                        else
                        {
                            CollectionMessage.Add("Сообщение " + " : " + "Ошибка SQL: " + ex.Message + ". Данные не будут сохраняться в БД с IP: " + ethernetObject.EthernetSer.IPAddressServer[0] + "." + ethernetObject.EthernetSer.IPAddressServer[1]
                               + "." + ethernetObject.EthernetSer.IPAddressServer[2] +
                                            "." + ethernetObject.EthernetSer.IPAddressServer[3] + " " + DateTime.Now);
                        }
                    }));
                }
            }
            finally
            {
                SqlConn.Close();
                SqlConn.Dispose();
            }
        }

        //private void ConnectDataBaseEthernetOperational(object obj)
        //{
        //    EthernetObject ethernetSendTread = (EthernetObject)obj;

        //    Npgsql.NpgsqlConnection SqlConn = new Npgsql.NpgsqlConnection();

        //    try
        //    {
        //        List<StopwatchItemNet> collectionTimer = new List<StopwatchItemNet>();

        //        Npgsql.NpgsqlCommand insert = new Npgsql.NpgsqlCommand();
               
        //        Npgsql.NpgsqlParameter parametrDateTime = new Npgsql.NpgsqlParameter();
        //        parametrDateTime.ParameterName = "@Time";
        //        parametrDateTime.DbType = DbType.DateTime;

        //        foreach (ItemNet itemNet in ethernetSendTread.EthernetSer.CollectionItemNetRec)
        //        {
        //            if (itemNet.IsSaveDatabase)
        //            {
        //                StopwatchItemNet timer = new StopwatchItemNet();
        //                timer.ItemNet = itemNet;
        //                timer.EmergencyTimerUp = new Stopwatch();
        //                timer.EmergencyTimerDown = new Stopwatch();

        //                collectionTimer.Add(timer);
        //            }
        //        }

        //        foreach (ItemNet itemNet in ethernetSendTread.EthernetSer.CollectionItemNetSend)
        //        {
        //            if (itemNet.IsSaveDatabase)
        //            {
        //                StopwatchItemNet timer = new StopwatchItemNet();
        //                timer.ItemNet = itemNet;
        //                timer.EmergencyTimerUp = new Stopwatch();
        //                timer.EmergencyTimerDown = new Stopwatch();

        //                collectionTimer.Add(timer);
        //            }
        //        }

        //        AppWPF app = (AppWPF)Application.Current;

        //        //SqlConnectionStringBuilder Sqlbuilder = new SqlConnectionStringBuilder();
        //        //Sqlbuilder.DataSource = app.ConfigProgramBin.SQLServerName;
        //        //Sqlbuilder.InitialCatalog = app.ConfigProgramBin.SQLDatabaseName;

        //        //if (((AppWPF)Application.Current).ConfigProgramBin.SQLSecuritySSPI)
        //        //{
        //        //    Sqlbuilder.IntegratedSecurity = true;
        //        //}
        //        //else
        //        //{
        //        //    Sqlbuilder.UserID = app.ConfigProgramBin.SQLUserName;
        //        //    Sqlbuilder.Password = app.ConfigProgramBin.SQLPassword;
        //        //}

        //        string connstring = String.Format("Server={0};Port={1};" +
        //            "User Id={2};Password={3};Database={4};",
        //            app.ConfigProgramBin.SQLServerName, 5432, app.ConfigProgramBin.SQLUserName,
        //            app.ConfigProgramBin.SQLPassword, app.ConfigProgramBin.SQLDatabaseName);

        //        SqlConn.ConnectionString = connstring;

        //        SqlConn.Open();

        //        Npgsql.NpgsqlCommand cmd = new Npgsql.NpgsqlCommand();
        //        cmd.Connection = SqlConn;

        //        CollectionSQLObject.Add(SqlConn);

        //        while (true)
        //        {
        //            if (IsStop)
        //            {
        //                SqlConn.Close();
        //                SqlConn.Dispose();

        //                return;
        //            }

        //            if (ethernetSendTread.DatabaseConnect)
        //            {
        //                foreach (StopwatchItemNet timer in collectionTimer)
        //                {
        //                    timer.Start();

        //                    if (timer.ElapsedMilliseconds >= 100)
        //                    {
        //                        timer.Reset();

        //                        //if (timer.ItemNet.TypeValue == "long")
        //                        //{
        //                        //    timer.ParametrValue.ParameterName = "@Value";
        //                        //    timer.ParametrValue.Value = timer.ItemNet.Value;
        //                        //    timer.ParametrValue.DbType = DbType.Int64;

        //                        //    parametrDateTime.Value = DateTime.Now;

        //                        //    cmd.Parameters.Clear();
        //                        //    cmd.CommandText = "Insert into " + timer.ItemNet.TableName + "(Value,Time) Values (@Value,@Time)";
        //                        //    cmd.Parameters.Add(timer.ParametrValue);
        //                        //    cmd.Parameters.Add(parametrDateTime);
        //                        //    cmd.ExecuteNonQuery();
        //                        //}
        //                        //else if (timer.ItemNet.TypeValue == "ulong")
        //                        //{
        //                        //    timer.ParametrValue.ParameterName = "@Value";
        //                        //    timer.ParametrValue.Value = timer.ItemNet.Value;
        //                        //    timer.ParametrValue.DbType = DbType.UInt64;

        //                        //    parametrDateTime.Value = DateTime.Now;

        //                        //    cmd.Parameters.Clear();
        //                        //    cmd.CommandText = "Insert into " + timer.ItemNet.TableName + "(Value,Time) Values (@Value,@Time)";
        //                        //    cmd.Parameters.Add(timer.ParametrValue);
        //                        //    cmd.Parameters.Add(parametrDateTime);
        //                        //    cmd.ExecuteNonQuery();
        //                        //}
        //                        //else if (timer.ItemNet.TypeValue == "int")
        //                        //{
        //                        //    timer.ParametrValue.ParameterName = "@Value";
        //                        //    timer.ParametrValue.Value = timer.ItemNet.Value;
        //                        //    timer.ParametrValue.DbType = DbType.Int32;

        //                        //    parametrDateTime.Value = DateTime.Now;

        //                        //    cmd.Parameters.Clear();
        //                        //    cmd.CommandText = "Insert into " + timer.ItemNet.TableName + "(Value,Time) Values (@Value,@Time)";
        //                        //    cmd.Parameters.Add(timer.ParametrValue);
        //                        //    cmd.Parameters.Add(parametrDateTime);
        //                        //    cmd.ExecuteNonQuery();
        //                        //}
        //                        //else if (timer.ItemNet.TypeValue == "uint")
        //                        //{
        //                        //    timer.ParametrValue.ParameterName = "@Value";
        //                        //    timer.ParametrValue.Value = timer.ItemNet.Value;
        //                        //    timer.ParametrValue.DbType = DbType.UInt32;

        //                        //    parametrDateTime.Value = DateTime.Now;

        //                        //    cmd.Parameters.Clear();
        //                        //    cmd.CommandText = "Insert into " + timer.ItemNet.TableName + "(Value,Time) Values (@Value,@Time)";
        //                        //    cmd.Parameters.Add(timer.ParametrValue);
        //                        //    cmd.Parameters.Add(parametrDateTime);
        //                        //    cmd.ExecuteNonQuery();
        //                        //}
        //                        //else if (timer.ItemNet.TypeValue == "short")
        //                        //{
        //                        //    timer.ParametrValue.ParameterName = "@Value";
        //                        //    timer.ParametrValue.Value = timer.ItemNet.Value;
        //                        //    timer.ParametrValue.DbType = DbType.Int16;

        //                        //    parametrDateTime.Value = DateTime.Now;

        //                        //    cmd.Parameters.Clear();
        //                        //    cmd.CommandText = "Insert into " + timer.ItemNet.TableName + "(Value,Time) Values (@Value,@Time)";
        //                        //    cmd.Parameters.Add(timer.ParametrValue);
        //                        //    cmd.Parameters.Add(parametrDateTime);
        //                        //    cmd.ExecuteNonQuery();
        //                        //}
        //                        //else if (timer.ItemNet.TypeValue == "ushort")
        //                        //{
        //                        //    timer.ParametrValue.ParameterName = "@Value";
        //                        //    timer.ParametrValue.Value = timer.ItemNet.Value;
        //                        //    timer.ParametrValue.DbType = DbType.UInt16;

        //                        //    parametrDateTime.Value = DateTime.Now;

        //                        //    cmd.Parameters.Clear();
        //                        //    cmd.CommandText = "Insert into " + timer.ItemNet.TableName + "(Value,Time) Values (@Value,@Time)";
        //                        //    cmd.Parameters.Add(timer.ParametrValue);
        //                        //    cmd.Parameters.Add(parametrDateTime);
        //                        //    cmd.ExecuteNonQuery();
        //                        //}
        //                        //else if (timer.ItemNet.TypeValue == "byte")
        //                        //{
        //                        //    timer.ParametrValue.ParameterName = "@Value";
        //                        //    timer.ParametrValue.Value = timer.ItemNet.Value;
        //                        //    timer.ParametrValue.DbType = DbType.Byte;

        //                        //    parametrDateTime.Value = DateTime.Now;

        //                        //    cmd.Parameters.Clear();
        //                        //    cmd.CommandText = "Insert into " + timer.ItemNet.TableName + "(Value,Time) Values (@Value,@Time)";
        //                        //    cmd.Parameters.Add(timer.ParametrValue);
        //                        //    cmd.Parameters.Add(parametrDateTime);
        //                        //    cmd.ExecuteNonQuery();
        //                        //}
        //                        //else if (timer.ItemNet.TypeValue == "sbyte")
        //                        //{
        //                        //    timer.ParametrValue.ParameterName = "@Value";
        //                        //    timer.ParametrValue.Value = timer.ItemNet.Value;
        //                        //    timer.ParametrValue.DbType = DbType.SByte;

        //                        //    parametrDateTime.Value = DateTime.Now;

        //                        //    cmd.Parameters.Clear();
        //                        //    cmd.CommandText = "Insert into " + timer.ItemNet.TableName + "(Value,Time) Values (@Value,@Time)";
        //                        //    cmd.Parameters.Add(timer.ParametrValue);
        //                        //    cmd.Parameters.Add(parametrDateTime);
        //                        //    cmd.ExecuteNonQuery();
        //                        //}
        //                        //else if (timer.ItemNet.TypeValue == "bool")
        //                        //{
        //                        //    timer.ParametrValue.ParameterName = "@Value";
        //                        //    timer.ParametrValue.Value = timer.ItemNet.Value;
        //                        //    timer.ParametrValue.DbType = DbType.Boolean;

        //                        //    parametrDateTime.Value = DateTime.Now;

        //                        //    cmd.Parameters.Clear();
        //                        //    cmd.CommandText = "Insert into " + timer.ItemNet.TableName + "(Value,Time) Values (@Value,@Time)";
        //                        //    cmd.Parameters.Add(timer.ParametrValue);
        //                        //    cmd.Parameters.Add(parametrDateTime);
        //                        //    cmd.ExecuteNonQuery();
        //                        //}
        //                        //else if (timer.ItemNet.TypeValue == "decimal")
        //                        //{
        //                        //    timer.ParametrValue.ParameterName = "@Value";
        //                        //    timer.ParametrValue.Value = timer.ItemNet.Value;
        //                        //    timer.ParametrValue.DbType = DbType.Decimal;

        //                        //    parametrDateTime.Value = DateTime.Now;

        //                        //    cmd.Parameters.Clear();
        //                        //    cmd.CommandText = "Insert into " + timer.ItemNet.TableName + "(Value,Time) Values (@Value,@Time)";
        //                        //    cmd.Parameters.Add(timer.ParametrValue);
        //                        //    cmd.Parameters.Add(parametrDateTime);
        //                        //    cmd.ExecuteNonQuery();
        //                        //}
        //                        //else if (timer.ItemNet.TypeValue == "float")
        //                        //{
        //                        //    timer.ParametrValue.ParameterName = "@Value";
        //                        //    timer.ParametrValue.Value = timer.ItemNet.Value;
        //                        //    timer.ParametrValue.DbType = DbType.Single;

        //                        //    parametrDateTime.Value = DateTime.Now;

        //                        //    cmd.Parameters.Clear();
        //                        //    cmd.CommandText = "Insert into " + timer.ItemNet.TableName + "(Value,Time) Values (@Value,@Time)";
        //                        //    cmd.Parameters.Add(timer.ParametrValue);
        //                        //    cmd.Parameters.Add(parametrDateTime);
        //                        //    cmd.ExecuteNonQuery();
        //                        //}
        //                        //else if (timer.ItemNet.TypeValue == "double")
        //                        //{
        //                        //    timer.ParametrValue.ParameterName = "@Value";
        //                        //    timer.ParametrValue.Value = timer.ItemNet.Value;
        //                        //    timer.ParametrValue.DbType = DbType.Double;

        //                        //    parametrDateTime.Value = DateTime.Now;

        //                        //    cmd.Parameters.Clear();
        //                        //    cmd.CommandText = "Insert into " + timer.ItemNet.TableName + "(Value,Time) Values (@Value,@Time)";
        //                        //    cmd.Parameters.Add(timer.ParametrValue);
        //                        //    cmd.Parameters.Add(parametrDateTime);
        //                        //    cmd.ExecuteNonQuery();
        //                        //}
        //                        //else if (timer.ItemNet.TypeValue == "char")
        //                        //{
        //                        //    timer.ParametrValue.ParameterName = "@Value";
        //                        //    timer.ParametrValue.Value = timer.ItemNet.Value;
        //                        //    timer.ParametrValue.DbType = DbType.StringFixedLength;

        //                        //    parametrDateTime.Value = DateTime.Now;

        //                        //    cmd.Parameters.Clear();
        //                        //    cmd.CommandText = "Insert into " + timer.ItemNet.TableName + "(Value,Time) Values (@Value,@Time)";
        //                        //    cmd.Parameters.Add(timer.ParametrValue);
        //                        //    cmd.Parameters.Add(parametrDateTime);
        //                        //    cmd.ExecuteNonQuery();
        //                        //}
        //                        //else if (timer.ItemNet.TypeValue == "string")
        //                        //{
        //                        //    timer.ParametrValue.ParameterName = "@Value";
        //                        //    timer.ParametrValue.Value = timer.ItemNet.Value;
        //                        //    timer.ParametrValue.DbType = DbType.String;

        //                        //    parametrDateTime.Value = DateTime.Now;

        //                        //    cmd.Parameters.Clear();
        //                        //    cmd.CommandText = "Insert into " + timer.ItemNet.TableName + "(Value,Time) Values (@Value,@Time)";
        //                        //    cmd.Parameters.Add(timer.ParametrValue);
        //                        //    cmd.Parameters.Add(parametrDateTime);
        //                        //    cmd.ExecuteNonQuery();
        //                        //}
        //                    }
        //                }
        //            }

        //            Thread.Sleep(20);
        //        }
        //    }
        //    catch (SystemException ex)
        //    {
        //        if (ex is SqlException)
        //        {
        //            SqlException sqlex = ex as SqlException;

        //            foreach (SqlError er in sqlex.Errors)
        //            {
        //                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
        //                {
        //                    if (CollectionMessage.Count > 300)
        //                    {
        //                        CollectionMessage.RemoveAt(0);

        //                        CollectionMessage.Insert(298, "Сообщение " + " : " + "Ошибка SQL: " + er.Message + ". Данные не будут сохраняться в БД с IP: " + ethernetSendTread.EthernetSer.IPAddressServer[0] + "." + ethernetSendTread.EthernetSer.IPAddressServer[1]
        //                           + "." + ethernetSendTread.EthernetSer.IPAddressServer[2] +
        //                                        "." + ethernetSendTread.EthernetSer.IPAddressServer[3] + " " + DateTime.Now);
        //                    }
        //                    else
        //                    {
        //                        CollectionMessage.Add("Сообщение " + " : " + "Ошибка SQL: " + er.Message + ". Данные не будут сохраняться в БД с IP: " + ethernetSendTread.EthernetSer.IPAddressServer[0] + "." + ethernetSendTread.EthernetSer.IPAddressServer[1]
        //                           + "." + ethernetSendTread.EthernetSer.IPAddressServer[2] +
        //                                        "." + ethernetSendTread.EthernetSer.IPAddressServer[3] + " " + DateTime.Now);
        //                    }
        //                }));
        //            }
        //        }
        //        else
        //        {
        //            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
        //            {
        //                if (CollectionMessage.Count > 300)
        //                {
        //                    CollectionMessage.RemoveAt(0);

        //                    CollectionMessage.Insert(298, "Сообщение " + " : " + "Ошибка SQL: " + ex.Message + ". Данные не будут сохраняться в БД с IP: " + ethernetSendTread.EthernetSer.IPAddressServer[0] + "." + ethernetSendTread.EthernetSer.IPAddressServer[1]
        //                       + "." + ethernetSendTread.EthernetSer.IPAddressServer[2] +
        //                                    "." + ethernetSendTread.EthernetSer.IPAddressServer[3] + " " + DateTime.Now);
        //                }
        //                else
        //                {
        //                    CollectionMessage.Add("Сообщение " + " : " + "Ошибка SQL: " + ex.Message + ". Данные не будут сохраняться в БД с IP: " + ethernetSendTread.EthernetSer.IPAddressServer[0] + "." + ethernetSendTread.EthernetSer.IPAddressServer[1]
        //                       + "." + ethernetSendTread.EthernetSer.IPAddressServer[2] +
        //                                    "." + ethernetSendTread.EthernetSer.IPAddressServer[3] + " " + DateTime.Now);
        //                }
        //            }));
        //        }
        //    }
        //    finally
        //    {
        //        SqlConn.Close();
        //        SqlConn.Dispose();
        //    }
        //} 

        private void ConnectedTCP(object obj)
        {
            EthernetObject ethernetObject = (EthernetObject)obj;

            try
            {                   
                string IPAddressServer = ethernetObject.EthernetSer.IPAddressServer[0] + "." + ethernetObject.EthernetSer.IPAddressServer[1] + "." + ethernetObject.EthernetSer.IPAddressServer[2] + "." + ethernetObject.EthernetSer.IPAddressServer[3];

                Stopwatch timeCheckLink = new Stopwatch();          
                Stopwatch timePeriod = new Stopwatch();

                IPEndPoint localPoint = new IPEndPoint(new IPAddress(ethernetObject.EthernetSer.IPAddressClient), ethernetObject.EthernetSer.PortClient);

                byte[] bRead = new byte[ethernetObject.EthernetSer.BufferSizeRec];
                byte[] bWrite = new byte[ethernetObject.EthernetSer.BufferSizeSend];

                int[] aDecimal = new int[3];

                byte[] formulaBuff;

                NetworkStream stream = null;

                int countLinkError = 0;

                while (true)
                {
                    if (Interlocked.CompareExchange(ref IsStop, 0, 0) == 0)
                    {
                        try
                        {
                            if (ethernetObject.IsReconnect)
                            {
                                ethernetObject.IsReconnect = false;

                                if (countLinkError == 3)
                                {
                                    countLinkError = 0;

                                    if (ethernetObject.EthernetSer.CollectionItemNetSend.Count > 0)
                                    {
                                        foreach (ItemNet item in ethernetObject.EthernetSer.CollectionItemNetSend)
                                        {
                                            item.Value = "Потеря связи";
                                        }
                                    }

                                    foreach (ItemNet item in ethernetObject.EthernetSer.CollectionItemNetRec)
                                    {
                                        item.Value = "Потеря связи";
                                    }

                                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                    {
                                        if (CollectionMessage.Count > 300)
                                        {
                                            CollectionMessage.RemoveAt(0);

                                            CollectionMessage.Insert(298, "Сообщение " + " : " + "Потеря связи c IP: " + ethernetObject.EthernetSer.IPAddressServer[0]
                                            + "." + ethernetObject.EthernetSer.IPAddressServer[1] + "." + ethernetObject.EthernetSer.IPAddressServer[2] +
                                            "." + ethernetObject.EthernetSer.IPAddressServer[3] + ", " + "повторная попытка соединения. " + DateTime.Now);
                                        }
                                        else
                                        {
                                            CollectionMessage.Add("Сообщение " + " : " + "Потеря связи c IP: " + ethernetObject.EthernetSer.IPAddressServer[0]
                                            + "." + ethernetObject.EthernetSer.IPAddressServer[1] + "." + ethernetObject.EthernetSer.IPAddressServer[2] +
                                            "." + ethernetObject.EthernetSer.IPAddressServer[3] + ", " + "повторная попытка соединения. " + DateTime.Now);
                                        }
                                    }));
                                }                                                                                                                                                                                                                                                           
                            }
                            
                            ethernetObject.TcpClient = new TcpClient(localPoint);
                            ethernetObject.TcpClient.ReceiveTimeout = ethernetObject.EthernetSer.Time * 2000;
                            ethernetObject.TcpClient.SendTimeout = ethernetObject.EthernetSer.Time * 2000;
                            ethernetObject.TcpClient.BeginConnect(IPAddress.Parse(IPAddressServer), ethernetObject.EthernetSer.PortServer, null, null);

                            while (true)
                            {
                                timeCheckLink.Start();

                                if ((ethernetObject.EthernetSer.Time * 1100) <= timeCheckLink.ElapsedMilliseconds)
                                {                                      
                                    throw new Exception("Не удалось подключится к " + IPAddressServer + " .");
                                }

                                if (ethernetObject.TcpClient.Connected)
                                {
                                    timeCheckLink.Reset();
                                    break;
                                }

                                if (Interlocked.CompareExchange(ref IsStop, 1, 1) == 1)
                                {
                                    return;
                                }

                                Thread.Sleep(StaticValues.TimeSleep);
                            }

                            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            {
                                if (CollectionMessage.Count > 300)
                                {
                                    CollectionMessage.RemoveAt(0);

                                    CollectionMessage.Insert(298, "Сообщение " + " : " + "Статус: соединение с сервером " + IPAddressServer + " выполнено." + " " + DateTime.Now);
                                }
                                else
                                {
                                    CollectionMessage.Add("Сообщение " + " : " + "Статус: соединение с сервером " + IPAddressServer + " выполнено." + " " + DateTime.Now);
                                }
                            }));

                            stream = ethernetObject.TcpClient.GetStream();

                            while (true)
                            {
                                timePeriod.Start();
                                timeCheckLink.Start();

                                if (ethernetObject.EthernetSer.CollectionItemNetSend.Count > 0)
                                {
                                    foreach (ItemNet item in ethernetObject.EthernetSer.CollectionItemNetSend)
                                    {
                                        if (item.TypeValue == "float")
                                        {
                                            formulaBuff = item.Formula(ethernetObject.EthernetSer.Time);

                                            if (formulaBuff != null)
                                            {
                                                bWrite[item.Range0] = formulaBuff[0];
                                                bWrite[item.Range0 + 1] = formulaBuff[1];
                                                bWrite[item.Range0 + 2] = formulaBuff[2];
                                                bWrite[item.Range0 + 3] = formulaBuff[3];

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToSingle(bWrite, item.Range0);
                                                }
                                            }                                                                                                                                           
                                        }
                                        else if (item.TypeValue == "double")
                                        {
                                            lock (ethernetObject.LockValue)
                                            {
                                                item.Value = BitConverter.ToDouble(item.Formula(ethernetObject.EthernetSer.Time), 0);
                                            }
                                        }
                                        else if (item.TypeValue == "decimal")
                                        {
                                            aDecimal[0] = BitConverter.ToInt32(bRead, item.Range0);
                                            aDecimal[1] = BitConverter.ToInt32(bRead, item.Range0 + 4);
                                            aDecimal[2] = BitConverter.ToInt32(bRead, item.Range0 + 8);

                                            lock (ethernetObject.LockValue)
                                            {
                                                item.Value = new Decimal(aDecimal);
                                            }
                                        }
                                        else if (item.TypeValue == "byte")
                                        {
                                            formulaBuff = item.Formula(ethernetObject.EthernetSer.Time);

                                            if (formulaBuff != null)
                                            {
                                                bWrite[item.Range0] = formulaBuff[0];

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = bWrite[item.Range0];
                                                }
                                            }                                               
                                        }
                                        else if (item.TypeValue == "sbyte")
                                        {
                                            formulaBuff = item.Formula(ethernetObject.EthernetSer.Time);

                                            if (formulaBuff != null)
                                            {
                                                bWrite[item.Range0] = formulaBuff[0];

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = (sbyte)bRead[item.Range0];
                                                }
                                            }                                                                                                
                                        }
                                        else if (item.TypeValue == "short")
                                        {
                                            formulaBuff = item.Formula(ethernetObject.EthernetSer.Time);

                                            if (formulaBuff != null)
                                            {
                                                bWrite[item.Range0] = formulaBuff[0];
                                                bWrite[item.Range0 + 1] = formulaBuff[1];

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToInt16(bWrite, item.Range0);
                                                }
                                            }                                                
                                        }
                                        else if (item.TypeValue == "ushort")
                                        {
                                            formulaBuff = item.Formula(ethernetObject.EthernetSer.Time);

                                            if (formulaBuff != null)
                                            {
                                                bWrite[item.Range0] = formulaBuff[0];
                                                bWrite[item.Range0 + 1] = formulaBuff[1];

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToUInt16(bWrite, item.Range0);
                                                }
                                            }                                                                                                   
                                        }
                                        else if (item.TypeValue == "int")
                                        {
                                            formulaBuff = item.Formula(ethernetObject.EthernetSer.Time);

                                            if (formulaBuff != null)
                                            {
                                                bWrite[item.Range0] = formulaBuff[0];
                                                bWrite[item.Range0 + 1] = formulaBuff[1];
                                                bWrite[item.Range0 + 2] = formulaBuff[2];
                                                bWrite[item.Range0 + 3] = formulaBuff[3];

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToInt32(bWrite, item.Range0);
                                                }
                                            }                                                
                                        }
                                        else if (item.TypeValue == "uint")
                                        {
                                            formulaBuff = item.Formula(ethernetObject.EthernetSer.Time);

                                            if (formulaBuff != null)
                                            {
                                                bWrite[item.Range0] = formulaBuff[0];
                                                bWrite[item.Range0 + 1] = formulaBuff[1];
                                                bWrite[item.Range0 + 2] = formulaBuff[2];
                                                bWrite[item.Range0 + 3] = formulaBuff[3];

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToUInt32(bWrite, item.Range0);
                                                }
                                            }                                                
                                        }
                                        else if (item.TypeValue == "long")
                                        {
                                            lock (ethernetObject.LockValue)
                                            {
                                                item.Value = BitConverter.ToInt64(bWrite, item.Range0);
                                            }                                                    
                                        }
                                        else if (item.TypeValue == "ulong")
                                        {
                                            lock (ethernetObject.LockValue)
                                            {
                                                item.Value = BitConverter.ToUInt64(bWrite, item.Range0);
                                            }
                                        }
                                        else if (item.TypeValue == "bool")
                                        {
                                            lock (ethernetObject.LockValue)
                                            {
                                                item.Value = BitConverter.ToBoolean(bWrite, item.Range0);
                                            }
                                        }
                                        else if (item.TypeValue == "char")
                                        {
                                            lock (ethernetObject.LockValue)
                                            {
                                                item.Value = BitConverter.ToChar(bWrite, item.Range0);
                                            }
                                        }
                                        else if (item.TypeValue == "string")
                                        {
                                            lock (ethernetObject.LockValue)
                                            {
                                                item.Value = BitConverter.ToString(bWrite, item.Range0);
                                            }
                                        }
                                    }

                                    stream.Write(bWrite, 0, bWrite.Length);
                                }

                                while (true)
                                {
                                    if (Interlocked.CompareExchange(ref IsStop, 1, 1) == 1)
                                    {
                                        return;
                                    }

                                    if((ethernetObject.EthernetSer.Time * 1100) <= timeCheckLink.ElapsedMilliseconds)
                                    {
                                        throw new Exception("Потеря связи с " + IPAddressServer + " .");
                                    }

                                    if (ethernetObject.TcpClient.Available == ethernetObject.EthernetSer.BufferSizeRec)
                                    {
                                        timeCheckLink.Reset();

                                        stream.Read(bRead, 0, bRead.Length);

                                        foreach (ItemNet item in ethernetObject.EthernetSer.CollectionItemNetRec)
                                        {
                                            if (item.TypeValue == "float")
                                            {
                                                if (item.FormulaText.Length > 0)
                                                {

                                                }

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToSingle(bRead, item.Range0);
                                                }                                                     
                                            }
                                            else if (item.TypeValue == "double")
                                            {
                                                if (item.FormulaText.Length > 0)
                                                {

                                                }

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToDouble(bRead, item.Range0);
                                                }
                                            }
                                            else if (item.TypeValue == "decimal")
                                            {
                                                aDecimal[0] = BitConverter.ToInt32(bRead, item.Range0);
                                                aDecimal[1] = BitConverter.ToInt32(bRead, item.Range0 + 4);
                                                aDecimal[2] = BitConverter.ToInt32(bRead, item.Range0 + 8);

                                                if (item.FormulaText.Length > 0)
                                                {

                                                }

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = new Decimal(aDecimal);
                                                }
                                            }
                                            else if (item.TypeValue == "byte")
                                            {
                                                if (item.FormulaText.Length > 0)
                                                {

                                                }

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = bRead[item.Range0];
                                                }
                                            }
                                            else if (item.TypeValue == "sbyte")
                                            {
                                                if (item.FormulaText.Length > 0)
                                                {

                                                }

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = (sbyte)bRead[item.Range0];
                                                }                                                        
                                            }
                                            else if (item.TypeValue == "short")
                                            {
                                                if (item.FormulaText.Length > 0)
                                                {

                                                }

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToInt16(bRead, item.Range0);
                                                }
                                            }
                                            else if (item.TypeValue == "ushort")
                                            {
                                                if (item.FormulaText.Length > 0)
                                                {

                                                }

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToUInt16(bRead, item.Range0);
                                                }
                                            }
                                            else if (item.TypeValue == "int")
                                            {
                                                if (item.FormulaText.Length > 0)
                                                {

                                                }

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToInt32(bRead, item.Range0);
                                                }
                                            }
                                            else if (item.TypeValue == "uint")
                                            {
                                                if (item.FormulaText.Length > 0)
                                                {

                                                }

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToUInt32(bRead, item.Range0);
                                                }
                                            }
                                            else if (item.TypeValue == "long")
                                            {
                                                if (item.FormulaText.Length > 0)
                                                {

                                                }

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToInt64(bRead, item.Range0);
                                                }
                                            }
                                            else if (item.TypeValue == "ulong")
                                            {
                                                if (item.FormulaText.Length > 0)
                                                {

                                                }

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToUInt64(bRead, item.Range0);
                                                }
                                            }
                                            else if (item.TypeValue == "bool")
                                            {
                                                if (item.FormulaText.Length > 0)
                                                {

                                                }

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToBoolean(bRead, item.Range0);
                                                }
                                            }
                                            else if (item.TypeValue == "char")
                                            {
                                                if (item.FormulaText.Length > 0)
                                                {

                                                }

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToChar(bRead, item.Range0);
                                                }
                                            }
                                            else if (item.TypeValue == "string")
                                            {
                                                if (item.FormulaText.Length > 0)
                                                {

                                                }

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToString(bRead, item.Range0);
                                                }
                                            }
                                        }

                                        lock(ethernetObject.LockBool)
                                        {
                                            ethernetObject.IsAvailableData = true;
                                        }

                                        break;
                                    }

                                    Thread.Sleep(StaticValues.TimeSleep);
                                }

                                while (true)
                                {
                                    if (Interlocked.CompareExchange(ref IsStop, 1, 1) == 1)
                                    {
                                        return;
                                    }

                                    if ((ethernetObject.EthernetSer.Time * 1000) <= timePeriod.ElapsedMilliseconds)
                                    {
                                        timePeriod.Reset();
                                        break;
                                    }

                                    Thread.Sleep(StaticValues.TimeSleep);
                                }
                            }                              
                        }
                        catch (Exception ex)
                        {
                            if (Interlocked.CompareExchange(ref IsStop, 0, 0) == 0)
                            {
                                ethernetObject.IsReconnect = true;
                                countLinkError++;
                                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                {
                                    if (CollectionMessage.Count > 300)
                                    {
                                        CollectionMessage.RemoveAt(0);

                                        CollectionMessage.Insert(298, "Сообщение " + " : " + "Исключение в опросе TCP/IP " + IPAddressServer + ": " + ex.Message + " " + DateTime.Now);
                                    }
                                    else
                                    {
                                        CollectionMessage.Add("Сообщение " + " : " + "Исключение в опросе TCP/IP " + IPAddressServer + ": " + ex.Message + " " + DateTime.Now);
                                    }
                                }));
                            }
                        }
                        finally
                        {
                            if (stream != null)
                            {
                                stream.Close();
                            }

                            if (ethernetObject.TcpClient != null)
                            {
                                ethernetObject.TcpClient.Close();
                            }                            

                            lock(ethernetObject.LockBool)
                            {
                                ethernetObject.IsAvailableData = false;
                            }                                                        
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    if (CollectionMessage.Count > 300)
                    {
                        CollectionMessage.RemoveAt(0);

                        CollectionMessage.Insert(298, "Сообщение " + " : " + "Аварийный выход из потока " + ethernetObject.EthernetSer.IPAddressServer[0]
                                                + "." + ethernetObject.EthernetSer.IPAddressServer[1] + "." + ethernetObject.EthernetSer.IPAddressServer[2] +
                                                "." + ethernetObject.EthernetSer.IPAddressServer[3] + ex.Message + " " + DateTime.Now);
                    }
                    else
                    {
                        CollectionMessage.Add("Сообщение " + " : " + "Аварийный выход из потока " + ethernetObject.EthernetSer.IPAddressServer[0]
                                                + "." + ethernetObject.EthernetSer.IPAddressServer[1] + "." + ethernetObject.EthernetSer.IPAddressServer[2] +
                                                "." + ethernetObject.EthernetSer.IPAddressServer[3] + ex.Message + " " + DateTime.Now);
                    }
                }));
            }
            finally
            {
                foreach (ItemNet item in ethernetObject.EthernetSer.CollectionItemNetSend)
                {
                    item.ItemModbus = null;
                }                                            
            }
        }

        private void ConnectedUDP(object obj)
        {
            EthernetObject ethernetObject = (EthernetObject)obj;

            try
            {
                int countLinkError = 0;

                string IPAddressServer = ethernetObject.EthernetSer.IPAddressServer[0] + "." + ethernetObject.EthernetSer.IPAddressServer[1] + "." + ethernetObject.EthernetSer.IPAddressServer[2] + "." + ethernetObject.EthernetSer.IPAddressServer[3];

                Stopwatch timeCheckLink = new Stopwatch();
                Stopwatch timePeriod = new Stopwatch();

                IPEndPoint localPoint = new IPEndPoint(new IPAddress(ethernetObject.EthernetSer.IPAddressClient), ethernetObject.EthernetSer.PortClient);

                IPEndPoint localIP = new IPEndPoint(new IPAddress(ethernetObject.EthernetSer.IPAddressClient), ethernetObject.EthernetSer.PortClient);
                IPEndPoint remoteIP = new IPEndPoint(IPAddress.Parse(ethernetObject.EthernetSer.IPAddressServer[0] + "." + ethernetObject.EthernetSer.IPAddressServer[1] + "." + ethernetObject.EthernetSer.IPAddressServer[2] + "." + ethernetObject.EthernetSer.IPAddressServer[3]), ethernetObject.EthernetSer.PortServer);
                IPEndPoint remoteIPRec = null;

                byte[] bRead = new byte[ethernetObject.EthernetSer.BufferSizeRec];
                byte[] bWrite = new byte[ethernetObject.EthernetSer.BufferSizeSend];

                int[] aDecimal = new int[3];

                byte[] formulaBuff;

                while (true)
                {
                    if (Interlocked.CompareExchange(ref IsStop, 0, 0) == 0)
                    {
                        try
                        {
                            if (ethernetObject.IsReconnect)
                            {
                                ethernetObject.IsReconnect = false;

                                if (countLinkError == 3)
                                {
                                    countLinkError = 0;

                                    if (ethernetObject.EthernetSer.CollectionItemNetSend.Count > 0)
                                    {
                                        foreach (ItemNet item in ethernetObject.EthernetSer.CollectionItemNetSend)
                                        {
                                            item.Value = "Потеря связи";
                                        }
                                    }

                                    foreach (ItemNet item in ethernetObject.EthernetSer.CollectionItemNetRec)
                                    {
                                        item.Value = "Потеря связи";
                                    }

                                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                    {
                                        if (CollectionMessage.Count > 300)
                                        {
                                            CollectionMessage.RemoveAt(0);

                                            CollectionMessage.Insert(298, "Сообщение " + " : " + "Потеря связи c UDP/IP: " + ethernetObject.EthernetSer.IPAddressServer[0]
                                            + "." + ethernetObject.EthernetSer.IPAddressServer[1] + "." + ethernetObject.EthernetSer.IPAddressServer[2] +
                                            "." + ethernetObject.EthernetSer.IPAddressServer[3] + " " + DateTime.Now);
                                        }
                                        else
                                        {
                                            CollectionMessage.Add("Сообщение " + " : " + "Потеря связи c UDP/IP: " + ethernetObject.EthernetSer.IPAddressServer[0]
                                            + "." + ethernetObject.EthernetSer.IPAddressServer[1] + "." + ethernetObject.EthernetSer.IPAddressServer[2] +
                                            "." + ethernetObject.EthernetSer.IPAddressServer[3] + " " + DateTime.Now);
                                        }
                                    }));
                                }
                            }
                       
                            ethernetObject.UdpClient = new UdpClient(localIP);
                            ethernetObject.UdpClient.Client.ReceiveTimeout = ethernetObject.EthernetSer.Time * 2000;
                            ethernetObject.UdpClient.Client.SendTimeout = ethernetObject.EthernetSer.Time * 2000;

                            while (true)
                            {
                                timePeriod.Start();
                                timeCheckLink.Start();

                                if (ethernetObject.EthernetSer.CollectionItemNetSend.Count > 0)
                                {
                                    foreach (ItemNet item in ethernetObject.EthernetSer.CollectionItemNetSend)
                                    {
                                        if (item.TypeValue == "float")
                                        {
                                            formulaBuff = item.Formula(ethernetObject.EthernetSer.Time);

                                            if (formulaBuff != null)
                                            {
                                                bWrite[item.Range0] = formulaBuff[0];
                                                bWrite[item.Range0 + 1] = formulaBuff[1];
                                                bWrite[item.Range0 + 2] = formulaBuff[2];
                                                bWrite[item.Range0 + 3] = formulaBuff[3];

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToSingle(bWrite, item.Range0);
                                                }
                                            }
                                        }
                                        else if (item.TypeValue == "double")
                                        {
                                            lock (ethernetObject.LockValue)
                                            {
                                                item.Value = BitConverter.ToDouble(item.Formula(ethernetObject.EthernetSer.Time), 0);
                                            }
                                        }
                                        else if (item.TypeValue == "decimal")
                                        {
                                            aDecimal[0] = BitConverter.ToInt32(bRead, item.Range0);
                                            aDecimal[1] = BitConverter.ToInt32(bRead, item.Range0 + 4);
                                            aDecimal[2] = BitConverter.ToInt32(bRead, item.Range0 + 8);

                                            lock (ethernetObject.LockValue)
                                            {
                                                item.Value = new Decimal(aDecimal);
                                            }
                                        }
                                        else if (item.TypeValue == "byte")
                                        {
                                            formulaBuff = item.Formula(ethernetObject.EthernetSer.Time);

                                            if (formulaBuff != null)
                                            {
                                                bWrite[item.Range0] = formulaBuff[0];

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = bWrite[item.Range0];
                                                }
                                            }
                                        }
                                        else if (item.TypeValue == "sbyte")
                                        {
                                            formulaBuff = item.Formula(ethernetObject.EthernetSer.Time);

                                            if (formulaBuff != null)
                                            {
                                                bWrite[item.Range0] = formulaBuff[0];

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = (sbyte)bRead[item.Range0];
                                                }
                                            }
                                        }
                                        else if (item.TypeValue == "short")
                                        {
                                            formulaBuff = item.Formula(ethernetObject.EthernetSer.Time);

                                            if (formulaBuff != null)
                                            {
                                                bWrite[item.Range0] = formulaBuff[0];
                                                bWrite[item.Range0 + 1] = formulaBuff[1];

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToInt16(bWrite, item.Range0);
                                                }
                                            }
                                        }
                                        else if (item.TypeValue == "ushort")
                                        {
                                            formulaBuff = item.Formula(ethernetObject.EthernetSer.Time);

                                            if (formulaBuff != null)
                                            {
                                                bWrite[item.Range0] = formulaBuff[0];
                                                bWrite[item.Range0 + 1] = formulaBuff[1];

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToUInt16(bWrite, item.Range0);
                                                }
                                            }
                                        }
                                        else if (item.TypeValue == "int")
                                        {
                                            formulaBuff = item.Formula(ethernetObject.EthernetSer.Time);

                                            if (formulaBuff != null)
                                            {
                                                bWrite[item.Range0] = formulaBuff[0];
                                                bWrite[item.Range0 + 1] = formulaBuff[1];
                                                bWrite[item.Range0 + 2] = formulaBuff[2];
                                                bWrite[item.Range0 + 3] = formulaBuff[3];

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToInt32(bWrite, item.Range0);
                                                }
                                            }
                                        }
                                        else if (item.TypeValue == "uint")
                                        {
                                            formulaBuff = item.Formula(ethernetObject.EthernetSer.Time);

                                            if (formulaBuff != null)
                                            {
                                                bWrite[item.Range0] = formulaBuff[0];
                                                bWrite[item.Range0 + 1] = formulaBuff[1];
                                                bWrite[item.Range0 + 2] = formulaBuff[2];
                                                bWrite[item.Range0 + 3] = formulaBuff[3];

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToUInt32(bWrite, item.Range0);
                                                }
                                            }
                                        }
                                        else if (item.TypeValue == "long")
                                        {
                                            lock (ethernetObject.LockValue)
                                            {
                                                item.Value = BitConverter.ToInt64(bWrite, item.Range0);
                                            }
                                        }
                                        else if (item.TypeValue == "ulong")
                                        {
                                            lock (ethernetObject.LockValue)
                                            {
                                                item.Value = BitConverter.ToUInt64(bWrite, item.Range0);
                                            }
                                        }
                                        else if (item.TypeValue == "bool")
                                        {
                                            lock (ethernetObject.LockValue)
                                            {
                                                item.Value = BitConverter.ToBoolean(bWrite, item.Range0);
                                            }
                                        }
                                        else if (item.TypeValue == "char")
                                        {
                                            lock (ethernetObject.LockValue)
                                            {
                                                item.Value = BitConverter.ToChar(bWrite, item.Range0);
                                            }
                                        }
                                        else if (item.TypeValue == "string")
                                        {
                                            lock (ethernetObject.LockValue)
                                            {
                                                item.Value = BitConverter.ToString(bWrite, item.Range0);
                                            }
                                        }
                                    }

                                    ethernetObject.UdpClient.Send(bWrite, bWrite.Length, remoteIP);
                                }

                                while (true)
                                {
                                    if (Interlocked.CompareExchange(ref IsStop, 1, 1) == 1)
                                    {
                                        return;
                                    }

                                    if ((ethernetObject.EthernetSer.Time * 1100) <= timeCheckLink.ElapsedMilliseconds)
                                    {
                                        throw new Exception("Потеря связи с " + IPAddressServer + " .");
                                    }
                                   
                                    if (ethernetObject.UdpClient.Available == ethernetObject.EthernetSer.BufferSizeRec)
                                    {
                                        timeCheckLink.Reset();

                                        ethernetObject.UdpClient.Receive(ref remoteIPRec);

                                        foreach (ItemNet item in ethernetObject.EthernetSer.CollectionItemNetRec)
                                        {
                                            if (item.TypeValue == "float")
                                            {
                                                if (item.FormulaText.Length > 0)
                                                {

                                                }

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToSingle(bRead, item.Range0);
                                                }                                                  
                                            }
                                            else if (item.TypeValue == "double")
                                            {
                                                if (item.FormulaText.Length > 0)
                                                {

                                                }

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToDouble(bRead, item.Range0);
                                                }                                                   
                                            }
                                            else if (item.TypeValue == "decimal")
                                            {
                                                aDecimal[0] = BitConverter.ToInt32(bRead, item.Range0);
                                                aDecimal[1] = BitConverter.ToInt32(bRead, item.Range0 + 4);
                                                aDecimal[2] = BitConverter.ToInt32(bRead, item.Range0 + 8);

                                                if (item.FormulaText.Length > 0)
                                                {

                                                }

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = new Decimal(aDecimal);
                                                }                                                 
                                            }
                                            else if (item.TypeValue == "byte")
                                            {
                                                if (item.FormulaText.Length > 0)
                                                {

                                                }

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = bRead[item.Range0];
                                                }                                                   
                                            }
                                            else if (item.TypeValue == "sbyte")
                                            {
                                                if (item.FormulaText.Length > 0)
                                                {

                                                }

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = (sbyte)bRead[item.Range0];
                                                }                                                  
                                            }
                                            else if (item.TypeValue == "short")
                                            {
                                                if (item.FormulaText.Length > 0)
                                                {

                                                }

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToInt16(bRead, item.Range0);
                                                }
                                            }
                                            else if (item.TypeValue == "ushort")
                                            {
                                                if (item.FormulaText.Length > 0)
                                                {

                                                }

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToUInt16(bRead, item.Range0);
                                                }                                                  
                                            }
                                            else if (item.TypeValue == "int")
                                            {
                                                if (item.FormulaText.Length > 0)
                                                {

                                                }

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToInt32(bRead, item.Range0);
                                                }                                                  
                                            }
                                            else if (item.TypeValue == "uint")
                                            {
                                                if (item.FormulaText.Length > 0)
                                                {

                                                }

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToUInt32(bRead, item.Range0);
                                                }                                                   
                                            }
                                            else if (item.TypeValue == "long")
                                            {
                                                if (item.FormulaText.Length > 0)
                                                {

                                                }

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToInt64(bRead, item.Range0);
                                                }                                                   
                                            }
                                            else if (item.TypeValue == "ulong")
                                            {
                                                if (item.FormulaText.Length > 0)
                                                {

                                                }

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToUInt64(bRead, item.Range0);
                                                }                                                   
                                            }
                                            else if (item.TypeValue == "bool")
                                            {
                                                if (item.FormulaText.Length > 0)
                                                {

                                                }

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToBoolean(bRead, item.Range0);
                                                }                                                  
                                            }
                                            else if (item.TypeValue == "char")
                                            {
                                                if (item.FormulaText.Length > 0)
                                                {

                                                }

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToChar(bRead, item.Range0);
                                                }                                                   
                                            }
                                            else if (item.TypeValue == "string")
                                            {
                                                if (item.FormulaText.Length > 0)
                                                {

                                                }

                                                lock (ethernetObject.LockValue)
                                                {
                                                    item.Value = BitConverter.ToString(bRead, item.Range0);
                                                }                                                  
                                            }
                                        }

                                        lock (ethernetObject.LockBool)
                                        {
                                            ethernetObject.IsAvailableData = false;
                                        }

                                        break;
                                    }

                                    Thread.Sleep(StaticValues.TimeSleep);
                                }

                                while (true)
                                {
                                    if (Interlocked.CompareExchange(ref IsStop, 1, 1) == 1)
                                    {
                                        return;
                                    }

                                    if ((ethernetObject.EthernetSer.Time * 1000) <= timePeriod.ElapsedMilliseconds)
                                    {
                                        timePeriod.Reset();
                                        break;
                                    }

                                    Thread.Sleep(StaticValues.TimeSleep);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            
                            if (Interlocked.CompareExchange(ref IsStop, 0, 0) == 0)
                            {
                                countLinkError++;

                                ethernetObject.IsReconnect = true;

                                lock (ethernetObject.LockBool)
                                {
                                    ethernetObject.IsAvailableData = false;
                                }

                                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                {
                                    if (CollectionMessage.Count > 300)
                                    {
                                        CollectionMessage.RemoveAt(0);

                                        CollectionMessage.Insert(298, "Сообщение " + " : " + "Исключение в опросе UDP/IP " + IPAddressServer + ": " + ex.Message + " " + DateTime.Now);
                                    }
                                    else
                                    {
                                        CollectionMessage.Add("Сообщение " + " : " + "Исключение в опросе UDP/IP " + IPAddressServer + ": " + ex.Message + " " + DateTime.Now);
                                    }
                                }));
                            }
                        }
                        finally
                        {
                            lock (ethernetObject.LockBool)
                            {
                                ethernetObject.IsAvailableData = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    if (CollectionMessage.Count > 300)
                    {
                        CollectionMessage.RemoveAt(0);

                        CollectionMessage.Insert(298, "Сообщение " + " : " + "Аварийный выход из потока " + ethernetObject.EthernetSer.IPAddressServer[0]
                                                + "." + ethernetObject.EthernetSer.IPAddressServer[1] + "." + ethernetObject.EthernetSer.IPAddressServer[2] +
                                                "." + ethernetObject.EthernetSer.IPAddressServer[3] + ex.Message + " " + DateTime.Now);
                    }
                    else
                    {
                        CollectionMessage.Add("Сообщение " + " : " + "Аварийный выход из потока " + ethernetObject.EthernetSer.IPAddressServer[0]
                                                + "." + ethernetObject.EthernetSer.IPAddressServer[1] + "." + ethernetObject.EthernetSer.IPAddressServer[2] +
                                                "." + ethernetObject.EthernetSer.IPAddressServer[3] + ex.Message + " " + DateTime.Now);
                    }
                }));
            }
            finally
            {
                foreach (ItemNet item in ethernetObject.EthernetSer.CollectionItemNetSend)
                {
                    item.ItemModbus = null;
                }
              
                if (ethernetObject.UdpClient != null)
                {
                    ethernetObject.UdpClient.Close();
                }
            }
        }

        //private void ConnectedEthernetOperational(object obj)
        //{
        //    EthernetObject ethernetSendTread = (EthernetObject)obj;

        //    string IPAddressServer = ethernetSendTread.EthernetSer.IPAddressServer[0] + "." + ethernetSendTread.EthernetSer.IPAddressServer[1] + "." + ethernetSendTread.EthernetSer.IPAddressServer[2] + "." + ethernetSendTread.EthernetSer.IPAddressServer[3];

        //    if (!IsStop)
        //    {
        //        try
        //        {
        //            bool IsLink = false;

        //            IPEndPoint localPoint = new IPEndPoint(new IPAddress(ethernetSendTread.EthernetSer.IPAddressClient), ethernetSendTread.EthernetSer.PortClient);

        //            ethernetSendTread.TcpClient = new TcpClient(localPoint);
        //            ethernetSendTread.TcpClient.ReceiveTimeout = ethernetSendTread.EthernetSer.Time * 2000;
        //            ethernetSendTread.TcpClient.SendTimeout = 5000;

        //            byte[] bRead = new byte[ethernetSendTread.EthernetSer.BufferSizeRec + 1];
        //            byte[] bWrite = new byte[ethernetSendTread.EthernetSer.BufferSizeSend + 1];

        //            ethernetSendTread.TcpClient.Connect(IPAddress.Parse(IPAddressServer), ethernetSendTread.EthernetSer.PortServer);

        //            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
        //            {
        //                if (CollectionMessage.Count > 300)
        //                {
        //                    CollectionMessage.RemoveAt(0);

        //                    CollectionMessage.Insert(298, "Сообщение " + " : " + "Статус: соединение с сервером " + IPAddressServer + " выполнено." + " " + DateTime.Now);
        //                }
        //                else
        //                {
        //                    CollectionMessage.Add("Сообщение " + " : " + "Статус: соединение с сервером " + IPAddressServer + " выполнено." + " " + DateTime.Now);
        //                }  
        //            }));

        //            NetworkStream stream = ethernetSendTread.TcpClient.GetStream();

        //            int[] aDecimal = new int[3];

        //            Stopwatch timeCheckExit = new Stopwatch();
        //            Stopwatch timeRecconect = new Stopwatch();

        //            byte[] formulaBuff;

        //            int link = 0;
        //            bool linkDetected = false;

        //            foreach (ItemNet item in ethernetSendTread.EthernetSer.CollectionItemNetRec)
        //            {
        //                if (item.Text.IndexOf("IsLink") != -1)
        //                {
        //                    link = item.Range0;
        //                }
        //            }

        //            while (true)
        //            {
        //                stream.Read(bRead, 0, bRead.Length);

        //                if (linkDetected)
        //                {
        //                    IsLink = BitConverter.ToBoolean(bRead, link);

        //                    if (IsLink)
        //                    {
        //                        timeRecconect.Restart();
        //                        ethernetSendTread.DatabaseConnect = true;
        //                    }
        //                    else
        //                    {
        //                        ethernetSendTread.DatabaseConnect = false;

        //                        timeRecconect.Start();

        //                        if (timeRecconect.ElapsedMilliseconds >= ((ethernetSendTread.EthernetSer.Time * 1000) * 3))
        //                        {
        //                            throw new Exception("Нет приема новых данных, разрываем соединение и переподключаемся.");
        //                        }
        //                    }
        //                }

        //                foreach (ItemNet item in ethernetSendTread.EthernetSer.CollectionItemNetRec)
        //                {
        //                    if (item.TypeValue == "float")
        //                    {
        //                        item.Value = BitConverter.ToSingle(bRead, item.Range0);
        //                    }
        //                    else if (item.TypeValue == "double")
        //                    {
        //                        item.Value = BitConverter.ToDouble(bRead, item.Range0);
        //                    }
        //                    else if (item.TypeValue == "decimal")
        //                    {
        //                        aDecimal[0] = BitConverter.ToInt32(bRead, item.Range0);
        //                        aDecimal[1] = BitConverter.ToInt32(bRead, item.Range0 + 4);
        //                        aDecimal[2] = BitConverter.ToInt32(bRead, item.Range0 + 8);

        //                        item.Value = new Decimal(aDecimal);
        //                    }
        //                    else if (item.TypeValue == "byte")
        //                    {
        //                        item.Value = bRead[item.Range0];
        //                    }
        //                    else if (item.TypeValue == "sbyte")
        //                    {
        //                        item.Value = (sbyte)bRead[item.Range0];
        //                    }
        //                    else if (item.TypeValue == "short")
        //                    {
        //                        item.Value = BitConverter.ToInt16(bRead, item.Range0);
        //                    }
        //                    else if (item.TypeValue == "ushort")
        //                    {
        //                        item.Value = BitConverter.ToUInt16(bRead, item.Range0);
        //                    }
        //                    else if (item.TypeValue == "int")
        //                    {
        //                        item.Value = BitConverter.ToInt32(bRead, item.Range0);
        //                    }
        //                    else if (item.TypeValue == "uint")
        //                    {
        //                        item.Value = BitConverter.ToUInt32(bRead, item.Range0);
        //                    }
        //                    else if (item.TypeValue == "long")
        //                    {
        //                        item.Value = BitConverter.ToInt64(bRead, item.Range0);
        //                    }
        //                    else if (item.TypeValue == "ulong")
        //                    {
        //                        item.Value = BitConverter.ToUInt64(bRead, item.Range0);
        //                    }
        //                    else if (item.TypeValue == "bool")
        //                    {
        //                        formula(item, ethernetSendTread.EthernetSer.Time);

        //                        item.Value = BitConverter.ToBoolean(bRead, item.Range0);
        //                    }
        //                    else if (item.TypeValue == "char")
        //                    {
        //                        item.Value = BitConverter.ToChar(bRead, item.Range0);
        //                    }
        //                    else if (item.TypeValue == "string")
        //                    {
        //                        item.Value = BitConverter.ToString(bRead, item.Range0);
        //                    }
        //                }

        //                foreach (ItemNet item in ethernetSendTread.EthernetSer.CollectionItemNetSend)
        //                {
        //                    if (item.TypeValue == "float")
        //                    {
        //                        formulaBuff = formula(item, ethernetSendTread.EthernetSer.Time);

        //                        if (formulaBuff == null)
        //                        {
        //                            bWrite[item.Range0] = formulaBuff[0];
        //                            bWrite[item.Range0 + 1] = formulaBuff[1];
        //                            bWrite[item.Range0 + 2] = formulaBuff[2];
        //                            bWrite[item.Range0 + 3] = formulaBuff[3];
        //                        }

        //                        item.Value = BitConverter.ToSingle(bWrite, item.Range0);
        //                    }
        //                    else if (item.TypeValue == "double")
        //                    {
        //                        item.Value = BitConverter.ToDouble(formula(item, ethernetSendTread.EthernetSer.Time), 0);
        //                    }
        //                    else if (item.TypeValue == "decimal")
        //                    {
        //                        aDecimal[0] = BitConverter.ToInt32(bRead, item.Range0);
        //                        aDecimal[1] = BitConverter.ToInt32(bRead, item.Range0 + 4);
        //                        aDecimal[2] = BitConverter.ToInt32(bRead, item.Range0 + 8);

        //                        item.Value = new Decimal(aDecimal);
        //                    }
        //                    else if (item.TypeValue == "byte")
        //                    {
        //                        formulaBuff = formula(item, ethernetSendTread.EthernetSer.Time);

        //                        if (formulaBuff == null)
        //                        {
        //                            bWrite[item.Range0] = formulaBuff[0];
        //                        }

        //                        item.Value = bWrite[item.Range0];
        //                    }
        //                    else if (item.TypeValue == "sbyte")
        //                    {
        //                        formulaBuff = formula(item, ethernetSendTread.EthernetSer.Time);

        //                        if (formulaBuff == null)
        //                        {
        //                            bWrite[item.Range0] = formulaBuff[0];
        //                        }

        //                        item.Value = (sbyte)bRead[item.Range0];
        //                    }
        //                    else if (item.TypeValue == "short")
        //                    {
        //                        formulaBuff = formula(item, ethernetSendTread.EthernetSer.Time);

        //                        if (formulaBuff == null)
        //                        {
        //                            bWrite[item.Range0] = formulaBuff[0];
        //                            bWrite[item.Range0 + 1] = formulaBuff[1];
        //                        }

        //                        item.Value = BitConverter.ToInt16(bWrite, item.Range0);
        //                    }
        //                    else if (item.TypeValue == "ushort")
        //                    {
        //                        formulaBuff = formula(item, ethernetSendTread.EthernetSer.Time);

        //                        if (formulaBuff == null)
        //                        {
        //                            bWrite[item.Range0] = formulaBuff[0];
        //                            bWrite[item.Range0 + 1] = formulaBuff[1];
        //                        }

        //                        item.Value = BitConverter.ToUInt16(bWrite, item.Range0);
        //                    }
        //                    else if (item.TypeValue == "int")
        //                    {
        //                        formulaBuff = formula(item, ethernetSendTread.EthernetSer.Time);

        //                        if (formulaBuff == null)
        //                        {
        //                            bWrite[item.Range0] = formulaBuff[0];
        //                            bWrite[item.Range0 + 1] = formulaBuff[1];
        //                            bWrite[item.Range0 + 2] = formulaBuff[2];
        //                            bWrite[item.Range0 + 3] = formulaBuff[3];
        //                        }

        //                        item.Value = BitConverter.ToInt32(bWrite, item.Range0);
        //                    }
        //                    else if (item.TypeValue == "uint")
        //                    {
        //                        formulaBuff = formula(item, ethernetSendTread.EthernetSer.Time);

        //                        if (formulaBuff == null)
        //                        {
        //                            bWrite[item.Range0] = formulaBuff[0];
        //                            bWrite[item.Range0 + 1] = formulaBuff[1];
        //                            bWrite[item.Range0 + 2] = formulaBuff[2];
        //                            bWrite[item.Range0 + 3] = formulaBuff[3];
        //                        }

        //                        item.Value = BitConverter.ToUInt32(bWrite, item.Range0);
        //                    }
        //                    else if (item.TypeValue == "long")
        //                    {
        //                        item.Value = BitConverter.ToInt64(bWrite, item.Range0);
        //                    }
        //                    else if (item.TypeValue == "ulong")
        //                    {
        //                        item.Value = BitConverter.ToUInt64(bWrite, item.Range0);
        //                    }
        //                    else if (item.TypeValue == "bool")
        //                    {
        //                        item.Value = BitConverter.ToBoolean(bWrite, item.Range0);
        //                    }
        //                    else if (item.TypeValue == "char")
        //                    {
        //                        item.Value = BitConverter.ToChar(bWrite, item.Range0);
        //                    }
        //                    else if (item.TypeValue == "string")
        //                    {
        //                        item.Value = BitConverter.ToString(bWrite, item.Range0);
        //                    }
        //                }

        //                stream.Write(bWrite, 0, bWrite.Length);

        //                IsLink = false;

        //                while (true)
        //                {
        //                    Thread.Sleep(20);

        //                    if (IsStop)
        //                    {
        //                        return;
        //                    }

        //                    timeCheckExit.Start();

        //                    if ((ethernetSendTread.EthernetSer.Time * 1000) >= timeCheckExit.ElapsedMilliseconds)
        //                    {
        //                        timeCheckExit.Reset();
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //        catch (SystemException ex)
        //        {
        //            if (!IsStop)
        //            {
        //                ethernetSendTread.IsReconnect = true;

        //                //this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
        //                //{
        //                //    CountLineTextMessage++;
        //                //    WindowErrorMessages.LBMessageError.Text += "\n" + "Сообщение " + CountLineTextMessage.ToString() + " : " + "Ошибка в опросе IP " + IPAddressServer + ": " + ex.Message + " " + DateTime.Now;
        //                //}));
        //            }
        //        }
        //        finally
        //        {
        //            ethernetSendTread.DatabaseConnect = false;

        //            if (ethernetSendTread.TcpClient != null)
        //            {
        //                ethernetSendTread.TcpClient.Close();
        //            }
        //        }
        //    }
        //}

        private void BStopProject_Click(object sender, RoutedEventArgs e)
        {            
            AppWPF app = (AppWPF)Application.Current;

            Interlocked.Exchange(ref IsStop, 1);

            foreach (EthernetObject client in CollectionTCPEthernetObject)
            {
                if (client.TcpClient != null)
                {
                    client.TcpClient.Close();
                }
            }

            foreach (EthernetObject client in CollectionUDPEthernetObject)
            {
                if (client.UdpClient != null)
                {
                    client.UdpClient.Close();
                }
            }

            foreach (SerialPort serialPort in CollectionSerialPortThread)
            {
                if (serialPort != null)
                {
                    if (serialPort.IsOpen)
                    {
                        serialPort.Close();
                    }
                }
            }

            foreach (SQLObject SqlCon in CollectionSQLObject)
            {
                if (SqlCon.SQL != null)
                {
                    SqlCon.SQL.Close();
                    SqlCon.SQL.Dispose();
                }
            }

            if (CollectionMessage.Count > 300)
            {
                CollectionMessage.RemoveAt(0);

                CollectionMessage.Insert(298, "Сообщение " + " : " + "Опрос остановлен" + " " + DateTime.Now);
            }
            else
            {
                CollectionMessage.Add("Сообщение " + " : " + "Опрос остановлен" + " " + DateTime.Now);
            } 

            IsBindingStartProject = false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Stream FileStream = File.Create(((AppWPF)Application.Current).StartPath);
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(FileStream, ((AppWPF)Application.Current).ConfigProgramBin);
            FileStream.Close();

            if (this.ProjectBin != null)
            {
                using (FileStream fs = File.Create(this.ProjectBin.PathProject))
                {
                    BinaryFormatter Projserializer = new BinaryFormatter();
                    Projserializer.Serialize(fs, this.ProjectBin);
                }
            }

            Interlocked.Exchange(ref IsStop, 1);
        }

        private void MIWindowMessage_Click(object sender, RoutedEventArgs e)
        {
            if (WindowErrorMessages.IsVisible)
            {
                WindowErrorMessages.Visibility = System.Windows.Visibility.Collapsed;

                ((AppWPF)Application.Current).ConfigProgramBin.IsWindowErrorMessage = false;
            }
            else
            {
                WindowErrorMessages.Visibility = System.Windows.Visibility.Visible;

                ((AppWPF)Application.Current).ConfigProgramBin.IsWindowErrorMessage = true;
            }

            e.Handled = true;
        }

        private void BArchive_Click(object sender, RoutedEventArgs e)
        {
            WindowArchive wArchive = new WindowArchive();
            wArchive.Owner = this;
            wArchive.Show();

            e.Handled = true;
        }

        public int IsStop;

        private void About(object sender, RoutedEventArgs e)
        {
            DialogWindowAbout DialogAbout = new DialogWindowAbout();
            DialogAbout.Owner = this;
            DialogAbout.ShowDialog();

            e.Handled = true;
        }

        private void BLockProject_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (IsBindingStartProject)
            {
                e.Cancel = true;
            }           
        }

        private void LoginManager(object sender, RoutedEventArgs e)
        {
            WindowLoginManager loginManager = new WindowLoginManager();
            loginManager.Owner = this;
            loginManager.ShowDialog();

            e.Handled = true;
        }
        private void MenuItemUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WebClient client = new WebClient();

                Match m;

                string baseURL = "http://www.primscada.com/Downloads/";
                string pattern = @">setup\w*";
                string result;

                double versionParse;
                double versionNew = Version;

                m = Regex.Match(client.DownloadString(baseURL), pattern,
                                RegexOptions.IgnoreCase | RegexOptions.Compiled);
                while (m.Success)
                {
                    result = m.Value.Replace(">setup_v", "");
                    result = result.Replace("_", ".");

                    versionParse = double.Parse(result);

                    if (versionNew < versionParse)
                    {
                        versionNew = versionParse;
                    }

                    m = m.NextMatch();
                }

                if (Version < versionNew)
                {
                    WindowUpdate windowUpdate = new WindowUpdate();
                    windowUpdate.VersionNew = versionNew;
                    windowUpdate.Owner = this;
                    windowUpdate.Show();
                }
            }
            catch(SystemException ex)
            {
                if (CollectionMessage.Count > 300)
                {
                    CollectionMessage.RemoveAt(0);

                    CollectionMessage.Insert(298, "Сообщение " + " : " + ex.Message + " " + " " + DateTime.Now);
                }
                else
                {
                    CollectionMessage.Add("Сообщение " + " : " + ex.Message + " " + " " + DateTime.Now);
                }
            }
                              
            e.Handled = true;
        }

        private void SaveAll(object sender, RoutedEventArgs e)
        {
            try
            {
                int count = ((AppWPF)Application.Current).CollectionSaveTabItem.Count;
                TabItemParent tabItemSelcted = null;
                List<TabItemParent> list = ((AppWPF)Application.Current).CollectionSaveTabItem;
                ItemScada IS = null;
                object obj = null;

                while (count != 0)
                {
                    tabItemSelcted = list[count - 1];

                    if (tabItemSelcted != null)
                    {
                        IS = tabItemSelcted.IS;

                        if (tabItemSelcted.isSave)
                        {
                            ((AppWPF)Application.Current).CollectionSaveTabItem.Remove(tabItemSelcted);
                            
                            if (tabItemSelcted is TabItemPage)
                            {
                                obj = ((AppWPF)Application.Current).CollectionPage[IS.Path];
                            }
                            else if (tabItemSelcted is TabItemControlPanel)
                            {
                                obj = ((AppWPF)Application.Current).CollectionControlPanel[IS.Path];
                            }

                            using (FileStream fs = File.Create((IS.Path)))
                            {
                                XamlWriter.Save(obj, fs);
                            }

                            tabItemSelcted.isSave = false;

                            StackPanel panel = (StackPanel)tabItemSelcted.Header;
                            Label l = (Label)panel.Children[0];

                            string name = (string)l.Content;

                            if (name.IndexOf('*') != -1)
                            {
                                l.Content = IS.Name;
                            }
                        }
                    }

                    count -= 1;
                }               
            }
            catch(Exception ex)
            {
                if (CollectionMessage.Count > 300)
                {
                    CollectionMessage.RemoveAt(0);

                    CollectionMessage.Insert(298, "Сообщение " + " : " + ex.Message + " " + " " + DateTime.Now);
                }
                else
                {
                    CollectionMessage.Add("Сообщение " + " : " + ex.Message + " " + " " + DateTime.Now);
                }
            }
                                  
            e.Handled = true;
        }
    }

    public class EthernetObject
    {        
        public object LockValue = new object();
        public object LockBool = new object();
        public EthernetSer EthernetSer;       
        public TcpClient TcpClient;
        public UdpClient UdpClient;
        public bool IsReconnect;
        public bool IsAvailableData;
        public bool IsWork;
    }

    public class EthernetOperationalObject
    {
        public object LockValue = new object();
        public object LockBool = new object();
        public EthernetSer EthernetSer;
        public EthernetOperational EthernetOperational;      
        public TcpClient TcpClient;
        public bool IsReconnect;
        public bool IsAvailableData;
        public bool IsWork;
    }

    public class SerialPortObject : SerialPort
    {
        public bool IsWork;
    }

    public class SQLObject
    {
        public bool IsWork;
        public Npgsql.NpgsqlConnection SQL;
    }

    public class ModbusObject
    {
        public object LockValue = new object();
        public object LockAvailableData = new object();
        public object LockSerialPort = new object();
        public Stopwatch TimerReconnect = new Stopwatch();
        public Stopwatch TimerPeriod = new Stopwatch();
        public ModbusControl ModbusControl;
        public SerialPort SerialPort;
        public ModbusSerialMaster ModbusSerialMaster;
        public List<StopwatchItemModbus> CollectionTimer = new List<StopwatchItemModbus>();
        public bool IsWork;
        public bool IsAvailableData;
        public bool IsReconnect;
    }

    public class StopwatchItemNet : Stopwatch
    {
        public ItemNet ItemNet;
        public Stopwatch EmergencyTimerUp;
        public Stopwatch EmergencyTimerDown;
        public object PrevValue;
    }

    public class StopwatchItemModbus : Stopwatch
    {
        public ItemModbus ItemModbus;
        public Stopwatch EmergencyTimerUp;
        public Stopwatch EmergencyTimerDown;
        public object PrevValue;
    }
}
