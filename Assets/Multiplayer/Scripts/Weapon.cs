using System;
using System.Collections;
using Mirror;
using UnityEngine;

public class Weapon : NetworkBehaviour
{
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private Collider trigger;
    [SerializeField] private Collider collider;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private bool isAutomatic;

    [SerializeField] private int maxCurrentAmmo = 30;   
    [SerializeField] private int maxReserveAmmo = 90;  
    [SerializeField] private float reloadTime = 3f;

    // SyncVar: server'da deðiþir, otomatik olarak herkese gider
    [SyncVar] private int _currentAmmo;    // Þarjördeki mermi
    [SyncVar] private int _reserveAmmo;    // Yedek mermi

    // hook = deðer deðiþince herkeste Hook_IsReloading çalýþýr (reload sesi için)
    [SyncVar(hook = nameof(Hook_IsReloading))] private bool _isReloading;

    private Rigidbody _rigidbody;
    private float _nextFireTime;        // Server: en erken ne zaman tekrar ateþ edilir
    private float _nextLocalFireTime;   // Client: ayný kontrol

    public bool IsAutomatic => isAutomatic;
    public int CurrentAmmo => _currentAmmo;
    public int ReserveAmmo => _reserveAmmo;
    public bool IsReloading => _isReloading;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnStartServer() //server baþlarken mermileri doldur
    {
        _currentAmmo = maxCurrentAmmo;
        _reserveAmmo = maxReserveAmmo;
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        CharacterController controller = other.GetComponent<CharacterController>();

        if (controller)
        {
            controller.Server_AttachWeapon(this);
        }
    }

    public void Fire()
    {
        if (_isReloading)
        {
            return;
        }

        if (Time.time < _nextLocalFireTime)
        {
            return;
        }
        _nextLocalFireTime = Time.time + fireRate;

        if (_currentAmmo <= 0)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayEmptyClick(transform.position);
            }
            return;
        }

        Server_Fire();
    }

    [Command]
    private void Server_Fire(NetworkConnectionToClient sender = null)
    {
        if (_isReloading || _currentAmmo <= 0)
        {
            return;
        }

        if (Time.time < _nextFireTime - 0.05f)
        {
            return;
        }

        if (BulletPool.Instance == null)
        {
            return;
        }

        _nextFireTime = Time.time + fireRate;
        _currentAmmo--;

        // Mermi alýnýr
        GameObject bullet = BulletPool.Instance.GetBullet(spawnTransform.position, spawnTransform.rotation);

        if (sender != null && sender.identity != null) // Mermiye kimin attýðýný yaz
        {
            bullet.GetComponent<Bullet>().shooterNetId = sender.identity.netId;
        }

        NetworkServer.Spawn(bullet);
    }

    public void Reload()
    {
        if (_isReloading || _currentAmmo >= maxCurrentAmmo || _reserveAmmo <= 0)
        {
            return;
        }

        Server_Reload();
    }

    [Command]
    private void Server_Reload()
    {
        if (_isReloading || _currentAmmo >= maxCurrentAmmo || _reserveAmmo <= 0)
        {
            return;
        }

        _isReloading = true;
        Invoke(nameof(FinishReload), reloadTime);
    }

    [Server]
    private void FinishReload()
    {
        int needed = maxCurrentAmmo - _currentAmmo;
        int toLoad = Mathf.Min(needed, _reserveAmmo);

        _currentAmmo += toLoad;
        _reserveAmmo -= toLoad;
        _isReloading = false;
    }

    // _isReloading deðiþince herkeste çalýþýr. Reload baþlayýnca ses çalar.
    private void Hook_IsReloading(bool oldValue, bool newValue)
    {
        if (newValue && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayReload(transform.position);
        }
    }
    public void OnAttach()
    {
        StopAllCoroutines();

        trigger.enabled = false;
        collider.enabled = false;
        _rigidbody.isKinematic = true;
    }

    public void OnDetach()
    {
        _rigidbody.isKinematic = false;
        _rigidbody.AddForce(-transform.forward * 10, ForceMode.VelocityChange);
        collider.enabled = true;
        StopAllCoroutines();
        StartCoroutine(ActivateTrigger());
    }

    private IEnumerator ActivateTrigger()
    {
        yield return new WaitForSeconds(1);
        trigger.enabled = true;
    }

    public Vector3 GetCrosshairScreenPos()
    {
        if (spawnTransform == null || Camera.main == null)
        {
            return new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        }

        Vector3 targetPoint = spawnTransform.position + (spawnTransform.forward * 30f);

        if (Physics.Raycast(spawnTransform.position, spawnTransform.forward, out RaycastHit hit, 30f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            targetPoint = hit.point;
        }

        return Camera.main.WorldToScreenPoint(targetPoint);
    }
}