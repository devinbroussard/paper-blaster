using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MovementBehavior
{
    //Variables for getting the camera, move position and rigidbody of the player
    private Camera _camera;
    private Vector3 _movePosition;
    private Rigidbody _rb;
    private bool _thrusterOn;
    private Quaternion _targetRotation;
    private float _rotationTime;
    private Quaternion _previousRotation;
    [SerializeField]
    private Renderer _flameRenderer;

    public bool ThrusterOn
    {
        get { return _thrusterOn; }
        set { _thrusterOn = value; }
    }

    /// <summary>
    /// On start set the max speed and the move speed
    /// Get the rigidbody from the player and the main camera
    /// </summary>
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        MaxSpeed = 5;
        MoveSpeed = 2;
        _rb.drag = .75f;
        _camera = Camera.main;
        _flameRenderer.enabled = false;
    }

    /// <summary>
    /// Call the function to look at the cursor every frame
    /// </summary>
    private void FixedUpdate()
    {
        LookAtCursor();

        if (_thrusterOn)
        {
            ActivateThruster();
            _flameRenderer.enabled = true;
        }
        else
            _flameRenderer.enabled = false;
           
    }

    /// <summary>
    /// The player will turn its forward to look at where the cursor is on the screen
    /// </summary>
    private void LookAtCursor()
    {
        //Call for raycasting to get where the cursor actually is on the game screen
        RaycastHit hit;
        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        //If the cast is true an the mouse is on the screen
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            //Move position is set to the position of the cursor
            _movePosition = hit.point;
        }

        //Set the direction to look and rotate to that direction
        Vector3 direction = (_movePosition - transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(direction);
        if (rot != _targetRotation)
        {
            _targetRotation = rot;
            _previousRotation = transform.rotation;
            _rotationTime = 0;
        }


        transform.rotation = Quaternion.Lerp(_previousRotation, _targetRotation, _rotationTime += Time.deltaTime * 6.5f);

        //Set the new rotation without the x or z to only rotate on the y axis
        Quaternion newRot = transform.rotation;
        newRot.x = 0;
        newRot.z = 0;
        //Rotate to the new rotation
        transform.rotation = newRot;
    }

    public override void Move()
    {
        _rb.AddForce(MoveDirection * MoveSpeed, ForceMode.Acceleration);
    }

    /// <summary>
    /// When spacebar is pressed the thruster will activate, causing the player to move
    /// </summary>
    public void ActivateThruster()
    {
        MoveDirection = transform.forward;
        Move();
    }
}
