using UnityEngine;

/// <summary>
/// Autonomous “fake” bird that drifts right, flaps every jumpInterval
/// and destroys itself once it is beyond destroyX in world-space.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class FakePlayerMover : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;    // horizontal
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float strength = 5f;     // jump impulse
    [SerializeField] private float jumpInterval = 1.1f;
    [SerializeField] private float destroyX = 10f;    // off-screen

    [Header("Visuals")]
    public Sprite[] sprites;          // assigned per prefab
    [SerializeField] private float frameTime = 0.15f;

    [Header("Audio")]
    public AudioClip flySound;        // optional wing-flap sfx

    /* ??????????????????????????? runtime ????????????????????????????*/
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    private Vector3 direction;
    private float jumpTimer;
    private int spriteIndex;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        direction = Vector3.up * strength;   // small lift on spawn
        jumpTimer = 0f;

        PlayFlapSfx();
        InvokeRepeating(nameof(AnimateSprite), frameTime, frameTime);
    }

    private void Update()
    {
        /* horizontal drift */
        transform.position += Vector3.right * speed * Time.deltaTime;

        /* gravity & auto-flap */
        direction.y += gravity * Time.deltaTime;
        transform.position += direction * Time.deltaTime;

        jumpTimer += Time.deltaTime;
        if (jumpTimer >= jumpInterval)
        {
            direction = Vector3.up * strength;
            jumpTimer = 0f;
            PlayFlapSfx();
        }

        /* clean-up */
        if (transform.position.x > destroyX)
            Destroy(gameObject);
    }

    private void AnimateSprite()
    {
        if (sprites == null || sprites.Length == 0) return;

        spriteIndex = (spriteIndex + 1) % sprites.Length;
        spriteRenderer.sprite = sprites[spriteIndex];
    }

    private void PlayFlapSfx()
    {
        if (flySound) audioSource.PlayOneShot(flySound);
    }
}
