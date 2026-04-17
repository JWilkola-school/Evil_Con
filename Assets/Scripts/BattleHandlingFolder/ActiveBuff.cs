using UnityEngine;

public enum StatType { Speed, Damage, Defense }

public class ActiveBuff
{
    public StatType targetStat;
    public int turnsLeft;

    // Constructor to create new buff
    public ActiveBuff(StatType stat, int turns)
    {
        this.targetStat = stat;
        this.turnsLeft = turns;
    }
}
