using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    [SerializeField] private bool isDummie;
    [SerializeField] private float moveSpeed;
    [SerializeField] private int maxHealth;
    [SerializeField] private float attackChainResetTime;
    [SerializeField] private int attackDamage = 2;
    [SerializeField] private GameObject corpseGameObject;
    [SerializeField] private GameObject HpBarPrefab;

    private NetworkVariable<int> _health;
    public int Health
    {
        get => _health.Value;
        private set
        {
            print(_health.Value - value);
            _health.Value += value;
            print(_health.Value - value);
            
            _hpBar.fillAmount = _health.Value / maxHealth;
            if (_health.Value <= 0) StartDeathClientRpc();
        } 
    }
    
    private NetworkManager _networkManager;
    private Animator _animator;
    private BoxCollider2D _collider; 
    private BoxCollider2D _attackHitbox;
    private AttackHitbox _hitbox;
    private List<GameObject> _gameObjectsDamaged;
    private Image _hpBar;
    private HPBarFollowPlayer _hpBarBehaviour;
    private Vector2 _moveDirection;
    private bool _running;
    private bool _attack;
    private int _attackNum;
    private bool _attacking;
    private float _attackChainResetTimer;
    private int _attackChainStage;
    private bool _dead;
    private bool _spriteFlipped;
    
    private static readonly int Hurt = Animator.StringToHash("Hurt");
    private static readonly int Death = Animator.StringToHash("Death");
    private static readonly int Attack1 = Animator.StringToHash("Attack1");
    private static readonly int Attack2 = Animator.StringToHash("Attack2");
    private static readonly int Attack3 = Animator.StringToHash("Attack3");
    private static readonly int Running = Animator.StringToHash("Running");
    private static readonly int Respawn = Animator.StringToHash("Respawn");

    private void Awake()
    {
        _health = new NetworkVariable<int>();
    }

    private void Start()
    {
        _hpBarBehaviour = Instantiate(HpBarPrefab).GetComponent<HPBarFollowPlayer>();
        _hpBarBehaviour.SetPlayerTransform(transform);
        _hpBar = _hpBarBehaviour.HpBarImage;
        _networkManager = FindObjectOfType<NetworkManager>();
        _animator = GetComponentInChildren<Animator>();
        _attackHitbox = GetComponentsInChildren<BoxCollider2D>()[1];
        _hitbox = _attackHitbox.GetComponent<AttackHitbox>();
        _collider = GetComponentInChildren<BoxCollider2D>();
        _gameObjectsDamaged = new List<GameObject>();

        if (_networkManager.IsServer)
        {
            Health = maxHealth;
            RespawnPlayer();
        }
        
    }

    private void Update()
    {
        if (_dead || !IsOwner || isDummie) return;

        GetInput();
        Move();

        CountDownAttackChainTimer();
        if (_attack && !_attacking)
        {
            Attack();
            _attacking = true;
        }
        
        if (_attacking)
        {
            CheckAttackHits(_hitbox.GetCollidingGameObjects());
        }
    }

    private void GetInput()
    {
        _moveDirection.x = Input.GetAxis("Horizontal");
        _moveDirection.y = Input.GetAxis("Vertical");
        _moveDirection = _moveDirection.normalized;

        _attack = Input.GetAxis("Attack") > 0;
    }
    
    private void Move()
    {
        if (_moveDirection.magnitude < 0.5f || _attacking)
        {
            _animator.SetInteger(Running, 0);
            return;
        }
        
        switch (_moveDirection.x)
        {
            case > 0.5f when _spriteFlipped:
                FlipSprite();
                break;
            case < -0.5f when !_spriteFlipped:
                FlipSprite();
                break;
        }

        transform.position +=
            (Vector3)_moveDirection * (moveSpeed * Time.deltaTime);
        _animator.SetInteger(Running, 1);
    }
    
    private void FlipSprite()
    {
        transform.rotation = Quaternion.Euler(0, Mathf.Pow(180, Convert.ToSingle(!_spriteFlipped)), 0);

        _spriteFlipped = !_spriteFlipped;
    }
    
    private void Attack()
    {
        _gameObjectsDamaged = new List<GameObject>();
        _attackChainStage++;
        
        switch (_attackChainStage)
        {
            case 1:
                _animator.SetTrigger(Attack1);
                break;
            case 2:
                _animator.SetTrigger(Attack2);
                break;
            case 3:
                _animator.SetTrigger(Attack3);
                _attackChainStage = 0;
                break;
        }
    }
    
    private void CheckAttackHits(List<GameObject> gameObjectsToDamage)
    {
        foreach (var hitGameObject in gameObjectsToDamage)
        {
            if(_gameObjectsDamaged.Contains(hitGameObject)) continue;
            
            Player hitplayer = hitGameObject.GetComponent<Player>();
            if(hitplayer is null) continue;
            
            hitplayer.GotHit(attackDamage);
            _gameObjectsDamaged.Add(hitGameObject);
        }
    }
    
    public void AttackEnded()
    {
        _gameObjectsDamaged.Clear();
        _attackChainResetTimer = attackChainResetTime;
        _attacking = false;
    }

    private void CountDownAttackChainTimer()
    {
        if(_attackChainResetTimer <= 0) return;
        
        _attackChainResetTimer -= Time.deltaTime;
        if (_attackChainResetTimer <= 0)
        {
            _attackChainStage = 0;
        }
    }
    
    private void GotHit(int hitDamage)
    {
        DamageServerRpc(hitDamage);
    }

    [ServerRpc(RequireOwnership =  false)]
    private void DamageServerRpc(int amount)
    {
        Health = Health - amount;
        if (Health > 0) _animator.SetTrigger(Hurt);
    }

    [ClientRpc]
    private void StartDeathClientRpc()
    {
        _dead = true;
        _animator.SetTrigger(Death);
        _collider.enabled = false;
    }

    public void EndDeath()
    {
        Instantiate(corpseGameObject, transform.position, quaternion.identity);
        if (IsOwner) StartCoroutine(StartTimer(2, StartRespawnServerRpc));
    }

    [ServerRpc]
    private void StartRespawnServerRpc()
    {
        RespawnPlayer();
    }

    private void RespawnPlayer()
    {
        _animator.SetTrigger(Respawn);
        transform.position = Vector3.zero + (Vector3)(2 * UnityEngine.Random.insideUnitCircle);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Destroy(_hpBarBehaviour.gameObject);
    }
    
    private IEnumerator StartTimer(float duration, Action methodToCall)
    {
        float timer = duration;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            
            yield return null;
        }
        methodToCall();
    }
}
