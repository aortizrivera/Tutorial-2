using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerScript : MonoBehaviour
{
    private Rigidbody2D rd2d;
    public float speed;

    public TextMeshProUGUI score;
    public TextMeshProUGUI lives;

    public AudioClip backgroundMusic;
    public AudioClip winMusic;
    public AudioClip loseMusic;
    public AudioSource musicSource;

    public GameObject winText;
    public GameObject loseText;

    private int scoreValue = 0;
    private int livesValue = 3;

    private bool gameOver = false;

    private bool facingRight = true;

    private bool isOnGround;
    public Transform groundcheck;
    public float checkRadius;
    public LayerMask allGround;

    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rd2d = GetComponent<Rigidbody2D>();
        score.text = scoreValue.ToString();
        lives.text = livesValue.ToString();

        winText.SetActive(false);
        loseText.SetActive(false);

        musicSource.clip = backgroundMusic;
        musicSource.Play();

        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float hozMovement = Input.GetAxis("Horizontal");
        float vertMovement = Input.GetAxis("Vertical");

        rd2d.AddForce(new Vector2(hozMovement * speed, vertMovement * speed));

        if (facingRight == false && hozMovement > 0)
        {
            Flip();
        }
        else if (facingRight == true && hozMovement < 0)
        {
            Flip();
        }

        isOnGround = Physics2D.OverlapCircle(groundcheck.position, checkRadius, allGround);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
        {
            anim.SetInteger("State", 1);
        }
        else
        {
            anim.SetInteger("State", 0);
        }
        
        if (Input.GetKey(KeyCode.W))
        {
            anim.SetInteger("State", 2);

            if (isOnGround == false)
            {

            }
            else
            {
                anim.SetInteger("State", 0);
            }
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector2 Scaler = transform.localScale;
        Scaler.x = Scaler.x * -1;
        transform.localScale = Scaler;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Coin")
        {
            scoreValue += 1;
            score.text = scoreValue.ToString();
            Destroy(collision.collider.gameObject);
        }

        if (scoreValue == 4)
        {
            Collider2D collider = collision.collider;

            if(collision.collider.tag == "Coin")
            {
                Vector2 contactPoint = collision.contacts[0].point;
                transform.position = new Vector2(85.0f, 0.0f);
            }

            livesValue = 3;
            lives.text = livesValue.ToString();
        }

        if (scoreValue >= 8)
        {
            winText.SetActive(true);

            if (!gameOver)
            {
                musicSource.clip = backgroundMusic;
                musicSource.Stop();
                musicSource.loop = false;
                musicSource.PlayOneShot(winMusic);
                gameOver = true;
            }
        }

        if (collision.collider.tag == "Enemy")
        {
            livesValue -= 1;
            lives.text = livesValue.ToString();
            Destroy(collision.collider.gameObject);
        }

        if (livesValue <= 0)
        {
            loseText.SetActive(true);
            AudioSource.PlayClipAtPoint(loseMusic, transform.position);
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground" && isOnGround)
        {
            if (Input.GetKey(KeyCode.W))
            {
                rd2d.AddForce(new Vector2(0, 3), ForceMode2D.Impulse);
                //the 3 in this line of code is the player's "jumpforce," and you change that number to get different jump behaviors.  You can also create a public variable for it and then edit it in the inspector.
            }
        }
    }
}
