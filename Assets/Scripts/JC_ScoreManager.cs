using System;
using System.Collections.Generic;
using UnityEngine;

public class JC_ScoreManager : MonoBehaviour
{
    private readonly HashSet<GameObject> _scoredEnemies = new HashSet<GameObject>();

    public static JC_ScoreManager Instance { get; private set; }

    public event Action<int> ScoreChanged;

    public int CurrentScore { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        CurrentScore = 0;
    }

    private void Start()
    {
        ScoreChanged?.Invoke(CurrentScore);
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
        return true;
    }
}
