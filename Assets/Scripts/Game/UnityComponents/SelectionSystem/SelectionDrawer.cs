using UnityEngine;
using System;

public class SelectionDrawer : MonoBehaviour
{
    [SerializeField] private RectTransform _boxVisual;
    [SerializeField] private UnitDragger _dragger;
    [SerializeField] private Camera _camera;

    private float _scaleMultiplyer;
    private Vector2 _startPosition;
    private Vector2 _previousMousePosition;
    private Vector2 _currentMousePosition;
    private PlayerInput _playerInput;

    private const float _referenceScreenHeightFor16x9 = 1080;
    private const float _referenceScreenHeightFor16x10 = 1140;
    private const float _referenceProportion16x10 = 1.6f;

    public RectTransform BoxVisual => _boxVisual;

    protected void Awake()
    {
        _startPosition = Vector2.zero;
        _currentMousePosition = _startPosition;
        _playerInput = new PlayerInput();
    }

    protected void OnEnable()
    {
        float screenProportion = (float) Math.Round((double) _camera.pixelWidth / _camera.pixelHeight, 1);
        switch (screenProportion) 
        {
            case _referenceProportion16x10:
                _scaleMultiplyer = _referenceScreenHeightFor16x10 / _camera.pixelHeight;
                break;
            default:
                _scaleMultiplyer = _referenceScreenHeightFor16x9 / _camera.pixelHeight;
                break;
        }      
        _dragger.MouseButtonClicked += SetStartPosition;    
        _playerInput.Enable();        
    }

    protected void OnDisable()
    {
        Undraw();
        _dragger.MouseButtonClicked -= SetStartPosition;
        _playerInput.Disable();
    }

    private void Update()
    {
        _previousMousePosition = _currentMousePosition;
        _currentMousePosition = _playerInput.Player.ClickPosition.ReadValue<Vector2>();

        if(_previousMousePosition != _currentMousePosition)
            DrawVisual(_startPosition, _currentMousePosition);
    }

    private void SetStartPosition(Vector2 startPosition) 
    {
        _startPosition = startPosition;
    }

    private void Undraw() 
    {
        _currentMousePosition = Vector2.zero;
        DrawVisual(Vector2.zero, Vector2.zero);
    }

    private void DrawVisual(Vector2 startPosition, Vector2 endPosition)
    {
        Vector2 difference = (endPosition - startPosition);
        float differenceX = 0;
        float differenceY = 0;

        if (difference.x != 0)
            differenceX = difference.x / Mathf.Abs(difference.x);

        if(difference.y != 0)
            differenceY = difference.y / Mathf.Abs(difference.y);

        _boxVisual.localScale = new Vector3(differenceX, differenceY, _boxVisual.localScale.z);
        _boxVisual.position = startPosition;

        Vector2 boxSize = new Vector2(Mathf.Abs(startPosition.x - endPosition.x), Mathf.Abs(startPosition.y - endPosition.y)) * _scaleMultiplyer;
        _boxVisual.sizeDelta = boxSize;       
    }
}