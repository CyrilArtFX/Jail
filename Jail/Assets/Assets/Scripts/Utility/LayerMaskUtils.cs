﻿using UnityEditor;
using UnityEngine;

namespace Jail.Utility
{
    public static class LayerMaskUtils
    {
        public static bool HasLayer(LayerMask mask, int layer)
        {
            return (mask.value & (1 << layer)) > 0;
        }
    }
}