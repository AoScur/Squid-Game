using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killer : MonoBehaviour
{
    private List<LivingEntity> targets;

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
                }
            }
        }
    }
}
