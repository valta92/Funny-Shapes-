using UnityEngine;
using System.Collections;
using GestureRecognizer;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {


    public bool Paused { get { return _paused; } }
    public int Score { get { return _score; } }
    public int Timer { get { return _timer; } }

    [SerializeField] private bool _paused;
    [SerializeField] private int _score;
    [SerializeField] private int _timer;
    [SerializeField] private string _requiredGestureName;
    [SerializeField]private DifficultyEnum _difficulty;

    private GestureBehaviour _gestureBehaviour;

    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if(_instance == null)
                _instance = GameObject.FindObjectOfType<GameManager>();
            return _instance;
        }
    }
    void Start()
    {
        _paused = true;
        _difficulty = DifficultyEnum.Easy;
        ResetPoints();
        GUIManager.Instance.SetHUDActive(false);
    }
    void OnEnable()
    {
        GestureBehaviour.OnGestureRecognition += OnGestureRecognition;
    }
    void OnDisable()
    {
        GestureBehaviour.OnGestureRecognition -= OnGestureRecognition;
    }


    public void OnStartGame()
    {
        _paused = false;
        ResetPoints();
        GUIManager.Instance.SetHUDActive(true);
        GUIManager.Instance.StartMenu.gameObject.SetActive(false);

        int index = (int)_difficulty;
        _timer = GameConstants.Timer.TimeInTheTimer[index];
        GUIManager.Instance.RefreshTime();

        StartCoroutine(StartTheTimer());
        ChangeTheRequiredGesture();
    }

    public void OnGameOver()
    {
        _paused = true;
        StopAllCoroutines();
        GUIManager.Instance.GameOverMenu.gameObject.SetActive(true);
        GUIManager.Instance.SetHUDActive(false);
    }
    public void ChangeTheRequiredGesture()
    {
        int m = GameConstants.GestureLibrary.ShapesNames.Length;
        int randomShapeindex = Random.Range(0, m);
        string randomShapeName = GameConstants.GestureLibrary.ShapesNames[randomShapeindex];

        _requiredGestureName = randomShapeName;
        GUIManager.Instance.SetDrawText(_requiredGestureName);
    }

    public void SetDifficulty(int index)
    {
        _difficulty = (DifficultyEnum)index;
    }

    public void ResetPoints()
    {
        SetScore(0);
        SetTime(GameConstants.Timer.TimeInTheTimer[(int)_difficulty]);
    }

    public void AddScore()
    {
        _score += GameConstants.Score.AddScore[(int)_difficulty];
        GUIManager.Instance.RefreshScore();
    }

    public void SetScore(int value)
    {
        _score = value;
        GUIManager.Instance.RefreshScore();
    }

    public void AddTime(int value)
    {
        _timer += GameConstants.Timer.AddTime[(int)_difficulty];
        GUIManager.Instance.RefreshTime();
    }

    public void SetTime(int value)
    {
        _timer = value;
        GUIManager.Instance.RefreshTime();
    }

    public void SetPause(bool state)
    {        
        if (state)
        {
            _paused = true;
            Time.timeScale = 0f;
        }
        else
        {
            _paused = false;
            Time.timeScale = 1f;
        }   
    }

    IEnumerator StartTheTimer()
    {
        while(_timer> 0 && !_paused)
        {
            _timer--;
            GUIManager.Instance.RefreshTime();
            yield return new WaitForSeconds(1f);
        }

        OnGameOver();
    }

    void OnGestureRecognition(Gesture g, Result r)
    {
        if (_requiredGestureName == null)
            return;


        if(r.Name == _requiredGestureName && r.Score > 1f)
        {
            AddScore();
            AddTime(3);
            GUIManager.Instance.RefreshScore();
            GUIManager.Instance.RefreshTime();
            ChangeTheRequiredGesture();
        }   
    }
}
