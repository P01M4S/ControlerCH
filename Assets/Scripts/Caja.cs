using UnityEngine;

public class Caja : MonoBehaviour, IDamageEnable, IInteractuable
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    void IDamageEnable.TakeDamage()
    {
        Debug.Log("Auch");
    }

    void IInteractuable.Interact()
    {
        Debug.Log("Yepa");
    }
}
