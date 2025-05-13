using UnityEngine;

public abstract class UIScreen : MonoBehaviour, IUIScreen
{
    protected bool isInitialized;

    protected virtual void Awake()
    {
        isInitialized = true;
    }

    protected virtual void OnEnable()
    {
        if (isInitialized)
        {
            OnScreenShown();
        }
    }

    protected virtual void OnDisable()
    {
        if (isInitialized)
        {
            OnScreenHidden();
        }
    }

    public virtual void Show()
    {
        gameObject?.SetActive(true);
        
        if (!isInitialized)
        {
            isInitialized = true;
            OnScreenShown();
        }
    }
    
    public virtual void Hide()
    {
        gameObject?.SetActive(false);
    }
    
    protected virtual void OnScreenShown() { }
    
    protected virtual void OnScreenHidden() { }
}
