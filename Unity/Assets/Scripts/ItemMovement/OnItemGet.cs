using UnityEngine;

public class OnItemGet : MonoBehaviour
{   
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            Destroy(gameObject);
        }
    }
}
