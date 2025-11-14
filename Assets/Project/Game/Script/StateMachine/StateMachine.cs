using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// ステートマシンクラス
    /// </summary>
    public class StateMachine<TOwner>
    {
        /// <summary>
        /// ステート基底クラス
        /// 各ステートクラスはこのクラスを継承する
        /// </summary>
        public abstract class StateBase
        {
            public StateMachine<TOwner> StateMachine;
            protected TOwner Owner => StateMachine.Owner;

            public virtual void Enter() { }
            public virtual void Update() { }
            public virtual void FixedUpdate() { }
            public virtual void Exit() { }
        }
        private TOwner Owner { get; }
        private StateBase _currentState; // 現在のステート
        private StateBase _prevState;    // 前のステート
        private readonly Dictionary<int, StateBase> _states = new Dictionary<int, StateBase>(); // 全てのステート定義

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="owner">StateMachineを使用するOwner</param>
        public StateMachine(TOwner owner)
        {
            Owner = owner;
        }

        /// <summary>
        /// ステート定義登録
        /// ステートマシン初期化後にこのメソッドを呼ぶ
        /// </summary>
        /// <param name="stateId">ステートID</param>
        /// <typeparam name="T">ステート型</typeparam>
        public void Add<T>(int stateId) where T : StateBase, new()
        {
            if (_states.ContainsKey(stateId))
            {
                Debug.LogError("already register stateId!! : " + stateId);
                return;
            }
            // ステート定義を登録
            var newState = new T
            {
                StateMachine = this
            };
            _states.Add(stateId, newState);
        }
        
        /// <summary>
        /// 現在のステートのIDを返します
        /// </summary>
        /// <returns></returns>
        public int GetCurrentStateId()
        {
            foreach (var pair in _states)
            {
                if (pair.Value == _currentState)
                {
                    var stateId = pair.Key;
                    return stateId;
                }
            }
            Debug.LogError($"State {_currentState.GetType().Name} does not exist!! : " + _states.Count);
            return -1;
        }
        
        /// <summary>
        /// ステート開始処理
        /// </summary>
        /// <param name="stateId">ステートID</param>
        public void Start(int stateId)
        {
            if (!_states.TryGetValue(stateId, out var nextState))
            {
                Debug.LogError("not set stateId!! : " + stateId);
                return;
            }
            // 現在のステートに設定して処理を開始
            _currentState = nextState;
            _currentState.Enter();
        }
        
        /// <summary>
        /// 次のステートに切り替える
        /// </summary>
        /// <param name="stateId">切り替えるステートID</param>
        /// <param name="shouldSkipState">現在のステートと同じ場合にスキップするか : デフォルト スキップ</param>
        public void ChangeState(int stateId, bool shouldSkipState = true)
        {
            if (!_states.TryGetValue(stateId, out var nextState))
            {
                Debug.LogError("not set stateId!! : " + stateId);
                return;
            }

            // ステートが同じ場合は何もしない
            if (shouldSkipState && _currentState == nextState) return;
            
            // 前のステートを保持
            _prevState = _currentState;
            // ステートを切り替える
            _currentState.Exit();
            _currentState = nextState;
            _currentState.Enter();
        }
        
        /// <summary>
        /// 現在のステートがあれば、そのステートのUpdate呼ぶ
        /// </summary>
        public void Update() => _currentState?.Update();
        
        /// <summary>
        /// 現在のステートがあれば、そのステートのFixedUpdate呼ぶ
        /// </summary>
        public void FixedUpdate() => _currentState?.FixedUpdate();
    }
}
