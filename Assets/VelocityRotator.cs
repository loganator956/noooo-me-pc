using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityRotator : MonoBehaviour
{
    private Vector3 _lastPos = Vector3.zero;
    public float MaxTurnAngle = 360;

    private void FixedUpdate()
    {
        Vector3 pos = transform.position;
        pos.y = 0;

        Vector3 delta = pos - _lastPos;

        Vector3 yLessForward = transform.forward;
        yLessForward.y = 0;
        yLessForward = yLessForward.normalized;

        float angle = Vector3.SignedAngle(yLessForward, delta.normalized, Vector3.up);

        transform.Rotate(transform.up, Mathf.Clamp(angle, -MaxTurnAngle, MaxTurnAngle) * Time.fixedDeltaTime * 4);

        _lastPos = pos;
    }
}
