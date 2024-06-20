using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CoinCollector
{
    public class CountdownTimer : MonoBehaviour
    {
        [SerializeField] private Text _timerText;
        private float _timeRemaining;
        private bool _isRunning = false;
        private bool _isPaused = false;

        public void StartTimer()
        {
            _isRunning = true;
            _isPaused = false;
            StartCoroutine(UpdateTimer());
        }

        private IEnumerator UpdateTimer()
        {
            while(_isRunning)
            {
                if(!_isPaused)
                {
                    if(_timeRemaining > 0)
                    {
                        _timeRemaining -= Time.deltaTime;
                        UpdateTimerText();
                    }
                    else
                    {
                        _isRunning = false;
                        _timeRemaining = 0;
                        UpdateTimerText();
                        TimerEnded();
                    }
                }
                yield return null;
            }
        }

        private void UpdateTimerText()
        {
            if(_timeRemaining < 0)
                _timeRemaining = 0;

            int minutes = Mathf.FloorToInt(_timeRemaining / 60);
            int seconds = Mathf.FloorToInt(_timeRemaining % 60);
            _timerText.text = $"{minutes:00}:{seconds:00}";
        }

        public void PauseTimer()
        {
            _isPaused = true;
        }

        public void ResumeTimer()
        {
            _isPaused = false;
        }

        public void StopTimer()
        {
            _isRunning = false;
        }

        public void ResetTimer()
        {
            StopTimer();
            _timeRemaining = 0;
            UpdateTimerText();
        }

        private void TimerEnded()
        {
            GameManager.Instance.OnTimerEnded();
        }

        public float GetTimeRemaining()
        {
            return _timeRemaining;
        }

        public bool IsRunning()
        {
            return _isRunning;
        }

        public bool IsPaused()
        {
            return _isPaused;
        }

        public void SetTimeRemaining(Level level)
        {
            _timeRemaining = level == Level.Easy ? 240f : level == Level.Medium ? 180f : 120f;
        }
    }
}
