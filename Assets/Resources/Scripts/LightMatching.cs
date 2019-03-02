using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.UI;
using UnityEngine.Rendering;
using Image = Vuforia.Image;
 
/*       *************************** HOW TO USE **********************************
 
    Just Attach to your AR camera or Create an empty GameObject in your scene and attach this script
    Most likely you'll have at least one light in your sceen. add your light to the public Light To Effect var in the editor.
    You'll need to modify the code if you have more than one light you'd like to affect. 
 
    To see get dev/debugging readouts create a screen overlay canvas with a couple of text objects. add them to the lightoutput vars in the editor
     
    Can varify that a this light estimation script works in iphone7, Samsung S6, and pixel
    Should also work in the Unity editor as well.
 
         ***************************   ENJOY!   **********************************
 
*/
public class LightMatching : MonoBehaviour
{   private bool mAccessCameraImage = true;
    // camera image pixel 
    private Image.PIXEL_FORMAT mPixelFormat = Image.PIXEL_FORMAT.UNKNOWN_FORMAT;// or RGBA8888, RGB888, RGB565, YUV, GRAYSCALE
    // Boolean flag telling whether the pixel format has been registered
    private bool mFormatRegistered = false;
    // during development you might want to set up a couple of text objects in a UI screen overlay for debugging
    public Text LightOutput1;
    public Text LightOutput2;
    // boolean used to start debugging 
    public bool debugging;
    //This is the directional light I was using in my scene. 
    public Light m_LightToEffect;
    // This color variable is being used to change the ambient light in the scene. goes from white to black depending on brightness of lights
    // You might want to thake that out depending on how your scene is looking
    private Color lightColor = new Color(1,1,1,1);
    private float ligtColorNum;
    //Use this to make adjustments if your light estimation is looking too bright or too dark.  I don't know what a double is.
    public double intesityModifier = 10.0;
    //Use this to make light temperature adjustments
    public int temperatureModifier= 3000;
    // To be honest I'm not sure what these are for. I cobbled this code from a bunch of different scripts. It's not messing anything up so I'm leaving in 
    public float? intensity { get; private set; }
    public float? colorTemperature { get; private set; }
    
 
    void Start()
    {
        // I beleive these are required setting to make adjustments to the ambient light in the scene. I could be wrong about that. 
        GraphicsSettings.lightsUseLinearIntensity = true;
        GraphicsSettings.lightsUseColorTemperature = true;
        // set up pixel format
        #if UNITY_EDITOR
        mPixelFormat = Image.PIXEL_FORMAT.GRAYSCALE; // Need Grayscale for Editor
        #else
        mPixelFormat = Image.PIXEL_FORMAT.RGB888; // Use RGB888 for mobile
        #endif
 
        // API for getting Vuforia Callbacks as of Unity 2018.1.0f2. 
        //The OnVuforiaStarted event is required for getting the camera pixel data for sure
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        // not sure if this is really needed but leaving in just to be safe
        VuforiaARController.Instance.RegisterOnPauseCallback(OnVuforiaPaused);
        //This behaves much like Update() - Most of the magic happens in OnTrackablesUpdated
        VuforiaARController.Instance.RegisterTrackablesUpdatedCallback(OnTrackablesUpdated);
        // not needed. just thought I'd leave it incase anybody needs to access these callbacks
        //  DeviceTrackerARController.Instance.RegisterTrackerStartedCallback(OnTrackerStarted);
        //  DeviceTrackerARController.Instance.RegisterDevicePoseStatusChangedCallback(OnDevicePoseStatusChanged);
          
        //text used for debugging
//        LightOutput1.text = "";
//        LightOutput2.text = "";
 
    }
 
