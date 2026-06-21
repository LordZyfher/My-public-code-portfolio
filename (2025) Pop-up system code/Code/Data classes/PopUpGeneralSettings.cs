using System;
using UnityEngine;

[Serializable]
public class PopUpGeneralSettings
{
    [Header("Settings")]
    public int minPopUpDurationS = 10;

    [Tooltip("Value below 0 or 0 = disabled")]
    public int maxPopUpDurationS = 0;
    public float lingerDuration = 1.5f;

    [Header("Animation")]
    public bool SlideIn = true;
    public bool SlideOut = true;
    public Vector3 SlideOffset = Vector3.zero;

    public float SaveInCloseQueueDuration => minPopUpDurationS + lingerDuration + 1f;
}
