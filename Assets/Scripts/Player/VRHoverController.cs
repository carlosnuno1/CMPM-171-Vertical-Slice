using UnityEngine;
using Unity.XR.CoreUtils;
using System.Collections;
using System.Collections.Generic;

public class VRHoverController : MonoBehaviour
{
    public Rigidbody rb;
    public Transform rayOrigin;
    public LayerMask groundLayer;
    public XROrigin m_XROrigin;

    public float targetHeight = 1.2f;
    public float rayLength = 2f;

    public float springStrength = 800f;
    public float damping = 50f;

    void FixedUpdate()
    {
        RaycastHit hit;

        if (Physics.Raycast(rayOrigin.position, Vector3.down, out hit, rayLength, groundLayer))
        {
            targetHeight = m_XROrigin.CameraInOriginSpaceHeight;
            
            Debug.DrawRay(rayOrigin.position, Vector3.down * rayLength, Color.red);

            float distance = hit.distance;

            Debug.Log("Distance: " + distance);

            float error = targetHeight - distance;

            Debug.Log("Error: " + error);

            float upwardVelocity = rb.linearVelocity.y;

            float force = (error * springStrength) - (upwardVelocity * damping);


            rb.AddForce(Vector3.up * force);
        }
    }
}