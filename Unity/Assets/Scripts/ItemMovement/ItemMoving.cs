using UnityEngine;

public class ItemMoving : MonoBehaviour
{
    // private bool _collided;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // --- Wall Detection ---
        // FÜr einzelne Objekte gut
        // collision.attachedRigidbody.gameObject.tag != "Player";
        // if (collision != null) _collided = true;
        // else _collided = false;

        if (collision.gameObject.TryGetComponent(out Health health) && collision.CompareTag("Player")) health.Heal(1);
    }

    public void Die()
    {
        Destroy(gameObject);
      

        // if (collision.gameObject.TryGetComponent<Health>(out Health health))  { health.Heal(1); }
    }
}
