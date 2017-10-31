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
        public EthernetOperationalSearch EthernetOperationalSearch;
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

                EthernetOperationalSearch = null;

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
            else if (TVEthernets.TVEthernet.SelectedItem is EthernetOperational)
            {
                EthernetOperational ethernetOperationalBinding = TVEthernets.TVEthernet.SelectedItem as EthernetOperational;

                TreeViewItem tvItemEthernet = null;

                foreach(var Item in TVEthernets.TVEthernet.Items)
                {
                    TreeViewItem treeItem = (TreeViewItem)TVEthernets.TVEthernet.ItemContainerGenerator.ContainerFromItem(Item);

                    foreach (var ItemSub in treeItem.Items)
                    {
                        if (ethernetOperationalBinding == ItemSub)
                        {
                            tvItemEthernet = treeItem;
                            break;
                        }
                    }
                }

                EthernetSearch = ((EthernetSer)tvItemEthernet.DataContext).ID;

                EthernetOperationalSearch = ethernetOperationalBinding.EthernetOperationalSearch;             

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
                EthernetOperationalSearch = null;
                DisplaySer.IsCollRec = false;
                DisplaySer.IsCollSend = false;
            }

            e.Handled = true;
        }
    }
}
