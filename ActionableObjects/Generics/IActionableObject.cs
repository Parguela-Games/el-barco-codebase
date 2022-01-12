using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manicomio.ActionableObjects
{
  public interface IActionableObject
  {
    void Interact();
    string GetInteractionText();
    bool IsInteracterActive();
    bool IsInteractiveByEnemies();
    bool IsAnimationPlaying();
  }
}