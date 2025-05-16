using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using System.Linq;

public class BoardAnimator : MonoBehaviour
{
    private const float TweenDuration = 0.25f;
    private AudioSource _audioSource;
    private AudioClip _collectionSound;

    public static BoardAnimator Instance { get; private set; }

    private void Awake() => Instance = this;

    public void Initialize(AudioSource audioSource, AudioClip collectionSound)
    {
        _audioSource = audioSource;
        _collectionSound = collectionSound;
    }

    public async Task AnimateSwap(Tile tile1, Tile tile2)
    {
        if (tile1?.icon?.transform == null || tile2?.icon?.transform == null)
            return;

        var sequence = DOTween.Sequence();
        
        sequence.Join(tile1.icon.transform.DOMove(tile2.icon.transform.position, TweenDuration))
                .Join(tile2.icon.transform.DOMove(tile1.icon.transform.position, TweenDuration));

        await sequence.Play().AsyncWaitForCompletion();
    }

    public async Task AnimatePopAndScore(Item itemType, List<Tile> connectedTiles)
    {
        var validTiles = connectedTiles?.Where(t => t?.icon?.transform != null).ToList();
        
        if (validTiles == null || validTiles.Count < 3 || itemType == null)
            return;

        if (_audioSource != null && _collectionSound != null)
            _audioSource.PlayOneShot(_collectionSound);

        if (ScoreCounter.Instance != null)
            ScoreCounter.Instance.Score += itemType.value * validTiles.Count;

        var deflateSequence = DOTween.Sequence();
        foreach (var tile in validTiles)
            deflateSequence.Join(tile.icon.transform.DOScale(Vector3.zero, TweenDuration));

        await deflateSequence.Play().AsyncWaitForCompletion();

        if (ItemManager.Items == null || ItemManager.Items.Count == 0)
            return;
            
        var inflateSequence = DOTween.Sequence();
        
        foreach (var tile in validTiles)
        {
            tile.Item = ItemManager.Items[Random.Range(0, ItemManager.Items.Count)];
            inflateSequence.Join(tile.icon.transform.DOScale(Vector3.one, TweenDuration));
        }

        await inflateSequence.Play().AsyncWaitForCompletion();
    }
}
