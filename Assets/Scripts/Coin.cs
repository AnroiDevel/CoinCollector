using UnityEngine;

namespace CoinCollector
{
    public class Coin : MonoBehaviour
    {
        private void Start()
        {
            GameManager.Instance.RegisterCoin(gameObject);
        }

        private void OnDestroy()
        {
            GameManager.Instance.UnregisterCoin(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag("Player"))
            {
                if(collision.TryGetComponent<PlayerBaseController>(out var playerController))
                {
                    playerController.AddCoin();
                    GameManager.Instance.CheckPlayerVictory(); // Проверяем условия победы после подбора монеты
                    Destroy(gameObject);
                }
            }
        }
    }
}
