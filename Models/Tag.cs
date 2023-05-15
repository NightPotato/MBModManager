namespace MBModManager.Models;

public class Tag
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Color { get; set; }
    public string? Uuid { get; set; }

    public Tag(int? id, string? text, string? color, string? uuid)
    {
        Id = id;
        Name = text;
        Color = color;
        Uuid = uuid;
    }
}
