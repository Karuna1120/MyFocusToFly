using UnityEngine;

public class FakeShadowPlayer : MonoBehaviour
{
    public Transform player;
    public float verticalOffset = 1.2f;

    public Sprite[] sprites;
    private SpriteRenderer spriteRenderer;
    private int spriteIndex;

    private Vector3 offset;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (player == null)
        {
            Player p = FindObjectOfType<Player>();
            if (p != null) player = p.transform;
        }

        // ????????????
        float direction = Random.value > 0.5f ? 1f : -1f;
        offset = new Vector3(0f, verticalOffset * direction, 0f);

        // ????
        InvokeRepeating(nameof(AnimateSprite), 0.15f, 0.15f);
    }

    private void Update()
    {
        if (player != null)
        {
            transform.position = player.position + offset;
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
