using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public LevelData levelData;

    public Transform levelParent; // empty object to keep hierarchy clean

    private GameObject currentLevel;
    public int currentLevelIndex = 0;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
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
            return;
        }

        LoadLevel(currentLevelIndex);
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