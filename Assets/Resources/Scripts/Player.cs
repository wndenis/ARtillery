using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Bullet bullet;
    public Transform gameroot;
    public float maxHp = 50f;
    public float hp = 10f;
    public Image overlayImage;
    public Texture2D hitTexture;
    public Text hpText;
    
    private float hitEffectDuration = 0.2f;
    private float hitEffectStartTime = -0.2f;

    private void Start()
    {
        StartCoroutine(FadeOut());
        if (hp < 1)
            hp = 1;
        if (maxHp < 1)
            maxHp = 1;
        if (hp > maxHp)
            hp = maxHp;
    }

    private void Update()
    {
        hpText.text = $"{hp}/{maxHp}";
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
        if (hp <= 0)
            return;
        var ray = Camera.main.ScreenPointToRay(screenPoint);
        var from = Camera.main.ScreenToWorldPoint(screenPoint);
        var b = Instantiate(bullet, gameroot);
        b.speed = 0.525f;
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

    private IEnumerator Fade(float fromAlpha, float toAlpha, float duration)
    {
        var imageColor = overlayImage.color;
        var textColor = hpText.color;

        for (var t = 0f; t < duration; t += Time.deltaTime)
        {
            var alpha = Mathf.Lerp(fromAlpha, toAlpha, t / duration);
            imageColor.a = alpha;
            textColor.a = 1 - alpha;
            overlayImage.color = imageColor;
            hpText.color = textColor;
            yield return null;
        }
        imageColor.a = toAlpha;
        textColor.a = 1 - toAlpha;
        overlayImage.color = imageColor;
        hpText.color = textColor;
    }
    

    private IEnumerator FadeIn()
    {
        yield return StartCoroutine(Fade(0, 1, 1));
        SceneManager.LoadScene(1);
    }
    
    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(Fade(1, 0, 1));
    }

    public void Damage(float amount = 1f)
    {
        if (hp > 0)
        {
            hp -= amount;
            hitEffectStartTime = Time.time;
            if (hp <= 0)
            {
                hp = 0;
                StartCoroutine(FadeIn());
            }
        }
    }
}
