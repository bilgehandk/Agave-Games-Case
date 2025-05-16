using UnityEngine;
using DG.Tweening;

public static class DOTweenExtensions
{
    public static Tween DOScaleSafe(this Transform transform, Vector3 endValue, float duration)
    {
        if (transform == null || !transform)
            return null;
            
        try
        {
            string id = "Scale_" + transform.GetInstanceID();
            
            return transform.DOScale(endValue, duration)
                .SetId(id)
                .SetTarget(transform)
                .SetAutoKill(true)
                .Play();
        }
        catch
        {
            return null;
        }
    }
    
    public static bool IsValidForTween(this Transform transform)
    {
        return transform != null && transform.gameObject != null && transform.gameObject.activeInHierarchy;
    }
    
    public static void DOKillSafe(this Transform transform, bool complete = false)
    {
        try
        {
            if (transform != null && transform)
            {
                DOTween.Kill(transform, complete);
                transform.DOKill(complete);
            }
        }
        catch { }
    }
}
