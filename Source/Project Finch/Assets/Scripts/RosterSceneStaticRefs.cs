using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MenuView {

    public class RosterSceneStaticRefs : MonoBehaviour
    {
        public static GameObject StatsGroup = null;
        [SerializeField] private GameObject StatsGroupObject = null;
        public static Text Aim = null;
        [SerializeField] private Text AimObject = null;
        public static Text Level = null;
        [SerializeField] private Text LevelObject = null;
        public static Text Experience = null;
        [SerializeField] private Text ExperienceObject = null;
        public static Text Fatigue = null;
        [SerializeField] private Text FatigueObject = null;
        public static Text HP = null;
        [SerializeField] private Text HPObject = null;
        public static Text Mobility = null;
        [SerializeField] private Text MobilityObject = null;

        private void Awake() {
            StatsGroup = StatsGroupObject;
            Aim = AimObject;
            Level = LevelObject;
            Experience = ExperienceObject;
            Fatigue = FatigueObject;
            HP = HPObject;
            Mobility = MobilityObject;
            Destroy(this);
        }
    }
}