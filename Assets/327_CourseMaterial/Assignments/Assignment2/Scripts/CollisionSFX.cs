using UnityEngine;
using System.Collections.Generic;
using Oculus.Interaction;
public class CollisionSFX : MonoBehaviour
{
    [Header("SFX Settings")]
    public float volumeMultiplier = 0.1f;
    public float volumeFloor = 0.1f;
    public float pitchMultiplier = 0.1f;
    public float pitchCeiling = 2f;
    public List<Boid> boids;
    public List<AudioSource> audioSources;
    public int currentAudioSourceIndex = 0;
    public int period = 50;
    public int collisions;
    [Header("Renderer Settings")]
    public Renderer renderer;
    public LeftController leftController;
    public float zoomMultiplier = 0.1f;
    [Header("Sine Wave Settings")]
    public float frequencyFloor = 440f;
    public float frequencyCeil = 2093f;
    [Range(0f, 1f)] public float amplitude = 0.25f;
    float phase;
    int count;
    float sampleRate;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        count = period;
        collisions = 0;
        sampleRate = AudioSettings.outputSampleRate;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (Boid boid in boids)
        {
            collisions += boid.collisions;
        }

        if (count > 0)
        {
            count--;
            return;
        }
        count = period;
        
        if (collisions == 0)
        {
            return;
        }

        if (renderer != null)
        {
            //renderer.material.SetFloat("_Collisions", collisions);
            if(leftController != null)
            {
                renderer.material.SetFloat("_Zoom", (leftController.thumbDot * zoomMultiplier) + 1);
            }
        }

        audioSources[currentAudioSourceIndex].volume = volumeFloor + collisions * volumeMultiplier;
        audioSources[currentAudioSourceIndex].pitch = pitchCeiling -  collisions * pitchMultiplier;
        audioSources[currentAudioSourceIndex].PlayOneShot(audioSources[currentAudioSourceIndex].clip);
        currentAudioSourceIndex = (currentAudioSourceIndex + 1) % audioSources.Count;
        collisions = 0;
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (leftController == null || !leftController.tracking) return;
        float frequency = frequencyFloor + (leftController.thumbDot * (frequencyCeil - frequencyFloor));
        float increment = 2.0f * Mathf.PI * frequency / sampleRate;

        for (int i = 0; i < data.Length; i += channels)
        {
            float s = amplitude * (float)Mathf.Sin(phase);
            for (int c = 0; c < channels; c++)
                data[i + c] += s;

            phase += increment;
            if (phase >= 2.0f * Mathf.PI) phase -= 2.0f * Mathf.PI;
        }
    }
}
