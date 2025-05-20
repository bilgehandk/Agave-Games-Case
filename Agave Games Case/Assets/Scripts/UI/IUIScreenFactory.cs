using UnityEngine;

public interface IUIScreenFactory
{
    UIScreen CreateScreen(GameScreenType screenType, GameObject prefab);
    void DestroyScreen(UIScreen screen);
}
