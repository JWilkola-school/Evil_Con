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
        this.attackNames = new string[] { "Cleave", "Battle Cry", "Overhead Swing", "SMASH!!!" };
    }

    /*public void doubleDamage() // aka battle cry
    {
        this.ApplyEffect(EffectType.DamageUp, 3);
        Debug.Log($"{allyName} let out a battle cry!");
    }

    public void chargeAttack()
    {
        this.pendingChargeDamage = this.currDamage * 3f;
        this.pendingChargeName = "Overhead Swing";
        Debug.Log($"{allyName} is raising his axe in the air!");
    }*/

    // Cleave: basic attack
    public override ActionPayload attack1()
    {
        return new ActionPayload 
        { 
            type = ActionType.Attack, 
            actionName = attackNames[0], 
            value = basicAttack(), 
            isAOE = false,
            effect = EffectType.None
        };
        //basicAttack();
    }

    // Battle Cry: buffs damage
    public override ActionPayload attack2()
    {
        this.ApplyEffect(EffectType.DamageUp, 3);
        return new ActionPayload
        {
            type = ActionType.Buff,
            actionName = attackNames[1],
            isAOE = false,
            effect = EffectType.None
        };
        //doubleDamage();
        //return -1.5f;
    }

    // Overhead Swing: high single target. Inflicts Crush
    public override ActionPayload attack3()
    {
        return new ActionPayload
        {
            type = ActionType.Charge,
            actionName = attackNames[2],
            value = basicAttack() * 3f,
            isAOE = false,
            effect = EffectType.Crush,
            effectDuration = 3
        };
        //chargeAttack();
        //return -3f;
    }

    // SMASH!!!: AoE attack. Inflicts Crush
    public override ActionPayload attack4()
    {
        return new ActionPayload
        {
            type = ActionType.Attack,
            actionName = attackNames[3],
            value = basicAttack() * 0.8f,
            isAOE = true,
            effect = EffectType.Crush,
            effectDuration = 3
        };
        //return basicAttack() * 0.8f;
    }
}
