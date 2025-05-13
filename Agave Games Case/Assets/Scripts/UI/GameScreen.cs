using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class GameScreen : UIScreen
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private Button resetButton;
    
    private const int DEFAULT_MOVES = 25;
    private const float UPDATE_INTERVAL = 0.5f;

    protected override void Awake()
    {
        base.Awake();
        InitializeTextComponents();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        InitializeTextComponents();
        resetButton.onClick.AddListener(ResetGame);
        StartCoroutine(ScoreUpdateRoutine());
        StartCoroutine(MovesUpdateRoutine());
    }
    
    protected override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
        resetButton?.onClick.RemoveListener(ResetGame);
    }
    
    private System.Collections.IEnumerator ScoreUpdateRoutine()
    {
        while (true)
        {
            if (ScoreCounter.Instance != null)
            {
                UpdateScoreText(ScoreCounter.Instance.Score);
            }
            yield return new WaitForSeconds(UPDATE_INTERVAL);
        }
    }
    
    private System.Collections.IEnumerator MovesUpdateRoutine()
    {
        while (true)
        {
            if (GameSettings.Instance != null)
            {
                UpdateMovesText(GameSettings.Instance.maxMoves);
            }
            yield return new WaitForSeconds(UPDATE_INTERVAL);
        }
    }

    private void InitializeTextComponents()
    {
        if (scoreText != null && movesText != null) return;
        
        var textComponents = GetComponentsInChildren<TextMeshProUGUI>(true);
        
        foreach (var text in textComponents)
        {
            if (text == null) continue;
            
            string textName = text.name.ToLower();
            string textContent = text.text.ToLower();
            
            if (scoreText == null && (textName.Contains("score") || textContent.Contains("score")))
            {
                scoreText = text;
            }
            else if (movesText == null && (textName.Contains("move") || textContent.Contains("move")))
            {
                movesText = text;
            }
        }
        
        if (scoreText == null || movesText == null)
        {
            System.Array.Sort(textComponents, (a, b) => 
            {
                if (a == null || b == null) return 0;
                return b.transform.position.y.CompareTo(a.transform.position.y);
            });
            
            if (scoreText == null && textComponents.Length > 0)
            {
                scoreText = textComponents[0];
            }
            
            if (movesText == null && textComponents.Length > 1)
            {
                movesText = textComponents[1];
            }
        }
        
        CreateScoreTextIfNeeded();
        CreateMovesTextIfNeeded();
    }
    
    private void CreateScoreTextIfNeeded()
    {
        if (scoreText != null) return;
        
        GameObject scoreTextObj = new GameObject("ScoreText");
        scoreTextObj.transform.SetParent(transform, false);
        
        RectTransform rectTransform = scoreTextObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.pivot = new Vector2(0, 1);
        rectTransform.anchoredPosition = new Vector2(10, -10);
        rectTransform.sizeDelta = new Vector2(200, 30);
        
        scoreText = scoreTextObj.AddComponent<TextMeshProUGUI>();
        scoreText.text = "Score: 0";
        scoreText.fontSize = 24;
        scoreText.alignment = TextAlignmentOptions.Left;
        scoreText.color = Color.white;
    }
    
    private void CreateMovesTextIfNeeded()
    {
        if (movesText != null) return;
        
        GameObject movesTextObj = new GameObject("MovesText");
        movesTextObj.transform.SetParent(transform, false);
        
        RectTransform rectTransform = movesTextObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(1, 1);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(1, 1);
        rectTransform.anchoredPosition = new Vector2(-10, -10);
        rectTransform.sizeDelta = new Vector2(200, 30);
        
        movesText = movesTextObj.AddComponent<TextMeshProUGUI>();
        movesText.text = "Moves: " + DEFAULT_MOVES;
        movesText.fontSize = 24;
        movesText.alignment = TextAlignmentOptions.Right;
        movesText.color = Color.white;
        
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.maxMoves = DEFAULT_MOVES;
            UpdateMovesText(DEFAULT_MOVES);
        }
    }


    private void ResetGame()
    {
        ResetScore();
        ResetMoves();
        
        if (Board.Instance != null)
        {
            ResetBoard();
        }
    }
    
    private void ResetScore()
    {
        if (ScoreCounter.Instance != null)
        {
            ScoreCounter.Instance.Score = 0;
            UpdateScoreText(0);
        }
    }
    
    private void ResetMoves()
    {
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.maxMoves = DEFAULT_MOVES;
            UpdateMovesText(DEFAULT_MOVES);
        }
    }
    
    private async void ResetBoard()
    {
        if (Board.Instance == null || Board.Instance.width <= 0 || 
            Board.Instance.height <= 0 || Board.Instance.tiles == null)
        {
            return;
        }
        
        if (ItemManager.Items == null || ItemManager.Items.Count == 0)
        {
            await ItemManager.Initialize();
            if (ItemManager.Items == null || ItemManager.Items.Count == 0) return;
        }
        
        try
        {
            DOTween.KillAll();
            
            List<Tile> validTiles = new List<Tile>();
            List<Item> newItems = new List<Item>();
            
            PrepareNewTiles(Board.Instance.width, Board.Instance.height, validTiles, newItems);
            ResetTileAnimations(validTiles);
            AnimateTileChanges(validTiles, newItems);
        }
        catch
        {
            FallbackBoardReset(Board.Instance.width, Board.Instance.height);
        }
    }
    
    private void PrepareNewTiles(int width, int height, List<Tile> validTiles, List<Item> newItems)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Tile tile = Board.Instance.tiles[x, y];
                if (tile == null || tile.icon == null || tile.icon.transform == null) continue;
                
                tile.IsSelected = false;
                tile.SetButtonColor(Color.white);
                
                int randomIndex = UnityEngine.Random.Range(0, ItemManager.Items.Count);
                Item randomItem = ItemManager.Items[randomIndex];
                
                if (randomItem != null)
                {
                    validTiles.Add(tile);
                    newItems.Add(randomItem);
                }
            }
        }
    }
    
    private void ResetTileAnimations(List<Tile> tiles)
    {
        foreach (Tile tile in tiles)
        {
            if (tile.icon != null && tile.icon.transform != null)
            {
                DOTween.Kill(tile.icon.transform);
                tile.icon.transform.localScale = Vector3.one;
            }
        }
    }
    
    private void AnimateTileChanges(List<Tile> tiles, List<Item> newItems)
    {
        Sequence animationSequence = DOTween.Sequence();
        Sequence shrinkSequence = DOTween.Sequence();
        
        foreach (Tile tile in tiles)
        {
            if (tile.icon != null && tile.icon.transform != null)
            {
                shrinkSequence.Join(tile.icon.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack));
            }
        }
        
        animationSequence.Append(shrinkSequence);
        
        animationSequence.AppendCallback(() => {
            for (int i = 0; i < tiles.Count; i++)
            {
                if (i < newItems.Count && tiles[i] != null)
                {
                    tiles[i].Item = newItems[i];
                }
            }
        });
        
        Sequence growSequence = DOTween.Sequence();
        
        foreach (Tile tile in tiles)
        {
            if (tile.icon != null && tile.icon.transform != null)
            {
                growSequence.Join(tile.icon.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack));
            }
        }
        
        animationSequence.Append(growSequence);
        animationSequence.SetUpdate(true);
        animationSequence.SetAutoKill(true);
        animationSequence.Play();
    }
    
    private void FallbackBoardReset(int width, int height)
    {
        if (ItemManager.Items == null || ItemManager.Items.Count == 0) return;
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Tile tile = Board.Instance.tiles[x, y];
                if (tile == null) continue;
                
                tile.IsSelected = false;
                tile.SetButtonColor(Color.white);
                
                int randomIndex = UnityEngine.Random.Range(0, ItemManager.Items.Count);
                if (randomIndex < ItemManager.Items.Count)
                {
                    tile.Item = ItemManager.Items[randomIndex];
                    
                    if (tile.icon != null && tile.icon.transform != null)
                    {
                        tile.icon.transform.localScale = Vector3.one;
                    }
                }
            }
        }
    }

    public void UpdateMovesText(int movesLeft)
    {
        if (movesText == null)
        {
            InitializeTextComponents();
        }
        
        if (movesText != null)
        {
            movesText.text = "Moves: " + movesLeft;
            EnsureTextIsVisible(movesText);
            Canvas.ForceUpdateCanvases();
        }
        else
        {
            TryFindAlternativeTextForMoves(movesLeft);
        }
    }

    public void UpdateScoreText(int score)
    {
        if (scoreText == null)
        {
            InitializeTextComponents();
        }
        
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
            EnsureTextIsVisible(scoreText);
        }
        else
        {
            TryFindAlternativeTextForScore(score);
        }
    }
    
    private void EnsureTextIsVisible(TextMeshProUGUI text)
    {
        if (text != null && !text.gameObject.activeInHierarchy)
        {
            text.gameObject.SetActive(true);
        }
    }
    
    private void TryFindAlternativeTextForMoves(int movesLeft)
    {
        var texts = GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (var text in texts)
        {
            if (text != scoreText)
            {
                movesText = text;
                movesText.text = "Moves: " + movesLeft;
                movesText.gameObject.SetActive(true);
                break;
            }
        }
    }
    
    private void TryFindAlternativeTextForScore(int score)
    {
        var texts = GetComponentsInChildren<TextMeshProUGUI>(true);
        if (texts.Length > 0)
        {
            scoreText = texts[0];
            scoreText.text = "Score: " + score;
            scoreText.gameObject.SetActive(true);
        }
    }
    
    protected override void OnScreenShown()
    {
        base.OnScreenShown();
        InitializeTextComponents();
        
        if (GameSettings.Instance != null)
        {
            UpdateMovesText(GameSettings.Instance.maxMoves);
            
            if (ScoreCounter.Instance != null)
            {
                UpdateScoreText(ScoreCounter.Instance.Score);
            }
        }
    }
    
    public void NavigateToEndScreen()
    {
        GameEvents.OnScreenChange(GameScreenType.EndScreen);
    }
    
    public void NavigateToStartScreen()
    {
        GameEvents.OnScreenChange(GameScreenType.StartScreen);
    }
    
    private void Update()
    {
        if (!gameObject.activeInHierarchy) return;
        
        UpdateTextVisibility();
    }
    
    private void UpdateTextVisibility()
    {
        if (scoreText != null && !scoreText.gameObject.activeInHierarchy)
        {
            scoreText.gameObject.SetActive(true);
            if (ScoreCounter.Instance != null)
            {
                scoreText.text = "Score: " + ScoreCounter.Instance.Score;
            }
        }
        
        if (movesText != null && !movesText.gameObject.activeInHierarchy)
        {
            movesText.gameObject.SetActive(true);
            if (GameSettings.Instance != null)
            {
                movesText.text = "Moves: " + GameSettings.Instance.maxMoves;
            }
        }
        
        if (scoreText == null || movesText == null)
        {
            InitializeTextComponents();
            
            if (scoreText != null && ScoreCounter.Instance != null)
            {
                scoreText.text = "Score: " + ScoreCounter.Instance.Score;
            }
            
            if (movesText != null && GameSettings.Instance != null)
            {
                movesText.text = "Moves: " + GameSettings.Instance.maxMoves;
            }
        }
    }
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (scoreText == null || movesText == null)
        {
            var textComponents = GetComponentsInChildren<TextMeshProUGUI>(true);
            if (textComponents.Length > 0)
            {
                foreach (var text in textComponents)
                {
                    if (text.name.ToLower().Contains("score") && scoreText == null)
                    {
                        scoreText = text;
                    }
                    if (text.name.ToLower().Contains("move") && movesText == null)
                    {
                        movesText = text;
                    }
                }
            }
        }
    }
#endif
}
