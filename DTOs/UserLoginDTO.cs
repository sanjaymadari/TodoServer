using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TodoServer.DTOs;

public class UserLoginDTO
{

    [JsonPropertyName("email")]
    [Required]
    public string Email { get; set; }

    [JsonPropertyName("password")]
    [Required]
    public string Password { get; set; }
}