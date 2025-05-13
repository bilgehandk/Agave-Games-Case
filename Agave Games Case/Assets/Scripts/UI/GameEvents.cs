using System;

public class GameEventsLegacy
{
    public static bool IsGameWin => GameEvents.IsGameWin;
    
    public static void OnGameOver(int score)
    {
        GameEvents.OnGameOver(score);
    }
    
    public static void OnGameWin(int score)
    {
        GameEvents.OnGameWin(score);
    }
    
    public static void OnScreenChange(GameScreenType screenType)
    {
        GameEvents.OnScreenChange(screenType);
    }
}
