using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [Header("Game Object Links")] public GameObject murmel;
    public GameObject tabsi;
    public GameObject filu;
    public GameObject mainCamera;
    public GameObject gameOverUI;
    public TextMeshProUGUI scoreTMP;

    [Header("Buff/Debuff Prefabs")] public GameObject hatRedBuff;
    public GameObject hatGreenBuff;
    public GameObject foodDebuff;

    private GameObject _activePlayer;
    private float _scaleFactor;
    private int _score;
    private int _scoreBuildPlatforms; //score at which new platforms need to be build
    private int _scoreBuildSpecial; //score at which new special pattern platforms will be build
    private float _highestPlatformY;
    private static bool _gameOver;
    public static bool IsGamerOver() => _gameOver;

    private void Awake()
    {
        _score = 0;
        _gameOver = false;
        _highestPlatformY = -7f;
        _scoreBuildPlatforms = 600;
        _scoreBuildSpecial = 1500;
    }

    // Start is called before the first frame update
    void Start()
    {
        //init every script with active character
        SetActivePlayer();
        BuffSystem.Instance.Init(_activePlayer);
        mainCamera.GetComponent<CameraFollow>().SetTarget(_activePlayer.transform);
        
        //Scale for aspect ratio
        var scaler = GameObject.Find("Scaler");
        _scaleFactor = SceneSelector.ScaleFactor();
        if(scaler != null)
            scaler.transform.localScale = new Vector3(_scaleFactor, _scaleFactor, _scaleFactor);

        _highestPlatformY *= _scaleFactor;
        var spawnPos = _activePlayer.transform.position;
        spawnPos.y -= 2.0f * _scaleFactor;
        var startPlatform = PlatformObjectPool.Instance.Get(PlatformType.Default);
        startPlatform.transform.position = spawnPos;
        SetPlatformsOfType(randomType: false);
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameOver) return;

        UpdateScore(_activePlayer.transform.localPosition.y);

        if (_score > _scoreBuildPlatforms)
        {
            StartCoroutine(ClearOldPlatforms(mainCamera.transform.position.y - 5.2f*_scaleFactor)); //platforms under the screen
            _scoreBuildPlatforms = _score + 600;
        }

        if (_activePlayer.transform.position.y < mainCamera.transform.position.y - 4.7f)
            StartCoroutine(InitGameover());
    }

    private void UpdateScore(float playerY)
    {
        var newScore = (int) (playerY * 10);
        if (newScore <= _score) return;
        _score = newScore;
        scoreTMP.text = _score.ToString();
    }

    private void SetActivePlayer()
    {
        switch (SceneSelector.character)
        {
            case Characters.Murmel:
                murmel.SetActive(true);
                tabsi.SetActive(false);
                filu.SetActive(false);
                _activePlayer = murmel;
                break;
            case Characters.Tabsi:
                tabsi.SetActive(true);
                murmel.SetActive(false);
                filu.SetActive(false);
                _activePlayer = tabsi;
                break;
            case Characters.Filu:
                filu.SetActive(true);
                murmel.SetActive(false);
                tabsi.SetActive(false);
                _activePlayer = filu;
                break;
            default:
                murmel.SetActive(true);
                tabsi.SetActive(false);
                filu.SetActive(false);
                _activePlayer = murmel;
                break;
        }
    }
    
    /// <summary>
    /// Places "amount" new platforms of "type" on top of the existing ones
    /// </summary>
    /// <param name="type">type of platform to build</param>
    /// <param name="amount">amount of platforms to be build</param>
    /// <param name="randomType">if true, platformType will be random and "type"-param is ignored</param>
    private void SetPlatformsOfType(PlatformType type = PlatformType.Default, int amount = 30, bool randomType = true)
    {
        var spawnPosition = new Vector3();
        for (int i = 0; i < amount; i++)
        {
            var platform = PlatformObjectPool.Instance.Get(randomType ? Platform.GetRandomPlatformtype() : type);
            spawnPosition.x = Random.Range(-2f, 2f) *_scaleFactor;
            _highestPlatformY += Random.Range(2f, 3f);
            spawnPosition.y = _highestPlatformY;
            platform.transform.position = spawnPosition;
            RandomItemSpawn(platform);
            if (platform.GetComponent<Platform>()?.type == PlatformType.Moving)
                platform.GetComponent<MoveX>()?.Init();
        }
    }

    private void RandomItemSpawn(GameObject platform)
    {
        var rnd = UnityEngine.Random.Range(0, 100);
        if (rnd <= 84) //Dont Spawn item
            return;

        var type = BuffType.ChristmasBuffRed;
        if (rnd > 84 && rnd <= 92)
            type = BuffType.ChristmasBuffRed;
        if (rnd > 92 && rnd <= 97)
            type = BuffType.FoodDebuff;
        if (rnd > 98)
            type = BuffType.ChristmasBuffGreen;

        SpawnBuffItemOnPlatform(platform, type);
    }

    private void SpawnBuffItemOnPlatform(GameObject platform, BuffType type)
    {
        GameObject item;
        var position = platform.transform.position;
        position.y += 0.45f;
        switch (type)
        {
            case BuffType.ChristmasBuffRed:
                item = Instantiate(hatRedBuff, platform.transform, true);
                item.transform.position = position;
                break;
            case BuffType.ChristmasBuffGreen:
                item = Instantiate(hatGreenBuff, platform.transform, true);
                item.transform.position = position;
                break;
            case BuffType.FoodDebuff:
                item = Instantiate(foodDebuff, platform.transform, true);
                item.transform.position = position;
                break;
        }
    }

    /// <summary>
    /// Returns all platforms, that where passed already, to the object pool.
    /// </summary>
    /// <param name="clearUnder">Position.y value under which will be cleared</param>
    /// <returns></returns>
    private IEnumerator ClearOldPlatforms(float clearUnder)
    {
        var platforms = PlatformObjectPool.Instance.GetActivePlatforms();
        foreach (var platform in platforms)
            if (platform.transform.position.y < clearUnder)
                PlatformObjectPool.Instance.Release(platform);

        if (_score > _scoreBuildSpecial)
        {
            switch (Random.Range(0, 2))
            {
                case 0:
                    SetPlatformsOfType(PlatformType.Moving, 20, false);
                    break;
                case 1:
                    SetPlatformsOfType(PlatformType.Breaking, 20, false);
                    break;
            }
            _scoreBuildSpecial = _score + 1500;
        }
        else
            SetPlatformsOfType();

        yield break;
    }

    /// <summary>
    /// Stops game and starts to reveal gameOverText
    /// </summary>
    private IEnumerator InitGameover()
    {
        _gameOver = true;

        //player dying "animation"
        _activePlayer.GetComponent<BoxCollider2D>().enabled = false;
        _activePlayer.GetComponent<Animator>().enabled = false;
        _activePlayer.GetComponent<Rigidbody2D>().velocity = new Vector2(_activePlayer.transform.position.x < 0 ? 1f : -1f, 7f);
        _activePlayer.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
        //will call Gameover() via TextRevealer's "allRevealed"-UnitEvent after GameOver-Text is shown
        gameOverUI.SetActive(true);
        yield return new WaitForSeconds(2f);
        _activePlayer.SetActive(false);
    }

    /// <summary>
    /// Shows Gameover UI and saves new highscore
    /// </summary>
    public void Gameover()
    {
        gameOverUI.transform.Find("Buttons").gameObject.SetActive(true);
        scoreTMP.color = Color.white;
        scoreTMP.transform.parent.GetComponent<TextMeshProUGUI>().color = Color.white;
        if (_score > PlayerPrefs.GetInt("highscore"))
        {
            PlayerPrefs.SetInt("highscore", _score);
            var newHS = gameOverUI.transform.Find("NewHighscoreText").gameObject;
            newHS.SetActive(true);
            StartCoroutine(StartSceneManager.PulseTMP(newHS.GetComponent<TextMeshProUGUI>(), 1.3f));
        }
    }

    public void OnMenuButtonClick(GameObject buttonsParent)
    {
        buttonsParent.SetActive(false);
        SceneManager.LoadSceneAsync(1); //StartScene
    }

    public void OnPlayAgainButtonClick(GameObject buttonsParent)
    {
        buttonsParent.SetActive(false);
        SceneManager.LoadSceneAsync(2); //GameScene
    }
}