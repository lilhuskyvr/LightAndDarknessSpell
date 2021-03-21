using System;
using System.Collections;
using ThunderRoad;
using UnityEngine;
using Random = System.Random;

namespace LightAndDarknessSpell
{
    public class Guardian : MonoBehaviour
    {
        private Creature _creature;

        private void Start()
        {
            _creature = GetComponent<Creature>();
            StartCoroutine(GuardianTransformation());
        }

        private IEnumerator GuardianTransformation()
        {
            var guardianHeight = 2;
            _creature.SetHeight(Player.characterData.height * guardianHeight);

            Catalog.GetData<ItemData>("SwordShortCommon").SpawnAsync(sword =>
            {
                sword.transform.localScale = guardianHeight*Vector3.one;
                sword.OnGrabEvent += (handle, hand) =>
                {
                    if (hand.playerHand == null)
                    {
                        hand.gripInfo.joint.anchor = hand.transform.InverseTransformPoint(hand.grip.position);
                    }
                    else
                    {
                        handle.Release();
                        sword.Despawn();
                    }
                };
                sword.transform.position = _creature.handLeft.transform.position;
                _creature.handLeft.Grab(sword.mainHandleLeft);
            });

            Catalog.GetData<ItemData>("SwordShortCommon").SpawnAsync(sword =>
            {
                sword.transform.localScale = guardianHeight*Vector3.one;
                sword.OnGrabEvent += (handle, hand) =>
                {
                    if (hand.playerHand == null)
                    {
                        hand.gripInfo.joint.anchor = hand.transform.InverseTransformPoint(hand.grip.position);
                    }
                    else
                    {
                        handle.Release();
                        sword.Despawn();
                    }
                };
                sword.transform.position = _creature.handRight.transform.position;
                _creature.handRight.Grab(sword.mainHandleRight);
            });
            
            yield return null;
        }
    }
}