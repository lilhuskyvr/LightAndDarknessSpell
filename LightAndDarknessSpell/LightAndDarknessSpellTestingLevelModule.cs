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
            DebugLogConsole.AddCommandInstance("lads_hs",
                "Heavenly Swords", "HeavenlySwords",
                this);
            DebugLogConsole.AddCommandInstance("lads_hf",
                "Heavenly Meteours", "HeavenlyFireArrows",
                this);
            
            DebugLogConsole.AddCommandInstance("lads_hl",
                "Heavenly Lightning", "HeavenlyLightning",
                this);
            
            DebugLogConsole.AddCommandInstance("lads_ho",
                "Heavenly Objects", "HeavenlyObjects",
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
        
        private void HeavenlyFireArrows()
        {
            GameManager.local.gameObject.GetComponent<LightAndDarknessSpellController>().lightSpellController
                .HeavenlyFireArrows();
        }
        
        
        private void HeavenlyLightning()
        {
            GameManager.local.gameObject.GetComponent<LightAndDarknessSpellController>().lightSpellController
                .HeavenlyLightning();
        }
        
        private void HeavenlyObjects()
        {
            GameManager.local.gameObject.GetComponent<LightAndDarknessSpellController>().lightSpellController
                .HeavenlyObjects();
        }

        private void ShadowWalk()
        {
            GameManager.local.gameObject.GetComponent<LightAndDarknessSpellController>().darknessSpellController.ShadowWalk();
        }
    }
}