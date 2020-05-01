using System;
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

        [Command("iam")][Cooldown(30000)]
        public async Task ObtainSOR([Remainder]SocketRole role)
        {
            var guild = _accounts.GetOrCreateGuildAccount(Context.Guild.Id);
            if (!guild.RoleIdIsInObtainableRolesList(role.Id)) throw new Exception("You cannot obtain this role via iam command.");
            
            var guildUser =  Context.User as SocketGuildUser;
            await guildUser.AddRoleAsync(role);
            var message = await ReplyAsync($"You successfully obtained role **{role.Name}**.");
            //_invokes.InstantiateNewTimer(message, 10);
        }

        [Command("iamnot")][Cooldown(30000)]
        public async Task RemoveSOR([Remainder]SocketRole role)
        {
            var guild = _accounts.GetOrCreateGuildAccount(Context.Guild.Id);
            if (!guild.RoleIdIsInObtainableRolesList(role.Id)) throw new Exception("You cannot remove this role via iam command.");

            var guildUser =  Context.User as SocketGuildUser;
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
    }
}
