using System;
using System.Linq;
using System.Threading.Tasks;
using AdvancedBot.Core.Services.DataStorage;
using Discord.Commands;
using Discord.WebSocket;

namespace AdvancedBot.Core.Commands.Modules
{
    public class PingableRolesModule : CustomModule
    {
        private GuildAccountService _accounts;

        public PingableRolesModule(GuildAccountService accounts)
        {
            _accounts = accounts;
        }

        [Command("addiwanttoplay")]
        public async Task AddIWantToPlay(SocketRole role, [Remainder]string trigger)
        {
            var guild = _accounts.GetOrCreateGuildAccount(Context.Guild.Id);
            guild.AddPingableRole(trigger, role.Id);

            _accounts.SaveGuildAccount(guild);
            await ReplyAsync($"Successfully added the trigger {trigger} for {role.Name}.");
        }

        [Command("removeiwanttoplay")]
        public async Task RemoveIWantToPlay([Remainder]string trigger)
        {
            var guild = _accounts.GetOrCreateGuildAccount(Context.Guild.Id);
            guild.RemovePingableRole(trigger);

            _accounts.SaveGuildAccount(guild);
            await ReplyAsync($"Successfully removed the trigger {trigger}.");
        }

        [Command("iwanttoplay")]
        public async Task IWantToPlay([Remainder]string trigger)
        {
            var guild = _accounts.GetOrCreateGuildAccount(Context.Guild.Id);
            if (!guild.PingableRoles.TryGetValue(trigger, out ulong roleId))
                throw new Exception($"Couldn't find the command for {trigger}");

            var role = Context.Guild.Roles.First(x => x.Id == roleId);

            await ReplyAsync($"Hey {role.Mention}, {Context.User.Mention} wants to play some games!");
        }
    }
}
