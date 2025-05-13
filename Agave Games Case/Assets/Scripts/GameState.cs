using UnityEngine;

public class GameState : MonoBehaviour
{
    private static GameState _instance;
    
    public static GameState Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject gameStateObj = new GameObject("GameState");
                gameStateObj.AddComponent<GameState>();
            }
            return _instance;
        }
    }

    public enum State
    {
        Playing,
        Win,
        GameOver
    }

    private State currentState = State.Playing;
    public State CurrentState => currentState;
    
    public void ResetState()
    {
        currentState = State.Playing;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void CheckWinCondition()
    {
        if (currentState != State.Playing)
            return;
        
        try
        {
            int currentScore = ScoreCounter.Instance?.Score ?? 0;
            int remainingMoves = GameSettings.Instance?.maxMoves ?? 0;
            int targetScore = GameSettings.Instance?.targetScore ?? 1000;
            
            if (currentScore >= targetScore)
                SetWinState();
            else if (remainingMoves <= 0)
                SetGameOverState();
        }
        catch
        {
            Debug.LogError("Error checking win condition.");
        }
    }

    private void SetWinState()
    {
        currentState = State.Win;
        int finalScore = ScoreCounter.Instance?.Score ?? 0;
        
        if (GameUI.Instance != null)
        {
            GameUI.Instance.ShowWin(finalScore);
            
            if (GameUI.Instance.CurrentScreenType != GameScreenType.EndScreen)
                GameUI.Instance.SwitchScreen(GameScreenType.EndScreen);
        }
    }

    private void SetGameOverState()
    {
        currentState = State.GameOver;
        
        if (GameUI.Instance != null && ScoreCounter.Instance != null)
            GameUI.Instance.ShowGameOver(ScoreCounter.Instance.Score);
    }
}
