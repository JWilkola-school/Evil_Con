using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OverworldBattleHandler : MonoBehaviour
{
    public List<BaseAllySetup> allies;
    public List<BaseEnemySetup> enemies;
    void Start()
    {
        allies = new List<BaseAllySetup>();
        enemies = new List<BaseEnemySetup>();
        loadDudebro();
    }

    public void loadDudebro()
    {
        allies.Add(new DudebroSetup());
    }

    public void addEnemy(BaseEnemySetup enemy)
    {
        enemies.Add(enemy);
    }

    public void addAlly(BaseAllySetup ally)
    {
        allies.Add(ally);
    }

    public BaseAllySetup[] getAllies()
    {
        return allies.ToArray();
    }

    public BaseEnemySetup[] getEnemies()
    {
        return enemies.ToArray();
    }

    public void clear()
    {
        allies.Clear();
        enemies.Clear();
        loadDudebro();
    }
}
