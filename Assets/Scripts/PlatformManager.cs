using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    [SerializeField] private bool isLoader;
    [SerializeField] private GameObject platformPatternPrefab;
    [SerializeField] private Transform spawnLocation;

    private PlayerController playerController;

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isLoader)
        {
            if (other.CompareTag("Finish"))
            {
                var newPattern = Instantiate(platformPatternPrefab, spawnLocation.position, Quaternion.identity);
                playerController.platformPatterns.Add(newPattern);
                Debug.Log("Loading");
            }
        }
        else
        {
            if (other.CompareTag("Finish"))
            {
                playerController.platformPatterns.RemoveAt(0);
                Destroy(other.transform.parent.gameObject);
                Debug.Log("Unloading");
            }
        }
    }
}
