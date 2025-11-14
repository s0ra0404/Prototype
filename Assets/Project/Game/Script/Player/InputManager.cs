using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public interface IPlayerInput
    {
        IObservable<Vector2> MoveStream { get; }
        IObservable<bool> SprintStream { get; }
        IObservable<bool> SneakSubject { get; }
        IObservable<Unit> InteractionsStream { get; }
    }

    public class InputManager : MonoBehaviour, IPlayerInput
    {
        [SerializeField] private InputAction _moveAction;
        [SerializeField] private InputAction _sprintAction;
        [SerializeField] private InputAction _sneakAction;
        [SerializeField] private InputAction _interactionsAction;
    
        // プレイヤーの移動入力を通知するSubject
        private readonly Subject<Vector2> _moveSubject = new();
        // プレイヤーのスプリント入力を通知するSubject
        private readonly Subject<bool> _sprintSubject = new();
        // プレイヤーのスネーク入力を通知するSubject
        private readonly Subject<bool> _sneakSubject = new();
        // プレイヤーのインタラクション入力を通知するSubject
        private readonly Subject<Unit> _interactionsSubject = new();

        // 外部からの読み取り専用プロパティ
        public IObservable<Vector2> MoveStream => _moveSubject;
        public IObservable<bool> SprintStream => _sprintSubject;
        public IObservable<bool> SneakSubject => _sneakSubject;
        public IObservable<Unit> InteractionsStream => _interactionsSubject;

        private void OnEnable()
        {
            _moveAction.Enable();
            _sprintAction.Enable();
            _sneakAction.Enable();
            _interactionsAction.Enable();
        }

        private void OnDisable()
        {
            _moveAction.Disable();
            _sprintAction.Disable();
            _sneakAction.Disable();
            _interactionsAction.Disable();
        }
        
        private void Start()
        {
            // 移動入力を購読します。
            _moveAction
                .PerformedAsObservable()
                .Subscribe(value =>
                {
                    // 通知
                    var input = value.ReadValue<Vector2>();
                    _moveSubject.OnNext(input);
                })
                .AddTo(this);
            
            // 移動入力のキャンセルを購読します。
            _moveAction
                .CanceledAsObservable()
                .Subscribe(value =>
                {
                    // 通知
                    _moveSubject.OnNext(Vector2.zero);
                })
                .AddTo(this);

            // スプリント入力の開始を購読します。
            _sprintAction
                .StartedAsObservable()
                .Subscribe(_ =>
                {
                    // 通知
                    _sprintSubject.OnNext(true);
                })
                .AddTo(this);
        
            // スプリント入力の終了を購読します。
            _sprintAction
                .CanceledAsObservable()
                .Subscribe(_ =>
                {
                    // 通知
                    _sprintSubject.OnNext(false);
                })
                .AddTo(this);
            
            // スネーク入力の開始を購読します。
            _sneakAction
                .StartedAsObservable()
                .Subscribe(_ =>
                {
                    // 通知
                    _sneakSubject.OnNext(true);
                })
                .AddTo(this);
        
            // スネーク入力の終了を購読します。
            _sneakAction
                .CanceledAsObservable()
                .Subscribe(_ =>
                {
                    // 通知
                    _sneakSubject.OnNext(false);
                })
                .AddTo(this);
        
            // インタラクション入力を購読します。
            _interactionsAction
                .PerformedAsObservable()
                .Subscribe(_ =>
                {
                    // 通知
                    _interactionsSubject.OnNext(Unit.Default);
                })
                .AddTo(this);
        }
    }
}


