using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UnitClicker : MonoBehaviour
{
    [SerializeField] protected Camera Camera;
    [SerializeField] protected LayerMask ClickableLayers;
    [SerializeField] private ClicksCounter _clicksCounter;

    protected PlayerInput PlayerInput;

    public UnityAction<PlayerUnit> UnitSelectedByClick;
    public UnityAction<PlayerUnit> UnitSelectedByShiftClick;

    protected virtual void OnEnable()
    {
        _clicksCounter.SecondClickIsPerformed += OnMouseClick;
        _clicksCounter.enabled = false;
        PlayerInput = new PlayerInput();
        PlayerInput.Enable();
        PlayerInput.Player.Click.performed += ctx => CountClick();
    }

    protected virtual void OnDisable()
    {
        PlayerInput.Player.Click.performed -= ctx => CountClick();
        PlayerInput.Disable();
    }

    protected virtual void OnMouseClick() 
    {
        PlayerUnit unit = TryToSelect();

        if (unit != null)
            if (PlayerInput.Player.Shift.ReadValue<float>() != 1)
                UnitSelectedByClick?.Invoke(unit);
            else
                UnitSelectedByShiftClick?.Invoke(unit);
    }

    protected virtual PlayerUnit TryToSelect() 
    {
        Ray ray = Camera.ScreenPointToRay(PlayerInput.Player.ClickPosition.ReadValue<Vector2>());
        RaycastHit hit;
        Vector3 targetPosition = PlayerInput.Player.ClickPosition.ReadValue<Vector2>();

        if (DetectIfMouseOverUI() == null)        
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ClickableLayers))           
                if (hit.collider.TryGetComponent<PlayerUnit>(out PlayerUnit unit))               
                    return unit;
        
        return null;
    }

    protected GameObject DetectIfMouseOverUI()
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = PlayerInput.Player.ClickPosition.ReadValue<Vector2>();

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, raycastResults);

        if (raycastResults.Count > 0)
            return raycastResults[0].gameObject;
        else
            return null;
    }  

    private void CountClick() 
    {
        if(_clicksCounter != null) 
        {
            if (!_clicksCounter.enabled)
            {
                _clicksCounter.enabled = true;
            }
        }      
    }
}