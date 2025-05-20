using UnityEngine;
using DG.Tweening;

public class GlobalTweenManager : MonoBehaviour
{
    public static GlobalTweenManager Instance { get; private set; }
    
    [SerializeField] private int tweensCapacity = 1250;
    [SerializeField] private int sequencesCapacity = 50;
    [SerializeField] private bool useSafeMode = true;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        if (!DOTween.instance)
            DOTween.SetTweensCapacity(tweensCapacity, sequencesCapacity);
        
        DOTween.useSafeMode = useSafeMode;
        DOTween.defaultAutoPlay = AutoPlay.None;
        DOTween.defaultAutoKill = true;
        
        GameEvents.BeforeScreenChange += KillAllTweens;
    }
    
    public void KillAllTweens()
    {
        try
        {
            DOTween.KillAll(true);
            DOTween.Clear(true);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error while killing all tweens: {e.Message}");
        }
    }
    
    private void OnDestroy()
    {
        GameEvents.BeforeScreenChange -= KillAllTweens;
        DOTween.KillAll();
        DOTween.Clear();
    }
}
