using System;
using System.Collections.Generic;
using UnityEngine;

public class JC_ScoreManager : MonoBehaviour
{
    private const string HighScorePreferenceKey = "JC_HighScore";

    private readonly HashSet<GameObject> _scoredEnemies = new HashSet<GameObject>();

    public static JC_ScoreManager Instance { get; private set; }

    public event Action<int> ScoreChanged;
    public event Action<int> HighScoreChanged;

    public int CurrentScore { get; private set; }
    public int HighScore { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        CurrentScore = 0;
        HighScore = PlayerPrefs.GetInt(HighScorePreferenceKey, 0);
    }

    private void Start()
    {
        ScoreChanged?.Invoke(CurrentScore);
        HighScoreChanged?.Invoke(HighScore);
    }

    public bool TryAddEnemyKill(GameObject enemyObject)
    {
        if (enemyObject == null)
        {
            return false;
        }

        if (!_scoredEnemies.Add(enemyObject))
        {
            return false;
        }

        CurrentScore += 1;
        ScoreChanged?.Invoke(CurrentScore);

        if (CurrentScore > HighScore)
        {
            HighScore = CurrentScore;
            PlayerPrefs.SetInt(HighScorePreferenceKey, HighScore);
            PlayerPrefs.Save();
            HighScoreChanged?.Invoke(HighScore);
        }

        return true;
    }
}
