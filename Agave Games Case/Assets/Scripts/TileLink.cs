using UnityEngine;

public class TileLink : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            if (spriteRenderer == null)
            {
                Debug.LogWarning("No SpriteRenderer found on TileLink object.");
            }
        }
    }
    
    public void SetColor(Color color)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }
} 