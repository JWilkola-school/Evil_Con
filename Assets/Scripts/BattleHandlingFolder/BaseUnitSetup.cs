using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BaseUnitSetup
{
    public float baseHP;
    public float currHP;

    public float baseDamage;
    public float currDamage;

    public float baseDefense;
    public float currDefense;

    public float baseSpeed;
    public float currSpeed;

    public bool canSpecial;
    public bool isBlocking;

    public GameObject characterPrefab;

    public string[] attackNames = new string[4];
    public List<StatusEffect> activeEffects = new List<StatusEffect>();
    public int[] currentCooldowns = new int[4];
    
    public int chargeTimeLeft = 0;
    public int pendingChargeTarget = 0; // Holds index position of enemy in the list that is targeted.

    public float basicAttack()
    {
        return currDamage;
    }
    public void ApplyEffect(EffectType type, int duration, float value = 0)
    {
        // Check if the already have this effect
        foreach (StatusEffect effect in activeEffects)
        {
            if (effect.type == type)
            {
                // Just refresh the timer to the maximum duration and stop!
                effect.turnsLeft = Mathf.Max(effect.turnsLeft, duration);
                Debug.Log($"{this.GetType().Name}'s {type} was refreshed to {effect.turnsLeft} turns!");
                return;
            }
        }

        // Brand new effect =  normal math
        if (type == EffectType.Charmed)
        {
            this.currDamage = Mathf.Max(1f, this.currDamage - value);
            Debug.Log($"{this.GetType().Name} was Charmed! Attack dropped by {value}.");
        }
        else if (type == EffectType.Tased)
        {
            this.currSpeed = Mathf.Max(1f, this.currSpeed - value);
            Debug.Log($"{this.GetType().Name} was Tased! Speed dropped by {value}.");
        }
        else if (type == EffectType.DamageUp)
        {
            this.currDamage = this.baseDamage * 2f;
        }
        else if (type == EffectType.SpeedUp)
        {
            this.currSpeed = this.baseSpeed * 2f;
        }

        // 2. Store the effect so it counts down
        activeEffects.Add(new StatusEffect(type, duration, value));
    }

    public void TickEffects()
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            StatusEffect effect = activeEffects[i];

            // Trigger Hemorrhage (Bleed) Damage at the exact start of the turn!
            if (effect.type == EffectType.Hemorrhage)
            {
                this.currHP -= effect.effectValue;
                this.currHP = Mathf.Max(0, this.currHP);
                Debug.Log($"{this.GetType().Name} bled for {effect.effectValue} damage!");
                // Note: You will need to tell the UI_Handler to update the health bar here in your State Machine!
            }

            effect.turnsLeft--;

            if (effect.turnsLeft <= 0)
            {
                RemoveEffectAndReset(effect);
                activeEffects.RemoveAt(i);
            }
        }
    }
    public void TickCooldowns()
    {
        for (int i = 0; i < currentCooldowns.Length; i++)
        {
            if (currentCooldowns[i] > 0)
                currentCooldowns[i]--;
        }
    }

    private void RemoveEffectAndReset(StatusEffect effect)
    {
        // Revert flat stat changes when they expire
        if (effect.type == EffectType.Charmed)
        {
            this.currDamage += effect.effectValue; // Give the attack back!
        }
        else if (effect.type == EffectType.Tased)
        {
            this.currSpeed += effect.effectValue;
        }
        else if (effect.type == EffectType.DamageUp)
        {
            this.currDamage = this.baseDamage;
        }
        else if (effect.type == EffectType.SpeedUp)
        {
            this.currSpeed = this.baseSpeed;
        }
    }

    public bool HasEffect(EffectType type)
    {
        foreach (StatusEffect effect in activeEffects)
        {
            if (effect.type == type) return true;
        }
        return false;
    }
}

public enum ActionType { Attack, Buff, Debuff, Charge }

[System.Serializable]
public class ActionPayload
{
    public ActionType type;
    public string actionName;
    public float value;             // Replaces the old float return damage
    public float effectValue;       // Use for heal, bleed, or stat drops
    public bool isAOE;              // Automatically bypasses targeting if true!
    public EffectType effect;       // What debuff does it apply? (Use EffectType.None if it doesn't)
    public int effectDuration;      // How many turns does it last?
    public int baseCooldown;        // Cooldown.
    public EffectType selfEffect;   // buffs applied to self
    public int selfEffectDuration;  // duration of self buff
}
