// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
    /// <summary>
    /// Логика взаимодействия для TVItemNet.xaml
    /// </summary>
    public partial class TVItemNet : UserControl
    {
        public TVEthernets TVEthernets;
        public ItemNet ItemNetSearch;
        public string EthernetSearch;
        public DisplaySer DisplaySer;

        public TVItemNet(DisplaySer displaySer)
        {
            InitializeComponent();

            DisplaySer = displaySer;
        }

        void Checked(Object sender, RoutedEventArgs e)
        {
            ItemNetSearch = (ItemNet)((CheckBox)sender).DataContext;           

            if (TVEthernets.TVEthernet.SelectedItem is EthernetSer)
            {
                EthernetSer ethernetSerBinding = TVEthernets.TVEthernet.SelectedItem as EthernetSer;

                EthernetSearch = ethernetSerBinding.ID;

                foreach (TreeViewItem tvItemNet in TVItemNets.Items)
                {
                    foreach (ItemNet itemNet in tvItemNet.Items)
                    {
                        if (!System.Object.ReferenceEquals(ItemNetSearch, itemNet))
                        {
                            itemNet.IsBinding = false;
                        }
                        else
                        {
                            if (tvItemNet == TVItemNets.Items[0])
                            {
                                DisplaySer.IsCollRec = true;
                                DisplaySer.IsCollSend = false;
                            }
                            else if (tvItemNet == TVItemNets.Items[1])
                            {
                                DisplaySer.IsCollRec = false;
                                DisplaySer.IsCollSend = true;
                            }
                        }
                    }                                 
                }
            }
                        
            ItemNetSearch.IsBinding = true;

            e.Handled = true;
        }

        void Unchecked(Object sender, RoutedEventArgs e)
        {
            ItemNet uncheckedItemNet = (ItemNet)((CheckBox)sender).DataContext;

            if (System.Object.ReferenceEquals(ItemNetSearch, uncheckedItemNet))
            {
                ItemNetSearch = null;
                EthernetSearch = null;
                DisplaySer.IsCollRec = false;
                DisplaySer.IsCollSend = false;
            }

            e.Handled = true;
        }
    }
}
