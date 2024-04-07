using System.Collections;
using System.Collections.Generic;
using General;
using Inputs;
using UnityEngine;

namespace Player
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance;
        
        [Header("Components")] 
        public Transform MeleeCentre;
        public List<GameObject> Ghosts = new();
        public PlayerCamera PlayerCameraScript;
        [HideInInspector] public Animator Animator;
        [HideInInspector] public CursorWorldRaycast CursorWorldRaycastScript;
        
        [Header("Spell Components")]
        public GameObject FireWall;
        
        [Header("Player Settings")]
        public Material DamagedMaterial;
        public float MeleeRadius;
        public float DamagedCooldown = 0.25f;
        public float IFrameDuration;
        public int MaxHealth = 200;
        public bool DebugActive;
        
        [Header("Arcane Weapon")]
        public int ArcaneWeaponDamage;
        public float ArcaneWeaponCooldown = 0.25f;

        [Header("Fire Wall")] 
        public int FireWallDamage;
        public float FireWallDamageCooldown;
        
        [HideInInspector] public float CurrentHealth;
        [HideInInspector] public MeshRenderer MeshRenderer;

        private Material _defaultMaterial;
        private IPlayerSpellState _currentState;
        private Vector3 _targetDirection;
        private Vector2 _previousMousePosition;
        private float _movementVelocity;
        private float _prevAngle;
        private float _rotateStartTime;
        private bool _playerIFrames;
        
        // Player states:
        private readonly ArcaneWeaponPlayerState _arcaneWeapon = new();
        private readonly FireWallPlayerState _fireWall = new();
        
        #region UnityFunctions

        private void Awake()
        {
            Instance = this;
            Animator = GetComponent<Animator>();
            CursorWorldRaycastScript = GetComponent<CursorWorldRaycast>();
            _currentState = _arcaneWeapon;
            CurrentHealth = MaxHealth;
            
            MeshRenderer = GetComponent<MeshRenderer>();
            _defaultMaterial = MeshRenderer.material;
            MeshRenderer.material = new Material(_defaultMaterial);
        }

        private void Update()
        {
            if (GameplayManager.Instance.Paused) return;
            _currentState.OnUpdate(this);
            CursorWorldRaycastScript.GetCursorDirection();
        }

        private void OnEnable()
        {
            InputManager.OnMouseDown += OnMouseDown;
        }

        private void OnDisable()
        {
            InputManager.OnMouseDown -= OnMouseDown;
        }

        #endregion

        #region PublicFunctions

        public void ChangeState(IPlayerSpellState state)
        {
            _currentState.OnEnd(this);
            _currentState = state;
            _currentState.OnStart(this);
        }

        public void ChangeSpell()
        {
            var gettingNewSpell = true;
            while (gettingNewSpell)
            {
                var random = Random.Range(0, 2);
                switch (random)
                {
                    case 0:
                    {
                        if(_currentState == _arcaneWeapon) continue;
                        ChangeState(_arcaneWeapon);
                        gettingNewSpell = false;
                    }
                        break;
                    case 1:
                    {
                        if(_currentState == _fireWall) continue;
                        ChangeState(_fireWall);
                        gettingNewSpell = false;
                    } break;
                }
            }
        }

        public void OnDamaged(int damage)
        {
            if(!_playerIFrames) _currentState.OnDamaged(this, damage);
        }

        public void OnDeath()
        {
            
        }
        
        public IEnumerator DamageShaderSwap(float duration)
        {
            var elapsedTime = duration;
            MeshRenderer.material = new Material(DamagedMaterial);
            while (elapsedTime > 0f)
            {
                elapsedTime -= Time.deltaTime;
                yield return null;
            }

            MeshRenderer.material = new Material(_defaultMaterial);
            yield return null;
        }

        public IEnumerator IFrames()
        {
            var elapsedTime = IFrameDuration;
            _playerIFrames = true;
            while (elapsedTime > 0f)
            {
                elapsedTime -= Time.deltaTime;
                yield return null;
            }

            _playerIFrames = false;
            yield return null;
        }
        

        #endregion

        #region PrivateFunctions

        private void OnMouseDown()
        {
            _currentState.OnAttack(this);
        }
        
        #endregion
    }
}
