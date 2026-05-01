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
        this.characterPrefab = Resources.Load<GameObject>("Prefabs/Security Guard");
        this.attackNames = new string[] { "Beat", "Leg Workout", "FREEZE!!!", "WHERE'S YOUR BADGE?!?!?!?" };
    }

    public void doubleSpeed()
    {
        this.ApplyEffect(EffectType.SpeedUp, 3);
        Debug.Log($"{allyName} doubled their speed for 3 turns!");
    }

    // Beat: basic attack
    public override float attack1() 
    {
        return basicAttack();
    }

    // Leg Workout: Doubles Speed of Security Guard
    public override float attack2()
    {
        doubleSpeed();
        return -1.5f;
    }

    // FREEZE!!!: Single target, Inflicts Stun
    public override float attack3()
    {
        return basicAttack();
    }

    // WHERE'S YOUR BADGE?!?!?!?: Single target, buffs Security Guard with Adrenaline
    public override float attack4()
    {
        return basicAttack();
    }
}

