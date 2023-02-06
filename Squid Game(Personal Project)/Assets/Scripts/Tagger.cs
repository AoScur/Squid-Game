using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class Tagger : MonoBehaviour
{
    private float frontTurnSpeed;
    private float backTurnSpeed = 2f;

    private float compareAngle = 10f;

    [HideInInspector]
    public static event Action<bool> OnKill;
    private bool isFront = false;
    private bool isFirstDelay = true;

    private GameManager gm;
    

    private void Start()
    {
        gm = GameManager.instance;
        frontTurnSpeed = UnityEngine.Random.Range(1f, 3f);
        StartCoroutine(RotateTagger());
    }

    IEnumerator RotateTagger()
    {
        while (!gm.isGameover)
        {
            if(isFirstDelay)
            {
                yield return new WaitForSeconds(3f);
                isFirstDelay = false;
            }
            if (!isFront)
            {
                while (Quaternion.Angle(transform.rotation, Quaternion.Euler(0, -180, 0)) > compareAngle)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, -180, 0), frontTurnSpeed * Time.deltaTime);
                    yield return null;
                }
                transform.rotation = Quaternion.Euler(0, -180, 0);
                OnKill?.Invoke(true);
                yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 2f));
                isFront = true;
            }
            else
            {
                OnKill?.Invoke(false);
                while (Quaternion.Angle(transform.rotation, Quaternion.Euler(0, 0, 0)) > compareAngle)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 0), backTurnSpeed * Time.deltaTime);
                    yield return null;
                }
                transform.rotation = Quaternion.Euler(0, 0, 0);
                yield return new WaitForSeconds(UnityEngine.Random.Range(3f, 5f));
                isFront = false;
            }
        }
    }
}
