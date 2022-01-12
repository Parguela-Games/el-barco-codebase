using Manicomio.ActionableObjects;
using UnityEngine;

public class Switch : StandardActionableObject
{
  [SerializeField] GameObject trigee;

  IActionableObject actionableTrigee;

  // Start is called before the first frame update
  protected override void InnerStart()
  {
    base.InnerStart();

    if (!trigee.TryGetComponent<IActionableObject>(out actionableTrigee))
    {
      throw new System.Exception("Trigee in switch must implement IActionableObject");
    }
  }

  protected override void InnerInteract() {
    base.InnerInteract();
    actionableTrigee.Interact();
  }

  // Update is called once per frame
  void Update()
  {

  }
}
