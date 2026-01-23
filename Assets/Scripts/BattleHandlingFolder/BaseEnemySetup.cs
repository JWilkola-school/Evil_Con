using UnityEngine;
using System.Collections;

[System.Serializable]

//Class that is used to set up each enemy for battle
public class BaseEnemySetup
{
    //Give the enemy a name
    public string enemyName;

    public float baseHP;
    public float currHP;

    public float baseDamage;
    public float currDamage;

    public float baseDefense;
    public float currDefense;
    
    public float baseSpeed;
    public float currSpeed;

    public bool canSpecial;
    public bool isBlocking;
    
}
