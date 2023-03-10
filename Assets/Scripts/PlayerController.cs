using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private Transform[] groundChecks;
    [SerializeField] private Transform[] wallChecks;
    [SerializeField] private AudioClip jumpSoundEffect;
    [SerializeField] private GameObject startScreen, deathScreen;
    [SerializeField] private TextMeshProUGUI coinText;
    public List<GameObject> platformPatterns;

    private float gravity = -50f;
    private CharacterController characterController;
    private Animator animator;
    private Vector3 velocity;
    private bool isGrounded;
    private float horizontalInput;
    private bool jumpPressed;
    private float jumpTimer;
    private float jumpGracePeriod = 0.2f;
    private int coins = 0;

    private void Awake()
    {
        Time.timeScale = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = 1;

        // Face Forward
        transform.forward = new Vector3(horizontalInput, 0, Mathf.Abs(horizontalInput) - 1);

        // IsGrounded
        isGrounded = false;
        
        foreach (var groundCheck in groundChecks)
        {
            if (Physics.CheckSphere(groundCheck.position, 0.1f, groundLayers, QueryTriggerInteraction.Ignore))
            {
                isGrounded = true;
                break;
            }
        }        

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0;
        }
        else
        {
            // Add gravity
            velocity.y += gravity * Time.deltaTime;
        }

        // Wallcheck - prevents player from getting stuck when running into a block and jumping
        var blocked = false;
        foreach (var wallCheck in wallChecks)
        {
            if (Physics.CheckSphere(wallCheck.position, 0.01f, groundLayers, QueryTriggerInteraction.Ignore))
            {
                blocked = true;
                break;
            }
        }

        if (!blocked)
        {
            foreach (GameObject platformPattern in platformPatterns)
            {
                platformPattern.transform.Translate(Vector3.left * runSpeed * Time.deltaTime);
            }
        }

        // Jumping
        jumpPressed = Input.GetButtonDown("Jump");

        if (jumpPressed)
        {
            jumpTimer = Time.time;
        }

        if (isGrounded && (jumpPressed || (jumpTimer > 0 && Time.time < jumpTimer + jumpGracePeriod)))
        {
            velocity.y += Mathf.Sqrt(jumpHeight * -2 * gravity);
            if (jumpSoundEffect != null)
            {
                AudioSource.PlayClipAtPoint(jumpSoundEffect, transform.position, 0.5f);
            }
            jumpTimer = -1;
        }

        // Vertical Velocity
        characterController.Move(velocity * Time.deltaTime);

        // Run Animation
        animator.SetFloat("Speed", horizontalInput);

        // Set Animator IsGrounded
        animator.SetBool("IsGrounded", isGrounded);

        // Set parameter for JumpFall Blend Tree Animation
        animator.SetFloat("VerticalSpeed", velocity.y);
    }

    public void Jump()
    {
        jumpTimer = Time.time;

        if (isGrounded && (jumpTimer > 0 && Time.time < jumpTimer + jumpGracePeriod))
        {
            velocity.y += Mathf.Sqrt(jumpHeight * -2 * gravity);
            if (jumpSoundEffect != null)
            {
                AudioSource.PlayClipAtPoint(jumpSoundEffect, transform.position, 0.5f);
            }
            jumpTimer = -1;
        }
    }

    public void StartGame()
    {
        if (startScreen.activeInHierarchy)
        {
            startScreen.SetActive(false);
            Time.timeScale = 1;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Death"))
        {
            StartCoroutine(EndGame());
        }

        if (other.CompareTag("Coin"))
        {
            coins++;
            Destroy(other.gameObject);

            coinText.text = coins + " Coins";
        }
    }

    IEnumerator EndGame()
    {
        runSpeed = 0;
        deathScreen.SetActive(true);

        yield return new WaitForSeconds(3f);

        Debug.Log("Reloading,..");
        SceneManager.LoadScene("MainScene");
    }
}
