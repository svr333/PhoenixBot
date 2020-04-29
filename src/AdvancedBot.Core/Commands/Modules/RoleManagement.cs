using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace AdvancedBot.Core.Commands.Modules
{
    public class RoleManagement : CustomModule
    {
        [Command("addrole")][Alias("ar")]
        public async Task AddRole(SocketGuildUser user, SocketRole role)
        {
            await user.AddRoleAsync(role);
            await ReplyAsync($"Successfully added role **{role.Name}** to **{user.Username}**.");
        }

        [Command("removerole")][Alias("rr")]
        public async Task RemoveRole(SocketGuildUser user, SocketRole role)
        {
            if (!user.Roles.Contains(role)) throw new Exception("User doesn't have this role");
            await user.RemoveRoleAsync(role);
            await ReplyAsync($"Successfully removed **{role.Name}** from **{user.Username}**.");
        }
    }
}
