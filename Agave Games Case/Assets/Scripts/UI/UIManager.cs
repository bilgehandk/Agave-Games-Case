using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject startScreenPrefab;
    [SerializeField] private GameObject gameScreenPrefab;
    [SerializeField] private GameObject endScreenPrefab;
    
    private Dictionary<GameScreenType, GameObject> screenPrefabs = new Dictionary<GameScreenType, GameObject>();
    private Dictionary<GameScreenType, UIScreen> activeScreens = new Dictionary<GameScreenType, UIScreen>();
    
    private IUIScreenFactory screenFactory;
    private GameScreenType currentScreenType;
    
    private void Awake()
    {
        InitializePrefabDictionary();
        screenFactory = UIScreenFactory.Instance;
        ShowScreen(GameScreenType.StartScreen);
    }
    
    private void InitializePrefabDictionary()
    {
        screenPrefabs[GameScreenType.StartScreen] = startScreenPrefab;
        screenPrefabs[GameScreenType.GameScreen] = gameScreenPrefab;
        screenPrefabs[GameScreenType.EndScreen] = endScreenPrefab;
    }
    
    public void ShowScreen(GameScreenType screenType)
    {
        HideCurrentScreen(screenType);
        EnsureScreenExists(screenType);
        DisplayScreen(screenType);
    }
    
    private void HideCurrentScreen(GameScreenType newScreenType)
    {
        if (currentScreenType == newScreenType || !activeScreens.ContainsKey(currentScreenType)) return;
        
        UIScreen currentScreen = activeScreens[currentScreenType];
        if (currentScreen == null) return;
        
        currentScreen.Hide();
        screenFactory.DestroyScreen(currentScreen);
        activeScreens.Remove(currentScreenType);
    }
    
    private void EnsureScreenExists(GameScreenType screenType)
    {
        if (!activeScreens.ContainsKey(screenType))
        {
            CreateScreen(screenType);
        }
    }
    
    private void DisplayScreen(GameScreenType screenType)
    {
        if (!activeScreens.TryGetValue(screenType, out UIScreen screen) || screen == null) return;
        
        screen.Show();
        currentScreenType = screenType;
    }
    
    private void CreateScreen(GameScreenType screenType)
    {
        if (!screenPrefabs.TryGetValue(screenType, out GameObject prefab) || prefab == null) return;
        
        UIScreen screen = screenFactory.CreateScreen(screenType, prefab);
        if (screen == null) return;
        
        screen.transform.SetParent(transform, false);
        activeScreens[screenType] = screen;
    }
}
