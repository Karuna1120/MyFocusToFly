using UnityEngine;

public class Towers : MonoBehaviour
{
    public static float globalSpeed = 5f;
    private static float speedIncrement = 0.2f;
    private static float maxSpeed = 10f;
    private float elapsedTime = 0f;
    private float leftEdge;

    private void Start()
    {
        leftEdge = Camera.main.ScreenToWorldPoint(Vector3.zero).x - 1f;
    }

    private void Update()
    {
        transform.position += Vector3.left * globalSpeed * Time.deltaTime;

        // ? 10 ?????
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= 10f && globalSpeed < maxSpeed)
        {
            globalSpeed += speedIncrement;
            elapsedTime = 0f;
        }

        if (transform.position.x < leftEdge)
        {
            Destroy(gameObject);
        }
    }
}
