using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

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

    public GameObject HitEffect_Prefab;
    public GameObject BloodSpray_Prefab;

    public static IObjectPool<HitEffect> hit_Pool;
    public static IObjectPool<BloodSprayEffect> blood_Pool;

    public event Action onGameOver;

    public bool isGameover { get; private set; }

    private void Awake()
    {
        if (instance != this)
        {        
            Destroy(gameObject);
        }
        isGameover = false;

        hit_Pool = new ObjectPool<HitEffect>(CreateHitEffect, OnGetHitEffect, OnReleaseHitEffect, OnDestroyHitEffect, maxSize: 40);
        blood_Pool = new ObjectPool<BloodSprayEffect>(CreateBloodSprayEffect, OnGetBloodSprayEffect, OnReleaseBloodSprayEffect, OnDestroyBloodSprayEffect, maxSize: 40);
    }

    private void Start()
    {        
        var player = GameObject.FindWithTag("Player").GetComponent<Player>();
        player.onDeath += EndGame;
        player.onCrossGoaLine += EndGame;
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

    public void Initialized()
    {
        foreach (var target in targets)
        {
            Destroy(target);
        }
        targets.Clear();
        hit_Pool.Clear();
        blood_Pool.Clear();
    }

    private void UpdateUI()
    {
        UIManager.instance.UpdateSurvivorText(targets.Count);
    }

    public void ShowRangking()
    {

    }

    public void BackToTitle()
    {        
        SceneManager.LoadScene("TitleScene");
        Initialized();
    }  

    public void ReStart()
    {
        Initialized();
        SceneManager.LoadScene("PlayScene");
    }


    private HitEffect CreateHitEffect()
    {
        HitEffect effect = Instantiate(HitEffect_Prefab).GetComponent<HitEffect>();
        effect.SetManagedPool(hit_Pool);
        return effect;
    }

    private void OnGetHitEffect(HitEffect effect)
    {
        effect.gameObject.SetActive(true);
    }

    private void OnReleaseHitEffect(HitEffect effect)
    {
        effect.gameObject.SetActive(false);
    }

    private void OnDestroyHitEffect(HitEffect effect)
    {
        Destroy(effect.gameObject);
    }

    private BloodSprayEffect CreateBloodSprayEffect()
    {
        BloodSprayEffect effect = Instantiate(BloodSpray_Prefab).GetComponent<BloodSprayEffect>();
        effect.SetManagedPool(blood_Pool);
        return effect;
    }

    private void OnGetBloodSprayEffect(BloodSprayEffect effect)
    {
        effect.gameObject.SetActive(true);
    }

    private void OnReleaseBloodSprayEffect(BloodSprayEffect effect)
    {
        effect.gameObject.SetActive(false);
    }

    private void OnDestroyBloodSprayEffect(BloodSprayEffect effect)
    {
        Destroy(effect.gameObject);
    }
}
