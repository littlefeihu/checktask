using Newtonsoft.Json;
namespace LexisNexis.Red.Common.Entity
{
    public class GuidCards
    {
         [JsonProperty("name")]
        public string GuidCardName { get; set; }
         [JsonProperty("comments")]
        public string Comments { get; set; }
    }
}
