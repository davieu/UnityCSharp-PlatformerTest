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
    private bool isDashing = false;
    // how fast you want to dash
    public float dashSpeed;
    // how long you want to dash for
    public float dashTime;
    // cooldown for when dash ability is ready.
    private float dashCooldown;
    // resets the cooldown to specified time. higher numbers = longer CD
    public float resetDashCooldown;

    // wall jumping limiters
    public int wallJumpMax = 1;
    private int wallJumpCount = 0;

    private void Awake() {
        // grab references for rigidbody and animator from game object
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    private void Update() {
        if (!isDashing) {
            horizontalInput = Input.GetAxis("Horizontal");
        }

        // flip player when moving left or right
        if (horizontalInput > 0.01f) {
            transform.localScale = Vector3.one; // shorthand for (1,1,1)
        } else if (horizontalInput < -0.01f) {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // set animator parameters
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());

        // wall jump logic
        if (wallJumpCooldown > .2) {
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            if (onWall() && !isGrounded()) {
                // when player is on wall. he will get stuck and not fall down
                body.gravityScale = 2;
                body.velocity = Vector2.zero;
            } 
            else {
                body.gravityScale = 5;
            }

            if (Input.GetMouseButton(2)) {
                jump();
            }


        } else {
            wallJumpCooldown += Time.deltaTime;
        }

        if (isGrounded()) {
            //isDashing = false;
            wallJumpCount = 0;
        }


        // starts the dash cooldown timer
        dashCooldown -= Time.deltaTime;
        // stops the timer once the cooldown is ready
        if (dashCooldown < 0) {
            dashCooldown = -1;
        } else {
            dashCooldown -= Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Keypad0) && !onWall()) {
            print(dashCooldown);
            if (dashCooldown <= 0) {
                StartCoroutine(Dash());
                isDashing = false;
            }
            isDashing = false;
        }

        //print("grounded: " + isGrounded());
        //print("onwall: " + onWall());
        //print("isDashing: " + isDashing);
    }

    IEnumerator Dash() {
        anim.SetTrigger("dash");
        float startTime = Time.time;
        float localScaleX = transform.localScale.x;
        isDashing = false;
        while (Time.time < startTime + dashTime && !onWall()) {
            isDashing = true;
            float movementSpeed = dashSpeed * Time.deltaTime;

            if (Mathf.Sign(localScaleX) == 1) {
                transform.Translate(movementSpeed, 0, 0);
            } else {
                transform.Translate(-movementSpeed, 0, 0);
            }

            dashCooldown = resetDashCooldown;
            yield return null;
        }
        isDashing = false;
    }

    private void jump() {
        if (isGrounded() && !isDashing) {   
            body.velocity = new Vector2(body.velocity.x, jumpSpeed);
            anim.SetTrigger("jump");
        }
        else if (onWall() && !isGrounded()) {
            //wallJumpCount = 0;
            // for getting off wall
            // Vector2(direction, pushed horizontally speed, pushed up speed set to zero since it for no direction pressed)
            // Vector2(direction, pushed horizontally speed, pushed up speed set to zero since it for no direction pressed)
            if (horizontalInput == 0) {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
                // flip player in opposite direction once pushed off wall.
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (wallJumpCount < wallJumpMax) {
                // for going up walls
                // get new vector2. Find out which direction the player is facing and create a force opposite to it.
                // Mathf.Sign() when it gets a negative number it returns -1 and returns 1 for positive numbers
                // Vector2(direction player is facing * velocity of pushed away from wall, velocity of pushed UP from wall 
                wallJumpCount += 1;
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);
            }

            wallJumpCooldown = 0;
        }
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

    public bool canAttack() {
        //return horizontalInput == 0 && isGrounded() && !onWall();
        return isGrounded() && !onWall() && !isDashing;
    }

    // Start is called before the first frame update
    void Start()
    {

    }
}
