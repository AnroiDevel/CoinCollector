using UnityEngine;
using UnityEngine.UI;

namespace CoinCollector
{
    public class CoinCounterUI : MonoBehaviour
    {
        [SerializeField] private PlayerBaseController _player1;
        [SerializeField] private PlayerBaseController _player2; // Изменяем тип на Player2Controller
        [SerializeField] private PlayerBaseController _playerAI;
        [SerializeField] private Text _player1CoinCountText;
        [SerializeField] private Text _player2CoinCountText;

        private PlayerBaseController _secondPlayer;

        private void Start()
        {
            _secondPlayer = _playerAI;
        }

        private void Update()
        {
            UpdateCoinCountUI();
        }

        public void SetPlayerAI(bool isAI)
        {
            _secondPlayer = isAI ? _playerAI : _player2;
        }

        private void UpdateCoinCountUI()
        {
            _player1CoinCountText.text = "Player 1: " + _player1.CoinCount.ToString();
            _player2CoinCountText.text = "Player 2: " + _secondPlayer.CoinCount.ToString();
        }
    }
}
