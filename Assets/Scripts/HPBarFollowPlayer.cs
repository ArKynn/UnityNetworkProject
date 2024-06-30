using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HPBarFollowPlayer : MonoBehaviour
{
    [SerializeField] private Image hpBarImage;
    
    private Transform playerTransform;
    private float _yFollowDisplacement;
    private Vector3 _playerPosition;
    
    public Image HpBarImage { get => hpBarImage; private set => hpBarImage = value; }

    private void Start()
    {
        _yFollowDisplacement = transform.localPosition.y;
    }

    private void Update()
    {
        _playerPosition = playerTransform.position;
        
        transform.position = new Vector3(_playerPosition.x,
            _playerPosition.y + _yFollowDisplacement,
            _playerPosition.z);
    }

    public void SetPlayerTransform(Transform newTrasform)
    {
        if(playerTransform is not null) return;

        playerTransform = newTrasform;
    }
}
