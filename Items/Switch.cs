using Manicomio.ActionableObjects;
using UnityEngine;

namespace Manicomio.ActionableObjects {
    public class Switch : StandardActionableObject {
        [SerializeField] GameObject trigee;

        IActionableObject actionableTrigee;

        // Start is called before the first frame update
        protected override void InnerStart() {
            base.InnerStart();

            if (!trigee.TryGetComponent<IActionableObject>(out actionableTrigee)) {
                throw new System.Exception("Trigee in switch must implement IActionableObject");
            }
        }

        protected override void InnerInteract(GameObject interactor = null) {
            base.InnerInteract(interactor);
            actionableTrigee.Interact(interactor);
        }

        // Update is called once per frame
        void Update() {

        }
    }
}
