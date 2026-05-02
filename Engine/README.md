# Bugsby Engine

TODO:
- Make a 'value' class for stats and other values that change. It can be used as an attribute like 'public Value<int> hp'
    - Values can have Base values and current values, and whenever they change trigger a corresponding 'Event'
- Make an 'event' parent class with listeners. Each value can be associated with an event that triggers when it changes.
    - Whenever an event happens, there's a pre-event function, which gets all the listeners of that class (or parent class) of event, and enacts them
        - This passes the event itself so it can change based on the pre-events
    - And then after the event enacts it does a post-event function, to trigger post-event changes