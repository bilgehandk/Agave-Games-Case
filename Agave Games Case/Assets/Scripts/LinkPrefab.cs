using UnityEngine;

/// <summary>
/// Handles the visual representation of a link between tiles
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class LinkPrefab : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private int defaultSortingOrder = 100;
    [SerializeField] private string sortingLayerName = "UI";
    [SerializeField] private Color defaultColor = new Color(0.5f, 0.8f, 1f, 0.8f);
    [SerializeField] private Material linkMaterial;
    
    private SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        InitializeSpriteRenderer();
        ConfigureSpriteRenderer();
    }
    
    private void InitializeSpriteRenderer()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            Debug.Log($"Added SpriteRenderer to {gameObject.name}");
        }
    }
    
    private void ConfigureSpriteRenderer()
    {
        // Set sprite if missing
        if (spriteRenderer.sprite == null)
        {
            spriteRenderer.sprite = Resources.Load<Sprite>("WhiteSquare");
            if (spriteRenderer.sprite == null)
            {
                spriteRenderer.sprite = WhiteSquareCreator.CreateWhiteSquareSprite();
            }
        }
        
        // Set color if too transparent
        if (spriteRenderer.color.a < 0.1f)
        {
            spriteRenderer.color = defaultColor;
        }
        
        // Set sorting layer
        if (!string.IsNullOrEmpty(sortingLayerName))
        {
            spriteRenderer.sortingLayerName = sortingLayerName;
        }
        spriteRenderer.sortingOrder = defaultSortingOrder;
        
        // Apply custom material if available
        if (linkMaterial != null)
        {
            spriteRenderer.material = linkMaterial;
        }
    }
    
    /// <summary>
    /// Sets the color of the link
    /// </summary>
    public void SetColor(Color color)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }
    
    /// <summary>
    /// Sets the sorting order of the link
    /// </summary>
    public void SetSortingOrder(int order)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = order;
        }
    }
}
