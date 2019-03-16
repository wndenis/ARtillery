using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DisplayScoreAndGotoStart : MonoBehaviour
{
    public Text scoreText;
    public Text oldRecordText;
    public GameObject newRecordText;
    public float delay = 5.4f;
    public int sceneNumber;
    // Start is called before the first frame update
    private IEnumerator Start()
    {
        var highScore = StaticScore.Load();
        scoreText.text = $"{StaticScore.Score}";
        oldRecordText.text = $"Рекорд:\n{highScore}";
        newRecordText.SetActive(StaticScore.Score > highScore);

        StaticScore.Save();
        yield return new WaitForSeconds(delay);
        SceneManager.LoadSceneAsync(sceneNumber);
    }
}
