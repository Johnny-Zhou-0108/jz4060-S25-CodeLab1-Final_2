using UnityEngine;
using System;

[CreateAssetMenu(menuName = "ScriptableObjects/FloatValueSO")]
public class FloatValueSO : ScriptableObject
{
    [SerializeField]
    private float value; // Current value

    public event Action<float> OnValueChange; // Event for value changes

    public float Value
    {
        get => value;
        set
        {
            this.value = value;
            OnValueChange?.Invoke(this.value); // Trigger event
        }
    }
}
