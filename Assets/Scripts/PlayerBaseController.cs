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
        public int CoinCount => _coinCount;

        protected virtual void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _mainCamera = Camera.main;
        }

        protected virtual void Update()
        {
            ProcessInputs();
        }

        protected virtual void FixedUpdate()
        {
            Move();
            RestrictMovement();
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
            {
                StopCoroutine(_speedBoostCoroutine);
                _moveSpeed /= 2; // Reset the speed boost before starting a new one
                _isSpeedBoosted = false;
            }

            _speedBoostCoroutine = StartCoroutine(SpeedBoost(duration, multiplier));
        }

        private IEnumerator SpeedBoost(float duration, float multiplier)
        {
            if(!_isSpeedBoosted)
            {
                _moveSpeed *= multiplier;
                _isSpeedBoosted = true;
            }

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
