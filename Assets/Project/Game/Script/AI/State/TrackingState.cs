using StateMachine;

namespace AI
{
    /// <summary>
    /// プレイヤーを追跡する状態
    /// </summary>
    public class TrackingState : StateMachine<AI>.StateBase
    {
        public override void Enter()
        {
            
        }

        public override void Update()
        {
            // プレイヤーの位置を目的地に設定
            Owner.Agent.SetDestination(Owner.Player.position);
        }
        
        public override void FixedUpdate()
        {
            
        }

        public override void Exit()
        {
            
        }
    }
}