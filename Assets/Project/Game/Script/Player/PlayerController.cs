using UnityEngine;
using Zenject;
using StateMachine;
using UniRx;

namespace Player
{
    public enum PlayerState
    {
        Idle,
        Move
    }
    
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private UnityEngine.Camera _camera;
        [SerializeField] private float _speed;
        [SerializeField] private float _sprintMultiply;
        [SerializeField] private float _sneakMultiply;
        
        [Inject] private readonly IPlayerInput _playerInput;
        [Inject] private readonly IWriteCurrentNoise _writeCurrentNoise;
        
        private StateMachine<PlayerController> _stateMachine;
        
        public Rigidbody Rigidbody => _rigidbody;
        public UnityEngine.Camera Camera => _camera;
        public float MoveSpeed { get; private set; }
        public Vector2 MoveInput { get; private set; }

        private void Awake()
        {
            _stateMachine = new StateMachine<PlayerController>(this);
            
            // ステートの初期化
            _stateMachine.Add<IdleState>((int)PlayerState.Idle);
            _stateMachine.Add<MoveState>((int)PlayerState.Move);

            MoveSpeed = _speed;
        }

        private void Start()
        {
            // 初期ステートを設定
            _stateMachine.Start((int) PlayerState.Idle);

            ReadStream();
        }
        
        private void Update()
        {
            _stateMachine.Update();
            CheckNoise();
        }

        private void FixedUpdate() => _stateMachine.FixedUpdate();

        private void ReadStream()
        {
            // 移動入力を購読します。
            _playerInput.MoveStream
                .Subscribe(input =>
                {
                    MoveInput = input;
                    
                    // 入力があるなら
                    if (input != Vector2.zero)
                    {
                        _stateMachine.ChangeState((int) PlayerState.Move);
                    }
                    else if(input == Vector2.zero)
                    {
                        _stateMachine.ChangeState((int) PlayerState.Idle);
                    }
                })
                .AddTo(this);
            
            // スプリント入力を購読します。
            _playerInput.SprintStream
                .Subscribe(isSprint =>
                {
                    // 入力があるなら
                    if (isSprint)
                    {
                        MoveSpeed = _speed * _sprintMultiply;
                    }
                    // 入力が無ければ
                    else
                    {
                        MoveSpeed = _speed;
                    }
                })
                .AddTo(this);
            
            _playerInput.SneakSubject
                .Subscribe(isSneak =>
                {
                    // 入力があるなら
                    if (isSneak)
                    {
                        MoveSpeed = _speed * _sneakMultiply;
                    }
                    // 入力が無ければ
                    else
                    {
                        MoveSpeed = _speed;
                    }
                })
                .AddTo(this);
        }

        private void CheckNoise()
        {
            if (_stateMachine.GetCurrentStateId() == (int)PlayerState.Idle)
            {
                _writeCurrentNoise.SetNoiseType(NoiseType.Silent);
            }
            else if (_stateMachine.GetCurrentStateId() == (int)PlayerState.Move)
            {
                _writeCurrentNoise.SetNoiseType(MoveSpeed > _speed ? NoiseType.Loud : NoiseType.Silent);
                if(!(MoveSpeed > _speed) && !(MoveSpeed < _speed)) _writeCurrentNoise.SetNoiseType(NoiseType.Normal);
            }
        }
    }
}