using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    private Transform _heldItem = null;
    private Rigidbody _heldItemRB = null;
    private Pickupable _heldItemPick = null;
    private Collider _trigger;

    private AnimationCurve _floatyForceMultiplierCurve = AnimationCurve.EaseInOut(0f, 0.1f, 0.6f, 1f);

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
            _heldItemRB.AddForce(d * _heldItemPick.FloatyForce * _floatyForceMultiplierCurve.Evaluate(d.magnitude));
            _heldItemRB.useGravity = false;
            _heldItemRB.MoveRotation(transform.rotation);
            Vector3.ClampMagnitude(d, 0.5f);
            _heldItem.position = transform.position - d;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.attachedRigidbody != null)
        {
            Pickupable pick = other.attachedRigidbody.transform.GetComponent<Pickupable>();
            if (pick != null)
            {
                pick.enabled = false;
                _heldItem = other.attachedRigidbody.transform;
                _heldItemPick = pick;
                _heldItemRB = other.attachedRigidbody;
            }
        }
    }
}
