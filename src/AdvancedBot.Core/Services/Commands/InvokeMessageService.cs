using System.Collections.Concurrent;
using System.Linq;
using System.Timers;
using Discord;
using Discord.WebSocket;

namespace AdvancedBot.Core.Services.Commands
{
    public class InvokeMessageService
    {
        private ConcurrentDictionary<SocketUserMessage, Timer> _activeInvokes;

        public InvokeMessageService()
        {
            _activeInvokes = new ConcurrentDictionary<SocketUserMessage, Timer>();
        }

        public void InstantiateNewTimer(IUserMessage message, int time)
        {
            var timer = new Timer();
            timer.Interval = time;
            timer.Start();
            timer.Elapsed += OnTimerElapsed;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            var timer = sender as Timer;
            var message = _activeInvokes.First(x => x.Value == timer).Key;
            timer.Stop();

            message.DeleteAsync();
            _activeInvokes.TryRemove(message, out Timer oldTimer);

            timer.Dispose();
        }
    }
}
