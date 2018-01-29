using Newtonsoft.Json;
namespace LexisNexis.Red.Common.BusinessModel
{
    public class GuideCard
    {

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("comments")]
        public string Comments { get; set; }
    }
}
