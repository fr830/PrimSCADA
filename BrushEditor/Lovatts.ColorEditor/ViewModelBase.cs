using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace BrushEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        public bool FireOnPropertyChanged = true;
        public bool PropertyDirty;

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            if (!FireOnPropertyChanged)
            {
                PropertyDirty = true;
                return;
            }

            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                PropertyDirty = false;

                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        public virtual void OnPropertyChanged<T>(Expression<Func<T>> propertyNameExpression)
        {
            OnPropertyChanged(((MemberExpression) propertyNameExpression.Body).Member.Name);
        }

        #endregion
    }
}