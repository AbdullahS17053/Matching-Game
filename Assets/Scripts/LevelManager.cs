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
        if (isReplay)
        {
            isReplay = false;
            UIManager.Instance.TriggerGameWon();
            return;
        }

        int levelsPerGroup = 4;

        int groupStart = (currentLevelIndex / levelsPerGroup) * levelsPerGroup;
        int groupEnd = groupStart + levelsPerGroup - 1;

        int newLevel;

        // If only one level (safety, though not your case)
        if (groupStart == groupEnd)
        {
            newLevel = groupStart;
        }
        else
        {
            do
            {
                newLevel = Random.Range(groupStart, groupEnd + 1);
            }
            while (newLevel == currentLevelIndex); // ❌ avoid same level
        }

        currentLevelIndex = newLevel;

        Debug.Log($"Next Random Level: {currentLevelIndex}");

        UIManager.Instance.TriggerGameWon();
    }
    
    public void LoadNextLevel()
    {
        LoadLevel(currentLevelIndex);
    }
    
    public void ReplayLevel()
    {
        isReplay = true;
        LoadLevel(lastPlayedLevelIndex);
        UIManager.Instance.ClearUI();
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