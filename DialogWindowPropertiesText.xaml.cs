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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SCADA
{
    /// <summary>
    /// Логика взаимодействия для DialogWindowPropertiesText.xaml
    /// </summary>
    public partial class DialogWindowPropertiesText : Window
    {
        public DialogWindowPropertiesText()
        {
            InitializeComponent();
        }

        private Text text;
        public Text Text
        {
            get { return text; }
            set { text = value; }
        }

        FlowDocument tempFlowDocument;
        TextEditor textEditor;
        RowDefinition Row0 = new RowDefinition();

        public DialogWindowPropertiesText(Text text)
        {
            InitializeComponent();

            Text = text;

            Row0.Height = new GridLength(30, GridUnitType.Auto);

            using(MemoryStream ms = new MemoryStream())
            {               
                XamlWriter.Save(Text.customFlowDocumentScrollViewer.Document, ms);
                ms.Position = 0;
                ms.Seek(0, SeekOrigin.Begin);
                tempFlowDocument = (FlowDocument)XamlReader.Load(ms);
            }

            textEditor = new TextEditor(tempFlowDocument);
            Grid.SetRow(textEditor, 0);
        }

        private void Apply(object sender, RoutedEventArgs e)
        {
            FlowDocument flowDocument = textEditor.richTextBox.Document;
            textEditor.richTextBox.Document = new FlowDocument();
            Text.customFlowDocumentScrollViewer.Document = flowDocument;
            Text.customFlowDocumentScrollViewer.Document.PageWidth = 30000;

            if (textEditor.IsSave)
            {
                ((AppWPF)Application.Current).SaveTabItem(Text.CanvasTab.TabItemParent);

                Text.TextSer.TextDocument = flowDocument;

                textEditor.IsSave = false;
            }
                           
            e.Handled = true;
            this.Close();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            this.Close();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            TreeViewItemEditor.IsSelected = true;           
            e.Handled = true;
        }
     
        private void TreeViewProperties_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {            
            TreeViewItem Selected = (TreeViewItem)e.NewValue;
            if ((string)Selected.Header == "Редактор")
            {
                GridSetting.Children.Clear();
                GridSetting.RowDefinitions.Clear();

                GroupBoxSetting.Header = "Редактор";
            
                GridSetting.RowDefinitions.Add(Row0);

                GridSetting.Children.Add(textEditor);

                Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    textEditor.richTextBox.Focus();
                }));                   
            }              
         
            e.Handled = true;
        }
    }
}
