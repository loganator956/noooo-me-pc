using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    private Transform _heldItem = null;
    private Rigidbody _heldItemRB = null;
    private Pickupable _heldItemPick = null;
    public IUsableTool HeldTool { get; private set; }
    private Collider _trigger;

    private AnimationCurve _floatyForceMultiplierCurve = AnimationCurve.EaseInOut(0f, 0.5f, 1f, 1.5f);
    private AnimationCurve _floatyRotateTorqueMultiplierCurve = AnimationCurve.EaseInOut(0f, 0.001f, 1f, 2.5f);

    private AnimationCurve _floatyRotateTorqueDragCurve = AnimationCurve.EaseInOut(0f, 0.985f, 0.9f, 1f);
    private AnimationCurve _floatyForceDragCurve = AnimationCurve.EaseInOut(0f, 0.985f, 1f, 1f);

    public const float FLOATY_FORCE = 1.5f;
    public float HardMaxDistance = 1f;
    public float FlingForce = 150f;

    private void Awake()
    {
        _trigger = GetComponent<Collider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _trigger.enabled = _heldItem == null;
        if (_heldItem != null)
        {
            Vector3 d = transform.position - _heldItem.position;
            _heldItemRB.AddForce(d * FLOATY_FORCE * _heldItemPick.FloatyForce * _floatyForceMultiplierCurve.Evaluate(d.magnitude));
            _heldItemRB.useGravity = false;
            //_heldItemRB.MoveRotation(transform.rotation);
            Debug.Log(d.magnitude);
            _heldItemRB.velocity *= _floatyForceDragCurve.Evaluate(d.magnitude);


            // rotation
            Vector3 fwd = Vector3.Cross(_heldItemRB.transform.forward, transform.forward);
            Vector3 updown = Vector3.Cross(_heldItemRB.transform.up, transform.up);

            _heldItemRB.angularVelocity *= _floatyRotateTorqueDragCurve.Evaluate(Mathf.Max(fwd.magnitude, updown.magnitude));

            _heldItemRB.AddTorque(_floatyRotateTorqueMultiplierCurve.Evaluate(fwd.magnitude) * fwd * 2); // looking forward (does rotation on x and y local axis)
            _heldItemRB.AddTorque(_floatyRotateTorqueMultiplierCurve.Evaluate(updown.magnitude) * updown * 2); // keeps gun handle down (does rotation on z)

            Vector3 forwardsVelocity = transform.forward * Vector3.Dot(_heldItemRB.velocity, transform.forward);
            Vector3 rightVelocity = transform.right * Vector3.Dot(_heldItemRB.velocity, transform.right);

            Vector3 forwardsDelta = transform.forward * Vector3.Dot(d, transform.forward);
            Vector3 rightDelta = transform.right * Vector3.Dot(d, transform.right);

            if (forwardsVelocity.z > 0)
                forwardsDelta = Vector3.ClampMagnitude(forwardsDelta, HardMaxDistance * 1.2f);
            else
                forwardsDelta = Vector3.ClampMagnitude(forwardsDelta, HardMaxDistance * 0.5f);
            if (true || rightVelocity.x > 0)
                rightDelta = Vector3.ClampMagnitude(rightDelta, HardMaxDistance);

            Vector3 clampedDelta = forwardsDelta + rightDelta;
            _heldItem.position = transform.position - clampedDelta;

            if (HeldTool != null )
            {
                // TODO: Enable button prompts for using tool
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_heldItem == null && other != null && other.attachedRigidbody != null && other.attachedRigidbody != _blacklistedRB) // TODO: make it so cant pickup a flung gun
        {
            Pickupable pick = other.attachedRigidbody.transform.GetComponent<Pickupable>();
            if (pick != null)
            {
                pick.enabled = false;
                _heldItem = other.attachedRigidbody.transform;
                _heldItemPick = pick;
                _heldItemRB = other.attachedRigidbody;
                HeldTool = _heldItem.gameObject.GetComponent<IUsableTool>();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody != null && other.attachedRigidbody == _blacklistedRB)
            _blacklistedRB = null;
    }

    private Rigidbody _blacklistedRB = null;

    public void ChuckTool()
    {
        _heldItemPick.enabled = true;
        _heldItemRB.useGravity = true;
        _heldItemRB.AddForce(transform.forward * FlingForce, ForceMode.Impulse);
        _heldItemRB.AddTorque(_heldItemRB.GetAccumulatedTorque() * 2, ForceMode.Impulse);
        _heldItemRB.AddTorque(_heldItem.right * 20, ForceMode.Impulse);

        _blacklistedRB = _heldItemRB;

        _heldItem = null;
        _heldItemPick = null;
        _heldItemRB = null;
        HeldTool = null;
    }
}
