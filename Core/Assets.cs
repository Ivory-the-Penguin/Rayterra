using System.Reflection.Metadata.Ecma335;
using Raylib_cs;

namespace Rayterra.Core;

public static class Assets
{
    private static readonly Dictionary<string, Texture2D> _textures = new();
    private static readonly Dictionary<string, Sound> _sounds = new();

    private static readonly Dictionary<string, TextureAtlas> _atlases = new();

    public static void CheckPath(string path)
    {

        if (!Path.Exists(path))
        {
            throw new InvalidOperationException($"Path '{path}' does not exist!");
        }
    }

    public static Texture2D LoadTexture(string name, string path)
    {
        CheckPath(path);

        _textures.Add(name, Raylib.LoadTexture(path));
        return _textures[name];
    }

    public static Sound LoadSound(string name, string path)
    {
        CheckPath(path);

        _sounds.Add(name, Raylib.LoadSound(path));
        return _sounds[name];
    }

    public static TextureAtlas InitAtlas(string name, string path, int tileSize)
    {
        CheckPath(path);

        LoadTexture(name, path);
        _atlases.Add(name, new TextureAtlas(GetTexture(name), tileSize));

        return _atlases[name];
    }

    public static Texture2D GetTexture(string name)
    {
        if (!_textures.ContainsKey(name))
        {
            throw new InvalidOperationException($"Texture '{name}' does not exist!");
        }

        return _textures[name];
    }

    public static Sound GetSound(string name)
    {
        if (!_sounds.ContainsKey(name))
        {
            throw new InvalidOperationException($"Sound '{name}' does not exist!");
        }

        return _sounds[name];
    }


    public static TextureAtlas GetAtlas(string name)
    {
        if (!_atlases.ContainsKey(name))
        {
            throw new InvalidOperationException($"Atlas '{name}' does not exist!");
        }

        return _atlases[name];
    }

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