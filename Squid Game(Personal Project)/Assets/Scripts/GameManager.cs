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

    public bool isGameover { get; private set; }

    private void Awake()
    {
        if (instance != this)
        {        
            Destroy(gameObject);
        }
    }

    private void Start()
    {        
        var find = GameObject.FindWithTag("Player");
        var player = find.GetComponent<Player>();
        player.onDeath += EndGame;
    }

    private void Update()
    {
        UpdateUI();
    }

    public void EndGame()
    {
        isGameover = true;
        
        //UIManager.instance.SetActiveGameoverUI(true);
    }

    private void UpdateUI()
    {
        UIManager.instance.UpdateSurvivorText(targets.Count);
    }

}
