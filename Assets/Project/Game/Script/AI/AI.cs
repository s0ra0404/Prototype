using System.Collections;
using Player;
using UnityEngine;
using StateMachine;
using UniRx;
using UnityEditor;
using UnityEngine.AI;
using Zenject;

namespace AI
{
    public class AI : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Transform _player;
        [SerializeField] private Transform[] _patrolPoints;
        [SerializeField] private LayerMask _playerLayer;
        [SerializeField] private float _searchDistance = 10f;
        [SerializeField] private int _seed;
        [SerializeField] private float _maxWaitTime = 6f,_minWaitTime = 4f;
        
        private StateMachine<AI> _stateMachine;
        
        [Inject] private readonly ISearchService _searchService;
        
        // プロパティ
        public NavMeshAgent Agent => _agent;
        public Transform Player => _player;
        public Transform[] PatrolPoints => _patrolPoints;
        
        public int CurrentPatrolIndex { get; private set; }
        
        public Vector3 SearchPoints { get; private set; }

        private void Awake()
        {
            _stateMachine = new StateMachine<AI>(this);
            _stateMachine.Add<IdleState>((int) AIState.Idle);
            _stateMachine.Add<PatrolState>((int) AIState.Patrol);
            _stateMachine.Add<SearchState>((int) AIState.Search);
            _stateMachine.Add<TrackingState>((int) AIState.Tracking);
            
            SetNearestPatrolPoint();
            _stateMachine.Start((int) AIState.Patrol);
            
            StartCoroutine(Search());
        }
        
        private void Update() => _stateMachine.Update();
        private void FixedUpdate() => _stateMachine.FixedUpdate();

        private IEnumerator Search()
        {
            while (this)
            {
                if (Random.Range(0, _seed) == 0 && _stateMachine.GetCurrentStateId() == (int)AIState.Patrol)
                {
                    Debug.Log("停止");
                    _stateMachine.ChangeState((int) AIState.Idle);
                    yield return new WaitForSeconds(Random.Range(_minWaitTime, _maxWaitTime));
                    SetNearestPatrolPoint();
                    _stateMachine.ChangeState((int) AIState.Patrol);
                }
                // プレイヤーがAIの検知範囲内にいるか
                if (!_searchService.IsPlayerInRange(transform.position, _player.position, _searchDistance))
                {
                    if (_stateMachine.GetCurrentStateId() == (int) AIState.Tracking)
                    {
                        Debug.Log("再巡回");
                        // 見失ったら巡回状態へ移行
                        SetNearestPatrolPoint();
                        _stateMachine.ChangeState((int) AIState.Patrol);
                    }
                    else
                    {
                        yield return null; continue;
                    }
                }

                // ノイズ検知 足音(ノイズ)を検知したら探索状態へ移行
                if (_stateMachine.GetCurrentStateId() != (int) AIState.Search && !_searchService.SearchPlayerNoise(NoiseType.Silent))
                {
                    SearchPoints = _player.position;
                    Debug.Log("ノイズ検知");
                    _stateMachine.ChangeState((int) AIState.Search);
                }
            
                Vector3 direction = (_player.position - transform.position).normalized;
                // 視覚検知 プレイヤーを発見したら追跡状態へ移行
                if (_stateMachine.GetCurrentStateId() != (int) AIState.Tracking && _searchService.CanSeePlayer(transform.position, direction, _searchDistance, _playerLayer))
                {
                    Debug.Log("プレイヤー発見");
                    _stateMachine.ChangeState((int) AIState.Tracking);
                }

                yield return null;
            }
        }
        
        
        // 最も近い巡回ポイントを設定
        private void SetNearestPatrolPoint()
        {
            var minDist = float.MaxValue;
            
            for (var i = 0; i < _patrolPoints.Length; i++)
            {
                var pos = transform.position;
                
                if (minDist > (_patrolPoints[i].position - pos).magnitude)
                {
                    minDist = (_patrolPoints[i].position - pos).magnitude;
                    CurrentPatrolIndex = i;
                }
            }
        }
        
        private void OnDrawGizmos()
        {
            for (var i = 0; i < _patrolPoints.Length; i++)
            {
                Gizmos.color = new Color(1,0,0,1);
                Gizmos.DrawSphere(_patrolPoints[i].position, 0.2f);

                Handles.Label(_patrolPoints[i].position + Vector3.forward, i.ToString(), new GUIStyle());
            }
            Gizmos.color = new Color(0,0,1,0.5f);
            Gizmos.DrawSphere(transform.position, _searchDistance);
        }
    }
}