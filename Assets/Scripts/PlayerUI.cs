using System.Collections.Generic;
using UnityEngine;

namespace CoinCollector
{

    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _hearts; // ������ ���� ��������
        [SerializeField] private PlayerHealth _playerHealth;

        // ����� ���� ��� ������ ���������
        [SerializeField] private GameObject _statusIcon; // ������ ������ ���������
        [SerializeField] private SpriteRenderer _statusIconSpriteRenderer; // ��������� SpriteRenderer ��� ������ ���������

        // ������� ��� ���������� ��������� �� ���������
        [SerializeField] private List<StateSpritePair> _stateSpritePairs;
        private Dictionary<PlayerState, Sprite> _stateSpriteDictionary;

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

            // ������������� �������
            _stateSpriteDictionary = new Dictionary<PlayerState, Sprite>();
            foreach(var pair in _stateSpritePairs)
            {
                _stateSpriteDictionary[pair.State] = pair.Sprite;
            }
        }

        private void UpdateLivesUI(int currentLives)
        {
            for(int i = 0; i < _hearts.Count; i++)
            {
                _hearts[i].SetActive(i < currentLives);
            }
        }

        // ����� ��� ����������� ������ ���������
        public void ShowStatusIcon(PlayerState state)
        {
            if(_statusIcon != null && _statusIconSpriteRenderer != null && _stateSpriteDictionary.ContainsKey(state))
            {
                _statusIconSpriteRenderer.sprite = _stateSpriteDictionary[state];
                _statusIcon.SetActive(true);
            }
        }

        // ����� ��� ������� ������ ���������
        public void HideStatusIcon()
        {
            if(_statusIcon != null)
            {
                _statusIcon.SetActive(false);
            }
        }

        [System.Serializable]
        private struct StateSpritePair
        {
            public PlayerState State;
            public Sprite Sprite;
        }
    }
}
