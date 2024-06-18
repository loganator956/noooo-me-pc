using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

namespace CharacterSystems.Movement
{
    [RequireComponent(typeof(CharacterMovement3D))]
    public class InputDirectionCharacterRotation3D : MonoBehaviour
    {
        [Header("Properties")]
        public float MaxTurnSpeed = 720.0f;

        private CharacterMovement3D _movement;

        private Vector2 _inputs;

        private void Awake()
        {
            _movement = GetComponent<CharacterMovement3D>();
        }

        private void Update()
        {
            if (_inputs == Vector2.zero) return;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(InputToWorldDirection(_inputs), Vector3.up), MaxTurnSpeed * Time.deltaTime);
        }

        public void OnMove(InputValue value)
        {
            _inputs = value.Get<Vector2>();
        }

        // TODO: Put this method into another class or something? 
        private Vector3 InputToWorldDirection(Vector2 input)
        {
            Vector3 forward = _movement.CameraForward * input.y;
            Vector3 right = Vector3.Cross(Vector3.up, _movement.CameraForward) * input.x;
            return forward + right;
        }
    } 
}
