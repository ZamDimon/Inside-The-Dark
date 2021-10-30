using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour {
    private const int MAX_ARRAY_SIZE = 1000;
    public const int START_ENEMY_NUMBER = 4;
    public Enemy[] enemies = new Enemy[MAX_ARRAY_SIZE];
    private static int enemyArrayLength = START_ENEMY_NUMBER;
    public int GetEnemyArrayLength() => enemyArrayLength; 
    public void IncreaseEnemyArray() => enemyArrayLength++;

    public void RemoveLastElement() {
        if (enemyArrayLength == 0) {
            Debug.LogError("You can't remove this element since the length of the array is already 0");
            return;
        }

        enemies[enemyArrayLength - 1] = null;
        enemyArrayLength--;
    }

    public Enemy GetEnemyByIndex(int index) {
        if (index < 0 || index >= GetEnemyArrayLength()) {
            Debug.Log("Enemy could not be found: index out of range");
            return null;
        }

        Enemy enemyToReturn = new Enemy();
        enemyToReturn.Equals(enemies[index]);
        enemyToReturn.health = enemyToReturn.maxHealth;
        
        return enemyToReturn;
    }

    public Enemy GetEnemyByType(EnemyType enemyType) {
        for (int i = 0; i < GetEnemyArrayLength(); ++i) {
            if (enemies[i].enemyType == enemyType)
                return enemies[i];
        }

        Debug.LogError("Enemy was not found");
        return null;
    }
}
