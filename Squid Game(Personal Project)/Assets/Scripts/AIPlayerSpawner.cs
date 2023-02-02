using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIPlayerSpawner : MonoBehaviour
{
    BoxCollider rangeCollider;
    public int AIPlayerCount;

    public AIPlayer[] aiPrefab;

    public AIData[] aiDatas;

    static public List<LivingEntity> targets = new List<LivingEntity>();

    private void Awake()
    {
        rangeCollider = GetComponent<BoxCollider>();
    }
    private void Start()
    { 
        for(int i = 0; i< AIPlayerCount; i++)
        { 
            CreateAI();
        }
    }

    private bool RandomPoint(out Vector3 result)
    {
        float range_X = rangeCollider.bounds.size.x;
        float range_Z = rangeCollider.bounds.size.z;

        range_X = Random.Range((range_X / 2) * -1, range_X / 2);
        range_Z = Random.Range((range_Z / 2) * -1, range_Z / 2);
        Vector3 randomPostion = new Vector3(range_X, 0f, range_Z);
        NavMeshHit hit;
        if(NavMesh.SamplePosition(randomPostion,out hit, 3.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    private void CreateAI()
    {
        Vector3 point;
        var aiType = aiPrefab[Random.Range(0, aiPrefab.Length)];
        AIData data = null;
        if (aiType.name.ToCharArray().GetValue(1).Equals('e'))
        {
            data = aiDatas[0];
        }
        else if (aiType.name.ToCharArray().GetValue(1).Equals('p'))
        {
            data = aiDatas[1];
        }
        else if (aiType.name.ToCharArray().GetValue(1).Equals('t'))
        {
            data = aiDatas[2];
        }

        if (RandomPoint(out point))
        {
            var ai = Instantiate(aiType, point, transform.rotation);
            ai.Setup(data);
            targets.Add(ai);

            ai.onDeath += () => targets.Remove(ai);
        }

    }
}
