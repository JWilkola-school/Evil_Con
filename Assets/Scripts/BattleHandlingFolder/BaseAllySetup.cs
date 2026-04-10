using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;

[System.Serializable]

//Class that is used to set up each hero for battle
public abstract class BaseAllySetup : BaseUnitSetup
{
    // Add any additional fields here!
    public string allyName;
    public string[] attackNames = new string[4];
    // Dynamic list to hold active buffs
    public List<ActiveBuff> activeBuffs = new List<ActiveBuff>();

    // For charge attacks
    protected int chargeTimeLeft;

    // For attacks, return a float of the amount of damage an attack does.
    // What about status moves? Healing moves? Charging Moves?
    // Damaging: 0 and above
    // Non-Damaging: -1 to -2
    // Charging -3 to -4
    // Healing: -5 and below
    // Why are ranges being used? Equality can be unpredictable for floating point values.
    // Mitigate this by using ranges instead.

    // These of course can be moved to BaseUnitSetup so that the enemy setup can inherit it if needbe
    public abstract float attack1();
    public abstract float attack2();
    public abstract float attack3();
    public abstract float attack4();

    // Counts buffs and handles the turn count of the buff until expiry
    public void TickBuffs()
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
    }

    private void RemoveBuffAndResetStat(StatType stat)
    {
        switch (stat)
        {
            case StatType.Speed:
                this.currSpeed = this.baseSpeed;
                Debug.Log($"{allyName}'s Speed returned to normal!");
                break;
            case StatType.Damage:
                this.currDamage = this.baseDamage;
                Debug.Log($"{allyName}'s Damage returned to normal!");
                break;
            case StatType.Defense:
                this.currDefense = this.baseDefense;
                Debug.Log($"{allyName}'s Defense returned to normal!");
                break;
        }
    }
}
