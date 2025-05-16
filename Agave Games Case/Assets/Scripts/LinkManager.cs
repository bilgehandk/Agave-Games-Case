using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class LinkManager : MonoBehaviour
{
    public static LinkManager Instance { get; private set; }

    [SerializeField] private AudioClip linkSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private int sortingOrder = 100;

    private List<Tile> currentChain = new List<Tile>();
    private bool isDragging = false;
    private Camera mainCamera;
    private Vector2 currentMousePosition;
    private List<Tile> linkedTiles = new List<Tile>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        GameEvents.BeforeScreenChange += CleanupAllTweens;
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Board.Instance == null || Board.Instance.tiles == null) 
            return;

        if (Input.GetMouseButtonDown(0))
            BeginDrag();
        else if (Input.GetMouseButtonUp(0))
            EndDrag();
        else if (isDragging)
            ContinueDrag();
    }

    private void BeginDrag()
    {
        Tile hitTile = GetTileUnderPosition(Input.mousePosition);

        if (hitTile != null)
        {
            isDragging = true;
            ClearChain();
            AddToChain(hitTile);
        }
    }

    private void ContinueDrag()
    {
        Vector2 screenPos = Input.mousePosition;
        currentMousePosition = screenPos;
        Tile hitTile = GetTileUnderPosition(screenPos);
        
        if (hitTile != null)
            TryAddToChain(hitTile);
    }

    private void EndDrag()
    {
        isDragging = false;
        Color highlightColor = new Color(0.5f, 0.8f, 1f);
        
        foreach (var tile in currentChain)
        {
            if (tile != null)
            {
                tile.SetButtonColor(highlightColor);
                tile.IsSelected = true;
            }
        }
        
        if (currentChain.Count > 0)
        {
            Tile lastTile = currentChain[currentChain.Count - 1];
            if (lastTile != null)
            {
                lastTile.SetButtonColor(highlightColor);
                lastTile.IsSelected = true;
                
                if (lastTile.icon != null && lastTile.icon.transform != null)
                {
                    string tweenID = "lastTilePulse_" + lastTile.GetInstanceID();
                    var sequence = DOTween.Sequence().SetId(tweenID);
                    sequence.SetAutoKill(true);
                    sequence.SetTarget(lastTile.icon.transform);
                    sequence.Append(lastTile.icon.transform.DOScale(1.2f, 0.1f));
                    sequence.Append(lastTile.icon.transform.DOScale(1f, 0.1f));
                    sequence.Play();
                }
            }
        }
        
        if (currentChain.Count >= 3)
            ConfirmChain();
        else
            ClearChain(true);
    }

    private Tile GetTileUnderPosition(Vector2 screenPosition)
    {
        if (EventSystem.current != null)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = screenPosition;
            
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            
            foreach (var result in results)
            {
                Tile tile = result.gameObject.GetComponent<Tile>();
                if (tile != null)
                    return tile;
            }
        }

        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null)
        {
            Tile tile = hit.collider.GetComponent<Tile>();
            if (tile != null)
                return tile;
        }

        if (Board.Instance != null && Board.Instance.tiles != null)
        {
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 10f));
            
            for (int x = 0; x < Board.Instance.Width; x++)
            {
                for (int y = 0; y < Board.Instance.Height; y++)
                {
                    Tile tile = Board.Instance.tiles[x, y];
                    if (tile == null) continue;
                    
                    RectTransform rectTransform = tile.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        if (RectTransformUtility.RectangleContainsScreenPoint(
                            rectTransform, screenPosition, mainCamera))
                            return tile;
                    }
                    else
                    {
                        Renderer renderer = tile.GetComponent<Renderer>();
                        if (renderer != null && renderer.bounds.Contains(worldPos))
                            return tile;
                    }
                }
            }
        }

        return null;
    }

    private void TryAddToChain(Tile tile)
    {
        if (!isDragging || currentChain.Contains(tile))
            return;

        if (currentChain.Count > 0)
        {
            Tile lastTile = currentChain[currentChain.Count - 1];
            
            if (lastTile.Item != tile.Item)
                return;
                
            bool isNeighbor = false;
            foreach (var neighbor in lastTile.Neighbors)
            {
                if (neighbor == tile)
                {
                    isNeighbor = true;
                    break;
                }
            }
            
            if (!isNeighbor)
                return;
        }
        
        AddToChain(tile);
        
        Color highlightColor = new Color(0.5f, 0.8f, 1f);
        
        foreach (var chainTile in currentChain)
        {
            if (chainTile != null)
            {
                chainTile.SetButtonColor(highlightColor);
                chainTile.IsSelected = true;
            }
        }
    }

    private void AddToChain(Tile tile)
    {
        currentChain.Add(tile);
        linkedTiles.Add(tile);
        
        tile.SetButtonColor(new Color(0.5f, 0.8f, 1f));
        tile.IsSelected = true;
        
        if (audioSource != null && linkSound != null)
            audioSource.PlayOneShot(linkSound);
        
        if (currentChain.Count > 1)
        {
            Tile prevTile = currentChain[currentChain.Count - 2];
            
            if (tile.icon != null)
            {
                Color originalColor = tile.icon.color;
                tile.icon.color = Color.white;
                DOTween.To(() => tile.icon.color, x => tile.icon.color = x, originalColor, 0.2f);
            }
            
            if (prevTile.icon != null)
            {
                Color originalColor = prevTile.icon.color;
                prevTile.icon.color = Color.white;
                DOTween.To(() => prevTile.icon.color, x => prevTile.icon.color = x, originalColor, 0.2f);
            }
        }
    }

    private void ClearChain(bool resetColors = false)
    {
        if (resetColors)
        {
            foreach (var tile in currentChain)
            {
                if (tile != null)
                {
                    tile.SetButtonColor(Color.white);
                    tile.IsSelected = false;
                }
            }
        }
        
        currentChain.Clear();
        linkedTiles.Clear();
    }

    private async void ConfirmChain()
    {
        if (currentChain.Count < 3)
            return;

        int scoreValue = currentChain.Count * currentChain[0].Item.value;
        
        if (ScoreCounter.Instance != null)
            ScoreCounter.Instance.Score += scoreValue;
        
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.maxMoves--;
            
            if (GameUI.Instance != null)
                GameUI.Instance.UpdateMovesText(GameSettings.Instance.maxMoves);
            
            GameState.Instance?.CheckWinCondition();
        }
        
        List<Tile> tilesToClear = new List<Tile>(currentChain);
        
        foreach (var tile in tilesToClear)
        {
            if (tile.icon != null && tile.icon.transform != null)
            {
                string tweenID = "tilePop_" + tile.GetInstanceID();
                
                Color originalColor = tile.icon.color;
                tile.icon.color = Color.white;
                
                var sequence = DOTween.Sequence().SetId(tweenID);
                sequence.SetAutoKill(true);
                sequence.SetTarget(tile.icon.transform);
                sequence.Append(tile.icon.transform.DOScale(1.2f, 0.1f));
                sequence.Append(tile.icon.transform.DOScale(0.8f, 0.1f));
                sequence.Play();
            }
        }
        
        if (tilesToClear.Count > 0 && tilesToClear[tilesToClear.Count - 1] != null)
        {
            try
            {
                ShowFloatingScoreText(tilesToClear[tilesToClear.Count - 1].transform.position, scoreValue);
            }
            catch (System.Exception)
            {
            }
        }
        
        try
        {
            if (Board.Instance != null)
                await Board.Instance.PopLinkedTiles(tilesToClear);
        }
        catch (System.Exception)
        {
        }
        
        ClearChain();
    }
    
    private void ShowFloatingScoreText(Vector3 position, int score)
    {
        GameObject scoreTextObj = new GameObject("ScoreText");
        scoreTextObj.transform.position = position;
        
        Canvas canvas = scoreTextObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = sortingOrder;
        
        scoreTextObj.AddComponent<CanvasScaler>().dynamicPixelsPerUnit = 100;
        
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(scoreTextObj.transform, false);
        
        Text scoreText = textObj.AddComponent<Text>();
        scoreText.text = "+" + score.ToString();
        scoreText.color = Color.yellow;
        
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null)
            font = Font.CreateDynamicFontFromOSFont("Arial", 14);
        
        scoreText.font = font;
        scoreText.fontSize = 30;
        scoreText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform rectTransform = scoreText.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(100, 50);
        
        DOTween.Sequence()
            .Append(scoreTextObj.transform.DOMoveY(scoreTextObj.transform.position.y + 100f, 1f))
            .Join(scoreText.DOFade(0, 1f))
            .OnComplete(() => Destroy(scoreTextObj));
    }

    private void CleanupAllTweens()
    {
        DOTween.Kill(this);
        DOTween.Kill(transform);
        
        ClearChain(true);
        
        if (linkedTiles != null)
        {
            foreach (var tile in linkedTiles)
            {
                if (tile != null)
                {
                    DOTween.Kill(tile);
                    if (tile.icon != null && tile.icon.transform != null)
                    {
                        DOTween.Kill(tile.icon);
                        DOTween.Kill(tile.icon.transform);
                    }
                }
            }
        }
    }
    
    private void OnDestroy()
    {
        GameEvents.BeforeScreenChange -= CleanupAllTweens;
        DOTween.Kill(this);
    }
}
