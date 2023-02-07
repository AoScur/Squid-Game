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

    public LayerMask targetsLayerMask;

    private IObjectPool<MuzzleEffect> _Pool;

    private void Awake()
    {
        _Pool = new ObjectPool<MuzzleEffect>(CreateEffect,OnGetEffect,OnReleaseEffect,OnDestroyEffect,maxSize:40);
    }

    private void Start()
    {
        Tagger.OnKill += KillPlayer;
        targets = GameManager.targets;
        GameManager.instance.onGameOver += EndGame;
    }

    private void OnDisable()
    {
        Tagger.OnKill -= KillPlayer;
    }


    private void KillPlayer(bool obj)
    {
        if (obj)
        {
            for (int i = targets.Count - 1; i >=0 ; i--)
            {
                if (targets[i].GetComponent<LivingEntity>().State != LivingEntity.States.Idle)
                {
                    RaycastHit hitInfo;
                    
                    var dir = (targets[i].transform.position + new Vector3(0, 1.4f, 0)) - firePos[3].position;
                    if (Physics.Raycast(firePos[3].position, dir, out hitInfo,100f,targetsLayerMask))
                    {
                        if (hitInfo.collider.gameObject == targets[i].gameObject)
                        {
                            targets[i].OnDie(hitInfo.point,dir);
                            //ÃÑ ½î´Â ÀÌÆåÆ®
                            var fireTurretIndex = Random.Range(0, firePos.Length - 1);
                            var muzzleEffect = _Pool.Get();
                            muzzleEffect.GetComponent<ParticleSystem>().Play();
                            muzzleEffect.transform.position = firePos[fireTurretIndex].position;
                            muzzleEffect.Fire();
                        }
                    }
                }
            }
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

    private void EndGame()
    {
        _Pool.Clear();
    }
}
