using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manicomio.ActionableObjects
{
  [RequireComponent(typeof(Light))]
  public class ActionableLight : MonoBehaviour, IActionableObject
  {
    Light actionableLight;

    // Start is called before the first frame update
    void Start()
    {
      actionableLight = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update() {}

    public void Interact() { 
      actionableLight.enabled = !actionableLight.enabled;
    }

    public string GetInteractionText() => "";

    public bool IsInteracterActive() => false;

    public bool IsInteractiveByEnemies() => false;

    public bool IsAnimationPlaying() => true;
  }
}
