using Unity.VisualScripting;
using UnityEngine;

public class FurlingSetup : BaseEnemySetup
{
    public FurlingSetup()
    {
        this.enemyName = "Furling";

        this.baseHP = 100f;
        this.currHP = 100f;

        this.baseDamage = 5f;
        this.currDamage = 5f;

        this.baseDefense = 2f;
        this.currDefense = 2f;

        this.baseSpeed = 5f;
        this.currSpeed = 5f;

        this.canSpecial = false; 
    }
}
