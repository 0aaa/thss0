using Microsoft.AspNetCore.Identity;
using Thss0.BLL.DTO;
using Thss0.BLL.Services.Interface;

namespace Thss0.BLL.Services
{
    public class AccountService : IService
    {
        private readonly UserManager<IdentityUser> _usrMngr;
        private readonly SignInManager<IdentityUser> _sgnInMngr;
        public AccountService(SignInManager<IdentityUser> sgnInMngr)
        {
            _usrMngr = sgnInMngr.UserManager;
            _sgnInMngr = sgnInMngr;
        }
        public async Task<IdentityResult> SignUp(UserDTO acntCrdntls)
        {
            var userToSignUp = ParseEntity(acntCrdntls);
            var userToAdd = new IdentityUser
            {
                UserName = userToSignUp.Name,
                PhoneNumber = userToSignUp.PhoneNumber,
                Email = userToSignUp.Email
            };
            var res = await _usrMngr.CreateAsync(userToAdd, userToSignUp.Password);
            await _usrMngr.AddToRoleAsync(userToAdd, acntCrdntls.Role);
            return res;
        }
        public async Task<SignInResult> SignIn(UserDTO acntCrdntls)
        {
            var userToSignIn = ParseEntity(acntCrdntls);
            return await _sgnInMngr.PasswordSignInAsync(userToSignIn.Name, userToSignIn.Password, false, false);
        }
        public async Task LogOut()
            => await _sgnInMngr.SignOutAsync();
        private UserDTO ParseEntity(UserDTO acntCrdntls)
        {
            switch (acntCrdntls.Role)
            {
                case "admin":
                    return (ProfessionalDTO)acntCrdntls;
                case "professional":
                    return (ProfessionalDTO)acntCrdntls;
                default:
                    return (ClientDTO)acntCrdntls;
            }
        }
    }
}