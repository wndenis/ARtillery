﻿using UnityEngine;
using System.Collections;
 using System.Linq;
 using Vuforia;

public class CameraFocus : MonoBehaviour
{

    private bool mVuforiaStarted = false;
    private bool mFlashEnabled = false;
    private bool active;

    private void Start()
    {
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(StartAfterVuforia);
    }

    private void OnDestroy()
    {
        VuforiaARController.Instance.UnregisterVuforiaStartedCallback(StartAfterVuforia);
    }

    private void StartAfterVuforia()
    {
        mVuforiaStarted = true;
        SetAutofocus();
        active = true;
    }

    private void Update()
    {
        if (Input.touches.Any(touch => touch.phase == TouchPhase.Began) || Input.GetMouseButtonDown(0))
        {
            SetAutofocus();
        }
    }

    void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            if (mVuforiaStarted)
            {
                SetAutofocus();
            }
        }
    }

    private void SetAutofocus()
    {
        CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO);
    }
}