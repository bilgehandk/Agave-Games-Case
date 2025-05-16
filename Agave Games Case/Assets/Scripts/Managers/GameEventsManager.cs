using System;

public static class GameEventsManager
{
    public static void OnGameOver(int score)
    {
        GameEvents.OnGameOver(score);
    }
    
    public static void OnGameWin(int score)
    {
        GameEvents.OnGameWin(score);
    }
}
