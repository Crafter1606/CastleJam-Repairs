using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public Action Died;

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Item")) Heal();
    }

    public void Damage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth > 0) return;
        currentHealth = 0;

        Died?.Invoke();
    }

    public void Heal()
    {
        Debug.Log("Healed");
        currentHealth += 1;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }
}
