using Microsoft.AspNetCore.Identity;
using Thss0.BLL.DTO;
using Thss0.BLL.Services.Interface;

namespace Thss0.BLL.Services
{
    public class ClientsService : IUsersService, IService
    {
        private const string CLIENT_ROLE = "client";
        private UserManager<IdentityUser> _usrMngr;
        public ClientsService(UserManager<IdentityUser> usrMngr)
        {
            _usrMngr = usrMngr;
        }
        public IEnumerable<UserDTO> GetAll()
            => _usrMngr.Users.Select(usr => new ClientDTO
            {
                Id = usr.Id,
                Name = usr.UserName,
                PhoneNumber = usr.PhoneNumber,
                Email = usr.Email
            });
        public async Task<UserDTO> Get(string id)
        {
            var userToGet = await _usrMngr.FindByIdAsync(id);
            return new ClientDTO()
            {
                Id = userToGet.Id,
                Name = userToGet.UserName,
                PhoneNumber = userToGet.PhoneNumber,
                Email = userToGet.Email
            };
        }
        public async Task<IdentityResult> Add(UserDTO usrCrdntls)
        {
            var clntDTO = (ClientDTO)usrCrdntls;
            var userToAdd = new IdentityUser
            {
                UserName = clntDTO.Name,
                PhoneNumber = clntDTO.PhoneNumber,
                Email = clntDTO.Email
            };
            var res = await _usrMngr.CreateAsync(userToAdd);
            await _usrMngr.AddToRoleAsync(userToAdd, CLIENT_ROLE);
            return res;
        }
        public async Task<IdentityResult> Delete(string id)
            => await _usrMngr.DeleteAsync(await _usrMngr.FindByIdAsync(id));
    }
}