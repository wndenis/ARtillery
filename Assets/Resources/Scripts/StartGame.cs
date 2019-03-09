﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    public Animator anim;
    public Text score;

    private static readonly int Speed = Animator.StringToHash("speed");

    private void Start()
    {   
        var highScore = StaticScore.Load();
        score.text = $"Рекорд: {highScore}";
    }

    // Start is called before the first frame update
    public void Click()
    {
        StaticScore.Score = 0;
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