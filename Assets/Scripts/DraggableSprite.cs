using UnityEngine;
using System.Collections;

public class DraggableSprite : MonoBehaviour
{
    public string itemID;

    [Header("Snap Settings")] [SerializeField] float snapDistance = 0.7f; // Adjust in Inspector

    [Header("Fade Settings")] [SerializeField] float fadeDuration = 1f; // Duration of fade out animation

    [Header("Scale Animation Settings")]
    [SerializeField] float scaleDuration = 0.2f;
    [SerializeField] float dragScale = 1.1f;

    [SerializeField] GameObject nextTargetPrefab; // Prefab for the next target to activate

    private Vector3 startPosition;
    private Vector3 originalScale;
    private bool isDragging = false;

    void Start()
    {
        startPosition = transform.position;
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            HandleTouch(Input.GetTouch(0));
        }
        else
        {
            HandleMouse();
        }
    }

    void HandleTouch(Touch touch)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(touch.position);
        worldPos.z = 0;

        if (touch.phase == TouchPhase.Began)
        {
            Collider2D hit = Physics2D.OverlapPoint(worldPos);

            if (hit != null && hit.gameObject == gameObject)
            {
                isDragging = true;
                OnDragStart();
            }
        }

        if (touch.phase == TouchPhase.Moved && isDragging)
        {
            transform.position = worldPos;
        }

        if (touch.phase == TouchPhase.Ended)
        {
            if (isDragging)
                CheckDrop();

            isDragging = false;
            OnDragEnd();
        }
    }

    void HandleMouse()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0;

        if (Input.GetMouseButtonDown(0))
        {
            Collider2D hit = Physics2D.OverlapPoint(worldPos);

            if (hit != null && hit.gameObject == gameObject)
            {
                isDragging = true;
                OnDragStart();
            }
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            transform.position = worldPos;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
                CheckDrop();

            isDragging = false;
            OnDragEnd();
        }
    }

    void CheckDrop()
    {
        DropTarget[] targets = FindObjectsOfType<DropTarget>();

        DropTarget closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (DropTarget target in targets)
        {
            if (target.targetID != itemID) continue;

            float distance = Vector2.Distance(transform.position, target.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = target;
            }
        }

        // ✅ Snap if within allowed distance
        if (closestTarget != null && closestDistance <= snapDistance)
        {
            transform.position = closestTarget.transform.position;
            // Fade out the target sprite and handle deactivation
            StartCoroutine(FadeOutAndActivateNext(closestTarget, targets));


            return;
        }

        // ❌ Otherwise reset
        transform.position = startPosition;
    }

    IEnumerator FadeOutAndActivateNext(DropTarget completedTarget, DropTarget[] allTargets)
    {
        SpriteRenderer targetRenderer = GetComponent<SpriteRenderer>();
        SpriteRenderer completedRenderer = completedTarget.GetComponent<SpriteRenderer>();

        if (targetRenderer != null && completedRenderer != null)
        {
            float elapsedTime = 0f;
            Color startColor = targetRenderer.color;
            Color completedStartColor = completedRenderer.color;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;

                float t = elapsedTime / fadeDuration;
                float alpha = Mathf.Lerp(startColor.a, 0f, t);
                float completedAlpha = Mathf.Lerp(completedStartColor.a, 0f, t);

                targetRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                completedRenderer.color = new Color(completedStartColor.r, completedStartColor.g, completedStartColor.b, completedAlpha);

                yield return null;
            }

            // Ensure fully transparent at end
            targetRenderer.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
            completedRenderer.color = new Color(completedStartColor.r, completedStartColor.g, completedStartColor.b, 0f);
        }

        // Disable AFTER fade completes
        completedTarget.gameObject.SetActive(false);

        if (nextTargetPrefab)
        {
            nextTargetPrefab.SetActive(true);
        }

        enabled = false;
        LevelManager.instance.CheckLevelComplete();
    }

    void OnDragStart()
    {
        // Optionally, add any logic that should occur when dragging starts
        StartCoroutine(ScaleTo(dragScale));
    }

    void OnDragEnd()
    {
        // Optionally, add any logic that should occur when dragging ends
        StartCoroutine(ScaleTo(1f));
    }

    IEnumerator ScaleTo(float targetScaleMultiplier)
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = originalScale * targetScaleMultiplier;
        float elapsed = 0f;

        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scaleDuration;
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        transform.localScale = endScale;
    }
}