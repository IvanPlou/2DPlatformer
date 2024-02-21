using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : BaseUnit
{
    [Header("Input")]
    [SerializeField] private string _movementButton = "Horizontal";
    [SerializeField] private string _jumpButton = "Jump";

   
    protected override void Update()
    {
        base.Update();
        SetMoveInput(Input.GetAxis(_movementButton));
        if(Input.GetButtonDown(_jumpButton))
        {
            TryJump();
        }
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            GameManager.Instance.PlayerDeath();
        }
    }

}
