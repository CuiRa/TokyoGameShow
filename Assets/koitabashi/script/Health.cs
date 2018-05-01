using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public const int maxHealth = 1;
    public int currentHealth = maxHealth;


    public void Start()
    {

    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            GetComponent<MeshRenderer>().material.color = Color.blue;
            Debug.Log("Dead!");
        }
    }
}