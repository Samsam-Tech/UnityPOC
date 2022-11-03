using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    TouchControls touchControls;
    Coroutine currentCoroutine;
    Camera cameraMain;
    [SerializeField] float zoomSpeed = 1f;
    [SerializeField] float rotationSpeed = 0.1f;
    public GameObject model;
    float secondaryTouchContact = 0;


    private void Awake()
    {
        touchControls = new TouchControls();
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
        Vector3 modelInitialPos = initialPosition;
        Quaternion initialObjRotation = model.transform.rotation;

        while (true)
        {
            Vector3 currentPosition = touchControls.Touch.PrimaryFingerPosition.ReadValue<Vector2>();

            if (modelInitialPos != currentPosition)
            {

                float distanceBetweenPosX = currentPosition.x - initialPosition.x;
                float distanceBetweenPosY = currentPosition.y - initialPosition.y;

                float rotateX = rotationSpeed * distanceBetweenPosX;
                float rotateY = rotationSpeed * distanceBetweenPosY;
                model.transform.rotation = Quaternion.Euler(rotateY, -rotateX, 0f) * initialObjRotation;
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
            secondaryTouchContact = 0;
        }

    }

    IEnumerator ZoomDetection()
    {
        float previousDistance = Vector2.Distance(touchControls.Touch.PrimaryFingerPosition.ReadValue<Vector2>(),
            touchControls.Touch.SecondaryFingerPosition.ReadValue<Vector2>());
        float distance = 0f;
        Vector3 initialPrimaryFingerPosition = touchControls.Touch.PrimaryFingerPosition.ReadValue<Vector2>();
        Vector3 initialSecondaryFingerPosition = touchControls.Touch.SecondaryFingerPosition.ReadValue<Vector2>();
        Vector3 currentCamPosition = cameraMain.transform.position;

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
                float targetDistance = distance;
                Vector3 movementAmount = new Vector3(0, 0, targetDistance) * zoomSpeed * Time.deltaTime;
                cameraMain.transform.Translate(movementAmount);
                // cameraMain.fieldOfView = Mathf.MoveTowards(cameraMain.fieldOfView, -targetDistance, zoomSpeed * Time.deltaTime); //speed to 30
            }
            // Zoom out
            else if (distance < previousDistance)
            {
                float targetDistance = distance;
                Vector3 movementAmount = new Vector3(0, 0, -targetDistance) * zoomSpeed * Time.deltaTime;
                cameraMain.transform.Translate(movementAmount);
                // cameraMain.fieldOfView = Mathf.MoveTowards(cameraMain.fieldOfView, targetDistance, zoomSpeed * Time.deltaTime); //speed to 30
            }

            previousDistance = distance;
            yield return null; //waits until next frame
        }
    }

}
