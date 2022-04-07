using System.Text.Json.Serialization;

namespace TodoServer.DTOs;
public record TodoDTO
{
    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("is_completed")]
    public bool IsCompleted { get; set; } = false;
}

public record TodoUpdateDTO
{
    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("is_completed")]
    public bool IsCompleted { get; set; } = false;
}