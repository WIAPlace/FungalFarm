using HSM;
using UnityEngine;

public class PlayerEffectDriver : MonoBehaviour
{
    public void CreateParticles(PlayerContext ctx, ParticleSystem system)
    {
        ParticleSystem newEffect = Instantiate(system, ctx.cinCam.transform.position, ctx.cinCam.transform.rotation);

        // 2. Play the particle system immediately (if "Play On Awake" is disabled)
        newEffect.Play();
    }
}
