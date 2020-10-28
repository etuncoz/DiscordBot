using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Models
{
    public class DotaHero
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonProperty("localized_name")]
        public string LocalizedName { get; set; }
        [JsonProperty("primary_attr")]
        public string PrimaryAttribute { get; set; }
        [JsonProperty("attack_type")]
        public string AttackType { get; set; }
        public List<string> Roles { get; set; }
        public int Legs { get; set; }
    }
}
