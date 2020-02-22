using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollisionHandler : MonoBehaviour {
    [SerializeField]
    Animation SwordAttack;


    private void OnTriggerStay(Collider other)
    {
        if ((!SwordAttack.isPlaying) && Input.GetKeyDown(KeyCode.Q) && other.transform.tag == "Enemy") //If the animation isnt playing already, and the the user pressed q, it must be an attack
        {
            other.GetComponent<EnemyAI>().Damage(4);
            Rigidbody rb = other.GetComponent<Rigidbody>();
            rb.AddExplosionForce(1, transform.position, 5, 0, ForceMode.Impulse);//knock back enemies hit
        }
    }
}
