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

    public abstract void attack1();
    public abstract void attack2();
    public abstract void attack3();
    public abstract void attack4();
}
