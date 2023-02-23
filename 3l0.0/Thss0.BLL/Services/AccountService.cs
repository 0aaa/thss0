using Microsoft.AspNetCore.Identity;
using Thss0.BLL.DTOs;
using Thss0.BLL.Services.Parents;

namespace Thss0.BLL.Services
{
    public class AccountService : IdentityManager
    {
        public async Task<IdentityResult> SignUp(UserDTO acntCrdntls)
        {
            var userToSignUp = ParseEntity(acntCrdntls);
            var userToAdd = new IdentityUser
            {
                UserName = userToSignUp.Name,
                PhoneNumber = userToSignUp.PhoneNumber,
                Email = userToSignUp.Email
            };
            var res = await SignInManager.UserManager.CreateAsync(userToAdd, userToSignUp.Password);
            await SignInManager.UserManager.AddToRoleAsync(userToAdd, acntCrdntls.Role);
            return res;
        }
        public async Task<SignInResult> SignIn(UserDTO acntCrdntls)
        {
            var userToSignIn = ParseEntity(acntCrdntls);
            return await SignInManager.PasswordSignInAsync(userToSignIn.Name, userToSignIn.Password, false, false);
        }
        public async Task LogOut()
            => await SignInManager.SignOutAsync();
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