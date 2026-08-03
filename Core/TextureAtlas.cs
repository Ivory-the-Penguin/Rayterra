using System.Numerics;
using Raylib_cs;

namespace Rayterra.Core;

public class TextureAtlas
{
    private Texture2D _atlas;
    private int _tileSize;

    public TextureAtlas(Texture2D texture, int tileSize)
    {
        _atlas = texture;
        _tileSize = tileSize;
    }

    public void RenderTile(int tileX, int tileY, Vector2 position, float scale)
    {
        Rectangle source = new()
        {
            X = tileX * _tileSize,
            Y = tileY * _tileSize,
            Width = _tileSize,
            Height = _tileSize
        };

        Rectangle dest = new()
        {
            X = position.X,
            Y = position.Y,
            Width = scale * _tileSize,
            Height = scale * _tileSize
        };

        Raylib.DrawTexturePro(_atlas, source, dest, new Vector2(dest.Width, dest.Height), 0, Color.White);
    }
}