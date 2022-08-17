using System.Collections.Generic;
using Game.Gameplay.Models.Units;
using UnityEngine;
using System.Linq;
using Zenject;

public class UnitSelector : MonoBehaviour
{
    [SerializeField] private UnitClicker _unitClicker;
    [SerializeField] private UnitClickerUI _unitClickerUI;
    [SerializeField] private UnitDragger _unitDragger;
    [SerializeField] private Camera _camera;
    [SerializeField] private ClicksCounter _clicksCounter;

    private UnitsInfo _unitsInfo;

    [Inject]
    public void Construct(UnitsInfo unitsInfo) 
    {
        _unitsInfo = unitsInfo;
        _unitsInfo.UnitsList = new List<PlayerUnit>();
        _unitsInfo.SelectedUnits = new List<PlayerUnit>();
    }

    private void OnEnable()
    {
        _unitClicker.UnitSelectedByClick += SelectByClick;
        _unitClicker.UnitSelectedByShiftClick += SelectByShiftClick;
        _unitClickerUI.UnitSelectedByClick += SelectByClick;
        _unitClickerUI.UnitSelectedByShiftClick += SelectByShiftClick;
        _unitDragger.MouseButtonWasReleased += SelectByDrag;
        _unitDragger.MouseButtonWasReleasedWithShift += DeselectByDrag;
    }

    private void OnDisable()
    {
        _unitClicker.UnitSelectedByClick -= SelectByClick;
        _unitClicker.UnitSelectedByShiftClick -= SelectByShiftClick;
        _unitClickerUI.UnitSelectedByClick -= SelectByClick;
        _unitClickerUI.UnitSelectedByShiftClick -= SelectByShiftClick;
        _unitDragger.MouseButtonWasReleased -= SelectByDrag;
        _unitDragger.MouseButtonWasReleasedWithShift -= DeselectByDrag;
    }

    private void SelectByClick(PlayerUnit unitToAdd) 
    {
        AddUnitToSelected(unitToAdd);
    }

    private void SelectByShiftClick(PlayerUnit unit)
    {
        if (!_unitsInfo.SelectedUnits.Contains(unit))        
            AddUnitToSelected(unit);        
        else        
            RemoveUnitFromSelected(unit);                  
    }

    private void SelectByDrag(RectTransform box, Vector2 mousePosition)
    {
        if(_clicksCounter.enabled)
            DeselectAll();
        Rect selectionArea = box.rect;
        selectionArea.center = new Vector2(-box.localScale.x * box.rect.width / 2 + mousePosition.x, -box.localScale.y * box.rect.height / 2 + mousePosition.y);
        List <PlayerUnit> units = _unitsInfo.UnitsList.Where(unit => selectionArea.Contains(_camera.WorldToScreenPoint(unit.transform.position))).ToList();

        if(units.Count > 0) 
        {
            foreach (var unit in units)
            {
                if (!_unitsInfo.SelectedUnits.Contains(unit))
                    AddUnitToSelected(unit);
            }
        }
    }

    private void DeselectByDrag(RectTransform box, Vector2 mousePosition)
    {
        Rect selectionArea = box.rect;
        selectionArea.center = new Vector2(-box.localScale.x * box.rect.width / 2 + mousePosition.x, -box.localScale.y * box.rect.height / 2 + mousePosition.y);
        List<PlayerUnit> units = _unitsInfo.UnitsList.Where(unit => selectionArea.Contains(_camera.WorldToScreenPoint(unit.transform.position))).ToList();
      
        if (units.Count > 0)
        {
            foreach (var unit in units)
            {
                if (_unitsInfo.SelectedUnits.Contains(unit))
                    RemoveUnitFromSelected(unit);
            }
        }
    }

    private void DeselectAll()
    {
        foreach(var unit in _unitsInfo.SelectedUnits) 
        {
            unit.SetUnactive();
            unit.UnitCard.SwitchFrameState(false);
        }
        _unitsInfo.SelectedUnits.Clear();
    }

    private void AddUnitToSelected(PlayerUnit unit) 
    {
        _unitsInfo.SelectedUnits.Add(unit);
        unit.SetActive();
        unit.UnitCard.SwitchFrameState(true);
    }

    private void RemoveUnitFromSelected(PlayerUnit unit) 
    {
        _unitsInfo.SelectedUnits.Remove(unit);
        unit.SetUnactive();
        unit.UnitCard.SwitchFrameState(false);
    }
}