using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TriggerDamage : MonoBehaviour
{
    public HeartSystem heartSystem;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            heartSystem.vida -= 1;
        }
    }
}
