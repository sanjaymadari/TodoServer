using System.Text.Json.Serialization;

namespace TodoServer.Models;
public record Todo
{
    [JsonPropertyName("id")]
    public long Id { get; set; }


    [JsonPropertyName("user_id")]
    public long UserId { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("is_completed")]
    public bool IsCompleted { get; set; } = false;
}