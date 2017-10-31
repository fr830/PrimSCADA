using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SCADA
{
    [Serializable]
    public class ClipboardManipulation
    {
        public ClipboardManipulation(int code)
        {
            Manipulation = code;
        }
        // Код операции: 1 = копирование объекта, 2 = вырезание объекта, 3 = копирование объектов в canvasControlPanel, 4 = вырезание объектов в canvasControlPanel
        private int manipulation;
        public int Manipulation
        {
            get { return manipulation; }
            set 
            {
                manipulation = value;

                if(value > 4 && value < 1)
                {
                    MessageBox.Show("Ошибка буфера обмена, аргумент класса ClipboardManipulation вышел из диапозона допустимых чисел 1-4", "Ошибка буфера обмена", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }  
}
