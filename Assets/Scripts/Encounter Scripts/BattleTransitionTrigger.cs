using UnityEngine;

public class BattleTransitionTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BattleTransitioner.InitiateForcedCombat(this.gameObject);
        }
    }
}
