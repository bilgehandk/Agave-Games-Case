using UnityEngine;

public static class GameSystemValidator
{
    private static bool _hasRun = false;
    
    public static void ValidateAndFix()
    {
        if (_hasRun)
            return;
            
        _hasRun = true;
        
        var settings = GameSettings.Instance;
        var gameUI = GameUI.Instance;

        if (gameUI != null)
            gameUI.UpdateMovesText(settings != null ? settings.maxMoves : 10);

        if (settings != null)
        {
            if (settings.maxMoves <= 0)
                settings.maxMoves = 10;
            
            if (settings.targetScore <= 0)
                settings.targetScore = 100;
        }
    }
}
