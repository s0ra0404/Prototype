using StateMachine;
using UnityEngine;

namespace AI
{
    public class IdleState : StateMachine<AI>.StateBase
    {
        public override void Enter()
        {
            Owner.Agent.isStopped = true;
        }

        public override void Exit()
        {
            Owner.Agent.isStopped = false;
        }
    }
}