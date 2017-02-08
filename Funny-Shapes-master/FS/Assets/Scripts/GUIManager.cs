using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {


    public RectTransform StartMenu;
    public RectTransform GameOverMenu;
    public RectTransform DifficultyPanel;

    [SerializeField]private RectTransform _HUD;
    [SerializeField]private Text _scoreText;
    [SerializeField]private Text _timeText;
    [SerializeField]private Text _drawText;


    private static GUIManager _instance;

    public static GUIManager Instance
    {
        get
        {
            if(_instance == null)
                _instance = GameObject.FindObjectOfType<GUIManager>();
            return _instance;
        }
    }

    public void SetHUDActive(bool state)
    {
        _HUD.gameObject.SetActive(state);
    }

    public void RefreshScore()
    {
        _scoreText.text = GameConstants.Score.scoreTemplate + GameManager.Instance.Score;
    }
    public void RefreshTime()
    {
        _timeText.text = GameConstants.Timer.timerTemplate + GameManager.Instance.Timer.ToString("0 : 00");
    }
    public void SetDrawText(string text)
    {
        _drawText.text = GameConstants.DrawGesture.textTemplate + text;
    }

}
