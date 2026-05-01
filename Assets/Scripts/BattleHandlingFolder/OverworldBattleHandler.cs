using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OverworldBattleHandler : MonoBehaviour
{
    // 1. Creates a universal "Instance" so the Battle Scene can find it instantly
    public static OverworldBattleHandler instance;

    public List<BaseAllySetup> allies;
    public List<BaseEnemySetup> enemies;

    void Awake()
    {
        // Make sure only 1 of these exists, and make it indestructible
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If overworld later, destroy the duplicate
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (allies == null) allies = new List<BaseAllySetup>();
        if (enemies == null) enemies = new List<BaseEnemySetup>();

        // Only load Dudebro if the list is empty (prevents duplicates)
        if (allies.Count == 0)
        {
            loadDudebro();
        }
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

    public void clearEnemies()
    {
        enemies.Clear();
    }
}