using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [Header("Settings")]
    private Vector3 targetPosition;
    public float rotationSpeed = 2f;

    [Header("Laser Settings")]
    public LineRenderer laserRenderer;
    public float laserMaxDistance = 20f;

    [Header("Collision Settings")]
    public LayerMask collisionLayers;

    [Header("Particle Settings")]
    public ParticleSystem activationParticles;
    public ParticleSystem collisionParticles;

    [Header("Audio Settings")]
    private bool isBeamSoundPlaying = false;

    private CapsuleCollider laserCollider;
    private Vector3 currentLaserDirection;
    private Vector3 particleStartPosition;
    private float currentLaserOpacity = 0f;

    private void Start()
    {
        laserRenderer.enabled = false;
        laserCollider = gameObject.AddComponent<CapsuleCollider>();
        laserCollider.isTrigger = true;
        laserCollider.direction = 2;
        laserCollider.enabled = false;

        particleStartPosition = activationParticles.transform.position;

        if (activationParticles != null)
            activationParticles.Stop();

        if (collisionParticles != null)
            collisionParticles.Stop();

        StartCoroutine(LaserCycle());
    }

    private void Update()
    {
        if (laserRenderer.enabled)
        {
            FireLaser();
            // Start playing beam sound if not already playing and conditions are met
            if (!isBeamSoundPlaying && laserCollider.enabled && currentLaserOpacity >= 1f)
            {
                if (SFXManager.Instance != null)
                {
                    SFXManager.Instance.PlaySpecificEffect("Laser", "Beam", 0.5f);
                    isBeamSoundPlaying = true;
                }
            }
            // Stop beam sound if conditions are not met
            else if (isBeamSoundPlaying && (!laserCollider.enabled || currentLaserOpacity < 1f))
            {
                if (SFXManager.Instance != null)
                {
                    SFXManager.Instance.StopEffect("Laser", "Beam");
                    isBeamSoundPlaying = false;
                }
            }
        }
        else if (!laserRenderer.enabled && isBeamSoundPlaying)
        {
            // Stop beam sound when laser is disabled
            if (SFXManager.Instance != null)
            {
                SFXManager.Instance.StopEffect("Laser", "Beam");
                isBeamSoundPlaying = false;
            }
        }
    }

    public void RestartLaser()
    {
        if (laserRenderer != null)
        {
            laserRenderer.enabled = false;
        }

        if (laserCollider != null)
        {
            laserCollider.enabled = false;
        }

        if (activationParticles != null)
        {
            activationParticles.Stop();
        }

        if (collisionParticles != null)
        {
            collisionParticles.Stop();
        }

        // Stop beam sound if it's playing
        if (isBeamSoundPlaying)
        {
            if (SFXManager.Instance != null)
            {
                SFXManager.Instance.StopEffect("Laser", "Beam");
                isBeamSoundPlaying = false;
            }
        }

        StopAllCoroutines();
        StartCoroutine(LaserCycle());
    }

    private void FireLaser()
    {
        if (laserRenderer == null || laserCollider == null) return;

        Vector3 laserStart = transform.position;
        RaycastHit hit;
        Vector3 laserEnd;

        if (Physics.Raycast(laserStart, currentLaserDirection, out hit, laserMaxDistance, collisionLayers))
        {
            laserEnd = hit.point;

            if (currentLaserOpacity == 1f && collisionParticles != null)
            {
                collisionParticles.transform.position = hit.point;
                if (!collisionParticles.isPlaying)
                {
                    collisionParticles.Play();
                }
            }
        }
        else
        {
            laserEnd = laserStart + currentLaserDirection * laserMaxDistance;

            if (collisionParticles != null && collisionParticles.isPlaying)
                collisionParticles.Stop();
        }

        if (currentLaserOpacity == 1f && activationParticles != null)
        {
            activationParticles.transform.position = particleStartPosition;
            if (!activationParticles.isPlaying)
                activationParticles.Play();
        }

        laserRenderer.SetPosition(0, laserStart);
        laserRenderer.SetPosition(1, laserEnd);

        UpdateLaserCollider(laserStart, laserEnd);
    }

    private void UpdateLaserCollider(Vector3 start, Vector3 end)
    {
        Vector3 center = (start + end) / 2;
        float length = Vector3.Distance(start, end);

        laserCollider.center = transform.InverseTransformPoint(center);
        laserCollider.height = length;
        laserCollider.radius = 0.1f;
        laserCollider.transform.rotation = Quaternion.LookRotation(end - start);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.ActivateRespawnEffects();
            }
        }
    }

    private IEnumerator LaserCycle()
    {
        while (true)
        {
            laserRenderer.enabled = false;
            laserCollider.enabled = false;
            SetLaserOpacity(0f);

            if (activationParticles != null)
                activationParticles.Stop();

            if (collisionParticles != null)
                collisionParticles.Stop();

            if (SFXManager.Instance != null)
            {
                SFXManager.Instance.PlaySpecificEffect("Laser", "Deactivate");
            }

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                targetPosition = player.transform.position;
                currentLaserDirection = (targetPosition - transform.position).normalized;
                transform.rotation = Quaternion.LookRotation(currentLaserDirection);
            }

            yield return new WaitForSeconds(1f);

            if (activationParticles != null)
            {
                activationParticles.transform.position = particleStartPosition;
                activationParticles.Play();
            }

            if (SFXManager.Instance != null)
            {
                SFXManager.Instance.PlaySpecificEffect("Laser", "Charge");
            }

            laserRenderer.enabled = true;
            SetLaserOpacity(0.2f);
            yield return new WaitForSeconds(1f);

            SetLaserOpacity(1f);
            laserCollider.enabled = true;

            yield return new WaitForSeconds(2f);

            if (activationParticles != null)
                activationParticles.Stop();
        }
    }

    private void SetLaserOpacity(float opacity)
    {
        currentLaserOpacity = opacity;

        if (laserRenderer != null)
        {
            Gradient gradient = laserRenderer.colorGradient;
            GradientColorKey[] colorKeys = gradient.colorKeys;
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[colorKeys.Length];

            for (int i = 0; i < colorKeys.Length; i++)
            {
                alphaKeys[i] = new GradientAlphaKey(opacity, colorKeys[i].time);
            }

            gradient.alphaKeys = alphaKeys;
            laserRenderer.colorGradient = gradient;
        }
    }
}
