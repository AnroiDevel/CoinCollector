using UnityEngine;

namespace CoinCollector
{
    public class Bonus : MonoBehaviour
    {
        [SerializeField] private float _bonusDuration = 5f;
        [SerializeField] private float _bonusSpeedMultiplier = 2f;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag("Player"))
            {
                ApplyBonus(collision.gameObject);
                Destroy(gameObject);
            }
        }

        private void ApplyBonus(GameObject player)
        {
            if(player.TryGetComponent<PlayerController>(out var playerController))
            {
                playerController.ApplySpeedBoost(_bonusDuration, _bonusSpeedMultiplier);
            }
            else
            {
                if(player.TryGetComponent<Player2Controller>(out var player2Controller))
                {
                    player2Controller.ApplySpeedBoost(_bonusDuration, _bonusSpeedMultiplier);
                }
            }
        }
    }
}
