using Microsoft.AspNetCore.SignalR;

namespace SurveyPortal.Hubs
{
    public class UserHub : Hub
    {
        public async Task NotifyUserRoleChange(string userId, string newRole)
        {
            await Clients.All.SendAsync("ReceiveUserRoleChange", userId, newRole);
        }

        public async Task NotifyNewUser(string userId, string userName)
        {
            await Clients.All.SendAsync("ReceiveNewUser", userId, userName);
        }
    }
}