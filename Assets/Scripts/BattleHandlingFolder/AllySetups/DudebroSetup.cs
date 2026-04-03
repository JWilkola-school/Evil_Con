using UnityEngine;

public class DudebroSetup : BaseAllySetup
{
    public DudebroSetup()
    {
        this.allyName = "DudeBro ManStrong";
        this.baseHP = 445f;
        this.currHP = 445f;
        this.baseDefense = 25f;
        this.currDefense = 25f;
        this.baseSpeed = 6f;
        this.currSpeed = 6f;
        this.baseDamage = 2.5f;
        this.currDamage = 2.5f;
        this.chargeTimeLeft = -1;
        this.canSpecial = true;
        this.characterPrefab = Resources.Load<GameObject>("Prefabs/DudeBro ManStrong (Battle)");
    }

    public override float attack1()
    {
        return basicAttack();
    }
    public override float attack2()
    {
        return basicAttack();
    }
    public override float attack3()
    {
        return basicAttack();
    }

    public override float attack4()
    {
        return basicAttack();
    }
}
