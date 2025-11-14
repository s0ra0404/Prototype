using System;
using Cysharp.Threading.Tasks;
using StateMachine;

namespace AI
{
    public class PatrolState : StateMachine<AI>.StateBase
    {
        private int _currentPatrolIndex = 0;
        
        public override void Enter()
        {
            _currentPatrolIndex = Owner.CurrentPatrolIndex;
        }

        public override void Update()
        {
            // 現在の巡回ポイントへ移動
            Owner.Agent.SetDestination(Owner.PatrolPoints[_currentPatrolIndex].position);

            var dist = (Owner.PatrolPoints[_currentPatrolIndex].position - Owner.transform.position).magnitude;
            
            // 巡回ポイントに到着したら次のポイントへ
            if (!(dist - 0.2 <= Owner.Agent.stoppingDistance)) return;
            
            // 次の巡回ポイントを設定
            if (_currentPatrolIndex >= Owner.PatrolPoints.Length - 1)
            {
                _currentPatrolIndex = 0;
            }
            else
            {
                _currentPatrolIndex++;
            }
        }
    }
}