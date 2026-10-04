using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int preloadCount = 20;

    private readonly Queue<GameObject> _freeBullets = new Queue<GameObject>(); //boþta bekleyen mermi

    private readonly HashSet<GameObject> _inPool = new HashSet<GameObject>(); //merminin zaten havuzda olup olmadýðýný hýzlýca kontrol etmek ve çift eklemeyi önlemek için

    public static BulletPool Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        Instance = this;
    }
    private void Start()
    {
        for (int i = 0; i < preloadCount; i++)
        {
            ReturnBullet(CreateBullet());
        }

        // Client'ta mermiyi Mirror deðil bu havuz yaratsýn ve yok etsin
        NetworkClient.RegisterPrefab(bulletPrefab, ClientSpawnBullet, ClientUnspawnBullet);
    }

    public GameObject GetBullet(Vector3 position, Quaternion rotation)
    {
        GameObject bullet;
        if (_freeBullets.Count > 0)
        {
            bullet = _freeBullets.Dequeue(); // Havuzdaki sýradaki mermiyi çek
        }
        else
        {
            bullet = CreateBullet(); // Havuz boþsa sýfýrdan yeni mermi klonla
        }
        _inPool.Remove(bullet);

        bullet.transform.SetPositionAndRotation(position, rotation); // Doðacaðý yere koy
        bullet.SetActive(true);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null && !rb.isKinematic)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        return bullet;
    }

    public void ReturnBullet(GameObject bullet)
    {
        
        if (!_inPool.Add(bullet))// Zaten havuzdaysa tekrar ekleme
        {
            return;
        }

        bullet.SetActive(false);
        _freeBullets.Enqueue(bullet);
    }

    private GameObject CreateBullet()
    {
        return Instantiate(bulletPrefab);
    }

    // Client'ta: server mermi yaratýnca Mirror bunu çaðýrýr
    private GameObject ClientSpawnBullet(SpawnMessage message)
    {
        return GetBullet(message.position, message.rotation);
    }

    // Client'ta: server mermiyi yok edince Mirror bunu çaðýrýr
    private void ClientUnspawnBullet(GameObject spawned)
    {
        ReturnBullet(spawned);
    }
}