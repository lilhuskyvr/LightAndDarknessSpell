using System.Collections;
using IngameDebugConsole;
using ThunderRoad;
using UnityEngine;

// ReSharper disable UnusedMember.Global

namespace LightAndDarknessSpell
{
    public class LightAndDarknessSpellTestingLevelModule : LevelModule
    {
        private LightAndDarknessSpellController _lightAndDarknessSpellController;

        public override IEnumerator OnLoadCoroutine(Level level)
        {
            
            DebugLogConsole.AddCommandInstance("lads_a",
                "Spawn Angel", "SpawnAngel",
                this);
            DebugLogConsole.AddCommandInstance("lads_b",
                "Spawn Banshee", "SpawnBanshee",
                this);
            DebugLogConsole.AddCommandInstance("lads_g",
                "Spawn Guardian", "SpawnGuardian",
                this);
            DebugLogConsole.AddCommandInstance("lads_h",
                "Heavenly Swords", "HeavenlySwords",
                this);
            DebugLogConsole.AddCommandInstance("lads_s",
                "Shadow Walk", "ShadowWalk",
                this);
            
            _lightAndDarknessSpellController =
                GameManager.local.gameObject.GetComponent<LightAndDarknessSpellController>();
            return base.OnLoadCoroutine(level);
        }

        private void SpawnAngel()
        {
            var position = Player.local.transform.position + Player.local.transform.forward;

            _lightAndDarknessSpellController.lightSpellController.SpawnAngel(position);
        }

        private void SpawnBanshee()
        {
            var position = Player.local.transform.position + Player.local.transform.forward;

            _lightAndDarknessSpellController.darknessSpellController.SpawnBanshee(position);
        }

        private void SpawnGuardian()
        {

            _lightAndDarknessSpellController.SpawnGuardian();
        }

        private void HeavenlySwords()
        {
            _lightAndDarknessSpellController.lightSpellController.HeavenlySwords();
        }
        
        private void ShadowWalk()
        {
            _lightAndDarknessSpellController.darknessSpellController.ShadowWalk();
        }
    }
}