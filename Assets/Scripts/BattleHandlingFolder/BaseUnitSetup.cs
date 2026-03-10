using UnityEngine;

public class BaseUnitSetup
{
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

    public GameObject characterPrefab;

    public float basicAttack()
    {
        return currDamage;
    }
}
