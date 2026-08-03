using Raylib_cs;

namespace Rayterra.Core;

public static class Assets
{
    private static readonly Dictionary<string, Texture2D> _textures = new();
    private static readonly Dictionary<string, Sound> _sounds = new();

    private static readonly Dictionary<string, TextureAtlas> _atlases = new();

    public static void LoadTexture(string name, string path) => _textures.Add(name, Raylib.LoadTexture(path));

    public static void LoadSound(string name, string path) => _sounds.Add(name, Raylib.LoadSound(path));

    public static void InitAtlas(string name, string path, int tileSize)
    {
        LoadTexture(name, path);
        _atlases.Add(name, new TextureAtlas(GetTexture(name), tileSize));
    }

    public static Texture2D GetTexture(string name) => _textures[name];

    public static Sound GetSound(string name) => _sounds[name];

    public static TextureAtlas GetAtlas(string name) => _atlases[name];

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