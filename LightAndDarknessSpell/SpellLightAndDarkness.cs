﻿using System;
using System.Collections;
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

        public override void Load(SpellCaster spellCaster)
        {
            base.Load(spellCaster);
            _lightAndDarknessSpellController = GameManager.local.gameObject.GetComponent<LightAndDarknessSpellController>();
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
                _lightAndDarknessSpellController.darknessSpellController.SpawnBanshee(collisionInstance.contactPoint);
            }
        }

        public override void OnImbueCollisionStart(CollisionInstance collisionInstance)
        {
            var lad = GameManager.local.gameObject.GetComponent<LightAndDarknessSpellController>();
            if (lad.data.isLightPath)
            {
                lad.lightSpellController.TryBanish(collisionInstance);
            }
            else
            {
                lad.darknessSpellController.TryDrainingBlood(collisionInstance);
            }
        }
    }
}