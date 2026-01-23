using UnityEngine;

public abstract class GenBattleObjects : MonoBehaviour
{
    public abstract float unitSpeed{ get;}
    public abstract string unitName{ get;}
    public abstract void localInit(GlobalBattleHandler globalBattleHandler);
    public abstract void localUpdate();
    public abstract void TakeAction();
    public abstract void addToList();
    public abstract void Die();
}

