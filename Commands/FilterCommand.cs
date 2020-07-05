using InternTest.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InternTest.Commands
{
    class FilterCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter)
        {
            var viewModel = (ViewModel)parameter;
            return !viewModel.Grouped;
        }

        public void Execute(object parameter)
        {
            var viewModel = (ViewModel)parameter;
            viewModel.Filter();
        }
    }
}
