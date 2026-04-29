using UnityEngine;
using TMPro;

public class UIHandler : MonoBehaviour
{
    public TextMeshProUGUI[] enemyNames;
    public TextMeshProUGUI[] enemyHealths;
    public TextMeshProUGUI[] allyNames;
    public TextMeshProUGUI[] allyHealths;

    private float[] allyBaseHPs;
    private float[] enemyBaseHPs;

    public void uiInit(BaseAllySetup[] allies, BaseEnemySetup[] enemies)
    {
        allyBaseHPs = new float[allies.Length];
        enemyBaseHPs = new float[enemies.Length];

        // Hide all text by default
        foreach (var txt in allyNames) if (txt != null) txt.gameObject.SetActive(false);
        foreach (var txt in allyHealths) if (txt != null) txt.gameObject.SetActive(false);
        foreach (var txt in enemyNames) if (txt != null) txt.gameObject.SetActive(false);
        foreach (var txt in enemyHealths) if (txt != null) txt.gameObject.SetActive(false);

        // Turn on and set text for each Ally
        for (int i = 0; i < allies.Length; i++)
        {
            if (i < allyHealths.Length && allyHealths[i] != null)
            {
                allyBaseHPs[i] = allies[i].baseHP;

                allyNames[i].gameObject.SetActive(true);
                allyNames[i].text = allies[i].allyName;

                allyHealths[i].gameObject.SetActive(true);
                allyHealths[i].text = allies[i].currHP + "/" + allies[i].baseHP;
            }
        }

        // Turn on and set text for each Enemy
        for (int i = 0; i < enemies.Length; i++)
        {
            if (i < enemyHealths.Length && enemyHealths[i] != null)
            {
                enemyBaseHPs[i] = enemies[i].baseHP;

                enemyNames[i].gameObject.SetActive(true);
                enemyNames[i].text = enemies[i].enemyName;

                enemyHealths[i].gameObject.SetActive(true);
                enemyHealths[i].text = enemies[i].currHP + "/" + enemies[i].baseHP;
            }
        }
    }

    public void updateHealthEnemy(int targetIndex, float newHP)
    {
        if (targetIndex < enemyHealths.Length && enemyHealths[targetIndex] != null)
        {
            enemyHealths[targetIndex].text = newHP + "/" + enemyBaseHPs[targetIndex];
        }
    }

    public void updateHealthAlly(int targetIndex, float newHP)
    {
        if (targetIndex < allyHealths.Length && allyHealths[targetIndex] != null)
        {
            allyHealths[targetIndex].text = newHP + "/" + allyBaseHPs[targetIndex];
        }
    }
}
