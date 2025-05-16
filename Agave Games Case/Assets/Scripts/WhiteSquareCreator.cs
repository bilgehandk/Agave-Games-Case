using UnityEngine;

/// <summary>
/// Utility class for creating white square sprites at runtime
/// </summary>
public static class WhiteSquareCreator
{
    private const int TextureSize = 32;
    private const float PixelsPerUnit = 100f;
    private const string SpriteName = "WhiteSquare";

    /// <summary>
    /// Creates a white square sprite at runtime if it doesn't exist in Resources
    /// </summary>
    /// <returns>A white square sprite</returns>
    public static Sprite CreateWhiteSquareSprite()
    {
        // Try to load from resources first
        Sprite sprite = Resources.Load<Sprite>(SpriteName);
        
        // If not found, create one programmatically
        if (sprite == null)
        {
            // Create a white texture
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
            
            // Create a sprite from the texture
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
