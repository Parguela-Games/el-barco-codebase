using Manicomio.ActionableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Manicomio.ActionableObjects {
    public class PickableObject : ActionableObject {
        [Header("UI")]
        [Tooltip("Image shown in the inventory UI")]
        [SerializeField]
        Texture2D inventorySprite;

        [SerializeField]
        Mesh detailedMesh;

        [SerializeField]
        Material detailedMaterial;

        protected override void InnerStart() { }

        protected override void InnerInteract(GameObject interactor = null) {
            ItemPickEvents.NotifyItemPick(this);
            Destroy(gameObject);
        }

        public override bool IsAnimationPlaying() => false;

        public Texture2D GetInventorySprite() => inventorySprite;
        public Mesh GetDetailedMesh() => detailedMesh;
        public Material GetDetailedMaterial() => detailedMaterial;
    }
}