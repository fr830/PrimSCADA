using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SCADA
{   
    public class CustomRichTextBox : RichTextBox
    {
        public Button ButtonPaste;
        public Button ButtonOpenFile;
        public Button ButtonSaveFile;
        public ComboBox ComboBoxFont;
        public ComboBox ComboBoxFontSize;
        public Button FontSizeUp;
        public Button FontSizeDown;
        public ToggleButton FontBold;
        public ToggleButton FontItalic;
        public ToggleButton FontUnderline;
        public ToggleButton FontStrikethrough;
        public ToggleButton FontOverline;
        public ToggleButton FontSuperscript;
        public ToggleButton FontSubscript;
        public ToggleButton NumberedList;
        public ToggleButton BulletedList;
        public ToggleButton AlignLeft;
        public ToggleButton AlignCenter;
        public ToggleButton AlignRight;
        public ToggleButton AlignFull;
        public Rectangle RectangleColor;
        private TextBox FontSizeTextBox;
        private TextBox FontTextBox;

        public Label DebugLabel;
        public bool IsCreateRunNull;
        public bool IsCreateRunBold;
        public bool IsCreateRunItalic;
        public bool IsCreateRunColor;
        public bool IsCreateRunSize;
        public bool IsCreateRunFont;
        public bool IsCreateRunScript;
        public bool IsCreateRunStrikethrough;
        public bool IsCreateRunOverline;
        public bool IsCreateRunUnderline;
        public TextPointer CreateRunTextPointer;
        public TextPointer startTextPointer;
        public TextPointer endTextPointer;
        public bool IsPaste;
        public bool IsLeftSpace;
        public bool IsTextNull;
        public bool IsText;

        public TextEditor TextEditor;

        double CorrectFontSize;
      
        FontFamily СorrectFont;

        public Run Run;
        
        public double[] FontSizes
        {
            get
            {
                return new double[] { 
                     3.0, 4.0, 5.0, 6.0, 6.5, 7.0, 7.5, 8.0, 8.5, 9.0, 9.5, 
                     10.0, 10.5, 11.0, 11.5, 12.0, 12.5, 13.0, 13.5, 14.0, 15.0,
                     16.0, 17.0, 18.0, 19.0, 20.0, 22.0, 24.0, 26.0, 28.0, 30.0,
                     32.0, 34.0, 36.0, 38.0, 40.0, 44.0, 48.0, 52.0, 56.0, 60.0, 64.0, 68.0, 72.0, 76.0,
                     80.0, 88.0, 96.0, 104.0, 112.0, 120.0, 128.0, 136.0, 144.0
                     };
            }
        }
               
        static CustomRichTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomRichTextBox), new FrameworkPropertyMetadata(typeof(RichTextBox)));
        }

        public CustomRichTextBox(TextEditor textEditor)
        {
            TextEditor = textEditor;

            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                Style s = new System.Windows.Style(typeof(ComboBoxItem));
                EventSetter es = new EventSetter(ComboBoxItem.PreviewMouseDownEvent, new MouseButtonEventHandler(ComboBoxItemFontSize));
                s.Setters.Add(es);

                ComboBoxFontSize.ItemContainerStyle = s;
                
                FontUnderline.Click += FontUnderline_Click;
                FontStrikethrough.Click += FontStrikethrough_Click;
                FontOverline.Click += FontOverline_Click;
                FontBold.Click += FontBold_Click;
                FontItalic.Click += FontItalic_Click;
                FontSuperscript.Click += FontSuperscript_Click;
                FontSubscript.Click += FontSubscript_Click;
                FontSizeUp.Click += FontSizeUp_Click;
                FontSizeDown.Click += FontSizeDown_Click;
                AlignCenter.Click += AlignCenter_Click;
                AlignLeft.Click += AlignLeft_Click;
                AlignRight.Click += AlignRight_Click;
                AlignFull.Click += AlignFull_Click;
                ButtonPaste.Click += ButtonPaste_Click;
                FontTextBox = ComboBoxFont.Template.FindName("PART_EditableTextBox", ComboBoxFont) as TextBox;                
                FontTextBox.GotKeyboardFocus += FontTextBox_GotKeyboardFocus;
                FontTextBox.PreviewKeyDown += FontTextBox_PreviewKeyDown;
                ComboBoxFont.DropDownClosed += ComboBoxFont_DropDownClosed;             
                ComboBoxFont.PreviewTextInput += ComboBoxFont_PreviewTextInput;              
                ComboBoxFontSize.AddHandler(PreviewKeyDownEvent, new RoutedEventHandler(ComboBoxFontSizePreviewKeyDown), true);
                ComboBoxFontSize.DropDownClosed += ComboBoxFontSize_DropDownClosed;
                FontSizeTextBox = ComboBoxFontSize.Template.FindName("PART_EditableTextBox", ComboBoxFontSize) as TextBox; 
                FontSizeTextBox.GotKeyboardFocus += FontSizeTextBox_GotKeyboardFocus;
                ButtonSaveFile.Click += ButtonSaveFile_Click;
                ButtonOpenFile.Click += ButtonOpenFile_Click;
                
                MenuItem menuItemPasteFontSize = new MenuItem();
                menuItemPasteFontSize.Command = ApplicationCommands.Paste;

                MenuItem menuItemCopyFontSize = new MenuItem();
                menuItemCopyFontSize.Command = ApplicationCommands.Copy;

                MenuItem menuItemPasteFont = new MenuItem();
                menuItemPasteFont.Command = ApplicationCommands.Paste;

                MenuItem menuItemCopyFont = new MenuItem();
                menuItemCopyFont.Command = ApplicationCommands.Copy;

                ContextMenu ContextMenuFontSize = new System.Windows.Controls.ContextMenu();
                ContextMenuFontSize.Items.Add(menuItemPasteFontSize);
                ContextMenuFontSize.Items.Add(menuItemCopyFontSize);

                ContextMenu ContextMenuFont = new System.Windows.Controls.ContextMenu();
                ContextMenuFont.Items.Add(menuItemPasteFont);
                ContextMenuFont.Items.Add(menuItemCopyFont);

                FontSizeTextBox.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
                FontSizeTextBox.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, FontSizeTextBoxPaste));
                FontSizeTextBox.ContextMenu = ContextMenuFontSize;

                FontTextBox.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, IgnoreCut));
                FontTextBox.ContextMenu = ContextMenuFont;
            }));

            ContextMenu contextMenu = new ContextMenu();

            Binding BindingInsertText = new Binding();
            BindingInsertText.Source = Application.Current.MainWindow;
            BindingInsertText.Path = new PropertyPath("IsBindingInsertText");
            BindingInsertText.Mode = BindingMode.OneWay;

            MenuItem menuItemPaste = new MenuItem();
            menuItemPaste.Header = "Вставить";
            menuItemPaste.Click += menuItemPaste_Click;
            menuItemPaste.SetBinding(MenuItem.IsEnabledProperty, BindingInsertText);

            contextMenu.Items.Add(menuItemPaste);

            this.ContextMenu = contextMenu;
        }

        void ButtonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            string s = Assembly.GetExecutingAssembly().Location;

            OpenFileDialog dialogOpen = new OpenFileDialog();
            dialogOpen.AddExtension = true;
            dialogOpen.CheckFileExists = true;
            dialogOpen.CheckPathExists = true;
            dialogOpen.Filter = "Текстовый объект(*.text)|*.text";
            dialogOpen.InitialDirectory = s;
            dialogOpen.Title = "Открыть текстовый объект";

            if (dialogOpen.ShowDialog() == true)
            {
                using (FileStream fs = new FileStream(dialogOpen.FileName, FileMode.Open))
                {
                    Document = (FlowDocument)XamlReader.Load(fs);

                    TextEditor.IsSave = true;
                }
            }

            e.Handled = true;
        }

        void ButtonSaveFile_Click(object sender, RoutedEventArgs e)
        {
            string s = Assembly.GetExecutingAssembly().Location;

            SaveFileDialog dialogSave = new SaveFileDialog();
            dialogSave.AddExtension = true;
            dialogSave.CheckPathExists = true;
            dialogSave.Filter = "Текстовый объект(*.text)|*.text";
            dialogSave.InitialDirectory = s;
            dialogSave.OverwritePrompt = true;
            dialogSave.Title = "Сохранить как...";

            if (dialogSave.ShowDialog() == true)
            {
                using (FileStream fs = new FileStream(dialogSave.FileName, FileMode.Create))
                {
                    XamlWriter.Save(this.Document, fs);
                }
            }

            e.Handled = true;
        }

        private void ComboBoxItemFontSize(object sender, MouseEventArgs e)
        {
            PresentationSource source = PresentationSource.FromVisual(this);

            double dpiX = 0, dpiY = 0;
            if (source != null)
            {
                dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
            }

            double d = dpiX / 72;

            double dText;

            if (e.Source is Border)
            {
                dText = (double)((ContentPresenter)((Border)e.Source).Child).Content;
            }
            else
            {
                dText = (double)((ContentPresenter)e.Source).Content;
            }

            if (!Selection.IsEmpty)
            {                                    
                Selection.ApplyPropertyValue(Inline.FontSizeProperty, dText * d);

                ComboBoxFontSize.Text = dText.ToString();                               
            }
            else
            {
                TextPointer textPoint = CaretPosition;
                startTextPointer = null;
                endTextPointer = null;
               
                TextElement textElement = null;

                if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.None)
                {
                    IsCreateRunNull = true;

                    CreateRunTextPointer = CaretPosition;

                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
                    })); 

                    return;
                }
                else if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementStart)
                {
                    textElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                    if (textElement is Paragraph)
                    {
                        IsCreateRunNull = true;

                        CreateRunTextPointer = CaretPosition;

                        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
                        })); 

                        return;
                    }
                    else if (textElement is ListItem)
                    {
                        IsCreateRunNull = true;

                        CreateRunTextPointer = CaretPosition;

                        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
                        })); 

                        return;
                    }
                }
                else if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementEnd)
                {
                    textElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                    if (textElement is LineBreak)
                    {
                        IsCreateRunNull = true;

                        CreateRunTextPointer = CaretPosition;

                        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
                        })); 
                       
                        return;
                    }
                }

                if (textPoint.GetTextRunLength(LogicalDirection.Backward) > 0) // Если позади каретки в текстовом элементе есть текст
                {
                    TextPointer textPointer = textPoint.GetNextContextPosition(LogicalDirection.Backward);

                    Run run = (Run)textPointer.GetAdjacentElement(LogicalDirection.Backward);

                    TextElement nextBackTextElement = (TextElement)run.ElementStart.GetAdjacentElement(LogicalDirection.Backward);

                    int i = CaretPosition.GetTextRunLength(LogicalDirection.Backward);

                    TextRange textRange = new TextRange(textPointer, CaretPosition);

                    int index = textRange.Text.LastIndexOfAny(new char[] { ' ', ',', '.', '?', '!', ':', ';' });

                    if (index != -1)
                    {
                        if (textRange.Text[i - 1] != ' ' && textRange.Text[i - 1] != ',' && textRange.Text[i - 1] != '.' && textRange.Text[i - 1] != '?' && textRange.Text[i - 1] != '!' && textRange.Text[i - 1] != ':' && textRange.Text[i - 1] != ';')
                        {
                            startTextPointer = CaretPosition.GetPositionAtOffset(index + 1 - i);
                        }
                        else
                        {
                            startTextPointer = null;
                        }
                    }
                    else
                    {
                        IsText = true;

                        StartTextPointer(nextBackTextElement);
                    }
                }
                else
                {
                    IsTextNull = true;

                    TextElement nextBackTextElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                    if (nextBackTextElement is Run)
                    {
                        nextBackTextElement = (TextElement)nextBackTextElement.ElementStart.GetAdjacentElement(LogicalDirection.Backward);
                    }

                    StartTextPointer(nextBackTextElement);
                }

                IsTextNull = false;

                IsText = false;

                if (textPoint.GetTextRunLength(LogicalDirection.Forward) > 0) // Если в текстовом элементе впереди каретки есть текст
                {
                    TextPointer textPointer = textPoint.GetNextContextPosition(LogicalDirection.Forward);

                    Run run = (Run)textPointer.GetAdjacentElement(LogicalDirection.Forward);

                    TextElement nextForwardTextElement = (TextElement)run.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);

                    int i = CaretPosition.GetTextRunLength(LogicalDirection.Forward);

                    TextRange textRange = new TextRange(CaretPosition, textPointer);

                    int index = textRange.Text.IndexOfAny(new char[] { ' ', ',', '.', '?', '!', ':', ';' });

                    if (index != -1)
                    {
                        if (textRange.Text[0] != ' ' && textRange.Text[0] != ',' && textRange.Text[0] != '.' && textRange.Text[0] != '?' && textRange.Text[0] != '!' && textRange.Text[0] != ':' && textRange.Text[0] != ';')
                        {
                            endTextPointer = CaretPosition.GetPositionAtOffset(index);
                        }
                        else
                        {
                            endTextPointer = null;
                        }
                    }
                    else
                    {
                        IsText = true;

                        EndTextPointer(nextForwardTextElement);
                    }
                }
                else
                {
                    IsTextNull = true;

                    TextElement nextForwardTextElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Forward);

                    if (nextForwardTextElement is Run)
                    {
                        nextForwardTextElement = (TextElement)nextForwardTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);
                    }

                    EndTextPointer(nextForwardTextElement);
                }

                IsText = false;

                IsTextNull = false;

                if (startTextPointer != null && endTextPointer != null)
                {
                    TextRange textRange = new TextRange(startTextPointer, endTextPointer);                   
                    textRange.ApplyPropertyValue(Inline.FontSizeProperty, dText * d);

                    ComboBoxFontSize.Text = dText.ToString();   
                }
                else
                {
                    if (dText != Run.FontSize / d)
                    {
                        IsCreateRunSize = true;

                        CreateRunTextPointer = CaretPosition;
                    }
                    else
                    {
                        IsCreateRunSize = false;
                    }
                }                
            }

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }));           
        }

        private void ComboBoxFontSizePreviewKeyDown(object sender, RoutedEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Escape))
            {
                e.Handled = true;
                              
                ComboBoxFontSize.Text = CorrectFontSize.ToString();
                
                this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }
            else if (Keyboard.IsKeyDown(Key.Enter))
            {
                string s = FontSizeTextBox.Text;

                e.Handled = true;

                double d;

                if (double.TryParse(FontSizeTextBox.Text, out d))
                {
                    if (d > 200)
                    {
                        ComboBoxFontSize.Text = CorrectFontSize.ToString();

                        MessageBox.Show("Размер шрифта не может быть больше 200", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                        this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

                        return;
                    }
                    else if (d < 1)
                    {                       
                        ComboBoxFontSize.Text = CorrectFontSize.ToString();

                        MessageBox.Show("Размер шрифта не может быть меньше 1", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                        this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

                        return;
                    }
                }
                else
                {
                    ComboBoxFontSize.Text = CorrectFontSize.ToString();

                    MessageBox.Show("Неверный формат числа", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                    this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

                    return;
                }
                               
                PresentationSource source = PresentationSource.FromVisual(this);

                double dpiX = 0, dpiY = 0;
                if (source != null)
                {
                    dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                    dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
                }

                d = dpiX / 72;

                double dText = double.Parse(ComboBoxFontSize.Text);

                if (!Selection.IsEmpty)
                {                   
                    Selection.ApplyPropertyValue(Inline.FontSizeProperty, dText * d);                   
                }
                else
                {
                    TextPointer textPoint = CaretPosition;
                    startTextPointer = null;
                    endTextPointer = null;

                    if (Selection.IsEmpty)
                    {
                        TextElement textElement = null;

                        if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.None)
                        {
                            IsCreateRunNull = true;

                            CreateRunTextPointer = CaretPosition;

                            this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

                            return;
                        }
                        else if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementStart)
                        {
                            textElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                            if (textElement is Paragraph)
                            {
                                IsCreateRunNull = true;

                                CreateRunTextPointer = CaretPosition;

                                this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

                                return;
                            }
                            else if (textElement is ListItem)
                            {
                                IsCreateRunNull = true;

                                CreateRunTextPointer = CaretPosition;

                                this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

                                return;
                            }
                        }
                        else if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementEnd)
                        {
                            textElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                            if (textElement is LineBreak)
                            {
                                IsCreateRunNull = true;

                                CreateRunTextPointer = CaretPosition;

                                this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

                                return;
                            }
                        }

                        if (textPoint.GetTextRunLength(LogicalDirection.Backward) > 0) // Если позади каретки в текстовом элементе есть текст
                        {
                            TextPointer textPointer = textPoint.GetNextContextPosition(LogicalDirection.Backward);

                            Run run = (Run)textPointer.GetAdjacentElement(LogicalDirection.Backward);

                            TextElement nextBackTextElement = (TextElement)run.ElementStart.GetAdjacentElement(LogicalDirection.Backward);

                            int i = CaretPosition.GetTextRunLength(LogicalDirection.Backward);

                            TextRange textRange = new TextRange(textPointer, CaretPosition);

                            int index = textRange.Text.LastIndexOfAny(new char[] { ' ', ',', '.', '?', '!', ':', ';' });

                            if (index != -1)
                            {
                                if (textRange.Text[i - 1] != ' ' && textRange.Text[i - 1] != ',' && textRange.Text[i - 1] != '.' && textRange.Text[i - 1] != '?' && textRange.Text[i - 1] != '!' && textRange.Text[i - 1] != ':' && textRange.Text[i - 1] != ';')
                                {
                                    startTextPointer = CaretPosition.GetPositionAtOffset(index + 1 - i);
                                }
                                else
                                {
                                    startTextPointer = null;
                                }
                            }
                            else
                            {
                                IsText = true;

                                StartTextPointer(nextBackTextElement);
                            }
                        }
                        else
                        {
                            IsTextNull = true;

                            TextElement nextBackTextElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                            if (nextBackTextElement is Run)
                            {
                                nextBackTextElement = (TextElement)nextBackTextElement.ElementStart.GetAdjacentElement(LogicalDirection.Backward);
                            }

                            StartTextPointer(nextBackTextElement);
                        }

                        IsTextNull = false;

                        IsText = false;

                        if (textPoint.GetTextRunLength(LogicalDirection.Forward) > 0) // Если в текстовом элементе впереди каретки есть текст
                        {
                            TextPointer textPointer = textPoint.GetNextContextPosition(LogicalDirection.Forward);

                            Run run = (Run)textPointer.GetAdjacentElement(LogicalDirection.Forward);

                            TextElement nextForwardTextElement = (TextElement)run.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);

                            int i = CaretPosition.GetTextRunLength(LogicalDirection.Forward);

                            TextRange textRange = new TextRange(CaretPosition, textPointer);

                            int index = textRange.Text.IndexOfAny(new char[] { ' ', ',', '.', '?', '!', ':', ';' });

                            if (index != -1)
                            {
                                if (textRange.Text[0] != ' ' && textRange.Text[0] != ',' && textRange.Text[0] != '.' && textRange.Text[0] != '?' && textRange.Text[0] != '!' && textRange.Text[0] != ':' && textRange.Text[0] != ';')
                                {
                                    endTextPointer = CaretPosition.GetPositionAtOffset(index);
                                }
                                else
                                {
                                    endTextPointer = null;
                                }
                            }
                            else
                            {
                                IsText = true;

                                EndTextPointer(nextForwardTextElement);
                            }
                        }
                        else
                        {
                            IsTextNull = true;

                            TextElement nextForwardTextElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Forward);

                            if (nextForwardTextElement is Run)
                            {
                                nextForwardTextElement = (TextElement)nextForwardTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);
                            }

                            EndTextPointer(nextForwardTextElement);
                        }

                        IsText = false;

                        IsTextNull = false;
                        
                        if (startTextPointer != null && endTextPointer != null)
                        {
                            TextRange textRange = new TextRange(startTextPointer, endTextPointer);                          
                            textRange.ApplyPropertyValue(Inline.FontSizeProperty, dText * d);                                                                                                                                                 
                        }
                        else
                        {                               
                            if (dText != Run.FontSize / d)
                            {
                                IsCreateRunSize = true;

                                CreateRunTextPointer = CaretPosition;
                            }
                            else
                            {
                                IsCreateRunSize = false;
                            }                                                          
                        }
                    }
                }

                this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));                
            }
        }
       
        private void FontSizeTextBoxPaste(object sender, ExecutedRoutedEventArgs e)
        {
            string pattern = @"^\d{1,3}(?:\.\d{0,1})?$";
            string s;

            if (FontSizeTextBox.SelectionLength > 0)
            {
                s = FontSizeTextBox.Text.Remove(FontSizeTextBox.SelectionStart, FontSizeTextBox.SelectionLength);
                s = s.Insert(FontSizeTextBox.SelectionStart, Clipboard.GetText());
            }
            else
            {
                s = FontSizeTextBox.Text.Insert(FontSizeTextBox.Text.Length, Clipboard.GetText());
            }

            double d = 0;

            if (!Regex.IsMatch(s, pattern))
            {
                e.Handled = true;
            }
            else if (double.TryParse(s, out d))
            {
                if (d < 1 || d > 200)
                {
                    e.Handled = true;
                }
                else
                {
                    FontSizeTextBox.Paste();
                }
            }           
        }

        private void IgnoreCut(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        void FontSizeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string pattern = @"^\d{1,3}(?:\.\d{0,1})?$";
            string s;

            if (FontSizeTextBox.SelectionLength > 0)
            {
                s = FontSizeTextBox.Text.Remove(FontSizeTextBox.SelectionStart, FontSizeTextBox.SelectionLength);
                s = s.Insert(FontSizeTextBox.SelectionStart, e.Text);
            }
            else
            {
                s = FontSizeTextBox.Text.Insert(FontSizeTextBox.Text.Length, e.Text);
            }

            double d = 0;
                       
            if (!Regex.IsMatch(s, pattern))
            {
                e.Handled = true;
            }

            if (double.TryParse(s, out d))
            {
                if (d < 1 || d > 200)
                {
                    e.Handled = true;
                }
            }
        }
        
        void FontSizeDown_Click(object sender, RoutedEventArgs e)
        {
            if (!Selection.IsEmpty)
            {
                object obj = Selection.GetPropertyValue(Inline.FontSizeProperty);

                if (obj != DependencyProperty.UnsetValue)
                {
                    PresentationSource source = PresentationSource.FromVisual(this);

                    double dpiX = 0, dpiY = 0;
                    if (source != null)
                    {
                        dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                        dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
                    }

                    double d = (double)obj;

                    double d2 = dpiX / 72;

                    double fontSizePixel = d / d2;

                    fontSizePixel = fontSizePixel - 1;

                    if (fontSizePixel >= 1)
                    {                      
                        Selection.ApplyPropertyValue(Inline.FontSizeProperty, fontSizePixel * d2);

                        ComboBoxFontSize.Text = fontSizePixel.ToString();
                    }
                }
            }
            else
            {
                TextPointer textPoint = CaretPosition;
                startTextPointer = null;
                endTextPointer = null;

                if (Selection.IsEmpty)
                {
                    TextElement textElement = null;

                    if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.None)
                    {
                        IsCreateRunNull = true;

                        CreateRunTextPointer = CaretPosition;                       

                        return;
                    }
                    else if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementStart)
                    {
                        textElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                        if (textElement is Paragraph)
                        {
                            IsCreateRunNull = true;

                            CreateRunTextPointer = CaretPosition;                            

                            return;
                        }
                        else if (textElement is ListItem)
                        {
                            IsCreateRunNull = true;

                            CreateRunTextPointer = CaretPosition;

                            return;
                        }
                    }
                    else if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementEnd)
                    {
                        textElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                        if (textElement is LineBreak)
                        {
                            IsCreateRunNull = true;

                            CreateRunTextPointer = CaretPosition;

                            return;
                        }
                    }

                    if (textPoint.GetTextRunLength(LogicalDirection.Backward) > 0) // Если позади каретки в текстовом элементе есть текст
                    {
                        TextPointer textPointer = textPoint.GetNextContextPosition(LogicalDirection.Backward);

                        Run run = (Run)textPointer.GetAdjacentElement(LogicalDirection.Backward);

                        TextElement nextBackTextElement = (TextElement)run.ElementStart.GetAdjacentElement(LogicalDirection.Backward);

                        int i = CaretPosition.GetTextRunLength(LogicalDirection.Backward);

                        TextRange textRange = new TextRange(textPointer, CaretPosition);

                        int index = textRange.Text.LastIndexOfAny(new char[] { ' ', ',', '.', '?', '!', ':', ';' });

                        if (index != -1)
                        {
                            if (textRange.Text[i - 1] != ' ' && textRange.Text[i - 1] != ',' && textRange.Text[i - 1] != '.' && textRange.Text[i - 1] != '?' && textRange.Text[i - 1] != '!' && textRange.Text[i - 1] != ':' && textRange.Text[i - 1] != ';')
                            {
                                startTextPointer = CaretPosition.GetPositionAtOffset(index + 1 - i);
                            }
                            else
                            {
                                startTextPointer = null;
                            }
                        }
                        else
                        {
                            IsText = true;

                            StartTextPointer(nextBackTextElement);
                        }
                    }
                    else
                    {
                        IsTextNull = true;

                        TextElement nextBackTextElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                        if (nextBackTextElement is Run)
                        {
                            nextBackTextElement = (TextElement)nextBackTextElement.ElementStart.GetAdjacentElement(LogicalDirection.Backward);
                        }

                        StartTextPointer(nextBackTextElement);
                    }

                    IsTextNull = false;

                    IsText = false;

                    if (textPoint.GetTextRunLength(LogicalDirection.Forward) > 0) // Если в текстовом элементе впереди каретки есть текст
                    {
                        TextPointer textPointer = textPoint.GetNextContextPosition(LogicalDirection.Forward);

                        Run run = (Run)textPointer.GetAdjacentElement(LogicalDirection.Forward);

                        TextElement nextForwardTextElement = (TextElement)run.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);

                        int i = CaretPosition.GetTextRunLength(LogicalDirection.Forward);

                        TextRange textRange = new TextRange(CaretPosition, textPointer);

                        int index = textRange.Text.IndexOfAny(new char[] { ' ', ',', '.', '?', '!', ':', ';' });

                        if (index != -1)
                        {
                            if (textRange.Text[0] != ' ' && textRange.Text[0] != ',' && textRange.Text[0] != '.' && textRange.Text[0] != '?' && textRange.Text[0] != '!' && textRange.Text[0] != ':' && textRange.Text[0] != ';')
                            {
                                endTextPointer = CaretPosition.GetPositionAtOffset(index);
                            }
                            else
                            {
                                endTextPointer = null;
                            }
                        }
                        else
                        {
                            IsText = true;

                            EndTextPointer(nextForwardTextElement);
                        }
                    }
                    else
                    {
                        IsTextNull = true;

                        TextElement nextForwardTextElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Forward);

                        if (nextForwardTextElement is Run)
                        {
                            nextForwardTextElement = (TextElement)nextForwardTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);
                        }

                        EndTextPointer(nextForwardTextElement);
                    }

                    IsText = false;

                    IsTextNull = false;
                 
                    if (startTextPointer != null && endTextPointer != null)
                    {
                        TextRange textRange = new TextRange(startTextPointer, endTextPointer);

                        object obj = textRange.GetPropertyValue(Inline.FontSizeProperty);

                        if (obj != DependencyProperty.UnsetValue)
                        {
                            PresentationSource source = PresentationSource.FromVisual(this);

                            double dpiX = 0, dpiY = 0;
                            if (source != null)
                            {
                                dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                                dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
                            }

                            double d = dpiX / 72;

                            if (CorrectFontSize - 1 >= 1)
                            {
                                Selection.ApplyPropertyValue(Inline.FontSizeProperty, (CorrectFontSize - 1) * d);

                                CorrectFontSize = CorrectFontSize - 1;

                                ComboBoxFontSize.Text = CorrectFontSize.ToString();
                            }
                        }                       
                    }                    
                }
            }

            e.Handled = true;
        }

        void FontSizeUp_Click(object sender, RoutedEventArgs e)
        {
            if (!Selection.IsEmpty)
            {
                object obj = Selection.GetPropertyValue(Inline.FontSizeProperty);

                if (obj != DependencyProperty.UnsetValue)
                {
                    PresentationSource source = PresentationSource.FromVisual(this);

                    double dpiX = 0, dpiY = 0;
                    if (source != null)
                    {
                        dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                        dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
                    }

                    double d = (double)obj;

                    double d2 = dpiX / 72;

                    double fontSizePixel = d / d2;

                    fontSizePixel = fontSizePixel + 1;

                    if (fontSizePixel <= 200)
                    {
                        Selection.ApplyPropertyValue(Inline.FontSizeProperty, fontSizePixel * d2);

                        ComboBoxFontSize.Text = fontSizePixel.ToString();
                    }
                }
            }
            else
            {
                TextPointer textPoint = CaretPosition;
                startTextPointer = null;
                endTextPointer = null;

                if (Selection.IsEmpty)
                {
                    TextElement textElement = null;

                    if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.None)
                    {
                        IsCreateRunNull = true;

                        CreateRunTextPointer = CaretPosition;

                        return;
                    }
                    else if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementStart)
                    {
                        textElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                        if (textElement is Paragraph)
                        {
                            IsCreateRunNull = true;

                            CreateRunTextPointer = CaretPosition;

                            return;
                        }
                        else if (textElement is ListItem)
                        {
                            IsCreateRunNull = true;

                            CreateRunTextPointer = CaretPosition;

                            return;
                        }
                    }
                    else if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementEnd)
                    {
                        textElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                        if (textElement is LineBreak)
                        {
                            IsCreateRunNull = true;

                            CreateRunTextPointer = CaretPosition;

                            return;
                        }
                    }

                    if (textPoint.GetTextRunLength(LogicalDirection.Backward) > 0) // Если позади каретки в текстовом элементе есть текст
                    {
                        TextPointer textPointer = textPoint.GetNextContextPosition(LogicalDirection.Backward);

                        Run run = (Run)textPointer.GetAdjacentElement(LogicalDirection.Backward);

                        TextElement nextBackTextElement = (TextElement)run.ElementStart.GetAdjacentElement(LogicalDirection.Backward);

                        int i = CaretPosition.GetTextRunLength(LogicalDirection.Backward);

                        TextRange textRange = new TextRange(textPointer, CaretPosition);

                        int index = textRange.Text.LastIndexOfAny(new char[] { ' ', ',', '.', '?', '!', ':', ';' });

                        if (index != -1)
                        {
                            if (textRange.Text[i - 1] != ' ' && textRange.Text[i - 1] != ',' && textRange.Text[i - 1] != '.' && textRange.Text[i - 1] != '?' && textRange.Text[i - 1] != '!' && textRange.Text[i - 1] != ':' && textRange.Text[i - 1] != ';')
                            {
                                startTextPointer = CaretPosition.GetPositionAtOffset(index + 1 - i);
                            }
                            else
                            {
                                startTextPointer = null;
                            }
                        }
                        else
                        {
                            IsText = true;

                            StartTextPointer(nextBackTextElement);
                        }
                    }
                    else
                    {
                        IsTextNull = true;

                        TextElement nextBackTextElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                        if (nextBackTextElement is Run)
                        {
                            nextBackTextElement = (TextElement)nextBackTextElement.ElementStart.GetAdjacentElement(LogicalDirection.Backward);
                        }

                        StartTextPointer(nextBackTextElement);
                    }

                    IsTextNull = false;

                    IsText = false;

                    if (textPoint.GetTextRunLength(LogicalDirection.Forward) > 0) // Если в текстовом элементе впереди каретки есть текст
                    {
                        TextPointer textPointer = textPoint.GetNextContextPosition(LogicalDirection.Forward);

                        Run run = (Run)textPointer.GetAdjacentElement(LogicalDirection.Forward);

                        TextElement nextForwardTextElement = (TextElement)run.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);

                        int i = CaretPosition.GetTextRunLength(LogicalDirection.Forward);

                        TextRange textRange = new TextRange(CaretPosition, textPointer);

                        int index = textRange.Text.IndexOfAny(new char[] { ' ', ',', '.', '?', '!', ':', ';' });

                        if (index != -1)
                        {
                            if (textRange.Text[0] != ' ' && textRange.Text[0] != ',' && textRange.Text[0] != '.' && textRange.Text[0] != '?' && textRange.Text[0] != '!' && textRange.Text[0] != ':' && textRange.Text[0] != ';')
                            {
                                endTextPointer = CaretPosition.GetPositionAtOffset(index);
                            }
                            else
                            {
                                endTextPointer = null;
                            }
                        }
                        else
                        {
                            IsText = true;

                            EndTextPointer(nextForwardTextElement);
                        }
                    }
                    else
                    {
                        IsTextNull = true;

                        TextElement nextForwardTextElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Forward);

                        if (nextForwardTextElement is Run)
                        {
                            nextForwardTextElement = (TextElement)nextForwardTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);
                        }

                        EndTextPointer(nextForwardTextElement);
                    }

                    IsText = false;

                    IsTextNull = false;

                    if (startTextPointer != null && endTextPointer != null)
                    {
                        TextRange textRange = new TextRange(startTextPointer, endTextPointer);

                        object obj = textRange.GetPropertyValue(Inline.FontSizeProperty);

                        PresentationSource source = PresentationSource.FromVisual(this);

                        double dpiX = 0, dpiY = 0;
                        if (source != null)
                        {
                            dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                            dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
                        }
                       
                        double d2 = dpiX / 72;
                        
                        if (obj != DependencyProperty.UnsetValue)
                        {
                            if (CorrectFontSize + 1 <= 200)
                            {
                                Selection.ApplyPropertyValue(Inline.FontSizeProperty, (CorrectFontSize + 1) * d2);

                                CorrectFontSize = CorrectFontSize + 1;

                                ComboBoxFontSize.Text = CorrectFontSize.ToString();
                            }
                        }
                    }
                }
            }

            e.Handled = true;
        }

        void FontTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {                     
            СorrectFont = (FontFamily)ComboBoxFont.SelectedItem;

            e.Handled = true;
        }

        void FontSizeTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {                   
            double d;

            if (double.TryParse(FontSizeTextBox.Text, out d))
            {                   
                PresentationSource source = PresentationSource.FromVisual(this);
                double dpiX = 0, dpiY = 0;
                if (source != null)
                {
                    dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                    dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
                }

                double d2 = dpiX / 72;
                double fontSize = Run.FontSize / d2;

                CorrectFontSize = fontSize;
            }
                                   
            e.Handled = true;
        }
        
        void ComboBoxFont_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ComboBoxFont.IsDropDownOpen = true;
        }
        
        void ComboBoxFontSize_DropDownClosed(object sender, EventArgs e)
        {                       
            this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));         
        }
        
        void FontSubscript_Click(object sender, RoutedEventArgs e)
        {
            FontSuperscript.IsChecked = false;

            TextPointer textPoint = CaretPosition;
            startTextPointer = null;
            endTextPointer = null;

            if (Selection.IsEmpty)
            {
                TextElement textElement = null;

                if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.None)
                {
                    IsCreateRunNull = true;

                    CreateRunTextPointer = CaretPosition;

                    return;
                }
                else if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementStart)
                {
                    textElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                    if (textElement is Paragraph)
                    {
                        IsCreateRunNull = true;

                        CreateRunTextPointer = CaretPosition;

                        return;
                    }
                    else if (textElement is ListItem)
                    {
                        IsCreateRunNull = true;

                        CreateRunTextPointer = CaretPosition;

                        return;
                    }
                }
                else if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementEnd)
                {
                    textElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                    if (textElement is LineBreak)
                    {
                        IsCreateRunNull = true;

                        CreateRunTextPointer = CaretPosition;

                        return;
                    }
                }

                if (textPoint.GetTextRunLength(LogicalDirection.Backward) > 0) // Если позади каретки в текстовом элементе есть текст
                {
                    TextPointer textPointer = textPoint.GetNextContextPosition(LogicalDirection.Backward);

                    Run run = (Run)textPointer.GetAdjacentElement(LogicalDirection.Backward);

                    TextElement nextBackTextElement = (TextElement)run.ElementStart.GetAdjacentElement(LogicalDirection.Backward);

                    int i = CaretPosition.GetTextRunLength(LogicalDirection.Backward);

                    TextRange textRange = new TextRange(textPointer, CaretPosition);

                    int index = textRange.Text.LastIndexOfAny(new char[] { ' ', ',', '.', '?', '!', ':', ';' });

                    if (index != -1)
                    {
                        if (textRange.Text[i - 1] != ' ' && textRange.Text[i - 1] != ',' && textRange.Text[i - 1] != '.' && textRange.Text[i - 1] != '?' && textRange.Text[i - 1] != '!' && textRange.Text[i - 1] != ':' && textRange.Text[i - 1] != ';')
                        {
                            startTextPointer = CaretPosition.GetPositionAtOffset(index + 1 - i);
                        }
                        else
                        {
                            startTextPointer = null;
                        }
                    }
                    else
                    {
                        IsText = true;

                        StartTextPointer(nextBackTextElement);
                    }
                }
                else
                {
                    IsTextNull = true;

                    TextElement nextBackTextElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                    if (nextBackTextElement is Run)
                    {
                        nextBackTextElement = (TextElement)nextBackTextElement.ElementStart.GetAdjacentElement(LogicalDirection.Backward);
                    }

                    StartTextPointer(nextBackTextElement);
                }

                IsTextNull = false;

                IsText = false;

                if (textPoint.GetTextRunLength(LogicalDirection.Forward) > 0) // Если в текстовом элементе впереди каретки есть текст
                {
                    TextPointer textPointer = textPoint.GetNextContextPosition(LogicalDirection.Forward);

                    Run run = (Run)textPointer.GetAdjacentElement(LogicalDirection.Forward);

                    TextElement nextForwardTextElement = (TextElement)run.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);

                    int i = CaretPosition.GetTextRunLength(LogicalDirection.Forward);

                    TextRange textRange = new TextRange(CaretPosition, textPointer);

                    int index = textRange.Text.IndexOfAny(new char[] { ' ', ',', '.', '?', '!', ':', ';' });

                    if (index != -1)
                    {
                        if (textRange.Text[0] != ' ' && textRange.Text[0] != ',' && textRange.Text[0] != '.' && textRange.Text[0] != '?' && textRange.Text[0] != '!' && textRange.Text[0] != ':' && textRange.Text[0] != ';')
                        {
                            endTextPointer = CaretPosition.GetPositionAtOffset(index);
                        }
                        else
                        {
                            endTextPointer = null;
                        }
                    }
                    else
                    {
                        IsText = true;

                        EndTextPointer(nextForwardTextElement);
                    }
                }
                else
                {
                    IsTextNull = true;

                    TextElement nextForwardTextElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Forward);

                    if (nextForwardTextElement is Run)
                    {
                        nextForwardTextElement = (TextElement)nextForwardTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);
                    }

                    EndTextPointer(nextForwardTextElement);
                }

                IsText = false;

                IsTextNull = false;

                if (startTextPointer != null && endTextPointer != null)
                {
                    TextRange textRange = new TextRange(startTextPointer, endTextPointer);

                    object obj = textRange.GetPropertyValue(Inline.BaselineAlignmentProperty);

                    if ((bool)FontSubscript.IsChecked)
                    {
                        if (obj == DependencyProperty.UnsetValue)
                            textRange.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Subscript);
                        else if ((BaselineAlignment)obj != BaselineAlignment.Subscript)
                            textRange.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Subscript);
                    }
                    else
                    {
                        if (obj == DependencyProperty.UnsetValue)
                            textRange.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Baseline);
                        else if ((BaselineAlignment)obj != BaselineAlignment.Baseline)
                            textRange.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Baseline);
                    }
                }
                else
                {
                    if ((bool)FontSubscript.IsChecked)
                    {
                        if (Run.BaselineAlignment != BaselineAlignment.Subscript)
                        {
                            IsCreateRunScript = true;

                            CreateRunTextPointer = CaretPosition;
                        }
                        else
                        {
                            IsCreateRunScript = false;
                        }
                    }
                    else
                    {
                        if (Run.BaselineAlignment != BaselineAlignment.Baseline)
                        {
                            IsCreateRunScript = true;

                            CreateRunTextPointer = CaretPosition;
                        }
                        else
                        {
                            IsCreateRunScript = false;
                        }
                    }
                }
            }
            else
            {
                object obj = Selection.GetPropertyValue(Inline.BaselineAlignmentProperty);

                if ((bool)FontSubscript.IsChecked)
                    {
                        if (obj == DependencyProperty.UnsetValue)
                            Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Subscript);
                        else if ((BaselineAlignment)obj != BaselineAlignment.Subscript)
                            Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Subscript);
                    }
                    else
                    {
                        if (obj == DependencyProperty.UnsetValue)
                            Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Baseline);
                        else if ((BaselineAlignment)obj != BaselineAlignment.Baseline)
                            Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Baseline);
                    }
            }

            e.Handled = true;
        }

        void FontSuperscript_Click(object sender, RoutedEventArgs e)
        {
            FontSubscript.IsChecked = false;

            TextPointer textPoint = CaretPosition;
            startTextPointer = null;
            endTextPointer = null;

            if (Selection.IsEmpty)
            {
                TextElement textElement = null;

                if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.None)
                {
                    IsCreateRunNull = true;

                    CreateRunTextPointer = CaretPosition;

                    return;
                }
                else if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementStart)
                {
                    textElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                    if (textElement is Paragraph)
                    {
                        IsCreateRunNull = true;

                        CreateRunTextPointer = CaretPosition;

                        return;
                    }
                    else if (textElement is ListItem)
                    {
                        IsCreateRunNull = true;

                        CreateRunTextPointer = CaretPosition;

                        return;
                    }
                }
                else if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementEnd)
                {
                    textElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                    if (textElement is LineBreak)
                    {
                        IsCreateRunNull = true;

                        CreateRunTextPointer = CaretPosition;

                        return;
                    }
                }

                if (textPoint.GetTextRunLength(LogicalDirection.Backward) > 0) // Если позади каретки в текстовом элементе есть текст
                {
                    TextPointer textPointer = textPoint.GetNextContextPosition(LogicalDirection.Backward);

                    Run run = (Run)textPointer.GetAdjacentElement(LogicalDirection.Backward);

                    TextElement nextBackTextElement = (TextElement)run.ElementStart.GetAdjacentElement(LogicalDirection.Backward);

                    int i = CaretPosition.GetTextRunLength(LogicalDirection.Backward);

                    TextRange textRange = new TextRange(textPointer, CaretPosition);

                    int index = textRange.Text.LastIndexOfAny(new char[] { ' ', ',', '.', '?', '!', ':', ';' });

                    if (index != -1)
                    {
                        if (textRange.Text[i - 1] != ' ' && textRange.Text[i - 1] != ',' && textRange.Text[i - 1] != '.' && textRange.Text[i - 1] != '?' && textRange.Text[i - 1] != '!' && textRange.Text[i - 1] != ':' && textRange.Text[i - 1] != ';')
                        {
                            startTextPointer = CaretPosition.GetPositionAtOffset(index + 1 - i);
                        }
                        else
                        {
                            startTextPointer = null;
                        }
                    }
                    else
                    {
                        IsText = true;

                        StartTextPointer(nextBackTextElement);
                    }
                }
                else
                {
                    IsTextNull = true;

                    TextElement nextBackTextElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                    if (nextBackTextElement is Run)
                    {
                        nextBackTextElement = (TextElement)nextBackTextElement.ElementStart.GetAdjacentElement(LogicalDirection.Backward);
                    }

                    StartTextPointer(nextBackTextElement);
                }

                IsTextNull = false;

                IsText = false;

                if (textPoint.GetTextRunLength(LogicalDirection.Forward) > 0) // Если в текстовом элементе впереди каретки есть текст
                {
                    TextPointer textPointer = textPoint.GetNextContextPosition(LogicalDirection.Forward);

                    Run run = (Run)textPointer.GetAdjacentElement(LogicalDirection.Forward);

                    TextElement nextForwardTextElement = (TextElement)run.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);

                    int i = CaretPosition.GetTextRunLength(LogicalDirection.Forward);

                    TextRange textRange = new TextRange(CaretPosition, textPointer);

                    int index = textRange.Text.IndexOfAny(new char[] { ' ', ',', '.', '?', '!', ':', ';' });

                    if (index != -1)
                    {
                        if (textRange.Text[0] != ' ' && textRange.Text[0] != ',' && textRange.Text[0] != '.' && textRange.Text[0] != '?' && textRange.Text[0] != '!' && textRange.Text[0] != ':' && textRange.Text[0] != ';')
                        {
                            endTextPointer = CaretPosition.GetPositionAtOffset(index);
                        }
                        else
                        {
                            endTextPointer = null;
                        }
                    }
                    else
                    {
                        IsText = true;

                        EndTextPointer(nextForwardTextElement);
                    }
                }
                else
                {
                    IsTextNull = true;

                    TextElement nextForwardTextElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Forward);

                    if (nextForwardTextElement is Run)
                    {
                        nextForwardTextElement = (TextElement)nextForwardTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);
                    }

                    EndTextPointer(nextForwardTextElement);
                }

                IsText = false;

                IsTextNull = false;

                if (startTextPointer != null && endTextPointer != null)
                {
                    TextRange textRange = new TextRange(startTextPointer, endTextPointer);

                    object obj = textRange.GetPropertyValue(Inline.BaselineAlignmentProperty);

                    if ((bool)FontSuperscript.IsChecked)
                    {
                        if (obj == DependencyProperty.UnsetValue)
                            textRange.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Superscript);
                        else if ((BaselineAlignment)obj != BaselineAlignment.Superscript)
                            textRange.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Superscript);
                    }
                    else
                    {
                        if (obj == DependencyProperty.UnsetValue)
                            textRange.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Baseline);
                        else if ((BaselineAlignment)obj != BaselineAlignment.Baseline)
                            textRange.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Baseline);
                    }
                }
                else
                {
                    if ((bool)FontSuperscript.IsChecked)
                    {
                        if (Run.BaselineAlignment != BaselineAlignment.Superscript)
                        {
                            IsCreateRunScript = true;

                            CreateRunTextPointer = CaretPosition;
                        }
                        else
                        {
                            IsCreateRunScript = false;
                        }
                    }
                    else
                    {
                        if (Run.BaselineAlignment != BaselineAlignment.Baseline)
                        {
                            IsCreateRunScript = true;

                            CreateRunTextPointer = CaretPosition;
                        }
                        else
                        {
                            IsCreateRunScript = false;
                        }
                    }
                }
            }
            else
            {
                object obj = Selection.GetPropertyValue(Inline.BaselineAlignmentProperty);

                if ((bool)FontSuperscript.IsChecked)
                {
                    if (obj == DependencyProperty.UnsetValue)
                        Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Superscript);
                    else if ((BaselineAlignment)obj != BaselineAlignment.Superscript)
                        Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Superscript);
                }
                else
                {
                    if (obj == DependencyProperty.UnsetValue)
                        Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Baseline);
                    else if ((BaselineAlignment)obj != BaselineAlignment.Baseline)
                        Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Baseline);
                }
            }

            e.Handled = true;
        }

        void AlignFull_Click(object sender, RoutedEventArgs e)
        {
            TextPointer textPoint = CaretPosition;

            if (Selection.IsEmpty)
            {
                if (AlignFull.IsChecked == true)
                {
                    textPoint.Paragraph.TextAlignment = TextAlignment.Justify;

                    AlignRight.IsChecked = false;
                    AlignLeft.IsChecked = false;
                    AlignCenter.IsChecked = false;
                }

                AlignFull.IsChecked = true;
            }
            else
            {
                if (Selection.GetPropertyValue(Paragraph.TextAlignmentProperty) == DependencyProperty.UnsetValue)
                {
                    Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Justify);
                }
            }

            e.Handled = true;
        }

        void AlignRight_Click(object sender, RoutedEventArgs e)
        {
            TextPointer textPoint = CaretPosition;

            if (Selection.IsEmpty)
            {
                if (AlignRight.IsChecked == true)
                {
                    textPoint.Paragraph.TextAlignment = TextAlignment.Right;

                    AlignFull.IsChecked = false;
                    AlignLeft.IsChecked = false;
                    AlignCenter.IsChecked = false;
                }

                AlignRight.IsChecked = true;
            }
            else
            {
                if (Selection.GetPropertyValue(Paragraph.TextAlignmentProperty) == DependencyProperty.UnsetValue)
                {
                    Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Right);
                }
            }

            e.Handled = true;
        }

        void AlignLeft_Click(object sender, RoutedEventArgs e)
        {
            TextPointer textPoint = CaretPosition;

            if (Selection.IsEmpty)
            {
                if (AlignLeft.IsChecked == true)
                {
                    textPoint.Paragraph.TextAlignment = TextAlignment.Left;

                    AlignFull.IsChecked = false;
                    AlignCenter.IsChecked = false;
                    AlignRight.IsChecked = false;
                }

                AlignLeft.IsChecked = true;
            }
            else
            {
                if (Selection.GetPropertyValue(Paragraph.TextAlignmentProperty) == DependencyProperty.UnsetValue)
                {
                    Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Left);
                }
            }

            e.Handled = true;
        }

        void AlignCenter_Click(object sender, RoutedEventArgs e)
        {
            TextPointer textPoint = CaretPosition;

            if (Selection.IsEmpty)
            {
                if (AlignCenter.IsChecked == true)
                {
                    textPoint.Paragraph.TextAlignment = TextAlignment.Center;

                    AlignFull.IsChecked = false;
                    AlignLeft.IsChecked = false;
                    AlignRight.IsChecked = false;

                }

                AlignCenter.IsChecked = true;
            }
            else
            {
                if (Selection.GetPropertyValue(Paragraph.TextAlignmentProperty) == DependencyProperty.UnsetValue)
                {
                    Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Center);
                }
            }
            
            e.Handled = true;
        }

        void FontTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == System.Windows.Input.Key.Enter))
            {              
                var CurrentFontText = ComboBoxFont.SelectedItem;

                if (CurrentFontText == null)
                {
                    ComboBoxFont.SelectedItem = СorrectFont;
                }
                else
                {                                               
                    if (!Selection.IsEmpty)
                    {
                        object obj = Selection.GetPropertyValue(Inline.FontFamilyProperty);
                    
                        if (ComboBoxFont.SelectedItem != obj)
                        {
                            Selection.ApplyPropertyValue(Inline.FontFamilyProperty, ComboBoxFont.SelectedItem);
                        }
                    }
                    else
                    {
                        TextPointer textPoint = CaretPosition;
                        startTextPointer = null;
                        endTextPointer = null;
                    
                        if (Selection.IsEmpty)
                        {
                            TextElement textElement = null;
                    
                            if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.None)
                            {
                                IsCreateRunNull = true;
                    
                                CreateRunTextPointer = CaretPosition;
                    
                                this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
                    
                                return;
                            }
                            else if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementStart)
                            {
                                textElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);
                    
                                if (textElement is Paragraph)
                                {
                                    IsCreateRunNull = true;
                    
                                    CreateRunTextPointer = CaretPosition;
                    
                                    this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
                    
                                    return;
                                }
                                else if (textElement is ListItem)
                                {
                                    IsCreateRunNull = true;
                    
                                    CreateRunTextPointer = CaretPosition;
                    
                                    this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
                    
                                    return;
                                }
                            }
                            else if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementEnd)
                            {
                                textElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);
                    
                                if (textElement is LineBreak)
                                {
                                    IsCreateRunNull = true;
                    
                                    CreateRunTextPointer = CaretPosition;
                    
                                    this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
                    
                                    return;
                                }
                            }
                    
                            if (textPoint.GetTextRunLength(LogicalDirection.Backward) > 0) // Если позади каретки в текстовом элементе есть текст
                            {
                                TextPointer textPointer = textPoint.GetNextContextPosition(LogicalDirection.Backward);
                    
                                Run run = (Run)textPointer.GetAdjacentElement(LogicalDirection.Backward);
                    
                                TextElement nextBackTextElement = (TextElement)run.ElementStart.GetAdjacentElement(LogicalDirection.Backward);
                    
                                int i = CaretPosition.GetTextRunLength(LogicalDirection.Backward);
                    
                                TextRange textRange = new TextRange(textPointer, CaretPosition);
                    
                                int index = textRange.Text.LastIndexOfAny(new char[] { ' ', ',', '.', '?', '!', ':', ';' });
                    
                                if (index != -1)
                                {
                                    if (textRange.Text[i - 1] != ' ' && textRange.Text[i - 1] != ',' && textRange.Text[i - 1] != '.' && textRange.Text[i - 1] != '?' && textRange.Text[i - 1] != '!' && textRange.Text[i - 1] != ':' && textRange.Text[i - 1] != ';')
                                    {
                                        startTextPointer = CaretPosition.GetPositionAtOffset(index + 1 - i);
                                    }
                                    else
                                    {
                                        startTextPointer = null;
                                    }
                                }
                                else
                                {
                                    IsText = true;
                    
                                    StartTextPointer(nextBackTextElement);
                                }
                            }
                            else
                            {
                                IsTextNull = true;
                    
                                TextElement nextBackTextElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);
                    
                                if (nextBackTextElement is Run)
                                {
                                    nextBackTextElement = (TextElement)nextBackTextElement.ElementStart.GetAdjacentElement(LogicalDirection.Backward);
                                }
                    
                                StartTextPointer(nextBackTextElement);
                            }
                    
                            IsTextNull = false;
                    
                            IsText = false;
                    
                            if (textPoint.GetTextRunLength(LogicalDirection.Forward) > 0) // Если в текстовом элементе впереди каретки есть текст
                            {
                                TextPointer textPointer = textPoint.GetNextContextPosition(LogicalDirection.Forward);
                    
                                Run run = (Run)textPointer.GetAdjacentElement(LogicalDirection.Forward);
                    
                                TextElement nextForwardTextElement = (TextElement)run.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);
                    
                                int i = CaretPosition.GetTextRunLength(LogicalDirection.Forward);
                    
                                TextRange textRange = new TextRange(CaretPosition, textPointer);
                    
                                int index = textRange.Text.IndexOfAny(new char[] { ' ', ',', '.', '?', '!', ':', ';' });
                    
                                if (index != -1)
                                {
                                    if (textRange.Text[0] != ' ' && textRange.Text[0] != ',' && textRange.Text[0] != '.' && textRange.Text[0] != '?' && textRange.Text[0] != '!' && textRange.Text[0] != ':' && textRange.Text[0] != ';')
                                    {
                                        endTextPointer = CaretPosition.GetPositionAtOffset(index);
                                    }
                                    else
                                    {
                                        endTextPointer = null;
                                    }
                                }
                                else
                                {
                                    IsText = true;
                    
                                    EndTextPointer(nextForwardTextElement);
                                }
                            }
                            else
                            {
                                IsTextNull = true;
                    
                                TextElement nextForwardTextElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Forward);
                    
                                if (nextForwardTextElement is Run)
                                {
                                    nextForwardTextElement = (TextElement)nextForwardTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);
                                }
                    
                                EndTextPointer(nextForwardTextElement);
                            }
                    
                            IsText = false;
                    
                            IsTextNull = false;
                    
                            if (startTextPointer != null && endTextPointer != null)
                            {
                                TextRange textRange = new TextRange(startTextPointer, endTextPointer);
                    
                                object obj = textRange.GetPropertyValue(Inline.FontFamilyProperty);
                    
                                if (ComboBoxFont.SelectedItem != obj)
                                {
                                    textRange.ApplyPropertyValue(Inline.FontFamilyProperty, ComboBoxFont.SelectedItem);
                                }
                            }
                            else
                            {
                                if (ComboBoxFont.SelectedItem != Run.FontFamily)
                                {
                                    IsCreateRunFont = true;
                    
                                    CreateRunTextPointer = CaretPosition;
                                }
                                else
                                {
                                    IsCreateRunFont = false;
                                }
                            }
                        }
                    }                      
                }                                   
                              
                e.Handled = true;

                this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }
            else if (e.Key == Key.Escape)
            {
                e.Handled = true;

                this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }
        }

        void ComboBoxFont_DropDownClosed(object sender, EventArgs e)
        {           
            ComboBox comboBox = sender as ComboBox;

            var CurrentFontText = comboBox.SelectedItem;

            if (CurrentFontText == null)
            {
                ComboBoxFont.SelectedItem = СorrectFont;
            }
            else
            {                    
                if (!Selection.IsEmpty)
                {
                    object obj = Selection.GetPropertyValue(Inline.FontFamilyProperty);

                    if (ComboBoxFont.SelectedItem != obj)
                    {
                        Selection.ApplyPropertyValue(Inline.FontFamilyProperty, ComboBoxFont.SelectedItem);
                    }
                }
                else
                {
                    TextPointer textPoint = CaretPosition;
                    startTextPointer = null;
                    endTextPointer = null;

                    if (Selection.IsEmpty)
                    {
                        TextElement textElement = null;

                        if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.None)
                        {
                            IsCreateRunNull = true;

                            CreateRunTextPointer = CaretPosition;

                            this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

                            return;
                        }
                        else if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementStart)
                        {
                            textElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                            if (textElement is Paragraph)
                            {
                                IsCreateRunNull = true;

                                CreateRunTextPointer = CaretPosition;

                                this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

                                return;
                            }
                            else if (textElement is ListItem)
                            {
                                IsCreateRunNull = true;

                                CreateRunTextPointer = CaretPosition;

                                this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

                                return;
                            }
                        }
                        else if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementEnd)
                        {
                            textElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                            if (textElement is LineBreak)
                            {
                                IsCreateRunNull = true;

                                CreateRunTextPointer = CaretPosition;

                                this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

                                return;
                            }
                        }

                        if (textPoint.GetTextRunLength(LogicalDirection.Backward) > 0) // Если позади каретки в текстовом элементе есть текст
                        {
                            TextPointer textPointer = textPoint.GetNextContextPosition(LogicalDirection.Backward);

                            Run run = (Run)textPointer.GetAdjacentElement(LogicalDirection.Backward);

                            TextElement nextBackTextElement = (TextElement)run.ElementStart.GetAdjacentElement(LogicalDirection.Backward);

                            int i = CaretPosition.GetTextRunLength(LogicalDirection.Backward);

                            TextRange textRange = new TextRange(textPointer, CaretPosition);

                            int index = textRange.Text.LastIndexOfAny(new char[] { ' ', ',', '.', '?', '!', ':', ';' });

                            if (index != -1)
                            {
                                if (textRange.Text[i - 1] != ' ' && textRange.Text[i - 1] != ',' && textRange.Text[i - 1] != '.' && textRange.Text[i - 1] != '?' && textRange.Text[i - 1] != '!' && textRange.Text[i - 1] != ':' && textRange.Text[i - 1] != ';')
                                {
                                    startTextPointer = CaretPosition.GetPositionAtOffset(index + 1 - i);
                                }
                                else
                                {
                                    startTextPointer = null;
                                }
                            }
                            else
                            {
                                IsText = true;

                                StartTextPointer(nextBackTextElement);
                            }
                        }
                        else
                        {
                            IsTextNull = true;

                            TextElement nextBackTextElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                            if (nextBackTextElement is Run)
                            {
                                nextBackTextElement = (TextElement)nextBackTextElement.ElementStart.GetAdjacentElement(LogicalDirection.Backward);
                            }

                            StartTextPointer(nextBackTextElement);
                        }

                        IsTextNull = false;

                        IsText = false;

                        if (textPoint.GetTextRunLength(LogicalDirection.Forward) > 0) // Если в текстовом элементе впереди каретки есть текст
                        {
                            TextPointer textPointer = textPoint.GetNextContextPosition(LogicalDirection.Forward);

                            Run run = (Run)textPointer.GetAdjacentElement(LogicalDirection.Forward);

                            TextElement nextForwardTextElement = (TextElement)run.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);

                            int i = CaretPosition.GetTextRunLength(LogicalDirection.Forward);

                            TextRange textRange = new TextRange(CaretPosition, textPointer);

                            int index = textRange.Text.IndexOfAny(new char[] { ' ', ',', '.', '?', '!', ':', ';' });

                            if (index != -1)
                            {
                                if (textRange.Text[0] != ' ' && textRange.Text[0] != ',' && textRange.Text[0] != '.' && textRange.Text[0] != '?' && textRange.Text[0] != '!' && textRange.Text[0] != ':' && textRange.Text[0] != ';')
                                {
                                    endTextPointer = CaretPosition.GetPositionAtOffset(index);
                                }
                                else
                                {
                                    endTextPointer = null;
                                }
                            }
                            else
                            {
                                IsText = true;

                                EndTextPointer(nextForwardTextElement);
                            }
                        }
                        else
                        {
                            IsTextNull = true;

                            TextElement nextForwardTextElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Forward);

                            if (nextForwardTextElement is Run)
                            {
                                nextForwardTextElement = (TextElement)nextForwardTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);
                            }

                            EndTextPointer(nextForwardTextElement);
                        }

                        IsText = false;

                        IsTextNull = false;

                        if (startTextPointer != null && endTextPointer != null)
                        {
                            TextRange textRange = new TextRange(startTextPointer, endTextPointer);

                            object obj = textRange.GetPropertyValue(Inline.FontFamilyProperty);

                            if (ComboBoxFont.SelectedItem != obj)
                            {
                                textRange.ApplyPropertyValue(Inline.FontFamilyProperty, ComboBoxFont.SelectedItem);
                            }
                        }
                        else
                        {
                            if (ComboBoxFont.SelectedItem != Run.FontFamily)
                            {
                                IsCreateRunFont = true;

                                CreateRunTextPointer = CaretPosition;
                            }
                            else
                            {
                                IsCreateRunFont = false;
                            }
                        }
                    }
                }                                  
            }
                        
            this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));            
        }

        void menuItemPaste_Click(object sender, RoutedEventArgs e)
        {
            IsPaste = true;

            this.Paste();

            e.Handled = true;
        }
       
        void ButtonPaste_Click(object sender, RoutedEventArgs e)
        {
            IsPaste = true;

            this.Paste();

            e.Handled = true;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            this.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() =>
            {                
                ComboBoxFontSize.ItemsSource = FontSizes;
            }));           
        }
      
        // Цикл связанный с кнопками TextDecoration
        void NextRunCheck(TextElement startTextElement, TextElement endTextElement, bool nextRun, TextPointer startTextPointer, UnderLine underLineCheck, OverLine overLineCheck, Strikethrough strikethroughCheck)
        {
            bool underLine = false;
            bool overLine = false;
            bool strikethrough = false;

            Run run = null;

            TextDecorationCollection textdecCollection = null;

            TextPointer nextTextPointer = startTextPointer;

            TextElement nextTextElement = startTextElement;

            while (nextRun)
            {
                if (nextTextElement == endTextElement)
                {
                    if (underLineCheck.stop)
                    {
                        FontUnderline.IsChecked = false;
                    }
                    else
                    {
                        if (underLineCheck.state)
                        {
                            FontUnderline.IsChecked = true;
                        }
                        else
                        {
                            FontUnderline.IsChecked = false;
                        }
                    }

                    if (strikethroughCheck.stop)
                    {
                        FontStrikethrough.IsChecked = false;
                    }
                    else
                    {
                        if (strikethroughCheck.state)
                        {
                            FontStrikethrough.IsChecked = true;
                        }
                        else
                        {
                            FontStrikethrough.IsChecked = false;
                        }
                    }

                    if (overLineCheck.stop)
                    {
                        FontOverline.IsChecked = false;
                    }
                    else
                    {
                        if (overLineCheck.state)
                        {
                            FontOverline.IsChecked = true;
                        }
                        else
                        {
                            FontOverline.IsChecked = false;
                        }
                    }       

                    return;
                }

                if (nextTextElement is LineBreak) // Только TextPointerContext.StartElement и переходим в начало цикла к следующему элементу
                {
                    nextTextPointer = nextTextElement.ElementEnd;

                    nextTextElement = (TextElement)nextTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);

                    continue;
                }
                else if (nextTextElement is Paragraph || nextTextElement is List || nextTextElement is ListItem)
                {
                    if (nextTextPointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementStart)
                    {
                        nextTextPointer = nextTextElement.ContentStart;

                        nextTextElement = (TextElement)nextTextPointer.GetAdjacentElement(LogicalDirection.Forward);
                    }
                    else if (nextTextPointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementEnd)
                    {
                        nextTextPointer = nextTextElement.ElementEnd;

                        nextTextElement = (TextElement)nextTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);
                    }

                    continue;
                }
                else if (nextTextElement is Run)
                {                    
                    run = (Run)nextTextElement;

                    nextTextPointer = nextTextElement.ElementEnd;

                    nextTextElement = (TextElement)nextTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);

                    textdecCollection = new TextDecorationCollection();

                    foreach (TextDecoration textDecoration in run.TextDecorations)
                    {
                        if (textDecoration.Location == TextDecorationLocation.OverLine)
                            overLine = true;
                        if (textDecoration.Location == TextDecorationLocation.Strikethrough)
                            strikethrough = true;
                        if (textDecoration.Location == TextDecorationLocation.Underline)
                            underLine = true;
                    }

                    if (overLine)
                    {
                        if (!overLineCheck.stop)
                        {
                            if (overLineCheck.start)
                            {
                                overLineCheck.stop = true;
                            }
                            else
                            {
                                overLineCheck.state = true;
                            }
                        }
                    }
                    else
                    {
                        if (!overLineCheck.stop)
                        {
                            if (overLineCheck.state)
                            {
                                overLineCheck.stop = true;
                            }
                        }
                    }

                    if (strikethrough)
                    {
                        if (!strikethroughCheck.stop)
                        {
                            if (strikethroughCheck.start)
                            {
                                strikethroughCheck.stop = true;
                            }
                            else
                            {
                                strikethroughCheck.state = true;
                            }
                        }
                    }
                    else
                    {
                        if (!strikethroughCheck.stop)
                        {
                            if (strikethroughCheck.state)
                            {
                                strikethroughCheck.stop = true;
                            }
                        }
                    }

                    if (underLine)
                    {
                        if (!underLineCheck.stop)
                        {
                            if (underLineCheck.start)
                            {
                                underLineCheck.stop = true;
                            }
                            else
                            {
                                underLineCheck.state = true;
                            }
                        }
                    }
                    else
                    {
                        if (!underLineCheck.stop)
                        {
                            if (underLineCheck.state)
                            {
                                underLineCheck.stop = true;
                            }
                        }
                    }                                                  
                }
            }
        }

        void SelectionEndElemendEndCheck(TextElement startTextElement, TextPointer startTextPointer, UnderLine underLineCheck, OverLine overLineCheck, Strikethrough strikethroughCheck)
        {
            bool nextRun = true;
            TextElement endTextElement = null;

            bool underLine = false;
            bool overLine = false;
            bool strikethrough = false;

            TextDecorationCollection textdecCollection = null;

            if (Selection.End.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementEnd)
            {
                endTextElement = (TextElement)Selection.End.GetAdjacentElement(LogicalDirection.Forward);

                if (endTextElement is Run)
                {
                    Run endRun = endTextElement as Run;

                    textdecCollection = new TextDecorationCollection();

                    foreach (TextDecoration textDecoration in endRun.TextDecorations)
                    {
                        if (textDecoration.Location == TextDecorationLocation.OverLine)
                            overLine = true;
                        if (textDecoration.Location == TextDecorationLocation.Strikethrough)
                            strikethrough = true;
                        if (textDecoration.Location == TextDecorationLocation.Underline)
                            underLine = true;
                    }

                    if (overLine)
                    {
                        if (!overLineCheck.stop)
                        {
                            if (overLineCheck.start)
                            {                            
                                overLineCheck.stop = true;                            
                            }
                            else
                            {
                                overLineCheck.state = true;
                            }                            
                        }
                    }
                    else
                    {
                        if (!overLineCheck.stop)
                        {
                            if (overLineCheck.state)
                            {
                                overLineCheck.stop = true;
                            }
                        }
                    }

                    if (strikethrough)
                    {
                        if (!strikethroughCheck.stop)
                        {
                            if (strikethroughCheck.start)
                            {                        
                                strikethroughCheck.stop = true;                               
                            }
                            else
                            {
                                strikethroughCheck.state = true;
                            }                          
                        }
                    }
                    else
                    {
                        if (!strikethroughCheck.stop)
                        {
                            if (strikethroughCheck.state)
                            {
                                strikethroughCheck.stop = true;
                            }
                        }
                    }

                    if (underLine)
                    {
                        if (!underLineCheck.stop)
                        {
                            if (underLineCheck.start)
                            {
                                underLineCheck.stop = true;
                            }
                            else
                            {
                                underLineCheck.state = true;
                            }                                                      
                        }
                    }
                    else
                    {
                        if (!underLineCheck.stop)
                        {
                            if (underLineCheck.state)
                            {
                                underLineCheck.stop = true;
                            }
                        }
                    }
                }

                if (startTextElement == endTextElement)
                {
                    if (underLineCheck.stop)
                    {
                        FontUnderline.IsChecked = false;
                    }
                    else
                    {
                        if (underLineCheck.state)
                        {
                            FontUnderline.IsChecked = true;
                        }
                        else
                        {
                            FontUnderline.IsChecked = false;
                        }
                    }

                    if (strikethroughCheck.stop)
                    {
                        FontStrikethrough.IsChecked = false;
                    }
                    else
                    {
                        if (strikethroughCheck.state)
                        {
                            FontStrikethrough.IsChecked = true;
                        }
                        else
                        {
                            FontStrikethrough.IsChecked = false;
                        }
                    }

                    if (overLineCheck.stop)
                    {
                        FontOverline.IsChecked = false;
                    }
                    else
                    {
                        if (overLineCheck.state)
                        {
                            FontOverline.IsChecked = true;
                        }
                        else
                        {
                            FontOverline.IsChecked = false;
                        }
                    }

                    return;
                }                  

                NextRunCheck(startTextElement, endTextElement, nextRun, startTextPointer, underLineCheck, overLineCheck, strikethroughCheck);
            }
        }

        void SelectionEndTextCheck(TextElement startTextElement, TextPointer startTextPointer, UnderLine underLineCheck, OverLine overLineCheck, Strikethrough strikethroughCheck)
        {
            TextElement endTextElement = null;

            bool underLine = false;
            bool overLine = false;
            bool strikethrough = false;

            if (Selection.End.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
            {
                bool nextRun = true;

                TextPointer endTextpointer = Selection.End.GetNextContextPosition(LogicalDirection.Backward);

                endTextElement = (TextElement)endTextpointer.GetAdjacentElement(LogicalDirection.Backward);

                TextRange textRange = new TextRange(endTextpointer, Selection.End);

                TextDecorationCollection collection = (TextDecorationCollection)textRange.GetPropertyValue(Inline.TextDecorationsProperty);
                TextDecorationCollection newCollection = new TextDecorationCollection();

                foreach (TextDecoration textDecoration in collection)
                {
                    if (textDecoration.Location == TextDecorationLocation.OverLine)
                        overLine = true;
                    if (textDecoration.Location == TextDecorationLocation.Strikethrough)
                        strikethrough = true;
                    if (textDecoration.Location == TextDecorationLocation.Underline)
                        underLine = true;
                }

                if (overLine)
                {
                    if (!overLineCheck.stop)
                    {
                        if (overLineCheck.start)
                        {
                            overLineCheck.stop = true;
                        }
                        else
                        {
                            overLineCheck.state = true;
                        }
                    }
                }
                else
                {
                    if (!overLineCheck.stop)
                    {
                        if (overLineCheck.state)
                        {
                            overLineCheck.stop = true;
                        }
                    }
                }

                if (strikethrough)
                {
                    if (!strikethroughCheck.stop)
                    {
                        if (strikethroughCheck.start)
                        {
                            strikethroughCheck.stop = true;
                        }
                        else
                        {
                            strikethroughCheck.state = true;
                        }
                    }
                }
                else
                {
                    if (!strikethroughCheck.stop)
                    {
                        if (strikethroughCheck.state)
                        {
                            strikethroughCheck.stop = true;
                        }
                    }
                }

                if (underLine)
                {
                    if (!underLineCheck.stop)
                    {
                        if (underLineCheck.start)
                        {
                            underLineCheck.stop = true;
                        }
                        else
                        {
                            underLineCheck.state = true;
                        }
                    }
                }
                else
                {
                    if (!underLineCheck.stop)
                    {
                        if (underLineCheck.state)
                        {
                            underLineCheck.stop = true;
                        }
                    }
                }

                if (startTextElement == endTextElement)
                {
                    if (underLineCheck.stop)
                    {
                        FontUnderline.IsChecked = false;
                    }
                    else
                    {
                        if (underLineCheck.state)
                        {
                            FontUnderline.IsChecked = true;
                        }
                        else
                        {
                            FontUnderline.IsChecked = false;
                        }
                    }

                    if (strikethroughCheck.stop)
                    {
                        FontStrikethrough.IsChecked = false;
                    }
                    else
                    {
                        if (strikethroughCheck.state)
                        {
                            FontStrikethrough.IsChecked = true;
                        }
                        else
                        {
                            FontStrikethrough.IsChecked = false;
                        }
                    }

                    if (overLineCheck.stop)
                    {
                        FontOverline.IsChecked = false;
                    }
                    else
                    {
                        if (overLineCheck.state)
                        {
                            FontOverline.IsChecked = true;
                        }
                        else
                        {
                            FontOverline.IsChecked = false;
                        }
                    }

                    return;
                }

                NextRunCheck(startTextElement, endTextElement, nextRun, startTextPointer, underLineCheck, overLineCheck, strikethroughCheck);
            }
        }

        void SelectionEndNoneCheck(TextElement startTextElement, TextPointer startTextPointer, UnderLine underLineCheck, OverLine overLineCheck, Strikethrough strikethroughCheck)
        {
            if (Selection.End.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.None)
            {
                TextElement endTextElement = (TextElement)Selection.End.GetAdjacentElement(LogicalDirection.Backward);

                bool nextRun = true;

                if (startTextElement == endTextElement)
                {
                    if (underLineCheck.stop)
                    {
                        FontUnderline.IsChecked = false;
                    }
                    else
                    {
                        if (underLineCheck.state)
                        {
                            FontUnderline.IsChecked = true;
                        }
                        else
                        {
                            FontUnderline.IsChecked = false;
                        }
                    }

                    if (strikethroughCheck.stop)
                    {
                        FontStrikethrough.IsChecked = false;
                    }
                    else
                    {
                        if (strikethroughCheck.state)
                        {
                            FontStrikethrough.IsChecked = true;
                        }
                        else
                        {
                            FontStrikethrough.IsChecked = false;
                        }
                    }

                    if (overLineCheck.stop)
                    {
                        FontOverline.IsChecked = false;
                    }
                    else
                    {
                        if (overLineCheck.state)
                        {
                            FontOverline.IsChecked = true;
                        }
                        else
                        {
                            FontOverline.IsChecked = false;
                        }
                    }

                    return;
                }

                NextRunCheck(startTextElement, endTextElement, nextRun, startTextPointer, underLineCheck, overLineCheck, strikethroughCheck);
            }
        }

        void SelectionEndStartCheck(TextElement startTextElement, TextPointer startTextPointer, UnderLine underLineCheck, OverLine overLineCheck, Strikethrough strikethroughCheck)
        {
            if (Selection.End.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementStart)
            {
                bool nextRun = true;

                TextElement endTextElement = (TextElement)Selection.End.GetAdjacentElement(LogicalDirection.Backward);

                if (startTextElement == endTextElement)
                {
                    if (underLineCheck.stop)
                    {
                        FontUnderline.IsChecked = false;
                    }
                    else
                    {
                        if (underLineCheck.state)
                        {
                            FontUnderline.IsChecked = true;
                        }
                        else
                        {
                            FontUnderline.IsChecked = false;
                        }
                    }

                    if (strikethroughCheck.stop)
                    {
                        FontStrikethrough.IsChecked = false;
                    }
                    else
                    {
                        if (strikethroughCheck.state)
                        {
                            FontStrikethrough.IsChecked = true;
                        }
                        else
                        {
                            FontStrikethrough.IsChecked = false;
                        }
                    }

                    if (overLineCheck.stop)
                    {
                        FontOverline.IsChecked = false;
                    }
                    else
                    {
                        if (overLineCheck.state)
                        {
                            FontOverline.IsChecked = true;
                        }
                        else
                        {
                            FontOverline.IsChecked = false;
                        }
                    }

                    return;
                }

                NextRunCheck(startTextElement, endTextElement, nextRun, startTextPointer, underLineCheck, overLineCheck, strikethroughCheck);
            }
        }

        private void UpdateToolBar()
        {
            TextPointer textPoint = CaretPosition;

            if (Selection.IsEmpty)
            {
                TextElement textElement = null;
                TextElement nextBackTextElement = null;
                TextPointer nextBackTextPointer = null;
                bool nextBack = true;

                if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.None) // Если нету ниодного элемента
                {
                    IsCreateRunNull = true;

                    CreateRunTextPointer = CaretPosition;

                    return;
                }

                if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementStart)
                {
                    textElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);
                 
                    if (textElement is Paragraph)
                    {
                        IsCreateRunNull = true;

                        CreateRunTextPointer = CaretPosition;

                        return;                     
                    }
                    else if (textElement is ListItem)
                    {
                        IsCreateRunNull = true;

                        CreateRunTextPointer = CaretPosition;

                        return;
                    }
                }
                else if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementEnd)
                {
                    textElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                    if (textElement is LineBreak)
                    {
                        IsCreateRunNull = true;

                        CreateRunTextPointer = CaretPosition;

                        return;
                    }
                }
                    
                if (textPoint.GetTextRunLength(LogicalDirection.Backward) == 0)
                {
                    textElement = (TextElement)textPoint.GetAdjacentElement(LogicalDirection.Backward);

                    if (textElement is Paragraph || textElement is LineBreak)
                        return;

                    nextBackTextElement = (TextElement)textElement.ElementStart.GetAdjacentElement(LogicalDirection.Backward);
                    nextBackTextPointer = textElement.ElementStart;
                }
                else
                {
                    TextPointer backwardTextPointer = textPoint.GetNextContextPosition(LogicalDirection.Backward);
                    textElement = (TextElement)backwardTextPointer.GetAdjacentElement(LogicalDirection.Backward);
                    nextBack = false;
                }

                while (nextBack)
                {
                    if (nextBackTextElement is Paragraph)
                    {
                        nextBack = false;
                    }
                    else
                    {
                        if (nextBackTextElement is Span)
                        {
                            if (nextBackTextPointer.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementEnd)
                            {
                                nextBackTextElement = (TextElement)nextBackTextElement.ContentEnd.GetAdjacentElement(LogicalDirection.Backward);
                                nextBackTextPointer = nextBackTextElement.ContentEnd;

                                if (nextBackTextElement is Run)
                                {
                                    Run runBack = nextBackTextElement as Run;
                                    if (runBack.Text.Length > 0)
                                    {
                                        CreateRunTextPointer = CaretPosition;
                                        textElement = nextBackTextElement;
                                        nextBack = false;
                                    }
                                    else
                                    {
                                        nextBackTextElement = (TextElement)nextBackTextElement.ElementStart.GetAdjacentElement(LogicalDirection.Backward);
                                        nextBackTextPointer = nextBackTextElement.ElementStart;
                                    }
                                }
                            }
                            else if (nextBackTextPointer.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementStart)
                            {
                                nextBackTextElement = (TextElement)nextBackTextElement.ElementStart.GetAdjacentElement(LogicalDirection.Backward);
                                nextBackTextPointer = nextBackTextElement.ElementEnd;
                            }
                        }
                        else if (nextBackTextElement is Run)
                        {
                            Run runBack = nextBackTextElement as Run;
                            if (runBack.Text.Length > 0)
                            {
                                if (runBack.Text.LastIndexOf(' ') == (runBack.Text.Length - 1))
                                {
                                    CreateRunTextPointer = CaretPosition;
                                    IsLeftSpace = true;
                                }
                                
                                textElement = nextBackTextElement;
                                nextBack = false;
                            }
                            else
                            {
                                nextBackTextElement = (TextElement)nextBackTextElement.ElementStart.GetAdjacentElement(LogicalDirection.Backward);
                                nextBackTextPointer = nextBackTextElement.ElementStart;
                            }
                        }
                        else if (nextBackTextElement is LineBreak)
                            nextBack = false;
                        else
                        {
                            nextBackTextElement = (TextElement)nextBackTextElement.ElementStart.GetAdjacentElement(LogicalDirection.Backward);
                            nextBackTextPointer = nextBackTextElement.ElementStart;
                        }
                    }
                }

                if (textElement is Run)
                {
                    Run = textElement as Run;

                    if (Run.TextDecorations.Count == 0)
                    {
                        FontUnderline.IsChecked = false;
                        FontStrikethrough.IsChecked = false;
                        FontOverline.IsChecked = false;
                    }
                    else
                    {
                        bool underline = false;
                        bool strikethrough = false;
                        bool overLine = false;

                        foreach (TextDecoration textDecoration in Run.TextDecorations)
                        {
                            if (textDecoration.Location == TextDecorationLocation.Underline)
                            {
                                underline = true;
                            }

                            if (textDecoration.Location == TextDecorationLocation.Strikethrough)
                            {
                                strikethrough = true;
                            }

                            if (textDecoration.Location == TextDecorationLocation.OverLine)
                            {
                                overLine = true;
                            }
                        }

                        if (underline)
                            FontUnderline.IsChecked = true;
                        else
                            FontUnderline.IsChecked = false;

                        if (strikethrough)
                            FontStrikethrough.IsChecked = true;
                        else
                            FontStrikethrough.IsChecked = false;

                        if (overLine)
                            FontOverline.IsChecked = true;
                        else
                            FontOverline.IsChecked = false;
                    }

                    if (Run.BaselineAlignment == BaselineAlignment.Subscript)
                    {
                        if (FontSubscript.IsChecked != true)
                        {
                            FontSubscript.IsChecked = true;
                        }
                    }
                    else if (Run.BaselineAlignment == BaselineAlignment.Superscript)
                    {
                        if (FontSuperscript.IsChecked != true)
                        {
                            FontSuperscript.IsChecked = true;
                        }
                    }
                    else
                    {
                        if (FontSuperscript.IsChecked == true)
                        {
                            FontSuperscript.IsChecked = false;
                        }

                        if (FontSubscript.IsChecked == true)
                        {
                            FontSubscript.IsChecked = false;
                        }
                    }

                    FontFamily currentFontFamily = Run.FontFamily;
                    if (currentFontFamily != null)
                    {
                        ComboBoxFont.SelectedItem = currentFontFamily;
                    }
                    else
                    {
                        ComboBoxFont.SelectedItem = null;
                    }

                    PresentationSource source = PresentationSource.FromVisual(this);

                    double dpiX = 0, dpiY = 0;
                    if (source != null)
                    {
                        dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                        dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
                    }

                    double d2 = dpiX / 72;
                                        
                    ComboBoxFontSize.Text = (Run.FontSize / d2).ToString();
                                                                         
                    if (Run.FontWeight == FontWeights.Bold)
                    {
                        if (FontBold.IsChecked != true)
                            FontBold.IsChecked = true;
                    }
                    else
                    {
                        if (FontBold.IsChecked == true)
                            FontBold.IsChecked = false;
                    }

                    if (Run.FontStyle == FontStyles.Italic)
                    {
                        if (FontItalic.IsChecked != true)
                            FontItalic.IsChecked = true;
                    }
                    else
                    {
                        if (FontItalic.IsChecked == true)
                            FontItalic.IsChecked = false;
                    }

                    Paragraph paragraph = CaretPosition.Paragraph;

                    if (paragraph.TextAlignment == TextAlignment.Center)
                    {
                        AlignCenter.IsChecked = true;
                        AlignFull.IsChecked = false;
                        AlignLeft.IsChecked = false;
                        AlignRight.IsChecked = false;
                    }
                    else if (paragraph.TextAlignment == TextAlignment.Justify)
                    {
                        AlignCenter.IsChecked = false;
                        AlignFull.IsChecked = true;
                        AlignLeft.IsChecked = false;
                        AlignRight.IsChecked = false;
                    }
                    else if (paragraph.TextAlignment == TextAlignment.Left)
                    {
                        AlignCenter.IsChecked = false;
                        AlignFull.IsChecked = false;
                        AlignLeft.IsChecked = true;
                        AlignRight.IsChecked = false;
                    }
                    else if (paragraph.TextAlignment == TextAlignment.Right)
                    {
                        AlignCenter.IsChecked = false;
                        AlignFull.IsChecked = false;
                        AlignLeft.IsChecked = false;
                        AlignRight.IsChecked = true;
                    }

                    RectangleColor.Fill = Run.Foreground;                   
                }
            }
            else
            {
                object value = Selection.GetPropertyValue(TextElement.FontFamilyProperty);

                ComboBoxFont.SelectedItem = (FontFamily)((value == DependencyProperty.UnsetValue) ? null : value);
                
                value = Selection.GetPropertyValue(TextElement.FontSizeProperty);

                if (value != DependencyProperty.UnsetValue)
                {
                    PresentationSource source = PresentationSource.FromVisual(this);
                    double dpiX = 0, dpiY = 0;
                    if (source != null)
                    {
                        dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                        dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
                    }

                    double d = dpiX / 72;
                    
                    ComboBoxFontSize.Text = (Run.FontSize / d).ToString();                   
                }
                else
                {                    
                    ComboBoxFontSize.Text = null;
                }
               
                TextPointer startTextPointer = Selection.Start;
                TextPointerContext startTextPointerContext = startTextPointer.GetPointerContext(LogicalDirection.Forward);

                TextPointer endTextPointer = Selection.End;
                TextPointerContext endTextPointerContext = startTextPointer.GetPointerContext(LogicalDirection.Forward);

                bool isOverLine = false, isStrikethrough = false, isUnderLine = false;
                OverLine overLineCheck;
                Strikethrough strikethroughCheck;
                UnderLine underLineCheck;

                overLineCheck.stop = false;
                strikethroughCheck.stop = false;
                underLineCheck.stop = false;

                overLineCheck.state = false;
                strikethroughCheck.state = false;
                underLineCheck.state = false;

                overLineCheck.start = false;
                strikethroughCheck.start = false;
                underLineCheck.start = false;

                if (startTextPointerContext == TextPointerContext.Text)
                {
                    TextPointer runTextPointer = startTextPointer.GetNextContextPosition(LogicalDirection.Forward);
                    TextElement startTextElement = (TextElement)runTextPointer.GetAdjacentElement(LogicalDirection.Forward);
                    Run startRun = (Run)startTextElement;

                    foreach (TextDecoration decoration in startRun.TextDecorations)
                    {
                        if (decoration == TextDecorations.Strikethrough[0])
                        {
                            isStrikethrough = true;
                        }
                        else if (decoration == TextDecorations.OverLine[0])
                        {
                            isOverLine = true;
                        }
                        else if (decoration == TextDecorations.Underline[0])
                        {
                            isUnderLine = true;
                        }
                    }

                    if (isOverLine)
                    {
                        if (!overLineCheck.stop)
                        {
                            overLineCheck.state = true;
                        }
                    }
                    else
                    {
                        if (!overLineCheck.stop)
                        {
                            overLineCheck.start = true;

                            if (overLineCheck.state)
                            {
                                overLineCheck.stop = true;
                            }
                        }
                    }

                    if (isStrikethrough)
                    {
                        if (!strikethroughCheck.stop)
                        {
                            strikethroughCheck.state = true;
                        }
                    }
                    else
                    {
                        if (!strikethroughCheck.stop)
                        {
                            strikethroughCheck.start = true;

                            if (strikethroughCheck.state)
                            {
                                strikethroughCheck.stop = true;
                            }
                        }
                    }

                    if (isUnderLine)
                    {
                        if (!underLineCheck.stop)
                        {
                            underLineCheck.state = true;
                        }
                    }
                    else
                    {
                        if (!underLineCheck.stop)
                        {
                            underLineCheck.start = true;

                            if (underLineCheck.state)
                            {
                                underLineCheck.stop = true;
                            }
                        }
                    }
                    
                    SelectionEndElemendEndCheck(startRun, startRun.ElementStart, underLineCheck, overLineCheck, strikethroughCheck);

                    SelectionEndTextCheck(startRun, startRun.ElementStart, underLineCheck, overLineCheck, strikethroughCheck);

                    SelectionEndNoneCheck(startRun, startRun.ElementStart, underLineCheck, overLineCheck, strikethroughCheck);

                    SelectionEndStartCheck(startRun, startRun.ElementStart, underLineCheck, overLineCheck, strikethroughCheck);
                }
                else if (startTextPointerContext == TextPointerContext.ElementStart)
                {
                    TextElement startTextElement = (TextElement)Selection.Start.GetAdjacentElement(LogicalDirection.Forward);

                    if (startTextElement is LineBreak)
                    {
                        SelectionEndElemendEndCheck(startTextElement, startTextElement.ElementStart, underLineCheck, overLineCheck, strikethroughCheck);

                        SelectionEndTextCheck(startTextElement, startTextElement.ElementStart, underLineCheck, overLineCheck, strikethroughCheck);

                        SelectionEndNoneCheck(startTextElement, startTextElement.ElementStart, underLineCheck, overLineCheck, strikethroughCheck);

                        SelectionEndStartCheck(startTextElement, startTextElement.ElementStart, underLineCheck, overLineCheck, strikethroughCheck);
                    }
                }
                else if (startTextPointerContext == TextPointerContext.ElementEnd)
                {
                    TextElement startTextElement = (TextElement)Selection.Start.GetAdjacentElement(LogicalDirection.Forward);

                    startTextElement = (TextElement)startTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);
                    
                    SelectionEndElemendEndCheck(startTextElement, startTextElement.ElementStart, underLineCheck, overLineCheck, strikethroughCheck);

                    SelectionEndTextCheck(startTextElement, startTextElement.ElementStart, underLineCheck, overLineCheck, strikethroughCheck);

                    SelectionEndNoneCheck(startTextElement, startTextElement.ElementStart, underLineCheck, overLineCheck, strikethroughCheck);

                    SelectionEndStartCheck(startTextElement, startTextElement.ElementStart, underLineCheck, overLineCheck, strikethroughCheck);                   
                }        
                                
                value = Selection.GetPropertyValue(FontWeightProperty);

                if (value == DependencyProperty.UnsetValue)
                {
                    FontBold.IsChecked = false;
                }
                else if((FontWeight)value == FontWeights.Bold)
                {
                    FontBold.IsChecked = true;
                }

                value = Selection.GetPropertyValue(FontStyleProperty);

                if (value == DependencyProperty.UnsetValue)
                {
                    FontItalic.IsChecked = false;
                }
                else if ((FontStyle)value == FontStyles.Italic)
                {
                    FontItalic.IsChecked = true;
                }

                value = Selection.GetPropertyValue(Inline.BaselineAlignmentProperty);

                if (value == DependencyProperty.UnsetValue)
                {
                    FontSubscript.IsChecked = false;

                    FontSuperscript.IsChecked = false;
                }
                else if ((BaselineAlignment)value == BaselineAlignment.Subscript)
                {
                    FontSubscript.IsChecked = true;
                }
                else if ((BaselineAlignment)value == BaselineAlignment.Superscript)
                {
                    FontSuperscript.IsChecked = true;
                }
                else
                {
                    FontSubscript.IsChecked = false;

                    FontSuperscript.IsChecked = false;
                }

                value = Selection.GetPropertyValue(Paragraph.TextAlignmentProperty);

                if (value == DependencyProperty.UnsetValue)
                {
                    AlignCenter.IsChecked = false;
                    AlignFull.IsChecked = false;
                    AlignLeft.IsChecked = false;
                    AlignRight.IsChecked = false;
                }

                value = Selection.GetPropertyValue(Inline.ForegroundProperty);

                if (value == DependencyProperty.UnsetValue)
                {
                    RectangleColor.Fill = null;
                }
            }
        }

        struct OverLine
        {
            public bool stop;
            public bool state;
            public bool start;
        }

        struct Strikethrough
        {
            public bool stop;
            public bool state;
            public bool start;
        }

        struct UnderLine
        {
            public bool stop;
            public bool state;
            public bool start;
        }

        public void StartTextPointer(TextElement nextBackTextElement)
        {
            bool next = true;
           
            while (next)
            {
                if (nextBackTextElement is Paragraph)
                {
                    next = false;

                    if (startTextPointer == null && !IsTextNull)
                    {
                        startTextPointer = CaretPosition.Paragraph.ContentStart;
                    }
                }
                else if (nextBackTextElement is LineBreak)
                {
                    next = false;

                    if (startTextPointer == null && !IsTextNull)
                    {
                        startTextPointer = nextBackTextElement.ElementEnd;
                    }
                }
                else if (nextBackTextElement is ListItem)
                {
                    next = false;

                    if (startTextPointer == null && !IsTextNull)
                    {
                        startTextPointer = nextBackTextElement.ContentStart;
                    }
                }
                else
                {
                    if (nextBackTextElement is Run)
                    {
                        Run runBack = nextBackTextElement as Run;
                        if (runBack.Text.Length > 0)
                        {
                            int index2 = runBack.Text.LastIndexOfAny(new char[] { ' ', ',', '.', '?', '!', ':', ';' });
                            if (index2 != -1)
                            {
                                if (index2 + 1 - runBack.Text.Length != 0)
                                    startTextPointer = nextBackTextElement.ContentEnd.GetPositionAtOffset(index2 + 1 - runBack.Text.Length);
                                else
                                {
                                    if (!IsText)
                                    {
                                        IsTextNull = true;
                                    }
                                    else
                                    {
                                        startTextPointer = nextBackTextElement.ElementEnd;
                                    }                                    
                                }      
                             
                                next = false;
                            }
                            else
                            {
                                IsText = true;

                                IsTextNull = false;

                                nextBackTextElement = (TextElement)nextBackTextElement.ElementStart.GetAdjacentElement(LogicalDirection.Backward);
                            }
                        }
                        else
                        {
                            if (!IsText)
                            {
                                IsTextNull = true;
                            }

                            nextBackTextElement = (TextElement)nextBackTextElement.ElementStart.GetAdjacentElement(LogicalDirection.Backward);
                        }
                    }
                    else
                    {
                        if (!IsText)
                        {
                            IsTextNull = true;
                        }

                        nextBackTextElement = (TextElement)nextBackTextElement.ElementStart.GetAdjacentElement(LogicalDirection.Backward);
                    }
                }
            }
        }

        public void EndTextPointer(TextElement nextForwardTextElement)
        {
            bool nextForward = true;

            while (nextForward)
            {
                if (nextForwardTextElement is Paragraph)
                {
                    nextForward = false;

                    if (endTextPointer == null && !IsTextNull)
                    {                                               
                        endTextPointer = CaretPosition.Paragraph.ContentEnd;
                    }
                }
                else if (nextForwardTextElement is LineBreak)
                {
                    nextForward = false;

                    if (startTextPointer == null && !IsTextNull)
                    {
                        startTextPointer = nextForwardTextElement.ElementStart;
                    }
                }
                else if (nextForwardTextElement is ListItem)
                {
                    nextForward = false;

                    if (endTextPointer == null && !IsTextNull)
                    {
                        endTextPointer = nextForwardTextElement.ContentEnd;
                    }
                }
                else
                {
                    if (nextForwardTextElement is Run)
                    {
                        Run runForward = nextForwardTextElement as Run;
                        if (runForward.Text.Length > 0)
                        {
                            int index2 = runForward.Text.LastIndexOfAny(new char[] { ' ', ',', '.', '?', '!', ':', ';' });

                            if (index2 != -1)
                            {
                                if (index2 != 0)
                                {
                                    endTextPointer = nextForwardTextElement.ContentStart.GetPositionAtOffset(index2);
                                }
                                else
                                {
                                    if (!IsText)
                                    {
                                        IsTextNull = true;
                                    }
                                    else
                                    {
                                        endTextPointer = nextForwardTextElement.ElementStart;
                                    }                                   
                                }
                                    
                                nextForward = false;
                            }
                            else
                            {
                                IsText = true;
                                IsTextNull = false;
           
                                nextForwardTextElement = (TextElement)nextForwardTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);
                            }
                        }
                        else
                        {
                            if (!IsText)
                            {
                                IsTextNull = true;
                            }

                            nextForwardTextElement = (TextElement)nextForwardTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);
                        }
                    }
                    else
                    {
                        if (!IsText)
                        {
                            IsTextNull = true;
                        }

                        nextForwardTextElement = (TextElement)nextForwardTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);
                    }
                }
            }
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            UpdateToolBar();

            CorrectFontSize = double.Parse(ComboBoxFontSize.Text);

            e.Handled = true;
        }

        void FontItalic_Click(object sender, RoutedEventArgs e)
        {
            TextPointer textPoint = CaretPosition;                              
            startTextPointer = null;
            endTextPointer = null;

            if (Selection.IsEmpty)
            {
                TextElement textElement = null;

                if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.None)
                {
                    IsCreateRunNull = true;

                    CreateRunTextPointer = CaretPosition;

                    return;
                }
                else if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementStart)
                {
                    textElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                    if (textElement is Paragraph)
                    {
                        IsCreateRunNull = true;

                        CreateRunTextPointer = CaretPosition;

                        return;
                    }
                    else if (textElement is ListItem)
                    {
                        IsCreateRunNull = true;

                        CreateRunTextPointer = CaretPosition;

                        return;
                    }
                }
                else if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementEnd)
                {
                    textElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                    if (textElement is LineBreak)
                    {
                        IsCreateRunNull = true;

                        CreateRunTextPointer = CaretPosition;

                        return;
                    }
                }

                if (textPoint.GetTextRunLength(LogicalDirection.Backward) > 0) // Если позади каретки в текстовом элементе есть текст
                {
                    TextPointer textPointer = textPoint.GetNextContextPosition(LogicalDirection.Backward);

                    Run run = (Run)textPointer.GetAdjacentElement(LogicalDirection.Backward);

                    TextElement nextBackTextElement = (TextElement)run.ElementStart.GetAdjacentElement(LogicalDirection.Backward);

                    int i = CaretPosition.GetTextRunLength(LogicalDirection.Backward);

                    TextRange textRange = new TextRange(textPointer, CaretPosition);

                    int index = textRange.Text.LastIndexOfAny(new char[] { ' ', ',', '.', '?', '!', ':', ';' });

                    if (index != -1)
                    {
                        if (textRange.Text[i - 1] != ' ' && textRange.Text[i - 1] != ',' && textRange.Text[i - 1] != '.' && textRange.Text[i - 1] != '?' && textRange.Text[i - 1] != '!' && textRange.Text[i - 1] != ':' && textRange.Text[i - 1] != ';')
                        {
                            startTextPointer = CaretPosition.GetPositionAtOffset(index + 1 - i);
                        }
                        else
                            startTextPointer = null;

                    }
                    else
                    {
                        IsText = true;

                        StartTextPointer(nextBackTextElement);
                    }
                }
                else
                {
                    IsTextNull = true;

                    TextElement nextBackTextElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                    if (nextBackTextElement is Run)
                    {
                        nextBackTextElement = (TextElement)nextBackTextElement.ElementStart.GetAdjacentElement(LogicalDirection.Backward);
                    }

                    StartTextPointer(nextBackTextElement);
                }

                IsText = false;

                IsTextNull = false;

                if (textPoint.GetTextRunLength(LogicalDirection.Forward) > 0) // Если в текстовом элементе впереди каретки есть текст
                {
                    TextPointer textPointer = textPoint.GetNextContextPosition(LogicalDirection.Forward);

                    Run run = (Run)textPointer.GetAdjacentElement(LogicalDirection.Forward);

                    TextElement nextForwardTextElement = (TextElement)run.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);

                    int i = CaretPosition.GetTextRunLength(LogicalDirection.Forward);

                    TextRange textRange = new TextRange(CaretPosition, textPointer);

                    int index = textRange.Text.IndexOfAny(new char[] { ' ', ',', '.', '?', '!', ':', ';' });

                    if (index != -1)
                    {
                        if (textRange.Text[0] != ' ' && textRange.Text[0] != ',' && textRange.Text[0] != '.' && textRange.Text[0] != '?' && textRange.Text[0] != '!' && textRange.Text[0] != ':' && textRange.Text[0] != ';')
                        {
                            endTextPointer = CaretPosition.GetPositionAtOffset(index);
                        }
                        else
                            endTextPointer = null;
                    }
                    else
                    {
                        IsText = true;

                        EndTextPointer(nextForwardTextElement);
                    }
                }
                else
                {
                    IsTextNull = true;

                    TextElement nextForwardTextElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Forward);

                    if (nextForwardTextElement is Run)
                    {
                        nextForwardTextElement = (TextElement)nextForwardTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);
                    }

                    EndTextPointer(nextForwardTextElement);
                }

                IsTextNull = false;

                IsText = false;

                if (startTextPointer != null && endTextPointer != null)
                {
                    TextRange textRange = new TextRange(startTextPointer, endTextPointer);

                    object obj = textRange.GetPropertyValue(TextElement.FontStyleProperty);

                    if (FontItalic.IsChecked == true)
                    {
                        if (obj == DependencyProperty.UnsetValue)
                            textRange.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
                        else if ((FontStyle)obj != FontStyles.Italic)
                            textRange.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
                    }
                    else if (FontItalic.IsChecked == false)
                    {
                        if (obj == DependencyProperty.UnsetValue)
                            textRange.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Normal);
                        else if ((FontStyle)obj != FontStyles.Normal)
                            textRange.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Normal);
                    }
                }
                else
                {
                    if ((bool)FontItalic.IsChecked)
                    {
                        if (Run.FontStyle != FontStyles.Italic)
                        {
                            IsCreateRunItalic = true;

                            CreateRunTextPointer = CaretPosition;
                        }
                        else
                        {
                            IsCreateRunItalic = false;
                        }
                    }
                    else
                    {
                        if (Run.FontStyle != FontStyles.Normal)
                        {
                            IsCreateRunItalic = true;

                            CreateRunTextPointer = CaretPosition;
                        }
                        else
                        {
                            IsCreateRunItalic = false;
                        }
                    }
                }
            }
            else
            {
                object obj = Selection.GetPropertyValue(TextElement.FontStyleProperty);

                if (FontItalic.IsChecked == true)
                {
                    if (obj == DependencyProperty.UnsetValue)
                        Selection.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
                    else if ((FontStyle)obj != FontStyles.Italic)
                        Selection.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
                }
                else if (FontItalic.IsChecked == false)
                {
                    if (obj == DependencyProperty.UnsetValue)
                        Selection.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Normal);
                    else if ((FontStyle)obj != FontStyles.Normal)
                        Selection.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Normal);
                }
            }
                      
            e.Handled = true;
        }

        void FontBold_Click(object sender, RoutedEventArgs e)
        {
            TextPointer textPoint = CaretPosition;
            startTextPointer = null;
            endTextPointer = null;

            if (Selection.IsEmpty)
            {
                TextElement textElement = null;

                if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.None)
                {
                    IsCreateRunNull = true;

                    CreateRunTextPointer = CaretPosition;

                    return;
                }
                else if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementStart)
                {
                    textElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                    if (textElement is Paragraph)
                    {
                        IsCreateRunNull = true;

                        CreateRunTextPointer = CaretPosition;

                        return;
                    }
                    else if (textElement is ListItem)
                    {
                        IsCreateRunNull = true;

                        CreateRunTextPointer = CaretPosition;

                        return;
                    }
                }
                else if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementEnd)
                {
                    textElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                    if (textElement is LineBreak)
                    {
                        IsCreateRunNull = true;

                        CreateRunTextPointer = CaretPosition;

                        return;
                    }
                }

                if (textPoint.GetTextRunLength(LogicalDirection.Backward) > 0) // Если позади каретки в текстовом элементе есть текст
                {
                    TextPointer textPointer = textPoint.GetNextContextPosition(LogicalDirection.Backward);

                    Run run = (Run)textPointer.GetAdjacentElement(LogicalDirection.Backward);

                    TextElement nextBackTextElement = (TextElement)run.ElementStart.GetAdjacentElement(LogicalDirection.Backward);

                    int i = CaretPosition.GetTextRunLength(LogicalDirection.Backward);

                    TextRange textRange = new TextRange(textPointer, CaretPosition);

                    int index = textRange.Text.LastIndexOfAny(new char[] { ' ', ',', '.', '?', '!', ':', ';' });

                    if (index != -1)
                    {
                        if (textRange.Text[i - 1] != ' ' && textRange.Text[i - 1] != ',' && textRange.Text[i - 1] != '.' && textRange.Text[i - 1] != '?' && textRange.Text[i - 1] != '!' && textRange.Text[i - 1] != ':' && textRange.Text[i - 1] != ';')
                        {
                            startTextPointer = CaretPosition.GetPositionAtOffset(index + 1 - i);
                        }
                        else
                        {
                            startTextPointer = null;
                        }                           
                    }
                    else
                    {
                        IsText = true;

                        StartTextPointer(nextBackTextElement);
                    }
                }
                else
                {
                    IsTextNull = true;

                    TextElement nextBackTextElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                    if (nextBackTextElement is Run)
                    {
                        nextBackTextElement = (TextElement)nextBackTextElement.ElementStart.GetAdjacentElement(LogicalDirection.Backward);
                    }
                  
                    StartTextPointer(nextBackTextElement);
                }

                IsTextNull = false;

                IsText = false;

                if (textPoint.GetTextRunLength(LogicalDirection.Forward) > 0) // Если в текстовом элементе впереди каретки есть текст
                {
                    TextPointer textPointer = textPoint.GetNextContextPosition(LogicalDirection.Forward);

                    Run run = (Run)textPointer.GetAdjacentElement(LogicalDirection.Forward);

                    TextElement nextForwardTextElement = (TextElement)run.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);

                    int i = CaretPosition.GetTextRunLength(LogicalDirection.Forward);

                    TextRange textRange = new TextRange(CaretPosition, textPointer);

                    int index = textRange.Text.IndexOfAny(new char[] { ' ', ',', '.', '?', '!', ':', ';' });

                    if (index != -1)
                    {
                        if (textRange.Text[0] != ' ' && textRange.Text[0] != ',' && textRange.Text[0] != '.' && textRange.Text[0] != '?' && textRange.Text[0] != '!' && textRange.Text[0] != ':' && textRange.Text[0] != ';')
                        {
                            endTextPointer = CaretPosition.GetPositionAtOffset(index);
                        }
                        else
                        {
                            endTextPointer = null;
                        }                           
                    }
                    else
                    {
                        IsText = true;

                        EndTextPointer(nextForwardTextElement);
                    }
                }
                else
                {
                    IsTextNull = true;

                    TextElement nextForwardTextElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Forward);

                    if (nextForwardTextElement is Run)
                    {
                        nextForwardTextElement = (TextElement)nextForwardTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);
                    }

                    EndTextPointer(nextForwardTextElement);
                }

                IsText = false;

                IsTextNull = false;

                if (startTextPointer != null && endTextPointer != null)
                {
                    TextRange textRange = new TextRange(startTextPointer, endTextPointer);

                    object obj = textRange.GetPropertyValue(TextElement.FontWeightProperty);

                    if ((bool)FontBold.IsChecked)
                    {
                        if (obj == DependencyProperty.UnsetValue)
                            textRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                        else if ((FontWeight)obj != FontWeights.Bold)
                            textRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                    }
                    else
                    {
                        if (obj == DependencyProperty.UnsetValue)
                            textRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
                        else if ((FontWeight)obj != FontWeights.Normal)
                            textRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
                    }
                }
                else
                {                    
                    if ((bool)FontBold.IsChecked)
                    {
                        if (Run.FontWeight != FontWeights.Bold)
                        {
                            IsCreateRunBold = true;

                            CreateRunTextPointer = CaretPosition;
                        }
                        else
                        {
                            IsCreateRunBold = false;
                        }
                    }
                    else
                    {
                        if (Run.FontWeight != FontWeights.Normal)
                        {
                            IsCreateRunBold = true;

                            CreateRunTextPointer = CaretPosition;
                        }
                        else
                        {
                            IsCreateRunBold = false;
                        }
                    }                             
                }
            }
            else
            {
                object obj = Selection.GetPropertyValue(TextElement.FontWeightProperty);

                if ((bool)FontBold.IsChecked)
                {
                    if (obj == DependencyProperty.UnsetValue)
                        Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                    else if ((FontWeight)obj != FontWeights.Bold)
                        Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                }
                else
                {
                    if (obj == DependencyProperty.UnsetValue)
                        Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
                    else if ((FontWeight)obj != FontWeights.Normal)
                        Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
                }
            }
            
            e.Handled = true;
        }

        void FontOverline_Click(object sender, RoutedEventArgs e)
        {
            TextPointer textPoint = CaretPosition;
            startTextPointer = null;
            endTextPointer = null;

            if (Selection.IsEmpty)
            {
                TextElement textElement = null;

                if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.None)
                {
                    IsCreateRunNull = true;

                    CreateRunTextPointer = CaretPosition;

                    return;
                }
                else if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementStart)
                {
                    textElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                    if (textElement is Paragraph)
                    {
                        IsCreateRunNull = true;

                        CreateRunTextPointer = CaretPosition;

                        return;
                    }
                    else if (textElement is ListItem)
                    {
                        IsCreateRunNull = true;

                        CreateRunTextPointer = CaretPosition;

                        return;
                    }
                }
                else if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementEnd)
                {
                    textElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                    if (textElement is LineBreak)
                    {
                        IsCreateRunNull = true;

                        CreateRunTextPointer = CaretPosition;

                        return;
                    }
                }

                if (textPoint.GetTextRunLength(LogicalDirection.Backward) > 0) // Если позади каретки в текстовом элементе есть текст
                {
                    TextPointer textPointer = textPoint.GetNextContextPosition(LogicalDirection.Backward);

                    Run run = (Run)textPointer.GetAdjacentElement(LogicalDirection.Backward);

                    TextElement nextBackTextElement = (TextElement)run.ElementStart.GetAdjacentElement(LogicalDirection.Backward);

                    int i = CaretPosition.GetTextRunLength(LogicalDirection.Backward);

                    TextRange textRange = new TextRange(textPointer, CaretPosition);

                    int index = textRange.Text.LastIndexOfAny(new char[] { ' ', ',', '.', '?', '!', ':', ';' });

                    if (index != -1)
                    {
                        if (textRange.Text[i - 1] != ' ' && textRange.Text[i - 1] != ',' && textRange.Text[i - 1] != '.' && textRange.Text[i - 1] != '?' && textRange.Text[i - 1] != '!' && textRange.Text[i - 1] != ':' && textRange.Text[i - 1] != ';')
                        {
                            startTextPointer = CaretPosition.GetPositionAtOffset(index + 1 - i);
                        }
                        else
                            startTextPointer = null;
                    }
                    else
                    {
                        IsText = true;

                        StartTextPointer(nextBackTextElement);
                    }
                }
                else
                {
                    IsTextNull = true;

                    TextElement nextBackTextElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                    if (nextBackTextElement is Run)
                    {
                        nextBackTextElement = (TextElement)nextBackTextElement.ElementStart.GetAdjacentElement(LogicalDirection.Backward);
                    }

                    StartTextPointer(nextBackTextElement);
                }

                IsText = false;

                IsTextNull = false;

                if (textPoint.GetTextRunLength(LogicalDirection.Forward) > 0) // Если в текстовом элементе впереди каретки есть текст
                {
                    TextPointer textPointer = textPoint.GetNextContextPosition(LogicalDirection.Forward);

                    Run run = (Run)textPointer.GetAdjacentElement(LogicalDirection.Forward);

                    TextElement nextForwardTextElement = (TextElement)run.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);

                    int i = CaretPosition.GetTextRunLength(LogicalDirection.Forward);

                    TextRange textRange = new TextRange(CaretPosition, textPointer);

                    int index = textRange.Text.IndexOfAny(new char[] { ' ', ',', '.', '?', '!', ':', ';' });

                    if (index != -1)
                    {
                        if (textRange.Text[0] != ' ' && textRange.Text[0] != ',' && textRange.Text[0] != '.' && textRange.Text[0] != '?' && textRange.Text[0] != '!' && textRange.Text[0] != ':' && textRange.Text[0] != ';')
                        {
                            endTextPointer = CaretPosition.GetPositionAtOffset(index);
                        }
                        else
                            endTextPointer = null;
                    }
                    else
                    {
                        IsText = true;

                        EndTextPointer(nextForwardTextElement);
                    }
                }
                else
                {
                    IsTextNull = true;

                    TextElement nextForwardTextElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Forward);

                    if (nextForwardTextElement is Run)
                    {
                        nextForwardTextElement = (TextElement)nextForwardTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);
                    }

                    EndTextPointer(nextForwardTextElement);
                }

                IsText = false;

                IsTextNull = false;

                if (startTextPointer != null && endTextPointer != null)
                {
                    TextRange textRange = new TextRange(startTextPointer, endTextPointer);

                    object obj = textRange.GetPropertyValue(Inline.TextDecorationsProperty);

                    if (obj == DependencyProperty.UnsetValue)
                    {
                        Run run = null;

                        bool underLine = false;
                        bool overLine = false;
                        bool strikethrough = false;

                        TextPointerContext startTextPointerContext = textRange.Start.GetPointerContext(LogicalDirection.Forward);

                        if (startTextPointerContext == TextPointerContext.ElementStart)
                        {
                            TextElement startTextElement = (TextElement)textRange.Start.GetAdjacentElement(LogicalDirection.Forward);

                            if (startTextElement is LineBreak)
                            {
                                SelectionEndElemendEnd(startTextElement, startTextElement.ElementStart);

                                SelectionEndText(startTextElement, startTextElement.ElementStart);

                                SelectionEndNone(startTextElement, startTextElement.ElementStart);

                                SelectionEndStart(startTextElement, startTextElement.ElementStart);
                            }
                        }
                        else if (startTextPointerContext == TextPointerContext.Text)
                        {
                            TextPointer endTextPointerSelection = textRange.Start.GetNextContextPosition(LogicalDirection.Forward); // находим конец первого Run

                            run = (Run)endTextPointerSelection.GetAdjacentElement(LogicalDirection.Forward);

                            TextRange textRangeText = new TextRange(textRange.Start, endTextPointerSelection);

                            TextDecorationCollection collection = (TextDecorationCollection)textRangeText.GetPropertyValue(Inline.TextDecorationsProperty);
                            TextDecorationCollection newCollection = new TextDecorationCollection();

                            foreach (TextDecoration textDecoration in collection)
                            {
                                if (textDecoration.Location == TextDecorationLocation.OverLine)
                                    overLine = true;
                                if (textDecoration.Location == TextDecorationLocation.Strikethrough)
                                    strikethrough = true;
                                if (textDecoration.Location == TextDecorationLocation.Underline)
                                    underLine = true;
                            }

                            if ((bool)FontOverline.IsChecked)
                                newCollection.Add(TextDecorations.OverLine[0]);
                            else
                            {
                                if (overLine)
                                    newCollection.Remove(TextDecorations.OverLine[0]);
                            }

                            if (strikethrough)
                                newCollection.Add(TextDecorations.Strikethrough[0]);

                            if (underLine)
                                newCollection.Add(TextDecorations.Underline[0]);

                            textRangeText.ApplyPropertyValue(Inline.TextDecorationsProperty, newCollection);

                            underLine = false;
                            overLine = false;
                            strikethrough = false;

                            // Переходим к следующему элементу
                            TextPointer textPointer = run.ElementEnd;
                            TextElement startTextElement = (TextElement)textPointer.GetAdjacentElement(LogicalDirection.Forward);

                            SelectionEndElemendEnd(startTextElement, textPointer);

                            SelectionEndText(startTextElement, textPointer);

                            SelectionEndNone(startTextElement, textPointer);

                            SelectionEndStart(startTextElement, textPointer);
                        }
                        else if (startTextPointerContext == TextPointerContext.ElementEnd)
                        {
                            TextElement startTextElement = (TextElement)textRange.Start.GetAdjacentElement(LogicalDirection.Forward);

                            if (startTextElement is Run)
                            {
                                // Переходим к следующему элементу
                                startTextPointer = startTextElement.ElementEnd;

                                startTextElement = (TextElement)startTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);

                                SelectionEndElemendEnd(startTextElement, startTextPointer);

                                SelectionEndText(startTextElement, startTextPointer);

                                SelectionEndNone(startTextElement, startTextPointer);

                                SelectionEndStart(startTextElement, startTextPointer);
                            }
                            else
                            {
                                SelectionEndElemendEnd(startTextElement, startTextElement.ContentEnd);

                                SelectionEndText(startTextElement, startTextElement.ContentEnd);

                                SelectionEndNone(startTextElement, startTextElement.ContentEnd);

                                SelectionEndStart(startTextElement, startTextElement.ContentEnd);
                            }
                        }
                    }
                    else
                    {
                        bool underLine = false;
                        bool overLine = false;
                        bool strikethrough = false;

                        TextDecorationCollection collection = (TextDecorationCollection)textRange.GetPropertyValue(Inline.TextDecorationsProperty);
                        TextDecorationCollection newCollection = new TextDecorationCollection();

                        foreach (TextDecoration textDecoration in collection)
                        {
                            if (textDecoration.Location == TextDecorationLocation.OverLine)
                                overLine = true;
                            if (textDecoration.Location == TextDecorationLocation.Strikethrough)
                                strikethrough = true;
                            if (textDecoration.Location == TextDecorationLocation.Underline)
                                underLine = true;
                        }

                        if ((bool)FontOverline.IsChecked)
                            newCollection.Add(TextDecorations.OverLine[0]);
                        else
                        {
                            if (overLine)
                                newCollection.Remove(TextDecorations.OverLine[0]);
                        }

                        if (strikethrough)
                            newCollection.Add(TextDecorations.Strikethrough[0]);

                        if (underLine)
                            newCollection.Add(TextDecorations.Underline[0]);

                        textRange.ApplyPropertyValue(Inline.TextDecorationsProperty, newCollection);
                    }
                }
                else
                {
                    bool overLine = false;

                    foreach (TextDecoration textDecoration in Run.TextDecorations)
                    {
                        if (textDecoration.Location == TextDecorationLocation.OverLine)
                            overLine = true;                       
                    }

                    if ((bool)FontOverline.IsChecked)
                    {
                        if (!overLine)
                        {
                            IsCreateRunOverline = true;

                            CreateRunTextPointer = CaretPosition;
                        }
                        else
                        {
                            IsCreateRunOverline = false;
                        }
                    }
                    else
                    {
                        if (overLine)
                        {
                            IsCreateRunOverline = true;

                            CreateRunTextPointer = CaretPosition;
                        }
                        else
                        {
                            IsCreateRunOverline = false;
                        }
                    }   
                }
            }
            else
            {
                object obj = Selection.GetPropertyValue(Inline.TextDecorationsProperty);

                if (obj == DependencyProperty.UnsetValue)
                {
                    Run run = null;

                    bool underLine = false;
                    bool overLine = false;
                    bool strikethrough = false;

                    TextPointerContext startTextPointerContext = Selection.Start.GetPointerContext(LogicalDirection.Forward);

                    if (startTextPointerContext == TextPointerContext.ElementStart)
                    {
                        TextElement startTextElement = (TextElement)Selection.Start.GetAdjacentElement(LogicalDirection.Forward);

                        if (startTextElement is LineBreak)
                        {
                            SelectionEndElemendEnd(startTextElement, startTextElement.ElementStart);

                            SelectionEndText(startTextElement, startTextElement.ElementStart);

                            SelectionEndNone(startTextElement, startTextElement.ElementStart);

                            SelectionEndStart(startTextElement, startTextElement.ElementStart);
                        }
                    }
                    else if (startTextPointerContext == TextPointerContext.Text)
                    {
                        TextPointer endTextPointerSelection = Selection.Start.GetNextContextPosition(LogicalDirection.Forward); // находим конец первого Run

                        run = (Run)endTextPointerSelection.GetAdjacentElement(LogicalDirection.Forward);

                        TextRange textRange = new TextRange(Selection.Start, endTextPointerSelection);

                        TextDecorationCollection collection = (TextDecorationCollection)textRange.GetPropertyValue(Inline.TextDecorationsProperty);
                        TextDecorationCollection newCollection = new TextDecorationCollection();

                        foreach (TextDecoration textDecoration in collection)
                        {
                            if (textDecoration.Location == TextDecorationLocation.OverLine)
                                overLine = true;
                            if (textDecoration.Location == TextDecorationLocation.Strikethrough)
                                strikethrough = true;
                            if (textDecoration.Location == TextDecorationLocation.Underline)
                                underLine = true;
                        }

                        if ((bool)FontOverline.IsChecked)
                            newCollection.Add(TextDecorations.OverLine[0]);
                        else
                        {
                            if (overLine)
                                newCollection.Remove(TextDecorations.OverLine[0]);
                        }

                        if (strikethrough)
                            newCollection.Add(TextDecorations.Strikethrough[0]);

                        if (underLine)
                            newCollection.Add(TextDecorations.Underline[0]);

                        textRange.ApplyPropertyValue(Inline.TextDecorationsProperty, newCollection);

                        underLine = false;
                        overLine = false;
                        strikethrough = false;

                        // Переходим к следующему элементу
                        TextPointer textPointer = run.ElementEnd;
                        TextElement startTextElement = (TextElement)textPointer.GetAdjacentElement(LogicalDirection.Forward);

                        SelectionEndElemendEnd(startTextElement, textPointer);

                        SelectionEndText(startTextElement, textPointer);

                        SelectionEndNone(startTextElement, textPointer);

                        SelectionEndStart(startTextElement, textPointer);
                    }
                    else if (startTextPointerContext == TextPointerContext.ElementEnd)
                    {
                        TextElement startTextElement = (TextElement)Selection.Start.GetAdjacentElement(LogicalDirection.Forward);

                        if (startTextElement is Run)
                        {
                            // Переходим к следующему элементу
                            startTextPointer = startTextElement.ElementEnd;

                            startTextElement = (TextElement)startTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);

                            SelectionEndElemendEnd(startTextElement, startTextPointer);

                            SelectionEndText(startTextElement, startTextPointer);

                            SelectionEndNone(startTextElement, startTextPointer);

                            SelectionEndStart(startTextElement, startTextPointer);
                        }
                        else
                        {
                            SelectionEndElemendEnd(startTextElement, startTextElement.ContentEnd);

                            SelectionEndText(startTextElement, startTextElement.ContentEnd);

                            SelectionEndNone(startTextElement, startTextElement.ContentEnd);

                            SelectionEndStart(startTextElement, startTextElement.ContentEnd);
                        }
                    }
                }
                else
                {
                    bool underLine = false;
                    bool overLine = false;
                    bool strikethrough = false;

                    TextDecorationCollection collection = (TextDecorationCollection)Selection.GetPropertyValue(Inline.TextDecorationsProperty);
                    TextDecorationCollection newCollection = new TextDecorationCollection();

                    foreach (TextDecoration textDecoration in collection)
                    {
                        if (textDecoration.Location == TextDecorationLocation.OverLine)
                            overLine = true;
                        if (textDecoration.Location == TextDecorationLocation.Strikethrough)
                            strikethrough = true;
                        if (textDecoration.Location == TextDecorationLocation.Underline)
                            underLine = true;
                    }

                    if ((bool)FontOverline.IsChecked)
                        newCollection.Add(TextDecorations.OverLine[0]);
                    else
                    {
                        if (overLine)
                            newCollection.Remove(TextDecorations.OverLine[0]);
                    }

                    if (strikethrough)
                        newCollection.Add(TextDecorations.Strikethrough[0]);

                    if (underLine)
                        newCollection.Add(TextDecorations.Underline[0]);

                    Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, newCollection);
                }
            }
            
            e.Handled = true;
        }

        // Находим в цикле отдельные Run и изменяем их TextDecorationCollection
        void NextRun(TextElement startTextElement, TextElement endTextElement, bool nextRun, TextPointer startTextPointer)
        {
            bool underLine = false;
            bool overLine = false;
            bool strikethrough = false;

            Run run = null;

            TextDecorationCollection textdecCollection = null;

            TextPointer nextTextPointer = startTextPointer;

            TextElement nextTextElement = startTextElement;

            while (nextRun)
            {
                if (nextTextElement == endTextElement)
                    return;

                if (nextTextElement is LineBreak) // Только TextPointerContext.StartElement и переходим в начало цикла к следующему элементу
                {
                    nextTextPointer = nextTextElement.ElementEnd;

                    nextTextElement = (TextElement)nextTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);
                    
                    continue;
                }
                else if (nextTextElement is Paragraph || nextTextElement is List || nextTextElement is ListItem) 
                {
                    if (nextTextPointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementStart)
                    {
                        nextTextPointer = nextTextElement.ContentStart;

                        nextTextElement = (TextElement)nextTextPointer.GetAdjacentElement(LogicalDirection.Forward);
                    }                      
                    else if (nextTextPointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementEnd)
                    {
                        nextTextPointer = nextTextElement.ElementEnd;

                        nextTextElement = (TextElement)nextTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);
                    }

                    continue;
                }               
                else if (nextTextElement is Run)
                {
                    run = (Run)nextTextElement;

                    nextTextPointer = nextTextElement.ElementEnd;

                    nextTextElement = (TextElement)nextTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);
                 
                    textdecCollection = new TextDecorationCollection();

                    foreach (TextDecoration textDecoration in run.TextDecorations)
                    {
                        if (textDecoration.Location == TextDecorationLocation.OverLine)
                            overLine = true;
                        if (textDecoration.Location == TextDecorationLocation.Strikethrough)
                            strikethrough = true;
                        if (textDecoration.Location == TextDecorationLocation.Underline)
                            underLine = true;
                    }

                    if ((bool)FontStrikethrough.IsChecked)
                    {                       
                        textdecCollection.Add(TextDecorations.Strikethrough[0]);
                        run.TextDecorations = textdecCollection;                       
                    }
                    else
                    {
                        if (strikethrough)
                        {
                            textdecCollection.Remove(TextDecorations.Strikethrough[0]);
                            run.TextDecorations = textdecCollection;
                        }
                    }

                    if (underLine)
                        textdecCollection.Add(TextDecorations.Underline[0]);

                    if (overLine)
                        textdecCollection.Add(TextDecorations.OverLine[0]);
                   
                    underLine = false;
                    overLine = false;
                    strikethrough = false;
                }                            
            }
        }

        // Зачеркнутое выделение когда Selection.End граничит с ElementEnd
        void SelectionEndElemendEnd(TextElement startTextElement, TextPointer startTextPointer)
        {
            bool nextRun = true;
            TextElement endTextElement = null;

            bool underLine = false;
            bool overLine = false;
            bool strikethrough = false;

            TextDecorationCollection textdecCollection = null;

            if (Selection.End.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementEnd)
            {              
                endTextElement = (TextElement)Selection.End.GetAdjacentElement(LogicalDirection.Forward);

                if (startTextElement == endTextElement)
                    return;
               
                if (endTextElement is Run)
                {
                    Run endRun = endTextElement as Run;

                    textdecCollection = new TextDecorationCollection();

                    foreach (TextDecoration textDecoration in endRun.TextDecorations)
                    {
                        if (textDecoration.Location == TextDecorationLocation.OverLine)
                            overLine = true;
                        if (textDecoration.Location == TextDecorationLocation.Strikethrough)
                            strikethrough = true;
                        if (textDecoration.Location == TextDecorationLocation.Underline)
                            underLine = true;
                    }

                    if ((bool)FontStrikethrough.IsChecked)
                    {                       
                        textdecCollection.Add(TextDecorations.Strikethrough[0]);
                        endRun.TextDecorations = textdecCollection;                       
                    }
                    else
                    {
                        if (strikethrough)
                        {
                            textdecCollection.Remove(TextDecorations.Strikethrough[0]);
                            endRun.TextDecorations = textdecCollection;
                        }
                    }

                    if (underLine)
                        textdecCollection.Add(TextDecorations.Underline[0]);

                    if (overLine)
                        textdecCollection.Add(TextDecorations.OverLine[0]);

                    underLine = false;
                    overLine = false;
                    strikethrough = false;                 
                }
                                         
                NextRun(startTextElement, endTextElement, nextRun, startTextPointer);
            }
        }

        // Зачеркнутое выделение когда Selection.End граничит с Text
        void SelectionEndText(TextElement startTextElement, TextPointer startTextPointer)
        {
            TextElement endTextElement = null;

            bool underLine = false;
            bool overLine = false;
            bool strikethrough = false;

            if (Selection.End.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
            {
                bool nextRun = true;

                TextPointer endTextpointer = Selection.End.GetNextContextPosition(LogicalDirection.Backward);

                endTextElement = (TextElement)endTextpointer.GetAdjacentElement(LogicalDirection.Backward);
               
                TextRange textRange = new TextRange(endTextpointer, Selection.End);
               
                TextDecorationCollection collection = (TextDecorationCollection)textRange.GetPropertyValue(Inline.TextDecorationsProperty);
                TextDecorationCollection newCollection = new TextDecorationCollection();

                foreach (TextDecoration textDecoration in collection)
                {
                    if (textDecoration.Location == TextDecorationLocation.OverLine)
                        overLine = true;
                    if (textDecoration.Location == TextDecorationLocation.Strikethrough)
                        strikethrough = true;
                    if (textDecoration.Location == TextDecorationLocation.Underline)
                        underLine = true;
                }

                if ((bool)FontStrikethrough.IsChecked)                                   
                    newCollection.Add(TextDecorations.Strikethrough[0]);              
                else
                {
                    if (strikethrough)
                        newCollection.Remove(TextDecorations.Strikethrough[0]);
                }

                if (overLine)
                    newCollection.Add(TextDecorations.OverLine[0]);

                if (underLine)
                    newCollection.Add(TextDecorations.Underline[0]);

                textRange.ApplyPropertyValue(Inline.TextDecorationsProperty, newCollection);

                underLine = false;
                overLine = false;
                strikethrough = false;

                if (startTextElement == endTextElement)
                    return;
                
                NextRun(startTextElement, endTextElement, nextRun, startTextPointer);                          
            }
        }

        void SelectionEndNone(TextElement startTextElement, TextPointer startTextPointer)
        {
            if (Selection.End.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.None)
            {
                TextElement endTextElement = (TextElement)Selection.End.GetAdjacentElement(LogicalDirection.Backward);

                bool nextRun = true;

                if (startTextElement == endTextElement)
                    return;

                NextRun(startTextElement, endTextElement, nextRun, startTextPointer);
            }            
        }

        void SelectionEndStart(TextElement startTextElement, TextPointer startTextPointer)
        {
            if (Selection.End.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementStart)
            {
                bool nextRun = true;

                TextElement endTextElement = (TextElement)Selection.End.GetAdjacentElement(LogicalDirection.Backward);

                if (startTextElement == endTextElement)
                    return;
               
                NextRun(startTextElement, endTextElement, nextRun, startTextPointer);              
            }
        }

        void FontStrikethrough_Click(object sender, RoutedEventArgs e)
        {
            TextPointer textPoint = CaretPosition;
            startTextPointer = null;
            endTextPointer = null;

            if (Selection.IsEmpty)
            {
                TextElement textElement = null;

                if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.None)
                {
                    IsCreateRunNull = true;
                    
                    CreateRunTextPointer = CaretPosition;

                    return;
                }
                else if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementStart)
                {
                    textElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);
                    
                    if (textElement is Paragraph)
                    {
                        IsCreateRunNull = true;

                        CreateRunTextPointer = CaretPosition;

                        return;
                    }
                    else if (textElement is ListItem)
                    {
                        IsCreateRunNull = true;

                        CreateRunTextPointer = CaretPosition;

                        return;
                    }
                }
                else if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementEnd)
                {
                    textElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                    if (textElement is LineBreak)
                    {
                        IsCreateRunNull = true;

                        CreateRunTextPointer = CaretPosition;

                        return;
                    }
                }

                if (textPoint.GetTextRunLength(LogicalDirection.Backward) > 0) // Если позади каретки в текстовом элементе есть текст
                {
                    TextPointer textPointer = textPoint.GetNextContextPosition(LogicalDirection.Backward);

                    Run run = (Run)textPointer.GetAdjacentElement(LogicalDirection.Backward);

                    TextElement nextBackTextElement = (TextElement)run.ElementStart.GetAdjacentElement(LogicalDirection.Backward);

                    int i = CaretPosition.GetTextRunLength(LogicalDirection.Backward);

                    TextRange textRange = new TextRange(textPointer, CaretPosition);

                    int index = textRange.Text.LastIndexOfAny(new char[] { ' ', ',', '.', '?', '!', ':', ';' });

                    if (index != -1)
                    {
                        if (textRange.Text[i - 1] != ' ' && textRange.Text[i - 1] != ',' && textRange.Text[i - 1] != '.' && textRange.Text[i - 1] != '?' && textRange.Text[i - 1] != '!' && textRange.Text[i - 1] != ':' && textRange.Text[i - 1] != ';')
                        {
                            startTextPointer = CaretPosition.GetPositionAtOffset(index + 1 - i);
                        }
                        else
                            startTextPointer = null;
                    }
                    else
                    {
                        IsText = true;

                        StartTextPointer(nextBackTextElement);
                    }
                }
                else
                {
                    IsTextNull = true;

                    TextElement nextBackTextElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                    if (nextBackTextElement is Run)
                    {
                        nextBackTextElement = (TextElement)nextBackTextElement.ElementStart.GetAdjacentElement(LogicalDirection.Backward);
                    }

                    StartTextPointer(nextBackTextElement);
                }

                IsText = false;

                IsTextNull = false;

                if (textPoint.GetTextRunLength(LogicalDirection.Forward) > 0) // Если в текстовом элементе впереди каретки есть текст
                {
                    TextPointer textPointer = textPoint.GetNextContextPosition(LogicalDirection.Forward);

                    Run run = (Run)textPointer.GetAdjacentElement(LogicalDirection.Forward);

                    TextElement nextForwardTextElement = (TextElement)run.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);

                    int i = CaretPosition.GetTextRunLength(LogicalDirection.Forward);

                    TextRange textRange = new TextRange(CaretPosition, textPointer);

                    int index = textRange.Text.IndexOfAny(new char[] { ' ', ',', '.', '?', '!', ':', ';' });

                    if (index != -1)
                    {
                        if (textRange.Text[0] != ' ' && textRange.Text[0] != ',' && textRange.Text[0] != '.' && textRange.Text[0] != '?' && textRange.Text[0] != '!' && textRange.Text[0] != ':' && textRange.Text[0] != ';')
                        {
                            endTextPointer = CaretPosition.GetPositionAtOffset(index);
                        }
                        else
                            endTextPointer = null;
                    }
                    else
                    {
                        IsText = true;

                        EndTextPointer(nextForwardTextElement);
                    }
                }
                else
                {
                    IsTextNull = true;

                    TextElement nextForwardTextElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Forward);

                    if (nextForwardTextElement is Run)
                    {
                        nextForwardTextElement = (TextElement)nextForwardTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);
                    }

                    EndTextPointer(nextForwardTextElement);
                }

                IsText = false;

                IsTextNull = false;

                if (startTextPointer != null && endTextPointer != null)
                {
                    TextRange textRange = new TextRange(startTextPointer, endTextPointer);

                    object obj = textRange.GetPropertyValue(Inline.TextDecorationsProperty);

                    if (obj == DependencyProperty.UnsetValue)
                    {
                        Run run = null;

                        bool underLine = false;
                        bool overLine = false;
                        bool strikethrough = false;

                        TextPointerContext startTextPointerContext = textRange.Start.GetPointerContext(LogicalDirection.Forward);

                        if (startTextPointerContext == TextPointerContext.ElementStart)
                        {
                            TextElement startTextElement = (TextElement)textRange.Start.GetAdjacentElement(LogicalDirection.Forward);

                            if (startTextElement is LineBreak)
                            {
                                SelectionEndElemendEnd(startTextElement, startTextElement.ElementStart);

                                SelectionEndText(startTextElement, startTextElement.ElementStart);

                                SelectionEndNone(startTextElement, startTextElement.ElementStart);

                                SelectionEndStart(startTextElement, startTextElement.ElementStart);
                            }
                        }
                        else if (startTextPointerContext == TextPointerContext.Text)
                        {
                            TextPointer firstEndTextPointerRun = textRange.Start.GetNextContextPosition(LogicalDirection.Forward); // находим конец первого Run

                            run = (Run)firstEndTextPointerRun.GetAdjacentElement(LogicalDirection.Forward);

                            TextRange textRangeText = new TextRange(textRange.Start, firstEndTextPointerRun);

                            TextDecorationCollection collection = (TextDecorationCollection)textRangeText.GetPropertyValue(Inline.TextDecorationsProperty);
                            TextDecorationCollection newCollection = new TextDecorationCollection();

                            foreach (TextDecoration textDecoration in collection)
                            {
                                if (textDecoration.Location == TextDecorationLocation.OverLine)
                                    overLine = true;
                                if (textDecoration.Location == TextDecorationLocation.Strikethrough)
                                    strikethrough = true;
                                if (textDecoration.Location == TextDecorationLocation.Underline)
                                    underLine = true;
                            }

                            if ((bool)FontStrikethrough.IsChecked)
                                newCollection.Add(TextDecorations.Strikethrough[0]);
                            else
                            {
                                if (strikethrough)
                                    newCollection.Remove(TextDecorations.Strikethrough[0]);
                            }

                            if (overLine)
                                newCollection.Add(TextDecorations.OverLine[0]);

                            if (underLine)
                                newCollection.Add(TextDecorations.Underline[0]);

                            textRangeText.ApplyPropertyValue(Inline.TextDecorationsProperty, newCollection);

                            underLine = false;
                            overLine = false;
                            strikethrough = false;

                            // Первый элемент от него поиск                            
                            TextElement startTextElement = run;

                            SelectionEndElemendEnd(startTextElement, startTextElement.ElementStart);

                            SelectionEndText(startTextElement, startTextElement.ElementStart);

                            SelectionEndNone(startTextElement, startTextElement.ElementStart);

                            SelectionEndStart(startTextElement, startTextElement.ElementStart);
                        }
                        else if (startTextPointerContext == TextPointerContext.ElementEnd)
                        {
                            TextElement startTextElement = (TextElement)textRange.Start.GetAdjacentElement(LogicalDirection.Forward);
                                                           
                            startTextElement = (TextElement)startTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);
                          
                            SelectionEndElemendEnd(startTextElement, startTextElement.ElementStart);

                            SelectionEndText(startTextElement, startTextElement.ElementStart);

                            SelectionEndNone(startTextElement, startTextElement.ElementStart);

                            SelectionEndStart(startTextElement, startTextElement.ElementStart);                           
                        }
                    }
                    else
                    {
                        bool underLine = false;
                        bool overLine = false;
                        bool strikethrough = false;

                        TextDecorationCollection collection = (TextDecorationCollection)textRange.GetPropertyValue(Inline.TextDecorationsProperty);
                        TextDecorationCollection newCollection = new TextDecorationCollection();

                        foreach (TextDecoration textDecoration in collection)
                        {
                            if (textDecoration.Location == TextDecorationLocation.OverLine)
                                overLine = true;
                            if (textDecoration.Location == TextDecorationLocation.Strikethrough)
                                strikethrough = true;
                            if (textDecoration.Location == TextDecorationLocation.Underline)
                                underLine = true;
                        }

                        if ((bool)FontStrikethrough.IsChecked)
                            newCollection.Add(TextDecorations.Strikethrough[0]);
                        else
                        {
                            if (strikethrough)
                                newCollection.Remove(TextDecorations.Strikethrough[0]);
                        }

                        if (overLine)
                            newCollection.Add(TextDecorations.OverLine[0]);

                        if (underLine)
                            newCollection.Add(TextDecorations.Underline[0]);

                        textRange.ApplyPropertyValue(Inline.TextDecorationsProperty, newCollection);
                    }
                }
                else
                {
                    bool strikethrough = false;

                    foreach (TextDecoration textDecoration in Run.TextDecorations)
                    {
                        if (textDecoration.Location == TextDecorationLocation.Strikethrough)
                            strikethrough = true;
                    }

                    if ((bool)FontStrikethrough.IsChecked)
                    {
                        if (!strikethrough)
                        {
                            IsCreateRunStrikethrough = true;

                            CreateRunTextPointer = CaretPosition;
                        }
                        else
                        {
                            IsCreateRunStrikethrough = false;
                        }
                    }
                    else
                    {
                        if (strikethrough)
                        {
                            IsCreateRunStrikethrough = true;

                            CreateRunTextPointer = CaretPosition;
                        }
                        else
                        {
                            IsCreateRunStrikethrough = false;
                        }
                    }   
                }                
            }
            else
            {                              
                object obj = Selection.GetPropertyValue(Inline.TextDecorationsProperty);

                if (obj == DependencyProperty.UnsetValue)
                {
                    Run run = null;

                    bool underLine = false;
                    bool overLine = false;
                    bool strikethrough = false;

                    TextPointerContext startTextPointerContext = Selection.Start.GetPointerContext(LogicalDirection.Forward);
                   
                    if (startTextPointerContext == TextPointerContext.ElementStart)
                    {
                        TextElement startTextElement = (TextElement)Selection.Start.GetAdjacentElement(LogicalDirection.Forward);
                        
                        if (startTextElement is LineBreak)
                        {
                            SelectionEndElemendEnd(startTextElement, startTextElement.ElementStart);

                            SelectionEndText(startTextElement, startTextElement.ElementStart);

                            SelectionEndNone(startTextElement, startTextElement.ElementStart);

                            SelectionEndStart(startTextElement, startTextElement.ElementStart);
                        }                                            
                    }
                    else if (startTextPointerContext == TextPointerContext.Text)
                    {
                        TextPointer firstEndTextPointerRun = Selection.Start.GetNextContextPosition(LogicalDirection.Forward); // находим конец первого Run

                        run = (Run)firstEndTextPointerRun.GetAdjacentElement(LogicalDirection.Forward);

                        TextRange textRange = new TextRange(Selection.Start, firstEndTextPointerRun);

                        TextDecorationCollection collection = (TextDecorationCollection)textRange.GetPropertyValue(Inline.TextDecorationsProperty);
                        TextDecorationCollection newCollection = new TextDecorationCollection();

                        foreach (TextDecoration textDecoration in collection)
                        {
                            if (textDecoration.Location == TextDecorationLocation.OverLine)
                                overLine = true;
                            if (textDecoration.Location == TextDecorationLocation.Strikethrough)
                                strikethrough = true;
                            if (textDecoration.Location == TextDecorationLocation.Underline)
                                underLine = true;
                        }

                        if ((bool)FontStrikethrough.IsChecked)                         
                            newCollection.Add(TextDecorations.Strikethrough[0]);
                        else
                        {
                            if (strikethrough)
                                newCollection.Remove(TextDecorations.Strikethrough[0]);
                        }

                        if (overLine)
                            newCollection.Add(TextDecorations.OverLine[0]);

                        if (underLine)
                            newCollection.Add(TextDecorations.Underline[0]);

                        textRange.ApplyPropertyValue(Inline.TextDecorationsProperty, newCollection);

                        underLine = false;
                        overLine = false;
                        strikethrough = false;

                        // Первый элемент от него поиск
                        TextPointer textPointer = run.ElementStart;
                        TextElement startTextElement = run;

                        SelectionEndElemendEnd(startTextElement, textPointer);

                        SelectionEndText(startTextElement, textPointer);

                        SelectionEndNone(startTextElement, textPointer);

                        SelectionEndStart(startTextElement, textPointer);
                    }
                    else if (startTextPointerContext == TextPointerContext.ElementEnd)
                    {
                        TextElement startTextElement = (TextElement)Selection.Start.GetAdjacentElement(LogicalDirection.Forward);
                                                                      
                        SelectionEndElemendEnd(startTextElement, startTextPointer);

                        SelectionEndText(startTextElement, startTextPointer);

                        SelectionEndNone(startTextElement, startTextPointer);

                        SelectionEndStart(startTextElement, startTextPointer);                       
                    }                 
                }
                else
                {
                    bool underLine = false;
                    bool overLine = false;
                    bool strikethrough = false;

                    TextDecorationCollection collection = (TextDecorationCollection)Selection.GetPropertyValue(Inline.TextDecorationsProperty);
                    TextDecorationCollection newCollection = new TextDecorationCollection();

                    foreach (TextDecoration textDecoration in collection)
                    {
                        if (textDecoration.Location == TextDecorationLocation.OverLine)
                            overLine = true;
                        if (textDecoration.Location == TextDecorationLocation.Strikethrough)
                            strikethrough = true;
                        if (textDecoration.Location == TextDecorationLocation.Underline)
                            underLine = true;
                    }

                    if ((bool)FontStrikethrough.IsChecked)                      
                        newCollection.Add(TextDecorations.Strikethrough[0]);
                    else
                    {
                        if (strikethrough)
                            newCollection.Remove(TextDecorations.Strikethrough[0]);
                    }

                    if (overLine)
                        newCollection.Add(TextDecorations.OverLine[0]);

                    if (underLine)
                        newCollection.Add(TextDecorations.Underline[0]);

                    Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, newCollection);
                }
            }

            e.Handled = true;
        }

        void FontUnderline_Click(object sender, RoutedEventArgs e)
        {
            TextPointer textPoint = CaretPosition;
            startTextPointer = null;
            endTextPointer = null;

            if (Selection.IsEmpty)
            {
                TextElement textElement = null;

                if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.None)
                {
                    IsCreateRunNull = true;

                    CreateRunTextPointer = CaretPosition;

                    return;
                }
                else if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementStart)
                {
                    textElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                    if (textElement is Paragraph)
                    {
                        IsCreateRunNull = true;

                        CreateRunTextPointer = CaretPosition;

                        return;
                    }
                    else if (textElement is ListItem)
                    {
                        IsCreateRunNull = true;

                        CreateRunTextPointer = CaretPosition;

                        return;
                    }
                }
                else if (CaretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementEnd)
                {
                    textElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                    if (textElement is LineBreak)
                    {
                        IsCreateRunNull = true;

                        CreateRunTextPointer = CaretPosition;

                        return;
                    }
                }

                if (textPoint.GetTextRunLength(LogicalDirection.Backward) > 0) // Если позади каретки в текстовом элементе есть текст
                {
                    TextPointer textPointer = textPoint.GetNextContextPosition(LogicalDirection.Backward);

                    Run run = (Run)textPointer.GetAdjacentElement(LogicalDirection.Backward);

                    TextElement nextBackTextElement = (TextElement)run.ElementStart.GetAdjacentElement(LogicalDirection.Backward);

                    int i = CaretPosition.GetTextRunLength(LogicalDirection.Backward);

                    TextRange textRange = new TextRange(textPointer, CaretPosition);

                    int index = textRange.Text.LastIndexOfAny(new char[] { ' ', ',', '.', '?', '!', ':', ';' });

                    if (index != -1)
                    {
                        if (textRange.Text[i - 1] != ' ' && textRange.Text[i - 1] != ',' && textRange.Text[i - 1] != '.' && textRange.Text[i - 1] != '?' && textRange.Text[i - 1] != '!' && textRange.Text[i - 1] != ':' && textRange.Text[i - 1] != ';')
                        {
                            startTextPointer = CaretPosition.GetPositionAtOffset(index + 1 - i);
                        }
                        else
                            startTextPointer = null;
                    }
                    else
                    {
                        IsText = true;

                        StartTextPointer(nextBackTextElement);
                    }
                }
                else
                {
                    IsTextNull = true;

                    TextElement nextBackTextElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Backward);

                    if (nextBackTextElement is Run)
                    {
                        nextBackTextElement = (TextElement)nextBackTextElement.ElementStart.GetAdjacentElement(LogicalDirection.Backward);
                    }

                    StartTextPointer(nextBackTextElement);
                }

                IsText = false;

                IsTextNull = false;

                if (textPoint.GetTextRunLength(LogicalDirection.Forward) > 0) // Если в текстовом элементе впереди каретки есть текст
                {
                    TextPointer textPointer = textPoint.GetNextContextPosition(LogicalDirection.Forward);

                    Run run = (Run)textPointer.GetAdjacentElement(LogicalDirection.Forward);

                    TextElement nextForwardTextElement = (TextElement)run.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);

                    int i = CaretPosition.GetTextRunLength(LogicalDirection.Forward);

                    TextRange textRange = new TextRange(CaretPosition, textPointer);

                    int index = textRange.Text.IndexOfAny(new char[] { ' ', ',', '.', '?', '!', ':', ';' });

                    if (index != -1)
                    {
                        if (textRange.Text[0] != ' ' && textRange.Text[0] != ',' && textRange.Text[0] != '.' && textRange.Text[0] != '?' && textRange.Text[0] != '!' && textRange.Text[0] != ':' && textRange.Text[0] != ';')
                        {
                            endTextPointer = CaretPosition.GetPositionAtOffset(index);
                        }
                        else
                            endTextPointer = null;
                    }
                    else
                    {
                        IsText = true;

                        EndTextPointer(nextForwardTextElement);
                    }
                }
                else
                {
                    IsTextNull = true;

                    TextElement nextForwardTextElement = (TextElement)CaretPosition.GetAdjacentElement(LogicalDirection.Forward);

                    if (nextForwardTextElement is Run)
                    {
                        nextForwardTextElement = (TextElement)nextForwardTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);
                    }

                    EndTextPointer(nextForwardTextElement);
                }

                IsText = false;

                IsTextNull = false;

                if (startTextPointer != null && endTextPointer != null)
                {
                    TextRange textRange = new TextRange(startTextPointer, endTextPointer);

                    object obj = textRange.GetPropertyValue(Inline.TextDecorationsProperty);

                    if (obj == DependencyProperty.UnsetValue)
                    {
                        Run run = null;

                        bool underLine = false;
                        bool overLine = false;
                        bool strikethrough = false;

                        TextPointerContext startTextPointerContext = textRange.Start.GetPointerContext(LogicalDirection.Forward);

                        if (startTextPointerContext == TextPointerContext.ElementStart)
                        {
                            TextElement startTextElement = (TextElement)textRange.Start.GetAdjacentElement(LogicalDirection.Forward);

                            if (startTextElement is LineBreak)
                            {
                                SelectionEndElemendEnd(startTextElement, startTextElement.ElementStart);

                                SelectionEndText(startTextElement, startTextElement.ElementStart);

                                SelectionEndNone(startTextElement, startTextElement.ElementStart);

                                SelectionEndStart(startTextElement, startTextElement.ElementStart);
                            }
                        }
                        else if (startTextPointerContext == TextPointerContext.Text)
                        {
                            TextPointer endTextPointerSelection = textRange.Start.GetNextContextPosition(LogicalDirection.Forward); // находим конец первого Run

                            run = (Run)endTextPointerSelection.GetAdjacentElement(LogicalDirection.Forward);

                            TextRange textRangeText = new TextRange(textRange.Start, endTextPointerSelection);

                            TextDecorationCollection collection = (TextDecorationCollection)textRangeText.GetPropertyValue(Inline.TextDecorationsProperty);
                            TextDecorationCollection newCollection = new TextDecorationCollection();

                            foreach (TextDecoration textDecoration in collection)
                            {
                                if (textDecoration.Location == TextDecorationLocation.OverLine)
                                    overLine = true;
                                if (textDecoration.Location == TextDecorationLocation.Strikethrough)
                                    strikethrough = true;
                                if (textDecoration.Location == TextDecorationLocation.Underline)
                                    underLine = true;
                            }

                            if ((bool)FontUnderline.IsChecked)
                                newCollection.Add(TextDecorations.Underline[0]);
                            else
                            {
                                if (underLine)
                                    newCollection.Remove(TextDecorations.Underline[0]);
                            }

                            if (strikethrough)
                                newCollection.Add(TextDecorations.Strikethrough[0]);

                            if (overLine)
                                newCollection.Add(TextDecorations.OverLine[0]);

                            textRangeText.ApplyPropertyValue(Inline.TextDecorationsProperty, newCollection);

                            underLine = false;
                            overLine = false;
                            strikethrough = false;

                            // Переходим к следующему элементу
                            TextPointer textPointer = run.ElementEnd;
                            TextElement startTextElement = (TextElement)textPointer.GetAdjacentElement(LogicalDirection.Forward);

                            SelectionEndElemendEnd(startTextElement, textPointer);

                            SelectionEndText(startTextElement, textPointer);

                            SelectionEndNone(startTextElement, textPointer);

                            SelectionEndStart(startTextElement, textPointer);
                        }
                        else if (startTextPointerContext == TextPointerContext.ElementEnd)
                        {
                            TextElement startTextElement = (TextElement)textRange.Start.GetAdjacentElement(LogicalDirection.Forward);

                            if (startTextElement is Run)
                            {
                                // Переходим к следующему элементу
                                startTextPointer = startTextElement.ElementEnd;

                                startTextElement = (TextElement)startTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);

                                SelectionEndElemendEnd(startTextElement, startTextPointer);

                                SelectionEndText(startTextElement, startTextPointer);

                                SelectionEndNone(startTextElement, startTextPointer);

                                SelectionEndStart(startTextElement, startTextPointer);
                            }
                            else
                            {
                                SelectionEndElemendEnd(startTextElement, startTextElement.ContentEnd);

                                SelectionEndText(startTextElement, startTextElement.ContentEnd);

                                SelectionEndNone(startTextElement, startTextElement.ContentEnd);

                                SelectionEndStart(startTextElement, startTextElement.ContentEnd);
                            }
                        }
                    }
                    else
                    {
                        bool underLine = false;
                        bool overLine = false;
                        bool strikethrough = false;

                        TextDecorationCollection collection = (TextDecorationCollection)textRange.GetPropertyValue(Inline.TextDecorationsProperty);
                        TextDecorationCollection newCollection = new TextDecorationCollection();

                        foreach (TextDecoration textDecoration in collection)
                        {
                            if (textDecoration.Location == TextDecorationLocation.OverLine)
                                overLine = true;
                            if (textDecoration.Location == TextDecorationLocation.Strikethrough)
                                strikethrough = true;
                            if (textDecoration.Location == TextDecorationLocation.Underline)
                                underLine = true;
                        }

                        if ((bool)FontUnderline.IsChecked)
                            newCollection.Add(TextDecorations.Underline[0]);
                        else
                        {
                            if (underLine)
                                newCollection.Remove(TextDecorations.Underline[0]);
                        }

                        if (strikethrough)
                            newCollection.Add(TextDecorations.Strikethrough[0]);

                        if (overLine)
                            newCollection.Add(TextDecorations.OverLine[0]);

                        textRange.ApplyPropertyValue(Inline.TextDecorationsProperty, newCollection);
                    }
                }
                else
                {
                    bool underLine = false;

                    foreach (TextDecoration textDecoration in Run.TextDecorations)
                    {
                        if (textDecoration.Location == TextDecorationLocation.Underline)
                            underLine = true;
                    }

                    if ((bool)FontUnderline.IsChecked)
                    {
                        if (!underLine)
                        {
                            IsCreateRunUnderline = true;

                            CreateRunTextPointer = CaretPosition;
                        }
                        else
                        {
                            IsCreateRunUnderline = false;
                        }
                    }
                    else
                    {
                        if (underLine)
                        {
                            IsCreateRunUnderline = true;

                            CreateRunTextPointer = CaretPosition;
                        }
                        else
                        {
                            IsCreateRunUnderline = false;
                        }
                    }
                }
            }
            else
            {
                object obj = Selection.GetPropertyValue(Inline.TextDecorationsProperty);

                if (obj == DependencyProperty.UnsetValue)
                {
                    Run run = null;

                    bool underLine = false;
                    bool overLine = false;
                    bool strikethrough = false;

                    TextPointerContext startTextPointerContext = Selection.Start.GetPointerContext(LogicalDirection.Forward);

                    if (startTextPointerContext == TextPointerContext.ElementStart)
                    {
                        TextElement startTextElement = (TextElement)Selection.Start.GetAdjacentElement(LogicalDirection.Forward);

                        if (startTextElement is LineBreak)
                        {
                            SelectionEndElemendEnd(startTextElement, startTextElement.ElementStart);

                            SelectionEndText(startTextElement, startTextElement.ElementStart);

                            SelectionEndNone(startTextElement, startTextElement.ElementStart);

                            SelectionEndStart(startTextElement, startTextElement.ElementStart);
                        }
                    }
                    else if (startTextPointerContext == TextPointerContext.Text)
                    {
                        TextPointer endTextPointerSelection = Selection.Start.GetNextContextPosition(LogicalDirection.Forward); // находим конец первого Run

                        run = (Run)endTextPointerSelection.GetAdjacentElement(LogicalDirection.Forward);

                        TextRange textRange = new TextRange(Selection.Start, endTextPointerSelection);

                        TextDecorationCollection collection = (TextDecorationCollection)textRange.GetPropertyValue(Inline.TextDecorationsProperty);
                        TextDecorationCollection newCollection = new TextDecorationCollection();

                        foreach (TextDecoration textDecoration in collection)
                        {
                            if (textDecoration.Location == TextDecorationLocation.OverLine)
                                overLine = true;
                            if (textDecoration.Location == TextDecorationLocation.Strikethrough)
                                strikethrough = true;
                            if (textDecoration.Location == TextDecorationLocation.Underline)
                                underLine = true;
                        }

                        if ((bool)FontUnderline.IsChecked)
                            newCollection.Add(TextDecorations.Underline[0]);
                        else
                        {
                            if (underLine)
                                newCollection.Remove(TextDecorations.Underline[0]);
                        }

                        if (strikethrough)
                            newCollection.Add(TextDecorations.Strikethrough[0]);

                        if (overLine)
                            newCollection.Add(TextDecorations.OverLine[0]);

                        textRange.ApplyPropertyValue(Inline.TextDecorationsProperty, newCollection);

                        underLine = false;
                        overLine = false;
                        strikethrough = false;

                        // Переходим к следующему элементу
                        TextPointer textPointer = run.ElementEnd;
                        TextElement startTextElement = (TextElement)textPointer.GetAdjacentElement(LogicalDirection.Forward);

                        SelectionEndElemendEnd(startTextElement, textPointer);

                        SelectionEndText(startTextElement, textPointer);

                        SelectionEndNone(startTextElement, textPointer);

                        SelectionEndStart(startTextElement, textPointer);
                    }
                    else if (startTextPointerContext == TextPointerContext.ElementEnd)
                    {
                        TextElement startTextElement = (TextElement)Selection.Start.GetAdjacentElement(LogicalDirection.Forward);

                        if (startTextElement is Run)
                        {
                            // Переходим к следующему элементу
                            startTextPointer = startTextElement.ElementEnd;

                            startTextElement = (TextElement)startTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);

                            SelectionEndElemendEnd(startTextElement, startTextPointer);

                            SelectionEndText(startTextElement, startTextPointer);

                            SelectionEndNone(startTextElement, startTextPointer);

                            SelectionEndStart(startTextElement, startTextPointer);
                        }
                        else
                        {
                            SelectionEndElemendEnd(startTextElement, startTextElement.ContentEnd);

                            SelectionEndText(startTextElement, startTextElement.ContentEnd);

                            SelectionEndNone(startTextElement, startTextElement.ContentEnd);

                            SelectionEndStart(startTextElement, startTextElement.ContentEnd);
                        }
                    }
                }
                else
                {
                    bool underLine = false;
                    bool overLine = false;
                    bool strikethrough = false;

                    TextDecorationCollection collection = (TextDecorationCollection)Selection.GetPropertyValue(Inline.TextDecorationsProperty);
                    TextDecorationCollection newCollection = new TextDecorationCollection();

                    foreach (TextDecoration textDecoration in collection)
                    {
                        if (textDecoration.Location == TextDecorationLocation.OverLine)
                            overLine = true;
                        if (textDecoration.Location == TextDecorationLocation.Strikethrough)
                            strikethrough = true;
                        if (textDecoration.Location == TextDecorationLocation.Underline)
                            underLine = true;
                    }

                    if ((bool)FontUnderline.IsChecked)
                        newCollection.Add(TextDecorations.Underline[0]);
                    else
                    {
                        if (underLine)
                            newCollection.Remove(TextDecorations.Underline[0]);
                    }

                    if (strikethrough)
                        newCollection.Add(TextDecorations.Strikethrough[0]);

                    if (overLine)
                        newCollection.Add(TextDecorations.OverLine[0]);

                    Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, newCollection);
                }
            }

            e.Handled = true;           
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyUp(e);

            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.V)
            {
                IsLeftSpace = false;
                IsPaste = true;

                CreateRunFalse();
            }                          
        }
      
        protected override void OnSelectionChanged(RoutedEventArgs e)
        {           
            if (IsLeftSpace)
            {
                if (CaretPosition.GetOffsetToPosition(CreateRunTextPointer) != 0)
                    IsLeftSpace = false;
            }              

            // Иначе при IsCreateRun true и перемещении каретки без события OnPreviewTextInput создается объект Run в нетом месте
            if (IsCreateRunBold || IsCreateRunColor || IsCreateRunFont || IsCreateRunItalic || IsCreateRunOverline || IsCreateRunScript || IsCreateRunSize || IsCreateRunStrikethrough || IsCreateRunUnderline || IsCreateRunNull)
            {
                if (CaretPosition.GetOffsetToPosition(CreateRunTextPointer) != 0)
                    CreateRunFalse();
            }
            
            base.OnSelectionChanged(e);
            
            if (IsPaste)
            {
                IsPaste = false;
              
                TextElement firstTextElement = (TextElement)this.Document.ContentStart.GetAdjacentElement(LogicalDirection.Forward);

                TextElement nextTextElement = null;

                TextPointer textPointer = null;

                Paragraph paragraph = null;

                if (firstTextElement is Paragraph)
                {
                    nextTextElement = (TextElement)firstTextElement.ContentStart.GetAdjacentElement(LogicalDirection.Forward);
                    paragraph = firstTextElement as Paragraph;

                    textPointer = firstTextElement.ElementStart;
                }
                else if (firstTextElement is List)
                {
                    nextTextElement = (TextElement)firstTextElement.ContentStart.GetAdjacentElement(LogicalDirection.Forward);                  

                    textPointer = firstTextElement.ElementStart;
                }
                    
                bool next = true;

                while (next)
                {
                    if (nextTextElement is Span)
                    {
                        Span span = nextTextElement as Span;
                        TextPointer textpointer = span.ElementStart;
                        Run run = (Run)span.ContentStart.GetAdjacentElement(LogicalDirection.Forward);

                        Run run1 = new Run(run.Text, textpointer);
                        run1.FontFamily = span.FontFamily;
                        run1.FontSize = span.FontSize;
                        run1.FontStretch = span.FontStretch;
                        run1.FontStyle = span.FontStyle;
                        run1.FontWeight = span.FontWeight;
                        run1.ForceCursor = span.ForceCursor;
                        run1.Foreground = span.Foreground;
                        run1.BaselineAlignment = span.BaselineAlignment;

                        run1.Typography.AnnotationAlternates = span.Typography.AnnotationAlternates;
                        run1.Typography.Capitals = span.Typography.Capitals;
                        run1.Typography.CapitalSpacing = span.Typography.CapitalSpacing;
                        run1.Typography.CaseSensitiveForms = span.Typography.CaseSensitiveForms;
                        run1.Typography.ContextualAlternates = span.Typography.ContextualAlternates;
                        run1.Typography.ContextualLigatures = span.Typography.ContextualLigatures;
                        run1.Typography.ContextualSwashes = span.Typography.ContextualSwashes;
                        run1.Typography.DiscretionaryLigatures = span.Typography.DiscretionaryLigatures;
                        run1.Typography.EastAsianExpertForms = span.Typography.EastAsianExpertForms;
                        run1.Typography.EastAsianLanguage = span.Typography.EastAsianLanguage;
                        run1.Typography.EastAsianWidths = span.Typography.EastAsianWidths;
                        run1.Typography.Fraction = span.Typography.Fraction;
                        run1.Typography.HistoricalForms = span.Typography.HistoricalForms;
                        run1.Typography.HistoricalLigatures = span.Typography.HistoricalLigatures;
                        run1.Typography.Kerning = span.Typography.Kerning;
                        run1.Typography.MathematicalGreek = span.Typography.MathematicalGreek;
                        run1.Typography.NumeralAlignment = span.Typography.NumeralAlignment;
                        run1.Typography.NumeralStyle = span.Typography.NumeralStyle;
                        run1.Typography.SlashedZero = span.Typography.SlashedZero;
                        run1.Typography.StandardLigatures = span.Typography.StandardLigatures;
                        run1.Typography.StandardSwashes = span.Typography.StandardSwashes;
                        run1.Typography.StylisticAlternates = span.Typography.StylisticAlternates;
                        run1.Typography.StylisticSet1 = span.Typography.StylisticSet1;
                        run1.Typography.StylisticSet10 = span.Typography.StylisticSet10;
                        run1.Typography.StylisticSet11 = span.Typography.StylisticSet11;
                        run1.Typography.StylisticSet12 = span.Typography.StylisticSet12;
                        run1.Typography.StylisticSet13 = span.Typography.StylisticSet13;
                        run1.Typography.StylisticSet14 = span.Typography.StylisticSet14;
                        run1.Typography.StylisticSet15 = span.Typography.StylisticSet15;
                        run1.Typography.StylisticSet16 = span.Typography.StylisticSet16;
                        run1.Typography.StylisticSet17 = span.Typography.StylisticSet17;
                        run1.Typography.StylisticSet18 = span.Typography.StylisticSet18;
                        run1.Typography.StylisticSet19 = span.Typography.StylisticSet19;
                        run1.Typography.StylisticSet2 = span.Typography.StylisticSet2;
                        run1.Typography.StylisticSet20 = span.Typography.StylisticSet20;
                        run1.Typography.StylisticSet3 = span.Typography.StylisticSet3;
                        run1.Typography.StylisticSet4 = span.Typography.StylisticSet4;
                        run1.Typography.StylisticSet5 = span.Typography.StylisticSet5;
                        run1.Typography.StylisticSet6 = span.Typography.StylisticSet6;
                        run1.Typography.StylisticSet7 = span.Typography.StylisticSet7;
                        run1.Typography.StylisticSet8 = span.Typography.StylisticSet8;
                        run1.Typography.StylisticSet9 = span.Typography.StylisticSet9;

                        run1.Typography.Variants = span.Typography.Variants;

                        run1.TextDecorations = span.TextDecorations;

                        paragraph.Inlines.Remove(span);
                        nextTextElement = (TextElement)run1.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);
                        textPointer = run1.ElementEnd;
                    }
                    else if (nextTextElement is Paragraph)
                    {
                        if (textPointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementStart)
                        {                         
                            paragraph = nextTextElement as Paragraph;

                            textPointer = paragraph.ContentStart;

                            nextTextElement = (TextElement)nextTextElement.ContentStart.GetAdjacentElement(LogicalDirection.Forward);                           
                        }
                        else if (textPointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementEnd)
                        {
                            nextTextElement = (TextElement)nextTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);

                            if (nextTextElement is Paragraph)
                            {
                                paragraph = nextTextElement as Paragraph;

                                nextTextElement = (TextElement)paragraph.ContentStart.GetAdjacentElement(LogicalDirection.Forward);

                                textPointer = paragraph.ContentStart;
                            }
                                
                            if(nextTextElement == null)
                                next = false;
                        }
                    }
                    else if (nextTextElement is LineBreak)
                    {
                        textPointer = nextTextElement.ElementEnd;

                        nextTextElement = (TextElement)nextTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);
                    }
                    else if (nextTextElement is List || nextTextElement is ListItem)
                    {
                        if (textPointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementEnd)
                        {
                            textPointer = nextTextElement.ElementEnd;

                            nextTextElement = (TextElement)nextTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);
                           
                            if (nextTextElement == null)
                                next = false;
                        }
                        else if (textPointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementStart)
                        {
                            textPointer = nextTextElement.ContentStart;

                            nextTextElement = (TextElement)nextTextElement.ContentStart.GetAdjacentElement(LogicalDirection.Forward); 
                        }
                    }
                    else if (nextTextElement is Run)
                    {
                        textPointer = nextTextElement.ElementEnd;

                        nextTextElement = (TextElement)nextTextElement.ElementEnd.GetAdjacentElement(LogicalDirection.Forward);
                    }
                }               
            }

            if (!IsCreateRunBold && !IsCreateRunColor && !IsCreateRunFont && !IsCreateRunItalic && !IsCreateRunOverline && !IsCreateRunScript && !IsCreateRunSize && !IsCreateRunStrikethrough && !IsCreateRunUnderline && !IsCreateRunNull)
            {
                UpdateToolBar();
            }
            
            e.Handled = true;
        }

        void CreateRunFalse()
        {
            IsCreateRunNull = false;
            IsCreateRunBold = false;
            IsCreateRunColor = false;
            IsCreateRunFont = false;
            IsCreateRunItalic = false;
            IsCreateRunOverline = false;
            IsCreateRunScript = false;
            IsCreateRunSize = false;
            IsCreateRunStrikethrough = false;
            IsCreateRunUnderline = false;
            IsTextNull = false;
            IsText = false;
        }
      
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            if (e.UndoAction != UndoAction.Clear)
            {
                TextEditor.IsSave = true;
            }

            if (IsLeftSpace && (!IsCreateRunBold && !IsCreateRunColor && !IsCreateRunFont && !IsCreateRunItalic && !IsCreateRunOverline && !IsCreateRunScript && !IsCreateRunSize && !IsCreateRunStrikethrough && !IsCreateRunUnderline && !IsCreateRunNull))
            {
                IsLeftSpace = false;

                TextRange range = new TextRange(this.Selection.End.GetPositionAtOffset(-1), this.Selection.End);

                range.ApplyPropertyValue(FontWeightProperty, Run.FontWeight);
                range.ApplyPropertyValue(FontStyleProperty, Run.FontStyle);
                range.ApplyPropertyValue(Run.ForegroundProperty, Run.Foreground);
                range.ApplyPropertyValue(FontFamilyProperty, Run.FontFamily);
                range.ApplyPropertyValue(FontSizeProperty, Run.FontSize);
                range.ApplyPropertyValue(Inline.TextDecorationsProperty, Run.TextDecorations);
                range.ApplyPropertyValue(Inline.BaselineAlignmentProperty, Run.BaselineAlignment);
                range.ApplyPropertyValue(Typography.VariantsProperty, Run.Typography.Variants);
                range.ApplyPropertyValue(Typography.FractionProperty, Run.Typography.Fraction);
                range.ApplyPropertyValue(Typography.NumeralAlignmentProperty, Run.Typography.NumeralAlignment);
                range.ApplyPropertyValue(Typography.SlashedZeroProperty, Run.Typography.SlashedZero);
            }

            if (IsCreateRunBold || IsCreateRunColor || IsCreateRunFont || IsCreateRunItalic || IsCreateRunOverline || IsCreateRunScript || IsCreateRunSize || IsCreateRunStrikethrough || IsCreateRunUnderline  || IsCreateRunNull)
            {
                if (IsLeftSpace)
                {
                    IsLeftSpace = false;
                }

                CreateRunFalse();
                  
                TextRange range = new TextRange(this.Selection.End.GetPositionAtOffset(-1), this.Selection.End);

                TextDecorationCollection textDecCollection = new TextDecorationCollection();

                if (FontBold.IsChecked == true)
                {
                    range.ApplyPropertyValue(FontWeightProperty, FontWeights.Bold);
                }
                else
                {
                    range.ApplyPropertyValue(FontWeightProperty, FontWeights.Normal);
                }

                if (FontItalic.IsChecked == true)
                {
                    range.ApplyPropertyValue(FontStyleProperty, FontStyles.Italic);
                }
                else
                {
                    range.ApplyPropertyValue(FontStyleProperty, FontStyles.Normal);
                }

                if (FontUnderline.IsChecked == true)
                {
                    textDecCollection.Add(TextDecorations.Underline[0]);
                }
                else
                {
                    textDecCollection.Remove(TextDecorations.Underline[0]);
                }

                if (FontStrikethrough.IsChecked == true)
                {
                    textDecCollection.Add(TextDecorations.Strikethrough[0]);
                }
                else
                {
                    textDecCollection.Remove(TextDecorations.Strikethrough[0]);
                }

                if (FontOverline.IsChecked == true)
                {
                    textDecCollection.Add(TextDecorations.OverLine[0]);
                }
                else
                {
                    textDecCollection.Remove(TextDecorations.OverLine[0]);
                }

                if (FontSubscript.IsChecked == true)
                {
                    range.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Subscript);
                }
                else if (FontSuperscript.IsChecked == true)
                {
                    range.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Superscript);
                }
                else
                {
                    range.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Baseline);
                }

                range.ApplyPropertyValue(Inline.TextDecorationsProperty, textDecCollection);
                range.ApplyPropertyValue(Run.ForegroundProperty, RectangleColor.Fill);
                range.ApplyPropertyValue(Run.FontFamilyProperty, ComboBoxFont.SelectedItem);

                PresentationSource source = PresentationSource.FromVisual(this);

                double dpiX = 0, dpiY = 0;
                if (source != null)
                {
                    dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                    dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
                }

                double d = dpiX / 72;

                double dText = double.Parse(ComboBoxFontSize.Text);

                range.ApplyPropertyValue(Run.FontSizeProperty, dText * d);

                if (!IsCreateRunNull)
                {
                    range.ApplyPropertyValue(Typography.VariantsProperty, Run.Typography.Variants);
                    range.ApplyPropertyValue(Typography.FractionProperty, Run.Typography.Fraction);
                    range.ApplyPropertyValue(Typography.NumeralAlignmentProperty, Run.Typography.NumeralAlignment);
                    range.ApplyPropertyValue(Typography.SlashedZeroProperty, Run.Typography.SlashedZero);
                }                   
            }                                 
        }       
    }
}
