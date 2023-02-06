using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance
    {
        get
        {
            if(m_instance ==null)
            {
                m_instance = FindObjectOfType<GameManager>();
            }
            return m_instance;
        }
    }

    private static GameManager m_instance;

    public static List<LivingEntity> targets = new List<LivingEntity>();

    public event Action onGameOver;

    public bool isGameover { get; private set; }

    private void Awake()
    {
        if (instance != this)
        {        
            Destroy(gameObject);
        }
        isGameover = false;
    }

    private void Start()
    {        
        var player = GameObject.FindWithTag("Player").GetComponent<Player>();
        player.onDeath += EndGame;
        var timer = GetComponent<Timer>();
        timer.onTimeOver += EndGame;
    }

    private void Update()
    {
        UpdateUI();
    }

    public void EndGame()
    {
        if (onGameOver != null)
        {
            onGameOver();
        }
        isGameover = true;
        
        UIManager.instance.SetActiveGameoverUI(true);
    }

    private void UpdateUI()
    {
        UIManager.instance.UpdateSurvivorText(targets.Count);
    }

}
