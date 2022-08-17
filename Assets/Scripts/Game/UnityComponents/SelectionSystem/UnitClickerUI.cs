using UnityEngine;

public class UnitClickerUI : UnitClicker
{
    protected override void OnEnable()
    {
        PlayerInput = new PlayerInput();
        PlayerInput.Enable();
        PlayerInput.Player.Click.performed += ctx => OnMouseClick();
    }

    protected override void OnDisable()
    {
        PlayerInput.Player.Click.performed -= ctx => OnMouseClick();
        PlayerInput.Disable();
    }

    protected override PlayerUnit TryToSelect()
    {
        GameObject objectUI = DetectIfMouseOverUI();

        if (objectUI != null) 
        {
            if (objectUI.TryGetComponent<Card>(out Card card))
                return card.TryToGetUnit();
        }

        return null;
    }
}