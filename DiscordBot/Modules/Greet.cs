using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class Greet : ModuleBase<SocketCommandContext>
    {
        [Command("salut")]
        public async Task GreetAsync()
        {
            await ReplyAsync("Selam canım");
        }

    }
}
