using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CoinCollector
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private GameObject _coinPrefab;
        [SerializeField] private float _spawnInterval = 2f;
        [SerializeField] private Text _victoryText;
        [SerializeField] private GameObject _gameWinPanel;
        [SerializeField] private GameObject _gameLoosePanel;
        [SerializeField] private AudioSource _coinSfx;
        [SerializeField] private PlayerChaserAI _dog;

        public event System.Action OnGameStart;
        public event System.Action<PlayerBaseController> OnPlayerVictory;

        private List<GameObject> _coins = new();
        private PlayerBaseController[] _players;
        private Camera _mainCamera;
        private int _playersAlive = 2;

        public GameObject[] Coins => _coins.ToArray();

        public bool GameStarted { get; private set; } = false;

        private void Awake()
        {
            Instance = this;
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, true);
        }

        public void StartGame()
        {
            _mainCamera = Camera.main;
            _players = FindObjectsOfType<PlayerBaseController>();
            GameStarted = true;
            OnGameStart?.Invoke();
            StartCoroutine(SpawnCoins());
        }

        public void SetDifficultGame(Level level)
        {
            _dog.SetSpeedLevel(level);
        }

        private IEnumerator SpawnCoins()
        {
            while(GameStarted)
            {
                SpawnCoin();
                yield return new WaitForSeconds(_spawnInterval);
            }
        }

        private void SpawnCoin()
        {
            Vector3 screenBottomLeft = _mainCamera.ViewportToWorldPoint(new Vector3(0, 0, _mainCamera.nearClipPlane));
            Vector3 screenTopRight = _mainCamera.ViewportToWorldPoint(new Vector3(1, 1, _mainCamera.nearClipPlane));
            float xPosition = Random.Range(screenBottomLeft.x, screenTopRight.x);
            float yPosition = Random.Range(screenBottomLeft.y, screenTopRight.y);
            Vector3 randomPosition = new Vector3(xPosition, yPosition, 0);
            GameObject coin = Instantiate(_coinPrefab, randomPosition, Quaternion.identity);
            _coins.Add(coin);
        }

        public void RegisterCoin(GameObject coin)
        {
            _coins.Add(coin);
        }

        public void UnregisterCoin(GameObject coin)
        {
            _coins.Remove(coin);
        }

        public void CleanUpCoins()
        {
            _coins.RemoveAll(item => item == null);
        }

        public void RestartScene()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void CheckPlayerVictory()
        {
            _coinSfx.Play();
            foreach(var player in _players)
            {
                if(player.CoinCount >= 50)
                {
                    GameStarted = false;
                    OnPlayerVictory?.Invoke(player);
                }
            }
        }

        public void OnPlayerDied()
        {
            _playersAlive--;
            if(_playersAlive <= 0)
            {
                GameOver();
            }
        }

        private void GameOver()
        {
            GameStarted = false;
            _gameLoosePanel.SetActive(true);
        }

        private void OnEnable()
        {
            OnPlayerVictory += HandlePlayerVictory;
        }

        private void OnDisable()
        {
            OnPlayerVictory -= HandlePlayerVictory;
        }

        private void HandlePlayerVictory(PlayerBaseController player)
        {
            GameStarted = false;
            _victoryText.text = $"{player.name}";
            _gameWinPanel.SetActive(true);

            foreach(var p in _players)
            {
                p.gameObject.SetActive(false);
            }
        }
    }
}
