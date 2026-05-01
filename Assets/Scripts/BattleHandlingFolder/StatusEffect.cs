using UnityEngine;

public enum EffectType
{
    SpeedUp, DamageUp, DefenseUp,
    Charmed, // attack down
    Tased, // speed down
    Adrenaline, // Extra action per turn
    Crush, // Lifesteal mark
    Cripple, // take bonus damage
    Hemorrhage // damage over time (bleed)
}

public class StatusEffect
{
    public EffectType type;
    public int turnsLeft;
    public float effectValue; // Stores the amount of bleed damage, or attack reduction!

    public StatusEffect(EffectType type, int turnsLeft, float effectValue = 0)
    {
        this.type = type;
        this.turnsLeft = turnsLeft;
        this.effectValue = effectValue;
    }
}
