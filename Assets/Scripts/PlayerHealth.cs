using System.Collections;
using UnityEngine;

namespace CoinCollector
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private int _maxLives = 3;
        [SerializeField] private float _invincibilityDuration = 1.5f;
        private int _currentLives;
        private bool _isInvincible = false;

        public delegate void LifeChanged(int currentLives);
        public event LifeChanged LifeChangedEvent;

        private void Start()
        {
            _currentLives = _maxLives;
            LifeChangedEvent?.Invoke(_currentLives);
        }

        public void TakeDamage()
        {
            if(!_isInvincible)
            {
                _currentLives--;
                LifeChangedEvent?.Invoke(_currentLives);

                if(_currentLives <= 0)
                {
                    Die();
                }
                else
                {
                    StartCoroutine(InvincibilityCoroutine());
                }
            }
        }

        private IEnumerator InvincibilityCoroutine()
        {
            _isInvincible = true;
            yield return new WaitForSeconds(_invincibilityDuration);
            _isInvincible = false;
        }

        private void Die()
        {
            gameObject.SetActive(false);
            GameManager.Instance.OnPlayerDied();
        }

        public int GetCurrentLives()
        {
            return _currentLives;
        }
    }
}
