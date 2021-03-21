using System;
using ThunderRoad;
using UnityEngine;

namespace LightAndDarknessSpell
{
    public class FrozenRagdollCreature : MonoBehaviour
    {
        private Creature _creature;

        private void Start()
        {
            _creature = gameObject.GetComponentInParent<Creature>();

            foreach (var part in _creature.ragdoll.parts)
            {
                if (part.rb != null && part.gameObject.GetComponent<FrozenRagdollPart>() == null)
                {
                    part.gameObject.AddComponent<FrozenRagdollPart>();
                }
            }
        }

        private void Update()
        {
            if (_creature.ragdoll.isGrabbed || _creature.ragdoll.isTkGrabbed)
            {
                Destroy(this);
            }
        }

        private void OnDestroy()
        {
            foreach (var part in _creature.ragdoll.parts)
            {
                if (part.gameObject.GetComponent<FrozenRagdollPart>() != null)
                    Destroy(part.gameObject.GetComponent<FrozenRagdollPart>());
            }
        }
    }
}