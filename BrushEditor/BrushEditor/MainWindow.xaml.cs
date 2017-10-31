using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using Lovatts.ColorEditor;

namespace BrushEditor
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ColorEditor colorEditor = new ColorEditor();
            colorEditor.ColorEditorViewModel.Color = Colors.Blue;
            colorEditor.ColorEditorViewModel.RGB.A = 67;

            Lovatts.ColorEditor.BrushEditor brushEditor = new Lovatts.ColorEditor.BrushEditor();
            brushEditor.BrushEditorViewModel.BrushType = BrushTypes.Radial;
            brushEditor.BrushEditorViewModel.Center = new Point(1, 0);
            brushEditor.BrushEditorViewModel.GradientStops.Add(new GradientStopViewModel(Colors.Red, 0));
            brushEditor.BrushEditorViewModel.GradientStops.Add(new GradientStopViewModel(Colors.Blue, 1));

            string xml = brushEditor.BrushEditorViewModel.SerializeBrushToXml();

            brushEditor.BrushEditorViewModel.DeserializeBrushFromXml(xml);

            Debug.WriteLine(xml);

        }
    }
}