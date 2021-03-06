using System;
using ThunderRoad;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LightAndDarknessSpell
{
    // ReSharper disable once UnusedType.Global
    public class SpellLightAndDarknessMergeFire : SpellMergeData
    {
        public override void Merge(bool active)
        {
            base.Merge(active);

            if (active || currentCharge < 1.0f)
                return;

            var ladc = GameManager.local.gameObject.GetComponent<LightAndDarknessSpellController>();

            if (ladc.data.isLightPath)
            {
                ladc.lightSpellController.HeavenlyFireArrows();
            }
            else
            {
                ladc.darknessSpellController.ShadowWalk();
            }

            currentCharge = 0;
        }
    }
}