using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput; 

    private void Awake() {
        // grab references for rigidbody and animator from game object
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    private void Update() {
        horizontalInput = Input.GetAxis("Horizontal");

        // flip player when moving left or right
        if(horizontalInput > 0.01f) {
            transform.localScale = Vector3.one; // shorthand for (1,1,1)
        } else if (horizontalInput < -0.01f) {
            transform.localScale = new Vector3(-1, 1, 1);
        }



        // set animator parameters
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());

        // wall jump logic
        if (wallJumpCooldown > 0.2f) {
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            if (onWall() && !isGrounded()) {
                // when player is on wall. he will get stuck and not fall down
                body.gravityScale = 0;
                body.velocity = Vector2.zero;
            } 
            else {
                body.gravityScale = 5;
            }

            if (Input.GetKey(KeyCode.Space)) {
                jump();
            }


        } else {
            wallJumpCooldown += Time.deltaTime;
        }

        //print(onWall() + ":wall");
        //print(isGrounded() + ":ground");
    }

    private void jump() {
        if (isGrounded()) {
            body.velocity = new Vector2(body.velocity.x, jumpSpeed);
            anim.SetTrigger("jump");
        }
        else if (onWall() && !isGrounded()){
            // for getting off wall
            // Vector2(direction, pushed horizontally speed, pushed up speed set to zero since it for no direction pressed)
            // Vector2(direction, pushed horizontally speed, pushed up speed set to zero since it for no direction pressed)
            if (horizontalInput == 0) {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
                // flip player in opposite direction once pushed off wall.
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else {
                // for going up walls
                // get new vector2. Find out which direction the player is facing and create a force opposite to it.
                // Mathf.Sign() when it gets a negative number it returns -1 and returns 1 for positive numbers
                // Vector2(direction player is facing * velocity of pushed away from wall, velocity of pushed UP from wall 
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);
            }

            wallJumpCooldown = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
    }

    private bool isGrounded() {
        // BoxCast(origin, size, angle, direction, virtualbox distance(how far you want the virtual box placed), layerMask (choose which layer you want it to collide with) )
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool onWall() {
        // BoxCast(origin, size, angle, direction, virtualbox distance(how far you want the virtual box placed), layerMask (choose which layer you want it to collide with) )
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    // Start is called before the first frame update
    void Start()
    {

    }
}
