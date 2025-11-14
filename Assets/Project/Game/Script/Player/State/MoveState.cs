using StateMachine;
using UnityEngine;

namespace Player
{
    public class MoveState : StateMachine<PlayerController>.StateBase
    {
        private Vector3 _inputDir = Vector3.zero;
        
        public override void Enter()
        {
        }
        
        public override void Update()
        {
        }
        
        public override void FixedUpdate()
        {
            var input = Owner.MoveInput;
            if (input == Vector2.zero) return;

            var rb = Owner.Rigidbody;
            var cam = Owner.Camera;

            // --- カメラの水平向き(Y軸のみ)を取得 ---
            Vector3 forward = cam.transform.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3 right = cam.transform.right;
            right.y = 0f;
            right.Normalize();

            // --- 入力方向をカメラ基準で変換 ---
            Vector3 moveDir = (forward * input.y + right * input.x).normalized;

            // --- 向きをカメラのY軸に合わせる ---
            if (forward != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(forward);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, 10f * Time.fixedDeltaTime));
            }

            // --- 移動 ---
            Vector3 move = moveDir * Owner.MoveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);
            
        }
        
        public override void Exit()
        {

        }
    }
}