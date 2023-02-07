using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BloodSprayEffect : MonoBehaviour
{
    private IObjectPool<BloodSprayEffect> _ManagedPool;

    public void SetManagedPool(IObjectPool<BloodSprayEffect> pool)
    {
        _ManagedPool = pool;
    }

    public void Bleeding()
    {
        Invoke("DestroyEffect", 1f);
    }

    public void DestroyEffect()
    {
        _ManagedPool.Release(this);
    }
}
