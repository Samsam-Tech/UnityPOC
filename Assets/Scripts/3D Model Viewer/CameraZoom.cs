using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] float zoomSpeed = 200;
    
    void Update()
    {
        float mouseScrollInput = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        transform.Translate(0f, 0f, mouseScrollInput);
    }
}
