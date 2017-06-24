using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationField : Singleton<NavigationField> {

    public enum LayerType { Walkable }

    private SortedDictionary<LayerType, float[]> _navField = new SortedDictionary<LayerType, float[]>();
    public SortedDictionary<LayerType, float[]> NavField
    {
        get { return _navField; }
    }

    public AnimationCurve walkableCurve;

    new private void Awake()
    {
        base.Awake();

        foreach (var type in Enum.GetValues(typeof(LayerType)))
            NavField.Add((LayerType)type, new float[GlobalMapSettings.Instance.Width * GlobalMapSettings.Instance.Height]);
    }
}