using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class StaticScore
{
    private const int PRIME = 1122761;
    public static int Score;

    public static void Save(int score)
    {
        var prevScore = Decode(PlayerPrefs.GetInt("HighScore"));
        var newScore = Mathf.Max(prevScore, score);
        PlayerPrefs.SetInt("HighScore", Encode(newScore));
    }
    public static void Save()
    {
        Save(Score);
    }

    private static int Encode(int val)
    {
        val = (val + 3) * PRIME + 3;
        return val;
    }

    private static int Decode(int val)
    {
        val -= 3;
        if (val % PRIME != 0)
            return 0;
        val = val / PRIME - 3;
        return val;
    }

    public static int Load()
    {
        return Decode(PlayerPrefs.GetInt("HighScore"));
    }
}
