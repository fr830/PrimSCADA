using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;

namespace BrushEditor
{
    public class BrushEditorViewModel : ViewModelBase
    {
        private readonly ObservableCollection<GradientStopViewModel> _gradientStops;
        private BrushTypes _availableBrushTypes;
        private BrushTypes _brushType = BrushTypes.Linear;
        private Point _center = new Point(.5, .5);

        private ColorEditorViewModel _colorEditorViewModel;
        private GradientStopViewModel _selectedGradientStop;
        private Point _startPoint = new Point(1, 0);

        public BrushEditorViewModel()
        {
            _gradientStops = new ObservableCollection<GradientStopViewModel>();
            _gradientStops.CollectionChanged += _gradientStops_CollectionChanged;

            AvailableBrushTypes = BrushTypes.Solid | BrushTypes.Linear | BrushTypes.Radial; 
            Brush = new LinearGradientBrush(Colors.Red, Colors.Black, 45);          
        }

        public ObservableCollection<GradientStopViewModel> GradientStops
        {
            get { return _gradientStops; }
        }

        public Point Center
        {
            get { return _center; }
            set
            {
                _center = value;

                OnPropertyChanged(() => Center);
                OnPropertyChanged(() => Brush);
            }
        }

        public Point StartPoint
        {
            get { return _startPoint; }
            set
            {
                _startPoint = value;

                OnPropertyChanged(() => StartPoint);
                OnPropertyChanged(() => Brush);
            }
        }

        public ColorEditorViewModel ColorEditorViewModel
        {
            get { return _colorEditorViewModel; }
            set
            {
                if (_colorEditorViewModel != null) _colorEditorViewModel.PropertyChanged -= _colorEditorViewModel_PropertyChanged;

                _colorEditorViewModel = value;

                _colorEditorViewModel.PropertyChanged += _colorEditorViewModel_PropertyChanged;
            }
        }

        public BrushTypes AvailableBrushTypes
        {
            get { return _availableBrushTypes; }
            set
            {
                _availableBrushTypes = value;

                OnPropertyChanged(() => AvailableBrushTypes);
                OnPropertyChanged(() => AvailableBrushTypeValues);
            }
        }

        public IEnumerable<Enum> AvailableBrushTypeValues
        {
            get { return GetFlags(AvailableBrushTypes); }
        }

        public BrushTypes BrushType
        {
            get { return _brushType; }
            set
            {
                _brushType = value;

                OnPropertyChanged(() => BrushType);
                OnPropertyChanged(() => Brush);
            }
        }

        public GradientStopViewModel SelectedGradientStop
        {
            get { return _selectedGradientStop; }
            set
            {
                _selectedGradientStop = value;

                OnPropertyChanged(() => SelectedGradientStop);

                if (_selectedGradientStop != null) ColorEditorViewModel.Color = _selectedGradientStop.Color;
            }
        }

        public RelayCommand AddCommand
        {
            get { return new RelayCommand(param => AddGradientStop()); }
        }

        public RelayCommand RemoveCommand
        {
            get { return new RelayCommand(param => RemoveGradientStop(), param => SelectedGradientStop != null); }
        }

        public Brush Brush
        {
            get
            {
                if (BrushType == BrushTypes.Solid)
                {
                    if (GradientStops.Count == 0) return new SolidColorBrush(Colors.Black);

                    return GradientStops[0].Brush;
                }

                if (BrushType == BrushTypes.Linear)
                {
                    var brush = new LinearGradientBrush();
                    brush.StartPoint = StartPoint;

                    foreach (GradientStopViewModel g in GradientStops)
                    {
                        brush.GradientStops.Add(new GradientStop(g.Color, g.Offset));
                    }
                    return brush;
                }

                if (BrushType == BrushTypes.Radial)
                {
                    var brush = new RadialGradientBrush();
                    brush.Center = Center;
                    foreach (GradientStopViewModel g in GradientStops)
                    {
                        brush.GradientStops.Add(new GradientStop(g.Color, g.Offset));
                    }
                    return brush;
                }

                return null;
            }

            set
            {
                GradientStops.Clear();

                if (value is SolidColorBrush)
                {
                    BrushType = BrushTypes.Solid;
                    var brush = (SolidColorBrush) value;

                    GradientStops.Add(new GradientStopViewModel {Color = brush.Color});
                }

                if (value is LinearGradientBrush)
                {
                    BrushType = BrushTypes.Linear;
                    var brush = (LinearGradientBrush) value;
                    StartPoint = brush.StartPoint;

                    for (int n = 0; n < brush.GradientStops.Count; n++)
                    {
                        GradientStops.Add(new GradientStopViewModel(brush.GradientStops[n]));
                    }
                }

                if (value is RadialGradientBrush)
                {
                    BrushType = BrushTypes.Radial;
                    var brush = (RadialGradientBrush) value;
                    Center = brush.Center;

                    for (int n = 0; n < brush.GradientStops.Count; n++)
                    {
                        GradientStops.Add(new GradientStopViewModel(brush.GradientStops[n]));
                    }
                }
            }
        }

        public string SerializeBrushToXml()
        {
            var settings = new XmlWriterSettings();

            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            settings.ConformanceLevel = ConformanceLevel.Fragment;

            var sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb, settings);

            var manager = new XamlDesignerSerializationManager(writer);
            manager.XamlWriterMode = XamlWriterMode.Expression;

            XamlWriter.Save(Brush, manager);

            return sb.ToString();
        }

        public void DeserializeBrushFromXml(string xamlText)
        {
            var doc = new XmlDocument();
            // may throw XmlException
            doc.LoadXml(xamlText);
            // may throw XamlParseException
            Brush = (Brush) XamlReader.Load(new XmlNodeReader(doc));
        }

        private static IEnumerable<Enum> GetFlags(Enum input)
        {
            foreach (Enum value in Enum.GetValues(input.GetType()))
                if (input.HasFlag(value))
                    yield return value;
        }

        /// <summary>
        /// Need to hook the PropertyChanged event of each GradientStop so we can tell if the color or offset has changed and update the brush
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _gradientStops_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (GradientStopViewModel viewModel in e.NewItems)
                {
                    viewModel.PropertyChanged += viewModel_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (GradientStopViewModel viewModel in e.OldItems)
                {
                    viewModel.PropertyChanged -= viewModel_PropertyChanged;
                }
            }

            OnPropertyChanged(() => Brush);
        }

        private void viewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(() => Brush);
        }

        private void AddGradientStop()
        {
            GradientStops.Add(new GradientStopViewModel());
            SelectedGradientStop = GradientStops.Last();
        }

        private void RemoveGradientStop()
        {
            GradientStops.Remove(SelectedGradientStop);
        }

        private void _colorEditorViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (SelectedGradientStop == null) return;

            if (e.PropertyName == "Color")
            {
                SelectedGradientStop.Color = ColorEditorViewModel.Color;
            }
        }
    }
}