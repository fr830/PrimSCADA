namespace BrushEditor
{
    public class HSLA : ViewModelBase
    {
        public bool LockHue;
        private double _a;
        private double _h;
        private double _l;
        private double _s;

        public double H
        {
            get { return _h; }
            set
            {
                if (LockHue) return;

                if (_h == value) return;

                _h = value;

                OnPropertyChanged(() => H);
            }
        }

        public double S
        {
            get { return _s; }
            set
            {
                if (_s == value) return;

                _s = value;

                OnPropertyChanged(() => S);
            }
        }

        public double L
        {
            get { return _l; }
            set
            {
                if (_l == value) return;

                _l = value;

                OnPropertyChanged(() => L);
            }
        }

        public double A
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
            OnPropertyChanged(() => H);
            OnPropertyChanged(() => S);
            OnPropertyChanged(() => L);
            OnPropertyChanged(() => A);
        }
    }
}