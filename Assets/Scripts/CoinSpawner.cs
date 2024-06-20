using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoinCollector
{
    public class CoinSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _coinPrefab;
        [SerializeField] private float _spawnInterval = 2f;
        private Camera _mainCamera;
        private List<GameObject> _coins = new();
        private Coroutine _spawnRoutine;
        private bool _isPaused = false;

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        public void StartSpawning()
        {
            _spawnRoutine = StartCoroutine(SpawnCoins());
        }

        public void StopSpawning()
        {
            if(_spawnRoutine != null)
            {
                StopCoroutine(_spawnRoutine);
            }
            _spawnRoutine = null;
        }

        public void PauseSpawning()
        {
            _isPaused = true;
        }

        public void ResumeSpawning()
        {
            _isPaused = false;
        }

        private IEnumerator SpawnCoins()
        {
            while(true)
            {
                if(!_isPaused)
                {
                    SpawnCoin();
                }
                yield return new WaitForSeconds(_spawnInterval);
            }
        }

        private void SpawnCoin()
        {
            Vector3 screenBottomLeft = _mainCamera.ViewportToWorldPoint(new Vector3(0, 0, _mainCamera.nearClipPlane));
            Vector3 screenTopRight = _mainCamera.ViewportToWorldPoint(new Vector3(1, 1, _mainCamera.nearClipPlane));
            float xPosition = Random.Range(screenBottomLeft.x, screenTopRight.x);
            float yPosition = Random.Range(screenBottomLeft.y, screenTopRight.y);
            Vector3 randomPosition = new(xPosition, yPosition, 0);
            GameObject coin = Instantiate(_coinPrefab, randomPosition, Quaternion.identity, transform);
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

        public GameObject[] GetCoins()
        {
            return _coins.ToArray();
        }
    }
}
