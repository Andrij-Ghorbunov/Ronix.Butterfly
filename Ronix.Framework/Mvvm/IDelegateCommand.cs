using System.Windows.Input;

namespace Ronix.Framework.Mvvm
{
    public interface IDelegateCommand : ICommand 
    {
        void RaiseCanExecuteChanged();
    }
}