using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameController : MonoBehaviour
{
    // Start is called before the first frame update
    // serves to initialize and store some important values
    public static int _playerScore;
    public static int _playerCombo;
    private Text _pCombo;
    private Text _pScore;
    void Start()
    {
       _playerScore = 0;
       _playerCombo = 1; 
       _pCombo = GameObject.Find("scoreText").GetComponent<Text>();
       _pScore = GameObject.Find("comboText").GetComponent<Text>();
    }

    // Update is called once per frame
    private void Update() {
        _pCombo.text = _playerCombo + "x";
        _pScore.text = _playerScore + "";
    }
}
