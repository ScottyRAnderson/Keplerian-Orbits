using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ToggleElement : MonoBehaviour
{
    public enum ToggleType
    { 
        TintSprite,
        ResizeSprite,
        SwapSprite
    }

    [SerializeField]
    private bool toggled;
    [SerializeField]
    private ToggleType toggleType = ToggleType.SwapSprite;
    [SerializeField]
    private Image imageComponent;

    [SerializeField]
    private Color tintColor = Color.white;
    [SerializeField]
    private Vector2 interactedSize;
    [SerializeField]
    private Sprite interactedSprite;

    private Color defaultColor;
    private Vector2 defaultSize;
    private Sprite defaultSprite;

    private void Awake()
    {
        if(!imageComponent){
            imageComponent = GetComponentInChildren<Image>();
        }

        defaultColor = imageComponent.color;
        defaultSize = imageComponent.rectTransform.sizeDelta;
        defaultSprite = imageComponent.sprite;
        SetToggled(toggled);
    }

    public void SetToggled(bool toggled)
    {
        this.toggled = toggled;
        switch (toggleType)
        {
            case ToggleType.TintSprite:
                imageComponent.color = this.toggled ? defaultColor * tintColor : defaultColor;
                break;
            case ToggleType.ResizeSprite:
                imageComponent.rectTransform.sizeDelta = this.toggled ? interactedSize : defaultSize;
                break;
            case ToggleType.SwapSprite:
                imageComponent.sprite = this.toggled ? interactedSprite : defaultSprite;
                break;
        }
    }

    public void OnToggle(){
        SetToggled(!toggled);
    }
}