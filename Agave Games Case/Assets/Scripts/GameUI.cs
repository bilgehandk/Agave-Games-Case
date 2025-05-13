using UnityEngine;
using System.Collections.Generic;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameObject startScreenObject;
    [SerializeField] private GameObject gameScreenObject;
    [SerializeField] private GameObject endScreenObject;
    
    private Dictionary<GameScreenType, UIScreen> screens = new Dictionary<GameScreenType, UIScreen>();
    private GameScreenType currentScreenType;
    
    public GameScreenType CurrentScreenType => currentScreenType;
    
    private static GameUI _instance;
    public static GameUI Instance 
    { 
        get 
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameUI>();
                
                if (_instance == null)
                {
                    GameObject gameUIObj = new GameObject("GameUI");
                    _instance = gameUIObj.AddComponent<GameUI>();
                }
            }
            return _instance;
        }
    }
    
    private IUIScreenFactory screenFactory;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            screenFactory = UIScreenFactory.Instance;
            InitializeScreens();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeScreens()
    {
        InstantiateScreen(GameScreenType.StartScreen, startScreenObject);
        GameEvents.ScreenChange += SwitchScreen;
    }
    
    private void InstantiateScreen(GameScreenType screenType, GameObject prefab)
    {
        if (prefab == null)
            return;
        
        UIScreen screen = screenFactory.CreateScreen(screenType, prefab);
        
        if (screen != null)
        {
            screen.transform.SetParent(transform, false);
            screens[screenType] = screen;
            
            screen.gameObject.SetActive(screenType == GameScreenType.StartScreen);
        }
    }
    
    private void OnDestroy()
    {
        GameEvents.ScreenChange -= SwitchScreen;
    }
    
    private void Start()
    {
        SwitchScreen(GameScreenType.StartScreen);
    }
    
    public void SwitchScreen(GameScreenType screenType)
    {
        if (currentScreenType == screenType && screens.ContainsKey(screenType) && screens[screenType] != null)
            return;
        
        if (screens.ContainsKey(currentScreenType) && screens[currentScreenType] != null)
        {
            UIScreen oldScreen = screens[currentScreenType];
            oldScreen.Hide();
            screens.Remove(currentScreenType);
            screenFactory.DestroyScreen(oldScreen);
        }
        
        if (!screens.ContainsKey(screenType) || screens[screenType] == null)
        {
            GameObject prefab = GetPrefabForScreen(screenType);
            if (prefab != null)
                InstantiateScreen(screenType, prefab);
            else
                return;
        }
        
        if (screens.ContainsKey(screenType) && screens[screenType] != null)
        {
            screens[screenType].Show();
            currentScreenType = screenType;
        }
    }
    
    private GameObject GetPrefabForScreen(GameScreenType screenType)
    {
        switch (screenType)
        {
            case GameScreenType.StartScreen: return startScreenObject;
            case GameScreenType.GameScreen: return gameScreenObject;
            case GameScreenType.EndScreen: return endScreenObject;
            default: return null;
        }
    }
    
    public void UpdateMovesText(int movesLeft)
    {
        if (screens.TryGetValue(GameScreenType.GameScreen, out UIScreen screen) && screen is GameScreen gameScreen)
            gameScreen.UpdateMovesText(movesLeft);
    }
    
    public void ShowGameOver(int score)
    {
        PrepareEndScreen(score, false);
        GameEvents.OnGameOver(score);
        SwitchScreen(GameScreenType.EndScreen);
    }
    
    public void ShowWin(int score)
    {
        PrepareEndScreen(score, true);
        GameEvents.OnGameWin(score);
        SwitchScreen(GameScreenType.EndScreen);
    }
    
    private void PrepareEndScreen(int score, bool isWin)
    {
        if (!screens.ContainsKey(GameScreenType.EndScreen) || screens[GameScreenType.EndScreen] == null)
            InstantiateScreen(GameScreenType.EndScreen, endScreenObject);
        
        if (screens.TryGetValue(GameScreenType.EndScreen, out UIScreen endScreen) && 
            endScreen is EndScreen endScreenImpl)
        {
            endScreenImpl.PreInitialize(score, isWin);
        }
    }
    
    public UIScreen GetCurrentScreen()
    {
        return screens.TryGetValue(currentScreenType, out UIScreen screen) ? screen : null;
    }
}
