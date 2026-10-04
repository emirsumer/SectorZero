using UnityEngine;

public partial class CharacterController
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform spineBone;
    [SerializeField] private Transform aimBone;

    private void AnimIsRunning(bool isRunning)
    {
        animator.SetBool("IsRunning", isRunning);
    }

    private void AnimMovement(float forward, float right)
    {
        animator.SetFloat("Forward", forward);
        animator.SetFloat("Right", right);
    }

    private void AnimWeapon()
    {
        animator.SetBool("HasWeapon", _attachedWeapon != null);
        animator.SetLayerWeight(1, _attachedWeapon != null ? 1 : 0);
    }
    private void AnimDeath()
    {
        animator.SetLayerWeight(1, 0);
        animator.SetTrigger("Death");
    }
    //Ölüm animasyonundan çýkýp karakteri baþtaki (ayakta) haline döndürür
    private void AnimRespawn()
    {
        animator.Rebind();      // Animator'ü sýfýrla, baþlangýç durumuna dön
        animator.Update(0f);    // Deðiþikliði hemen uygula
        AnimWeapon();           // Silah durumunu tekrar ayarla (silahsýz, 1. katman kapalý)
    }
    private void LockSpine()
    {
        if (_attachedWeapon == null || _isDead)
        {
            return;
        }

        Quaternion targetRot = Quaternion.LookRotation(transform.forward, transform.up);
        
        spineBone.rotation = targetRot * Quaternion.Euler(camParent.transform.rotation.eulerAngles.x,0,0);
        aimBone.rotation = spineBone.rotation * Quaternion.Euler(0, 40, 0);
    }
}
