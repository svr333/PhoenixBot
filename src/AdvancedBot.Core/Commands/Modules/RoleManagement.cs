using AdvancedBot.Core.Commands;

namespace AdvancedBot.Core.Commands.Modules
{
    public class RoleManagement : CustomModule
    {
        [Command("addrole")]
        public async Task AddRole(SocketGuildUser user, SocketRole role)
        {
            await user.AddRoleAsync(role);
            await ReplyAsync("Successfully added role **{role.Name}** to **{user.Username}**");
        }
    }
}
