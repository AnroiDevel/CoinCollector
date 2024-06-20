using System.Collections;
using UnityEngine;

namespace CoinCollector
{
    public class PlayerBaseController : MonoBehaviour
    {
        [SerializeField] protected float _moveSpeed = 5f;
        [SerializeField] private PlayerUI _playerUI;
        protected Rigidbody2D _rb;
        protected Vector2 _moveDirection;
        protected Camera _mainCamera;
        protected int _coinCount = 0;
        private Coroutine _speedBoostCoroutine;
        private Vector2 _currentVelocity = Vector2.zero;
        private float _defaultMoveSpeed;
        public int CoinCount => _coinCount;

        // Новые поля для UI состояния игрока

        protected virtual void Start()
        {
            _defaultMoveSpeed = _moveSpeed;
            _rb = GetComponent<Rigidbody2D>();
            _mainCamera = Camera.main;
        }

        protected virtual void Update()
        {
            ProcessInputs();
            RestrictMovement(); // Вызываем здесь, если нужно ограничить перемещение каждый кадр
        }

        protected virtual void FixedUpdate()
        {
            Move();
        }

        protected virtual void ProcessInputs()
        {
            // This method will be overridden in derived classes
        }

        protected void Move()
        {
            _rb.velocity = Vector2.SmoothDamp(_rb.velocity, _moveDirection * _moveSpeed, ref _currentVelocity, 0.2f);
        }

        protected void RestrictMovement()
        {
            Vector3 playerPos = transform.position;
            Vector3 viewportPos = _mainCamera.WorldToViewportPoint(playerPos);

            viewportPos.x = Mathf.Clamp(viewportPos.x, 0.05f, 0.95f);
            viewportPos.y = Mathf.Clamp(viewportPos.y, 0.05f, 0.95f);

            transform.position = _mainCamera.ViewportToWorldPoint(viewportPos);
        }

        public void ApplySpeedBoost(float duration, float multiplier)
        {
            if(_speedBoostCoroutine != null)
                return;
            _speedBoostCoroutine = StartCoroutine(SpeedBoost(duration, multiplier));
        }

        private IEnumerator SpeedBoost(float duration, float multiplier)
        {
            _playerUI.ShowStatusIcon(PlayerState.SpeedBoost); // Показать иконку ускорения
            _moveSpeed *= multiplier;
            yield return new WaitForSeconds(duration);
            _moveSpeed /= multiplier;
            _playerUI.HideStatusIcon(); // Скрыть иконку состояния
            StartCoroutine(StopMovement(2f)); // Останавливаем игрока на 2 секунды после ускорения
        }

        private IEnumerator StopMovement(float duration)
        {
            _moveSpeed = 0;

            _playerUI.ShowStatusIcon(PlayerState.Stunned); // Показать иконку оглушения
            Vector2 originalVelocity = _rb.velocity;
            _rb.velocity = Vector2.zero;
            yield return new WaitForSeconds(duration);
            _rb.velocity = originalVelocity;
            _playerUI.HideStatusIcon(); // Скрыть иконку состояния
            _moveSpeed = _defaultMoveSpeed;
            _speedBoostCoroutine = null;
        }

        public void AddCoin()
        {
            _coinCount++;
            print(CoinCount);
        }

        public void SetSpeedLevel(Level level)
        {
            _moveSpeed = _defaultMoveSpeed * (int)level / 100;
        }
    }
}
