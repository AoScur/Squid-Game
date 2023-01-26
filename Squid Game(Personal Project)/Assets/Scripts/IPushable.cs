using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPushable
{
    void OnPush(float strength, Vector3 hitPoint, Vector3 hitNormal);
}
