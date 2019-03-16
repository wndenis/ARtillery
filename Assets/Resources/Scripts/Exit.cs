using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    public Player player;
    private bool _active;
    private IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(2f);
        _active = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && _active)
            if (player)
                player.Damage(10000, false);
            else
                Application.Quit();
    }
}
