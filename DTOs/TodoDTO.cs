namespace TodoServer.DTOs;
public record TodoDTO
{
    public string Description { get; set; }
    public bool IsCompleted { get; set; } = false;
}

public record TodoUpdateDTO
{
    public string Description { get; set; }
    public bool IsCompleted { get; set; } = false;
}