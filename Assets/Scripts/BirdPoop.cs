using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdPoop : MonoBehaviour
{

    ParticleSystem ps;
    PoopSplatControl poopSplatControl;

    // Use this for initialization
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        poopSplatControl = FindObjectOfType<PoopSplatControl>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnParticleCollision(GameObject other)
    {
        if(other.name=="PoopDetector"){
            poopSplatControl.ShowSplat();
        }
    }

    private void OnParticleTrigger()
    {
        List<ParticleSystem.Particle> entered = new List<ParticleSystem.Particle>();

        ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, entered);

        if (entered.Count > 0)
        {


            for (int p = 0; p < entered.Count; p++)
            {
                
                ParticleSystem.Particle pa = entered[p];
                Splash splash = Instantiate(Resources.Load("SplashTiny", typeof(Splash)), pa.position, Quaternion.identity) as Splash;
                splash.splashSize = "SmallSoft";

                pa.remainingLifetime = 0;
                entered[p] = pa;

            }
        }
    }
}
