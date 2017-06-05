using UnityEngine;

[CreateAssetMenu()]
public sealed class MapSettingsData : UpdatableData
{
    public NoiseSettings noiseSetting;
    public float heightMultiplier;
    public AnimationCurve heightCurve;
    public bool useFalloff;

#if UNITY_EDITOR

    protected override void OnValidate()
    {
        noiseSetting.ValidateValues();
        base.OnValidate();
    }

#endif
}
