using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : UIScreen
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI lostText;
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject mainMenuButton;

    protected override void Awake()
    {
        base.Awake();
        SetButtonsVisibility(false);
        SetTextElementsVisibility(false);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SetupButtonListeners();
        
        InitializeTweenManager();
        CleanupTweens();
    }

    private void InitializeTweenManager()
    {
        if (GlobalTweenManager.Instance == null)
        {
            GameObject tweenManagerObj = new GameObject("GlobalTweenManager");
            tweenManagerObj.AddComponent<GlobalTweenManager>();
            DontDestroyOnLoad(tweenManagerObj);
        }
    }

    private void CleanupTweens()
    {
        DOTween.KillAll(true);
        DOTween.Clear(true);
    }

    private void SetupButtonListeners()
    {
        if (restartButton) restartButton.GetComponent<Button>()?.onClick.AddListener(OnRestartButtonClicked);
        if (mainMenuButton) mainMenuButton.GetComponent<Button>()?.onClick.AddListener(OnMainMenuButtonClicked);
    }

    protected override void OnScreenShown()
    {
        base.OnScreenShown();
        DisplayScore();
        ShowGameResult();
        SetButtonsVisibility(true);
    }

    public void OnRestartButtonClicked()
    {
        CleanupTweens();
        SafelyResetBoard();
        ResetGameState();
        StartCoroutine(DelayedScreenChange(GameScreenType.GameScreen, 0.15f));
    }
    
    private void SafelyResetBoard()
    {
        if (Board.Instance == null || Board.Instance.gameObject == null) return;
        
        Board.Instance.KillTweens();
        var audioSource = Board.Instance.GetComponent<AudioSource>();
        if (audioSource != null && audioSource.gameObject != null && !audioSource.enabled)
        {
            audioSource.gameObject.SetActive(true);
            audioSource.enabled = true;
        }
    }
    
    private void ResetGameState()
    {
        GameSettings.Instance?.ResetMoves();
        if (ScoreCounter.Instance != null) ScoreCounter.Instance.Score = 0;
        GameState.Instance?.ResetState();
    }

    private System.Collections.IEnumerator DelayedScreenChange(GameScreenType screenType, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        CleanupTweens();
        yield return null;
        yield return null; // Two frames for safety
        
        GameEvents.OnScreenChange(screenType);
    }

    public void OnMainMenuButtonClicked()
    {
        DOTween.KillAll(true);
        ResetGameState();
        StartCoroutine(DelayedScreenChange(GameScreenType.StartScreen, 0.05f));
    }

    public void PreInitialize(int finalScore, bool isWin)
    {
        if (scoreText) scoreText.text = $"Final Score: {finalScore}";
        if (winText) winText.gameObject.SetActive(isWin);
        if (lostText) lostText.gameObject.SetActive(!isWin);
        SetButtonsVisibility(true);
    }

    public override void Show()
    {
        base.Show();
        SetTextElementsVisibility(true);
        SetButtonsVisibility(true);
        DisplayScore();
        ShowGameResult();
    }

    public override void Hide()
    {
        base.Hide();
        SetButtonsVisibility(false);
        SetTextElementsVisibility(false);
    }

    private void SetButtonsVisibility(bool isVisible)
    {
        if (restartButton) restartButton.SetActive(isVisible);
        if (mainMenuButton) mainMenuButton.SetActive(isVisible);
    }

    private void SetTextElementsVisibility(bool isVisible)
    {
        if (lostText) lostText.gameObject.SetActive(isVisible && !GameEvents.IsGameWin);
        if (winText) winText.gameObject.SetActive(isVisible && GameEvents.IsGameWin);
        if (scoreText) scoreText.gameObject.SetActive(isVisible);
    }

    private void DisplayScore()
    {
        if (scoreText) scoreText.text = "Score: " + GetCurrentScore();
    }

    private int GetCurrentScore()
    {
        return ScoreCounter.Instance ? ScoreCounter.Instance.Score : 0;
    }

    private void ShowGameResult()
    {
        if (winText) winText.gameObject.SetActive(GameEvents.IsGameWin);
        if (lostText) lostText.gameObject.SetActive(!GameEvents.IsGameWin);
    }
}
