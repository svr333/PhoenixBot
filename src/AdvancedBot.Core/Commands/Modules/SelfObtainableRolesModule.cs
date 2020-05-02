using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvancedBot.Core.Commands.Preconditions;
using AdvancedBot.Core.Services.Commands;
using AdvancedBot.Core.Services.DataStorage;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace AdvancedBot.Core.Commands.Modules
{
    public class SelfObtainableRoles : CustomModule
    {
        private GuildAccountService _accounts;
        private InvokeMessageService _invokes;

        public SelfObtainableRoles(GuildAccountService accounts, InvokeMessageService invokes)
        {
            _accounts = accounts;
            _invokes = invokes;
        }

        [Command("iam")][Cooldown(300000)]
        public async Task ObtainSOR([Remainder]SocketRole role)
        {
            var guild = _accounts.GetOrCreateGuildAccount(Context.Guild.Id);
            if (!guild.RoleIdIsInObtainableRolesList(role.Id)) throw new Exception("You cannot obtain this role via iam command.");
            
            var guildUser =  Context.User as SocketGuildUser;

            if (guildUser.Roles.Contains(role)) throw new Exception($"**{guildUser.Nickname}** already has this role.");
            await guildUser.AddRoleAsync(role);
            var message = await ReplyAsync($"You successfully obtained role **{role.Name}**.");
            //_invokes.InstantiateNewTimer(message, 10);
        }

        [Command("iamnot")]
        public async Task RemoveSOR([Remainder]SocketRole role)
        {
            var guild = _accounts.GetOrCreateGuildAccount(Context.Guild.Id);
            if (!guild.RoleIdIsInObtainableRolesList(role.Id)) throw new Exception("You cannot remove this role via iam command.");

            var guildUser =  Context.User as SocketGuildUser;

            if (!guildUser.Roles.Contains(role)) throw new Exception($"**{guildUser.Nickname}** doesn't have this role.");
            await guildUser.RemoveRoleAsync(role);
            var message = await ReplyAsync($"Role **{role.Name}** successfully removed from your account.");
            //_invokes.InstantiateNewTimer(message, 10);
        }

        [Command("addiam")][RequireCustomPermission(GuildPermission.ManageRoles)]
        public async Task AddSORToList([Remainder]SocketRole role)
        {
            var guild = _accounts.GetOrCreateGuildAccount(Context.Guild.Id);
            guild.AddSelfObtainableRole(role.Id);

            _accounts.SaveGuildAccount(guild);
            await ReplyAsync($"Successfully added **{role.Name}** with id **{role.Id}** to the list of obtainable roles.");
        }

        [Command("removeiam")][RequireCustomPermission(GuildPermission.ManageRoles)]
        public async Task RemoveSORFromList([Remainder]SocketRole role)
        {
            var guild = _accounts.GetOrCreateGuildAccount(Context.Guild.Id);
            guild.RemoveSelfObtainableRole(role.Id);

            _accounts.SaveGuildAccount(guild);
            await ReplyAsync($"Successfully removed **{role.Name}** with id **{role.Id}** to the list of obtainable roles.");
        }

        [Command("listiam")][RequireCustomPermission(GuildPermission.ManageRoles)]
        public async Task ListIAm()
        {
            var guild = _accounts.GetOrCreateGuildAccount(Context.Guild.Id);

            var roles = new List<IRole>();

            for (int i = 0; i < guild.SelfObtainableRoles.Count; i++)
            {
                var roleId = guild.SelfObtainableRoles[i];
                var currentRole = Context.Guild.Roles.First(x => x.Id == roleId);
                if (!(currentRole is null))
                    roles.Add(currentRole);
            }
            if (roles.Count is 0) throw new Exception("This server doesn't have any self obtainable roles.");
            await ReplyAsync($"Self Obtainable Roles for **{Context.Guild.Name}**\n" + 
                            $"▬▬▬▬▬▬▬▬▬▬▬▬\n" +
                            $"`{string.Join("´, ´", roles.Select(x => $"{x.Name}"))}`");
        }
    }
}
