using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] AudioClip sound;

    public virtual void OnPickup(GameObject collector)
    {
        PlaySound();
    }
    
    protected void PlaySound()
    {
        if (sound != null)
        {
            AudioSource.PlayClipAtPoint(sound, transform.position);
        }
    }
}
