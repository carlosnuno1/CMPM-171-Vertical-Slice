using UnityEngine;
using Unity.XR.CoreUtils;

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

    public bool grounded;

    void FixedUpdate()
    {
        RaycastHit hit;

        if (Physics.Raycast(rayOrigin.position, Vector3.down, out hit, m_XROrigin.CameraInOriginSpaceHeight + rayLength, groundLayer))
        {
            grounded = true;

            Debug.DrawRay(rayOrigin.position, Vector3.down * (m_XROrigin.CameraInOriginSpaceHeight + rayLength), Color.red);

            targetHeight = m_XROrigin.CameraInOriginSpaceHeight;

            float distance = hit.distance;

            float error = Mathf.Max(targetHeight - distance, 0f);

            float upwardVelocity = rb.linearVelocity.y;

            float force = (error * springStrength) - (upwardVelocity * damping);

            rb.AddForce(Vector3.up * force);
        }
        else
        {
            grounded = false;
        }
    }
}