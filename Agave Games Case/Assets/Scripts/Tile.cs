using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public int x;
    public int y;

    private Item item;

    public static List<Tile> CurrentLink = new List<Tile>();

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
    public Tile Right => x < Board.Instance.width - 1 ? Board.Instance.tiles[x + 1, y] : null;
    public Tile Top => y > 0 ? Board.Instance.tiles[x, y - 1] : null;
    public Tile Bottom => y < Board.Instance.height - 1 ? Board.Instance.tiles[x, y + 1] : null;

    public Tile[] Neighbors => new[]
    {
        Left,
        Right,
        Top,
        Bottom,
    };
    

    private void Start()
    {
        if (button != null && Board.Instance != null)
        {
            button.onClick.AddListener(() => Board.Instance.Select(this));
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
                // Nesne yok edilmişse sessizce geç
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
            if (neighbor == null || exclude.Contains(neighbor) || neighbor.Item != Item) continue;

            result.AddRange(neighbor.GetConnectedTiles(exclude));
        }

        return result;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (CurrentLink.Count == 0)
        {
            CurrentLink.Add(this);
            SetButtonColor(Color.cyan); // İlk seçilen çipin rengini değiştir
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CurrentLink.Count > 0 && !CurrentLink.Contains(this) && Item == CurrentLink[0].Item)
        {
            CurrentLink.Add(this);
            SetButtonColor(Color.cyan); // Linke eklenen çipin rengini değiştir
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (CurrentLink.Count > 1)
        {
            Board.Instance.ProcessLink(CurrentLink);
        }

        foreach (var tile in CurrentLink)
        {
            tile.SetButtonColor(Color.white);
        }

        CurrentLink.Clear();
    }
}
