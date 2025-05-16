using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    public static Board Instance { get; private set; }
    
    [SerializeField] private AudioClip collectionSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject linkPrefab;
    [SerializeField] private Transform linkContainer;

    public Row[] rows;
    
    public Tile[,] tiles { get; private set; }

    public int Width => rows != null ? rows.Max(row => row.tiles.Length) : 0;
    public int Height => rows != null ? rows.Length : 0;

    private const float TweenDuration = 0.25f;

    private List<Tile> _draggedTiles = new List<Tile>();
    private List<GameObject> _linkObjects = new List<GameObject>();
    
    private Transform _boardContainer;

    public void Awake()
    {
        Instance = this;
        DOTween.SetTweensCapacity(1250, 50);
        
        _boardContainer = transform.parent;
    }
    
    private async void Start()
    {
        await ItemManager.Initialize();
        
        tiles = new Tile[Width, Height];

        for (var y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (rows != null && y < rows.Length && x < rows[y].tiles.Length)
                {
                    var tile = rows[y].tiles[x];
                    
                    if (tile != null)
                    {
                        tile.x = x;
                        tile.y = y;
                        
                        if (ItemManager.Items != null && ItemManager.Items.Count > 0)
                        {
                            tile.Item = ItemManager.Items[Random.Range(0, ItemManager.Items.Count)];
                        }
                        
                        tiles[x, y] = tile;
                        tile.SetButtonColor(Color.white);
                    }
                }
            }
        }
        
        SetupLinkContainer();
    }
    
    private void SetupLinkContainer()
    {
        if (linkContainer == null && _boardContainer != null)
        {
            Transform existingContainer = _boardContainer.Find("LinkContainer");
            
            if (existingContainer != null)
            {
                linkContainer = existingContainer;
            }
            else
            {
                GameObject newContainer = new GameObject("LinkContainer");
                linkContainer = newContainer.transform;
                linkContainer.SetParent(_boardContainer);
                linkContainer.localPosition = Vector3.zero;
                linkContainer.localScale = Vector3.one;
            }
        }
    }
    
    void Update()
    {
        if (tiles == null || tiles.GetLength(0) == 0 || tiles.GetLength(1) == 0) return;
    }
    
    public void StartDrag(Tile tile)
    {
        if (tile == null) return;
        
        ResetDraggedTiles();
        _draggedTiles.Add(tile);
        tile.SetButtonColor(Color.cyan);
    }
    
    public void DragToTile(Tile tile)
    {
        if (tile == null || _draggedTiles.Contains(tile)) return;
        
        if (_draggedTiles.Count > 0)
        {
            Tile lastTile = _draggedTiles[_draggedTiles.Count - 1];
            
            if (Array.IndexOf(lastTile.Neighbors, tile) != -1 && tile.Item.Equals(lastTile.Item))
            {
                _draggedTiles.Add(tile);
                tile.SetButtonColor(Color.cyan);
                
                CreateLink(lastTile, tile);
            }
        }
    }
    
    private void CreateLink(Tile tileA, Tile tileB)
    {
        if (linkPrefab == null)
            return;
        
        if (linkContainer == null)
        {
            GameObject container = new GameObject("LinkContainer");
            container.transform.SetParent(_boardContainer ? _boardContainer : transform.parent, false);
            container.transform.localPosition = Vector3.zero;
            linkContainer = container.transform;
        }
        
        Vector3 startPos = tileA.icon ? tileA.icon.transform.position : tileA.transform.position;
        Vector3 endPos = tileB.icon ? tileB.icon.transform.position : tileB.transform.position;
        
        Vector3 centerPos = (startPos + endPos) / 2f;
        
        Vector3 direction = endPos - startPos;
        bool isHorizontal = Mathf.Abs(direction.x) > Mathf.Abs(direction.y);
        
        GameObject linkObject = Instantiate(linkPrefab, centerPos, Quaternion.identity, linkContainer);
        _linkObjects.Add(linkObject);
        
        if (isHorizontal)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            linkObject.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else 
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            if (direction.y > 0)
            {
                linkObject.transform.rotation = Quaternion.Euler(0, 0, 90);
            }
            else
            {
                linkObject.transform.rotation = Quaternion.Euler(0, 0, -90);
            }
            
            Vector3 currentScale = linkObject.transform.localScale;
            linkObject.transform.localScale = new Vector3(currentScale.y, currentScale.x, currentScale.z);
        }
        
        SpriteRenderer spriteRenderer = linkObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.cyan;
            spriteRenderer.sortingOrder = 10;
        }
    }
    
    private void ClearLinks()
    {
        foreach (GameObject linkObject in _linkObjects)
        {
            if (linkObject != null)
            {
                Destroy(linkObject);
            }
        }
        
        _linkObjects.Clear();
    }
    
    public async void EndDrag()
    {
        if (_draggedTiles.Count >= 3)
        {
            if (ScoreCounter.Instance != null)
            {
                int value = _draggedTiles[0].Item.value;
                ScoreCounter.Instance.Score += value * _draggedTiles.Count;
            }
            
            if (audioSource != null && collectionSound != null)
            {
                audioSource.PlayOneShot(collectionSound);
            }
            
            await PopLinkedTiles(_draggedTiles);
        }
        else
        {
            ResetDraggedTiles();
        }
        
        ClearLinks();
    }
    
    private void ResetDraggedTiles()
    {
        if (_draggedTiles != null)
        {
            foreach (var tile in _draggedTiles)
            {
                if (tile != null)
                {
                    tile.SetButtonColor(Color.white);
                }
            }
            _draggedTiles.Clear();
        }
        
        ClearLinks();
    }
    
    private void ResetAllTiles()
    {
        if (tiles == null) return;
        
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (x < Width && y < Height && tiles[x, y] != null)
                {
                    tiles[x, y].SetButtonColor(Color.white);
                }
            }
        }
    }

    public async Task PopLinkedTiles(List<Tile> tilesToClear)
    {
        if (tilesToClear == null || tilesToClear.Count == 0)
            return;
            
        var deflateSequence = DOTween.Sequence();
        
        foreach (var tile in tilesToClear)
        {
            if (tile != null && tile.icon != null)
            {
                deflateSequence.Join(tile.icon.transform.DOScale(Vector3.zero, TweenDuration));
            }
        }
        
        await deflateSequence.Play().AsyncWaitForCompletion();
        
        var inflateSequence = DOTween.Sequence();
        
        foreach (var tile in tilesToClear)
        {
            if (tile != null && tile.icon != null && ItemManager.Items != null && ItemManager.Items.Count > 0)
            {
                try
                {
                    tile.Item = ItemManager.Items[Random.Range(0, ItemManager.Items.Count)];
                    inflateSequence.Join(tile.icon.transform.DOScale(Vector3.one, TweenDuration));
                    tile.SetButtonColor(Color.white);
                }
                catch (System.Exception)
                {
                }
            }
        }
        
        await inflateSequence.Play().AsyncWaitForCompletion();
        
        ResetAllTiles();
        ResetDraggedTiles();
    }

    public void KillTweens()
    {
        DOTween.KillAll();
    }
}
