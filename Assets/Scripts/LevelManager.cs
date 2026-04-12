using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public LevelData levelData;

    public Transform levelParent; // empty object to keep hierarchy clean

    private GameObject currentLevel;
    public int currentLevelIndex = 0;
    private bool isReplay = false;
    private int lastPlayedLevelIndex = 0;
    private const string SelectedLevelKey = "SelectedLevel";

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (PlayerPrefs.HasKey(SelectedLevelKey))
        {
            currentLevelIndex = PlayerPrefs.GetInt(SelectedLevelKey);
        }

        LoadLevel(currentLevelIndex);
    }

    public void LoadLevel(int index)
    {
        if (currentLevel != null)
        {
            Destroy(currentLevel);
        }

        lastPlayedLevelIndex = index;
        currentLevel = Instantiate(levelData.levels[index], levelParent);
    }

    public void NextLevel()
    {
        AudioController.Instance.PlaySound("Win");

        if (isReplay)
        {
            isReplay = false;
            LoadNextLevel();
            return;
        }

        int levelsPerGroup = 4;

        int groupStart = (currentLevelIndex / levelsPerGroup) * levelsPerGroup;
        int groupEnd = groupStart + levelsPerGroup - 1;

        int newLevel;

        do
        {
            newLevel = Random.Range(groupStart, groupEnd + 1);
        }
        while (newLevel == currentLevelIndex);

        currentLevelIndex = newLevel;

        Debug.Log($"Next Random Level: {currentLevelIndex}");

        // ✅ Directly load next level (no UI call)
        LoadNextLevel();
    }
    
    public void LoadNextLevel()
    {
        UIManager.Instance.ClearUI();
        LoadLevel(currentLevelIndex);
    }
    
    public void ReplayLevel()
    {
        isReplay = true;
        UIManager.Instance.ClearUI();
        LoadLevel(lastPlayedLevelIndex);
    }

    public void CheckLevelComplete()
    {
        DraggableSprite[] items = currentLevel.GetComponentsInChildren<DraggableSprite>();

        foreach (var item in items)
        {
            if (item.enabled) return; // still draggable → not complete
        }

        Debug.Log("Level Complete!");
        UIManager.Instance.TriggerEmoji();
        Invoke(nameof(NextLevel), 2f);
    }
}