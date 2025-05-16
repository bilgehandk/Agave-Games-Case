using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LinkPrefab : MonoBehaviour
{
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
        }
    }
    private void ConfigureSpriteRenderer()
    {
        if (spriteRenderer.sprite == null)
        {
            spriteRenderer.sprite = Resources.Load<Sprite>("WhiteSquare");
            if (spriteRenderer.sprite == null)
            {
                spriteRenderer.sprite = WhiteSquareCreator.CreateWhiteSquareSprite();
            }
        }
        if (spriteRenderer.color.a < 0.1f)
        {
            spriteRenderer.color = defaultColor;
        }
        if (!string.IsNullOrEmpty(sortingLayerName))
        {
            spriteRenderer.sortingLayerName = sortingLayerName;
        }
        spriteRenderer.sortingOrder = defaultSortingOrder;
        if (linkMaterial != null)
        {
            spriteRenderer.material = linkMaterial;
        }
    }
    public void SetColor(Color color)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }
    public void SetSortingOrder(int order)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = order;
        }
    }
}
