using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerSpawner : MonoBehaviour
{
    public int AIPlayerCount;

    public AIPlayer[] aiPrefab;

    public AIData[] aiDatas;
    public Transform[] createPoints;

    private List<LivingEntity> targets = new List<LivingEntity>();

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
        var aiType = aiPrefab[Random.Range(0, aiPrefab.Length)];
        var data = aiDatas[Random.Range(0,aiDatas.Length)];

        var ai = Instantiate(aiType, point.position, point.rotation);
        ai.Setup(data);
        targets.Add(ai);

        ai.onDeath += () => targets.Remove(ai);
    }
}
