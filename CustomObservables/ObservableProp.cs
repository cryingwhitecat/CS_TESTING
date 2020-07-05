using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace InternTest.CustomObservables
{
    public class ObservableProp<T> : INotifyPropertyChanged
    {
        private T value;
        public T Value
        {
            get { return value; }
            set
            {
                {
                    this.value = value;
                    OnPropertyChanged();
                }
            }
        }

        public static implicit operator ObservableProp<T>(T value)
        {
            return new ObservableProp<T> { value = value };
        }
        public static implicit operator T(ObservableProp<T> wrapper)
        {
            return wrapper.value;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
