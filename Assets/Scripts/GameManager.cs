using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [SerializeField] private Button nextBtn;
    [SerializeField] private Button replayBtn;
    [SerializeField] private Button backBtn;
    [SerializeField] private Button menuBtn;
    
    [Header("Level Selection Settings")]
    [SerializeField] private LevelData levelData;
    [SerializeField] private GameObject levelSelectionPanel;
    [SerializeField] private Button levelBtnPrefab;
    [SerializeField] private GameObject levelBtnParent;
    [SerializeField] private bool isMenu;
    
    private const string SelectedLevelKey = "SelectedLevel";

    [Header("Tap Effect Settings")] [SerializeField]
    private float scaleAmount = 1.1f;

    [SerializeField] private float scaleDuration = 0.1f;
    [SerializeField] private float afterEffectDelay = 0.1f;
    
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        if (nextBtn)
        {
            nextBtn.onClick.AddListener(OnNextButtonClicked);
        }
        if (backBtn)
        {
            backBtn.onClick.AddListener(OnBackButtonClicked);
        }
        if (replayBtn)
        {
            replayBtn.onClick.AddListener(OnReplayButtonClicked);
        }

        if (menuBtn)
        {
            menuBtn.onClick.AddListener(OnMenuButtonClicked);
        }
        
        AudioController.Instance.PlayMusic("BGM");
    }

    private void Start()
    {
        if (isMenu)
        {
            SetUpLevels();
        }
    }

    void SetUpLevels()
    {
        for (int i = 0; i < levelData.levels.Length; i++)
        {
            int index = i; // Capture the current index for the lambda
            Button levelBtn = Instantiate(levelBtnPrefab, levelBtnParent.transform);
            levelBtn.GetComponentInChildren<TextMeshProUGUI>().text = $"{i + 1}";
            levelBtn.onClick.AddListener(() =>
            {
                StartCoroutine(PlayTapEffect(() =>
                {
                    Debug.Log($"Level {index + 1} Button Clicked!");

                    // ✅ Save selected level
                    PlayerPrefs.SetInt(SelectedLevelKey, index);
                    PlayerPrefs.Save();

                    SceneManager.LoadScene(1);
                }));
            });
            if(index<=PlayerPrefs.GetInt("HighestLevel", 0))
                levelBtn.interactable = true;
            else
                levelBtn.interactable = false;
        }
    }
    void OnNextButtonClicked()
    {
        StartCoroutine(PlayTapEffect(() =>
        {
            Debug.Log("Play Button Clicked!");
            //AudioController.Instance.PlaySound("Click");
            if (!isMenu)
            {
                LevelManager.instance.LoadNextLevel();
                if (UIManager.Instance != null)
                    UIManager.Instance.ClearUI();
            }
            else
            {
                levelSelectionPanel.SetActive(true);
            }
        }));
    }

    void OnReplayButtonClicked()
    {
        StartCoroutine(PlayTapEffect(() =>
        {
            Debug.Log("Replay Button Clicked!");
            LevelManager.instance.ReplayLevel();
        }));
    }
    void OnBackButtonClicked()
    {
        StartCoroutine(PlayTapEffect(() =>
        {
            Debug.Log("Back Button Clicked!");
            if (isMenu)
            {
                levelSelectionPanel.SetActive(false);
            }
            else
            {
                SceneManager.LoadScene(0);
            }
            
        }));
    }
    
    void OnMenuButtonClicked()
    {
        StartCoroutine(PlayTapEffect(() =>
        {
            Debug.Log("Menu Button Clicked!");
            SceneManager.LoadScene(0);
        }));
    }

    public IEnumerator PlayTapEffect(System.Action onComplete)
    {
        AudioController.Instance.PlaySound("Click");
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
