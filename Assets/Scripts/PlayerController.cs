using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class PlayerController : MonoBehaviour
{


    private CharacterController _characterController;
    private Transform _camera;
    private Animator _animator;


    [SerializeField] private float _horizontal;
    [SerializeField] private float _vertical;
    [SerializeField] private float _movementSpeed=5;
    private float _turnSmoothVelocity;
    private float _turnSmoothTime=0.1f;
    [SerializeField] private float _jumpHeight=1;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private Vector3 _playerGravity;
    [SerializeField] LayerMask _floorLayer;
    [SerializeField] Transform _sensorPosition;
    [SerializeField] float _sensorRadius=0.5f;
    void Awake(){
        _characterController=GetComponent<CharacterController>();
        _animator=GetComponentInChildren<Animator>();
        _camera=Camera.main.transform;
    }
    void Update(){
        _horizontal=Input.GetAxis("Horizontal");
        _vertical=Input.GetAxis("Vertical");
        Movement();
        if(Input.GetButtonDown("Jump") && IsGrounded()) Jump();
        Gravity();
    }

    void Movement(){
        Vector3 direction = new Vector3(_horizontal,0,_vertical);
        // _characterController.Move(direction*_movementSpeed*Time.deltaTime);
        _animator.SetFloat("VelZ", direction.magnitude);
        _animator.SetFloat("VelX", 0);
        
        if(direction!=Vector3.zero){
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _camera.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, smoothAngle, 0);
            Vector3 moveDirection = Quaternion.Euler(0,targetAngle, 0)*Vector3.forward;
            _characterController.Move(moveDirection*_movementSpeed*Time.deltaTime);
        }
    }

    void Jump(){
        _playerGravity.y=Mathf.Sqrt(_jumpHeight*-2*_gravity);
        _animator.SetBool("IsJumping",true);
    }

    bool IsGrounded(){
        return Physics.CheckSphere(_sensorPosition.position, _sensorRadius, _floorLayer);
    }
    void Gravity(){
        if(!IsGrounded()) _playerGravity.y += _gravity *Time.deltaTime;
        else if(IsGrounded() && _playerGravity.y <0) {
            _playerGravity.y=-1;
            _animator.SetBool("IsJumping",false);
        }
        _characterController.Move(_playerGravity*Time.deltaTime);
    }

}