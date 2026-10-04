using System;
using Mirror;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private float force;
    [SerializeField] private float duration = 5f;

    [HideInInspector] public uint shooterNetId; // Mermiyi atan karakterin numarasý

    private Rigidbody _rigidbody;
    private bool _isReturnedToPool;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    public override void OnStartClient()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayShot(transform.position);
        }
    }

    // Sadece server'da, mermi her spawn olduðunda çalýþýr
    public override void OnStartServer()
    {
        _isReturnedToPool = false;

        CancelInvoke(); // Eski zamanlayýcýyý iptal et
        _rigidbody.AddForce(transform.forward * force, ForceMode.VelocityChange);
        Invoke(nameof(ReturnToPool), duration); // Süre dolunca yok et
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)// Silah alma alaný gibi trigger'larý yoksay
        {
            return;
        }

        if (other.GetComponent<CharacterController>() != null)
        {
            return;
        }

        ReturnToPool(); // Duvar/zemin gibi bir þeye çarptý
    }

    // Mermiyi kapatýp havuza geri koyar (karakter de hasar verince bunu çaðýrýr)
    [Server]
    public void ReturnToPool()
    {
        if (_isReturnedToPool)
        {
            return;
        }
        _isReturnedToPool = true;

        CancelInvoke();
        shooterNetId = 0;

        NetworkServer.UnSpawn(gameObject);

        if (BulletPool.Instance != null)
        {
            BulletPool.Instance.ReturnBullet(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}