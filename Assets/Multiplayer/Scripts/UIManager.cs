using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image healthBarFill;
    [SerializeField] private GameObject crosshair; 
    [SerializeField] private TMP_Text ammoText;

    private int _lastCurrentAmmo = -1;
    private int _lastReserveAmmo = -1;
    private bool _lastReloading;

    public static UIManager Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        Instance = this;
    }
    public void SetHealth(float fillAmount)
    {
        if (healthBarFill == null)
        {
            return;
        }

        healthBarFill.fillAmount = Mathf.Clamp01(fillAmount);
    }
    public void SetCrosshairActive(bool isActive)
    {
        if (crosshair != null)
        {
            crosshair.SetActive(isActive);
        }
    }
    public void UpdateCrosshairPosition(Vector3 screenPos)
    {
        if (crosshair != null)
        {
            crosshair.transform.position = screenPos; //ekranda verilen noktaya taþýr
        }
    }  
    public void SetAmmoActive(bool isActive)
    {
        if (ammoText == null)
        {
            return;
        }

        ammoText.gameObject.SetActive(isActive);

        _lastCurrentAmmo = -1;
    }

    // Mermi yazýsýný günceller (CharacterController her karede çaðýrýr)
    public void UpdateAmmoText(int currentAmmo, int reserveAmmo, bool isReloading)
    {
        if (ammoText == null)
        {
            return;
        }

        // Hiçbir þey deðiþmediyse boþuna yazý yazma
        if (currentAmmo == _lastCurrentAmmo && reserveAmmo == _lastReserveAmmo && isReloading == _lastReloading)
        {
            return;
        }

        _lastCurrentAmmo = currentAmmo;
        _lastReserveAmmo = reserveAmmo;
        _lastReloading = isReloading;

        if (isReloading)
        {
            ammoText.text = "Reloading";
        }
        else
        {
            ammoText.text = currentAmmo + " / " + reserveAmmo;
        }

        // Þarjör boþsa kýrmýzý, doluysa beyaz
        ammoText.color = currentAmmo == 0 ? Color.red : Color.white;
    }
}
