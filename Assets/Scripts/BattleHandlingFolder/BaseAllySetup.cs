using UnityEngine;
using System.Collections;

[System.Serializable]

//Class that is used to set up each hero for battle
public class BaseAllySetup
{
    //The characters name
    public string allyName;

    //The characters stats, base and current because they could change mid battle
    public float baseHP;
    public float currHP;
    
    public float baseDefense;
    public float currDefense;
    
    public float baseDamage;
    public float currDamage;

    public float baseSpeed;
    public float currSpeed;

    public bool canSpecial;
    public bool isBlocking;
}
