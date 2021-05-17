using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    // position from which the fireballs will be fired
    [SerializeField] private Transform firePoint;
    // place all temp fireballs that were created
    [SerializeField] private GameObject[] fireballs;
    private Animator anim;
    private PlayerMovement playerMovement;
    private float cooldownTimer = Mathf.Infinity;

    private void Awake() {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    private void Update() {
        if (Input.GetMouseButton(0) && cooldownTimer > attackCooldown && playerMovement.canAttack()) {
            Attack();
        }
        cooldownTimer += Time.deltaTime;
    }

    private void Attack() {
        anim.SetTrigger("attack");
        cooldownTimer = 0;
        // Object pooling - pool fireballs - 
        // Multiple fireballs already created. 
        // Fireball deactivated on hit and waits to be reused. 
        // Recommended when you're creating lots of objects

        // Everytime fireball is fired/attack() a fireball will be reset to the position 
        // of the firePoint
        fireballs[FindFireballs()].transform.position = firePoint.position;
        // using the SetDirection method from the projectile component. 
        // gives what direction player is facingg and which direction fireballs should face
        fireballs[FindFireballs()].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
    }

    private int FindFireballs() {
        for (int i = 0; i < fireballs.Length; i++) {
            // check if specific fireball is not active in the hierarchy
            if (!fireballs[i].activeInHierarchy) {
                // returns its index to the attack() method so that it can use it to fire
                // for example, fireball #3 is not active. It returns that index and it
                // and that fireball can/will be used for attack()/firing
                return i;
            }
        }
        return 0;
    }


    // Start is called before the first frame update
    void Start() {

    }
}
