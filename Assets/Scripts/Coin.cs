using UnityEngine;

namespace CoinCollector
{
    public class Coin : MonoBehaviour
    {
        private CoinSpawner _coinSpawner;

        private void Start()
        {
            _coinSpawner = GameManager.Instance.CoinSpawner;
            _coinSpawner.RegisterCoin(gameObject);
        }

        private void OnDestroy()
        {
            if(_coinSpawner != null)
            {
                _coinSpawner.UnregisterCoin(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag("Player"))
            {
                if(collision.TryGetComponent<PlayerBaseController>(out var playerController))
                {
                    playerController.AddCoin();
                    GameManager.Instance.CheckVictory(); // Проверяем условия победы после подбора монеты
                    Destroy(gameObject);
                }
            }
        }
    }
}
