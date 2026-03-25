using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Button playbtn;
    [SerializeField] private Button backBtn;
    
    [Header("Tap Effect Settings")] [SerializeField]
    private float scaleAmount = 1.1f;

    [SerializeField] private float scaleDuration = 0.1f;
    [SerializeField] private float afterEffectDelay = 0.1f;
    
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        if (playbtn)
        {
            playbtn.onClick.AddListener(OnPlayButtonClicked);
        }
        if (backBtn)
        {
            backBtn.onClick.AddListener(OnBackButtonClicked);
        }
    }

    void OnPlayButtonClicked()
    {
        StartCoroutine(PlayTapEffect(() =>
        {
            Debug.Log("Play Button Clicked!");
            //AudioController.Instance.PlaySound("Click");
            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                LevelManager.instance.LoadNextLevel();
                if (UIManager.Instance != null)
                    UIManager.Instance.ClearUI();
            }
            else
            {
                SceneManager.LoadScene(1);
            }
        }));
    }
    
    void OnBackButtonClicked()
    {
        StartCoroutine(PlayTapEffect(() =>
        {
            Debug.Log("Back Button Clicked!");
            //AudioController.Instance.PlaySound("Click");
            SceneManager.LoadScene(0);
        }));
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1)
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ClearUI();
            }

            SceneManager.sceneLoaded -= OnSceneLoaded; // IMPORTANT (avoid duplicate calls)
        }
    }
    
    public IEnumerator PlayTapEffect(System.Action onComplete)
    {
        //AudioController.Instance.PlaySound("Click");
        GameObject clicked = null;

        if (EventSystem.current != null)
        {
            clicked = EventSystem.current.currentSelectedGameObject;
        }
        if (clicked == null)
        {
            onComplete?.Invoke();
            yield break;
        }

        Transform clickedTransform = clicked.transform;

        Vector3 originalScale = clickedTransform.localScale;
        Vector3 targetScale = originalScale * scaleAmount;

        float timer = 0f;
        while (timer < scaleDuration)
        {
            clickedTransform.localScale = Vector3.Lerp(originalScale, targetScale, timer / scaleDuration);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        clickedTransform.localScale = targetScale;

        timer = 0f;
        while (timer < scaleDuration)
        {
            clickedTransform.localScale = Vector3.Lerp(targetScale, originalScale, timer / scaleDuration);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        clickedTransform.localScale = originalScale;

        yield return new WaitForSeconds(afterEffectDelay);

        onComplete?.Invoke();
    }
}
