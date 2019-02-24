using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Bullet bullet;
    public Transform gameroot;
    public float hp = 10f;
    public Texture2D hitTexture;
    private float hitEffectDuration = 0.2f;
    private float hitEffectStartTime = -0.2f;
    

    private void Update()
    {
        var screenPoint = Vector2.zero;
        var invalidTouch = true;
        
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        
        #if PLATFORM_ANDROID
        foreach (var touch in Input.touches) {
            if (touch.phase == TouchPhase.Began)
            {
                screenPoint = touch.position;
                invalidTouch = false;
                break;
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            invalidTouch = false;
            screenPoint = Input.mousePosition;
        }
        #else
        if (Input.GetMouseButtonDown(0))
        {
            invalidTouch = false;
            screenPoint = Input.mousePosition;
        }
        #endif
        if (invalidTouch)
            return;
        var ray = Camera.main.ScreenPointToRay(screenPoint);
        var from = Camera.main.ScreenToWorldPoint(screenPoint);
        var b = Instantiate(bullet, gameroot);
        b.speed = 0.45f;
        b.direction = ray.direction;
        b.transform.position = from + ray.direction * 0.075f;
    }

    private void OnGUI()
    {
        if (Time.time - hitEffectStartTime < hitEffectDuration)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), hitTexture);
        }
    }

    public void Damage(float amount = 1f)
    {
        hp -= amount;
//        if (hp <= 0)
//            Application.Quit();
        hitEffectStartTime = Time.time;
    }
}
