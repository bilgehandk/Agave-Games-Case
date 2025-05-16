using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Manages the visual representation of links between tiles
/// </summary>
public class LinkVisualizer : MonoBehaviour
{
    [Header("Link Settings")]
    [SerializeField] private Transform linkPrefab;
    [SerializeField] private int linkSortingOrder = 200;
    [SerializeField] private Material linkMaterial;
    [SerializeField] private Color linkColor = new Color(0.5f, 0.8f, 1f, 0.8f);
    [SerializeField] private float linkWidth = 0.15f;
    
    [Header("Animation Settings")]
    [SerializeField] private float createDuration = 0.1f;
    [SerializeField] private float removeDuration = 0.1f;
    [SerializeField] private Ease createEase = Ease.OutBack;
    [SerializeField] private Ease removeEase = Ease.InBack;
    
    private Dictionary<string, Transform> activeLinks = new Dictionary<string, Transform>();
    private Transform linkContainer;
    
    private void Awake()
    {
        InitializeLinkContainer();
    }
    
    private void InitializeLinkContainer()
    {
        linkContainer = new GameObject("LinkContainer").transform;
        linkContainer.SetParent(transform);
        linkContainer.localPosition = Vector3.zero;
    }
    
    /// <summary>
    /// Creates a visual link between two tiles
    /// </summary>
    /// <param name="sourceTile">Source tile of the link</param>
    /// <param name="targetTile">Target tile of the link</param>
    public void CreateLink(Tile sourceTile, Tile targetTile)
    {
        if (sourceTile == null || targetTile == null || linkPrefab == null)
        {
            Debug.LogWarning("Cannot create link with null parameters");
            return;
        }
            
        string linkId = GetLinkId(sourceTile, targetTile);
        
        if (activeLinks.ContainsKey(linkId))
            return;
            
        Transform linkInstance = Instantiate(linkPrefab, linkContainer);
        linkInstance.name = $"Link_{linkId}";
        
        ConfigureLinkTransform(linkInstance, sourceTile, targetTile);
        ConfigureLinkRenderer(linkInstance);
        AnimateLinkCreation(linkInstance, sourceTile, targetTile);
        
        activeLinks.Add(linkId, linkInstance);
    }
    
    private void ConfigureLinkTransform(Transform linkInstance, Tile sourceTile, Tile targetTile)
    {
        Vector3 sourcePos = sourceTile.transform.position;
        Vector3 targetPos = targetTile.transform.position;
        
        Vector3 midPoint = (sourcePos + targetPos) / 2f;
        linkInstance.position = midPoint;
        
        Vector3 direction = targetPos - sourcePos;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        linkInstance.rotation = Quaternion.Euler(0, 0, angle);
        
        float distance = Vector3.Distance(sourcePos, targetPos);
        linkInstance.localScale = new Vector3(0, linkWidth, 1f);
    }
    
    private void ConfigureLinkRenderer(Transform linkInstance)
    {
        Renderer renderer = linkInstance.GetComponent<Renderer>();
        if (renderer == null)
            return;
            
        renderer.sortingOrder = linkSortingOrder;
        if (linkMaterial != null)
        {
            renderer.material = linkMaterial;
        }
        
        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(propBlock);
        propBlock.SetColor("_Color", linkColor);
        renderer.SetPropertyBlock(propBlock);
    }
    
    private void AnimateLinkCreation(Transform linkInstance, Tile sourceTile, Tile targetTile)
    {
        float distance = Vector3.Distance(sourceTile.transform.position, targetTile.transform.position);
        linkInstance.DOScaleX(distance, createDuration).SetEase(createEase);
    }
    
    /// <summary>
    /// Removes a visual link between two tiles
    /// </summary>
    /// <param name="sourceTile">Source tile of the link</param>
    /// <param name="targetTile">Target tile of the link</param>
    public void RemoveLink(Tile sourceTile, Tile targetTile)
    {
        if (sourceTile == null || targetTile == null)
            return;
            
        string linkId = GetLinkId(sourceTile, targetTile);
        
        if (activeLinks.TryGetValue(linkId, out Transform linkTransform))
        {
            if (linkTransform != null)
            {
                linkTransform.DOScaleX(0, removeDuration).SetEase(removeEase).OnComplete(() => {
                    Destroy(linkTransform.gameObject);
                });
            }
            
            activeLinks.Remove(linkId);
        }
    }
    
    /// <summary>
    /// Clears all active links
    /// </summary>
    public void ClearAllLinks()
    {
        foreach (var link in activeLinks.Values)
        {
            if (link != null)
            {
                DOTween.Kill(link);
                Destroy(link.gameObject);
            }
        }
        
        activeLinks.Clear();
    }
    
    /// <summary>
    /// Generates a unique ID for a link between two tiles
    /// </summary>
    private string GetLinkId(Tile tile1, Tile tile2)
    {
        int id1 = tile1.GetInstanceID();
        int id2 = tile2.GetInstanceID();
        
        return id1 < id2 ? $"{id1}_{id2}" : $"{id2}_{id1}";
    }
    
    private void OnDestroy()
    {
        ClearAllLinks();
    }
}
