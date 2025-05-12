using System.Collections;
using UnityEngine;
public class SimpleNoiseFilter : INoiseFilter{
    Noise noise = new Noise();
    NoiseSettings.SimpleNoiseSettings settings;

    public SimpleNoiseFilter(NoiseSettings.SimpleNoiseSettings settings, int seed = 0){
        this.settings = settings;
        noise = new Noise(seed);
    }

    public float Evaluate(Vector3 point){
        float noiseValue = 0;
        float frequency = settings.baseRoughness;
        float amplitude = 1;

        for(int i =0; i<settings.numLayers; i++){
            float v = noise.Evaluate(point * frequency + settings.center);
            noiseValue += (v + 1) * 0.5f * amplitude;
            frequency *= settings.roughness;
            amplitude *= settings.persistence;
        }
        noiseValue = Mathf.Max(0, noiseValue - settings.minValue);

        return noiseValue * settings.strength;
    }
}