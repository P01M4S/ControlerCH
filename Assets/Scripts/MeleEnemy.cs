using UnityEngine;
using UnityEngine.Timeline;

public class MeleEnemy : Enemy, IDamageEnable
{
    float maxHealth = 50;
    float currentHealth = 25;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Attack();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Attack()
    {
        base.Attack();
        Debug.Log("Que te meto");
    }

    public void TakeDamage()
    {
        throw new System.NotImplementedException();
    }
    void IDamageEnable.TakeDamage(float damage)
    {
        Debug.Log("Auch");
    }
}
