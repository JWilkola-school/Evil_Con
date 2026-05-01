using System.Collections.Generic;
using UnityEngine;

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
    
    public int chargeTimeLeft = 0;
    public float pendingChargeDamage = 0; // holds damage for a turn
    public string pendingChargeName = ""; // holds name of attack for a turn
    public int pendingChargeTarget = 0; // Holds index position of enemy in the list that is targeted.


    public float basicAttack()
    {
        return currDamage;
    }
    public void ApplyEffect(EffectType type, int duration, float value = 0)
    {
        // 1. If it's a direct stat change, apply the math immediately!
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
            this.currDamage = this.baseDamage * 2f; // Assuming double damage for Battle Cry
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

    /*public void TickBuffs()
    {
        // loop backwards in the list
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            activeBuffs[i].turnsLeft--; // tick down the turn

            if (activeBuffs[i].turnsLeft <= 0)
            {
                // Buff expired. Find what stat to reset.
                RemoveBuffAndResetStat(activeBuffs[i].targetStat);

                // Remove from list
                activeBuffs.RemoveAt(i);
            }
        }
    }*/

    /*private void RemoveBuffAndResetStat(StatType stat)
    {
        // checks to see if there is another of the same buff active
        foreach (ActiveBuff buff in activeBuffs)
        {
            if (buff.targetStat == stat && buff.turnsLeft > 0)
            {
                Debug.Log($"{this.GetType().Name} still has another {stat} buff active! Skipping reset.");
                return;
            }
        }



        switch (stat)
        {
            case StatType.Speed:
                this.currSpeed = this.baseSpeed;
                break;
            case StatType.Damage:
                this.currDamage = this.baseDamage;
                break;
            case StatType.Defense:
                this.currDefense = this.baseDefense;
                break;
        }
        Debug.Log($"{this.GetType().Name}'s {stat} returned to normal!");
    }*/
}
