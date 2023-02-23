using System;
using System.Windows.Input;

namespace Thss0.UI.Infrastructure
{
    internal class RelayCommand : ICommand
    {
        private readonly Action<object> _actn;
        private readonly Predicate<object> _prdcte;
        public RelayCommand(Action<object> actn, Predicate<object> prdcte)
        {
            _actn = actn;
            _prdcte = prdcte;
        }
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
        public bool CanExecute(object prmtr)
            => _prdcte == null || _prdcte(prmtr);
        public void Execute(object prmtr)
            => _actn(prmtr);
    }
}