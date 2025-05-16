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
    
    public static Sequence CreateSafeSequence(string id = null)
    {
        Sequence sequence = DOTween.Sequence();
        
        if (!string.IsNullOrEmpty(id))
            sequence.SetId(id);
        
        sequence.SetAutoKill(true);
        return sequence;
    }
    
    public void KillAllTweens()
    {
        try
        {
            DOTween.KillAll(true);
            DOTween.Clear(true);
        }
        catch { }
    }
    
    public static bool IsValidForTween(Transform transform)
    {
        return transform != null && transform.gameObject != null && transform.gameObject.activeInHierarchy;
    }
    
    private void OnDestroy()
    {
        GameEvents.BeforeScreenChange -= KillAllTweens;
        DOTween.KillAll();
        DOTween.Clear();
    }
}
