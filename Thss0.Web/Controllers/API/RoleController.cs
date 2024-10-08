﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thss0.Web.Extensions;
using Thss0.Web.Models;
using Thss0.Web.Models.ViewModels;

namespace Thss0.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, professional")]
    public class RoleController(RoleManager<IdentityRole> roleManager) : Controller
    {
        [HttpGet("{printBy:int?}/{page:int?}/{order:bool?}")]
        public async Task<ActionResult<IEnumerable<ViewModel>>> GetRoles(int printBy = 20, int page = 1, bool order = true)
        {
            var roles = await roleManager.Roles.ToListAsync();
            return Json(new Response
            {
                Content = (order ? roles.OrderBy(role => role.Name) : roles.OrderByDescending(role => role.Name))
                                            .Skip((page - 1) * printBy).Take(printBy)
                                            .Select(role => new ViewModel { Id = role.Id, Name = role.Name ?? "" })
                , TotalAmount = roles.Count
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleViewModel>> GetRole(string id)
        {
            if (id == null || roleManager.Roles == null)
            {
                return NotFound();
            }
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return InitializeRole(role);
        }

        [HttpPost]
        public async Task<ActionResult<RoleViewModel>> Post(string roleName)
        {
            new InitializationHelper().Validation(ModelState, roleName);
            if (ModelState.IsValid)
            {
                var result = await roleManager.CreateAsync(new IdentityRole { Name = roleName });
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
                    var roleToUpdate = await roleManager.FindByIdAsync(id);
                    if (roleToUpdate != null)
                    {
                        var properties = typeof(RoleViewModel).GetProperties().Where(p => !new[] { "Id" }.Contains(p.Name)).ToArray();
                        for (int i = 0; i < properties.Length; i++)
                        {
                            properties[i].SetValue(roleToUpdate, properties[i].GetValue(role));
                        }
                        var result = await roleManager.UpdateAsync(roleToUpdate);
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
                    if (!Exists(role.Name))
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
            if (roleManager.Roles == null)
            {
                return BadRequest(new { err = "Entity set \"dbo.AspNetRoles\" is null." });
            }
            var roleToDelete = await roleManager.FindByIdAsync(id);
            if (roleToDelete != null)
            {
                var result = await roleManager.DeleteAsync(roleToDelete);
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

        private static RoleViewModel InitializeRole(IdentityRole source)
            => (RoleViewModel)new InitializationHelper().InitializeViewModel(source, new RoleViewModel());

        private bool Exists(string roleName)
            => roleManager.Roles.Any(role => role.Name == roleName);
    }
}
