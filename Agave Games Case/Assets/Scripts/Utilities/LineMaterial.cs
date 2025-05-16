using UnityEngine;
using UnityEngine.Rendering;
public class LineMaterial : MonoBehaviour
{
    private static Material lineMaterial;
    private const string DefaultShaderName = "Sprites/Default";
    private const string FallbackShaderName = "Unlit/Color";
    public static Material GetLineMaterial()
    {
        if (lineMaterial != null)
            return lineMaterial;
        Shader shader = Shader.Find(DefaultShaderName) ?? Shader.Find(FallbackShaderName);
        if (shader == null)
        {
            Debug.LogError("Failed to find suitable shader for line material");
            return null;
        }
        lineMaterial = new Material(shader);
        lineMaterial.color = Color.cyan;
        lineMaterial.SetFloat("_ZTest", (float)CompareFunction.Always);
        lineMaterial.enableInstancing = false;
        lineMaterial.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
        lineMaterial.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
        lineMaterial.SetInt("_ZWrite", 0);
        lineMaterial.renderQueue = 4000;
        return lineMaterial;
    }
    public static void ResetLineMaterial()
    {
        if (lineMaterial != null)
        {
            Destroy(lineMaterial);
            lineMaterial = null;
        }
    }
}
