#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using HammerGolf.LevelSystem;
using HammerGolf.Gameplay;
using HammerGolf.RotationSystem;

namespace HammerGolf.Editor
{
    public class SetupLevelSystem
    {
        [MenuItem("Tools/Setup Level System Scene")]
        public static void SetupSceneAndLevel()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            
            // 1. Create Highlight Material with new Custom Shader
            string highlightMatPath = "Assets/Materials/HighlightMaterial.mat";
            Material highlightMat = AssetDatabase.LoadAssetAtPath<Material>(highlightMatPath);
            if (highlightMat == null)
            {
                if (!AssetDatabase.IsValidFolder("Assets/Materials"))
                    AssetDatabase.CreateFolder("Assets", "Materials");
                
                Shader highlightShader = Shader.Find("HammerGolf/HighlightOutline");
                highlightMat = new Material(highlightShader);
                highlightMat.SetColor("_HighlightColor", Color.yellow);
                AssetDatabase.CreateAsset(highlightMat, highlightMatPath);
            }
            else
            {
                highlightMat.shader = Shader.Find("HammerGolf/HighlightOutline");
                highlightMat.SetColor("_HighlightColor", Color.yellow);
            }

            // 2. Setup Singletons
            GameObject singletons = new GameObject("Singletons");
            singletons.AddComponent<InputManager>();
            singletons.AddComponent<CameraEventsController>();

            // 3. Setup Player & Ball
            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player.prefab");
            GameObject ballPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Ball.prefab");

            GameObject playerInstance = null;
            GameObject ballInstance = null;

            if (playerPrefab != null) playerInstance = PrefabUtility.InstantiatePrefab(playerPrefab) as GameObject;
            if (ballPrefab != null) ballInstance = PrefabUtility.InstantiatePrefab(ballPrefab) as GameObject;

            if (playerInstance != null && ballInstance != null)
            {
                playerInstance.name = "Player";
                ballInstance.name = "Ball";

                var spinController = playerInstance.GetComponent<CharacterSpinController>();
                var ballController = ballInstance.GetComponent<BallThrowController>();
                
                if (spinController != null && ballController != null)
                {
                    SerializedObject so = new SerializedObject(ballController);
                    so.FindProperty("spinController").objectReferenceValue = spinController;
                    so.FindProperty("orbitPivot").objectReferenceValue = playerInstance.transform;
                    so.ApplyModifiedProperties();
                }
            }

            // 4. Setup Main Camera
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                var camController = mainCamera.gameObject.AddComponent<CameraController>();
                if (playerInstance != null && ballInstance != null)
                {
                    SerializedObject soCam = new SerializedObject(camController);
                    soCam.FindProperty("player").objectReferenceValue = playerInstance.transform;
                    soCam.FindProperty("ball").objectReferenceValue = ballInstance.GetComponent<Rigidbody>();
                    soCam.ApplyModifiedProperties();
                }
            }

            // 5. Setup Level Manager
            GameObject levelManagerGO = new GameObject("LevelManager");
            var levelManager = levelManagerGO.AddComponent<LevelManager>();
            GameObject envRoot = new GameObject("EnvironmentRoot");
            levelManager.environmentRoot = envRoot.transform;
            if (playerInstance != null)
                levelManager.playerTransform = playerInstance.transform;

            // 6. Setup Rotation Controller & TimeStop Manager
            GameObject rotationControllerGO = new GameObject("RotationController");
            rotationControllerGO.AddComponent<RotationController>();
            
            var rotationSelector = rotationControllerGO.AddComponent<RotationSelector>();
            if (mainCamera != null) {
                SerializedObject soSelector = new SerializedObject(rotationSelector);
                soSelector.FindProperty("_camera").objectReferenceValue = mainCamera;
                soSelector.ApplyModifiedProperties();
            }

            InputActionAsset inputActionAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/InputSystem_Actions.inputactions");
            GameObject timeStopManagerGO = new GameObject("TimeStopManager");
            var timeStopManager = timeStopManagerGO.AddComponent<TimeStopManager>();
            timeStopManager.inputActions = inputActionAsset;
            timeStopManager.highlightMaterial = highlightMat;

