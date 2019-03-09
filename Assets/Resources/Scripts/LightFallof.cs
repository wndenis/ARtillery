using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFallof : MonoBehaviour
{
    public float duration = 1f;

    private Light _light;
    // Start is called before the first frame update
    void Start()
    {
        _light = GetComponent<Light>();
        StartCoroutine(Falloff());
    }

    IEnumerator Falloff()
    {
        yield return new WaitForSeconds(0.1f);
        var oldInstensity = _light.intensity;
        for (var t = 0f; t <= duration; t += Time.deltaTime)
        {
            _light.intensity = Mathf.Lerp(oldInstensity, 0, t / duration);
            yield return null;
        }
    }
}
