using System;
using System.Text.Json.Serialization;

namespace MBModManager.Data;

public class ModInfo {
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public string? Author { get; set; }
    public int? NexusId { get; set; }
    [JsonPropertyName("isEnabled")]
    public bool IsEnabled { get; set; }
    [JsonPropertyName("isInstalled")]
    public bool IsInstalled { get; set; }
    public Tag[]? Tags { get; set; }
    [JsonPropertyName("depends_on")]
    public ModInfo[]? DependsOn { get; set; }
    public string[]? ModFiles { get; set; }


    public ModInfo(int id, string modName, string desc, string image, string auth, int nexusModId, Tag[] tags) {
        Id = id;
        Name = modName;
        Description = desc;
        Image = image;
        Author = auth;
        NexusId = nexusModId;
        IsEnabled = false;
        IsInstalled = false;
        Tags = tags;
        DependsOn = Array.Empty<ModInfo>();
        ModFiles = Array.Empty<string>();
    }
}

public class Tag {
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Color { get; set; }
    public string? Uuid { get; set; }

    public Tag (int? id, string? text, string? color, string? uuid) {
        Id = id;
        Name = text;
        Color = color;
        Uuid = uuid;
    }
}