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
{
    public int step = 4;
    private bool mAccessCameraImage = true;
    // camera image pixel 
    private Image.PIXEL_FORMAT mPixelFormat = Image.PIXEL_FORMAT.UNKNOWN_FORMAT;// or RGBA8888, RGB888, RGB565, YUV, GRAYSCALE
    // Boolean flag telling whether the pixel format has been registered
    private bool mFormatRegistered = false;
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
    public float? colorTemperature { get; private set; }
    
 
    void Start()
    {
        GraphicsSettings.lightsUseLinearIntensity = true;
        GraphicsSettings.lightsUseColorTemperature = true;
        // set up pixel format
        #if UNITY_EDITOR
        mPixelFormat = Image.PIXEL_FORMAT.GRAYSCALE; // Need Grayscale for Editor
        #else
        mPixelFormat = Image.PIXEL_FORMAT.RGB888; // Use RGB888 for mobile
        #endif
        
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        VuforiaARController.Instance.RegisterOnPauseCallback(OnVuforiaPaused);
        VuforiaARController.Instance.RegisterTrackablesUpdatedCallback(OnTrackablesUpdated);
    }
 
    private void OnVuforiaStarted()
    {
        mFormatRegistered = CameraDevice.Instance.SetFrameFormat(mPixelFormat, true);
    }
    
    /// <summary>
    /// Called when app is paused / resumed
    /// </summary>
    private void OnVuforiaPaused(bool paused)
    {
        if (paused)
            UnregisterFormat();
        else
            RegisterFormat();
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
        
        byte[] pixels = image.Pixels;
        if (pixels != null && pixels.Length > 0)
        {
            double totalLuminance = 0.0;
            for (int p = 0; p < pixels.Length; p += step)
                totalLuminance += pixels[p] * 0.299 + pixels[p + 1] * 0.587 + pixels[p + 2] * 0.114;
            
            totalLuminance /= pixels.Length;
            ligtColorNum = (float)totalLuminance * 0.0255f;
            //ligtColorNum is put in for RGB. will change color along the gray scale
            lightColor = new Color(ligtColorNum, ligtColorNum, ligtColorNum, 1.0f);
            
            totalLuminance /= 255.0;
            totalLuminance *= intesityModifier;
            m_LightToEffect.intensity = (float)totalLuminance;
            //m_LightToEffect.color = lightColor; //TODO ?????
 
            RenderSettings.ambientIntensity = m_LightToEffect.intensity;
            RenderSettings.ambientLight = lightColor;
            colorTemperature = (float?)(totalLuminance * temperatureModifier);
            m_LightToEffect.colorTemperature = (float)colorTemperature;
        }
    }
    
    /// <summary>
    /// Unregister the camera pixel format (e.g. call this when app is paused)
    /// </summary>
    private void UnregisterFormat()
    {
        CameraDevice.Instance.SetFrameFormat(mPixelFormat, false);
        mFormatRegistered = false;
    }
    
    /// <summary>
    /// Register the camera pixel format
    /// </summary>
    private void RegisterFormat()
    {
        if (CameraDevice.Instance.SetFrameFormat(mPixelFormat, true))
            mFormatRegistered = true;
        else
            mFormatRegistered = false;
    }

    private void OnDestroy()
    {
        VuforiaARController.Instance.UnregisterVuforiaStartedCallback(OnVuforiaStarted);
        VuforiaARController.Instance.UnregisterOnPauseCallback(OnVuforiaPaused);
        VuforiaARController.Instance.UnregisterTrackablesUpdatedCallback(OnTrackablesUpdated);
    }
}