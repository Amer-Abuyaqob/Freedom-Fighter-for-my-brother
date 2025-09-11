using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollectItem : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();
        if (item != null)
        {
            item.OnPickup(gameObject); // Pass the player as collector
            Destroy(collision.gameObject);
        }
    }
}
