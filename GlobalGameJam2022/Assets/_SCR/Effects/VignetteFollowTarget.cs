using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class VignetteFollowTarget : MonoBehaviour
{
    [SerializeField] private Transform target = null;
    [SerializeField] private Camera playerCamera = null;
    [SerializeField] private VolumeProfile volume = null;
    private UnityEngine.Rendering.Universal.Vignette vignette = null;

    private void Start() 
    {
        if (!volume.TryGet(out vignette)) throw new System.NullReferenceException(nameof(vignette));
    }

    private void LateUpdate() 
    {
        if (vignette == null)
            return;

        if (target == null)
            return;

        vignette.center.Override(playerCamera.WorldToViewportPoint(target.position));
    }
}
