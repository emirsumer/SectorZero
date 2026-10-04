using UnityEngine;
using UnityEngine.UI;

public partial class CharacterController
{
    [SerializeField] private GameObject characterCanvas; 
    [SerializeField] private Image healthBar;            

    private Image _playerHealthBar;                     

    private void InitUI()
    {
        if (isLocalPlayer)
        {
            GameObject barObject = GameObject.Find("PlayerHealthBar");
            if (barObject != null)
            {
                _playerHealthBar = barObject.GetComponent<Image>();
            }

            if (characterCanvas != null)
            {
                characterCanvas.SetActive(false);
            }
        }

        UpdateHealthBar();
    }

    private void UpdateCanvasToCamera()
    {
        if (characterCanvas == null || !characterCanvas.activeSelf || Camera.main == null)
        {
            return;
        }
        characterCanvas.transform.LookAt(Camera.main.transform);
    }

    private void UpdateHealthBar()
    {
        float fillAmount = Mathf.Clamp01(_health / 100f);

        if (isLocalPlayer)
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.SetHealth(fillAmount);
            }
            else if (_playerHealthBar != null)
            {
                _playerHealthBar.fillAmount = fillAmount;
            }
        }
        else
        {
            if (healthBar != null)
            {
                healthBar.fillAmount = fillAmount;
            }
        }
    }
}