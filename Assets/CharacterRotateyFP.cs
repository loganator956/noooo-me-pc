using CharacterSystems.Movement;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class CharacterRotateyFP : MonoBehaviour
{
    private Vector2 _lookInputs;

    public float LookSpeedHorizontal = 5f;
    public float LookSpeedVertical = 5f;

    private float _vAngle = 0f;

    private Transform _cameraTransform;
    private CharacterMovement3D _movement;

    private void Awake()
    {
        _cameraTransform = GetComponentInChildren<Camera>().transform;
        _movement = GetComponent<CharacterMovement3D>();
    }

    // Update is called once per frame
    void Update()
    {
        _vAngle += _lookInputs.y * LookSpeedVertical * Time.deltaTime;
        _vAngle = Mathf.Clamp(_vAngle, -89f, 89f);

        _cameraTransform.localRotation = Quaternion.Euler(_vAngle, 0, 0);

        transform.Rotate(0, _lookInputs.x * Time.deltaTime * LookSpeedHorizontal, 0);

        _movement.CameraForward = transform.forward;
    }

    private void OnLook(InputValue inputValue)
    {
        _lookInputs = inputValue.Get<Vector2>();
    }
}
