using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public int width = 8;
    public int height = 8;
    public int maxMoves = 25;
    public int targetScore = 1000;
    
    private int initialMaxMoves;

    private static GameSettings _instance;
    public static GameSettings Instance 
    { 
        get 
        {
            if (_instance == null)
            {
                GameObject gameSettingsObj = new GameObject("GameSettings");
                _instance = gameSettingsObj.AddComponent<GameSettings>();
                DontDestroyOnLoad(gameSettingsObj);
            }
            return _instance;
        }
        private set { _instance = value; } 
    }

    private void Awake()
    {
        InitializeSingleton();
        initialMaxMoves = maxMoves;
    }
    
    private void InitializeSingleton()
    {
        if (_instance == null)
        {
            _instance = this;
            
            if (transform.parent != null)
            {
                transform.SetParent(null); 
            }
            
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    public void ResetMoves()
    {
        maxMoves = initialMaxMoves;
        
        if (GameUI.Instance != null)
            GameUI.Instance.UpdateMovesText(maxMoves);
    }
}