using System.Collections.Generic;
using UnityEngine;


namespace CoinCollector
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _hearts; // Список всех сердечек
        [SerializeField] private PlayerHealth _playerHealth;

        private void OnEnable()
        {
            if(_playerHealth != null)
            {
                _playerHealth.LifeChangedEvent += UpdateLivesUI;
            }
        }

        private void OnDisable()
        {
            if(_playerHealth != null)
            {
                _playerHealth.LifeChangedEvent -= UpdateLivesUI;
            }
        }

        private void Start()
        {
            if(_playerHealth != null)
            {
                UpdateLivesUI(_playerHealth.GetCurrentLives());
            }
        }

        private void UpdateLivesUI(int currentLives)
        {
            for(int i = 0; i < _hearts.Count; i++)
            {
                _hearts[i].SetActive(i < currentLives);
            }
        }
    }

}