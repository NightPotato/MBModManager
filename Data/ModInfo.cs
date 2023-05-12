using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBModManager.Data {

    public class ModInfo {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Author { get; set; }
        public string[]? Tags { get; set; }
        public string[]? depends_on { get; set; }
        public string[]? ModFiles { get; set; }


        public ModInfo(string modName, string desc, string auth) {
            Name = modName;
            Description = desc;
            Author = auth;
            Tags = new string[] { };
            depends_on = new string[] { };
            ModFiles = new string[] { };
        }
    }

}
