// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SCADA
{
    class ColorBrushPickerBorder: Control
    {
        public WrapPanel wrapPanelColor;
        public StackPanel stackPanelRecentColor;
        public Button advanceColor;
        public Xceed.Wpf.Toolkit.DropDownButton DropButton;
        public CustomRichTextBox CustomRichTextBox;
        public Rectangle RectangleColor;
        public Brush Brush;
        public GridPropertiesImageGeneral Grid;

        static ColorBrushPickerBorder()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorBrushPickerBorder), new FrameworkPropertyMetadata(typeof(ColorBrushPickerBorder)));
        }

        public ColorBrushPickerBorder(Xceed.Wpf.Toolkit.DropDownButton dropButton, GridPropertiesImageGeneral grid, Rectangle rectangleColor)
        {
            DropButton = dropButton;
            RectangleColor = rectangleColor;
            Grid = grid;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                wrapPanelColor = GetTemplateChild("WrapPanelColor") as WrapPanel;
                stackPanelRecentColor = GetTemplateChild("StackPanelRecentColor") as StackPanel;
                advanceColor = GetTemplateChild("AdvanceColor") as Button;

                advanceColor.Click += advanceColor_Click;

                ColorButton white = new ColorButton(CustomRichTextBox, Grid, Brushes.White, stackPanelRecentColor, DropButton, RectangleColor);

                ColorButton black = new ColorButton(CustomRichTextBox, Grid, Brushes.Black, stackPanelRecentColor, DropButton, RectangleColor);

                ColorButton red = new ColorButton(CustomRichTextBox, Grid, Brushes.Red, stackPanelRecentColor, DropButton, RectangleColor);

                ColorButton green = new ColorButton(CustomRichTextBox, Grid, Brushes.Green, stackPanelRecentColor, DropButton, RectangleColor);

                ColorButton orange = new ColorButton(CustomRichTextBox, Grid, Brushes.Orange, stackPanelRecentColor, DropButton, RectangleColor);

                ColorButton yellow = new ColorButton(CustomRichTextBox, Grid, Brushes.Yellow, stackPanelRecentColor, DropButton, RectangleColor);

                ColorButton blue = new ColorButton(CustomRichTextBox, Grid, Brushes.Blue, stackPanelRecentColor, DropButton, RectangleColor);

                ColorButton pink = new ColorButton(CustomRichTextBox, Grid, Brushes.Pink, stackPanelRecentColor, DropButton, RectangleColor);

                ColorButton gray = new ColorButton(CustomRichTextBox, Grid, Brushes.Gray, stackPanelRecentColor, DropButton, RectangleColor);

                ColorButton gold = new ColorButton(CustomRichTextBox, Grid, Brushes.Gold, stackPanelRecentColor, DropButton, RectangleColor);

                ColorButton greenYellow = new ColorButton(CustomRichTextBox, Grid, Brushes.GreenYellow, stackPanelRecentColor, DropButton, RectangleColor);

                ColorButton purple = new ColorButton(CustomRichTextBox, Grid, Brushes.Purple, stackPanelRecentColor, DropButton, RectangleColor);

                ColorButton indigo = new ColorButton(CustomRichTextBox, Grid, Brushes.Indigo, stackPanelRecentColor, DropButton, RectangleColor);

                ColorButton navy = new ColorButton(CustomRichTextBox, Grid, Brushes.Navy, stackPanelRecentColor, DropButton, RectangleColor);

                ColorButton olive = new ColorButton(CustomRichTextBox, Grid, Brushes.Olive, stackPanelRecentColor, DropButton, RectangleColor);

                ColorButton tomato = new ColorButton(CustomRichTextBox, Grid, Brushes.Tomato, stackPanelRecentColor, DropButton, RectangleColor);

                ColorButton tan = new ColorButton(CustomRichTextBox, Grid, Brushes.Tan, stackPanelRecentColor, DropButton, RectangleColor);

                ColorButton orchid = new ColorButton(CustomRichTextBox, Grid, Brushes.Orchid, stackPanelRecentColor, DropButton, RectangleColor);

                wrapPanelColor.Children.Add(white);
                wrapPanelColor.Children.Add(black);
                wrapPanelColor.Children.Add(red);
                wrapPanelColor.Children.Add(green);
                wrapPanelColor.Children.Add(orange);
                wrapPanelColor.Children.Add(yellow);
                wrapPanelColor.Children.Add(blue);
                wrapPanelColor.Children.Add(pink);
                wrapPanelColor.Children.Add(gray);
                wrapPanelColor.Children.Add(gold);
                wrapPanelColor.Children.Add(greenYellow);
                wrapPanelColor.Children.Add(purple);
                wrapPanelColor.Children.Add(indigo);
                wrapPanelColor.Children.Add(navy);
                wrapPanelColor.Children.Add(olive);
                wrapPanelColor.Children.Add(tomato);
                wrapPanelColor.Children.Add(tan);
                wrapPanelColor.Children.Add(orchid);

                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

                if (mainWindow.RecentColorCollection.Count > 0)
                {
                    foreach (ColorButton colorButton in mainWindow.RecentColorCollection)
                    {
                        ColorButton newColorButton = new ColorButton(CustomRichTextBox, Grid, colorButton.Brush, stackPanelRecentColor, DropButton, RectangleColor);

                        stackPanelRecentColor.Children.Add(newColorButton);
                    }
                }
            }));
        }

        void advanceColor_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            DialogWindowBrushColor dialoWindow = new DialogWindowBrushColor();
            dialoWindow.Owner = Application.Current.MainWindow;

            Lovatts.ColorEditor.BrushEditor brushEditor = (Lovatts.ColorEditor.BrushEditor)dialoWindow.Content;
            brushEditor.ParentWindow = dialoWindow;

            if (dialoWindow.ShowDialog() == true)
            {
                Brush = brushEditor.BrushEditorViewModel.Brush;

                Grid.NewBrush = Brush;

                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

                ColorButton colorButton = new ColorButton(CustomRichTextBox, Grid, Brush, stackPanelRecentColor, DropButton, RectangleColor);

                if (mainWindow.RecentColorCollection.Count < 7)
                {
                    stackPanelRecentColor.Children.Add(colorButton);

                    mainWindow.RecentColorCollection.Add(colorButton);
                }
                else
                {
                    mainWindow.RecentColorCollection.RemoveAt(0);
                    mainWindow.RecentColorCollection.Add(colorButton);

                    stackPanelRecentColor.Children.RemoveAt(0);
                    stackPanelRecentColor.Children.Add(colorButton);
                }

                RectangleColor.Fill = Brush;
            }
        }
    }
}
