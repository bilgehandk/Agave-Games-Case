using UnityEngine;

public class UINavigator : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    
    private void Awake()
    {
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
        }
    }
    
    public void NavigateTo(GameScreenType screenType)
    {
        uiManager?.ShowScreen(screenType);
    }
    
    public void NavigateToStartScreen() => NavigateTo(GameScreenType.StartScreen);
    
    public void NavigateToGameScreen() => NavigateTo(GameScreenType.GameScreen);
    
    public void NavigateToEndScreen() => NavigateTo(GameScreenType.EndScreen);
}
