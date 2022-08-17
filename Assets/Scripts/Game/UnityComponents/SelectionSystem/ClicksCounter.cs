using UnityEngine;
using UnityEngine.Events;

public class ClicksCounter : MonoBehaviour
{
    [SerializeField] private float _secondClickDelayTime;

    private PlayerInput _playerInput;
    private float _elapsedTime;

    public UnityAction SecondClickIsPerformed;

    private void Awake()
    {
        _playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        _elapsedTime = 0;
        _playerInput.Enable();
        _playerInput.Player.Click.performed += ctx => OnSecondClickPerformed();
    }

    private void OnDisable()
    {
        _playerInput.Player.Click.performed -= ctx => OnSecondClickPerformed();
        _playerInput.Disable();
    }

    private void Update()
    {
        if(_elapsedTime >= _secondClickDelayTime) 
        {
            this.enabled = false;
        }
        _elapsedTime += Time.deltaTime;
    }

    private void OnSecondClickPerformed() 
    {
        SecondClickIsPerformed?.Invoke();
        this.enabled = false;
    }
}