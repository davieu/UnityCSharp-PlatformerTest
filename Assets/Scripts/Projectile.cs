using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    private float direction;
    private bool hit;
    // will represent how many seconds the projectile has been active
    private float lifetime;

    // references
    private BoxCollider2D boxCollider;
    private Animator anim;

    private void Awake() {
        // get references to the components
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }


    // Update is called once per frame
    private void Update()
    {
        if (hit) return;
        float movementSpeed = speed * Time.deltaTime * direction;
        transform.Translate(movementSpeed, 0, 0);

        // keeps adding time as the projectile is fired.
        lifetime += Time.deltaTime;
        // the bigger the number the longer the fireball will stay out
        if (lifetime > 5) {
            // basically destroys the fireball. Sets it as inactive in the array of fireballs
            gameObject.SetActive(false);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision) {
        hit = true;
        boxCollider.enabled = false;
        if (direction == 1) {
            transform.localRotation = Quaternion.Euler(0, 0, 90);
        } else {
            transform.localRotation = Quaternion.Euler(0, 0, -90);
        }
        
        anim.SetTrigger("explode");
    }

    public void SetDirection(float _direction) {
        // resets the fireballs lifetime timer. 
        lifetime = 0;
        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;

        float localScaleX = transform.localScale.x;
        // If not facing the right direction then flip the fireball
        if (Mathf.Sign(localScaleX) != _direction) {
            // we simply flip the value
            localScaleX = -localScaleX;
        }

        // create new vector to adjust the direction of the fireball
        // transform.localScale.y,z simply means to keep same value that
        // the transform already has
        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }

    private void Deactivate() {
        // deactivates fireball once explosion has finished
        gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start() {

    }
}