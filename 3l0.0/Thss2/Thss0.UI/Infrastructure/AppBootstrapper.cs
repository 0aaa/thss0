using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Windows;
using Thss0.UI.Models;
using Thss0.UI.ViewModel;

namespace Thss0.UI.Infrastructure
{
    internal class AppBootstrapper : BootstrapperBase
    {
        private SimpleContainer _cntnr;
        public AppBootstrapper()
        {
            _cntnr = new SimpleContainer();
            Initialize();
        }

        protected override void Configure()
        {
            _cntnr.Singleton<IWindowManager, WindowManager>();
            _cntnr.Singleton<IEventAggregator, EventAggregator>();
            _cntnr.PerRequest<IShell, ShellViewModel>();
        }
        protected override object GetInstance(Type srvce, string ky)
            => _cntnr.GetInstance(srvce, ky);
        protected override IEnumerable<object> GetAllInstances(Type srvce)
            => _cntnr.GetAllInstances(srvce);
        protected override void BuildUp(object instnce)
            => _cntnr.BuildUp(instnce);
        protected override void OnStartup(object sndr, StartupEventArgs evntArgs)
            => DisplayRootViewFor<IShell>();
    }
}