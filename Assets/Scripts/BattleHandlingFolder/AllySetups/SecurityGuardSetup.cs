using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

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

    // Beat: basic attack
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

    // Leg Workout: Doubles Speed of Security Guard
    public override ActionPayload attack2()
    {
        this.ApplyEffect(EffectType.SpeedUp, 3);
        return new ActionPayload
        {
            type = ActionType.Buff,
            actionName = attackNames[1],
            isAOE = false,
            effect = EffectType.None,
            selfEffect = EffectType.SpeedUp,
            selfEffectDuration = 3,
            baseCooldown = 2
        };
    }

    // FREEZE!!!: Single target, Inflicts Tased
    public override ActionPayload attack3()
    {
        return new ActionPayload
        {
            type = ActionType.Attack,
            actionName = attackNames[2],
            value = basicAttack(),
            isAOE = false,
            effect = EffectType.Tased,
            effectDuration = 3,
            effectValue = 3f,
            baseCooldown = 2
        };
    }

    // WHERE'S YOUR BADGE?!?!?!?: Single target, buffs Security Guard with Adrenaline
    public override ActionPayload attack4()
    {
        this.ApplyEffect(EffectType.Adrenaline, 3);
        return new ActionPayload
        {
            type = ActionType.Attack,
            actionName = attackNames[3],
            value = basicAttack() * 1.2f,
            isAOE = false,
            effect = EffectType.None,
            selfEffect = EffectType.Adrenaline,
            selfEffectDuration = 1,
            baseCooldown = 5
        };
    }
}

