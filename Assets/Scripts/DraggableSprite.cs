using UnityEngine;
using System.Collections;

public class DraggableSprite : MonoBehaviour
{
    public string itemID;

    [Header("Snap Settings")] [SerializeField] float snapDistance = 0.5f; // Adjust in Inspector

    [Header("Fade Settings")] [SerializeField] float fadeDuration = 1f; // Duration of fade out animation
    
    [SerializeField] GameObject nextTargetPrefab; // Prefab for the next target to activate

    private Vector3 startPosition;
    private bool isDragging = false;

    void Start()
    {
        startPosition = transform.position;
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

        if (targetRenderer != null)
        {
            float elapsedTime = 0f;
            Color startColor = targetRenderer.color;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;

                float t = elapsedTime / fadeDuration;
                float alpha = Mathf.Lerp(startColor.a, 0f, t);

                targetRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

                yield return null;
            }

            // Ensure fully transparent at end
            targetRenderer.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
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
}