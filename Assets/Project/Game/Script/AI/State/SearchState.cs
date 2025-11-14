using StateMachine;
using UnityEngine;

namespace AI
{
    public class SearchState : StateMachine<AI>.StateBase
    {
        private Vector3 _currentPatrolPoint;
        
        public override void Enter()
        {
            _currentPatrolPoint = Owner.SearchPoints;
        }

        public override void Update()
        {
            Owner.Agent.SetDestination(_currentPatrolPoint);
            
            var dist = (_currentPatrolPoint - Owner.transform.position).magnitude;
            
            // 探索ポイントに到着したら通常巡回へ
            if (dist <= Owner.Agent.stoppingDistance)
            {
                StateMachine.ChangeState((int) AIState.Patrol);
            }
        }
        
        public override void FixedUpdate()
        {
            
        }

        public override void Exit()
        {
            
        }
    }
}