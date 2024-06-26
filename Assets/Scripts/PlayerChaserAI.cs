using UnityEngine;

namespace CoinCollector
{
    public class PlayerChaserAI : AIController
    {
        private Vector2 _startPosition;
        private bool _returningToStart = false;
        private GameObject _initialTargetPlayer;
        [SerializeField] private Level _hardLevel = Level.Easy;

        protected override void Start()
        {
            base.Start();
            _startPosition = transform.position; // Сохраняем стартовую позицию
            _initialTargetPlayer = FindClosestPlayer(); // Изначально находим ближайшего игрока
            SetSpeedLevel(_hardLevel);
        }

        protected override void ProcessInputs()
        {
            if(!GameManager.Instance.GameStarted)
                return;

            if(_returningToStart)
            {
                // Возвращаемся на стартовую позицию
                Vector2 direction = (_startPosition - (Vector2)transform.position).normalized;
                _moveDirection = AvoidObstacles(direction);

                // Если достигли стартовой позиции, начинаем преследование второго игрока
                if(Vector2.Distance(transform.position, _startPosition) < 1f)
                {
                    _returningToStart = false;
                    _initialTargetPlayer = FindNextPlayer(_initialTargetPlayer);
                }
            }
            else
            {
                // Преследуем текущего игрока
                if(_initialTargetPlayer != null)
                {
                    Vector2 direction = (_initialTargetPlayer.transform.position - transform.position).normalized;
                    _moveDirection = direction; // Устанавливаем направление без AvoidObstacles
                }
                else
                {
                    _initialTargetPlayer = FindClosestPlayer();
                }
            }

            CheckIfStuck();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.gameObject.CompareTag("Player"))
            {
                if(collision.gameObject.TryGetComponent<PlayerHealth>(out var playerHealth))
                {
                    playerHealth.TakeDamage();
                }

                _returningToStart = true;
            }
        }

        protected override GameObject FindTarget()
        {
            return FindClosestPlayer();
        }

        private GameObject FindClosestPlayer()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            GameObject closestPlayer = null;
            float minDistance = Mathf.Infinity;
            Vector2 currentPosition = transform.position;

            foreach(GameObject player in players)
            {
                if(player != gameObject) // Игнорируем самого себя
                {
                    float distance = Vector2.Distance(currentPosition, player.transform.position);
                    if(distance < minDistance)
                    {
                        closestPlayer = player;
                        minDistance = distance;
                    }
                }
            }

            return closestPlayer;
        }

        private GameObject FindNextPlayer(GameObject currentPlayer)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach(GameObject player in players)
            {
                if(player != currentPlayer && player != gameObject) // Игнорируем текущего игрока и самого себя
                {
                    return player;
                }
            }
            return FindClosestPlayer();
        }
    }
}
