using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class MuzzleEffect : MonoBehaviour
{
    private IObjectPool<MuzzleEffect> _ManagedPool;

    public void SetManagedPool(IObjectPool<MuzzleEffect> pool)
    {
        _ManagedPool = pool;
    }

    public void Fire()
    {
        Invoke("DestroyEffect", 0.3f);
    }

    public void DestroyEffect()
    {
        _ManagedPool.Release(this);
    }
}
