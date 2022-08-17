using UnityEngine;
using UnityEngine.Events;

public class UnitDragger : MonoBehaviour
{
    [SerializeField] protected Camera Camera;
    [SerializeField] protected SelectionDrawer Drawer;    

    protected PlayerInput InputOfPlayer;
    protected Vector2 StartPosition;

    public UnityAction<Vector2> MouseButtonClicked;
    public UnityAction<RectTransform, Vector2> MouseButtonWasReleased;
    public UnityAction<RectTransform, Vector2> MouseButtonWasReleasedWithShift;

    protected void Awake()
    {
        StartPosition = Vector2.zero;
        InputOfPlayer = new PlayerInput();
    }

    protected void OnEnable()
    {
        InputOfPlayer.Enable();
        InputOfPlayer.Player.Click.performed += ctx => OnClick();
        InputOfPlayer.Player.Click.canceled += ctx => OnRelease();
    }

    protected void OnDisable()
    {        
        InputOfPlayer.Player.Click.performed -= ctx => OnClick();
        InputOfPlayer.Player.Click.canceled -= ctx => OnRelease();
        InputOfPlayer.Disable();
    }

    private void OnClick() 
    {
        Drawer.enabled = true;
        StartPosition = InputOfPlayer.Player.ClickPosition.ReadValue<Vector2>();
        MouseButtonClicked?.Invoke(StartPosition);
    }

    private void OnRelease() 
    {
        if (InputOfPlayer.Player.Shift.ReadValue<float>() == 1)
            MouseButtonWasReleasedWithShift?.Invoke(Drawer.BoxVisual, InputOfPlayer.Player.ClickPosition.ReadValue<Vector2>());
        else
            MouseButtonWasReleased?.Invoke(Drawer.BoxVisual, InputOfPlayer.Player.ClickPosition.ReadValue<Vector2>());

        Drawer.enabled = false;
    }
}