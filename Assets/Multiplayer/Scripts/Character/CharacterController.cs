using System;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public partial class CharacterController : NetworkBehaviour
{
    [SerializeField] private GameObject cameraObject;
    [SerializeField] private Collider capsuleCollider;
    [SerializeField] private GameObject deathCamera;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        cameraObject.SetActive(false);
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        InitUI();
        cameraObject.SetActive(true);
        SetCursor(false);
    }

    void Update()
    {
        if (_isDead)
        {
            return;
        }

        HandleMovementInput();
        HandleWeaponInput();
        UpdateCanvasToCamera();
    }

    private void LateUpdate()
    {
        LockSpine();
        UpdateWeaponUI();
    }

    private void SetCursor(bool isVisible)
    {
        if (isVisible)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

}