using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private Rigidbody _rb;

    private float KillTimer = 10f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_rb.velocity.magnitude > 0.2f)
        {
            transform.LookAt(transform.position + _rb.velocity);
        }
        else if (_rb.velocity.magnitude < 0.05f)
        {
            KillTimer -= Time.deltaTime;
            if (KillTimer < 0)
            {
                Destroy(gameObject, 2f);
                transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, Mathf.Clamp01(Mathf.Abs(KillTimer)));
            }
        }
    }
}
