# UToolbox

This project hosts a series of C# scripts and other facilities to help out in Unity projects. Also included are unit tests for each feature. I've started this project to speed up development of game prototypes and to make sure I re-use my own code in a more structured way, trying to keep it bug free and more carefully written.

This is not meant to be a ready-made monolithic tool pack, just a gathering point for a series of things that can each be included in your own project as needed. Each feature uses very few scripts and is made to have small dependencies and coupling. As I go along, I'll try to pack each feature in a separate `unitypackage` for ease of use.

## What's in the box?

- A generic `SmartBag<T>` (inspired by [Reigns' procedural card system](https://www.youtube.com/watch?v=tDdtbh-oUTU&t=2s)) system to use with any sort of game logic element, which allows for highly flexible and controllable random draws. It features a lock / cooldown mechanism for used items, pre- and post- conditions and weighted random draws. It's all in the `SmartBag.cs` file.

### In development

To be defined.

### Wish-list

- A simple `SpriteFader`, including an alpha fade or dissolve mode, both controlled by a grayscale texture, to use for transitions or as a more general visual effect.
- Everybody loves working with spreadsheets, so I think a **basic decorator to a CSV file parser/writer utility** would be useful. Not sure on what features I would include, though.

## General implementation notes

1. Empty objects will mostly be represented by `null` when possible. Check for it in case of errors.
1. Errors will be logged using Unity's `Debug.LogError` or `Debug.LogWarning` in case it should not be too noisy, as in search methods.
1. Most inputs will be treated in public API methods. Private methods will assume inputs are safe.
