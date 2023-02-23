using Microsoft.AspNetCore.Identity;
using Thss0.BLL.DTO;

namespace Thss0.BLL.Services
{
    internal interface IUsersService
    {
        IEnumerable<UserDTO> GetAll();
        Task<UserDTO> Get(string id);
        Task<IdentityResult> Add(UserDTO usrCrdntls);
        Task<IdentityResult> Delete(string id);
    }
}