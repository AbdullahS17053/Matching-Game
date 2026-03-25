using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup levelWonUI;
    [SerializeField] private GameObject winPanel;
    [SerializeField] TextMeshProUGUI levelNumberText;
    
    [SerializeField] private bool isGame;
    public static UIManager Instance { get; private set; }
    
    void Awake()
    {
        Instance = this;

        if (isGame)
        {
            levelWonUI.alpha = 0f;
            winPanel.transform.localPosition = new Vector2(0, +Screen.height);
        }
    }
    
    public void ClearUI()
    {
        levelWonUI.alpha = 0f;
        winPanel.transform.localPosition = new Vector2(0, +Screen.height);
        Invoke(nameof(DisableWonUI), 0.5f);
    }

    void DisableWonUI()
    {
        levelWonUI.gameObject.SetActive(false);
    }
    
    public void TriggerGameWon()
    {
        Debug.Log("Game Won!");
        levelWonUI.gameObject.SetActive(true);
        //AudioController.Instance.PlaySound("Win");
        levelNumberText.text = $"{LevelManager.instance.currentLevelIndex}";
        levelWonUI.LeanAlpha(1, 0.5f);
        //pauseBtn.SetActive(false);
        winPanel.LeanMoveLocalY(0, 0.5f).setEaseOutExpo().delay = 0.1f;
        PlayerPrefs.Save();
    }
}
