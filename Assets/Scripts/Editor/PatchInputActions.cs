#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.InputSystem;
using System.IO;
using UnityEngine;

namespace HammerGolf.Editor
{
    [InitializeOnLoad]
    public class PatchInputActions
    {
        static PatchInputActions()
        {
            EditorApplication.delayCall += () =>
            {
                string path = "Assets/InputSystem_Actions.inputactions";
                if (File.Exists(path))
                {
                    var text = File.ReadAllText(path);
                    if (!text.Contains("TimeStop"))
                    {
                        var asset = InputActionAsset.FromJson(text);
                        var map = asset.FindActionMap("Player");
                        if (map != null)
                        {
                            map.AddAction("TimeStop", type: InputActionType.Button, binding: "<Keyboard>/f");
                            map.AddAction("RotateSelected", type: InputActionType.Button, binding: "<Keyboard>/r");
                            map.AddAction("Restart", type: InputActionType.Button, binding: "<Keyboard>/p");
                            File.WriteAllText(path, asset.ToJson());
                            AssetDatabase.ImportAsset(path);
                            Debug.Log("[PatchInputActions] Added TimeStop, RotateSelected, and Restart actions to InputSystem_Actions.inputactions.");
                        }
                    }
                }
            };
        }
    }
}
#endif
