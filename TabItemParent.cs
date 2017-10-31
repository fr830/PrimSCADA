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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SCADA
{
    public class TabItemParent : TabItem
    {
        public bool isSave { get; set; }
        public ItemScadaTabItem IS { get; set; }
        public CanvasTab CanvasTab { get; set; }

        static TabItemParent()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabItemParent), new FrameworkPropertyMetadata(typeof(TabItemParent)));
        }

        protected TabItemParent()
        { }

        public virtual void DeleteTabItem()
        {}
      
        public void CloseTabItem(object sender, RoutedEventArgs e)
        {
            Window MainWindow = ((AppWPF)System.Windows.Application.Current).MainWindow;

            ((MainWindow)MainWindow).TabControlMain.Items.Remove(this);

            e.Handled = true;
        }

        public void RenameTab()
        {
            StackPanel panel = (StackPanel)this.Header;
            panel.ToolTip = IS.Path;
            Label l = (Label)panel.Children[0];

            if (l.Content != null)
            {
                string name = (string)l.Content;

                if (name.LastIndexOf('*') == -1)
                {
                    l.Content = IS.Name;
                }
                else
                {
                    l.Content = IS.Name + "*";
                }
            }          
        }
    }
}
