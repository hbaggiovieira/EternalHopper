using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D playerCollider;

    public LayerMask platformLayer; // Defina isso no inspector para corresponder à layer das plataformas

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        // Raycast para cima para detectar plataformas
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 1f, platformLayer);

        if (hit.collider != null)
        {
            // Se estiver indo para cima e abaixo da plataforma, ignore a colisão
            if (rb.velocity.y > 0 && transform.position.y < hit.collider.bounds.min.y)
            {
                Physics2D.IgnoreCollision(hit.collider, playerCollider, true);
            }
            else
            {
                // Reativa a colisão quando não estiver mais abaixo da plataforma
                Physics2D.IgnoreCollision(hit.collider, playerCollider, false);
            }
        }
    }
}
