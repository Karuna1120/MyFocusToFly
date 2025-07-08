using UnityEngine;

/// <summary>
/// Simple follower that mirrors the main Player up/down at a fixed vertical
/// offset, flipping the offset randomly on spawn so it can appear above
/// or below the real bird.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class FakeShadowPlayer : MonoBehaviour
{
    [SerializeField] private float verticalOffset = 1.2f;   // base distance
    [SerializeField] private float frameTime = 0.15f;  // flap speed

    public Transform player;                     // filled at runtime
    public Sprite[] sprites;                    // shared bird sprites

    /* ??????????????????????????? runtime ????????????????????????????*/
    private SpriteRenderer spriteRenderer;
    private int spriteIndex;
    private Vector3 offset;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        /* fall back to first Player found if none injected */
        if (player == null)
        {
            Player p = FindObjectOfType<Player>();
            if (p) player = p.transform;
        }

        /* randomly choose above or below */
        float dir = Random.value > 0.5f ? 1f : -1f;
        offset = new Vector3(0f, verticalOffset * dir, 0f);

        InvokeRepeating(nameof(AnimateSprite), frameTime, frameTime);
    }

    private void Update()
    {
        if (player)
            transform.position = player.position + offset;
    }

    private void AnimateSprite()
    {
        if (sprites == null || sprites.Length == 0) return;

        spriteIndex = (spriteIndex + 1) % sprites.Length;
        spriteRenderer.sprite = sprites[spriteIndex];
    }
}
