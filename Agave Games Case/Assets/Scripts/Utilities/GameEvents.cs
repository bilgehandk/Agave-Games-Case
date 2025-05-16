using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action<GameScreenType> ScreenChange;
    public static event Action<int> GameOver;
    public static event Action<int> GameWin;
    public static event Action BeforeScreenChange;
    
    public static bool IsGameWin { get; private set; }

    public static void OnScreenChange(GameScreenType screenType)
    {
        BeforeScreenChange?.Invoke();
        
        if (DG.Tweening.DOTween.instance != null)
        {
            DG.Tweening.DOTween.KillAll(true);
            DG.Tweening.DOTween.Clear(true);
        }
        
        ScreenChange?.Invoke(screenType);
    }

    public static void OnGameOver(int score)
    {
        IsGameWin = false;
        GameOver?.Invoke(score);
    }

    public static void OnGameWin(int score)
    {
        IsGameWin = true;
        GameWin?.Invoke(score);
    }
}
