using System.Text.Json.Serialization;

namespace TodoServer.DTOs;

public class UserDTO
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; }
}