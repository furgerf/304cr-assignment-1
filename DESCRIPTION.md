# Overview
The application consists of three main parts: the model, the path finding logic and the view. Since not much special user input handling is required and the user input is received by means of events in the view, the "controller"-part in the traditional
MVC-layout is integrated in the view. The view displays the model and allows the user to adjust the aspects of the model (map and entity locations). Once the path finding is started, the path finding logic uses that model as a basis for the algorithms.

This document contains information about the different parts in some detail, what is working as well as some known issues and limitations.

# Model
The `Map` forms the core as it represents the world which contains the `Entity` __Player__ and the `Entity` __Target__. The `Map` contains an instance of a `Graph` which is a container for the raw data of the model, a 2-dimensional array (matrix) of `Node`s
which are connected by `Edge`s.

## Map
The `Map` class can be considered a wrapper around the actual data which is accessible via its `Graph`-property. Its main purpose is to regulate access to the map; that is offer methods that can read or write certain properties.

Furthermore, this class
offers several events that are triggered when parts of the `Map` change.

Lastly, it offers methods to save/load maps to/from text files and to _regenerate_ the map, which creates a new, random map according to some parameters.

## Graph
The `Graph` class simplifies access to the `Node`-matrix as a whole. An example of that is the property `PassibleNodeCount` which returns the number of passible nodes on the map. More importantly though, it offers three different methods for creating a `Graph` (and thus, a `Map`):
- `EmptyGraph(int width, int height)`: This creates a `Graph` with a specific width and height but leaves all `Node`s _empty_, which means they're all un-foggy street `Node`s.
- `FromData(string[] data)`: This instantiates a `Graph` with an array of strings that contain data from a previously stored `Graph`.
- `Random(int width, int height, double[] weights, double fog)`: The last and probably most interesting method creates a random `Graph` according to some parameters. The weights are used to determine how common specific kinds of `Terrain`s should be and the fog parameter is used to determine how likely it is that a tile is covered by fog.

Since it is possible to load a `Graph` from text data, it is also possible to perform the reverse action; that is to retrieve a text representation of the current `Graph`. For that purpose, the `object`'s `ToString()`-method has been overridden.

## Node

## Edge

## Entity


Interesting stuff
- multi-step removal of shitty foggy nodes
- partition into segments
- looks for alternative paths

Issues
- after unsuccessfully investigating fog, moves back to start and from there to next fog
 -> reason: would have to restart pathfinding all the time
 -> this is slightly reduced by ignoring common parts of the path
- can NOT find path when one would be possible if clear patch is found after investigating fog but that clear patch doesn't lead to the target
 -> no backtracking to last clear area
