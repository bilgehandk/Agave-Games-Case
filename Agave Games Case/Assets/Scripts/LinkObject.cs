using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Handles the visual representation and animation of links between tiles
/// </summary>
public class LinkObject : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image image;
    [SerializeField] private Color linkColor = Color.white;
    
    [Header("Size Settings")]
    [SerializeField] private float horizontalWidth = 50f;
    [SerializeField] private float horizontalHeight = 10f;
    [SerializeField] private float verticalWidth = 10f;
    [SerializeField] private float verticalHeight = 50f;
    
    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private Ease appearEase = Ease.OutBack;
    [SerializeField] private Ease disappearEase = Ease.InBack;
    
    private void Awake()
    {
        InitializeComponents();
    }
    
    private void InitializeComponents()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        
        if (image == null)
            image = GetComponent<Image>();
        
        if (image != null)
            image.color = linkColor;
        
        transform.localScale = Vector3.zero;
    }
    
    /// <summary>
    /// Sets up and animates a link between two tiles
    /// </summary>
    /// <param name="firstTile">The first tile in the match</param>
    /// <param name="secondTile">The second tile in the match</param>
    /// <returns>The animation sequence</returns>
    public Sequence AnimateBetweenTiles(Tile firstTile, Tile secondTile)
    {
        if (firstTile == null || secondTile == null)
        {
            Debug.LogWarning("Cannot animate between null tiles");
            return DOTween.Sequence();
        }
        
        // Determine if this is a horizontal or vertical match
        bool isHorizontal = firstTile.y == secondTile.y;
        
        // Position between the two tiles
        transform.position = Vector3.Lerp(
            firstTile.transform.position, 
            secondTile.transform.position, 
            0.5f
        );
        
        SetLinkOrientation(isHorizontal, firstTile, secondTile);
        
        // Create animation sequence
        Sequence sequence = DOTween.Sequence();
        
        // Start with scale 0
        transform.localScale = Vector3.zero;
        
        // Animate the link
        sequence.Append(transform.DOScale(Vector3.one, animationDuration).SetEase(appearEase));
        sequence.AppendInterval(0.1f);
        sequence.Append(transform.DOScale(Vector3.zero, animationDuration).SetEase(disappearEase));
        
        return sequence;
    }
    
    private void SetLinkOrientation(bool isHorizontal, Tile firstTile, Tile secondTile)
    {
        if (isHorizontal)
        {
            rectTransform.sizeDelta = new Vector2(horizontalWidth, horizontalHeight);
            
            // Calculate rotation based on direction
            float angle = firstTile.x < secondTile.x ? 0f : 180f;
            rectTransform.eulerAngles = new Vector3(0, 0, angle);
        }
        else
        {
            rectTransform.sizeDelta = new Vector2(verticalWidth, verticalHeight);
            
            // Calculate rotation based on direction
            float angle = firstTile.y < secondTile.y ? 90f : -90f;
            rectTransform.eulerAngles = new Vector3(0, 0, angle);
        }
    }
    
    /// <summary>
    /// Creates and animates a link object between two tiles
    /// </summary>
    /// <param name="firstTile">First tile in the match</param>
    /// <param name="secondTile">Second tile in the match</param>
    /// <param name="linkPrefab">Prefab to use for the link</param>
    /// <param name="parent">Optional parent transform</param>
    /// <returns>The created link object</returns>
    public static LinkObject CreateLink(Tile firstTile, Tile secondTile, GameObject linkPrefab, Transform parent = null)
    {
        if (linkPrefab == null || firstTile == null || secondTile == null)
        {
            Debug.LogWarning("Cannot create link with null parameters");
            return null;
        }
        
        // Instantiate the link object
        GameObject linkObj = Instantiate(linkPrefab, parent);
        LinkObject link = linkObj.GetComponent<LinkObject>();
        
        if (link == null)
            link = linkObj.AddComponent<LinkObject>();
        
        // Animate the link
        link.AnimateBetweenTiles(firstTile, secondTile).OnComplete(() => {
            // Destroy the link object when animation completes
            Destroy(linkObj);
        });
        
        return link;
    }
} 