using Raylib_cs;

namespace Core;

public static class Assets
{
    private static readonly Dictionary<string, Texture2D> _textures = new(); private static readonly Dictionary<string, Sound> _sounds = new();

    public static void LoadTexture(string name, string path) => _textures.Add(name, Raylib.LoadTexture(path));

    public static void LoadSound(string name, string path) => _sounds.Add(name, Raylib.LoadSound(path));

    public static Texture2D GetTexture(string name) => _textures[name];

    public static Sound GetSound(string name) => _sounds[name];

    public static void Cleanup()
    {
        foreach (var item in _textures)
        {
            Raylib.UnloadTexture(item.Value);
        }

        foreach (var item in _sounds)
        {
            Raylib.UnloadSound(item.Value);
        }
    }
}