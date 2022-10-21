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
    [SerializeField] float zoomSpeed = 9f;
    [SerializeField] float panningSpeed = 0.001f;
    [SerializeField] GameObject cube;
    [SerializeField] TextMeshProUGUI distanceText;
    [SerializeField] TextMeshProUGUI previousDistanceText;
    [SerializeField] TextMeshProUGUI panningText;
    [SerializeField] float distanceAllowance = 1.5f;
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


        if (!touchControls.Touch.SecondaryTouchContact.IsPressed())
        {
            currentCoroutine = StartCoroutine(Rotation());
        }

    }
    private void RotateEnd(InputAction.CallbackContext context)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

    }

    IEnumerator Rotation()
    {
        Vector3 initialPosition = touchControls.Touch.PrimaryFingerPosition.ReadValue<Vector2>();
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

            if (touchControls.Touch.SecondaryTouchContact.IsPressed())
            {
                yield break; //end a Coroutine before it's finished
            }

            yield return null;


        }
    }

    private void ZoomStart(InputAction.CallbackContext context)
    {
        currentCoroutine = StartCoroutine(ZoomDetection());
    }

    private void ZoomEnd(InputAction.CallbackContext context)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

    }

    // IEnumerator PanDetection()
    // {
    //     Vector3 initialPrimaryFingerPosition = touchControls.Touch.PrimaryFingerPosition.ReadValue<Vector2>();
    //     Vector3 initialSecondaryFingerPosition = touchControls.Touch.SecondaryFingerPosition.ReadValue<Vector2>();
    //     Vector3 currentCamPosition = cameraTransform.position;
    //     while (true)
    //     {
    //         Vector3 currentPrimaryFingerPosition = touchControls.Touch.PrimaryFingerPosition.ReadValue<Vector2>();
    //         Vector3 currentSecondaryFingerPosition = touchControls.Touch.SecondaryFingerPosition.ReadValue<Vector2>();
    //     }
    // }

    IEnumerator ZoomDetection()
    {
        float previousDistance = Vector2.Distance(touchControls.Touch.PrimaryFingerPosition.ReadValue<Vector2>(),
            touchControls.Touch.SecondaryFingerPosition.ReadValue<Vector2>());
        Vector3 initialPrimaryFingerPosition = touchControls.Touch.PrimaryFingerPosition.ReadValue<Vector2>();
        Vector3 initialSecondaryFingerPosition = touchControls.Touch.SecondaryFingerPosition.ReadValue<Vector2>();
        Vector3 currentCamPosition = cameraTransform.position;
        previousDistanceText.SetText("Previous Distance: " + previousDistance);

        while (true)
        {
            //Detection
            float distance = Vector2.Distance(touchControls.Touch.PrimaryFingerPosition.ReadValue<Vector2>(),
            touchControls.Touch.SecondaryFingerPosition.ReadValue<Vector2>());
            Vector3 currentPrimaryFingerPosition = touchControls.Touch.PrimaryFingerPosition.ReadValue<Vector2>();
            Vector3 currentSecondaryFingerPosition = touchControls.Touch.SecondaryFingerPosition.ReadValue<Vector2>();

            // distanceText.SetText("Distance: " + distance);

            // Zoom in
            if (distance > previousDistance)
            {
                Vector3 targetPosition = cameraTransform.position;
                targetPosition.z += 1;
                cameraTransform.position = Vector3.Slerp(cameraTransform.position, targetPosition, Time.deltaTime * zoomSpeed);
                panningText.SetText("ZOOM IN!");
            }
            // Zoom out
            else if (distance < previousDistance)
            {
                Vector3 targetPosition = cameraTransform.position;
                targetPosition.z -= 1;
                cameraTransform.position = Vector3.Slerp(cameraTransform.position, targetPosition, Time.deltaTime * zoomSpeed);
                panningText.SetText("ZOOM OUT!");
            }
            // else if (distance == previousDistance)
            // {
            // panningText.SetText("PANNING!");
            // if (currentPrimaryFingerPosition != initialPrimaryFingerPosition)
            // {
            //     float distanceBetweenPosX = currentPrimaryFingerPosition.x - initialPrimaryFingerPosition.x;
            //     float distanceBetweenPosY = currentPrimaryFingerPosition.y - initialPrimaryFingerPosition.y;

            //     float posX = distanceBetweenPosX * panningSpeed * Time.deltaTime;
            //     float posY = distanceBetweenPosY * panningSpeed * Time.deltaTime;

            //     // cameraTransform.Translate(-posX, -posY, 0f);

            //     Vector3 newCamPos = new Vector3(-(posX - currentCamPosition.x), -(posY - currentCamPosition.y), currentCamPosition.z);

            //     Camera.main.transform.position = newCamPos;

            // }
            // }

            previousDistance = distance;
            yield return null; //waits until next frame
        }
    }

}
