using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneDelayed : MonoBehaviour
{
    public float delay = 5.4f;
    public int sceneNumber;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadSceneAsync(sceneNumber);
    }
}
