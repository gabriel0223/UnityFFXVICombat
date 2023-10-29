using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class ImpactVFX : MonoBehaviour
{
    [SerializeField] private Light _light;
    [SerializeField] private float _animationDuration;
    [SerializeField] private float _lightIntensity;

    // Start is called before the first frame update
    void Start()
    {
        _light.DOIntensity(_lightIntensity, _animationDuration).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo);
    }
}
