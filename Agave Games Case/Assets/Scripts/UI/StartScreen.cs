using UnityEngine;
using UnityEngine.UI;

public class StartScreen : UIScreen
{
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject exitButton;
    
    protected override void Awake()
    {
        base.Awake();
        ShowButtons();
    }
    
    protected override void OnEnable()
    {
        base.OnEnable();
        SetupButtonListeners();
        ResetGameData();
    }
    
    private void ResetGameData()
    {
        GameSettings.Instance?.ResetMoves();
        if (ScoreCounter.Instance != null)
        {
            ScoreCounter.Instance.Score = 0;
        }
    }
    
    private void SetupButtonListeners()
    {
        GetButtonComponent(startButton)?.onClick.AddListener(OnStartButtonClicked);
        GetButtonComponent(exitButton)?.onClick.AddListener(OnExitButtonClicked);
    }
    
    private Button GetButtonComponent(GameObject buttonObj)
    {
        if (buttonObj == null) return null;
        
        Button button = buttonObj.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
        }
        return button;
    }
    
    private void OnStartButtonClicked()
    {
        GameSettings.Instance?.ResetMoves();
        if (ScoreCounter.Instance != null)
        {
            ScoreCounter.Instance.Score = 0;
        }
        GameState.Instance?.ResetState();
        
        GameEvents.OnScreenChange(GameScreenType.GameScreen);
    }
    
    private void OnExitButtonClicked()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    public override void Show()
    {
        base.Show();
        ShowButtons();
    }
    
    public override void Hide()
    {
        base.Hide();
        HideButtons();
    }
    
    private void ShowButtons()
    {
        startButton?.SetActive(true);
        exitButton?.SetActive(true);
    }
    
    private void HideButtons()
    {
        startButton?.SetActive(false);
        exitButton?.SetActive(false);
    }
}