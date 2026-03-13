using UnityEngine;
using System.Collections;

[System.Serializable]

//Class that is used to set up each hero for battle
public abstract class BaseAllySetup : BaseUnitSetup
{
    // Add any additional fields here!
    public string allyName;
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
}
