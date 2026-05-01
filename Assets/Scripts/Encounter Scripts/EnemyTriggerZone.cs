using UnityEngine;

public class EnemyTriggerZone : MonoBehaviour
{
    // We keep this private so the Inspector doesn't even show it
    private GameObject myActualEnemy;

    void Start()
    {
        // AUTOMATION: Tell the trigger to look at its parent object and 
        // grab the main OverworldEnemy script/object automatically!
        OverworldEnemy parentScript = GetComponentInParent<OverworldEnemy>();

        if (parentScript != null)
        {
            myActualEnemy = parentScript.gameObject;
        }
        else
        {
            Debug.LogError("Trigger zone couldn't find an OverworldEnemy parent!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // We pass the automatically discovered enemy and the player!
            BattleTransitioner.InitiateForcedCombat(myActualEnemy, other.gameObject);
        }
    }
}
