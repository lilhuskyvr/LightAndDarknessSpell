using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable ParameterHidesMember

namespace LightAndDarknessSpell
{
    public class SpellLightAndDarkness : SpellCastProjectile
    {
        private LightAndDarknessSpellController _lightAndDarknessSpellController;
        private List<Item> _items;

        public override void Load(SpellCaster spellCaster)
        {
            base.Load(spellCaster);
            _lightAndDarknessSpellController =
                GameManager.local.gameObject.GetComponent<LightAndDarknessSpellController>();
            _items = new List<Item>();
        }

        protected override void OnProjectileCollision(CollisionInstance collisionInstance)
        {
            base.OnProjectileCollision(collisionInstance);

            if (_lightAndDarknessSpellController.data.isLightPath)
            {
                _lightAndDarknessSpellController.lightSpellController.SpawnAngel(collisionInstance.contactPoint);
            }
            else
            {
                _lightAndDarknessSpellController.darknessSpellController.SpawnShadow(collisionInstance.contactPoint);
            }
        }

        public override void UpdateImbue()
        {
            base.UpdateImbue();
            if (imbue.energy >= imbue.maxEnergy)
            {
                var item = imbue.colliderGroup.collisionHandler.item;

                if (item.gameObject.GetComponent<AngelItem>() == null)
                    item.gameObject.AddComponent<AngelItem>();
            }
        }

        public override void OnImbueCollisionStart(CollisionInstance collisionInstance)
        {
            var item = collisionInstance.sourceCollider.attachedRigidbody.GetComponentInParent<Item>();
            var lad = GameManager.local.gameObject.GetComponent<LightAndDarknessSpellController>();
            if (lad.data.isLightPath)
            {
                try
                {
                    if (collisionInstance.targetCollider.attachedRigidbody != null)
                    {
                        var targetCreature =
                            collisionInstance.targetCollider.attachedRigidbody.GetComponentInParent<Creature>();

                        if (targetCreature != null)
                        {
                            if (!targetCreature.isPlayer && targetCreature.factionId != 2)
                            {
                                lad.lightSpellController.HeavenlyPunish(targetCreature, item);
                            }
                        }
                        else
                        {
                            var targetItem = collisionInstance.targetCollider.attachedRigidbody
                                .GetComponentInParent<Item>();

                            targetItem.mainHandler.TryRelease();
                        }
                    }
                }
                catch (Exception exception)
                {
                    //ignore
                }
            }
        }
    }
}