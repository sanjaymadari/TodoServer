using System.Text.Json.Serialization;

namespace TodoServer.DTOs;

public class UserLoginDTO
{

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; }
}