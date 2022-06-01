using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerBehavior : MonoBehaviour
{
    //Score and difficulty
    public static int Score;
    public static int CurrentScore;
    private int _page;
    public static int CurrentPaperAmount;
    public static int CurrentStickerAmount;
    private bool _pageCheck;
    private int _stickerThreshold;

    [SerializeField] private GameObject pauseCanvas;
    public static bool pauseCheck;

    //Spawning variables
    private int _stickerSpawnSpeed;
    private int _paperSpawnAmount;
    private RoutineBehaviour.TimedAction _spawnStickerAction;
    [SerializeField] private GameObject _paperBall;
    [SerializeField] private GameObject _sticker;
    private float _rectCornerX = 22.25f;
    private float _rectCornerZ = 12.5f;
    [SerializeField] private Transform _playerTransform;
    private static AudioSource _audio;
    [SerializeField]
    private AudioSource _audioRef;

    public static void IncreaseScore(int value)
    {
        GameManagerBehavior.Score++;
        GameManagerBehavior.CurrentScore++;
    }

    void Start()
    {
        pauseCanvas.SetActive(false);
        pauseCheck = false;
        _page = 1;
        Score = 0;
        CurrentScore = 0;
        _paperSpawnAmount = 3;
        _stickerSpawnSpeed = 5;
        _stickerThreshold = 5;
        _spawnStickerAction = new RoutineBehaviour.TimedAction();
        PageSetup();
        _audio = _audioRef;
    }

    void Update()
    {
        GameLoop();
        pauseCanvas.SetActive(pauseCheck);
    }

    void GameLoop()
    {
        if (CurrentPaperAmount == 0 && !_pageCheck && CurrentStickerAmount == 0)
        {
            Debug.Log("Board Cleared");
            _pageCheck = true;
            _page++;
            _paperSpawnAmount = 3 + _page;
            _stickerSpawnSpeed = 5 + Mathf.RoundToInt(_page / 2);
            RoutineBehaviour.Instance.StartNewTimedAction(args => { PageSetup(); _pageCheck = false; }, TimedActionCountType.SCALEDTIME, 3f);
        }
        if (CurrentScore > _stickerThreshold && CurrentPaperAmount > 0)
        {
            if (!_spawnStickerAction.IsActive)
            {
                _spawnStickerAction = RoutineBehaviour.Instance.StartNewTimedAction(args => SpawnObject(1, _sticker), TimedActionCountType.SCALEDTIME, _stickerSpawnSpeed);
            }
        }
    }

    void PageSetup()
    {
        SpawnObject(_paperSpawnAmount, _paperBall);
        CurrentScore = 0;
    }

    void SpawnObject(int amount, GameObject spawn)
    {
        //Spawn the given amount of objects
        for (int i = 0; i < amount; i++)
        {
            Vector2 spawnPosition = RandomPointOnPerimeter(0, 0, _rectCornerX, _rectCornerZ);

            GameObject newSpawn = Instantiate(spawn, new Vector3(spawnPosition.x, 0.5f, spawnPosition.y), Quaternion.identity);

            if (spawn.tag == "Sticker") {
                SeekingBehaviour steer = newSpawn.GetComponent<SeekingBehaviour>();
                steer.Target = _playerTransform;
                CurrentStickerAmount++;
            }
        }
    }

    Vector2 RandomPointOnPerimeter(float x1, float y1, float x2, float y2)
    {
        //Pick a random side of the rectangle
        Vector2 point = new Vector2();
        int side = Random.Range(0, 4);
        //Bottom side
        if (side == 0)
        {
            point.x = Random.Range(0, x2);
            point.y = y1;
        }
        //Top side
        if (side == 1)
        {
            point.x = Random.Range(0, x2);
            point.y = y2;
        }
        //Left side
        if (side == 2)
        {
            point.x = x1;
            point.y = Random.Range(0, y2);
        }
        //Right side
        if (side == 3)
        {
            point.x = x2;
            point.y = Random.Range(0, y2);
        }
        return point;
    }

    public static void PauseGame()
    {
        Time.timeScale = 0;
        pauseCheck = true;
        _audio.Pause();
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1;
        pauseCheck = false;
        _audio.UnPause();
    }
}
