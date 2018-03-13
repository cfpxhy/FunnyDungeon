using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;

public class FireAttackEffect : MonoBehaviour {
    //private SkillCD skillCD;
    private string poolName = "Effect";
    private SpawnPool effectSpawnPool;
    private ParticleSystem particle;
    private List<ParticleSystem> particleList;
    private ParticleSystem[] particleArray;
    [HideInInspector]
    public bool isAdd = false;

    void Start () {
        //skillCD = GameObject.Find("SkillButton").GetComponent<SkillCD>();
        effectSpawnPool = PoolManager.Pools[poolName];
	}

    void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            if (gameObject.tag == "Thunder")
            {
                effectSpawnPool.Despawn(transform, 0.7f);
            }
            else
            {
                effectSpawnPool.Despawn(transform, 1.2f);
            }
        }
    }
}
