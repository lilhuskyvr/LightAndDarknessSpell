using System;
using ThunderRoad;
using UnityEngine;

namespace LightAndDarknessSpell
{
    public class FrozenRagdollPart : MonoBehaviour
    {
        private RagdollPart _ragdollPart;
        private Vector3 _impact;
        private Vector3 _bluntImpact;
        private float _multiplier;

        private void Start()
        {
            _ragdollPart = GetComponent<RagdollPart>();
            _ragdollPart.rb.constraints = RigidbodyConstraints.FreezeAll;
            // _ragdollPart.rb.isKinematic = true;
            _multiplier = 1;
            _ragdollPart.collisionHandler.OnCollisionStartEvent += CollisionHandlerOnOnCollisionStartEvent;
        }

        private void CollisionHandlerOnOnCollisionStartEvent(CollisionInstance collisioninstance)
        {
            var damageVector = _multiplier * collisioninstance.sourceCollider.attachedRigidbody.mass *
                               collisioninstance.impactVelocity;
            _impact += damageVector;
            if (collisioninstance.damageStruct.damageType == DamageType.Blunt)
                _bluntImpact += damageVector;
            _multiplier += 0.5f;
        }

        private void OnDestroy()
        {
            _ragdollPart.rb.constraints = RigidbodyConstraints.None;
            // _ragdollPart.rb.isKinematic = false;
            _ragdollPart.rb.AddForce(_impact, ForceMode.Impulse);
            _ragdollPart.ragdoll.creature.locomotion.rb.AddForce(_bluntImpact, ForceMode.VelocityChange);
            _ragdollPart.collisionHandler.OnCollisionStartEvent -= CollisionHandlerOnOnCollisionStartEvent;
        }
    }
}