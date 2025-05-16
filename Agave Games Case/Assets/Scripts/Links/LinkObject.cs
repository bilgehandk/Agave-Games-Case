using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class LinkObject : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image image;
    [SerializeField] private Color linkColor = Color.white;
    [SerializeField] private float horizontalWidth = 50f;
    [SerializeField] private float horizontalHeight = 10f;
    [SerializeField] private float verticalWidth = 10f;
    [SerializeField] private float verticalHeight = 50f;
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private Ease appearEase = Ease.OutBack;
    [SerializeField] private Ease disappearEase = Ease.InBack;
    private void Awake()
    {
        InitializeComponents();
    }
    private void InitializeComponents()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        if (image == null)
            image = GetComponent<Image>();
        if (image != null)
            image.color = linkColor;
        transform.localScale = Vector3.zero;
    }
    public Sequence AnimateBetweenTiles(Tile firstTile, Tile secondTile)
    {
        if (firstTile == null || secondTile == null)
        {
            return DOTween.Sequence();
        }
        bool isHorizontal = firstTile.y == secondTile.y;
        transform.position = Vector3.Lerp(
            firstTile.transform.position, 
            secondTile.transform.position, 
            0.5f
        );
        SetLinkOrientation(isHorizontal, firstTile, secondTile);
        Sequence sequence = DOTween.Sequence();
        transform.localScale = Vector3.zero;
        sequence.Append(transform.DOScale(Vector3.one, animationDuration).SetEase(appearEase));
        sequence.AppendInterval(0.1f);
        sequence.Append(transform.DOScale(Vector3.zero, animationDuration).SetEase(disappearEase));
        return sequence;
    }
    private void SetLinkOrientation(bool isHorizontal, Tile firstTile, Tile secondTile)
    {
        if (isHorizontal)
        {
            rectTransform.sizeDelta = new Vector2(horizontalWidth, horizontalHeight);
            float angle = firstTile.x < secondTile.x ? 0f : 180f;
            rectTransform.eulerAngles = new Vector3(0, 0, angle);
        }
        else
        {
            rectTransform.sizeDelta = new Vector2(verticalWidth, verticalHeight);
            float angle = firstTile.y < secondTile.y ? 90f : -90f;
            rectTransform.eulerAngles = new Vector3(0, 0, angle);
        }
    }
    public static LinkObject CreateLink(Tile firstTile, Tile secondTile, GameObject linkPrefab, Transform parent = null)
    {
        if (linkPrefab == null || firstTile == null || secondTile == null)
        {
            return null;
        }
        GameObject linkObj = Instantiate(linkPrefab, parent);
        LinkObject link = linkObj.GetComponent<LinkObject>();
        if (link == null)
            link = linkObj.AddComponent<LinkObject>();
        link.AnimateBetweenTiles(firstTile, secondTile).OnComplete(() => {
            Destroy(linkObj);
        });
        return link;
    }
}