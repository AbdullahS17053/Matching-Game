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
    
    private const string HighestLevelKey = "HighestLevel";
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
        else
        {
            currentLevelIndex = PlayerPrefs.GetInt(HighestLevelKey, 0);
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
        // If replay, don't progress
        if (isReplay)
        {
            isReplay = false;
            //LoadLevel(lastPlayedLevelIndex);
            UIManager.Instance.TriggerGameWon(true);
            return;
        }

        currentLevelIndex++;

        if (currentLevelIndex >= levelData.levels.Length)
        {
            Debug.Log("All Levels Completed!");
            UIManager.Instance.TriggerGameWon(false);
            return;
        }

        int savedHighest = PlayerPrefs.GetInt(HighestLevelKey, 0);

        if (currentLevelIndex > savedHighest)
        {
            PlayerPrefs.SetInt(HighestLevelKey, currentLevelIndex);
            PlayerPrefs.Save();
        }

        UIManager.Instance.TriggerGameWon(true);
    }
    
    public void LoadNextLevel()
    {
        if (currentLevelIndex < levelData.levels.Length)
        {
            LoadLevel(currentLevelIndex);
        }
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