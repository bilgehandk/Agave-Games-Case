#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateWhiteSprite : MonoBehaviour
{
    [MenuItem("Tools/Create White Square Sprite")]
    public static void CreateWhiteSquare()
    {
        // Create a 32x32 white texture
        Texture2D tex = new Texture2D(32, 32);
        Color white = Color.white;
        
        for (int y = 0; y < tex.height; y++)
        {
            for (int x = 0; x < tex.width; x++)
            {
                tex.SetPixel(x, y, white);
            }
        }
        
        tex.Apply();
        
        // Make sure the Resources directory exists
        if (!Directory.Exists("Assets/Resources"))
        {
            Directory.CreateDirectory("Assets/Resources");
        }
        
        // Save the texture as a PNG
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes("Assets/Resources/WhiteSquare.png", bytes);
        AssetDatabase.Refresh();
        
        // Load the texture and create a sprite from it
        TextureImporter importer = AssetImporter.GetAtPath("Assets/Resources/WhiteSquare.png") as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.filterMode = FilterMode.Bilinear;
            AssetDatabase.ImportAsset("Assets/Resources/WhiteSquare.png", ImportAssetOptions.ForceUpdate);
        }
        
        Debug.Log("White square sprite created at Assets/Resources/WhiteSquare.png");
    }
}
#endif
