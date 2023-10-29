using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes sure the world UI object with this script is always
/// facing the camera.
/// </summary>
public class WorldUIFaceCamera : MonoBehaviour
{
    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward, 
            _mainCamera.transform.rotation * Vector3.up);
    }
}
