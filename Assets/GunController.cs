using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour, IUsableTool
{
    public GameObject BulletPrefab;

    public float BulletForce = 5f;
    public Vector3 BulletStartPos = Vector3.zero;
    public Vector3 LocalBulletFireDirection = Vector3.forward;
    public Vector3 RecoilVector = -Vector3.right;
    public float RecoilForce = 10f;

    void IUsableTool.UseTool()
    {
        GameObject b = Instantiate(BulletPrefab);
        b.transform.position = transform.TransformPoint(BulletStartPos);
        Rigidbody rb = b.GetComponent<Rigidbody>();
        rb.AddForce(transform.TransformDirection(LocalBulletFireDirection) * BulletForce, ForceMode.Impulse);
        GetComponent<Rigidbody>().AddTorque(transform.TransformDirection(RecoilVector) * RecoilForce, ForceMode.Impulse);
    }

    // TODO: Add object oriented ammo (like reference the ammo box and take things out and player has to replace it??


    private void OnDrawGizmosSelected()
    {
        Vector3 bulletPoint = transform.TransformPoint(BulletStartPos);
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(bulletPoint, 0.075f);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(bulletPoint, transform.TransformPoint(BulletStartPos + LocalBulletFireDirection * 0.1f));
    }

    private void OnValidate()
    {
        LocalBulletFireDirection = LocalBulletFireDirection.normalized;
    }
}
