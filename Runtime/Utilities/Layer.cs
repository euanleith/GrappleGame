using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utilities {

    public static class Layer {

        public const string DEFAULT = "Default";
        public const string TRANSPARENT_FX = "TransparentFX";
        public const string IGNORE_RAYCAST = "IgnoreRaycast";
        public const string GROUND = "Ground";
        public const string WATER = "Water";
        public const string UI = "UI";
        public const string UNGRAPPLEABLE_DEATH = "UngrapplableDeath";
        public const string GRAPPLEABLE_DEATH = "GrapplableDeath";
        public const string ENEMY = "Enemy";
        public const string TRANSITION = "Transition";
        public const string CAMERA = "Camera";
        public const string ENEMY_ATTACK = "EnemyAttack";
        public const string PLAYER = "Player";
        public const string GRAPPLEABLE_PLAYER_TRAVERSABLE = "GrappleablePlayerTraversable";
        public const string UNGRAPPLEABLE_GROUND = "UngrappleableGround";
        public const string GRAPPLE_TRAVERSABLE_DEATH = "GrappleTraversableDeath";
        public const string GRAPPLE_TRAVERSABLE = "GrapleTraversable";
        public const string ENEMY_INVULNERABLE = "EnemyInvulnerable";
        public const string UNGRAPPLEABLE_PLAYER_TRAVERSABLE = "UngrappleablePlayerTraversable";
        public const string ENEMY_TRAVERSABLE = "EnemyTraversable";
        public const string GRAPPLEABLE_PLATFORM = "GrappleablePlatform";
        public const string SWING = "Swing";
        public const string TRAVERSABLE_GROUND = "TraversableGround";

        public static bool IsDeathLayer(int layer) {
            return LayerEqualsAny(layer,
                UNGRAPPLEABLE_DEATH,
                GRAPPLEABLE_DEATH,
                GRAPPLE_TRAVERSABLE_DEATH
            );
        }

        public static int LayerToInt(string layer) {
            return LayerMask.NameToLayer(layer);
        }

        public static int[] LayersToInts(params string[] layers) {
            return layers.Select(layer => LayerToInt(layer)).ToArray();
        }

        public static string LayerToString(int layer) {
            return LayerMask.LayerToName(layer);
        }

        public static string[] LayersToStrings(params int[] layers) {
            return layers.Select(layer => LayerToString(layer)).ToArray();
        }

        public static bool LayerEquals(int layerInt, string layerStr) {
            return LayerToString(layerInt).Equals(layerStr);
        }

        public static bool LayerEqualsAny(int layerInt, params string[] layersStr) {
            foreach (string layerStr in layersStr) {
                if (LayerEquals(layerInt, layerStr)) return true;
            }
            return false;
        }

        public static bool LayerEqualsAny(string layerStr, params int[] layersInt) {
            foreach (int layerInt in layersInt) {
                if (LayerEquals(layerInt, layerStr)) return true;
            }
            return false;
        }

        public static bool LayerEqualsAny(string layerStr, params string[] layersStr) {
            return layersStr.Contains(layerStr);
        }

        public static bool LayerEqualsAny(string layerStr, List<string> layersStr) {
            return layersStr.Contains(layerStr);
        }

        public static bool LayerEqualsAny(int layerInt, params int[] layersInt) {
            return layersInt.Contains(layerInt);
        }

        public static bool LayerEqualsAny(int layerInt, List<int> layersInt) {
            return layersInt.Contains(layerInt);
        }

        public static int ToLayerMask(int layer) {
            return 1 << layer;
        }

        public static int ToLayerMask(string layer) {
            return ToLayerMask(LayerToInt(layer));
        }

        public static int ToLayerMask(params int[] layers) {
            return CombineLayerMasks(layers.Select(layer => ToLayerMask(layer)).ToArray());
        }

        public static int ToLayerMask(params string[] layers) {
            return ToLayerMask(LayersToInts(layers));
        }

        // see https://discussions.unity.com/t/check-if-layer-is-in-layermask/16007/2
        public static bool LayerMaskContains(LayerMask mask, int layer) {
            return mask == (mask | (1 << layer));
        }

        public static int NegativeLayerMask(int layerMask) {
            return ~layerMask;
        }

        public static int CombineLayerMasks(params int[] layerMasks) {
            int res = 0;
            foreach (int layerMask in layerMasks) {
                res |= layerMask;
            }
            return res;
        }

        public static int AddToLayerMask(int baseMask, params string[] layers) {
            return CombineLayerMasks(baseMask, ToLayerMask(layers));
        }

        public static int RemoveFromLayerMask(int baseMask, params int[] layers) {
            return baseMask & ~CombineLayerMasks(ToLayerMask(layers));
        }

        public static int RemoveFromLayerMask(int baseMask, params string[] layers) {
            return RemoveFromLayerMask(baseMask, LayersToInts(layers));
        }
    }
}