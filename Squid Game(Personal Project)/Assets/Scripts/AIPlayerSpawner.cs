using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerSpawner : MonoBehaviour
{
    public int AIPlayerCount;

    public AIPlayer aiPrefab;

    public AIData[] aiDatas;
    public Transform[] createPoints;

    private List<LivingEntity> targets;

    private void Start()
    {
        for(int i = 0; i< AIPlayerCount; i++)
        {
            CreateAI();
        }
    }

    private void CreateAI()
    {
        var point = createPoints[Random.Range(0, createPoints.Length)];
        var ai = GetComponent<AIPlayer>();
        var data = aiDatas[Random.Range(0,aiDatas.Length)];
        ai.Setup(data);

        ai.onDeath += () => targets.Remove(ai);
    }
}
