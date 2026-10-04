using UnityEngine;

public class FootstepSFX : MonoBehaviour
{
    public void PlayFootRight()
    {
        PlayStep(isRightFoot: true);
    }

    public void PlayFootLeft()
    {
        PlayStep(isRightFoot: false);
    }
    private void PlayStep(bool isRightFoot)
    {
        if (AudioManager.Instance == null) return;

        if (isRightFoot)
        {
            AudioManager.Instance.PlayRightFootstep(transform.position);
        }
        else
        {
            AudioManager.Instance.PlayLeftFootstep(transform.position);
        }
    }
}
