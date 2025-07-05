using UnityEngine;

public class AttactPlane : MonoBehaviour
{
    [Header("Flight Settings")]
    public float speed = 7f;

    [Header("Sprite Settings")]
    public Sprite spriteFacingRight;

    [Header("Audio")]
    public AudioClip explodeSound;

    private Vector3 target;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        // ? ?????????????
        Vector3 screenSize = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
        Vector3 spawnPos = transform.position;

        float targetX = screenSize.x + 1f;
        float targetY = spawnPos.y < 0 ? screenSize.y + 1f : -screenSize.y - 1f;
        target = new Vector3(targetX, targetY, 0);

        // ? ??????
        spriteRenderer.sprite = spriteFacingRight;

        // ? ????????
        Vector3 moveDir = (target - spawnPos).normalized;
        float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (explodeSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(explodeSound);
            }

            FindObjectOfType<GameManager>().GameOver();
            Destroy(gameObject);
        }
    }
}
