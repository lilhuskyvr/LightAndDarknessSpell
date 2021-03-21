using ThunderRoad;
using UnityEngine;

namespace LightAndDarknessSpell
{
    public class FrozenItem : MonoBehaviour
    {
        private Item _item;
        private Vector3 _defaultVelocity;
        private Vector3 _defaultAngularVelocity;

        private void Start()
        {
            _item = gameObject.GetComponentInParent<Item>();
            _defaultVelocity = _item.rb.velocity;
            _defaultAngularVelocity = _item.rb.angularVelocity;

            _item.rb.constraints = RigidbodyConstraints.FreezeAll;

            _item.OnGrabEvent += ItemOnOnGrabEvent;
            _item.OnTelekinesisGrabEvent += ItemOnOnTelekinesisGrabEvent;
            _item.OnDespawnEvent += ItemOnOnDespawnEvent;
        }

        private void ItemOnOnTelekinesisGrabEvent(Handle handle, SpellTelekinesis telegrabber)
        {
            Destroy(this);
        }

        private void ItemOnOnDespawnEvent()
        {
            Destroy(this);
        }

        private void ItemOnOnGrabEvent(Handle handle, RagdollHand ragdollhand)
        {
            Destroy(this);
        }


        private void OnDestroy()
        {
            _item.rb.constraints = RigidbodyConstraints.None;
            _item.rb.ResetInertiaTensor();

            _item.rb.velocity = _defaultVelocity;
            _item.rb.angularVelocity = _defaultAngularVelocity;
        }
    }
}