using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Tagger : MonoBehaviour
{
    float moveSpeed = 1f;

    float angle = 0f;
    float compareAngle = 10f;

    public enum TaggerStates
    {
        None = -1,
        Front,
        Back,
    }

    private TaggerStates taggerState = TaggerStates.None;

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
                case TaggerStates.Front:
                    {
                        StartCoroutine(RotateFrontImage());
                        break;
                    }
                case TaggerStates.Back:
                    {
                        StartCoroutine(RotateBackImage());
                        break;
                    }
            }
        }
    }

    IEnumerator RotateFrontImage()
    {
        while (Quaternion.Angle(transform.rotation, Quaternion.Euler(0, -180, 0)) > compareAngle)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, -180, 0), moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, -180, 0);
    }

    IEnumerator RotateBackImage()
    {
        while (Quaternion.Angle(transform.rotation, Quaternion.Euler(0, 0, 0)) > compareAngle) 
            {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 0), moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void Start()
    {
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            TaggerState = TaggerStates.Front;
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            TaggerState = TaggerStates.Back;
        }
    }
}
