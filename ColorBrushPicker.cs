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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SCADA
{
    /// <summary>
    /// Выполните шаги 1a или 1b, а затем 2, чтобы использовать этот настраиваемый элемент управления в файле XAML.
    ///
    /// Шаг 1a. Использование настраиваемого элемента управления в файле XAML, существующем в текущем проекте.
    /// Добавьте атрибут XmlNamespace к корневому элементу файла разметки, где он 
    /// должен использоваться:
    ///
    ///     xmlns:MyNamespace="clr-namespace:AAP.Controls"
    ///
    ///
    /// Шаг 1b. Использование этого настраиваемого элемента управления в файле XAML, существующем в текущем проекте.
    /// Добавьте атрибут XmlNamespace к корневому элементу файла разметки, где он 
    /// должен использоваться:
    ///
    ///     xmlns:MyNamespace="clr-namespace:AAP.Controls;assembly=AAP.Controls"
    ///
    /// Потребуется также добавить ссылку на проект из проекта, в котором находится файл XAML
    /// в данный проект и пересобрать во избежание ошибок компиляции:
    ///
    ///     Правой кнопкой мыши щелкните проект в обозревателе решений и выберите команду
    ///     "Добавить ссылку"->"Проекты"->[Выберите этот проект]
    ///
    ///
    /// Шаг 2)
    /// Продолжайте дальше и используйте элемент управления в файле XAML.
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    public class ColorBrushPickerTextControl : Control
    {
        public WrapPanel wrapPanelColor;
        public StackPanel stackPanelRecentColor;
        public Button advanceColor;
        public Xceed.Wpf.Toolkit.DropDownButton DropButton;
        public CustomRichTextBox CustomRichTextBox;
        public Rectangle RectangleColor;
        public GridPropertiesImageGeneral Grid;

        static ColorBrushPickerTextControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorBrushPickerTextControl), new FrameworkPropertyMetadata(typeof(ColorBrushPickerTextControl)));
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
            DialogWindowBrushColor dialoWindow = new DialogWindowBrushColor();
            dialoWindow.Owner = Application.Current.MainWindow;

            Lovatts.ColorEditor.BrushEditor brushEditor = (Lovatts.ColorEditor.BrushEditor)dialoWindow.Content;
            brushEditor.ParentWindow = dialoWindow;

            if (dialoWindow.ShowDialog() == true)
            {
                Brush newBrush = brushEditor.BrushEditorViewModel.Brush;

                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

                ColorButton colorButton = new ColorButton(CustomRichTextBox, Grid, newBrush, stackPanelRecentColor, DropButton, RectangleColor);        
       
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

                RectangleColor.Fill = newBrush;

                TextPointer textPoint = CustomRichTextBox.CaretPosition;
                CustomRichTextBox.startTextPointer = null;
                CustomRichTextBox.endTextPointer = null;

                if (CustomRichTextBox.Selection.IsEmpty)
                {
                    TextElement textElement = null;

                    if (CustomRichTextBox.CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.None)
                    {
                        CustomRichTextBox.IsCreateRunNull = true;

                        CustomRichTextBox.CreateRunTextPointer = CustomRichTextBox.CaretPosition;

                        e.Handled = true;

                        return;
                    }
                    else if (CustomRichTextBox.CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementStart)
                    {
                        textElement = (TextElement)CustomRichTextBox.CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                        if (textElement is Paragraph)
                        {
                            CustomRichTextBox.IsCreateRunNull = true;

                            CustomRichTextBox.CreateRunTextPointer = CustomRichTextBox.CaretPosition;

                            e.Handled = true;

                            return;
                        }
                        else if (textElement is ListItem)
                        {
                            CustomRichTextBox.IsCreateRunNull = true;

                            CustomRichTextBox.CreateRunTextPointer = CustomRichTextBox.CaretPosition;

                            e.Handled = true;

                            return;
                        }
                    }
                    else if (CustomRichTextBox.CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementEnd)
                    {
                        textElement = (TextElement)CustomRichTextBox.CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                        if (textElement is LineBreak)
                        {
                            CustomRichTextBox.IsCreateRunNull = true;

                            CustomRichTextBox.CreateRunTextPointer = CustomRichTextBox.CaretPosition;

                            e.Handled = true;

                            return;
                        }
                    }

                    if (textPoint.GetTextRunLength(LogicalDirection.Backward) > 0) // Если позади каретки в текстовом элементе есть текст
                    {
                        TextPointer textPointer = textPoint.GetNextContextPosition(LogicalDirection.Backward);

                        Run run = (Run)textPointer.GetAdjacentElement(LogicalDirection.Backward);

                        TextElement nextBackTextElement = (TextElement)run.ElementStart.GetAdjacentElement(LogicalDirection.Backward);

                        int i = CustomRichTextBox.CaretPosition.GetTextRunLength(LogicalDirection.Backward);

                        TextRange textRange = new TextRange(textPointer, CustomRichTextBox.CaretPosition);

                        int index = textRange.Text.LastIndexOfAny(new char[] { ' ', ',', '.', '?', '!', ':', ';' });

                        if (index != -1)
                        {
                            if (textRange.Text[i - 1] != ' ' && textRange.Text[i - 1] != ',' && textRange.Text[i - 1] != '.' && textRange.Text[i - 1] != '?' && textRange.Text[i - 1] != '!' && textRange.Text[i - 1] != ':' && textRange.Text[i - 1] != ';')
                            {
                                CustomRichTextBox.startTextPointer = CustomRichTextBox.CaretPosition.GetPositionAtOffset(index + 1 - i);
                            }
                            else
                            {
                                CustomRichTextBox.startTextPointer = null;
                            }
                        }
                        else
                        {
                            CustomRichTextBox.IsText = true;

                            CustomRichTextBox.StartTextPointer(nextBackTextElement);
                        }
                    }
                    else
                    {
                        CustomRichTextBox.IsTextNull = true;

                        TextElement nextBackTextElement = (TextElement)CustomRichTextBox.CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                        if (nextBackTextElement is Run)
                        {
                            nextBackTextElement = (TextElement)nextBackTextElement.ElementStart.GetAdjacentElement(LogicalDirection.Backward);
                        }

                        CustomRichTextBox.StartTextPointer(nextBackTextElement);
                    }

                    CustomRichTextBox.IsTextNull = false;

                    CustomRichTextBox.IsText = false;

                    if (textPoint.GetTextRunLength(LogicalDirection.Forward) > 0) // Если в текстовом элементе впереди каретки есть текст
                    {
                        TextPointer textPointer = textPoint.GetNextContextPosition(LogicalDirection.Forward);

                        Run run = (Run)textPointer.GetAdjacentElement(LogicalDirection.Forward);

                        TextElement nextForwardTextElement = (TextElement)run.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);

                        int i = CustomRichTextBox.CaretPosition.GetTextRunLength(LogicalDirection.Forward);

                        TextRange textRange = new TextRange(CustomRichTextBox.CaretPosition, textPointer);

                        int index = textRange.Text.IndexOfAny(new char[] { ' ', ',', '.', '?', '!', ':', ';' });

                        if (index != -1)
                        {
                            if (textRange.Text[0] != ' ' && textRange.Text[0] != ',' && textRange.Text[0] != '.' && textRange.Text[0] != '?' && textRange.Text[0] != '!' && textRange.Text[0] != ':' && textRange.Text[0] != ';')
                            {
                                CustomRichTextBox.endTextPointer = CustomRichTextBox.CaretPosition.GetPositionAtOffset(index);
                            }
                            else
                            {
                                CustomRichTextBox.endTextPointer = null;
                            }
                        }
                        else
                        {
                            CustomRichTextBox.IsText = true;

                            CustomRichTextBox.EndTextPointer(nextForwardTextElement);
                        }
                    }
                    else
                    {
                        CustomRichTextBox.IsTextNull = true;

                        TextElement nextForwardTextElement = (TextElement)CustomRichTextBox.CaretPosition.GetAdjacentElement(LogicalDirection.Forward);

                        if (nextForwardTextElement is Run)
                        {
                            nextForwardTextElement = (TextElement)nextForwardTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);
                        }

                        CustomRichTextBox.EndTextPointer(nextForwardTextElement);
                    }

                    CustomRichTextBox.IsText = false;

                    CustomRichTextBox.IsTextNull = false;

                    if (CustomRichTextBox.startTextPointer != null && CustomRichTextBox.endTextPointer != null)
                    {
                        TextRange textRange = new TextRange(CustomRichTextBox.startTextPointer, CustomRichTextBox.endTextPointer);

                        textRange.ApplyPropertyValue(TextElement.ForegroundProperty, newBrush);
                    }
                    else
                    {
                        if (CustomRichTextBox.Run.Foreground is SolidColorBrush && newBrush is SolidColorBrush)
                        {
                            SolidColorBrush runBrush = CustomRichTextBox.Run.Foreground as SolidColorBrush;
                            SolidColorBrush newSolidColorBrush = newBrush as SolidColorBrush;

                            if (runBrush.Color.A != newSolidColorBrush.Color.A || runBrush.Color.B != newSolidColorBrush.Color.B || runBrush.Color.G != newSolidColorBrush.Color.G || runBrush.Color.R != newSolidColorBrush.Color.R || runBrush.Color.ScA != newSolidColorBrush.Color.ScA || runBrush.Color.ScB != newSolidColorBrush.Color.ScB || runBrush.Color.ScG != newSolidColorBrush.Color.ScG || runBrush.Color.ScR != newSolidColorBrush.Color.ScR || runBrush.Opacity != newSolidColorBrush.Opacity)
                            {
                                CustomRichTextBox.IsCreateRunColor = true;

                                CustomRichTextBox.CreateRunTextPointer = CustomRichTextBox.CaretPosition;
                            }
                            else
                            {
                                CustomRichTextBox.IsCreateRunColor = false;
                            }
                        }
                        else if (CustomRichTextBox.Run.Foreground is LinearGradientBrush && newBrush is LinearGradientBrush)
                        {
                            LinearGradientBrush runBrush = CustomRichTextBox.Run.Foreground as LinearGradientBrush;
                            LinearGradientBrush newLinearGradientBrush = newBrush as LinearGradientBrush;

                            int count = 0;

                            if (runBrush.ColorInterpolationMode != newLinearGradientBrush.ColorInterpolationMode || runBrush.EndPoint != newLinearGradientBrush.EndPoint || runBrush.Opacity != newLinearGradientBrush.Opacity || runBrush.MappingMode != newLinearGradientBrush.MappingMode || runBrush.SpreadMethod != newLinearGradientBrush.SpreadMethod || runBrush.StartPoint != newLinearGradientBrush.StartPoint || runBrush.GradientStops.Count != newLinearGradientBrush.GradientStops.Count)
                            {
                                CustomRichTextBox.IsCreateRunColor = true;

                                CustomRichTextBox.CreateRunTextPointer = CustomRichTextBox.CaretPosition;
                            }
                            else
                            {
                                GradientStop newGradientStop;

                                foreach (GradientStop runGradientStop in runBrush.GradientStops)
                                {
                                    newGradientStop = newLinearGradientBrush.GradientStops[count];

                                    if (runGradientStop.Color.A != newGradientStop.Color.A || runGradientStop.Color.B != newGradientStop.Color.B || runGradientStop.Color.G != newGradientStop.Color.G || runGradientStop.Color.R != newGradientStop.Color.R || runGradientStop.Color.ScA != newGradientStop.Color.ScA || runGradientStop.Color.ScB != newGradientStop.Color.ScB || runGradientStop.Color.ScG != newGradientStop.Color.ScG || runGradientStop.Color.ScR != newGradientStop.Color.ScR)
                                    {
                                        CustomRichTextBox.IsCreateRunColor = true;

                                        CustomRichTextBox.CreateRunTextPointer = CustomRichTextBox.CaretPosition;

                                        e.Handled = true;

                                        return;
                                    }
                                    else
                                    {
                                        CustomRichTextBox.IsCreateRunColor = false;
                                    }

                                    count++;
                                }
                            }
                        }
                        else if (CustomRichTextBox.Run.Foreground is RadialGradientBrush && newBrush is RadialGradientBrush)
                        {
                            RadialGradientBrush runBrush = CustomRichTextBox.Run.Foreground as RadialGradientBrush;
                            RadialGradientBrush newRadialGradientBrush = newBrush as RadialGradientBrush;

                            int count = 0;

                            if (runBrush.Center != newRadialGradientBrush.Center || runBrush.ColorInterpolationMode != newRadialGradientBrush.ColorInterpolationMode || runBrush.GradientOrigin != newRadialGradientBrush.GradientOrigin || runBrush.MappingMode != newRadialGradientBrush.MappingMode || runBrush.Opacity != newRadialGradientBrush.Opacity || runBrush.RadiusX != newRadialGradientBrush.RadiusX || runBrush.RadiusY != newRadialGradientBrush.RadiusY || runBrush.SpreadMethod != newRadialGradientBrush.SpreadMethod || runBrush.GradientStops.Count != newRadialGradientBrush.GradientStops.Count)
                            {
                                CustomRichTextBox.IsCreateRunColor = true;

                                CustomRichTextBox.CreateRunTextPointer = CustomRichTextBox.CaretPosition;
                            }
                            else
                            {
                                GradientStop newGradientStop;

                                foreach (GradientStop runGradientStop in runBrush.GradientStops)
                                {
                                    newGradientStop = newRadialGradientBrush.GradientStops[count];

                                    if (runGradientStop.Color.A != newGradientStop.Color.A || runGradientStop.Color.B != newGradientStop.Color.B || runGradientStop.Color.G != newGradientStop.Color.G || runGradientStop.Color.R != newGradientStop.Color.R || runGradientStop.Color.ScA != newGradientStop.Color.ScA || runGradientStop.Color.ScB != newGradientStop.Color.ScB || runGradientStop.Color.ScG != newGradientStop.Color.ScG || runGradientStop.Color.ScR != newGradientStop.Color.ScR)
                                    {
                                        CustomRichTextBox.IsCreateRunColor = true;

                                        CustomRichTextBox.CreateRunTextPointer = CustomRichTextBox.CaretPosition;

                                        e.Handled = true;

                                        return;
                                    }
                                    else
                                    {
                                        CustomRichTextBox.IsCreateRunColor = false;
                                    }

                                    count++;
                                }
                            }
                        }
                        else
                        {
                            CustomRichTextBox.IsCreateRunColor = true;

                            CustomRichTextBox.CreateRunTextPointer = CustomRichTextBox.CaretPosition;
                        }
                    }
                }
                else
                {
                    CustomRichTextBox.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, newBrush);
                }              
            }
           
            e.Handled = true;
        }

        public ColorBrushPickerTextControl(Xceed.Wpf.Toolkit.DropDownButton dropButton, CustomRichTextBox customRichTextBox, Rectangle rectangleColor)
        {
            DropButton = dropButton;
            CustomRichTextBox = customRichTextBox;
            RectangleColor = rectangleColor;
        }
    }
}
