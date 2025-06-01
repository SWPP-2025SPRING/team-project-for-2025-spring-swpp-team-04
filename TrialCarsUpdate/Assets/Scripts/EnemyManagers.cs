using UnityEngine;
using System.Collections.Generic;

public class EnemyManagers : MonoBehaviour
{
    private List<EnemyData> enemies = new List<EnemyData>();

    [System.Serializable]
    private class EnemyData
    {
        public GameObject enemyObject;
        public Vector3 startPosition;
        public Quaternion startRotation;

        public EnemyData(GameObject obj)
        {
            enemyObject = obj;
            startPosition = obj.transform.position;
            startRotation = obj.transform.rotation;
        }

        public void ResetEnemy()
        {
            if (enemyObject != null)
            {
                enemyObject.transform.position = startPosition;
                enemyObject.transform.rotation = startRotation;

                // Reactivate enemy if it was disabled/destroyed
                enemyObject.SetActive(true);

                // Optionally reset health, AI state, etc.
                CharacterController controller = enemyObject.GetComponent<CharacterController>();
                if (controller != null)
                {
                    controller.enabled = false; // Disable briefly to allow position reset
                    controller.transform.position = startPosition;
                    controller.enabled = true;
                }
            }
        }
    }

    void Start()
    {
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in allEnemies)
        {
            enemies.Add(new EnemyData(enemy));
        }
    }

    public void ResetAllEnemies()
    {
        foreach (var enemy in enemies)
        {
            enemy.ResetEnemy();
        }
    }
}
