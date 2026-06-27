using System;
using System.Collections.Generic;
using UnityEngine;

namespace HammerGolf.LevelSystem
{
    public enum ObstacleType
    {
        Normal,
        Portal
    }

    [Serializable]
    public struct ObstacleData
    {
        public ObstacleType type;
        
        [Header("Main Object")]
        public GameObject prefab1;
        public Vector3 position1;
        public Vector3 eulerAngles1;
        
        [Header("Secondary Object (For Portal)")]
        public GameObject prefab2;
        public Vector3 position2;
        public Vector3 eulerAngles2;
        
        [Header("Rotation Settings")]
        public bool isRotatable;
        public Vector3 rotationAxis; // e.g. (0,1,0) for Y-axis
    }

    [CreateAssetMenu(fileName = "NewLevelData", menuName = "HammerGolf/Level Data")]
    public class LevelData : ScriptableObject
    {
        [Header("Player")]
        public Vector3 playerSpawnPosition = new Vector3(0, 1, 0);

        [Header("Goal")]
        public GameObject goalPrefab;
        public Vector3 goalPosition = new Vector3(0, 0, 20);

        [Header("Obstacles")]
        public List<ObstacleData> obstacles = new List<ObstacleData>();
    }
}
