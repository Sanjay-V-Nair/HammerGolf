using System.Collections.Generic;
using UnityEngine;

namespace HammerGolf.LevelSystem
{
    [CreateAssetMenu(fileName = "AllLevels", menuName = "HammerGolf/All Levels Data")]
    public class AllLevels : ScriptableObject
    {
        public List<LevelData> levels = new List<LevelData>();
    }
}
