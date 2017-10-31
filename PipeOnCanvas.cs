﻿using System;
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
    public class PipeOnCanvas : ControlOnCanvasPage
    {
        public double Diameter { get; set; }
        public TextBox TextBoxDiameter { get; set; }
        public ComboBox ComboBoxEnvironment { get; set; }

        private int intEnvironment;
        public virtual int IntEnvironment
        {
            get { return intEnvironment; }
            set { intEnvironment = value; }
        }
      
        static PipeOnCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PipeOnCanvas), new FrameworkPropertyMetadata(typeof(PipeOnCanvas)));
        }

        protected PipeOnCanvas(ControlOnCanvasSer ser) 
            : base(ser)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

            TextBoxDiameter = mainWindow.TextBoxDiameter;
            ComboBoxEnvironment = mainWindow.ComboBoxEnvironment;
        }
    }
}
