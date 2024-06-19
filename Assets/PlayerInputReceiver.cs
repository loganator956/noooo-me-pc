using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReceiver : MonoBehaviour
{
    private PlayerHand _hand;

    private void Awake()
    {
        _hand = GetComponentInChildren<PlayerHand>();
    }

    private void OnFire()
    {
        if (_hand.HeldTool != null)
        {
            _hand.HeldTool.UseTool();
        }
    }
}
