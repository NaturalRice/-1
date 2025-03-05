using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {
    public float currentHealth;
    public void Heal(float amount) {
        currentHealth += amount;
    }
}
