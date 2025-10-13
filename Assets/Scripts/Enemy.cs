using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float movementSpeed = 5;
    public float actionDamage = 10;
    public void Movement()
    {
        Debug.Log("movimiento");
    }

    public virtual void Attack()
    {
        Debug.Log("Ataquel");
    }
}
