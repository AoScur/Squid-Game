using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���� �� AI�� �ൿ���� ����
public interface SetPattern
{
    public void Pattern(float goalZ, float objZ);
}

public class HeavyAIPatten : SetPattern
{
    public void Pattern(float goalZ, float objZ)
    {
        if (Mathf.Abs(goalZ-objZ) > 100)
        {
            //Random();
        }
        else if (Mathf.Abs(goalZ - objZ) > 30)
        {
            //Random();
        }
        else
        {
            //RunOnly();
        }
    }
}

public class FastAIPatten : SetPattern
{
    public void Pattern(float goalZ, float objZ)
    {
        if (Mathf.Abs(goalZ - objZ) > 100)
        {
            //RunOnly();
        }
        else if (Mathf.Abs(goalZ - objZ) > 30)
        {
            //Random();
        }
        else
        {
            //RunOnly();
        }
    }
}

public class StrongAIPatten : SetPattern
{
    public void Pattern(float goalZ, float objZ)
    {
        if (Mathf.Abs(goalZ - objZ) > 100)
        {
            //Fight();
        }
        else if (Mathf.Abs(goalZ - objZ) > 30)
        {
            //Random();
        }
        else
        {
            //RunOnly();
        }
    }
}