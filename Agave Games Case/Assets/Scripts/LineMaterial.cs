using UnityEngine;

public class LineMaterial : MonoBehaviour
{
    private static Material lineMaterial;

    public static Material GetLineMaterial()
    {
        if (lineMaterial != null)
            return lineMaterial;
            
        Shader shader = Shader.Find("Sprites/Default") ?? Shader.Find("Unlit/Color");
        
        if (shader == null)
            return null;

        lineMaterial = new Material(shader);
        lineMaterial.color = Color.cyan;
        lineMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
        lineMaterial.enableInstancing = false;
        lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        lineMaterial.SetInt("_ZWrite", 0);
        lineMaterial.renderQueue = 4000;
        
        return lineMaterial;
    }

    public static void ResetLineMaterial()
    {
        lineMaterial = null;
    }
}
