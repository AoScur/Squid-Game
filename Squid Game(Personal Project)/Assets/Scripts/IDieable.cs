using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDieable
{
    void OnDie(Vector3 hitPoint, Vector3 hitNormal);
}
