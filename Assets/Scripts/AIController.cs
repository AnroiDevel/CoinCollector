using System.Collections;
using UnityEngine;

namespace CoinCollector
{
    public abstract class AIController : PlayerBaseController
    {
        protected GameObject _target; // Цель, к которой стремится ИИ
        protected float _raycastDistance = 5.0f;
        protected float _avoidanceStrength = 5.0f;
        protected float _stuckTimeThreshold = 1.0f; // Время, после которого считаем, что ИИ застрял
        protected float _stuckTime = 0.0f;
        protected bool _isStuck = false;
        protected Vector2 _lastValidDirection = Vector2.zero;
        protected float _averageSpeed = 0.0f;
        protected Vector2 _lastPosition;
        protected LayerMask _obstacleLayer;
        protected LayerMask _dogLayer; // Новый слой для собаки
        protected LayerMask _canLayer; // Новый слой для консервной банки
        protected float _targetCheckInterval = 1.0f; // Период проверки текущей цели
        protected float _timeSinceLastTargetCheck = 0.0f;
        protected float _edgeAvoidanceDistance = 0.5f; // Расстояние до края экрана, когда начинаем учитывать edgeAvoidance

        protected override void Start()
        {
            base.Start();
            _obstacleLayer = LayerMask.GetMask("Obstacle");
            _dogLayer = LayerMask.GetMask("Dog"); // Убедитесь, что собака имеет слой "Dog"
            _canLayer = LayerMask.GetMask("Can"); // Убедитесь, что консервная банка имеет слой "Can"
            _timeSinceLastTargetCheck = _targetCheckInterval; // Начинаем сразу с проверки
            _lastPosition = transform.position;
        }

        protected override void ProcessInputs()
        {
            if(!GameManager.Instance.GameStarted)
                return;

            _timeSinceLastTargetCheck += Time.deltaTime;
            if(_timeSinceLastTargetCheck >= _targetCheckInterval || _target == null)
            {
                _target = FindTarget();
                _timeSinceLastTargetCheck = 0.0f;
            }

            if(_target != null)
            {
                Vector2 direction = (_target.transform.position - transform.position).normalized;
                if(!_isStuck)
                {
                    _moveDirection = AvoidObstacles(direction);
                }
                else
                {
                    if(_stuckTime >= _stuckTimeThreshold)
                    {
                        _target = FindTarget();
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

        protected virtual void CheckIfStuck()
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

        protected virtual Vector2 AvoidObstacles(Vector2 direction)
        {
            Vector2 avoidance = direction;

            // Проверка всех направлений для обнаружения препятствий
            for(int i = 0; i < 360; i += 15)
            {
                Vector2 rayDirection = Quaternion.Euler(0, 0, i) * direction;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, _raycastDistance, _obstacleLayer | _dogLayer | _canLayer);

                if(hit.collider != null)
                {
                    if(hit.collider.CompareTag("Obstacle") || hit.collider.CompareTag("Dog") || hit.collider.CompareTag("Can"))
                    {
                        Debug.DrawRay(transform.position, rayDirection * _raycastDistance, Color.red);
                        _isStuck = true;
                        _stuckTime += Time.deltaTime;

                        // Поиск направления обхода
                        Vector2 obstacleDirection = (hit.point - (Vector2)transform.position).normalized;
                        Vector2 avoidanceDirection = Vector2.Perpendicular(obstacleDirection);
                        avoidance += avoidanceDirection * _avoidanceStrength;
                        _lastValidDirection = avoidanceDirection;
                    }
                }
                else
                {
                    Debug.DrawRay(transform.position, rayDirection * _raycastDistance, Color.green);
                }
            }

            if(!_isStuck)
            {
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

        protected abstract GameObject FindTarget();

        protected override void FixedUpdate()
        {
            Move();
            RestrictMovement();
        }

        protected override void Update()
        {
            ProcessInputs();
        }
    }
}
