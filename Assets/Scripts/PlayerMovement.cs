using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float jumpForce = 300f;
    [SerializeField] private Transform leftFoot, rightFoot;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private AudioClip jumpSound, pickUpSound;
    [SerializeField] private GameObject AppleParticles;

    [SerializeField] private TMP_Text appleText;
    [SerializeField] private Slider healthSlider;
    private float horizontalValue;
    private float rayDistance = 0.25f;
    private bool isGrounded;
    private bool canMove;
    private int startingHealth = 5;
    private int currentHealth = 0;
    public int applesCollected = 0;

    private Rigidbody2D rgbd;
    private SpriteRenderer rend;
    private AudioSource audioSource;
    private Animator anim;
    
    void Start()
    {
        canMove = true;
        currentHealth = startingHealth;
        appleText.text = "" + applesCollected;
        rgbd = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        horizontalValue = Input.GetAxis("Horizontal");

        if (horizontalValue < 0)
        {
            FlipSprite(true);
        }

        if(horizontalValue > 0)
        {
            FlipSprite(false);
        }

        if (Input.GetButtonDown("Jump") && CheckIfGrounded() == true)
        {
            Jump();
        }
        
        anim.SetFloat("MoveSpeed", Mathf.Abs(rgbd.velocity.x));
        anim.SetFloat("VetricalSpeed", rgbd.velocity.y);
        anim.SetBool("isGrounded", CheckIfGrounded());

    }

    private void FixedUpdate()
    {
        if (!canMove)
        {
            return;
        }
        rgbd.velocity = new Vector2(horizontalValue * moveSpeed * Time.deltaTime, rgbd.velocity.y);

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Apple"))
        {
            Destroy(other.gameObject);
            applesCollected++;
            appleText.text = "" + applesCollected;
            audioSource.pitch = Random.Range(0.9f, 2.0f);
            audioSource.PlayOneShot(pickUpSound, 0.5f);
            Instantiate(AppleParticles, other.transform.position, Quaternion.identity);
        }
        if (other.CompareTag("Health"))
        {
            RestorHealth(other.gameObject);
        }
    }

    private void FlipSprite(bool direction)
    {
        rend.flipX = direction;
    }
    private void Jump()
    {
        rgbd.AddForce(new Vector2(0, jumpForce));
        audioSource.PlayOneShot(jumpSound, 0.3f);
    }
    public void TakeDamage(int dmgAmount)
    {
        currentHealth -= dmgAmount;
        healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            Respawn();
        }
    }
    public void TakeKnockback(float knockbackForce, float upwards)
    {
        canMove = false;
        rgbd.AddForce(new Vector2(knockbackForce, upwards));
        Invoke("canMoveAgain", 0.25f);
    }
    public void canMoveAgain()
    {
        canMove = true;
    }
    private void Respawn()
    {
        currentHealth = startingHealth;
        healthSlider.value = currentHealth;
        transform.position = spawnPosition.position;
        rgbd.velocity = Vector2.zero;

    }
    private void RestorHealth(GameObject healthPickup)
    {
        if (currentHealth >= startingHealth)
        {
            return;
        }
        else
        {
            int healthToRestor = healthPickup.GetComponent<HealthPickup>().healthAmount;
            currentHealth = healthToRestor;
            UpdateHealthBar();
            Destroy(healthPickup);
            if (currentHealth >= startingHealth)
            {
                currentHealth = startingHealth;
            }
        }
    }
    private void UpdateHealthBar()
    {
        healthSlider.value = currentHealth;
    }
    private bool CheckIfGrounded()
    {
        RaycastHit2D leftHit = Physics2D.Raycast(leftFoot.position, Vector2.down, rayDistance, whatIsGround);
        RaycastHit2D rightHit = Physics2D.Raycast(rightFoot.position, Vector2.down, rayDistance, whatIsGround);

        if (leftHit.collider != null && leftHit.collider.CompareTag("Ground") || rightHit.collider != null && rightHit.collider.CompareTag("Ground"))
        {
            return true;
        }
        else
            return false;
    }
}
