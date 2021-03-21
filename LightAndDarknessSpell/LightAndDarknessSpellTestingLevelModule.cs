using System.Collections;
using IngameDebugConsole;
using ThunderRoad;
using UnityEngine;

// ReSharper disable UnusedMember.Global

namespace LightAndDarknessSpell
{
    public class LightAndDarknessSpellTestingLevelModule : LevelModule
    {
        public override IEnumerator OnLoadCoroutine(Level level)
        {
            DebugLogConsole.AddCommandInstance("lads_a",
                "Spawn Angel", "SpawnAngel",
                this);
            DebugLogConsole.AddCommandInstance("lads_b",
                "Spawn Banshee", "SpawnBanshee",
                this);
            DebugLogConsole.AddCommandInstance("lads_h",
                "Heavenly Swords", "HeavenlySwords",
                this);
            DebugLogConsole.AddCommandInstance("lads_s",
                "Shadow Walk", "ShadowWalk",
                this);


            return base.OnLoadCoroutine(level);
        }

        private void SpawnAngel()
        {
            var position = Player.local.transform.position + Player.local.transform.forward;

            GameManager.local.gameObject.GetComponent<LightAndDarknessSpellController>().lightSpellController
                .SpawnAngel(position);
        }

        private void SpawnBanshee()
        {
            var position = Player.local.transform.position + Player.local.transform.forward;

            GameManager.local.gameObject.GetComponent<LightAndDarknessSpellController>().darknessSpellController
                .SpawnShadow(position);
        }

        private void HeavenlySwords()
        {
            GameManager.local.gameObject.GetComponent<LightAndDarknessSpellController>().lightSpellController
                .HeavenlySwords();
        }

        private void ShadowWalk()
        {
            GameManager.local.gameObject.GetComponent<LightAndDarknessSpellController>().darknessSpellController.ShadowWalk();
        }
    }
}