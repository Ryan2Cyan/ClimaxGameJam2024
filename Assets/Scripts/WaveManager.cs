using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    EnemyManager enemyManager;

    float timer;
    [SerializeField] float waveInterval;
    [Range(1, 50)]
    [SerializeField] int waveSize;
    int waveCounter;
  

    //The wave manager tells the enemy manager to spawn enemies each wave
    void Start()
    {
        enemyManager = FindObjectOfType<EnemyManager>();
        if (enemyManager == null)
        {
            print("WARNING: No Enemy Manager in scene!");
        }
    }


    void Update()
    {
       
        if (timer >= waveInterval)
        {
            enemyManager.SpawnWave(waveSize);
            waveCounter++;
            print("WAVE " + waveCounter + " START!");
            UpdateWaveParamaters();

            timer = 0;
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    void UpdateWaveParamaters() //after each wave, we can adjust the no of enemies each wave will spawn
    {
        waveSize++;
    }
}