using Discord;
using Discord.Commands;
using DiscordBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class DotaCommands : ModuleBase<SocketCommandContext>
    {
        private List<DotaHero> _dotaHeroes { get; set; }
        private readonly string _openAiDotaHeroesUrl = ConfigurationManager.AppSettings["OpenAiDotaHeroesUrl"];
        private List<string> _blockedUsernames = ConfigurationManager.AppSettings["BlockedUsernames"]?
            .Replace(" ", "")
            .Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .ToList();

        private static readonly Random _random = new Random();

        public DotaCommands()
        {
            SetDotaHeroes();
        }


        [Command("salut")]
        public async Task GreetAsync()
        {
            await ReplyAsync("Selam canım");
        }

        [Command("random")]
        public async Task RandomAsync()
        {
            if (_blockedUsernames != null && _blockedUsernames.Contains(this.Context.User.Username))
            {
                await ReplyAsync($"{this.Context.Message.Author.Mention} sen sikimlen oyna..");
                return;
            }

            var randomId = _random.Next(1, _dotaHeroes.Count);

            var randomHero = _dotaHeroes.FirstOrDefault(h => h.Id == randomId);

            await ReplyAsync($"{randomHero.LocalizedName} time biatch! {this.Context.Message.Author.Mention}");
        }

        [Command("randomcarry")]
        public async Task RandomCarryAsync()
        {
            if (_blockedUsernames != null && _blockedUsernames.Contains(this.Context.User.Username))
            {
                await ReplyAsync($"{this.Context.Message.Author.Mention} sen sikimlen oyna..");
                return;
            }

            var randomId = _random.Next(1, _dotaHeroes.Count);

            const string carryKeyword = "Carry";

            DotaHero randomHero = _dotaHeroes.FirstOrDefault(h => h.Roles.Contains(carryKeyword) && h.Id < randomId);

            if (randomHero == null)
            {
                randomHero = _dotaHeroes.FirstOrDefault(h => h.Roles.Contains(carryKeyword) && h.Id > randomId);
            }

            await ReplyAsync($"{randomHero.LocalizedName} ile kerileyecen! {this.Context.Message.Author.Mention}");
        }

        [Command("randomsup")]
        public async Task RandomSupportAsync()
        {
            if (_blockedUsernames != null && _blockedUsernames.Contains(this.Context.User.Username))
            {
                await ReplyAsync($"{this.Context.Message.Author.Mention} sen sikimlen oyna..");
                return;
            }

            var randomId = _random.Next(1, _dotaHeroes.Count);

            const string supportKeyword = "Support";

            DotaHero randomHero = _dotaHeroes.FirstOrDefault(h => h.Roles.Contains(supportKeyword) && h.Id < randomId);

            if (randomHero == null)
            {
                randomHero = _dotaHeroes.FirstOrDefault(h => h.Roles.Contains(supportKeyword) && h.Id > randomId);
            }

            await ReplyAsync($"{randomHero.LocalizedName} ile suportlayacan! {this.Context.Message.Author.Mention}");
        }

        private List<DotaHero> GetDotaHeroes()
        {
            using (var client = new HttpClient())
            {
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, new Uri(_openAiDotaHeroesUrl));
                httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var sendResult = client.GetAsync(httpRequest.RequestUri).Result;
                sendResult.EnsureSuccessStatusCode();
                var httpResponse = sendResult.Content.ReadAsStringAsync().Result;

                return JsonConvert.DeserializeObject<List<DotaHero>>(httpResponse);
            }
        }

        private void SetDotaHeroes()
        {
            if (_dotaHeroes == null || _dotaHeroes.Count == default(int))
            {
                //TODO: cache dotaheroes so we dont have to get them via api evertime
                _dotaHeroes = GetDotaHeroes();
            }
        }

    }
}
