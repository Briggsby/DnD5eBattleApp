using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Microsoft.Xna.Framework;

namespace BugsbyEngine;

/// <summary>
/// Represents an event that can be triggered, and have responses that either trigger before and modify the event, or triggered after and respond
/// to the effect.
/// </summary>


public abstract class Event
{
    public List<IEventListener<Event>> PreEventListeners { get; set; }
    public string EventType { get; set; } = String.Empty;

    public virtual void FinishPreEvent(IEventListener<Event> listener)
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

public interface IEventListener<TEvent> where TEvent : Event
{
    void OnPreEvent(TEvent e);
    void OnPostEvent(TEvent e);
}

public class EventManager
{
    public static Dictionary<(Type, string), List<IEventListener<Event>>> preEventListeners = [];
    public static Dictionary<(Type, string), List<IEventListener<Event>>> postEventListeners = [];
    public static List<Event> ongoingEvents = new List<Event>();


    public static void AddPreEventListener<T>(IEventListener<T> listener, string eventType = "") where T : Event
    {
        if (!preEventListeners.ContainsKey((typeof(T), eventType)))
        {
            preEventListeners[(typeof(T), eventType)] = new List<IEventListener<Event>>();
        }
        preEventListeners[(typeof(T), eventType)].Add((IEventListener<Event>)listener);
    }

    public static void AddPostEventListener<T>(IEventListener<T> listener, string eventType = "") where T : Event
    {
        if (!postEventListeners.ContainsKey((typeof(T), eventType)))
        {
            postEventListeners[(typeof(T), eventType)] = new List<IEventListener<Event>>();
        }
        postEventListeners[(typeof(T), eventType)].Add((IEventListener<Event>)listener);
    }

    public static void TriggerEvent<T>(T e) where T : Event
    {
        ongoingEvents.Add(e);
        e.PreEventListeners = new List<IEventListener<Event>>();

        if (preEventListeners.ContainsKey((typeof(T), e.EventType)))
        {
            foreach (var listener in preEventListeners[(typeof(T), e.EventType)])
            {
                e.PreEventListeners.Add(listener);
                listener.OnPreEvent(e);
            }
        }
    }

    public static void FinishEvent<T>(T e) where T : Event
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
