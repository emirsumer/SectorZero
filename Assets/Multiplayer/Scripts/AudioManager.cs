using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource gameMusic;
    [SerializeField] private AudioSource shotSFX;
    [SerializeField] private AudioSource reloadSFX;
    [SerializeField] private AudioSource emptySFX;
    [SerializeField] private AudioSource pickupSFX;
    [SerializeField] private AudioSource dropSFX;
    [SerializeField] private AudioSource rightFootstepSFX;
    [SerializeField] private AudioSource leftFootstepSFX;
    [SerializeField] private AudioSource hitSFX;
    [SerializeField] private AudioSource deathSFX;

    public static AudioManager Instance;
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
        PlayMusic();
    }
    public void PlayMusic()
    {
        if (gameMusic == null)
        {
            return;
        }

        gameMusic.loop = true;

        if (!gameMusic.isPlaying)
        {
            gameMusic.Play();
        }
    }

    public void StopMusic()
    {
        if (gameMusic != null)
        {
            gameMusic.Stop();
        }
    }
    public void PlayShot(Vector3 position)
    {
        PlaySoundAt(shotSFX, position);
    }

    public void PlayReload(Vector3 position)
    {
        PlaySoundAt(reloadSFX, position);
    }

    public void PlayEmptyClick(Vector3 position)
    {
        PlaySoundAt(emptySFX, position);
    }

    public void PlayPickup(Vector3 position)
    {
        PlaySoundAt(pickupSFX, position);
    }

    public void PlayDrop(Vector3 position)
    {
        PlaySoundAt(dropSFX, position);
    }

    public void PlayRightFootstep(Vector3 position)
    {
        PlaySoundAt(rightFootstepSFX, position);
    }
    public void PlayLeftFootstep(Vector3 position)
    {
        PlaySoundAt(leftFootstepSFX, position);
    }
    public void PlayHit(Vector3 position)
    {
        PlaySoundAt(hitSFX, position);
    }

    public void PlayDeath(Vector3 position)
    {
        PlaySoundAt(deathSFX, position);
    }
    private void PlaySoundAt(AudioSource source, Vector3 position)
    {
        if (source == null || source.clip == null)
        {
            return;
        }
        source.transform.position = position; //Sesi olayýn olduðu yere taþý
        source.PlayOneShot(source.clip);
    }
}