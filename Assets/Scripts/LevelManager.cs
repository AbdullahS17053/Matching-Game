using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public LevelData levelData;

    public Transform levelParent; // empty object to keep hierarchy clean

    private GameObject currentLevel;
    public int currentLevelIndex = 0;
    
    private const string HighestLevelKey = "HighestLevel";

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        currentLevelIndex = PlayerPrefs.GetInt(HighestLevelKey, 0);
        LoadLevel(currentLevelIndex);
    }

    public void LoadLevel(int index)
    {
        // Destroy old level
        if (currentLevel != null)
        {
            Destroy(currentLevel);
        }

        // Spawn new level
        currentLevel = Instantiate(levelData.levels[index], levelParent);
    }

    public void NextLevel()
    {
        currentLevelIndex++;

        if (currentLevelIndex >= levelData.levels.Length)
        {
            Debug.Log("All Levels Completed!");
            UIManager.Instance.TriggerGameWon(false);
            return;
        }

        // Save highest level reached
        int savedHighest = PlayerPrefs.GetInt(HighestLevelKey, 0);

        if (currentLevelIndex > savedHighest)
        {
            PlayerPrefs.SetInt(HighestLevelKey, currentLevelIndex);
            PlayerPrefs.Save();
        }

        UIManager.Instance.TriggerGameWon(true);
        //LoadLevel(currentLevelIndex);
    }
    
    public void LoadNextLevel()
    {
        if (currentLevelIndex < levelData.levels.Length)
        {
            LoadLevel(currentLevelIndex);
        }
    }

    public void CheckLevelComplete()
    {
        DraggableSprite[] items = currentLevel.GetComponentsInChildren<DraggableSprite>();

        foreach (var item in items)
        {
            if (item.enabled) return; // still draggable → not complete
        }

        Debug.Log("Level Complete!");
        Invoke(nameof(NextLevel), 1f);
    }
}