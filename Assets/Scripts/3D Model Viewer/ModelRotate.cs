using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelRotate : MonoBehaviour
{
    Vector3 pressPoint; // initial pos of mouse
    Quaternion currentRotation; // initial rotation of obj
    [SerializeField] float rotationSpeed = 0.3f;
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //left click
        {
            pressPoint = Input.mousePosition;
            currentRotation = transform.rotation;
        }
        else if (Input.GetMouseButton(0)) // if held down
        {

            float distanceBetweenMousePosX = Input.mousePosition.x - pressPoint.x;
            float distanceBetweenMousePosY = Input.mousePosition.y - pressPoint.y;

            float mouseX = rotationSpeed * distanceBetweenMousePosX;
            float mouseY = rotationSpeed * distanceBetweenMousePosY;

            transform.rotation = Quaternion.Euler(mouseY, -mouseX, 0f) * currentRotation;
        }
    }
}
