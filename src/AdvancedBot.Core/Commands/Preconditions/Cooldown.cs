using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Discord.Commands;

namespace AdvancedBot.Core.Commands.Preconditions
{
    public class CooldownAttribute : PreconditionAttribute
    {
        private ConcurrentDictionary<ulong, DateTime> _cooldowns = new ConcurrentDictionary<ulong, DateTime>();
        private uint _cooldownInMs = 0;

        public CooldownAttribute(uint cooldownInMs)
        {
            _cooldownInMs = cooldownInMs;
        }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (_cooldownInMs == 0) { return Task.FromResult(PreconditionResult.FromSuccess()); }

            if (!_cooldowns.ContainsKey(context.Guild.Id))
            {
                _cooldowns.TryAdd(context.Guild.Id, DateTime.Now);
                return Task.FromResult(PreconditionResult.FromSuccess());
            }
            _cooldowns.TryGetValue(context.Guild.Id, out DateTime lastExecution);
            if ((DateTime.Now - lastExecution).TotalMilliseconds >= _cooldownInMs)
            {
                    return Task.FromResult(PreconditionResult.FromSuccess());
            }
            var timeLeftInMins = (DateTime.Now - lastExecution).Minutes;
            var timeLeftInSecs = (DateTime.Now - lastExecution).Seconds;

            return Task.FromResult(PreconditionResult.FromError($"Command is still on cooldown. Try again in **{timeLeftInMins}** minutes and **{timeLeftInSecs}** seconds."));
        }
    }
}
