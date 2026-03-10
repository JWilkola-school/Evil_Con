using UnityEngine;
using System.Collections;

[System.Serializable]

//Class that is used to set up each hero for battle
public class BaseAllySetup : BaseUnitSetup
{
    // Add any additional fields here!
    public string allyName;
    // For charge attacks
    protected int chargeTimeLeft;
}
