using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurveyPortal.Models;
using SurveyPortal.Hubs; // SignalR hub'ını ekleyin

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IHubContext<UserHub> _hubContext; // SignalR hub'ını ekleyin

    public AdminController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IHubContext<UserHub> hubContext) // SignalR hub'ını ekleyin
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _hubContext = hubContext; // SignalR hub'ını ekleyin
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        ViewBag.Layout = "_AdminLayout";
        base.OnActionExecuting(context);
    }

    public IActionResult Dashboard()
    {
        return View();
    }

    public async Task<IActionResult> Users()
    {
        var users = _userManager.Users.ToList();
        var userRoles = new List<UserRoleViewModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userRoles.Add(new UserRoleViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = roles.ToList()
            });
        }

        return View(userRoles);
    }

    [HttpPost]
    public async Task<IActionResult> ChangeUserRole(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, role);

            // SignalR üzerinden istemcilere bildirim gönder
            await _hubContext.Clients.All.SendAsync("ReceiveUserRoleChange", userId, role);
        }

        return RedirectToAction("Users");
    }
}

public class UserRoleViewModel
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public List<string> Roles { get; set; }
}