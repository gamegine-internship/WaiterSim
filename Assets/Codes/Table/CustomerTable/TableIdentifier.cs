using UnityEngine;
using System.Collections;

public class TableIdentifier : MonoBehaviour
{
    [Header("Table Setup")]
    public int tableNumber;
    public bool chosen = false;

    [Header("Drink Movement Stages")]
    public Transform[] beforeGoToTable;   // Intermediate positions
    public Transform[] tableDrinkSps;     // Final positions

    [Header("Visual Indicators")]
    public GameObject[] indicator;

    [Header("Material Settings")]
    public MeshRenderer meshRenderer;
    public Material defaultMaterial;

    [Header("Dependencies")]
    
    public GameObject player;

    [Header("Animation Durations")]
    public float moveUpDuration = 2f;
    public float moveDownDuration = 2f;

    private void Start()
    {
        foreach (var ind in indicator)
            if (ind != null) ind.SetActive(false);

    }

    private void Update()
    {
        foreach (var ind in indicator)
            if (ind != null) ind.SetActive(chosen);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("TrayTag")) return;

    }
    
    private IEnumerator MoveSmoothly(Transform obj, Vector3 targetPos, Quaternion targetRot, float duration)
    {
        float elapsed = 0f;
        Vector3 startPos = obj.position;
        Quaternion startRot = obj.rotation;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            obj.position = Vector3.Lerp(startPos, targetPos, t);
            obj.rotation = Quaternion.Slerp(startRot, targetRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.position = targetPos;
        obj.rotation = targetRot;
        
    }
        
    }

    
 

