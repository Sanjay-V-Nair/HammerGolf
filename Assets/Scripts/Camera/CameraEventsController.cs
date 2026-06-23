using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HammerGolf
{
    public class CameraEventsController : MonoBehaviour
    {
        public static CameraEventsController Instance { get; private set; }

        public static EventHandler SwitchToBall;
        public static EventHandler SwitchToPlayer;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
    } 
}
