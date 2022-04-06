namespace TodoServer.Models;
public record Todo
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; } = false;
}