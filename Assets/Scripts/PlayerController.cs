using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody rigi;
    private Vector3 velocity;
    private void Awake()
    {
        rigi = GetComponent<Rigidbody>();
    }
    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    private void FixedUpdate()
    {
        rigi.MovePosition(rigi.position + velocity * Time.fixedDeltaTime);
    }

    public void LookAt(Vector3 _lookAt)
    {
        Vector3 heightCorrectedPoint = new Vector3(_lookAt.x, transform.position.y, _lookAt.z);
        transform.LookAt(heightCorrectedPoint);
    }
}
