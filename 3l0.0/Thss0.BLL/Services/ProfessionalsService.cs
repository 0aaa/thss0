using Microsoft.AspNetCore.Identity;
using Thss0.BLL.DTOs;
using Thss0.BLL.Services.Parents;

namespace Thss0.BLL.Services
{
    public class ProfessionalsService : IdentityManager, IUsersService
    {
        private const string PROFESSIONAL_ROLE = "professional";
        private UserManager<IdentityUser> _usrMngr;
        public ProfessionalsService()
        {
            _usrMngr = SignInManager.UserManager;
        }
        public IEnumerable<UserDTO> GetAll()
            => _usrMngr.Users.Select(usr => new ProfessionalDTO
            {
                Id = usr.Id,
                Name = usr.UserName,
                PhoneNumber = usr.PhoneNumber,
                Email = usr.Email
            });
        public async Task<UserDTO> Get(string id)
        {
            var userToGet = await _usrMngr.FindByIdAsync(id);
            return new ProfessionalDTO()
            {
                Id = userToGet.Id,
                Name = userToGet.UserName,
                PhoneNumber = userToGet.PhoneNumber,
                Email = userToGet.Email
            };
        }
        public async Task<IdentityResult> Add(UserDTO usrCrdntls)
        {
            var prfsnlDTO = (ProfessionalDTO)usrCrdntls;
            var userToAdd = new IdentityUser
            {
                UserName = prfsnlDTO.Name,
                PhoneNumber = prfsnlDTO.PhoneNumber,
                Email = prfsnlDTO.Email
            };
            var res = await _usrMngr.CreateAsync(userToAdd);
            await _usrMngr.AddToRoleAsync(userToAdd, PROFESSIONAL_ROLE);
            return res;
        }
        public async Task<IdentityResult> Delete(string id)
            => await _usrMngr.DeleteAsync(await _usrMngr.FindByIdAsync(id));
    }
}