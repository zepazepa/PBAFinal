using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombHit : MonoBehaviour
{
    public Rigidbody tank, bomb;
    public HealthBar healthBar;
    public ParticleSystem smoke1, smoke2;
    public ParticleSystem fire;
    public int damage = 5;
    public int health;
    int maxHealth;

    // Start is called before the first frame update
    void Start()
    {
        tank = GetComponent<Rigidbody>();
        maxHealth = healthBar.health; // ambil max health
        health = maxHealth; 

        smoke1.enableEmission = false;
        smoke2.enableEmission = false;
        fire.enableEmission = false;
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.health = health;
        if (health / (double)maxHealth <=0.75 && health / (double)maxHealth > 0.5)
        {
            smoke1.enableEmission = true;
        }
        else if (health / (double)maxHealth <= 0.5 && health / (double)maxHealth > 0.25)
        {
            smoke1.enableEmission = false;
            smoke2.enableEmission = true;

        }
        else if (health / (double)maxHealth <= 0.25 && health / (double)maxHealth > 0)
        {
            fire.enableEmission = true;
        }
        else
        {
            Destroy(tank);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        bomb = collision.rigidbody;
        health -= damage;

    }
}
