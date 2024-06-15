using UnityEngine;

namespace CoinCollector
{
    public class ImprovedAIController : PlayerBaseController
    {
        private GameObject _targetCoin;
        private float _raycastDistance = 5.0f;
        private float _avoidanceStrength = 5.0f;
        private float _stuckTimeThreshold = 1.0f; // Время, после которого считаем, что ИИ застрял
        private float _stuckTime = 0.0f;
        private bool _isStuck = false;
        private Vector2 _lastValidDirection = Vector2.zero;
        private float _averageSpeed = 0.0f;
        private Vector2 _lastPosition;
        private LayerMask _obstacleLayer;
        private LayerMask _wallLayer;
        private float _targetCheckInterval = 1.0f; // Период проверки текущей цели
        private float _timeSinceLastTargetCheck = 0.0f;
        private float _edgeAvoidanceDistance = 0.5f; // Расстояние до края экрана, когда начинаем учитывать edgeAvoidance

        protected override void Start()
        {
            base.Start();
            _obstacleLayer = LayerMask.GetMask("Obstacle");
            _wallLayer = LayerMask.GetMask("Wall");
            _timeSinceLastTargetCheck = _targetCheckInterval; // Начинаем сразу с проверки
            _lastPosition = transform.position;
        }

        protected override void ProcessInputs()
        {
            if(!GameManager.Instance.GameStarted)
                return;

            _timeSinceLastTargetCheck += Time.deltaTime;
            if(_timeSinceLastTargetCheck >= _targetCheckInterval || _targetCoin == null)
            {
                _targetCoin = FindClosestCoin();
                _timeSinceLastTargetCheck = 0.0f;
            }

            if(_targetCoin != null)
            {
                Vector2 direction = (_targetCoin.transform.position - transform.position).normalized;
                if(!_isStuck)
                {
                    _moveDirection = AvoidObstacles(direction);
                }
                else
                {
                    if(_stuckTime >= _stuckTimeThreshold)
                    {
                        _targetCoin = FindClosestVisibleCoin();
                        _stuckTime = 0.0f;
                        _isStuck = false;
                    }
                    else
                    {
                        _moveDirection = _lastValidDirection.normalized;
                        _stuckTime += Time.deltaTime;
                    }
                }
            }
            else
            {
                _moveDirection = Vector2.zero;
            }

            CheckIfStuck();
        }

        private void CheckIfStuck()
        {
            float currentSpeed = ((Vector2)transform.position - _lastPosition).magnitude / Time.deltaTime;
            _averageSpeed = (_averageSpeed + currentSpeed) / 2.0f;
            _lastPosition = transform.position;

            if(currentSpeed < _averageSpeed * 0.3f) // Считаем застревание, если скорость меньше половины средней скорости
            {
                _isStuck = true;
            }
            else
            {
                _isStuck = false;
                _stuckTime = 0.0f;
            }
        }

        private Vector2 AvoidObstacles(Vector2 direction)
        {
            Vector2 avoidance = direction;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, _raycastDistance, _obstacleLayer | _wallLayer);
            if(hit.collider != null && (hit.collider.CompareTag("Obstacle") || hit.collider.CompareTag("Wall")))
            {
                _isStuck = true;
                _stuckTime += Time.deltaTime;

                // Поиск направления обхода
                Vector2 obstacleDirection = (hit.point - (Vector2)transform.position).normalized;
                Vector2 avoidanceDirection = Vector2.Perpendicular(obstacleDirection);
                avoidance = avoidanceDirection * _avoidanceStrength;
                _lastValidDirection = avoidanceDirection;
            }
            else
            {
                _isStuck = false;
                _stuckTime = 0.0f;
                _lastValidDirection = direction;
            }

            // Обход краев экрана
            Vector3 viewportPos = _mainCamera.WorldToViewportPoint(transform.position);
            Vector2 edgeAvoidance = Vector2.zero;
            if(viewportPos.x <= _edgeAvoidanceDistance)
            {
                edgeAvoidance += Vector2.right;
            }
            else if(viewportPos.x >= 1.0f - _edgeAvoidanceDistance)
            {
                edgeAvoidance += Vector2.left;
            }
            if(viewportPos.y <= _edgeAvoidanceDistance)
            {
                edgeAvoidance += Vector2.up;
            }
            else if(viewportPos.y >= 1.0f - _edgeAvoidanceDistance)
            {
                edgeAvoidance += Vector2.down;
            }

            if(!_isStuck && edgeAvoidance.magnitude > 0)
            {
                avoidance += edgeAvoidance.normalized;
            }

            return avoidance.normalized;
        }

        private GameObject FindClosestVisibleCoin()
        {
            GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
            GameObject closestCoin = null;
            float minDistance = Mathf.Infinity;
            Vector2 currentPosition = transform.position;

            foreach(GameObject coin in coins)
            {
                Vector2 direction = (coin.transform.position - transform.position);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, _raycastDistance, _obstacleLayer | _wallLayer);
                if(hit.collider == null)
                {
                    float distance = Vector2.Distance(currentPosition, coin.transform.position);
                    if(distance < minDistance)
                    {
                        closestCoin = coin;
                        minDistance = distance;
                    }
                }
            }

            return closestCoin != null ? closestCoin : FindClosestCoin();
        }

        protected override void FixedUpdate()
        {
            Move();
            RestrictMovement();
        }

        protected override void Update()
        {
            ProcessInputs();
        }

        private GameObject FindClosestCoin()
        {
            GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
            GameObject closestCoin = null;
            float minDistance = Mathf.Infinity;
            Vector2 currentPosition = transform.position;

            foreach(GameObject coin in coins)
            {
                float distance = Vector2.Distance(currentPosition, coin.transform.position);
                if(distance < minDistance)
                {
                    closestCoin = coin;
                    minDistance = distance;
                }
            }

            return closestCoin;
        }
    }
}
