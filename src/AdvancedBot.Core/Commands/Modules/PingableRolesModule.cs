using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvancedBot.Core.Commands.Preconditions;
using AdvancedBot.Core.Services.DataStorage;
using Discord;
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

        [Command("addiwanttoplay")][RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task AddIWantToPlay(SocketRole role, [Remainder]string trigger)
        {
            var guild = _accounts.GetOrCreateGuildAccount(Context.Guild.Id);
            guild.AddPingableRole(trigger, role.Id);

            _accounts.SaveGuildAccount(guild);
            await ReplyAsync($"Successfully added the trigger {trigger} for {role.Name}.");
        }

        [Command("removeiwanttoplay")][RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task RemoveIWantToPlay([Remainder]string trigger)
        {
            var guild = _accounts.GetOrCreateGuildAccount(Context.Guild.Id);
            guild.RemovePingableRole(trigger);

            _accounts.SaveGuildAccount(guild);
            await ReplyAsync($"Successfully removed the trigger {trigger}.");
        }

        [Command("listiwanttoplay")][RequireCustomPermission(GuildPermission.ManageRoles)]
        public async Task ListIWantToPlay()
        {
            var guild = _accounts.GetOrCreateGuildAccount(Context.Guild.Id);
            var roles = new List<IRole>();

            for (int i = 0; i < guild.PingableRoles.Count; i++)
            {
                var roleId = guild.PingableRoles.Values.ToArray()[i];
                var currentRole = Context.Guild.Roles.First(x => x.Id == roleId);
                if (!(currentRole is null))
                    roles.Add(currentRole);
            }

            if (roles.Count is 0) throw new Exception("This server doesn't have any pingable roles.");
            await ReplyAsync($"IWantToPlayRoles for **{Context.Guild.Name}**\n" + 
                            $"▬▬▬▬▬▬▬▬▬▬▬▬\n" + 
                            $"`{string.Join("`, `", roles.Select(x => $"{x.Name}"))}`");
        }

        [Command("iwanttoplay")][Cooldown(600000)]
        public async Task IWantToPlay([Remainder]string trigger)
        {
            var guild = _accounts.GetOrCreateGuildAccount(Context.Guild.Id);
            if (!guild.PingableRoles.TryGetValue(trigger, out ulong roleId))
                throw new Exception($"Couldn't find the command for {trigger}.");

            var role = Context.Guild.Roles.First(x => x.Id == roleId);
            var guildUser = Context.User as SocketGuildUser;
            if (guildUser.Roles.FirstOrDefault(x => x.Id == role.Id) is null)
                throw new Exception($"You need role **{role.Name}** in order to use this command.");

            await ReplyAsync($"Hey <@&{role.Id}>, {Context.User.Mention} wants to play!");
        }
    }
}
