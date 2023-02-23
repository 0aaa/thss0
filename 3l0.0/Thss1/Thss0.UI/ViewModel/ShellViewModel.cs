using Caliburn.Micro;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Input;
using Thss0.BLL.DTO;
using Thss0.BLL.Services;
using Thss0.BLL.Services.Interface;
using Thss0.DAL.Context;
using Thss0.UI.Infrastructure;
using Thss0.UI.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Thss0.UI.ViewModel
{
    internal class ShellViewModel : PropertyChangedBase, IShell
    {
        private const ushort COMMANDS_QTY = 3;
        private SignInManager<IdentityUser> _sgnInMngr;
        private IService _enttySrvce;
        private AccountService _acntSrvce;
        private ClientsService _clntSrvce;
        private ProfessionalsService _prfsnlsSrvce;
        private ProceduresService _prcdrsSrvce;
        private ObservableCollection<EntityDTO> _entty;
        private CollectionView _enttyVw;
        private EntityDTO _slctdEntty;
        public CollectionView EntityView
        {
            get => _enttyVw;
            private set
            {
                _enttyVw = value;
                NotifyOfPropertyChange("EntityView");
            }
        }
        public EntityDTO SelectedEntity
        {
            get => _slctdEntty;
            set
            {
                _slctdEntty = value;
                NotifyOfPropertyChange("SelectedEntity");
            }
        }

        public ICommand[] CommandsArr { get; private set; }
        public ShellViewModel()
        {
            var someOptions = Options.Create(new IdentityOptions());
            var usrMngr = new UserManager<IdentityUser>(new UserStore<IdentityUser>(new Thss0ContextDTO(new DbContextOptions<Thss0Context>())), someOptions, new PasswordHasher<IdentityUser>()
                                , new List<UserValidator<IdentityUser>> { new UserValidator<IdentityUser>() }, new List<PasswordValidator<IdentityUser>> { new PasswordValidator<IdentityUser>() }, new UpperInvariantLookupNormalizer()
                                , new IdentityErrorDescriber(), new ServiceCollection().BuildServiceProvider()
                                , new ServiceCollection().BuildServiceProvider().GetRequiredService<ILogger<UserManager<IdentityUser>>>());
            _sgnInMngr = null;
            CommandsArr = new RelayCommand[COMMANDS_QTY];
            //_acntSrvce = new AccountService(_sgnInMngr);
            _clntSrvce = new ClientsService(usrMngr);
            _prfsnlsSrvce = new ProfessionalsService(usrMngr);
            _prcdrsSrvce = new ProceduresService(usrMngr, new DbContextOptions<Thss0Context>());
            _entty = new ObservableCollection<EntityDTO>(_clntSrvce.GetAll());

            _enttyVw = (CollectionView)CollectionViewSource.GetDefaultView(_entty);
            CommandsArr[0] = new RelayCommand(actn => { _entty = new ObservableCollection<EntityDTO>(_clntSrvce.GetAll()); _enttyVw = (CollectionView)CollectionViewSource.GetDefaultView(_entty); }
                                , prdcte => _entty.Count > 0);
            CommandsArr[1] = new RelayCommand(actn => { _entty = new ObservableCollection<EntityDTO>(_prfsnlsSrvce.GetAll()); _enttyVw = (CollectionView)CollectionViewSource.GetDefaultView(_entty); }
                                , prdcte => _entty.Count > 0);
            CommandsArr[2] = new RelayCommand(actn => { _entty = new ObservableCollection<EntityDTO>(_prcdrsSrvce.GetAll().Result); _enttyVw = (CollectionView)CollectionViewSource.GetDefaultView(_entty); }
                                , prdcte => _entty.Count > 0);
        }
    }
}