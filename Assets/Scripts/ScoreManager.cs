using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static int currentHP;
    public static int score;

    public int maxHP = 1000;
    public Text hpText; // Using UnityEngine.UI.Text instead of TextMeshPro
    public GameObject gameOverPanel; // Set this in Inspector (GameOverPanel under Hidden ResultMenuCanvas)

    private bool isDead = false;

    void Start()
    {
        currentHP = maxHP;
        UpdateHPUI();
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false); // Hide it at start
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;

        string tag = collision.gameObject.tag;

        switch (tag)
        {
            case "Enemy":
                ReduceHP(100);
                break;
            case "Tree":
                ReduceHP(50);
                break;
            case "Border":
                ReduceHP(20);
                break;
        }
    }

    public void scoreUpdate(int amount)
    {
        if (isDead) return;

        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        UpdateHPUI();

        if (currentHP == 0 && !isDead)
        {
            Die();
        }
    }

    void ReduceHP(int amount)
    {
        if (isDead) return;

        currentHP -= amount;
        currentHP = Mathf.Max(currentHP, 0);
        UpdateHPUI();

        if (currentHP == 0)
        {
            Die();
        }
    }

    void UpdateHPUI()
    {
        if (hpText != null)
            hpText.text = "" + currentHP;
    }

    void Die()
    {
        if (isDead) return; // prevent double-death
        Debug.LogWarning("Die !!");

        isDead = true;

        Time.timeScale = 0f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        else
            Debug.LogWarning("FailMenu UI is not assigned!");
    }
}
