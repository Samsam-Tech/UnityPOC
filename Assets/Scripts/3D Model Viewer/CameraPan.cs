using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPan : MonoBehaviour
{
    Vector3 pressPoint; // initial pos of mouse
    Vector3 camPos; // initial pos of cam
    [SerializeField] float panningSpeed = 0.1f;

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) //right
        {
            pressPoint = Input.mousePosition;
            camPos = Camera.main.transform.position;
        }
        else if (Input.GetMouseButton(1))
        {
            if (pressPoint != Input.mousePosition)
            {
                float distanceBetweenMousePosX = Input.mousePosition.x - pressPoint.x;
                float distanceBetweenMousePosY = Input.mousePosition.y - pressPoint.y;

                float mouseX = panningSpeed * distanceBetweenMousePosX;
                float mouseY = panningSpeed * distanceBetweenMousePosY;

                Vector3 newCamPos = new Vector3( -(mouseX - camPos.x), -(mouseY - camPos.y), camPos.z);

                Camera.main.transform.position = newCamPos;
            }
        }
    }
}
