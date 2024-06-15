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
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private AudioSource _coinSfx;

        public event System.Action OnGameStart;
        public event System.Action<PlayerBaseController> OnPlayerVictory;

        private List<GameObject> _coins = new List<GameObject>();
        private PlayerBaseController[] _players;
        private Camera _mainCamera;

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

            // Получаем мировые координаты краев экрана
            Vector3 screenBottomLeft = _mainCamera.ViewportToWorldPoint(new Vector3(0, 0, _mainCamera.nearClipPlane));
            Vector3 screenTopRight = _mainCamera.ViewportToWorldPoint(new Vector3(1, 1, _mainCamera.nearClipPlane));

            // Вычисляем координаты для спавна монет внутри границ экрана
            float xPosition = Random.Range(screenBottomLeft.x, screenTopRight.x);
            float yPosition = Random.Range(screenBottomLeft.y, screenTopRight.y);
            Vector3Int randomPosition = new Vector3Int((int)xPosition, (int)yPosition, 0);

            // Создаем монету в случайной позиции
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
            _gameOverPanel.SetActive(true);

            foreach(var p in _players)
            {
                p.gameObject.SetActive(false);
            }
        }
    }
}
