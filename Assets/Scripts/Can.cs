using UnityEngine;

namespace CoinCollector
{
    public class Can : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag("Player"))
            {
                if(collision.TryGetComponent<PlayerBaseController>(out var playerController))
                {
                    playerController.ApplySpeedBoost(2f, 2f); // ��������� �� 10 ����������� � ��������� ���������
                }
            }
        }
    }
}
