using System.Text.Json.Serialization;

namespace TopCharts.Domain.Model.Api
{
    public class BlockData
    {
        public string Text { get; set; }
        [JsonPropertyName("text_truncated")]
        public string TextTruncated { get; set; }
    }
}