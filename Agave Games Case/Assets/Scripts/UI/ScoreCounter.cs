using TMPro;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    private int _score;
    public static ScoreCounter Instance { get; private set; }

    public int Score
    {
        get => _score;
        set
        {
            if (_score == value)
                return;

            _score = value;
            scoreText.SetText($"Score: {_score}");
        }
    }

    [SerializeField] private TextMeshProUGUI scoreText;

    private void Awake() => Instance = this;
}
