﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MBModManager.Models;

public class ModInfo
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public string? Author { get; set; }
    public int? NexusId { get; set; }
    [JsonProperty("isEnabled")] public bool IsEnabled { get; set; }
    [JsonProperty("isInstalled")] public bool IsInstalled { get; set; }
    public Tag[]? Tags { get; set; }
    [JsonProperty("depends_on")] public ModInfo[]? DependsOn { get; set; }
    public List<string>? ModFiles { get; set; }

    public ModInfo(int id, string modName, string desc, string image, string auth, int nexusModId, Tag[] tags)
    {
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
        ModFiles = new List<string>();
    }
}
