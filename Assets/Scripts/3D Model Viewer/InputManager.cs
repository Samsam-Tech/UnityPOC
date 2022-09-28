using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InputManager : MonoBehaviour
{
    TouchControls touchControls;
    Coroutine currentCoroutine;
    Transform cameraTransform;
    Camera cameraMain;
    [SerializeField] float zoomSpeed = 7f;
    [SerializeField] GameObject cube;
    float secondaryTouchContact = 0;

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
        cameraMain = Camera.main;
        secondaryTouchContact = 0;
        touchControls.Touch.SecondaryTouchContact.started += ctx => ZoomStart(ctx); //syntax for subscribing to an event and to pass info from that event to the func
        touchControls.Touch.SecondaryTouchContact.canceled += ctx => ZoomEnd(ctx);
        touchControls.Touch.PrimaryTouchContact.started += ctx => RotateStart(ctx);
        touchControls.Touch.PrimaryTouchContact.canceled += ctx => RotateEnd(ctx);
    }

    private void RotateStart(InputAction.CallbackContext context)
    {
        Vector3 initialPosition = touchControls.Touch.PrimaryFingerPosition.ReadValue<Vector2>();

        if (secondaryTouchContact == 0)
        {
            currentCoroutine = StartCoroutine(Rotation(initialPosition));
        }

    }
    private void RotateEnd(InputAction.CallbackContext context)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

    }

    IEnumerator Rotation(Vector3 initialPosition)
    {
        float rotationSpeed = 0.2f;
        Quaternion initialObjRotation = cube.transform.rotation;

        while (true)
        {
            Vector3 currentPosition = touchControls.Touch.PrimaryFingerPosition.ReadValue<Vector2>();

            if (initialPosition != currentPosition)
            {
                float distanceBetweenPosX = currentPosition.x - initialPosition.x;
                float distanceBetweenPosY = currentPosition.y - initialPosition.y;

                float rotateX = rotationSpeed * distanceBetweenPosX;
                float rotateY = rotationSpeed * distanceBetweenPosY;

                cube.transform.rotation = Quaternion.Euler(rotateY, -rotateX, 0f) * initialObjRotation;
            }

            yield return null;

            if (secondaryTouchContact == 1)
            {
                yield break;
            }
        }
    }

    private void ZoomStart(InputAction.CallbackContext context)
    {
        secondaryTouchContact = context.ReadValue<float>();
        currentCoroutine = StartCoroutine(ZoomDetection());
    }

    private void ZoomEnd(InputAction.CallbackContext context)
    {
        secondaryTouchContact = context.ReadValue<float>();
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

    }

    IEnumerator ZoomDetection()
    {
        float previousDistance = Vector2.Distance(touchControls.Touch.PrimaryFingerPosition.ReadValue<Vector2>(),
            touchControls.Touch.SecondaryFingerPosition.ReadValue<Vector2>());
        float distance = 0f;
        Vector3 initialPrimaryFingerPosition = touchControls.Touch.PrimaryFingerPosition.ReadValue<Vector2>();
        Vector3 initialSecondaryFingerPosition = touchControls.Touch.SecondaryFingerPosition.ReadValue<Vector2>();
        Vector3 currentCamPosition = cameraTransform.transform.position;

        while (true)
        {
            //Detection
            distance = Vector2.Distance(touchControls.Touch.PrimaryFingerPosition.ReadValue<Vector2>(),
            touchControls.Touch.SecondaryFingerPosition.ReadValue<Vector2>());
            Vector3 currentPrimaryFingerPosition = touchControls.Touch.PrimaryFingerPosition.ReadValue<Vector2>();
            Vector3 currentSecondaryFingerPosition = touchControls.Touch.SecondaryFingerPosition.ReadValue<Vector2>();

            // Zoom in
            if (distance > previousDistance)
            {
                Vector3 targetPosition = cameraTransform.transform.position;
                targetPosition.z += 1;
                cameraTransform.transform.position = Vector3.Slerp(cameraTransform.transform.position, targetPosition, Time.deltaTime * zoomSpeed);
            }
            // Zoom out
            else if (distance < previousDistance)
            {
                Vector3 targetPosition = cameraTransform.transform.position;
                targetPosition.z -= 1;
                cameraTransform.transform.position = Vector3.Slerp(cameraTransform.transform.position, targetPosition, Time.deltaTime * zoomSpeed);
            }

            previousDistance = distance;
            yield return null; //waits until next frame
        }
    }

}
