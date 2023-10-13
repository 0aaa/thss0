using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thss0.Web.Extensions;
using Thss0.Web.Models.ViewModels;

namespace Thss0.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
            => _roleManager = roleManager;

        [HttpGet("{order:bool?}/{printBy:int?}/{page:int?}")]
        public async Task<ActionResult<IEnumerable<RoleViewModel>>> GetRoles(bool order = true, int printBy = 20, int page = 1)
        {
            var roles = await _roleManager.Roles.ToListAsync();
            if (!roles.Any())
            {
                return NoContent();
            }
            return Json(new
            {
                content = (order ? roles.OrderBy(role => role.Name) : roles.OrderByDescending(role => role.Name))
                                            .Skip((page - 1) * printBy).Take(printBy)
                                            .Select(role => new RoleViewModel { Id = role.Id, Name = role.Name })
                , total_amount = roles.Count
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleViewModel>> GetRole(string id)
        {
            if (id == null || _roleManager.Roles == null)
            {
                return NotFound();
            }
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return InitializeUser(role);
        }

        [HttpPost]
        public async Task<ActionResult<RoleViewModel>> Post(string roleName)
        {
            new EntityInitializer().Validation(ModelState, roleName);
            if (ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole { Name = roleName });
                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                    {
                        ModelState.AddModelError(err.Code, err.Description);
                    }
                    return BadRequest(ModelState);
                }
                return Ok(roleName);
            }
            return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RoleViewModel>> Put(string id, RoleViewModel role)
        {
            if (id != role.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var roleToUpdate = await _roleManager.FindByIdAsync(id);
                    if (roleToUpdate != null)
                    {
                        var properties = typeof(RoleViewModel).GetProperties().Where(p => !new[] { "Id" }.Contains(p.Name)).ToArray();
                        for (ushort i = 0; i < properties.Length; i++)
                        {
                            properties[i].SetValue(roleToUpdate, properties[i].GetValue(role));
                        }
                        var result = await _roleManager.UpdateAsync(roleToUpdate);
                        if (!result.Succeeded)
                        {
                            foreach (var err in result.Errors)
                            {
                                ModelState.AddModelError(err.Code, err.Description);
                            }
                            return BadRequest(ModelState);
                        }
                    }
                }
                catch (Exception e)
                {
                    if (!RoleExists(role.Name))
                    {
                        return NotFound();
                    }
                    Console.WriteLine(e.Message);
                }
                return Ok(role);
            }
            return BadRequest(role);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<RoleViewModel>> Delete(string id)
        {
            if (_roleManager.Roles == null)
            {
                return BadRequest(new { err = "Entity set \"dbo.AspNetRoles\" is null." });
            }
            var roleToDelete = await _roleManager.FindByIdAsync(id);
            if (roleToDelete != null)
            {
                var result = await _roleManager.DeleteAsync(roleToDelete);
                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                    {
                        ModelState.AddModelError(err.Code, err.Description);
                    }
                    return BadRequest(ModelState);
                }
                return Ok();
            }
            return NotFound();
        }

        private RoleViewModel InitializeUser(IdentityRole source)
            => (RoleViewModel)new EntityInitializer().InitializeViewModel(source, new RoleViewModel());

        private bool RoleExists(string roleName)
            => _roleManager.Roles.Any(role => role.Name == roleName);
    }
}
