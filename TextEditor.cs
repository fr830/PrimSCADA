// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SCADA
{
    public class TextEditor : ContentControl
    {
        public Grid grid = new Grid();
        public RowDefinition row0 = new RowDefinition();
        public RowDefinition row1 = new RowDefinition();
        public CustomRichTextBox richTextBox; 
        public ToolBar toolBar = new ToolBar();

        public bool IsSave;

        private ObservableCollection<FontFamily> _systemFonts = new ObservableCollection<FontFamily>();
        public ObservableCollection<FontFamily> SystemFonts
        {
            get { return _systemFonts; }
        }

        public void LoadSystemFonts()
        {
            _systemFonts.Clear();

            var fonts = Fonts.SystemFontFamilies.OrderBy(f => f.ToString());

            foreach (var f in fonts)
                _systemFonts.Add(f);
        }

        static TextEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextEditor), new FrameworkPropertyMetadata(typeof(ContentControl)));
        }

        public TextEditor(FlowDocument flowDocument)
        {
            richTextBox = new CustomRichTextBox(this);

            KeyBinding keyBindingBold = new KeyBinding(ApplicationCommands.NotACommand, Key.B, ModifierKeys.Control);
            KeyBinding keyBindingSizeUp = new KeyBinding(ApplicationCommands.NotACommand, Key.OemCloseBrackets, ModifierKeys.Control);
            KeyBinding keyBindingSizeDown = new KeyBinding(ApplicationCommands.NotACommand, Key.OemOpenBrackets, ModifierKeys.Control);
            KeyBinding keyBindingItalic = new KeyBinding(ApplicationCommands.NotACommand, Key.I, ModifierKeys.Control);
            KeyBinding keyBindingUnderline = new KeyBinding(ApplicationCommands.NotACommand, Key.U, ModifierKeys.Control);
            KeyBinding keyBindingSuperscript = new KeyBinding(ApplicationCommands.NotACommand, Key.OemPlus, ModifierKeys.Control);
            KeyBinding keyBindingSubscript = new KeyBinding(ApplicationCommands.NotACommand, Key.OemPlus, ModifierKeys.Control | ModifierKeys.Shift);                     
            KeyBinding keyBindingAlignLeft = new KeyBinding(ApplicationCommands.NotACommand, Key.L, ModifierKeys.Control);
            KeyBinding keyBindingAlignCenter = new KeyBinding(ApplicationCommands.NotACommand, Key.E, ModifierKeys.Control);
            KeyBinding keyBindingAlignRight = new KeyBinding(ApplicationCommands.NotACommand, Key.R, ModifierKeys.Control);
            KeyBinding keyBindingAlignFull = new KeyBinding(ApplicationCommands.NotACommand, Key.J, ModifierKeys.Control);
            richTextBox.InputBindings.Add(keyBindingBold);
            richTextBox.InputBindings.Add(keyBindingSizeUp);
            richTextBox.InputBindings.Add(keyBindingSizeDown);
            richTextBox.InputBindings.Add(keyBindingItalic);
            richTextBox.InputBindings.Add(keyBindingUnderline);
            richTextBox.InputBindings.Add(keyBindingSuperscript);
            richTextBox.InputBindings.Add(keyBindingSubscript);           
            richTextBox.InputBindings.Add(keyBindingAlignLeft);
            richTextBox.InputBindings.Add(keyBindingAlignCenter);
            richTextBox.InputBindings.Add(keyBindingAlignRight);
            richTextBox.InputBindings.Add(keyBindingAlignFull);

            Image imageOpenFile = new Image();
            imageOpenFile.Source = new BitmapImage(new Uri("Images/OpenFileRichTextBox22.png", UriKind.Relative));

            Image imageSaveFile = new Image();
            imageSaveFile.Source = new BitmapImage(new Uri("Images/Save24.png", UriKind.Relative));

            Image imageFontBold = new Image();
            imageFontBold.Source = new BitmapImage(new Uri("Images/FontBold24.png", UriKind.Relative));

            Image imageFontItalic = new Image();
            imageFontItalic.Source = new BitmapImage(new Uri("Images/FontItalic24.png", UriKind.Relative));

            Image imageFontUnderline = new Image();
            imageFontUnderline.Source = new BitmapImage(new Uri("Images/FontUnderline24.png", UriKind.Relative));

            Image imageStrikethrough = new Image();
            imageStrikethrough.Source = new BitmapImage(new Uri("Images/Strikethrough24.png", UriKind.Relative));

            Image imageOverline = new Image();
            imageOverline.Source = new BitmapImage(new Uri("Images/Overline16.png", UriKind.Relative));

            Image imageFontSuperscript = new Image();
            imageFontSuperscript.Source = new BitmapImage(new Uri("Images/Superscript24.png", UriKind.Relative));

            Image imageFontSubscript = new Image();
            imageFontSubscript.Source = new BitmapImage(new Uri("Images/Subscript24.png", UriKind.Relative));

            Image imageNumberedList = new Image();
            imageNumberedList.Source = new BitmapImage(new Uri("Images/NumberedList24.png", UriKind.Relative));

            Image imageBulletedList = new Image();
            imageBulletedList.Source = new BitmapImage(new Uri("Images/BulletedList24.png", UriKind.Relative));

            Image imageFontSizeUp = new Image();
            imageFontSizeUp.Source = new BitmapImage(new Uri("Images/FontSizeUp24.png", UriKind.Relative));

            Image imageFontSizeDown = new Image();
            imageFontSizeDown.Source = new BitmapImage(new Uri("Images/FontSizeDown24.png", UriKind.Relative));

            Image imageAlignLeft = new Image();
            imageAlignLeft.Source = new BitmapImage(new Uri("Images/AlignLeft24.png", UriKind.Relative));

            Image imageAlignCenter = new Image();
            imageAlignCenter.Source = new BitmapImage(new Uri("Images/AlignCenter24.png", UriKind.Relative));

            Image imageAlignRight = new Image();
            imageAlignRight.Source = new BitmapImage(new Uri("Images/AlignRight24.png", UriKind.Relative));

            Image imageAlignFull = new Image();
            imageAlignFull.Source = new BitmapImage(new Uri("Images/AlignFull24.png", UriKind.Relative));

            Image imageUndo = new Image();
            imageUndo.Source = new BitmapImage(new Uri("Images/Undo24.png", UriKind.Relative));

            Image imageRedo = new Image();
            imageRedo.Source = new BitmapImage(new Uri("Images/Redo24.png", UriKind.Relative));

            Image imageCut = new Image();
            imageCut.Source = new BitmapImage(new Uri("Images/Cut24.png", UriKind.Relative));

            Image imageCopy = new Image();
            imageCopy.Source = new BitmapImage(new Uri("Images/Copy24.png", UriKind.Relative));

            Image imagePaste = new Image();
            imagePaste.Source = new BitmapImage(new Uri("Images/Insert24.png", UriKind.Relative));

            Image imageColor = new Image();
            imageColor.Source = new BitmapImage(new Uri("Images/FontColor24.png", UriKind.Relative));

            Button buttonOpenDoc = new Button();
            buttonOpenDoc.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            buttonOpenDoc.ToolTip = "Открыть документ (Ctrl + O)";
            buttonOpenDoc.Content = imageOpenFile;

            Button buttonSaveFile = new Button();
            buttonSaveFile.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            buttonSaveFile.ToolTip = "Сохранить текс как... (Ctrl + S)";
            buttonSaveFile.Content = imageSaveFile;

            Separator separator1 = new Separator();

            Button buttonUndo = new Button();
            buttonUndo.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            buttonUndo.ToolTip = "Отменить (Ctrl + Z)";
            buttonUndo.Command = ApplicationCommands.Undo;
            buttonUndo.Content = imageUndo;

            Button buttonRedo = new Button();
            buttonRedo.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            buttonRedo.ToolTip = "Вернуть (Ctrl + Y)";
            buttonRedo.Command = ApplicationCommands.Redo;
            buttonRedo.Content = imageRedo;

            Separator separator2 = new Separator();

            Button buttonCut = new Button();
            buttonCut.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            buttonCut.ToolTip = "Вырезать (Ctrl + X)";
            buttonCut.Command = ApplicationCommands.Cut;
            buttonCut.Content = imageCut;

            Button buttonCopy = new Button();
            buttonCopy.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            buttonCopy.ToolTip = "Копировать (Ctrl + C)";
            buttonCopy.Command = ApplicationCommands.Copy;
            buttonCopy.Content = imageCopy;

            Button buttonPaste = new Button();
            buttonPaste.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            buttonPaste.ToolTip = "Вставить (Ctrl + V)";
            buttonPaste.Content = imagePaste;

            Separator separator3 = new Separator();

            LoadSystemFonts();
         
            ComboBox comboBoxFont = new ComboBox();
                              
            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(VirtualizingStackPanel));
            factory.SetValue(VirtualizingStackPanel.OrientationProperty, Orientation.Vertical);

            Binding binding = new Binding();
            binding.Source = SystemFonts;
           
            comboBoxFont.Height = 25;
            comboBoxFont.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            comboBoxFont.ToolTip = "Шрифт (Ctrl + F)";
            comboBoxFont.IsEditable = true;
            comboBoxFont.IsReadOnly = false;
            comboBoxFont.Width = 140;
            comboBoxFont.IsTextSearchEnabled = true;
            comboBoxFont.SetBinding(ComboBox.ItemsSourceProperty, binding);
            comboBoxFont.ItemsPanel = new ItemsPanelTemplate(factory);
                     
            Binding binding2 = new Binding();
            binding2.Path = new PropertyPath("Source");
          
            FrameworkElementFactory textBlock = new FrameworkElementFactory(typeof(TextBlock));
            textBlock.SetBinding(TextBlock.TextProperty, binding2);
            textBlock.SetBinding(TextBlock.FontFamilyProperty, binding2);

            FrameworkElementFactory stackPanel = new FrameworkElementFactory(typeof(StackPanel));
            stackPanel.AppendChild(textBlock);

            DataTemplate dataTemplate = new DataTemplate();
            dataTemplate.VisualTree = stackPanel;

            comboBoxFont.ItemTemplate = dataTemplate;

            ComboBox comboBoxFontSize = new ComboBox();
            comboBoxFontSize.IsTextSearchEnabled = false;
            comboBoxFontSize.Height = 25;
            comboBoxFontSize.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            comboBoxFontSize.ToolTip = "Размер шрифта (Ctrl + P)";
            comboBoxFontSize.IsEditable = true;
            comboBoxFontSize.IsReadOnly = false;
            comboBoxFontSize.Width = 60;

            StackPanel panelFont = new StackPanel();
            panelFont.Orientation = Orientation.Horizontal;
            panelFont.Children.Add(comboBoxFont);
            panelFont.Children.Add(comboBoxFontSize);
          
            Button fontSizeUp = new Button();
            fontSizeUp.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            fontSizeUp.ToolTip = "Увеличить размер шрифта (Ctrl + })";
            fontSizeUp.Content = imageFontSizeUp;

            Button fontSizeDown = new Button();
            fontSizeDown.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            fontSizeDown.ToolTip = "Уменьшить размер шрифта (Ctrl + {)";
            fontSizeDown.Content = imageFontSizeDown;

            ToggleButton fontBold = new ToggleButton();
            fontBold.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            fontBold.ToolTip = "Полужирный шрифт (Ctrl + B)";
            fontBold.Content = imageFontBold;

            ToggleButton fontItalic = new ToggleButton();
            fontItalic.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            fontItalic.ToolTip = "Курсив (Ctrl + I)";
            fontItalic.Content = imageFontItalic;

            ToggleButton fontUnderline = new ToggleButton();
            fontUnderline.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            fontUnderline.ToolTip = "Подчеркнутый шрифт (Ctrl + U)";
            fontUnderline.Content = imageFontUnderline;

            ToggleButton fontStrikethrough = new ToggleButton();
            fontStrikethrough.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            fontStrikethrough.ToolTip = "Зачеркнутый шрифт";          
            fontStrikethrough.Content = imageStrikethrough;

            ToggleButton fontOverline = new ToggleButton();
            fontOverline.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            fontOverline.ToolTip = "Надчеркнутый шрифт";
            fontOverline.Content = imageOverline;

            ToggleButton fontSuperscript = new ToggleButton();
            fontSuperscript.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            fontSuperscript.ToolTip = "Подстрочный знак (Ctrl + +)";
            fontSuperscript.Content = imageFontSuperscript;

            ToggleButton fontSubscript = new ToggleButton();
            fontSubscript.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            fontSubscript.ToolTip = "Надстрочный знак (Ctrl + Shift + +)";
            fontSubscript.Content = imageFontSubscript;
          
            Separator separator4 = new Separator();

            ToggleButton numberedList = new ToggleButton();
            numberedList.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            numberedList.Command = EditingCommands.ToggleNumbering;
            numberedList.Content = imageNumberedList;

            ToggleButton bulletedList = new ToggleButton();
            bulletedList.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            bulletedList.Command = EditingCommands.ToggleBullets;
            bulletedList.Content = imageBulletedList;

            Separator separator5 = new Separator();

            ToggleButton alignLeft = new ToggleButton();
            alignLeft.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            alignLeft.ToolTip = "Выровнять текст по левому краю (Ctrl + L)";
            alignLeft.Content = imageAlignLeft;

            ToggleButton alignCenter = new ToggleButton();
            alignCenter.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            alignCenter.ToolTip = "Выровнять по центру (Ctrl + E)";
            alignCenter.Content = imageAlignCenter;

            ToggleButton alignRight = new ToggleButton();
            alignRight.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            alignRight.ToolTip = "Выровнять текст по правому краю (Ctrl + R)";
            alignRight.Content = imageAlignRight;

            ToggleButton alignFull = new ToggleButton();
            alignFull.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            alignFull.ToolTip = "Выровнять по ширине (Ctrl + J)";
            alignFull.Content = imageAlignFull;

            Rectangle rectangleColor = new Rectangle();
            rectangleColor.Width = 24;
            rectangleColor.Height = 5;

            Xceed.Wpf.Toolkit.DropDownButton fontColorConteiner = new Xceed.Wpf.Toolkit.DropDownButton();

            Border fontColor = new Border();
            fontColor.BorderThickness = new Thickness(0);
            fontColor.Style = (Style)Application.Current.FindResource("ControlOnToolBar");
            fontColor.ToolTip = "Цвет шрифта (Ctrl + J)";
            fontColor.Child = imageColor;

            StackPanel panelColor = new StackPanel();
            panelColor.Children.Add(fontColor);
            panelColor.Children.Add(rectangleColor);

            fontColorConteiner.Content = panelColor;
            fontColorConteiner.DropDownContent = new ColorBrushPickerTextControl(fontColorConteiner, richTextBox, rectangleColor);
                      
            toolBar.Items.Add(buttonOpenDoc);
            toolBar.Items.Add(buttonSaveFile);
            toolBar.Items.Add(separator1);
            toolBar.Items.Add(buttonUndo);
            toolBar.Items.Add(buttonRedo);
            toolBar.Items.Add(separator2);
            toolBar.Items.Add(buttonCut);
            toolBar.Items.Add(buttonCopy);
            toolBar.Items.Add(buttonPaste);
            toolBar.Items.Add(separator3);
            toolBar.Items.Add(panelFont);
            toolBar.Items.Add(fontSizeUp);
            toolBar.Items.Add(fontSizeDown);
            toolBar.Items.Add(fontBold);
            toolBar.Items.Add(fontItalic);
            toolBar.Items.Add(fontUnderline);
            toolBar.Items.Add(fontStrikethrough);
            toolBar.Items.Add(fontOverline);
            toolBar.Items.Add(fontSuperscript);
            toolBar.Items.Add(fontSubscript);            
            toolBar.Items.Add(separator4);
            toolBar.Items.Add(numberedList);
            toolBar.Items.Add(bulletedList);
            toolBar.Items.Add(separator5);
            toolBar.Items.Add(alignLeft);
            toolBar.Items.Add(alignCenter);
            toolBar.Items.Add(alignRight);
            toolBar.Items.Add(alignFull);
            toolBar.Items.Add(fontColorConteiner);

            richTextBox.AlignCenter = alignCenter;
            richTextBox.AlignFull = alignFull;
            richTextBox.AlignLeft = alignLeft;
            richTextBox.AlignRight = alignRight;
            richTextBox.BulletedList = bulletedList;
            richTextBox.ButtonSaveFile = buttonSaveFile;
            richTextBox.ButtonOpenFile = buttonOpenDoc;
            richTextBox.ButtonPaste = buttonPaste;
            richTextBox.ComboBoxFont = comboBoxFont;
            richTextBox.ComboBoxFontSize = comboBoxFontSize;
            richTextBox.FontBold = fontBold;
            richTextBox.FontItalic = fontItalic;
            richTextBox.FontSizeDown = fontSizeDown;
            richTextBox.FontSizeUp = fontSizeUp;
            richTextBox.FontStrikethrough = fontStrikethrough;
            richTextBox.FontSubscript = fontSubscript;
            richTextBox.FontSuperscript = fontSuperscript;
            richTextBox.FontUnderline = fontUnderline;
            richTextBox.NumberedList = numberedList;
            richTextBox.FontOverline = fontOverline;
            richTextBox.RectangleColor = rectangleColor;

            richTextBox.Document = flowDocument;
            richTextBox.AcceptsTab = true;

            Grid.SetRow(richTextBox, 1);
            Grid.SetRow(toolBar, 0);
            row0.Height = new GridLength(30, GridUnitType.Auto);
            row1.Height = new GridLength(600, GridUnitType.Pixel);
            grid.RowDefinitions.Add(row0);
            grid.RowDefinitions.Add(row1);
            grid.Children.Add(toolBar);
            grid.Children.Add(richTextBox);

            this.Content = grid;
        }
    }
}
