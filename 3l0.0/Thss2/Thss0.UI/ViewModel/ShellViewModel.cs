using Caliburn.Micro;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Input;
using Thss0.BLL.DTOs;
using Thss0.BLL.Services;
using Thss0.UI.Infrastructure;
using Thss0.UI.Models;

namespace Thss0.UI.ViewModel
{
    internal class ShellViewModel : PropertyChangedBase, IShell
    {
        private const ushort COMMANDS_QTY = 3;
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
        public UserManager<IdentityUser> UserManager{ get; set; }
        public SignInManager<IdentityUser> SignInManager { get; set; }
        public IServiceProvider ServiceProvider { get; set; }
        public ILogger<UserManager<IdentityUser>> LoggerUm { get; set; }
        public ILogger<SignInManager<IdentityUser>> LoggerSm { get; set; }
        public AuthenticationOptions AuthOptions{ get; set; }
        public ShellViewModel()
        {
            CommandsArr = new RelayCommand[COMMANDS_QTY];
            //_clntSrvce = new ClientsService();
            //_prfsnlsSrvce = new ProfessionalsService();
            //_prcdrsSrvce = new ProceduresService();
            /*_entty = new ObservableCollection<EntityDTO>(_clntSrvce.GetAll());
            _slctdEntty = new ClientDTO();

            _enttyVw = (CollectionView)CollectionViewSource.GetDefaultView(_entty);
            CommandsArr[0] = new RelayCommand(actn =>
            {
                _entty = new ObservableCollection<EntityDTO>(_clntSrvce.GetAll());
                _enttyVw = (CollectionView)CollectionViewSource.GetDefaultView(_entty);
            }
            , prdcte => _entty.Count > 0);
            CommandsArr[1] = new RelayCommand(actn =>
            {
                _entty = new ObservableCollection<EntityDTO>(_prfsnlsSrvce.GetAll());
                _enttyVw = (CollectionView)CollectionViewSource.GetDefaultView(_entty);
            }
            , prdcte => _entty.Count > 0);
            CommandsArr[2] = new RelayCommand(actn =>
            {
                _entty = new ObservableCollection<EntityDTO>(_prcdrsSrvce.GetAll().Result);
                _enttyVw = (CollectionView)CollectionViewSource.GetDefaultView(_entty);
            }
            , prdcte => _entty.Count > 0);
            string testValue = "test";*/
            //_acntSrvce.SignIn(new UserDTO { Name = testValue, Password = testValue, PhoneNumber = testValue, Email = testValue });
            //_acntSrvce.SignUp(new UserDTO { Name = testValue + "Test", Password = "TestTest*" + testValue, PhoneNumber = "0123456789", Email = testValue + "test@gmail.com" });
        }
        public void Create()
        {
            _acntSrvce = new AccountService(
                new Thss0Context(new DbContextOptionsBuilder<Thss0Context>().UseSqlServer("Initial Catalog=Thss0Context; Integrated Security=True; Encrypt=False").Options)
                ,UserManager, ServiceProvider, LoggerUm, SignInManager, LoggerSm, AuthOptions);
            
        }
    }
}