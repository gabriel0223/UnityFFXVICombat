using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Class that controls a distortion VFX within a sphere. 
/// </summary>
public class DistortionSphere : MonoBehaviour
{
    [SerializeField] private MeshRenderer _mesh;

    private Material _distortionMaterial;

    private void Awake()
    {
        _distortionMaterial = new Material(_mesh.material);
        _mesh.material = _distortionMaterial;
    }

    public Tween AnimateDistortion(float startValue, float endValue, float duration)
    {
        return DOVirtual.Float(startValue, endValue, duration,
            value => _distortionMaterial.SetFloat(
                "_DistortionIntensity", value));
    }
}
