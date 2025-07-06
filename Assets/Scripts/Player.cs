using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public Sprite[] sprites;

    private int spriteIndex;

    private Vector3 direction;

    public float gravity = -9.8f;

    public float strength = 5f;

    public AudioClip jumpSound;

    private AudioSource audioSource;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

    }

    private void Start()
    {
        InvokeRepeating(nameof(AnimateSprite), 0.15f, 0.15f);
    }


    private void OnEnable()
    {
        Vector3 position = transform.position;
        position.y = 0f;
        transform.position = position;
        direction = Vector3.zero;
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            direction = Vector3.up * strength;
            audioSource.PlayOneShot(jumpSound);
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            direction = Vector3.up * strength;
            audioSource.PlayOneShot(jumpSound);
        }
    }

    private void Update()
    {
        HandleInput();

        direction.y += gravity * Time.deltaTime;
        transform.position += direction * Time.deltaTime;
    }
    private void AnimateSprite()
    {
        spriteIndex++;
        if (spriteIndex >= sprites.Length)
        {
            spriteIndex = 0;
        }
        spriteRenderer.sprite = sprites[spriteIndex];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            FindObjectOfType<GameManager2>().GameOver();
        }
        else if (other.CompareTag("Scoring"))
        {
            FindObjectOfType<GameManager2>().IncreaseScore();
        }
        else if (other.CompareTag("Deadzone"))
        {
            FindObjectOfType<GameManager2>().GameOver();
        }
    }

}
