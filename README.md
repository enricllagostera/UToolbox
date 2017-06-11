# UToolbox

This project hosts a series of C# scripts and other facilities to help out in Unity projects. Also included are unit tests for each feature. I've started this project to speed up development of game prototypes and to make sure I re-use my own code in a more structured way, trying to keep it bug free and more carefully written.

This is not meant to be a ready-made monolithic tool pack, just a gathering point for a series of things that can each be included in your own project as needed. Each feature uses very few scripts and is made to have small dependencies and coupling. As I go along, I'll try to pack each feature in a separate `unitypackage` for ease of use.

## What's in the box?

### In development

- A generic `ConditionalWeightedBag` (inspired by [Reigns' procedural card system](https://www.youtube.com/watch?v=tDdtbh-oUTU&t=2s)) system to use with any sort of game element, which allows for highly flexible and controllable random draws.

### Wish-list

- A simple `SpriteFader`, including alpha fades and dissolves controlled by a grayscale texture, to use for transitions or as a more general visual effect.
- Everybody loves working with spreadsheets, so I think a basic decorator to a CSV file parser/writer utility would be useful. Not sure on what features I would include, though.
