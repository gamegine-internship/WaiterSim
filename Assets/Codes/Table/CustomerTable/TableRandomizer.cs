using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class TableRandomizer : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Table Setup")]
    public GameObject tablePrefab;
    public int numberOfTables = 14;

    void Start()
    {
        SpawnTables();
    }

    void SpawnTables()
    {
        List<Transform> availablePositions = new List<Transform>(spawnPoints);
        int spawnCount = Mathf.Min(numberOfTables, availablePositions.Count);
        
        int chosenID = Random.Range(1, spawnCount + 1);
        
        for (int i = 0; i < spawnCount; i++)
        {
            int index = Random.Range(0, availablePositions.Count );
            Transform chosenSpot = availablePositions[index];

            GameObject table = Instantiate(tablePrefab, chosenSpot.position, chosenSpot.rotation);

            
            TableIdentifier tableID = table.GetComponent<TableIdentifier>();
            if (tableID != null)
            {
                tableID.tableNumber = i + 1;
            }

            if (chosenID == tableID.tableNumber)
            {
                tableID.ChosenAccepter();
            }

            availablePositions.RemoveAt(index);
        }
        
        
    }
}