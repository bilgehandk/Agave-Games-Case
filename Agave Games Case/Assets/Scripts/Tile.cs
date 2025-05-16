using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tile : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler
{
    public int x;
    public int y;

    private Item item;

    public bool IsSelected { get; set; }

    public Item Item
    {
        get => item;
        set
        {
            if (item == value)
                return;

            item = value;
            if (icon != null && item != null && item.sprite != null)
            {
                icon.sprite = item.sprite;
            }
        }
    }
    
    public Image icon;
    public Button button;

    public Tile Left => x > 0 ? Board.Instance.tiles[x - 1, y] : null;
    public Tile Right => x < Board.Instance.Width - 1 ? Board.Instance.tiles[x + 1, y] : null;
    public Tile Top => y > 0 ? Board.Instance.tiles[x, y - 1] : null;
    public Tile Bottom => y < Board.Instance.Height - 1 ? Board.Instance.tiles[x, y + 1] : null;

    public Tile[] Neighbors => new[]
    {
        Left,
        Right,
        Top,
        Bottom,
    };
    
    public Vector3 GetCenterPosition()
    {
        if (icon != null)
        {
            return icon.transform.position;
        }
        
        return transform.position;
    }
    
    private void Start()
    {
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
        }
    }
    
    public void SetButtonColor(Color color)
    {
        if (button != null)
        {
            try
            {
                ColorBlock colors = button.colors;
                colors.normalColor = color;
                colors.highlightedColor = color;
                colors.selectedColor = color;
                colors.pressedColor = color;
                button.colors = colors;
            }
            catch (System.Exception)
            {
                Debug.LogWarning($"Failed to set button color on tile at position ({x}, {y})");
            }
        }
    }

    public List<Tile> GetConnectedTiles(HashSet<Tile> exclude = null)
    {
        var result = new List<Tile> { this };

        if (exclude == null)
        {
            exclude = new HashSet<Tile> { this };
        }
        else
        {
            exclude.Add(this);
        }

        foreach (var neighbor in Neighbors)
        {
            if (neighbor == null || exclude.Contains(neighbor) || !neighbor.Item.Equals(Item)) continue;

            result.AddRange(neighbor.GetConnectedTiles(exclude));
        }

        return result;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (Board.Instance != null)
        {
            Board.Instance.StartDrag(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0) && Board.Instance != null)
        {
            Board.Instance.DragToTile(this);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Board.Instance != null)
        {
            Board.Instance.EndDrag();
        }
    }
}
