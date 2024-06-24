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
    private AnimationCurve _floatyRotateTorqueMultiplierCurve = AnimationCurve.EaseInOut(0f, 0.25f, 1f, 1f);

    public const float FLOATY_FORCE = 1.5f;
    public float HardMaxDistance = 1f;

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

            if (d.magnitude < 0.2f)
            {
                _heldItemRB.velocity *= 0.999f;
            }


            // rotation
            Vector3 fwd = Vector3.Cross(_heldItemRB.transform.forward, transform.forward);

            _heldItemRB.AddTorque(_floatyRotateTorqueMultiplierCurve.Evaluate(fwd.magnitude) * fwd * 5); // looking forward (does rotation on x and y local axis)
            _heldItemRB.AddTorque(Vector3.Cross(_heldItemRB.transform.up, transform.up) * 5); // keeps gun handle down (does rotation on z)

            Vector3 forwardsVelocity = transform.forward * Vector3.Dot(_heldItemRB.velocity, transform.forward);
            Vector3 rightVelocity = transform.right * Vector3.Dot(_heldItemRB.velocity, transform.right);

            Vector3 forwardsDelta = transform.forward * Vector3.Dot(d, transform.forward);
            Vector3 rightDelta = transform.right * Vector3.Dot(d, transform.right);

            Debug.Log(forwardsVelocity.z);
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
        if (_heldItem == null && other != null && other.attachedRigidbody != null)
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
}