            // 7. Setup Goal Prefab
            string goalPath = "Assets/Prefabs/Goal.prefab";
            GameObject goalPrefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(goalPath);
            if (goalPrefabAsset != null)
            {
                GameObject goalPrefabRoot = PrefabUtility.LoadPrefabContents(goalPath);
                if (goalPrefabRoot != null)
                {
                    if (goalPrefabRoot.GetComponent<GoalView>() == null)
                    {
                        var view = goalPrefabRoot.AddComponent<GoalView>();
                        view.goalCollider = goalPrefabRoot.GetComponentInChildren<Collider>();
                        PrefabUtility.SaveAsPrefabAsset(goalPrefabRoot, goalPath);
                        Debug.Log("[Setup] Added GoalView to Goal.prefab");
                    }
                    PrefabUtility.UnloadPrefabContents(goalPrefabRoot);
                }
            }

            // 8. Create AllLevels Data & Sample Level Data
            string allLevelsPath = "Assets/Settings/AllLevels.asset";
            AllLevels allLevels = AssetDatabase.LoadAssetAtPath<AllLevels>(allLevelsPath);
            if (allLevels == null)
            {
                if (!AssetDatabase.IsValidFolder("Assets/Settings"))
                    AssetDatabase.CreateFolder("Assets", "Settings");

                allLevels = ScriptableObject.CreateInstance<AllLevels>();
                AssetDatabase.CreateAsset(allLevels, allLevelsPath);
            }

            string levelDataPath = "Assets/Settings/SampleLevelData.asset";
            LevelData sampleLevel = AssetDatabase.LoadAssetAtPath<LevelData>(levelDataPath);
            if (sampleLevel == null)
            {
                sampleLevel = ScriptableObject.CreateInstance<LevelData>();
                AssetDatabase.CreateAsset(sampleLevel, levelDataPath);
            }
            
            // Re-populate level data
            sampleLevel.goalPrefab = goalPrefabAsset;
            sampleLevel.goalPosition = new Vector3(0, 0, 20);

            sampleLevel.obstacles.Clear();
            var speedPad = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Pads/SpeedBoostPad.prefab");
            if (speedPad != null)
            {
                sampleLevel.obstacles.Add(new ObstacleData() { 
                    type = ObstacleType.Normal,
                    prefab1 = speedPad, 
                    position1 = new Vector3(0, 0, 5), 
                    eulerAngles1 = Vector3.zero 
                });
            }
            
            var sectionPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/RotationBlocks/Section.prefab");
            if (sectionPrefab != null)
            {
                sampleLevel.obstacles.Add(new ObstacleData() { 
                    type = ObstacleType.Normal,
                    prefab1 = sectionPrefab, 
                    position1 = new Vector3(0, 0, 10), 
                    eulerAngles1 = Vector3.zero,
                    isRotatable = true,
                    rotationAxis = Vector3.forward // Explicitly demonstrating the dynamic override
                });
            }

            var portalPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Portal.prefab");
            if (portalPrefab != null)
            {
                sampleLevel.obstacles.Add(new ObstacleData() {
                    type = ObstacleType.Portal,
                    prefab1 = portalPrefab,
                    position1 = new Vector3(3, 0, 15),
                    eulerAngles1 = Vector3.zero,
                    prefab2 = portalPrefab,
                    position2 = new Vector3(-3, 0, 15),
                    eulerAngles2 = Vector3.zero
                });
            }
            
            EditorUtility.SetDirty(sampleLevel);

            // Add sample level to AllLevels if missing
            if (!allLevels.levels.Contains(sampleLevel))
            {
                allLevels.levels.Add(sampleLevel);
                EditorUtility.SetDirty(allLevels);
            }
            
            levelManager.allLevels = allLevels;
            levelManager.currentLevelIndex = 0;

            // Save Scene
            string scenePath = "Assets/Scenes/LevelSystemScene.unity";
            EditorSceneManager.SaveScene(scene, scenePath);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[Setup] Fixes applied. Level System Scene setup complete.");
        }
    }
}
#endif
