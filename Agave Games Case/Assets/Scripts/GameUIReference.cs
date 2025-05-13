using UnityEngine;

public static class GameUIReference
{
    private static GameUI _instance;
    
    public static GameUI GetInstance()
    {
        if (_instance == null)
        {
            _instance = GameObject.FindObjectOfType<GameUI>();
        }
        return _instance;
    }
    
    public static void ShowGameOverScreen(int score)
    {
        var ui = GetInstance();
        if (ui != null)
        {
            ui.ShowGameOver(score);
        }
    }
    
    public static void ShowWinScreen(int score)
    {
        var ui = GetInstance();
        if (ui != null)
        {
            ui.ShowWin(score);
        }
    }
    
    public static void UpdateMovesText(int moves)
    {
        var ui = GetInstance();
        if (ui != null)
        {
            ui.UpdateMovesText(moves);
        }
    }
}
