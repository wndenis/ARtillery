﻿using UnityEngine;
using System.Collections;
 using System.Linq;
 using Vuforia;

public class CameraFocus : MonoBehaviour
{

    private bool mVuforiaStarted = false;
    private bool mFlashEnabled = false;
    private bool active;

    void Start()
    {
        VuforiaARController vuforia = VuforiaARController.Instance;

        if (vuforia != null)
            vuforia.RegisterVuforiaStartedCallback(StartAfterVuforia);
    }

    private void StartAfterVuforia()
    {
        mVuforiaStarted = true;
        SetAutofocus();
        active = true;
    }

    private void Update()
    {
        if (Input.touches.Any(touch => touch.phase == TouchPhase.Began))
        {
            SetAutofocus();
        }
    }

    void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            // App resumed
            if (mVuforiaStarted)
            {
                // App resumed and vuforia already started
                // but lets start it again...
                SetAutofocus(); // This is done because some android devices lose the auto focus after resume
                // this was a bug in vuforia 4 and 5. I haven't checked 6, but the code is harmless anyway
            }
        }
    }

    private void SetAutofocus()
    {
        CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO);
    }
}