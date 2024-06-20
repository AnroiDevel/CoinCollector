using UnityEngine;
using UnityEngine.UI;


namespace CoinCollector
{
    public class SelectMode : MonoBehaviour
    {
        [SerializeField] private GameObject _player2;
        [SerializeField] private GameObject _playerAI;
        [SerializeField] private CoinCounterUI _counterUI;

        [SerializeField] private Color _selectedColor;
        [SerializeField] private Color _unselectedColor;
        [SerializeField] private Sprite _selectedSprite;
        [SerializeField] private Sprite _unselectedSprite;

        [SerializeField] private Image[] _imagesLevel;
        [SerializeField] private Text[] _textsLevels;
        [SerializeField] private Toggle _selectedAIToggle;

        private void Start()
        {
            SelectDifficultLevel(0);
            SelectedAI(_selectedAIToggle.isOn);
        }

        public void SelectedAI(bool isAI)
        {
            _player2.SetActive(!isAI);
            _playerAI.SetActive(isAI);
            _counterUI.SetPlayerAI(isAI);
        }

        public void SelectDifficultLevel(int num)
        {
            for(int i = 0; i < _imagesLevel.Length; i++)
            {
                _imagesLevel[i].sprite = i == num ? _selectedSprite : _unselectedSprite;
                _textsLevels[i].color = i == num ? _selectedColor : _unselectedColor;
            }

            Level level = num == 0 ? Level.Easy : num == 1 ? Level.Medium : Level.Hard;
            GameManager.Instance.SetDifficultGame(level);

        }

    }

}