    private void OnVuforiaStarted()
    {
        // Try register camera image format
        
        if (CameraDevice.Instance.SetFrameFormat(mPixelFormat, true))
        {
//            Debug.Log("Successfully registered pixel format " + mPixelFormat.ToString());
            mFormatRegistered = true;
        }
        else
        {
//            Debug.LogError("Failed to register pixel format " + mPixelFormat.ToString() +
//                "\n the format may be unsupported by your device;" +
//                "\n consider using a different pixel format.");
            mFormatRegistered = false;
        }
    }
    /// <summary>
    /// Called when app is paused / resumed
    /// </summary>
    private void OnVuforiaPaused(bool paused)
    {
        if (paused)
        {
//            Debug.Log("App was paused");
            UnregisterFormat();
        }
        else
        {
//            Debug.Log("App was resumed");
            RegisterFormat();
        }
    }
 
 
    /// <summary>
    /// Called each time the Vuforia state is updated
    /// </summary>
    private void OnTrackablesUpdated()
    {
        //Here's the getting camera pixel part of the code. not sure how it works but it works
        if (!mFormatRegistered) return;
        if (!mAccessCameraImage) return;
        Vuforia.Image image = CameraDevice.Instance.GetCameraImage(mPixelFormat);
        if (image == null) return;
        string imageInfo = mPixelFormat + " image: \n";
        imageInfo += " size: " + image.Width + " x " + image.Height + "\n";
        imageInfo += " bufferSize: " + image.BufferWidth + " x " + image.BufferHeight + "\n";
        imageInfo += " stride: " + image.Stride;
//        Debug.Log(imageInfo);
        byte[] pixels = image.Pixels;
        if (pixels != null && pixels.Length > 0)
        {
//            Debug.Log("Image pixels: " + pixels[0] + "," + pixels[1] + "," + pixels[2] + ",...");
            // I have no idea what the double type is or does. seems to be similar to float
            double totalLuminance = 0.0;
            for (int p = 0; p < pixels.Length; p += 4)
            {
                totalLuminance += pixels[p] * 0.299 + pixels[p + 1] * 0.587 + pixels[p + 2] * 0.114;
 
            }
 
            totalLuminance /= (pixels.Length);
            //this takes the totalLuminance in the line above and puts it in the decimal range. Needs to be cast as a float.  My math is arbitrary but works out
            ligtColorNum = (float)totalLuminance * 0.0255f;
            //ligtColorNum is put in for RGB. will change color along the gray scale
            lightColor = new Color(ligtColorNum, ligtColorNum, ligtColorNum, 1.0f);
//            Debug.Log("color ++++++++++++++++++++++++++++++++++++++++++++++++++++" + ligtColorNum);
            // I got this math from someone else's code. seems to convert totalLuminance to smaller number for adjusting light luminance.
            totalLuminance /= 255.0;
            totalLuminance *= intesityModifier;
//            Debug.Log("Total luminance ========================" + totalLuminance);
            m_LightToEffect.intensity = (float)totalLuminance;
            //originally tried to change color of light in scene but it looked flickery. Could be fun though for someone to try light color effects
            //  m_LightToEffect.color = lightColor;
 
            //This is the secret ingredient in the special sauce. This adjusts the Ambient light that's always present in a scene.
            //If you were to only adjust the lights in the scene it would never go completely dark if you turned the lights off in a room.
            //still won't go completely dark because your screen will have some illumingation, but it gets pretty close.
            RenderSettings.ambientIntensity = m_LightToEffect.intensity;
            //I'm changing the color of the ambient light on the grayscale. It's a little redundant but I think it helps. Experiment and see how you feel. 
            RenderSettings.ambientLight = lightColor;
//            Debug.Log("light intensity = " + m_LightToEffect.intensity);
            //I'm not exacly sure what the color temp does. It was a setting in someone else's code, but seems to be working out.
            colorTemperature = (float?)(totalLuminance * temperatureModifier);
            m_LightToEffect.colorTemperature = (float)colorTemperature;
//            Debug.Log("calculating color temperature =========================" + colorTemperature);
        }
    }
 
    /// <summary>
    /// Unregister the camera pixel format (e.g. call this when app is paused)
    /// </summary>
    private void UnregisterFormat()
    {
//        Debug.Log("Unregistering camera pixel format " + mPixelFormat.ToString());
        CameraDevice.Instance.SetFrameFormat(mPixelFormat, false);
        mFormatRegistered = false;
    }
    /// <summary>
    /// Register the camera pixel format
    /// </summary>
    private void RegisterFormat()
    {
        if (CameraDevice.Instance.SetFrameFormat(mPixelFormat, true))
        {
//            Debug.Log("Successfully registered camera pixel format " + mPixelFormat.ToString());
            mFormatRegistered = true;
        }
        else
        {
//            Debug.LogError("Failed to register camera pixel format " + mPixelFormat.ToString());
            mFormatRegistered = false;
        }
    }
 
 
   
}