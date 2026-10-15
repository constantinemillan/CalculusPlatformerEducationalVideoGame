using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    //Floats
    public float maxSpeed = 3;
    public float speed = 50f;
    public float jumpPower = 150f;

    //Booleans
    public bool grounded;
    public bool canDoubleJump;

    //Stats
    public int curHealth;
    public int maxHealth = 100;

    //References
    private Rigidbody2D rb2d;
    private Animator anim;
    private gameMaster gm;



    void Start()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();

        curHealth = maxHealth;
        gm = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<gameMaster>();
    }


    void Update()
    {


        anim.SetBool("Grounded", grounded);
        anim.SetFloat("Speed", Mathf.Abs(rb2d.velocity.x));

       
        if (Input.GetButtonDown("Jump"))
        {
            if (grounded)
            {
                rb2d.AddForce(Vector2.up * jumpPower);
                canDoubleJump = true;
            }
            else
            {

                if (canDoubleJump)
                {

                    canDoubleJump = false;
                    rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
                    rb2d.AddForce(Vector2.up * jumpPower);

                }

            }
        }

        if (curHealth > maxHealth) {
            curHealth = maxHealth;
        }

        if (curHealth <= 0) {

            curHealth = 0;

            Die();

        }



    }

    void FixedUpdate()
    {
        Vector3 easeVelocity = rb2d.velocity;
        easeVelocity.y = rb2d.velocity.y;
        easeVelocity.z = 0.0f;
        easeVelocity.x *= 0.75f;

        float h = Input.GetAxis("Horizontal");

        //Fake friction / Easing the x speed of our player
        if (grounded)
        {

            rb2d.velocity = easeVelocity;

        }


        //Moving the player
        rb2d.AddForce((Vector2.right * speed) * h);

        //Limiting the speed of the player
        if (rb2d.velocity.x > maxSpeed)
        {
            rb2d.velocity = new Vector2(maxSpeed, rb2d.velocity.y);
        }

        if (rb2d.velocity.x < -maxSpeed)
        {
            rb2d.velocity = new Vector2(-maxSpeed, rb2d.velocity.y);
        }

    }

    void Die() {

        Application.LoadLevel(Application.loadedLevel);

    }

    public void Damage(int dmg){

        curHealth -= dmg;
        gameObject.GetComponent<Animation>().Play("Player_RedFlash");


    }


    public IEnumerator Knockback(float knockDur, float knockbackPwr, Vector3 knockbackDir){

        float timer = 0;
        rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
        while( knockDur > timer){

            timer+=Time.deltaTime;

            rb2d.AddForce(new Vector3(knockbackDir.x * -100, knockbackDir.y * knockbackPwr, transform.position.z));

        }

        yield return 0;

    }

    void OnTriggerEnter2D(Collider2D col)
    {

        if (col.CompareTag("Coin"))
        {
            Destroy(col.gameObject);
            gm.points += 1;
        
        }
    }


}
