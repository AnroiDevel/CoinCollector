using UnityEngine;

namespace CoinCollector
{
    public class PlayerController : PlayerBaseController
    {
        protected override void ProcessInputs()
        {
            if(!GameManager.Instance.GameStarted)
                return;

            float moveX = Input.GetAxis("Horizontal");
            float moveY = Input.GetAxis("Vertical");
            _moveDirection = new Vector2(moveX, moveY).normalized;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(!GameManager.Instance.GameStarted)
                return;

            if(other.CompareTag("SpeedBoost"))
            {
                ApplySpeedBoost(5f, 2f); // ���������� �������� �� 5 ������ � 2 ����
                Destroy(other.gameObject);
            }

        }
    }
}