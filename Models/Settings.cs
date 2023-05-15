namespace MBModManager.Models
{
    public class Settings
    {
        public string? GamePath { get; set; }
        public string? GameVersion { get; }
        public string? SavesPath { get; set; }

        public Settings(string game, string saves, string ver)
        {
            GamePath = game;
            SavesPath = saves;
            GameVersion = ver;
        }
    }



}
