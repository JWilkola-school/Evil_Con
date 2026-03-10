using UnityEngine;

public class SecurityGuardSetup : BaseAllySetup
{
    public SecurityGuardSetup()
    {
        this.allyName = "Security Guard";
        this.baseHP = 150f;
        this.currHP = 150f;
        this.baseDefense = 10f;
        this.currDefense = 10f;
        this.baseSpeed = 4f;
        this.currSpeed = 4f;
        this.baseDamage = 3f;
        this.currDamage = 3f;
        this.chargeTimeLeft = -1;
        this.canSpecial = false;
        this.characterPrefab = Resources.Load<GameObject>("Prefabs/Security Guard (Battle)");
    }

    public void doubleSpeed()
    {
        this.currSpeed = this.currSpeed * 2;
    }
}

