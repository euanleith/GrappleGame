using System;
using UnityEngine;

[Serializable]
public class Decay {

    protected float decay;
    [SerializeField] private float max;
    [SerializeField] private float current;
    private Action onFinished;
    bool active = false;

    public Decay(float max, float decay = 1, Action onFinished = null) {
        this.max = max;
        this.decay = decay;
        this.onFinished = onFinished;
        Deactivate();
    }

    protected virtual float GetDecay() {
        return decay;
    }

    public float GetCurrent() {
        return current;
    }

    public void Update() {
        if (current >= 0) {
            current -= GetDecay();
        } else if (active) OnFinished();
    }

    public void Activate(float? duration = null) {
        current = duration ?? max;
        active = true;
    }

    public bool IsActive() {
        return active;
    }

    public void Deactivate() {
        current = 0;
    }

    private void OnFinished() {
        onFinished?.Invoke();
        active = false;
    }

    public void SetOnFinished(Action onFinished) {
        this.onFinished = onFinished;
    }
}

public struct Unit { }