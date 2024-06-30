using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    private List<GameObject> collidingGameObjects;

    private void Start()
    {
        collidingGameObjects = new List<GameObject>();
    }

    private void Update()
    {
        collidingGameObjects.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<Player>() is null || other.gameObject == gameObject) return;
        
        collidingGameObjects.Add(other.gameObject);
    }

    public List<GameObject> GetCollidingGameObjects() => collidingGameObjects;
}
