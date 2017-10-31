namespace BrushEditor
{
    public class RGBA : ViewModelBase
    {
        private byte _a = 255;
        private byte _b;
        private byte _g;
        private byte _r;

        public byte R
        {
            get { return _r; }
            set
            {
                if (_r == value) return;

                _r = value;

                OnPropertyChanged(() => R);
            }
        }

        public byte G
        {
            get { return _g; }
            set
            {
                if (_g == value) return;

                _g = value;

                OnPropertyChanged(() => G);
            }
        }

        public byte B
        {
            get { return _b; }
            set
            {
                if (_b == value) return;

                _b = value;

                OnPropertyChanged(() => B);
            }
        }

        public byte A
        {
            get { return _a; }
            set
            {
                if (_a == value) return;

                _a = value;

                OnPropertyChanged(() => A);
            }
        }

        public void FirePropertyChangedForAll()
        {
            OnPropertyChanged(() => R);
            OnPropertyChanged(() => G);
            OnPropertyChanged(() => B);
            OnPropertyChanged(() => A);
        }
    }
}