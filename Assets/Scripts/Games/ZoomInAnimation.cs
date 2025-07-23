using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class ZoomInAnimation : MonoBehaviour
{
    [SerializeField] Transform root;
    public RectTransform target;
    public RectTransform[] multyTarget;
    [SerializeField] float zoomSize = 1;
    [SerializeField] float duration = 1;
    [SerializeField] Ease ease = Ease.OutQuad;

    public UnityEvent onPlay;

    public void Play()
    {
        if (target)
        {
            target.SetParent(root);
            target.gameObject.SetActive(true);
            var sequance = DOTween.Sequence();
            sequance.SetUpdate(true);
            sequance.Insert(0, target.DOLocalMove(Vector3.zero, duration).SetEase(ease));
            sequance.Insert(0, target.DOScale(zoomSize, duration).SetEase(ease));
            sequance.Play();
            onPlay.Invoke();
        }
        else if (multyTarget.Length > 0)
        {
            var sequance = DOTween.Sequence();
            sequance.SetUpdate(true);
            var item = GroupRects(multyTarget.Where(x => x.gameObject.activeInHierarchy).ToArray());
            item.SetParent(root);
            sequance.Insert(0, item.DOLocalMove(Vector3.zero, duration).SetEase(ease));
            sequance.Insert(0, item.DOScale(zoomSize, duration).SetEase(ease));
            sequance.Play();
            onPlay.Invoke();
        }
    }

    RectTransform GroupRects(RectTransform[] targets)
    {
        if (targets == null || targets.Length == 0)
        {
            Debug.LogWarning("No targets assigned.");
            return null;
        }

        // Buat parent baru
        GameObject groupGO = new GameObject("GroupedRect", typeof(RectTransform));
        RectTransform groupRect = groupGO.GetComponent<RectTransform>();
        groupRect.SetParent(targets[0].parent, false);
        groupRect.localScale = Vector3.one;
        groupRect.localRotation = Quaternion.identity;

        // Inisialisasi min dan max posisi
        Vector2 min = Vector2.positiveInfinity;
        Vector2 max = Vector2.negativeInfinity;

        foreach (var rect in targets)
        {
            Vector2 size = rect.sizeDelta;
            Vector2 pivotOffset = new Vector2(size.x * (0.5f - rect.pivot.x), size.y * (0.5f - rect.pivot.y));
            Vector2 worldPos = rect.anchoredPosition + pivotOffset;

            Vector2 rectMin = worldPos - size * 0.5f;
            Vector2 rectMax = worldPos + size * 0.5f;

            min = Vector2.Min(min, rectMin);
            max = Vector2.Max(max, rectMax);
        }

        Vector2 center = (min + max) / 2f;
        Vector2 sizeDelta = max - min;

        groupRect.anchorMin = new Vector2(0.5f, 0.5f);
        groupRect.anchorMax = new Vector2(0.5f, 0.5f);
        groupRect.pivot = new Vector2(0.5f, 0.5f);
        groupRect.anchoredPosition = center;
        groupRect.sizeDelta = sizeDelta;

        // Set semua target menjadi anak dari groupRect tanpa mengubah world posisinya
        foreach (var rect in targets)
        {
            Vector3 worldPos = rect.position;
            rect.SetParent(groupRect, true);
            rect.position = worldPos; // jaga world position
        }

        return groupRect;
    }
}
