// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SCADA
{
    [Serializable]
    public class Login
    {
        public string LoginName { get; set; }
        public SecureString Password { get; set; }
        public Group Group { get; set; }
    }

    [Serializable]
    public class Group
    {
        public string GroupName { get; set; }
        public string GroupPolicy { get; set; }
    }

    [Serializable]
    public class SerializationSetting : INotifyPropertyChanged
    {
        public bool IsWindowUpdate { get; set; }
        public bool IsWindowErrorMessage { get; set; }
        public string PathBrowseProject { get; set; } // путь по умолчанию, для создания новых проектов
        public bool CreateFolder { get; set; } // Создавать папку под проект

        public SerializationSetting()
        {
            SQLSecuritySSPI = true;

            CollectionLogins = new List<Login>();

            Group group = new Group();
            group.GroupName = "Администратор";
            group.GroupPolicy = "Все права";

            CollectionGroup = new List<Group>();
            CollectionGroup.Add(group);

            CollectionGroupPolicy = new List<string>();
            CollectionGroupPolicy.Add("Все права");
            CollectionGroupPolicy.Add("Закрытие приложения");
            CollectionGroupPolicy.Add("Остановка приложения");
            CollectionGroupPolicy.Add("Запуск приложения");
        }

        public List<Login> CollectionLogins { get; set; }
        public List<Group> CollectionGroup { get; set; }
        public List<string> CollectionGroupPolicy { get; set; }

        public string SQLServerName { get; set; }
        public string SQLDatabaseName { get; set; }
        public string SQLPassword { get; set; }
        public string SQLUserName { get; set; }
        public bool SQLSecuritySSPI { get; set; }
        public bool UseDatabaseSer;

        public bool UseDatabase
        {
            get { return UseDatabaseSer; }
            set
            {
                UseDatabaseSer = value;
                OnPropertyChanged(new PropertyChangedEventArgs("UseDatabase"));
            }
        }

        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }
    }
}
