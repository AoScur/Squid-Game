using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Killer : MonoBehaviour
{
    public GameObject muzzleParticle_Prefab;

    public GameObject[] turrets;
    public Transform[] firePos;

    private List<LivingEntity> targets;

    private IObjectPool<MuzzleEffect> _Pool;

    private void Awake()
    {
        _Pool = new ObjectPool<MuzzleEffect>(CreateEffect,OnGetEffect,OnReleaseEffect,OnDestroyEffect,maxSize:40);
    }

    void Start()
    {
        Tagger.OnKill += KillPlayer;
        targets = GameManager.targets;

    }

    private void KillPlayer(bool obj)
    {
        if (obj)
        {
            for (int i = targets.Count - 1; i >=0 ; i--)
            {
                if (targets[i].GetComponent<LivingEntity>().State != LivingEntity.States.Idle)
                {
                    targets[i].State = LivingEntity.States.Die;
                    var muzzleEffect = _Pool.Get();
                    muzzleEffect.transform.position = firePos[Random.Range(0,firePos.Length-1)].position;
                    muzzleEffect.Fire();
                }
            }
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
        var muzzleEffect = _Pool.Get();
        muzzleEffect.transform.position = firePos[0].position;
        muzzleEffect.Fire();

        }
    }

    private MuzzleEffect CreateEffect()
    {
        MuzzleEffect effect = Instantiate(muzzleParticle_Prefab).GetComponent<MuzzleEffect>();
        effect.SetManagedPool(_Pool);
        return effect;
    }

    private void OnGetEffect(MuzzleEffect effect)
    {
        effect.gameObject.SetActive(true);
    }

    private void OnReleaseEffect(MuzzleEffect effect)
    {
        effect.gameObject.SetActive(false);
    }

    private void OnDestroyEffect(MuzzleEffect effect)
    {
        Destroy(effect.gameObject);
    }
}
