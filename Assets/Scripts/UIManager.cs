using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup levelWonUI;
    [SerializeField] private GameObject winPanel;

    [SerializeField] private CanvasGroup emoji;

    [SerializeField] private GameObject nextBtn;
    
    [SerializeField] private bool isGame;
    public static UIManager Instance { get; private set; }
    
    void Awake()
    {
        Instance = this;

        if (isGame)
        {
            emoji.alpha = 0f;
            levelWonUI.alpha = 0f;
            winPanel.transform.localPosition = new Vector2(0, +Screen.height);
        }
    }
    
    public void ClearUI()
    {
        emoji.alpha = 0f;
        levelWonUI.alpha = 0f;
        winPanel.transform.localPosition = new Vector2(0, +Screen.height);
        Invoke(nameof(DisableWonUI), 0.5f);
    }

    public void DisableWonUI()
    {
        emoji.gameObject.SetActive(false);
        levelWonUI.gameObject.SetActive(false);
    }
    
    public void TriggerEmoji()
    {
        AudioController.Instance.PlaySound("Win");
        emoji.gameObject.SetActive(true);
        emoji.LeanAlpha(1, 0.5f);
    }
    public void TriggerGameWon()
    {
        Debug.Log("Game Won!");
        AudioController.Instance.PlaySound("Win");
        levelWonUI.gameObject.SetActive(true);
        //AudioController.Instance.PlaySound("Win");
        
        levelWonUI.LeanAlpha(1, 0.5f);
        //pauseBtn.SetActive(false);
        winPanel.LeanMoveLocalY(0, 0.5f).setEaseOutExpo().delay = 0.1f;
        PlayerPrefs.Save();
    }
}
