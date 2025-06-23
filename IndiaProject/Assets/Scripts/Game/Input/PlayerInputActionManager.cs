using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputActionManager : MonoBehaviour
{
    private PlayerController actions;
    Action jumpAction;
    Action<float, float> MoveAction;
    private void JumpFunc(InputAction.CallbackContext context)
    {
        jumpAction?.Invoke();
        Debug.Log("Jump");
    }
    private void MoveFunc(InputAction.CallbackContext context)
    {
        var moveVector = context.ReadValue<Vector2>();
        MoveAction?.Invoke(moveVector.x, moveVector.y);
        Debug.Log("移动");
    }
    #region 生命周期
    void OnEnable()
    {
        actions = new();
        actions.Player.Enable();
    }

    void Start()
    {
        actions.Player.Jump.performed += JumpFunc;
        actions.Player.Move.performed += MoveFunc;
    }

    void Update()
    {

    }

    void OnDisable()
    {
        actions.Player.Disable();
        actions.Player.Jump.performed -= JumpFunc;
        actions.Player.Move.performed -= MoveFunc;
        jumpAction = null;
        MoveAction = null;
    }
    #endregion
}
