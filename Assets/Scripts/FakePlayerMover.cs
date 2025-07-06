using UnityEngine;

public class FakePlayerMover : MonoBehaviour
{
    public float speed = 5f;
    public float gravity = -9.8f;
    public float strength = 5f;
    public float jumpInterval = 1.1f;

    public Sprite[] sprites;
    private SpriteRenderer spriteRenderer;
    private int spriteIndex;

    public AudioClip flySound;
    private AudioSource audioSource;

    private Vector3 direction;
    private float jumpTimer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        direction = Vector3.up * strength; // ? jump immediately on spawn
        jumpTimer = 0f;

        if (flySound != null)
        {
            audioSource.PlayOneShot(flySound);
        }

        InvokeRepeating(nameof(AnimateSprite), 0.15f, 0.15f);
    }

    private void Update()
    {
        // Horizontal movement
        transform.position += Vector3.right * speed * Time.deltaTime;

        // Gravity
        direction.y += gravity * Time.deltaTime;
        transform.position += direction * Time.deltaTime;

        // Auto jump every interval
        jumpTimer += Time.deltaTime;
        if (jumpTimer >= jumpInterval)
        {
            direction = Vector3.up * strength;
            jumpTimer = 0f;

            if (flySound != null)
                audioSource.PlayOneShot(flySound);
        }

        // Destroy when off screen
        if (transform.position.x > 10f)
        {
            Destroy(gameObject);
        }
    }

    private void AnimateSprite()
    {
        if (sprites == null || sprites.Length == 0) return;

        spriteIndex++;
        if (spriteIndex >= sprites.Length)
        {
            spriteIndex = 0;
        }

        spriteRenderer.sprite = sprites[spriteIndex];
    }
}
