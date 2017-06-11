# SmartBag system

## `SmartBag`

This is a bag for random draws which takes into account:

- The weight of each item.
- A **lock** interval during which an item cannot be drawn.
- A set of pre-conditions which must be present in the bag's state for an item to be drawn.

#### Data

- `items` : collection of all possible `ConditionedItem`.
- `state` : list of current `Condition`.

#### Methods

- Draw() : draws a random item, calls its `Use` method, advances the bag's timer and returns the item.
- Pick(id) : draws a specific item, calls its `Use` method, advances the bag's timer and returns the item.
- Peek() : draws a random item and returns it without using it.
- Filter(conditions) : returns all items that fit a set of conditions.
- SetCondition(condition) : updates a condition on the bag.
- GetCondition(condition) : returns a condition in the bag's state.
- SetState(condition[]) : changes the whole state of the bag.
- GetState(condition[]) : returns the bag's whole state.
- Tick() : advances the timer in all items.

## `ConditionedItem`

This is an abstract class that serves as a base for any item you might want to put in a `SmartBag`. It's mostly a container for data needed by the bag controller.

#### Data

- id
- weight
- lockTimer
- lockedInterval
- preConditions
- postConditions

#### Methods

- Use() : resets the `lockTimer` and returns the post-conditions for this item.
- Tick() : advances the `lockTimer` (`lockTimer--`).
- IsLocked() : checks is the item is in locked condition (`lockTimer > 0`).

## `Condition`

#### Static

- Parse(string) : creates a new `Condition` object from a string. The symbol `!` stands for false and the rest of the string becomes the id.
- CheckAll(Condition[] state, Condition[] query) : checks if all the given *query* and *state* conditions are compatible.
- Check(Condition current, Condition query) : checks if the given *query* and *state* are compatible.

#### Data

- id : string
- status : bool
