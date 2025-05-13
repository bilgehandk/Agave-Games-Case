using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEditor.PackageManager;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    public static Board Instance { get; private set; }
    
    [SerializeField] private AudioClip collectionSound;

    [SerializeField] private AudioSource audioSource;

    public Row[] rows;
    
    public Tile[,] tiles { get; private set; }

    public int width => rows != null ? rows.Max(row => row.tiles.Length) : 0;

    public int height => rows != null ? rows.Length : 0;

    private const float TweenDuration = 0.25f;

    private List<Tile> _selection = new List<Tile>();

    public void Awake()
    {
        Instance = this;
        DOTween.SetTweensCapacity(1250, 50);
    }
    
    private async void Start()
    {
        await ItemManager.Initialize();
        
        tiles = new Tile[width, height];

        for (var y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
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
    }
    
    void Update()
    {
        if (tiles == null || tiles.GetLength(0) == 0 || tiles.GetLength(1) == 0) return;
    }
    
    public async void Select(Tile tile)
    {
        if (tile == null || _selection.Contains(tile))
            return;

        if (_selection.Count == 0)
        {
            _selection.Add(tile);
            tile.SetButtonColor(Color.cyan);
        }
        else if (_selection.Count == 1)
        {
            if (Array.IndexOf(_selection[0].Neighbors, tile) != -1)
            {
                _selection.Add(tile);
                tile.SetButtonColor(Color.cyan);
            }
            else
            {
                ResetSelections();
                _selection.Add(tile);
                tile.SetButtonColor(Color.cyan);
            }
        }

        if (_selection.Count < 2) return;

        await Swap(_selection[0], _selection[1]);

        if (CanPop())
        {
            ResetSelections();
            await Pop();
        }
        else
        {
            await Swap(_selection[0], _selection[1]);
            ResetSelections();
        }
        
    }
    
    private void ResetSelections()
    {
        if (_selection != null)
        {
            foreach (var tile in _selection)
            {
                if (tile != null)
                {
                    tile.SetButtonColor(Color.white);
                }
            }
            _selection.Clear();
        }
    }
    
    private void ResetAllTiles()
    {
        if (tiles == null) return;
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (x < width && y < height && tiles[x, y] != null)
                {
                    tiles[x, y].SetButtonColor(Color.white);
                }
            }
        }
    }

    public async Task Swap(Tile tile1, Tile tile2)
    {
        var icon1 = tile1.icon;
        var icon2 = tile2.icon;

        var icon1Transform = icon1.transform;
        var icon2Transform = icon2.transform;

        var sequence = DOTween.Sequence();

        sequence.Join(icon1Transform.DOMove(icon2Transform.position, TweenDuration))
            .Join(icon2Transform.DOMove(icon1Transform.position, TweenDuration));

        await sequence.Play().AsyncWaitForCompletion();
    
        icon1Transform.SetParent(tile2.transform);
        icon2Transform.SetParent(tile1.transform);

        tile1.icon = icon2;
        tile2.icon = icon1;

        (tile1.Item, tile2.Item) = (tile2.Item, tile1.Item);
        
        return;
    }

    private bool CanPop()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (x < width && y < height && tiles[x, y] != null && tiles[x, y].GetConnectedTiles().Skip(1).Count() >= 2)
                    return true;
            }
        }

        return false;
    }

    private async Task Pop()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (x >= width || y >= height || tiles[x, y] == null)
                    continue;
                    
                var tile = tiles[x, y];

                var connectedTiles = tile.GetConnectedTiles();

                if (connectedTiles.Skip(1).Count() < 2)
                    continue;

                var deflateSequence = DOTween.Sequence();

                foreach (var connectedTile in connectedTiles)
                {
                    if (connectedTile != null && connectedTile.icon != null)
                    {
                        deflateSequence.Join(connectedTile.icon.transform.DOScale(Vector3.zero, TweenDuration));
                    }
                }
                
                if (ScoreCounter.Instance != null)
                {
                    ScoreCounter.Instance.Score += tile.Item.value * connectedTiles.Count;
                }
                
                if (audioSource != null && collectionSound != null)
                {
                    audioSource.PlayOneShot(collectionSound);
                }

                await deflateSequence.Play().AsyncWaitForCompletion();
                
                var inflateSequence = DOTween.Sequence();

                foreach (var connectedTile in connectedTiles)
                {
                    if (connectedTile != null)
                    {
                        connectedTile.Item = ItemManager.Items[Random.Range(0, ItemManager.Items.Count)];
                        if (connectedTile.icon != null)
                        {
                            inflateSequence.Join(connectedTile.icon.transform.DOScale(Vector3.one, TweenDuration));
                        }
                        connectedTile.SetButtonColor(Color.white);
                    }
                }

                await inflateSequence.Play().AsyncWaitForCompletion();

                x = 0;
                y = 0;
            }
        }
        
        ResetAllTiles();
    }
    
    public void ProcessLink(List<Tile> link)
    {
        int score = link.Count;

        foreach (var tile in link)
        {
            tile.Item = ItemManager.Items[Random.Range(0, ItemManager.Items.Count)];
            tile.icon.transform.DOScale(Vector3.zero, TweenDuration).OnComplete(() =>
            {
                tile.icon.transform.DOScale(Vector3.one, TweenDuration);
                tile.SetButtonColor(Color.white);
            });
        }

        if (ScoreCounter.Instance != null)
        {
            ScoreCounter.Instance.Score += score;
        }
        
        ResetAllTiles();
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
                catch (System.Exception error)
                {
                    Debug.LogError("Tile is null: " + error.Message);
                }
            }
        }
        
        await inflateSequence.Play().AsyncWaitForCompletion();
        
        ResetAllTiles();
    }

    public void KillTweens()
    {
        DOTween.KillAll();
    }
}
