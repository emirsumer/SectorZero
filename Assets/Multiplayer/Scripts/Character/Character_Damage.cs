using Mirror;
using UnityEngine;

public partial class CharacterController
{
    [SerializeField] private float respawnTime = 5f; 

    // Karakterin caný. SyncVar: server deðiþtirir, herkese gider. Deðiþince Hook_Health çalýþýr.
    [SyncVar(hook = nameof(Hook_Health))] private float _health = 100;

    private bool _isDead;

    private void ApplyDamage(float damageAmount)
    {
        if (_isDead)
        {
            return;
        }

        _health = Mathf.Clamp(_health - damageAmount, 0, 100);

        if (_health <= 0)
        {
            _isDead = true;                         
            DropWeapon();                           
            Invoke(nameof(Respawn), respawnTime); //r.Time kadar bekle , sonra Respawn çaðýr
        }
    }

    [Server]
    private void Respawn()
    {
        if (!_isDead)
        {
            return;
        }

        Transform startPosition = NetworkManager.singleton.GetStartPosition();

        if (startPosition != null)
        {
            transform.position = startPosition.position;  
            transform.rotation = startPosition.rotation;  
        }

        ResetMovement();   

        _isDead = false;    

        _health = 100;
    }

    private void Hook_Health(float oldHealth, float newHealth)
    {
        UpdateHealthBar();    

        if (newHealth > 0)
        {
            if (oldHealth <= 0)  
            {
                Revive();
            }
            else if (newHealth < oldHealth)
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayHit(transform.position);
                }
            }

            return;  
        }

        Die();
    }
    private void Die()
    {
        _isDead = true;
        AnimDeath();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDeath(transform.position);
        }

        _rigidbody.isKinematic = true;
        capsuleCollider.enabled = false;

        if (!isLocalPlayer)
        {
            return;
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetCrosshairActive(false);
            UIManager.Instance.SetAmmoActive(false);
        }

        cameraObject.SetActive(false);
        deathCamera.SetActive(true);
    }
    private void Revive()
    {
        _isDead = false;

        _rigidbody.isKinematic = false;     
        capsuleCollider.enabled = true;   

        AnimRespawn();   

        if (!isLocalPlayer)
        {
            return;
        }

        deathCamera.SetActive(false);    
        cameraObject.SetActive(true);  

        if (_attachedWeapon != null && UIManager.Instance != null)
        {
            UIManager.Instance.SetCrosshairActive(true);
            UIManager.Instance.SetAmmoActive(true);
        }
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        Bullet bullet = other.GetComponent<Bullet>();

        if (bullet == null)
        {
            return;
        }

        if (!bullet.gameObject.activeInHierarchy)
        {
            return;
        }

        if (bullet.shooterNetId == netId)
        {
            return;
        }

        if (_isDead)
        {
            return;
        }

        ApplyDamage(15);  
        bullet.ReturnToPool(); 
    }
}