// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SCADA
{
    [Serializable]
    public class ItemScada
    {
        // Глубина вложения элемента
        private int attachments;
        public int Attachments
        {
            get { return attachments; }
            set { attachments = value; }
        }

        private string path;
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        // Вложеная папка 
        private string attachmentFolder;
        public string AttachmentFolder
        {
            get { return attachmentFolder; }
            set { attachmentFolder = value; }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [NonSerialized]
        private TreeViewItem treeItem;
        public TreeViewItem TreeItem
        {
            get { return treeItem; }
            set { treeItem = value; }
        }

        [NonSerialized]
        private TreeViewItem parentItem;
        public TreeViewItem ParentItem
        {
            get { return parentItem; }
            set { parentItem = value; }
        }
    }

    [Serializable]
    public class ItemScadaTabItem: ItemScada
    {
        private bool isFocus;
        public bool IsFocus
        {
            get { return isFocus; }
            set { isFocus = value; }
        }

        private bool isOpen;
        public bool IsOpen
        {
            get { return isOpen; }
            set { isOpen = value; }
        }
    }

    [Serializable]
    public class FolderScada: ItemScada, IComparable
    {
        public int CompareTo(object obj)
        {
            FolderScada otherFolderScada = obj as FolderScada;

            if (otherFolderScada != null) return this.Attachments.CompareTo(otherFolderScada.Attachments);
            else throw new ArgumentException("Пустой объект для сортировки");
        }
            
        [NonSerialized]
        private List<TreeViewItem> childItem;
        public List<TreeViewItem> ChildItem
        {
            get { return childItem; }
            set { childItem = value; }
        }

        private bool isExpanded;
        public bool IsExpand
        {
            get { return isExpanded; }
            set { isExpanded = value; }
        }       
    }
    [Serializable]
    public class PageScada: ItemScadaTabItem, IComparable
    {
        public int CompareTo(object obj)
        {
            PageScada otherPageScada = obj as PageScada;

            if (otherPageScada != null) return this.Attachments.CompareTo(otherPageScada.Attachments);

            else throw new ArgumentException("Пустой объект для сортировки");
        }                     
    }

    [Serializable]
    public class SerializationProject
    {     
        public string ProjectName
        {
            get;
            set;
        }

        public string PathProject
        {
            get;
            set;
        }

        private Dictionary<string, PageScada> collectionPageScada = new Dictionary<string,PageScada>();
        public Dictionary<string, PageScada> CollectionPageScada
        {
            get { return collectionPageScada; }
        }

        private Dictionary<string, FolderScada> collectionFolderScada = new Dictionary<string,FolderScada>();
        public Dictionary<string, FolderScada> CollectionFolderScada
        {
            get { return collectionFolderScada; }
        }       
    }
}
