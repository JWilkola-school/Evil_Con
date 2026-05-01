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

    public void doubleDamage()
    {
        this.currDamage = this.baseDamage * 2;
        ActiveBuff newBuff = new ActiveBuff(StatType.Damage, 3);
        this.activeBuffs.Add(newBuff);
        Debug.Log($"{allyName} doubled their damage for 3 turns!");
    }

    public void chargeAttack()
    {
        this.pendingChargeDamage = this.currDamage * 3f;
        this.pendingChargeName = "Overhead Swing";
        Debug.Log($"{allyName} is raising his axe in the air!");
    }

    public override float attack1()
    {
        return basicAttack();
    }
    public override float attack2()
    {
        doubleDamage();
        return -1.5f;
    }
    public override float attack3()
    {
        chargeAttack();
        return -3f;
    }

    public override float attack4()
    {
        
        return basicAttack();
    }
}
