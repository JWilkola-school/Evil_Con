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
        this.characterPrefab = Resources.Load<GameObject>("Prefabs/DudeBro ManStrong");
        this.attackNames = new string[] { "Cleave", "Battle Cry", "Overhead Swing", "SMASH!!! (WIP)" };
    }

    public void doubleDamage() // aka battle cry
    {
        this.ApplyEffect(EffectType.DamageUp, 3);
        Debug.Log($"{allyName} let out a battle cry!");
    }

    public void chargeAttack()
    {
        this.pendingChargeDamage = this.currDamage * 3f;
        this.pendingChargeName = "Overhead Swing";
        Debug.Log($"{allyName} is raising his axe in the air!");
    }

    // Cleave: basic attack
    public override float attack1()
    {
        return basicAttack();
    }

    // Battle Cry: buffs damage
    public override float attack2()
    {
        doubleDamage();
        return -1.5f;
    }

    // Overhead Swing: high single target. Inflicts Crush
    public override float attack3()
    {
        chargeAttack();
        return -3f;
    }

    // SMASH!!!: AoE attack. Inflicts Crush
    public override float attack4()
    {
        return basicAttack();
    }
}
