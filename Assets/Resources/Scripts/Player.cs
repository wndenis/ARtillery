using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Bullet bullet;
    public Transform gameroot;
    private void Update()
    {
        var screenPoint = Vector2.zero;
        
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        
        #if PLATFORM_ANDROID
        foreach (var touch in Input.touches) {
            if (touch.phase == TouchPhase.Began)
            {
                screenPoint = touch.position;
                break;
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            screenPoint = Input.mousePosition;
        }
        #else
        if (Input.GetMouseButtonDown(0))
        {
            screenPoint = Input.mousePosition;
        }
        #endif
        if (screenPoint == Vector2.zero)
            return;
        var ray = Camera.main.ScreenPointToRay(screenPoint);
        var from = Camera.main.ScreenToWorldPoint(screenPoint);
        var b = Instantiate(bullet, gameroot);
        b.speed = 0.45f;
        b.direction = ray.direction;
        b.transform.position = from + ray.direction * 0.05f;
            //ShootRay(ray);
    }
}
