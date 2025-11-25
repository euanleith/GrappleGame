using System;
using UnityEngine;

[Serializable]
public class Cooldown : Decay {

    public Cooldown(float max, float decay = 1, Action f = null) : base(max, decay, f) {
    }

    protected override float GetDecay() {
        return decay * Time.deltaTime;
    }

}