using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform transformCamera;

    [Header("Movement")]
    public float arrowKeySpeed = 80;
    public float mouseDragSensitivity = 2.8f;
    [Range(0, 0.99f)] public float movementSmoothing = .75f;

    [Header("Scrolling")]
    public float scrollSensitivity = 1.6f;

    [Header("X Bounds")]
    public float minimumX = -70;
    public float maximumX = 70;

    [Header("Y Bounds")]
    public float minimumY = 18;
    public float maximumY = 80;

    [Header("Z Bounds")]
    public float minimumZ = -130;
    public float maximumZ = 70;

    private Vector3 targetPosition;

    void Update()
    {
        ArrowKeyMovement();
        MouseDragMovement();
        Zooming();
        MoveTowardsTarget();
    }

    void ArrowKeyMovement()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            targetPosition.z += arrowKeySpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            targetPosition.z -= arrowKeySpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            targetPosition.x += arrowKeySpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            targetPosition.x -= arrowKeySpeed * Time.deltaTime;
        }
    }
    void MouseDragMovement()
    {
        if (Input.GetMouseButton(1))
        {
            Vector3 movement = new Vector3(-Input.GetAxis("Mouse X"), 0, -Input.GetAxis("Mouse Y")) * mouseDragSensitivity;

            if (movement != Vector3.zero)
            {
                targetPosition += movement;
            }
        }
    }
    void Zooming()
    {
        float scrollDelta = -Input.mouseScrollDelta.y;

        if (scrollDelta != 0)
        {
            targetPosition.y += scrollDelta * scrollSensitivity;
        }
    }
    void MoveTowardsTarget()
    {
        targetPosition.x = Mathf.Clamp(targetPosition.x, minimumX, maximumX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minimumY, maximumY);
        targetPosition.z = Mathf.Clamp(targetPosition.z, minimumZ, maximumZ);
        if (transformCamera.position != targetPosition)
        {
            transformCamera.position = Vector3.Lerp(transformCamera.position, targetPosition, 1 - movementSmoothing);
        }
    }
}
