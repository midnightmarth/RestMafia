public static class NoiseFilterFactory{


    public static INoiseFilter CreateNoiseFilter(NoiseSettings settings, int seed = 0){
        return settings.filterType switch
        {
            NoiseSettings.FilterType.Simple => new SimpleNoiseFilter(settings.simpleNoiseSettings, seed),
            NoiseSettings.FilterType.Rigid => new RigidNoiseFilter(settings.rigidNoiseSettings, seed),
            _ => null,
        };
    }
}