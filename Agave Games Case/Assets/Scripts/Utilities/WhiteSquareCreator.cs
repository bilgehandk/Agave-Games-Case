using UnityEngine;

public static class WhiteSquareCreator
{
    private const int TextureSize = 32;
    private const float PixelsPerUnit = 100f;
    private const string SpriteName = "WhiteSquare";

    public static Sprite CreateWhiteSquareSprite()
    {
        Sprite sprite = Resources.Load<Sprite>(SpriteName);
        if (sprite == null)
        {
            Texture2D texture = new Texture2D(TextureSize, TextureSize);
            Color white = Color.white;
            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    texture.SetPixel(x, y, white);
                }
            }
            texture.Apply();
            sprite = Sprite.Create(
                texture, 
                new Rect(0, 0, texture.width, texture.height), 
                new Vector2(0.5f, 0.5f), 
                PixelsPerUnit
            );
            sprite.name = SpriteName;
        }
        return sprite;
    }
}
