using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPushable
{
    void OnPush(Vector3 hitPoint, Vector3 hitNormal);
}
