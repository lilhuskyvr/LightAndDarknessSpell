using System;
using ThunderRoad;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LightAndDarknessSpell
{
    // ReSharper disable once UnusedType.Global
    public class SpellLightAndDarknessMergeLightning : SpellMergeData
    {
        public override void Merge(bool active)
        {
            base.Merge(active);

            if (active || currentCharge < 1.0f)
                return;

            var ladc = GameManager.local.gameObject.GetComponent<LightAndDarknessSpellController>();

            if (ladc.data.isLightPath)
            {
                ladc.lightSpellController.HeavenlyLightning();
            }
            else
            {
                ladc.darknessSpellController.ShadowWalk();
            }

            currentCharge = 0;
        }
    }
}