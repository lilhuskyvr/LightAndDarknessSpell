using System;
using ThunderRoad;
using UnityEngine;

namespace LightAndDarknessSpell
{
    public class AngelItem : MonoBehaviour
    {
        private Item _item;

        private void Awake()
        {
            _item = GetComponent<Item>();
            var lad = GameManager.local.gameObject.GetComponent<LightAndDarknessSpellController>();
            if (lad.data.isLightPath)
                lad.lightSpellController.PaintItem(_item);
            _item.OnDespawnEvent += ItemOnOnDespawnEvent;
        }

        private void ItemOnOnDespawnEvent(EventTime eventtime)
        {
            if (eventtime != EventTime.OnEnd) return;
            Destroy(this);
        }
    }
}