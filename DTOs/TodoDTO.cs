using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TodoServer.DTOs;
public record TodoDTO
{
    [Required]
    [MinLength(3)]
    [MaxLength(255)]
    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("is_completed")]
    public bool IsCompleted { get; set; } = false;
}

public record TodoUpdateDTO
{
    [MinLength(3)]
    [MaxLength(255)]
    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("is_completed")]
    public bool IsCompleted { get; set; } = false;
}