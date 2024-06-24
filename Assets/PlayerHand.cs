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

    private AnimationCurve _floatyForceMultiplierCurve = AnimationCurve.EaseInOut(0f, 0.1f, 0.2f, 0.3f);

    public const float FLOATY_FORCE = 1.5f;

    private void Awake()
    {
        _trigger = GetComponent<Collider>();
        _floatyForceMultiplierCurve.AddKey(0.6f, 0.7f);
        _floatyForceMultiplierCurve.AddKey(1f, 1.1f);
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
            Debug.Log(Vector3.Cross(_heldItemRB.transform.forward, transform.forward).magnitude);
            _heldItemRB.AddTorque(Vector3.Cross(_heldItemRB.transform.forward,transform.forward) * 2);
            _heldItemRB.AddTorque(Vector3.Cross(_heldItemRB.transform.up, transform.up));

            //_heldItem.position = transform.position - d;

            
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
