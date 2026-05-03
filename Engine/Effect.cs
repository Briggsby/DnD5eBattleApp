using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Microsoft.Xna.Framework;

namespace BugsbyEngine;

/// <summary>
/// Represents an Effect that can be triggered, and have responses that either trigger before and modify the Effect, or triggered after and respond
/// to the effect.
/// Effects differ from Events in that Effects can be modified or prevented before resolving.
/// </summary>


public abstract class Effect
{
    public List<IEffectListener<Effect>> PreEffectListeners { get; set; }
    public string EffectType { get; set; } = String.Empty;

    public virtual void FinishPreEffect(IEffectListener<Effect> listener)
    {
        PreEffectListeners.Remove(listener);
        if (PreEffectListeners.Count == 0)
        {
            EnactEffect();
        }
    }
    public virtual void EnactEffect()
    {
        EffectManager.FinishEffect(this);
    }
}

public interface IEffectListener<TEffect> where TEffect : Effect
{
    void OnPreEffect(TEffect e);
    void OnPostEffect(TEffect e);
}

public class EffectManager
{
    public static Dictionary<(Type, string), List<IEffectListener<Effect>>> preEffectListeners = [];
    public static Dictionary<(Type, string), List<IEffectListener<Effect>>> postEffectListeners = [];
    public static List<Effect> ongoingEffects = new List<Effect>();


    public static void AddPreEffectListener<T>(IEffectListener<T> listener, string EffectType = "") where T : Effect
    {
        if (!preEffectListeners.ContainsKey((typeof(T), EffectType)))
        {
            preEffectListeners[(typeof(T), EffectType)] = new List<IEffectListener<Effect>>();
        }
        preEffectListeners[(typeof(T), EffectType)].Add((IEffectListener<Effect>)listener);
    }

    public static void AddPostEffectListener<T>(IEffectListener<T> listener, string EffectType = "") where T : Effect
    {
        if (!postEffectListeners.ContainsKey((typeof(T), EffectType)))
        {
            postEffectListeners[(typeof(T), EffectType)] = new List<IEffectListener<Effect>>();
        }
        postEffectListeners[(typeof(T), EffectType)].Add((IEffectListener<Effect>)listener);
    }

    public static void TriggerEffect<T>(T e) where T : Effect
    {
        ongoingEffects.Add(e);
        e.PreEffectListeners = new List<IEffectListener<Effect>>();

        if (preEffectListeners.ContainsKey((typeof(T), e.EffectType)))
        {
            foreach (var listener in preEffectListeners[(typeof(T), e.EffectType)])
            {
                e.PreEffectListeners.Add(listener);
                listener.OnPreEffect(e);
            }
        }
    }

    public static void FinishEffect<T>(T e) where T : Effect
    {
        if (postEffectListeners.ContainsKey((typeof(T), e.EffectType)))
        {
            foreach (var listener in postEffectListeners[(typeof(T), e.EffectType)])
            {
                listener.OnPostEffect(e);
            }
        }
        ongoingEffects.Remove(e);
    }
}
