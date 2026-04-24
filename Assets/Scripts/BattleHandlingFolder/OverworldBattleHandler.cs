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
        // 2. The Singleton Pattern: Make sure only ONE of these exists, and make it indestructible
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // THIS IS THE MAGIC LINE!
        }
        else
        {
            // If we walk back into the Overworld later, destroy the duplicate
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // 3. Fix the wipeout bug: ONLY create new lists if they are completely null.
        // This preserves the "Furling" you set up in the Unity Inspector!
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

    public void clear()
    {
        allies.Clear();
        enemies.Clear();
        loadDudebro();
    }
}