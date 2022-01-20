using Manicomio.ActionableObjects;
using UnityEngine;

namespace Manicomio.ActionableObjects {
    public class KeyItem : ActionableObject {
        protected override void InnerStart() { }

        protected override void InnerInteract(GameObject interactor = null) {
            ItemPickEvents.NotifyItemPick(this);
            Destroy(gameObject);
        }

        public override bool IsAnimationPlaying() => false;
    }
}