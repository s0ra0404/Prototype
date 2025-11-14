using System;
using UniRx;
using UnityEngine.InputSystem;

public static class InputActionRxExtensions
{
    /// <summary>
    /// InputAction.performed を Observable に変換します。
    /// </summary>
    public static IObservable<InputAction.CallbackContext> PerformedAsObservable(this InputAction action)
    {
        return Observable.Create<InputAction.CallbackContext>(observer =>
        {
            // ハンドラを登録
            Action<InputAction.CallbackContext> handler = observer.OnNext;

            action.performed += handler;

            // 解除処理を返す
            return Disposable.Create(() =>
            {
                action.performed -= handler;
            });
        });
    }

    /// <summary>
    /// InputAction.started を Observable に変換します。
    /// </summary>
    public static IObservable<InputAction.CallbackContext> StartedAsObservable(this InputAction action)
    {
        return Observable.Create<InputAction.CallbackContext>(observer =>
        {
            Action<InputAction.CallbackContext> handler = observer.OnNext;

            action.started += handler;

            return Disposable.Create(() =>
            {
                action.started -= handler;
            });
        });
    }

    /// <summary>
    /// InputAction.canceled を Observable に変換します。
    /// </summary>
    public static IObservable<InputAction.CallbackContext> CanceledAsObservable(this InputAction action)
    {
        return Observable.Create<InputAction.CallbackContext>(observer =>
        {
            Action<InputAction.CallbackContext> handler = observer.OnNext;

            action.canceled += handler;

            return Disposable.Create(() =>
            {
                action.canceled -= handler;
            });
        });
    }
}