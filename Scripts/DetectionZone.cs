using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DetectionZone : MonoBehaviour
{
    public UnityEvent noCollidersRemain;
    public List<Collider2D> detectedColliders = new List<Collider2D>();

    private Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();

        // Ensure it's a trigger
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Add if not already detected
        if (!detectedColliders.Contains(collision))
        {
            detectedColliders.Add(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
        if (detectedColliders.Contains(collision))
        {
            detectedColliders.Remove(collision);
        }

    
        if (detectedColliders.Count == 0)
        {
            noCollidersRemain.Invoke();
        }
    }
}


