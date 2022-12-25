using System.Text.Json.Serialization;

namespace GadgetMarket.Model
{
    public class GadgetsPageModel
    {
        [JsonPropertyName("result")]
        public List<Gadget> Gadgets { get; set; } = null!;

        [JsonPropertyName("hits")]
        public int TotalCount { get; set; }
    }
}
