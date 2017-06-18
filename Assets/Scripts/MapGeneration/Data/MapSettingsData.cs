using UnityEngine;

[CreateAssetMenu()]
public sealed class MapSettingsData : UpdatableData
{
    public NoiseSettings noiseSetting;
    public float heightMultiplier;
    public AnimationCurve heightCurve;

    public int riverCount;
    public Vector2 riverLengthMinMax;

    public bool useFalloff;

#if UNITY_EDITOR

    protected override void OnValidate()
    {
        noiseSetting.ValidateValues();
        base.OnValidate();
    }

#endif
}
