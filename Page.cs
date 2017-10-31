// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace SCADA
{
    [Serializable]
    public class Page
    {
        public CollectionsText CollectionText { get; set; }

        public CollectionsPipe CollectionPipe { get; set; }

        public CollectionsPipe90 CollectionPipe90 { get; set; }

        public CollectionsDisplay CollectionDisplay { get; set; }

        public CollectionsImage CollectionImage { get; set; }
      
        public Page()
        {
            CollectionPipe90 = new CollectionsPipe90();
            CollectionPipe = new CollectionsPipe();
            CollectionText = new CollectionsText();
            CollectionDisplay = new CollectionsDisplay();
            CollectionImage = new CollectionsImage();
        }
    }

    public class CollectionsPipe : List<PipeSer>
    { }

    public class CollectionsPipe90 : List<Pipe90Ser>
    { }

    public class CollectionsText : List<TextSer>
    { }

    public class CollectionsDisplay : List<DisplaySer>
    { }

    public class CollectionsImage : List<ImageSer>
    { }

    [Serializable]
    public class TextSer : ControlOnCanvasSer
    {
        public string ColorBackGround { get; set; }
        public string ColorBorder { get; set; }
        public double BorderThickness { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public FlowDocument TextDocument { get; set; }

        public TwoPointSegment RightSize { get; set; }
        public TwoPointSegment LeftSize { get; set; }
        public TwoPointSegment TopSize { get; set; }
        public TwoPointSegment DownSize { get; set; }
        public FivePointSegment Border { get; set; }

        public TextSer(int ZIndex, int transform, Point point)
            : base(ZIndex, transform)
        {
            RightSize = new TwoPointSegment();
            LeftSize = new TwoPointSegment();
            TopSize = new TwoPointSegment();
            DownSize = new TwoPointSegment();
            Border = new FivePointSegment();

            Width = 40;
            Height = 22;
            Сoordinates = point;
            BorderThickness = 2;
            ColorBorder = new ColorConverter().ConvertToString(Colors.Black);
            ColorBackGround = new ColorConverter().ConvertToString(Colors.Transparent);

            RightSize.point = new Point[2] { new Point(45, 3), new Point(45, 24) };
            LeftSize.point = new Point[2] { new Point(0, 3), new Point(0, 24) };
            TopSize.point = new Point[2] { new Point(3, 0), new Point(42, 0) };
            DownSize.point = new Point[2] { new Point(3, 27), new Point(42, 27) };
            Border.point = new Point[5] { new Point(0, 0), new Point(45, 0), new Point(45, 27), new Point(0, 27), new Point(0, 0) };
        }

        public TextSer()
        { }
    }

    [Serializable]
    public class PipeSer : ControlOnCanvasSer
    {
        public PipeSer(int ZIndex, int transform , Point point)
            : base(ZIndex, transform)
        {
            RightSize = new TwoPointSegment();
            LeftSize = new TwoPointSegment();
            TopSize = new TwoPointSegment();
            DownSize = new TwoPointSegment();
            LeftFlange = new FivePointSegment();
            RightFlange = new FivePointSegment();
            Pipe = new FivePointSegment();
            BorderPipe = new FivePointSegment();

            Сoordinates = point;
            RightSize.point = new Point[2] {new Point(145.5, 28), new Point(145.5, 7)};
            LeftSize.point = new Point[2] {new Point(2.5, 7), new Point(2.5, 28)};
            TopSize.point = new Point[2] {new Point(5, 3), new Point(142, 3)};
            DownSize.point = new Point[2] {new Point(5, 31), new Point(142, 31)};
            RightFlange.point = new Point[5] {new Point(142, 0), new Point(147, 0), new Point(147, 34), new Point(142, 34), new Point(142, 0)};
            LeftFlange.point = new Point[5] {new Point(0, 0), new Point(5, 0), new Point(5, 34), new Point(0, 34), new Point(0, 0)};
            Pipe.point = new Point[5] {new Point(143, 31), new Point(143, 3), new Point(4, 3), new Point(4, 31), new Point(143, 31)};
            BorderPipe.point = new Point[5] {new Point(0, 0), new Point(147, 0), new Point(147, 34), new Point(0, 34), new Point(0, 0)};
            Environment = 4;
        }

        public PipeSer()
        { }

        public TwoPointSegment RightSize { get; set; }
        public TwoPointSegment LeftSize { get; set; }
        public TwoPointSegment TopSize { get; set; }
        public TwoPointSegment DownSize { get; set; }
        public FivePointSegment LeftFlange { get; set; }
        public FivePointSegment RightFlange { get; set; }
        public FivePointSegment Pipe { get; set; }
        public FivePointSegment BorderPipe { get; set; }

        private int environment = 4;
        public int Environment
        {
            get { return environment; }
            set { environment = value; }
        }
    }

    [Serializable]
    public class ImageSer : ControlOnCanvasSer
    {
        public string ColorBackGround { get; set; }
        public Brush ColorBorder { get; set; }
        public double BorderThickness { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string PathImage { get; set; }
        public string StretchImage { get; set; }
        public string LibraryImage { get; set; }
        public bool IsPathImage { get; set; }

        public TwoPointSegment RightSize { get; set; }
        public TwoPointSegment LeftSize { get; set; }
        public TwoPointSegment TopSize { get; set; }
        public TwoPointSegment DownSize { get; set; }
        public FivePointSegment Border { get; set; }

        public ImageSer(int ZIndex, int transform, Point point)
            : base(ZIndex, transform)
        {
            RightSize = new TwoPointSegment();
            LeftSize = new TwoPointSegment();
            TopSize = new TwoPointSegment();
            DownSize = new TwoPointSegment();
            Border = new FivePointSegment();

            IsPathImage = true;

            StretchImage = "Uniform";

            Width = 40;
            Height = 22;
            Сoordinates = point;
            BorderThickness = 2;
            ColorBorder = new SolidColorBrush(Colors.Black);
            ColorBackGround = new ColorConverter().ConvertToString(Colors.Transparent);

            RightSize.point = new Point[2] { new Point(45, 3), new Point(45, 24) };
            LeftSize.point = new Point[2] { new Point(0, 3), new Point(0, 24) };
            TopSize.point = new Point[2] { new Point(3, 0), new Point(42, 0) };
            DownSize.point = new Point[2] { new Point(3, 27), new Point(42, 27) };
            Border.point = new Point[5] { new Point(0, 0), new Point(45, 0), new Point(45, 27), new Point(0, 27), new Point(0, 0) };
        }

        public ImageSer()
        { }
    }

    [Serializable]
    public class DisplaySer : ControlOnCanvasSer
    {
        public bool IsCollSend { get; set; }
        public bool IsCollRec { get; set; }
        public string EthernetSearch { get; set; }
        public string ModbusSearch { get; set; }
        public EthernetOperationalSearch EthernetOperationalSearch { get; set; }
        public bool IsEthernet { get; set; }
        public bool IsModbus { get; set; }
        public ItemNet ItemNetSearch { get; set; }
        public ItemModbus ItemModbusSearch { get; set; }
        public string ColorBackGround { get; set; }
        public string ColorBorder { get; set; }
        public double BorderThickness { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public int PeriodTime { get; set; }
        
        public TwoPointSegment RightSize { get; set; }
        public TwoPointSegment LeftSize { get; set; }
        public TwoPointSegment TopSize { get; set; }
        public TwoPointSegment DownSize { get; set; }
        public FivePointSegment Border { get; set; }

        public DisplaySer(int ZIndex, int transform, Point point)
            : base(ZIndex, transform)
        {
            PeriodTime = 1000;

            RightSize = new TwoPointSegment();
            LeftSize = new TwoPointSegment();
            TopSize = new TwoPointSegment();
            DownSize = new TwoPointSegment();
            Border = new FivePointSegment();

            Width = 40;
            Height = 22;
            Сoordinates = point;
            BorderThickness = 2;
            ColorBorder = new ColorConverter().ConvertToString(Colors.Black);
            ColorBackGround = new ColorConverter().ConvertToString(Colors.Transparent);

            RightSize.point = new Point[2] { new Point(45, 3), new Point(45, 24) };
            LeftSize.point = new Point[2] { new Point(0, 3), new Point(0, 24) };
            TopSize.point = new Point[2] { new Point(3, 0), new Point(42, 0) };
            DownSize.point = new Point[2] { new Point(3, 27), new Point(42, 27) };
            Border.point = new Point[5] { new Point(0, 0), new Point(45, 0), new Point(45, 27), new Point(0, 27), new Point(0, 0) };
        }

        public DisplaySer()
        { }
    }

    [Serializable]
    public class ControlOnCanvasSer : IComparable
    {
        public Point Сoordinates { get; set; }
        public int Transform { get; set; }
        public int ZIndex {get; set;}

        [NonSerialized()]
        public ControlOnCanvas ControlItem;

        public ControlOnCanvasSer(int zIndex, int transform)
        {
            Transform = transform;
            ZIndex = zIndex;
        } 

        public ControlOnCanvasSer()
        { }

        public int CompareTo(object obj)
        {
            ControlOnCanvasSer otherControlOnCanvasSer = obj as ControlOnCanvasSer;

            if (otherControlOnCanvasSer != null) return this.ZIndex.CompareTo(otherControlOnCanvasSer.ZIndex);
            else throw new ArgumentException("Пустой объект для сортировки");
        }
    }

    [Serializable]
    public class Pipe90Ser: ControlOnCanvasSer
    {
        public Pipe90Ser(int ZIndex, int transform, Point point)
            : base(ZIndex, transform)
        {
            RightDownSize = new TwoPointSegment();
            LeftDownSize = new TwoPointSegment();
            TopSize = new TwoPointSegment();
            DownSize = new TwoPointSegment();
            LeftFlange = new FivePointSegment();
            RightFlange = new FivePointSegment();
            Pipe90 = new SevenPointSegment();
            BorderPipe90 = new FivePointSegment();
            TopImage = new FivePointSegment();
            DownImage = new FivePointSegment();
            DownLenghtSize = new TwoPointSegment();
            TopLenghtSize = new TwoPointSegment();

            Сoordinates = point;
            RightDownSize.point = new Point[2] { new Point(31, 69), new Point(31, 31) };
            LeftDownSize.point = new Point[2] { new Point(3, 69), new Point(3, 31) };
            TopSize.point = new Point[2] { new Point(69, 3), new Point(31, 3) };
            DownSize.point = new Point[2] { new Point(69, 31), new Point(31, 31) };
            DownLenghtSize.point = new Point[2] { new Point(6.5, 72), new Point(27.5, 72) };
            TopLenghtSize.point = new Point[2] { new Point(72, 6.5), new Point(72, 27.5) };
            RightFlange.point = new Point[5] { new Point(74, 34), new Point(69, 34), new Point(69, 0), new Point(74, 0), new Point(74, 34) };
            LeftFlange.point = new Point[5] { new Point(0, 74), new Point(0, 69), new Point(34, 69), new Point(34, 74), new Point(0, 74) };
            Pipe90.point = new Point[7] { new Point(69, 3), new Point(3, 3), new Point(3, 31), new Point(3, 70), new Point(31, 70), new Point(31, 31), new Point(69, 31) };
            BorderPipe90.point = new Point[5] { new Point(0, 0), new Point(74, 0), new Point(74, 74), new Point(0, 74), new Point(0, 0) };
            TopImage.point = new Point[5] { new Point(31, 31), new Point(3, 3), new Point(70, 3), new Point(70, 31), new Point(31, 31) };
            DownImage.point = new Point[5] { new Point(4, 3), new Point(31, 30), new Point(31, 70), new Point(3, 70), new Point(3, 3) };
            Environment = 4;
        }

        public Pipe90Ser()
        { }

        public TwoPointSegment RightDownSize { get; set; }
        public TwoPointSegment LeftDownSize { get; set; }
        public TwoPointSegment TopSize { get; set; }
        public TwoPointSegment DownSize { get; set; }
        public FivePointSegment LeftFlange { get; set; }
        public FivePointSegment RightFlange { get; set; }
        public SevenPointSegment Pipe90 { get; set; }
        public FivePointSegment BorderPipe90 { get; set; }
        public FivePointSegment TopImage { get; set; }
        public FivePointSegment DownImage { get; set; }
        public TwoPointSegment DownLenghtSize { get; set; }
        public TwoPointSegment TopLenghtSize { get; set; }
       
        private int environment = 4;
        public int Environment
        {
            get { return environment; }
            set { environment = value; }
        }
    }

    [Serializable]
    public class TwoPointSegment
    {
        public Point[] point { get; set; }

        public TwoPointSegment()
        {
            point = new Point[2];
        }
    }

    [Serializable]
    public class FivePointSegment
    {
        public Point[] point { get; set; }

        public FivePointSegment()
        {
            point = new Point[5];
        }
    }

    [Serializable]
    public class SevenPointSegment
    {
        public Point[] point { get; set; }

        public SevenPointSegment()
        {
            point = new Point[7];
        }
    }

    [Serializable]
    public class EthernetOperationalSearch
    {
        public int BufferSizeRec { get; set; }
        public int BufferSizeSend { get; set; }
        public string Description { get; set; }
        public ushort Port { get; set; }

        public static bool operator ==(EthernetOperationalSearch op1, EthernetOperationalSearch op2)
        {           
            if ((object)op1 == null)
            {
                return (object)op2 == null;
            }

            return op1.Equals(op2);
        }

        public static bool operator !=(EthernetOperationalSearch op1, EthernetOperationalSearch op2)
        {
            if ((object)op1 != null)
            {
                if ((object)op2 == null)
                {
                    return true;
                }
                else
                {
                    return !op1.Equals(op2);
                }
            }
            else
            {
                return (object)op2 != null;                
            } 
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to ItemNet return false.
            EthernetOperationalSearch ethernetOperationalSearch = obj as EthernetOperationalSearch;
            if ((System.Object)ethernetOperationalSearch == null)
            {
                return false;
            }

            // Return true if the fields match:
            if (BufferSizeRec == ethernetOperationalSearch.BufferSizeRec && BufferSizeSend == ethernetOperationalSearch.BufferSizeSend && Description == ethernetOperationalSearch.Description && Port == ethernetOperationalSearch.Port)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Equals(EthernetOperationalSearch ethernetOperationalSearch)
        {
            // If parameter is null return false:
            if ((object)ethernetOperationalSearch == null)
            {
                return false;
            }

            // Return true if the fields match:
            if (BufferSizeRec == ethernetOperationalSearch.BufferSizeRec && BufferSizeSend == ethernetOperationalSearch.BufferSizeSend && Description == ethernetOperationalSearch.Description && Port == ethernetOperationalSearch.Port)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public EthernetOperationalSearch()
        {
        }
    }  
}
