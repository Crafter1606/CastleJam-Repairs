using UnityEngine;
using UnityEngine.Events;



public class OnItemGet : MonoBehaviour
{   [SerializeField] private GameObject player;
    
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            Destroy(gameObject);
        }
    }
}
