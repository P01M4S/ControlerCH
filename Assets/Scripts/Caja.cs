using UnityEngine;

public class Caja : MonoBehaviour, IDamageEnable, IInteractuable, IGrabable
{
    public float _health;
    
    void IInteractuable.Interact()
    {
        Debug.Log("Yepa");
    }
    public void Grab()
    {
        Debug.Log("Epa");
    }
    public void TakeDamage(float damage)
    {
        _health -= damage;
        if(_health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
