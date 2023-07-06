using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    private EnemyAI _enemyAI;
    
    public Stats stats = new Stats();

    private void Start()
    {
        _enemyAI = GetComponent<EnemyAI>();
        _enemyAI.Initialize(this);
    }
}
