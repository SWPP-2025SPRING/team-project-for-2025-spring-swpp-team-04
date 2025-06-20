using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiText : MonoBehaviour
{
    public Text scoreText;


    void Start()
    {
        //this.gameObject.GetComponent<ScoreManager>
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = string.Format($"{ScoreManager.score}");
    }
}
