using System.Collections;
using UnityEngine;

namespace CoinCollector
{
    public class PlayerBaseController : MonoBehaviour
    {
        [SerializeField] protected float _moveSpeed = 5f;
        protected Rigidbody2D _rb;
        protected Vector2 _moveDirection;
        protected Camera _mainCamera;
        protected int _coinCount = 0;
        private Coroutine _speedBoostCoroutine;
        private bool _isSpeedBoosted = false;
        private Vector2 _currentVelocity = Vector2.zero;
        private float _defaultMoveSpeed;
        public int CoinCount => _coinCount;

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
            if(!_isSpeedBoosted)
            {
                _speedBoostCoroutine = StartCoroutine(SpeedBoost(duration, multiplier));
                _isSpeedBoosted = true;
            }
        }

        public void SetSpeedLevel(Level level) => _moveSpeed = _defaultMoveSpeed * (int)level / 100;

        private IEnumerator SpeedBoost(float duration, float multiplier)
        {
            _moveSpeed *= multiplier;
            yield return new WaitForSeconds(duration);
            _moveSpeed /= multiplier;
            _isSpeedBoosted = false;
        }

        public void AddCoin()
        {
            _coinCount++;
            print(CoinCount);
        }
    }
}
