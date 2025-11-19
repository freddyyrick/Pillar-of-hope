    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Unity.VisualScripting;
    using UnityEngine;
    using UnityEngine.Events;


    public class Damageable : MonoBehaviour

    {
        public UnityEvent<int, Vector2> damageableHit;
        public UnityEvent damageableDeath;
        public UnityEvent<int, int> healthChanged;
        Animator animator;

        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private int _health;
        [SerializeField] private bool _isAlive = true;

        [SerializeField] private float invincibilityTimer = 0.25f;
        [SerializeField] private bool isInvincible = false;
        private float timeSinceHit = 0f;

        public int MaxHealth
        {
            get { return _maxHealth; }
            set { _maxHealth = value; }
        }

        public int Health
        {
            get => _health;
            private set
            {
                _health = value;
                healthChanged?.Invoke(_health, _maxHealth);

                if (_health <= 0 && IsAlive)
                {
                    IsAlive = false; // trigger death
                }
            }
        }


        public bool IsAlive
        {
            get => _isAlive;
            private set
    {
                // Only proceed if the state is actually changing
                if (_isAlive != value)
                {
                    _isAlive = value;
                    
                    if (animator != null)
                        animator.SetBool("isAlive", value);
                
                    // If the state changed to NOT ALIVE, invoke the death event
                    if (!value) // value is false (boss died)
                    {
                        damageableDeath?.Invoke(); // ⬅️ THIS IS THE MISSING CALL!
                    }
                }
            }
        }

        public bool LockVelocity
        {
            get
            {
                return animator.GetBool(AnimationStrings.lockVelocity);
            }

            set
            {
                animator.SetBool(AnimationStrings.lockVelocity, value);
            }
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();
            Health = MaxHealth;
            IsAlive = true;
        }

        private void Update()
        {
            // Handle invincibility timer
            if (isInvincible)
            {
                timeSinceHit += Time.deltaTime;

                if (timeSinceHit >= invincibilityTimer)
                {
                    isInvincible = false;
                    timeSinceHit = 0f;
                }
            }
        }

        public bool Hit(int damage, Vector2 knockback)
        {
            if (IsAlive && !isInvincible)
            {
                Health -= damage;
                isInvincible = true;

                animator.SetTrigger(AnimationStrings.hitTrigger);
                LockVelocity = true;

                damageableHit?.Invoke(damage, knockback);
                CharacterEvents.OnCharacterDamaged(gameObject, damage); 
                return true;
            }
            return false;
        }


        public void Heal(int healthRestore)
        {
            if (IsAlive)
            {
                int maxHeal = Mathf.Max(MaxHealth - Health, 0);
                int actualHeal = Mathf.Min(healthRestore, maxHeal);

                if (actualHeal > 0)
                {
                    Health += actualHeal;
                    CharacterEvents.OnCharacterHealed(gameObject, actualHeal); // ✅ fixed call
                }
            }
        }
    }
