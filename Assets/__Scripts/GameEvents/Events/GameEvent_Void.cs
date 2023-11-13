using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameEvents/GameEvent Void")]
public class GameEvent_Void : BaseGameEvent<Void>
{
    public void Raise() => Raise(new Void());
}