using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerTest : NetworkBehaviour
{
    [SerializeField] private float moveSpeed;
    private Vector2 _moveDirection;
    
    // Start is called before the first frame update
    void Start()
    {
        _moveDirection = new Vector2();
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) return;    
        GetInput();
        Move();
    }
    
    private void GetInput()
    {
        _moveDirection.x = Input.GetAxis("Horizontal");
        _moveDirection.y = Input.GetAxis("Vertical");
        _moveDirection = _moveDirection.normalized;
        
    }
    
    private void Move()
    {
        transform.position +=
            (Vector3)_moveDirection * (moveSpeed * Time.deltaTime);
    }
}
