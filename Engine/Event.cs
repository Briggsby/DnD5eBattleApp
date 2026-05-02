using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Microsoft.Xna.Framework;

namespace BugsbyEngine;

/// <summary>
/// Represents an event that can be triggered, and have responses that either trigger before and modify the event, or triggered after and respond
/// to the effect.
/// </summary>


public abstract class IEvent
{
    public List<IEventListener<IEvent>> PreEventListeners { get; set; }
    public string EventType { get; set; } = String.Empty;

    public virtual void FinishPreEvent(IEventListener<IEvent> listener)
    {
        PreEventListeners.Remove(listener);
        if (PreEventListeners.Count == 0)
        {
            EnactEvent();
        }
    }
    public virtual void EnactEvent()
    {
        EventManager.FinishEvent(this);
    }
}

public interface IEventListener<TEvent> where TEvent : IEvent
{
    void OnPreEvent(TEvent e);
    void OnPostEvent(TEvent e);
}

public class EventManager
{
    public static Dictionary<(Type, string), List<IEventListener<IEvent>>> preEventListeners = [];
    public static Dictionary<(Type, string), List<IEventListener<IEvent>>> postEventListeners = [];
    public static List<IEvent> ongoingEvents = new List<IEvent>();


    public static void AddPreEventListener<T>(IEventListener<T> listener, string eventType = "") where T : IEvent
    {
        if (!preEventListeners.ContainsKey((typeof(T), eventType)))
        {
            preEventListeners[(typeof(T), eventType)] = new List<IEventListener<IEvent>>();
        }
        preEventListeners[(typeof(T), eventType)].Add((IEventListener<IEvent>)listener);
    }

    public static void AddPostEventListener<T>(IEventListener<T> listener, string eventType = "") where T : IEvent
    {
        if (!postEventListeners.ContainsKey((typeof(T), eventType)))
        {
            postEventListeners[(typeof(T), eventType)] = new List<IEventListener<IEvent>>();
        }
        postEventListeners[(typeof(T), eventType)].Add((IEventListener<IEvent>)listener);
    }

    public static void TriggerEvent<T>(T e) where T : IEvent
    {
        ongoingEvents.Add(e);
        e.PreEventListeners = new List<IEventListener<IEvent>>();

        if (preEventListeners.ContainsKey((typeof(T), e.EventType)))
        {
            foreach (var listener in preEventListeners[(typeof(T), e.EventType)])
            {
                e.PreEventListeners.Add(listener);
                listener.OnPreEvent(e);
            }
        }
    }

    public static void FinishEvent<T>(T e) where T : IEvent
    {
        if (postEventListeners.ContainsKey((typeof(T), e.EventType)))
        {
            foreach (var listener in postEventListeners[(typeof(T), e.EventType)])
            {
                listener.OnPostEvent(e);
            }
        }
        ongoingEvents.Remove(e);
    }
}
