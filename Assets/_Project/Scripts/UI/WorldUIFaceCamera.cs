using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUIFaceCamera : MonoBehaviour
{
    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward, _mainCamera.transform.rotation * Vector3.up);
    }
}
