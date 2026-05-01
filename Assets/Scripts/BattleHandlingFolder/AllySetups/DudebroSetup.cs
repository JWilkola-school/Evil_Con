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
            effect = EffectType.None,
            selfEffect = EffectType.DamageUp,
            selfEffectDuration = 3,
            baseCooldown = 2
        };
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
            effectDuration = 3,
            baseCooldown = 3
        };
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
            effectDuration = 3,
            baseCooldown = 4
        };
    }
}
