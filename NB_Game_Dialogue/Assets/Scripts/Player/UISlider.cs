using UnityEngine;
using UnityEngine.UI;

public class UISlider : MonoBehaviour
{
    [SerializeField]
    private RectTransform sliderRectTransform; // RectTransform of the slider (Fill)

    [SerializeField]
    private FloatValueSO floatValue; // ScriptableObject holding the health value

    private Vector2 originalSize; // Store the original size of the slider

    private void OnEnable()
    {
        // Cache the original size of the slider
        originalSize = sliderRectTransform.sizeDelta;

        // Subscribe to value changes
        floatValue.OnValueChange += SetValue;
        SetValue(floatValue.Value); // Initialize slider
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        floatValue.OnValueChange -= SetValue;
    }

    public void SetValue(float currentValue)
    {
        // Clamp currentValue to ensure it's within the range [0, 1]
        currentValue = Mathf.Clamp01(currentValue);

        // Update the width of the slider based on the current value
        sliderRectTransform.sizeDelta = new Vector2(originalSize.x * currentValue, originalSize.y);

        Debug.Log($"Slider updated: {currentValue}, Width: {sliderRectTransform.sizeDelta.x}"); // Debugging output
    }
}
