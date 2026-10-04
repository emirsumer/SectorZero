using Mirror;
using UnityEngine;

public partial class CharacterController
{
    [SerializeField][SyncVar] private float moveSpeed = 3;    
    [SerializeField] private float rotateSpeed = 15;           
    [SerializeField] private Transform camParent;          

    private float _pitch;   // Kameranýn yukarý-aþaðý açýsý (server'da tutulur)

    private void HandleMovementInput()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        // Hareket ve fare deðerlerini server'a gönder
        Server_Move(InputManager.GetHorizontal(), InputManager.GetVertical(), InputManager.GetLookDelta());

        if (InputManager.GetRunDown())
        {
            Server_Run(true);
        }
        if (InputManager.GetRunUp())
        {
            Server_Run(false);
        }
    }

    [Command]
    private void Server_Move(float horizontal, float vertical, Vector2 mouseDelta)
    {
        if (_isDead)
        {
            return;
        }

        // Çaprazda ekstra hýzlanmayý engeller (vektör uzunluðu en fazla 1 olur)
        Vector2 inputVector = Vector2.ClampMagnitude(new Vector2(horizontal, vertical), 1f);

        Vector3 moveDirection = (transform.forward * inputVector.y) + (transform.right * inputVector.x);

        _rigidbody.linearVelocity = (moveDirection * moveSpeed) + (Vector3.up * _rigidbody.linearVelocity.y);

        transform.Rotate(Vector3.up * mouseDelta.x * rotateSpeed);

        // Fareyi yukarý-aþaðý çekince kamera döner ama -80 ile +80 derece arasýnda kalýr (takla atmasýn)
        _pitch = Mathf.Clamp(_pitch + mouseDelta.y * rotateSpeed, -80f, 80f);
        camParent.localRotation = Quaternion.Euler(_pitch, 0, 0);

        AnimMovement(vertical, horizontal);
    }

    [Command]
    private void Server_Run(bool isRunning)
    {
        moveSpeed = isRunning ? 7 : 3;      
        AnimIsRunning(isRunning);
    }

    [Server]
    private void ResetMovement()
    {
        _pitch = 0;                                     
        camParent.localRotation = Quaternion.identity; 
        moveSpeed = 3;                                 
    }
}