using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float distance = 10f;
    public Vector3 offset;

    Transform focus;
    Vector3 originalDelta;

    public void Calibrate(Transform focus)
    {
        this.focus = focus;
        originalDelta = transform.position - focus.position;
        originalDelta.x *= .01f;
    }

    private void LateUpdate()
    {
        if (focus == null) return;

        // Calculate target position
        Vector3 targetPosition = focus.position + offset * 0.5f + (originalDelta.normalized * distance);

        // Constrain or scale horizontal movement (x-axis)
        targetPosition.x *= 0.5f; // Reduce horizontal shift
        // Or use a fixed range
        // targetPosition.x = Mathf.Clamp(targetPosition.x, -5f, 5f);

        // Smoothly update camera position and rotation
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 2.5f);
        Quaternion targetRotation = Quaternion.LookRotation((focus.position + offset - transform.position).normalized, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 1f);
    }
}
