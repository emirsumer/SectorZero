using Mirror;
using UnityEngine;

public partial class CharacterController
{
    [SerializeField] private Transform rightHandTransform; 

    // Elimizdeki silah. SyncVar: server deðiþtirir, herkese gider. Deðiþince Hook_Weapon çalýþýr.
    [SyncVar(hook = nameof(Hook_Weapon))] private Weapon _attachedWeapon;

    private void HandleWeaponInput()
    {
        if (!isLocalPlayer || _attachedWeapon == null)
        {
            return; 
        }
        bool isFiring = _attachedWeapon.IsAutomatic ? InputManager.GetFireHeld() : InputManager.GetFireDown();

        if (isFiring)
        {
            _attachedWeapon.Fire(); 
        }
        if (InputManager.GetReloadDown())
        {
            _attachedWeapon.Reload(); 
        }
        if (InputManager.GetDropDown())
        {
            Server_DetachWeapon();
        }
    }

    private void UpdateWeaponUI()
    {
        if (!isLocalPlayer || _attachedWeapon == null || UIManager.Instance == null)
        {
            return;
        }

        UIManager.Instance.UpdateCrosshairPosition(_attachedWeapon.GetCrosshairScreenPos());

        UIManager.Instance.UpdateAmmoText(_attachedWeapon.CurrentAmmo, _attachedWeapon.ReserveAmmo, _attachedWeapon.IsReloading);
    }

    [Server]
    public void Server_AttachWeapon(Weapon targetWeapon)
    {
        if (_attachedWeapon != null)
        {
            return;
        }

        if (_isDead)
        {
            return;
        }

        _attachedWeapon = targetWeapon;  

        // Silahýn kontrol yetkisini bu oyuncuya ver (ateþ komutunu o gönderebilsin)
        _attachedWeapon.GetComponent<NetworkIdentity>().AssignClientAuthority(GetComponent<NetworkIdentity>().connectionToClient);

        AnimWeapon(); 
    }

    [Command]
    private void Server_DetachWeapon()
    {
        DropWeapon();
    }

    [Server]
    private void DropWeapon()
    {
        if (_attachedWeapon == null)
        {
            return;
        }

        _attachedWeapon.GetComponent<NetworkIdentity>().RemoveClientAuthority();

        _attachedWeapon = null;    
        AnimWeapon(); 
    }

    private void Hook_Weapon(Weapon oldWeapon, Weapon newWeapon)
    {
        if (newWeapon != null)
        {
            newWeapon.transform.position = rightHandTransform.position;     
            newWeapon.transform.rotation = rightHandTransform.rotation;    
            newWeapon.transform.parent = rightHandTransform;              
            newWeapon.OnAttach();                                          

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayPickup(newWeapon.transform.position);
            }

            if (isLocalPlayer && UIManager.Instance != null)
            {
                UIManager.Instance.SetCrosshairActive(true);
                UIManager.Instance.SetAmmoActive(true);
            }
        }
        else
        {
            if (oldWeapon == null)
            {
                return;
            }

            oldWeapon.OnDetach();                   
            oldWeapon.transform.parent = null;  

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayDrop(oldWeapon.transform.position);
            }

            if (isLocalPlayer && UIManager.Instance != null)
            {
                UIManager.Instance.SetCrosshairActive(false);
                UIManager.Instance.SetAmmoActive(false);
            }
        }
    }
}