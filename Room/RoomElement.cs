using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface RoomElement
{
    // todo maybe make this abstract, since not all elements will require init/reset/disable
    //  also might want to put bool enabled here
    public void Init();
    public void Reset();
    public void Disable();
}
