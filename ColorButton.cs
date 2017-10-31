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
    /// Выполните шаги 1a или 1b, а затем 2, чтобы использовать этот пользовательский элемент управления в файле XAML.
    ///
    /// Шаг 1a. Использование пользовательского элемента управления в файле XAML, существующем в текущем проекте.
    /// Добавьте атрибут XmlNamespace в корневой элемент файла разметки, где он 
    /// будет использоваться:
    ///
    ///     xmlns:MyNamespace="clr-namespace:AAP.Controls"
    ///
    ///
    /// Шаг 1б. Использование пользовательского элемента управления в файле XAML, существующем в другом проекте.
    /// Добавьте атрибут XmlNamespace в корневой элемент файла разметки, где он 
    /// будет использоваться:
    ///
    ///     xmlns:MyNamespace="clr-namespace:AAP.Controls;assembly=AAP.Controls"
    ///
    /// Потребуется также добавить ссылку из проекта, в котором находится файл XAML,
    /// на данный проект и пересобрать во избежание ошибок компиляции:
    ///
    ///     Щелкните правой кнопкой мыши нужный проект в обозревателе решений и выберите
    ///     "Добавить ссылку"->"Проекты"->[Поиск и выбор проекта]
    ///
    ///
    /// Шаг 2)
    /// Теперь можно использовать элемент управления в файле XAML.
    ///
    ///     <MyNamespace:ColorButton/>
    ///
    /// </summary>
    public class ColorButton : Button
    {
        public CustomRichTextBox CustomRichTextBox;
        public Brush Brush;
        public StackPanel StackPanelRecentColor;
        public Xceed.Wpf.Toolkit.DropDownButton DropButton;
        public Rectangle RectangleColor;
        public GridPropertiesImageGeneral GridProp;

        static ColorButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorButton), new FrameworkPropertyMetadata(typeof(ColorButton)));
        }

        protected override void OnClick()
        {
            base.OnClick();

            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            
            if (this.Parent != StackPanelRecentColor)
            {
                ColorButton colorButton = new ColorButton(CustomRichTextBox, GridProp, Brush, StackPanelRecentColor, DropButton, RectangleColor);               

                if (mainWindow.RecentColorCollection.Count < 7)
                {
                    StackPanelRecentColor.Children.Add(colorButton);

                    mainWindow.RecentColorCollection.Add(colorButton);
                }
                else
                {
                    mainWindow.RecentColorCollection.RemoveAt(0);
                    mainWindow.RecentColorCollection.Add(colorButton);

                    StackPanelRecentColor.Children.RemoveAt(0);
                    StackPanelRecentColor.Children.Add(colorButton);
                }                
            }

            RectangleColor.Fill = Brush;

            DropButton.IsOpen = false;          

            if (GridProp == null)
            {
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

                        return;
                    }
                    else if (CustomRichTextBox.CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementStart)
                    {
                        textElement = (TextElement)CustomRichTextBox.CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                        if (textElement is Paragraph)
                        {
                            CustomRichTextBox.IsCreateRunNull = true;

                            CustomRichTextBox.CreateRunTextPointer = CustomRichTextBox.CaretPosition;

                            return;
                        }
                        else if (textElement is ListItem)
                        {
                            CustomRichTextBox.IsCreateRunNull = true;

                            CustomRichTextBox.CreateRunTextPointer = CustomRichTextBox.CaretPosition;

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

                        textRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brush);
                    }
                    else
                    {
                        if (CustomRichTextBox.Run.Foreground is SolidColorBrush && Brush is SolidColorBrush)
                        {
                            SolidColorBrush runBrush = CustomRichTextBox.Run.Foreground as SolidColorBrush;
                            SolidColorBrush newBrush = Brush as SolidColorBrush;

                            if (runBrush.Color.A != newBrush.Color.A || runBrush.Color.B != newBrush.Color.B || runBrush.Color.G != newBrush.Color.G || runBrush.Color.R != newBrush.Color.R || runBrush.Color.ScA != newBrush.Color.ScA || runBrush.Color.ScB != newBrush.Color.ScB || runBrush.Color.ScG != newBrush.Color.ScG || runBrush.Color.ScR != newBrush.Color.ScR || runBrush.Opacity != newBrush.Opacity)
                            {
                                CustomRichTextBox.IsCreateRunColor = true;

                                CustomRichTextBox.CreateRunTextPointer = CustomRichTextBox.CaretPosition;
                            }
                            else
                            {
                                CustomRichTextBox.IsCreateRunColor = false;
                            }
                        }
                        else if (CustomRichTextBox.Run.Foreground is LinearGradientBrush && Brush is LinearGradientBrush)
                        {
                            LinearGradientBrush runBrush = CustomRichTextBox.Run.Foreground as LinearGradientBrush;
                            LinearGradientBrush newBrush = Brush as LinearGradientBrush;

                            int count = 0;

                            if (runBrush.ColorInterpolationMode != newBrush.ColorInterpolationMode || runBrush.EndPoint != newBrush.EndPoint || runBrush.Opacity != newBrush.Opacity || runBrush.MappingMode != newBrush.MappingMode || runBrush.SpreadMethod != newBrush.SpreadMethod || runBrush.StartPoint != newBrush.StartPoint || runBrush.GradientStops.Count != newBrush.GradientStops.Count)
                            {
                                CustomRichTextBox.IsCreateRunColor = true;

                                CustomRichTextBox.CreateRunTextPointer = CustomRichTextBox.CaretPosition;
                            }
                            else
                            {
                                GradientStop newGradientStop;

                                foreach (GradientStop runGradientStop in runBrush.GradientStops)
                                {
                                    newGradientStop = newBrush.GradientStops[count];

                                    if (runGradientStop.Color.A != newGradientStop.Color.A || runGradientStop.Color.B != newGradientStop.Color.B || runGradientStop.Color.G != newGradientStop.Color.G || runGradientStop.Color.R != newGradientStop.Color.R || runGradientStop.Color.ScA != newGradientStop.Color.ScA || runGradientStop.Color.ScB != newGradientStop.Color.ScB || runGradientStop.Color.ScG != newGradientStop.Color.ScG || runGradientStop.Color.ScR != newGradientStop.Color.ScR)
                                    {
                                        CustomRichTextBox.IsCreateRunColor = true;

                                        CustomRichTextBox.CreateRunTextPointer = CustomRichTextBox.CaretPosition;

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
                        else if (CustomRichTextBox.Run.Foreground is RadialGradientBrush && Brush is RadialGradientBrush)
                        {
                            RadialGradientBrush runBrush = CustomRichTextBox.Run.Foreground as RadialGradientBrush;
                            RadialGradientBrush newBrush = Brush as RadialGradientBrush;

                            int count = 0;

                            if (runBrush.Center != newBrush.Center || runBrush.ColorInterpolationMode != newBrush.ColorInterpolationMode || runBrush.GradientOrigin != newBrush.GradientOrigin || runBrush.MappingMode != newBrush.MappingMode || runBrush.Opacity != newBrush.Opacity || runBrush.RadiusX != newBrush.RadiusX || runBrush.RadiusY != newBrush.RadiusY || runBrush.SpreadMethod != newBrush.SpreadMethod || runBrush.GradientStops.Count != newBrush.GradientStops.Count)
                            {
                                CustomRichTextBox.IsCreateRunColor = true;

                                CustomRichTextBox.CreateRunTextPointer = CustomRichTextBox.CaretPosition;
                            }
                            else
                            {
                                GradientStop newGradientStop;

                                foreach (GradientStop runGradientStop in runBrush.GradientStops)
                                {
                                    newGradientStop = newBrush.GradientStops[count];

                                    if (runGradientStop.Color.A != newGradientStop.Color.A || runGradientStop.Color.B != newGradientStop.Color.B || runGradientStop.Color.G != newGradientStop.Color.G || runGradientStop.Color.R != newGradientStop.Color.R || runGradientStop.Color.ScA != newGradientStop.Color.ScA || runGradientStop.Color.ScB != newGradientStop.Color.ScB || runGradientStop.Color.ScG != newGradientStop.Color.ScG || runGradientStop.Color.ScR != newGradientStop.Color.ScR)
                                    {
                                        CustomRichTextBox.IsCreateRunColor = true;

                                        CustomRichTextBox.CreateRunTextPointer = CustomRichTextBox.CaretPosition;

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
                    CustomRichTextBox.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, Brush);
                }
            }
            else
            {
                GridProp.NewBrush = Brush;
            }
            
        }

        public ColorButton(CustomRichTextBox customRichTextBox, GridPropertiesImageGeneral grid, Brush brush, StackPanel stackPanelRecentColor, Xceed.Wpf.Toolkit.DropDownButton dropButton, Rectangle rectangleColor)
        {
            GridProp = grid;
            CustomRichTextBox = customRichTextBox;
            Brush = brush;
            StackPanelRecentColor = stackPanelRecentColor;
            DropButton = dropButton;
            this.Background = brush;
            RectangleColor = rectangleColor;
        }
    }
}
