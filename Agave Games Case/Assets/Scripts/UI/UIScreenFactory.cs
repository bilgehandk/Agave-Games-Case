using UnityEngine;

public class UIScreenFactory : IUIScreenFactory
{
    private static UIScreenFactory instance;
    public static UIScreenFactory Instance => instance ?? (instance = new UIScreenFactory());
    
    private UIScreenFactory() { }
    
    public UIScreen CreateScreen(GameScreenType screenType, GameObject prefab)
    {
        if (prefab == null) return null;
            
        GameObject screenObject = Object.Instantiate(prefab);
        if (screenObject == null) return null;
        
        UIScreen screen = GetOrAddScreenComponent(screenType, screenObject);
        
        if (screen == null)
        {
            Object.Destroy(screenObject);
        }
        
        return screen;
    }
    
    private UIScreen GetOrAddScreenComponent(GameScreenType screenType, GameObject screenObject)
    {
        return screenType switch
        {
            GameScreenType.StartScreen => FindOrAddComponent<StartScreen>(screenObject),
            GameScreenType.GameScreen => FindOrAddComponent<GameScreen>(screenObject),
            GameScreenType.EndScreen => FindOrAddComponent<EndScreen>(screenObject),
            _ => null
        };
    }
    
    private T FindOrAddComponent<T>(GameObject gameObject) where T : UIScreen
    {
        T component = gameObject.GetComponent<T>();
        
        if (component != null) return component;
        
        component = gameObject.GetComponentInChildren<T>();
        return component ?? gameObject.AddComponent<T>();
    }

    public void DestroyScreen(UIScreen screen)
    {
        if (screen != null)
        {
            Object.Destroy(screen.gameObject);
        }
    }
}