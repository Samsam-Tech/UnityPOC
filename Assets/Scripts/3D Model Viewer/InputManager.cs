using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    TouchControls touchControls;
    Coroutine zoomCoroutine;
    Transform cameraTransform;
    [SerializeField] float zoomSpeed = 7f;

    private void Awake()
    {
        touchControls = new TouchControls();
        cameraTransform = Camera.main.transform;
    }

    private void OnEnable()
    {
        touchControls.Enable();
    }

    private void OnDisable()
    {
        touchControls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        touchControls.Touch.SecondaryTouchContact.started += ctx => ZoomStart(ctx); //syntax for subscribing to an event and to pass info from that event to the func
        touchControls.Touch.SecondaryTouchContact.canceled += ctx => ZoomEnd(ctx);
    }

    private void ZoomStart(InputAction.CallbackContext context)
    {
        zoomCoroutine = StartCoroutine(ZoomDetection());
    }

    private void ZoomEnd(InputAction.CallbackContext context)
    {
        StopCoroutine(zoomCoroutine);
    }

    IEnumerator ZoomDetection()
    {
        float previousDistance = Vector2.Distance(touchControls.Touch.PrimaryFingerPosition.ReadValue<Vector2>(),
            touchControls.Touch.SecondaryFingerPosition.ReadValue<Vector2>());
        float distance = 0f;

        while (true)
        {
            //Detection
            distance = Vector2.Distance(touchControls.Touch.PrimaryFingerPosition.ReadValue<Vector2>(),
            touchControls.Touch.SecondaryFingerPosition.ReadValue<Vector2>());
            
            // Zoom out
            if (distance > previousDistance)
            {
                Vector3 targetPosition = cameraTransform.position;
                targetPosition.z += 1;
                cameraTransform.position = Vector3.Slerp(cameraTransform.position, targetPosition, Time.deltaTime * zoomSpeed);
            }
            // Zoom in
            else if (distance < previousDistance)
            {
                Vector3 targetPosition = cameraTransform.position;
                targetPosition.z -= 1;
                cameraTransform.position = Vector3.Slerp(cameraTransform.position, targetPosition, Time.deltaTime * zoomSpeed);
            }

            previousDistance = distance;
            yield return null; //waits until next frame
        }
    }

}
