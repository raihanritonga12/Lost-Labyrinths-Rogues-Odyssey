using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpertLevelManager : MonoBehaviour
{
    private Dictionary<int, float> tierValuePairs = new Dictionary<int, float>();
    public LevelGenerator levelGenerator;
    public LevelGraphGen levelGraphGen;
    private Player player;
    // Start is called before the first frame update
    void Start()
    {
        // Set default values for the dictionary
        tierValuePairs.Add(1, 1f);
        tierValuePairs.Add(2, 1.5f);
        tierValuePairs.Add(3, 2.5f);

        player = SessionManager.player;
    }

    public void GetNextLevelDifficulty()
    {
        int currentLevelDiff = SessionManager.difficulty;

        // You can access the values from the dictionary using tierValuePairs[currentLevelDiff]
    }

    private float CalculateKilledEnemyPercentage()
    {
        float maxEnemyScore = (levelGenerator.commonEnemySpawned * tierValuePairs[1]) + (levelGenerator.eliteEnemySpawned * tierValuePairs[2]) + tierValuePairs[3];
        float enemyScore = (levelGenerator.commonEnemyKilled * tierValuePairs[1]) + (levelGenerator.eliteEnemyKilled * tierValuePairs[2]) + tierValuePairs[3];

        return enemyScore / maxEnemyScore;
    }

    private float CalculateItemScorePercentage()
    {
        float treasureRoomCount = levelGraphGen.treasureCount;

        float maxTreasureScore = treasureRoomCount * tierValuePairs[3];
        float maxShopScore = 7.5f;

        float maxItemScore = maxTreasureScore + maxShopScore;

        int commonItemGet = SessionManager.commonItemGet;
        int rareItemGet = SessionManager.rareItemGet;
        int legendItemGet = SessionManager.legendItemGet;

        float itemScore = (commonItemGet * tierValuePairs[1]) + (rareItemGet * tierValuePairs[2]) + (legendItemGet * tierValuePairs[3]);

        return itemScore / maxItemScore;
    }

    private float CalculateDamageTakenPercentage()
    {
        float maxHealth = SessionManager.playerMaxHealth;
        float currentHealth = player.GetComponentInChildren<Stats>().currentHealth;

        return currentHealth / maxHealth;
    }

    private int CalculateNextLevelDifficulty()
    {
        float killedEnemyPercentage = CalculateKilledEnemyPercentage();
        Debug.Log("killedEnemyPercent = " + killedEnemyPercentage);

        float itemScorePercentage = CalculateItemScorePercentage();
        Debug.Log("itemScorePercentage = " + itemScorePercentage);

        float damageTakenPercentage = CalculateDamageTakenPercentage();
        Debug.Log("damageTakenPercent = " + damageTakenPercentage);

        int currentLevelDifficulty = SessionManager.difficulty;
        int maxLevel = SessionManager.maxExpertModeLevel;
        int currentLevel = SessionManager.currentExpertModeLevel;

        float deltaNewDifficulty = (((killedEnemyPercentage + itemScorePercentage + damageTakenPercentage) / 3) + 0.5f) * ((100 - currentLevelDifficulty) / (maxLevel - currentLevel));
        Debug.Log("Delta = " + deltaNewDifficulty);
        return (int)(currentLevelDifficulty + deltaNewDifficulty);
    }

    public void SetNextExpertLevelDifficulty()
    {
        SessionManager.difficulty = CalculateNextLevelDifficulty();
    }
}
