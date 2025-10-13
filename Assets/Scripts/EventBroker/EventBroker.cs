using System;
using System.Collections.Generic;

public static class EventBroker
{
    private static readonly Dictionary<string, List<Action>> actionList = new();
    private static readonly Dictionary<string, List<SubscriptionWrapper>> actionTList = new();

    private class SubscriptionWrapper
    {
        public Delegate OriginalAction { get; }
        public Action<object> WrappedAction { get; }

        public SubscriptionWrapper(Delegate originalAction, Action<object> wrappedAction)
        {
            OriginalAction = originalAction;
            WrappedAction = wrappedAction;
        }
    }

    public static void Subscribe(string key, Action action)
    {
        if (!actionList.ContainsKey(key))
            actionList[key] = new List<Action>();

        actionList[key].Add(action);
    }

    public static void Subscribe<T>(string key, Action<T> action)
    {
        if (!actionTList.ContainsKey(key))
            actionTList[key] = new List<SubscriptionWrapper>();

        Action<object> wrapped = obj => action((T)obj);
        var wrapper = new SubscriptionWrapper(action, wrapped);
        actionTList[key].Add(wrapper);
    }

    public static void UnSubscribe(string key, Action action)
    {
        if (actionList.ContainsKey(key))
        {
            actionList[key].Remove(action);
            if (actionList[key].Count == 0)
                actionList.Remove(key);
        }
    }

    public static void UnSubscribe<T>(string key, Action<T> action)
    {
        if (actionTList.ContainsKey(key))
        {
            actionTList[key].RemoveAll(w => w.OriginalAction.Equals(action));
            if (actionTList[key].Count == 0)
                actionTList.Remove(key);
        }
    }

    public static void Publish(string key)
    {
        if (!actionList.ContainsKey(key))
            return;

        foreach (var action in actionList[key])
        {
            action();
        }
    }

    public static void Publish<T>(string key, T eventMessage)
    {
        if (actionTList.ContainsKey(key))
        {
            foreach (var wrapper in actionTList[key])
            {
                wrapper.WrappedAction(eventMessage);
            }
        }
    }
}
/* 
    EventBroker.Subscribe(Events.ON_FIRST_TOUCH, OnFirstTouch);
    EventBroker.Subscribe<(int count, bool isTrue)>(Events.ON_SHOOT, OnShoot);
    EventBroker.Publish(Events.ON_LEVEL_SUCCESS);


    EventBroker.UnSubscribe(Events.ON_FIRST_TOUCH, OnFirstTouch);
    EventBroker.UnSubscribe<(int count, bool isTrue)>(Events.ON_SHOOT, OnShoot);
 */