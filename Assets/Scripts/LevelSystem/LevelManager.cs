using System.Collections.Generic;
using UnityEngine;
using HammerGolf.RotationSystem;

namespace HammerGolf.LevelSystem
{
    public class LevelManager : MonoBehaviour
    {
        [Header("Settings")]
        public AllLevels allLevels;
        public int currentLevelIndex = 0;
        public Transform environmentRoot;
        public Transform playerTransform;

        private List<GameObject> spawnedElements = new List<GameObject>();

        private void Start()
        {
            if (allLevels != null && allLevels.levels.Count > 0)
            {
                LoadLevel(currentLevelIndex);
            }
        }

        public void LoadLevel(int index)
        {
            if (allLevels == null || index < 0 || index >= allLevels.levels.Count)
            {
                Debug.LogWarning("[LevelManager] Invalid level index or missing AllLevels data.");
                return;
            }

            ClearLevel();
            currentLevelIndex = index;
            LevelData levelData = allLevels.levels[index];

            if (playerTransform != null)
            {
                playerTransform.position = levelData.playerSpawnPosition;
            }

            if (levelData.goalPrefab != null)
            {
                GameObject goalInstance = Instantiate(levelData.goalPrefab, levelData.goalPosition, Quaternion.identity, environmentRoot);
                spawnedElements.Add(goalInstance);
            }

            foreach (var obstacle in levelData.obstacles)
            {
                if (obstacle.type == ObstacleType.Normal)
                {
                    if (obstacle.prefab1 != null)
                    {
                        GameObject instance = Instantiate(obstacle.prefab1, obstacle.position1, Quaternion.Euler(obstacle.eulerAngles1), environmentRoot);
                        spawnedElements.Add(instance);
                        ProcessObstacleProperties(instance, obstacle);
                    }
                }
                else if (obstacle.type == ObstacleType.Portal)
                {
                    if (obstacle.prefab1 != null && obstacle.prefab2 != null)
                    {
                        GameObject portal1 = Instantiate(obstacle.prefab1, obstacle.position1, Quaternion.Euler(obstacle.eulerAngles1), environmentRoot);
                        GameObject portal2 = Instantiate(obstacle.prefab2, obstacle.position2, Quaternion.Euler(obstacle.eulerAngles2), environmentRoot);
                        
                        spawnedElements.Add(portal1);
                        spawnedElements.Add(portal2);

                        ProcessObstacleProperties(portal1, obstacle);
                        ProcessObstacleProperties(portal2, obstacle);
                        
                        var interaction1 = portal1.GetComponentInChildren<PortalInteraction>();
                        var interaction2 = portal2.GetComponentInChildren<PortalInteraction>();
                        
                        if (interaction1 != null && interaction2 != null)
                        {
                            interaction1.SetLinkedPortal(interaction2);
                            interaction2.SetLinkedPortal(interaction1);
                        }
                    }
                }
            }
        }
        
        private void ProcessObstacleProperties(GameObject instance, ObstacleData data)
        {
            // Apply Rotatable Override
            if (data.isRotatable)
            {
                var existingRotatable = instance.GetComponent<IRotatable>();
                var registrar = instance.GetComponent<RotatableRegistrar>();
                
                // If it doesn't have a registrar, we need to add one.
                if (registrar == null)
                {
                    registrar = instance.AddComponent<RotatableRegistrar>();
                }
                
                // Add DynamicRotatable
                var dynRotatable = instance.AddComponent<DynamicRotatable>();
                dynRotatable.RotationAxis = data.rotationAxis;
                
                // If there's an existing MonoBehaviour implementing IRotatable, destroy it
                if (existingRotatable != null && existingRotatable is MonoBehaviour mb)
                {
                    DestroyImmediate(mb);
                }
                
                // Hook the new one up without losing collider references
                registrar.SetRotatable(dynRotatable);
            }
        }

        public void ClearLevel()
        {
            foreach (var element in spawnedElements)
            {
                if (element != null)
                {
                    Destroy(element);
                }
            }
            spawnedElements.Clear();
        }
    }
}
