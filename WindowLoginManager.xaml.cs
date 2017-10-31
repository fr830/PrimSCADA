// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SCADA
{
    /// <summary>
    /// Логика взаимодействия для WindowLoginManager.xaml
    /// </summary>
    public partial class WindowLoginManager : Window
    {
        public WindowLoginManager()
        {
            InitializeComponent();

            Group.ItemsSource = ((AppWPF)Application.Current).ConfigProgramBin.CollectionGroup;

            Policy.ItemsSource = ((AppWPF)Application.Current).ConfigProgramBin.CollectionGroupPolicy;

            Binding bindEnablePilicy = new Binding();
            bindEnablePilicy.Source = Group;
            bindEnablePilicy.Path = new PropertyPath("SelectedItem");
            bindEnablePilicy.Converter = new EnablePolicyConverter();

            Policy.SetBinding(Control.IsEnabledProperty, bindEnablePilicy);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((string)((CheckBox)e.Source).DataContext == "Все права")
            {
                foreach (object obj in Policy.Items)
                {
                    ListBoxItem myListBoxItem = (ListBoxItem)(Policy.ItemContainerGenerator.ContainerFromItem(obj));

                    ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(myListBoxItem);

                    DataTemplate myDataTemplate = myContentPresenter.ContentTemplate;

                    CheckBox checkBox = (CheckBox)myDataTemplate.FindName("CheckBox", myContentPresenter);
                    checkBox.IsChecked = true;
                }
            }

            e.Handled = true;
        }

        private childItem FindVisualChild<childItem>(DependencyObject obj)
    where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if ((string)((CheckBox)e.Source).DataContext == "Все права")
            {
                foreach (object obj in Policy.Items)
                {
                    ListBoxItem myListBoxItem = (ListBoxItem)(Policy.ItemContainerGenerator.ContainerFromItem(obj));

                    ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(myListBoxItem);

                    DataTemplate myDataTemplate = myContentPresenter.ContentTemplate;

                    CheckBox checkBox = (CheckBox)myDataTemplate.FindName("CheckBox", myContentPresenter);
                    checkBox.IsChecked = false;
                }
            }

            e.Handled = true;
        }

        private void Group_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((Group)e.AddedItems[0]).GroupName == "Администратор")
            {
                foreach (object obj in Policy.Items)
                {
                    ListBoxItem myListBoxItem = (ListBoxItem)(Policy.ItemContainerGenerator.ContainerFromItem(obj));

                    ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(myListBoxItem);

                    DataTemplate myDataTemplate = myContentPresenter.ContentTemplate;

                    CheckBox checkBox = (CheckBox)myDataTemplate.FindName("CheckBox", myContentPresenter);
                    checkBox.IsChecked = true;
                }
            }

            e.Handled = true;
        }
    }
}
