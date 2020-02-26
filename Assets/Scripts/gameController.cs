using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameController : MonoBehaviour
{
    // Start is called before the first frame update
    // serves to initialize and store some important values
    public const float TOTAL_PLAYER_HEALTH = 100;
    
    public static int _playerScore;
    public static int _playerCombo;
    public static float _playerHealth;
    public static string _gameState; // switch this to indicate different states, e.g. menu, gameplay, death, etc.
    public float _playerHealthDecreaseRate; // decrease rate should change depending on BPM of song (faster BPM = faster rate)
    public static float _playerHealthRecoveryRate = 10;
    public static float _damageRate = 3; // potentially change this according to difficulty?

    private float lastTime;
    private Text _pCombo;
    private Text _pScore;
    void Start()
    {
        _gameState = null;
       _playerScore = 0;
       _playerCombo = 1; 
       _pCombo = GameObject.Find("scoreText").GetComponent<Text>();
       _pScore = GameObject.Find("comboText").GetComponent<Text>();
       _playerHealth = 100f;
       _playerHealthDecreaseRate = 0.4f;
       lastTime = Time.time;
       
       // do not render the pause canvas on launch
       GameObject.Find("PauseCanvas").SetActive(false);
    }

    // Update is called once per frame
    private void Update() {
        // score handler
        _pCombo.text = _playerCombo + "x";
        _pScore.text = _playerScore + "";

        // health/healthbar handler
        //Debug.Log(_playerHealth);
        healthBarHandler();
        GameObject.Find("HealthBar").GetComponent<Slider>().value = _playerHealth / TOTAL_PLAYER_HEALTH; 
    }

    private void healthBarHandler(){
        // decreases playerhealth at a fixed rate
        if(_playerHealth <=0){
            _gameState = "game_over";
            Debug.Log(_gameState); 
            return;
        }
        if(Time.time - lastTime >= 0.1){
            _playerHealth -= _playerHealthDecreaseRate;
            lastTime = Time.time;
        }
    }
}
