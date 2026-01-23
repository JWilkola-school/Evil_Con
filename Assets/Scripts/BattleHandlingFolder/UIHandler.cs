using UnityEngine;
using TMPro;

public class UIHandler : MonoBehaviour
{
    public TextMeshProUGUI enemyUI;

    public TextMeshProUGUI allyUI;

    float allyBaseHP;
    float enemyBaseHP;

    public void uiInit(float allyHP, float enemyHP)
    {
        allyBaseHP = allyHP;
        enemyBaseHP = enemyHP;

        allyUI.text = allyBaseHP + "/" + allyBaseHP;
        enemyUI.text = enemyBaseHP + "/" + enemyBaseHP;
    }

    public void updateHealthEnemy(float newHP)
    {
        enemyUI.text = newHP + "/" + enemyBaseHP;
    }

    public void updateHealthAlly(float newHP)
    {
        allyUI.text = newHP + "/" + allyBaseHP;
    }
}
