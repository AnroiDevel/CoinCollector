using UnityEngine;


namespace CoinCollector
{
    public class SelectMode : MonoBehaviour
    {
        [SerializeField] private GameObject _player2;
        [SerializeField] private GameObject _playerAI;
        [SerializeField] private CoinCounterUI _counterUI;

        public void SelectedAI(bool isAI)
        {
            _player2.SetActive(!isAI);
            _playerAI.SetActive(isAI);
            _counterUI.SetPlayerAI(isAI);
        }

    }

}