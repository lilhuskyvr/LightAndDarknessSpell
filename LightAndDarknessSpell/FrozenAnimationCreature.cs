using ThunderRoad;
using UnityEngine;

namespace LightAndDarknessSpell
{
    public class FrozenAnimationCreature : MonoBehaviour
    {
        private Creature _creature;
        private float _defaultLocomotionSpeed;
        private float _defaultAnimatorSpeed;
        private string _defaultCreatureBrainId;

        private void Start()
        {
            _creature = gameObject.GetComponentInParent<Creature>();
            _defaultCreatureBrainId = _creature.brain.instance.id;
            _defaultLocomotionSpeed = _creature.locomotion.speed;
            _defaultAnimatorSpeed = _creature.animator.speed;
            _creature.brain.Stop();
            if (_creature.animator.isHuman)
            {
                _creature.brain.Load("FrozenCreature");
            }

            _creature.StopAnimation();
            _creature.brain.StopAllCoroutines();
            _creature.locomotion.MoveStop();
            _creature.locomotion.speed = 0;
            _creature.animator.speed = 0;
        }

        private void OnDestroy()
        {
            _creature.brain.Load(_defaultCreatureBrainId);
            _creature.locomotion.speed = _defaultLocomotionSpeed;
            _creature.animator.speed = _defaultAnimatorSpeed;
        }
    }
}