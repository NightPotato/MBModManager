using System.Collections.Generic;

namespace MBModManager.Data {

    public class ModInfo {
        public int? Id {  get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string? Author { get; set; }
        public int? NexusId { get; set; }
        public bool isEnabled { get; set; }
        public bool isInstalled { get; set; }
        public Tag[]? Tags { get; set; }
        public ModInfo[]? depends_on { get; set; }
        public List<string>? ModFiles { get; set; }


        public ModInfo(int id, string modName, string desc, string image, string auth, int NexusModId, Tag[] tags) {
            Id = id;
            Name = modName;
            Description = desc;
            Image = image;
            Author = auth;
            NexusId = NexusModId;
            isEnabled = false;
            isInstalled = false;
            Tags = tags;
            depends_on = new ModInfo[] { };
            ModFiles = new List<string> { };
        }
    }

    public class Tag {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Color { get; set; }
        public string? UUID { get; set; }

        public Tag (int? id, string? text, string? color, string? uuid) {
            Id = id;
            Name = text;
            Color = color;
            UUID = uuid;
        }
    }


}
