using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    bool IsActive { get; set; }
    void OnActivated();
    void OnDeactivated();
}
