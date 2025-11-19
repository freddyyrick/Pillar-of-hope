    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
using UnityEngine.UI;
    using TMPro;

    public class Healthbar : MonoBehaviour
    {
        // Start is called before the first frame update
        public Slider healthSlider;
        public TMP_Text healthBarText;

        Damageable playerDamageable;

        private void Awake()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if(player == null)
            {
                Debug.LogError("Player object not found in the scene. Make sure the player has the 'Player' tag assigned.");
            }

            playerDamageable = player.GetComponent<Damageable>();
        }
        void Start()
        {

            healthSlider.value = CalculateSliderPercentage(playerDamageable.Health, playerDamageable.MaxHealth);
            healthBarText.text = "HP" + playerDamageable.Health + " / " + playerDamageable.MaxHealth;
        }


        private void OnEnable()
        {
            playerDamageable.healthChanged.AddListener(OnPlayerHealthChanged);
        }

        private void OnDisable()
        {
            playerDamageable.healthChanged.RemoveListener(OnPlayerHealthChanged);
        }
        private float CalculateSliderPercentage(float currentHealth, float maxHealth)
        {
            return currentHealth / maxHealth;
        }   
        
        private void OnPlayerHealthChanged(int newHealth, int maxHealth)
        {
            healthSlider.value = CalculateSliderPercentage(newHealth, maxHealth);
            healthBarText.text = "HP" + playerDamageable.Health + " / " + playerDamageable.MaxHealth;
        }
    }
