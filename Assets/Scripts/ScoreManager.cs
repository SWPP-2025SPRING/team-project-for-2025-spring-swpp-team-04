using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// public class ScoreManager : MonoBehaviour
// {
//     public int maxHP = 1000;
//     public int currentHP;
//     public TextMeshProUGUI hpText;

//     public GameObject pauseMenuUI;           // reference to PauseMenu UI (set in Inspector)
//     public TextMeshProUGUI failMessageText; 
//     private bool isDead = false;


//     public static int score;

//     private void Start()
//     {
//         currentHP = maxHP;
//         UpdateHPUI();
//     }

//     public void scoreUpdate(int InputScore)
//     {
//         if (currentHP == 0)
//         {
//             if (InputScore > 0)
//             {
//                 currentHP += InputScore;
//                 UpdateHPUI();
//             }
//         }
//         else
//         {
//             currentHP += InputScore;
//             UpdateHPUI();
//         }
//     }

//     private void OnCollisionEnter(Collision collision)
//     {
//         string tag = collision.gameObject.tag;

//         if (tag == "Enemy")
//             ReduceHP(100);
//         else if (tag == "Tree")
//             ReduceHP(50);
//         else if (tag == "Border")
//             ReduceHP(20);
//     }

//     void ReduceHP(int amount)
//     {
//         currentHP -= amount;
//         currentHP = Mathf.Max(currentHP, 0);
//         UpdateHPUI();

//         if (currentHP == 0)
//         {
//             Debug.Log("Game Over!");
//             // You can trigger explosion, death animation, or stop the car here.
//         }
//     }

//     void UpdateHPUI()
//     {
//         if (hpText != null)
//             hpText.text = "HP: " + currentHP;
//     }

//     void Die()
//     {
//         isDead = true;

//         // Stop game time
//         Time.timeScale = 0f;

//         // Show pause menu
//         if (pauseMenuUI != null)
//             pauseMenuUI.SetActive(true);

//         // Show failure message
//         if (failMessageText != null)
//         {
//             failMessageText.text = "You Fail!";
//             failMessageText.gameObject.SetActive(true);
//         }

//         // Optional: trigger death animation, disable movement, etc.
//     }
// }

using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    // public int maxHP = 1000;
    public static int currentHP;
    public TextMeshProUGUI hpText;
    public static int score;

    public GameObject failMenuUI; // Reference to the 'FailMenu' in Canvas

    private bool isDead = false;

    void Awake()
    {
        Debug.Log("PlayerHP instance: " + gameObject.name);
    }

    void Start()
    {
        currentHP = 1000;
        UpdateHPUI();
    }
    // public void AddHP(int amount)
    // {
    //     if (isDead) return;

    //     currentHP += amount;
    //     currentHP = Mathf.Min(currentHP, maxHP); // cap to max
    //     UpdateHPUI();
    // }
    // public void scoreUpdate(int InputScore)
    // {
    //     if (currentHP == 0)
    //     {
    //         if (InputScore > 0)
    //         {
    //             currentHP += InputScore;
    //             UpdateHPUI();
    //         }
    //     }
    //     else
    //     {
    //         currentHP += InputScore;
    //         UpdateHPUI();
    //     }
    // }
    public void scoreUpdate(int amount)
    {
        Debug.LogWarning("Inside Score Update amount: " + amount);
        if (isDead) return;

        Debug.LogWarning("Score Update: Before HP: " + currentHP);
        currentHP += amount;
        currentHP = Mathf.Min(currentHP, 1000);
        Debug.LogWarning("Score Update: After HP: " + currentHP);

        UpdateHPUI();
        Debug.LogWarning("Score Update: Update UI: " + currentHP);
    }

    // private void OnCollisionEnter(Collision collision)
    // {
    //     if (isDead) return;

    //     string tag = collision.gameObject.tag;

    //     if (tag == "Enemy")
    //         ReduceHP(100);
    //     else if (tag == "Tree")
    //         ReduceHP(50);
    //     else if (tag == "Border")
    //         ReduceHP(20);
    // }
    private void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;

        GameObject other = collision.gameObject;

        // // ✅ Ignore collision with inactive or dead enemies
        // if (other.CompareTag("Enemy") && !other.activeInHierarchy)
        //     return;

        string tag = other.tag;

        if (tag == "Enemy") {
            // if (!other.activeInHierarchy) 
            //     return;
            Debug.LogWarning("Enemy Collision with player!" );
            ReduceHP(100);
        }    
        else if (tag == "Tree")
            ReduceHP(50);
        else if (tag == "Border")
            ReduceHP(20);
    }

    // void ReduceHP(int amount)
    // {
    //     currentHP -= amount;
    //     currentHP = Mathf.Max(currentHP, 0);
    //     UpdateHPUI();

    //     if (currentHP == 0 && !isDead)
    //     {
    //         Die();
    //     }
    // }
    void ReduceHP(int amount)
    {   
        Debug.LogWarning("Inside Reduce HP, amount: " + amount);
        if (isDead) return;

        Debug.LogWarning("Reduce HP: Before HP : " + currentHP);    
        currentHP -= amount;
        currentHP = Mathf.Max(currentHP, 0);
        Debug.LogWarning("Reduce HP: After HP: " + currentHP);

        UpdateHPUI();
        Debug.LogWarning("Reduce HP: Update UI: " + currentHP);

        Debug.LogWarning("Reduce HP: Check if HP is 0");
        if (currentHP == 0)
        {
            Debug.LogWarning("Reduce HP: Die");
            Die();
        }
    }

    // void Die()
    // {
    //     isDead = true;

    //     // ✅ Stop all game time including timers
    //     Time.timeScale = 0f;

    //     // ✅ Show the fail menu
    //     if (failMenuUI != null)
    //         failMenuUI.SetActive(true);
    //     else
    //         Debug.LogError("FailMenu UI is not assigned!");
    // }
    void Die()
    {
        if (isDead) return; // prevent double-death
        Debug.LogWarning("Die !!");

        isDead = true;

        Time.timeScale = 0f;

        if (failMenuUI != null)
            failMenuUI.SetActive(true);
        else
            Debug.LogWarning("FailMenu UI is not assigned!");
    }

    void UpdateHPUI()
    {
        if (hpText != null)
            hpText.text = "HP: " + currentHP;
    }

}
