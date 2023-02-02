using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Tagger : MonoBehaviour
{
    private float forntTurnSpeed;
    private float backTurnSpeed = 2f;

    private float compareAngle = 10f;

    public bool isKill = false;

    public enum TaggerStates
    {
        Back,
        Front,
    }

    private void Start()
    {
        forntTurnSpeed = Random.Range(1f, 3f);
    }

    private TaggerStates taggerState = TaggerStates.Back;

    public TaggerStates TaggerState
    {
        get { return taggerState; }
        private set
        {
            var prevState = taggerState;
            taggerState = value;

            if (prevState == taggerState)
                return;

            switch (taggerState)
            {
                case TaggerStates.Back:
                    {
                        StartCoroutine(RotateBackImage());
                        break;
                    }
                case TaggerStates.Front:
                    {
                        StartCoroutine(RotateFrontImage());
                        break;
                    }
            }
        }
    }

    IEnumerator RotateFrontImage()
    {
        while (Quaternion.Angle(transform.rotation, Quaternion.Euler(0, -180, 0)) > compareAngle)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, -180, 0), forntTurnSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, -180, 0);
        isKill = true;

    }

    IEnumerator RotateBackImage()
    {
        isKill = false;
        while (Quaternion.Angle(transform.rotation, Quaternion.Euler(0, 0, 0)) > compareAngle) 
            {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 0), backTurnSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void ChangeTaggerState()
    {
        if(taggerState == TaggerStates.Back)
        {
            TaggerState = TaggerStates.Front;
        }
        else
        {
            TaggerState = TaggerStates.Back;
        }
    }
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.F3))
    //    {
    //        TaggerState = TaggerStates.Front;
    //    }
    //    if (Input.GetKeyDown(KeyCode.F4))
    //    {
    //        TaggerState = TaggerStates.Back;
    //    }
    //}
}
