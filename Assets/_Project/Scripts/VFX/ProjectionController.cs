using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Controls the projection VFX left by certain skills.
/// </summary>
public class ProjectionController : MonoBehaviour
{
    private Material _materialInstance;
    private Renderer[] _renderers;

    private void Start()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        _materialInstance = new Material(_renderers[0].material);

        foreach (Renderer mesh in _renderers)
        {
            mesh.material = _materialInstance;
            mesh.shadowCastingMode = ShadowCastingMode.Off;
        }

        PlayFadeAnimation();
    }

    private void PlayFadeAnimation()
    {
        Sequence projectionSequence = DOTween.Sequence();
        projectionSequence.AppendInterval(0.5f);
        projectionSequence.Append(DOVirtual.Float(1, 0, 1f,
            value => _materialInstance.SetFloat("_Alpha", value)));
        projectionSequence.AppendCallback(() => Destroy(gameObject));
    }
}
