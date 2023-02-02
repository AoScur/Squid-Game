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

    private Tagger tagger;

    public bool isStop { get; private set; }

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
        isStop = false;
        tagger = GameObject.FindWithTag("Tagger").GetComponent<Tagger>();
        StartCoroutine(GoAndStop());
    }

    private void Update()
    {
        UpdateUI();
    }

    public IEnumerator GoAndStop()
    {
        isStop = !isStop;
        tagger.ChangeTaggerState();
        yield return new WaitForSeconds(Random.value * 5f);
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
