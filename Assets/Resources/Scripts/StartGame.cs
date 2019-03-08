using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public Animator anim;

    private static readonly int Speed = Animator.StringToHash("speed");

    // Start is called before the first frame update
    public void Click()
    {
        anim.SetFloat(Speed, -1f);
        anim.speed = 1;
        StartCoroutine(Load());
    }

    private IEnumerator Load()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        var asyncLoad = SceneManager.LoadSceneAsync(1);
        asyncLoad.allowSceneActivation = false;
        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.85f)
                asyncLoad.allowSceneActivation = true;
            yield return null;
        }
    }
}
