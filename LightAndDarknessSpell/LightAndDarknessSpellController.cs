using System;
using System.Collections;
using ThunderRoad;
using UnityEngine;

namespace LightAndDarknessSpell
{
    public class LightAndDarknessSpellData
    {
        // ReSharper disable once InconsistentNaming
        public bool isLightPath { get; set; }
    }

    public class LightAndDarknessSpellController : MonoBehaviour
    {
        public LightAndDarknessSpellData data = new LightAndDarknessSpellData();
        public LightSpellController lightSpellController = new LightSpellController();
        public DarknessSpellController darknessSpellController = new DarknessSpellController();
    }
}