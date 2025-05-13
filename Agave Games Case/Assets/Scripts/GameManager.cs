using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeGameSystems();
    }

    private void InitializeGameSystems()
    {   
        if (GameUI.Instance == null)
            FindObjectOfType<GameUI>();
    }

    private void Start()
    {
        ValidateGameSystems();
        GameSystemValidator.ValidateAndFix();
    }
    
    private void ValidateGameSystems()
    {
        if (GameState.Instance != null && GameSettings.Instance != null && 
            ScoreCounter.Instance != null && GameUI.Instance != null)
        {
            PerformAdditionalValidation();
        }
    }
    
    private void PerformAdditionalValidation()
    {
        try
        {
            if (GameSettings.Instance.maxMoves <= 0)
                GameSettings.Instance.maxMoves = 10;
            
            GameUI.Instance.UpdateMovesText(GameSettings.Instance.maxMoves);
            GameState.Instance.CheckWinCondition();
        }
        catch (System.Exception)
        {
            // Silent catch to maintain stability
        }
    }
}
