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


        private void Update()
        {
            if(GameManager.Instance.GameStarted)
                UpdateCoinCountUI();
        }

        public void SetPlayerAI(bool isAI)
        {
            _secondPlayer = isAI ? _playerAI : _player2;
        }

        private void UpdateCoinCountUI()
        {
            var allScore = _player1.CoinCount + _secondPlayer.CoinCount;

            _player1CoinCountText.text = "Монеты: " + allScore.ToString();
            //_player2CoinCountText.text = "Player 2: " + _secondPlayer.CoinCount.ToString();
        }
    }
}
