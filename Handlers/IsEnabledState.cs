
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MBModManager.Handlers
{
    class IsEnabledState : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _enabled;
        
        public bool Enabled {
            get { return _enabled; }
            set { _enabled = value; OnPropertyChanged(); }
        }

        public void Set(bool newState) {
            Enabled = newState;
        }

        public IsEnabledState(bool c) {
            this.Enabled = c;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
