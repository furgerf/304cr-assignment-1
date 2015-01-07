# Overview
The application consists of three main parts: the model, the path finding logic and the view. Since not much special user input handling is required and the user input is received by means of events in the view, the "controller"-part in the traditional
MVC-layout is integrated in the view. The view displays the model and allows the user to adjust the aspects of the model (map and entity locations). Once the path finding is started, the path finding logic uses that model as a basis for the algorithms.

This document contains information about the different parts in some detail, what is working as well as some known issues and limitations.


# Components
## Model
The `Map` forms the core as it represents the world which contains the `Entity` __Player__ and the `Entity` __Target__. The `Map` contains an instance of a `Graph` which is a container for the raw data of the model, a 2-dimensional array (matrix) of `Node`s
which are connected by `Edge`s.

### Map
The `Map` class can be considered a wrapper around the actual data which is accessible via its `Graph`-property. Its main purpose is to regulate access to the map; that is offer methods that can read or write certain properties.

Furthermore, this class
offers several events that are triggered when parts of the `Map` change.

Lastly, it offers methods to save/load maps to/from text files and to _regenerate_ the map, which creates a new, random map according to some parameters.

### Graph
The `Graph` class simplifies access to the `Node`-matrix as a whole. An example of that is the property `PassibleNodeCount` which returns the number of passible nodes on the map. More importantly though, it offers three different methods for creating a `Graph` (and thus, a `Map`):
- `EmptyGraph(int width, int height)`: This creates a `Graph` with a specific width and height but leaves all `Node`s _empty_, which means they're all un-foggy street `Node`s.
- `FromData(string[] data)`: This instantiates a `Graph` with an array of strings that contain data from a previously stored `Graph`.
- `Random(int width, int height, double[] weights, double fog)`: The last and probably most interesting method creates a random `Graph` according to some parameters. The weights are used to determine how common specific kinds of `Terrain`s should be and the fog parameter is used to determine how likely it is that a tile is covered by fog.

Since it is possible to load a `Graph` from text data, it is also possible to perform the reverse action; that is to retrieve a text representation of the current `Graph`. For that purpose, the `object`'s `ToString()`-method has been overridden.

### Node
Finally, the smallest building block of the model is the `Node`. It contains information about the cost for moving to the tile (with a 32-bit integer maxvalue denoting an impassible `Node`) which is determined by the `Terrain` of the `Node`. Also, it has a
reference to any `Entity` that is currently on the `Node`. Lastly, each `Node` contains a flag that defines whether the `Node` is foggy or whether it is known to the player.

`Node`s are arranged in the `Graph` in a matrix which means that each `Node` has an identifying (x/y)-coordinate and that each has up to four neighbors.

### Edge
Adjacent `Node`s are connected via `Edge`s. The only way to instantiate an `Edge` is with two `Node`s in order to help prevent reference irregularities. The `Edge` itself doesn't contain additional data and is merely used to connect `Node`s.

### Entity
A `Entity` is a special object that can be placed on the `Map`. Each `Entity` has a type and each type of `Entity` only has one instance. Currently, only the __Player__ and __Target__ `Entity` exist.


## View
Only one form has been used, the `MainForm`. It has four main components: three custom `UserControl`s `AlgorithmSettings`, `MapSettings`, and `Status` and a large `Panel` where the map is drawn on.

The three custom `UserControl`s can be hidden with the buttons on the toolbar on the left-hand side of the Form, which makes usage of the Program on screens with smaller resolution easier. Each component of the main UI as well as its usage is explained
below.

### AlgorithmSettings
The user can adjust settings related to the path finding algorithm as well as how fog exploration should be handled. Currently, these options are available:

__Path finding algorithm__
- Dijkstra: Use the Dijkstra-Algorithm for path finding
- AStar: Use the A\*-Algorithm for path finding

__Fog exploration method__
- MinCost: When in fog, always move to one of the adjacent `Node`s with the lowest cost
- MinDistanceToTarget: When in fog, always move to one of the `Node`s which has the lowest manhatten distance to the __Target__
- MinCostPlusDistanceToTarget: When in fog, always move to one of the `Node`s where the sum of the `Node`'s cost and the manhatten distance to the __Target__ is lowest

__Foggy node selection method__
- ClosestToPlayer: Move to one of the `Node`s where the path to the adjacent clear `Node` is minimal
- ClosestToTarget: Move to one of the `Node`s that has the lowest manhatten distance to the __Target__
- MinClosestToPlayerPlustTarget: Move to one of the `Node`s where the sum of the path to the adjacent clear `Node` and the manhatten distance to the __Target__ is lowest

Apart from changing algorithm-related settings, the path finding can be started, restarted, and reset here. After a path finding calculation has been executed, the different steps of the process can be displayed on the map by navigating through the steps
with the buttons ("First", "Prev.", "Next", "Last") or by scrolling the mouse wheel. Holding down the _control_ key while scrolling moves 5 steps in either direction, holding down the _shift_ key moves 10 steps and holding down both the _control_ and
_shift_ key while scrolling moves 50 steps.

Besides drawing a visual representation of the step on the map, further information can be obtained like a comment on what happens during the step, how many `Node`s have been explored up to this point and how much the cost of moving up to this point has
been.

### MapSettings
This component lets the user change the size of the map and the CellSize - the latter is merely for visual convenience. Apart from merely changing the dimensions of the map, the user can save the current map, load a previously saved map or create a new
random map with the current dimensions by _regenerating_ it.

In order to have some control about the generated map, weights can be used to increase or decrease the occurrence of different terrain types and the likelyhood that any give tile is covered in fog.

### Status
Contrary to the other two components, this one is merely informative and displays some numbers about the map and the entities. Since the individual elements are fairly self-explanatory I will not elaborate on them.

### Map
The visual representation of the `Map`. The different terrains are displayed with icons and foggy tiles have a transparent gray overlay.

In order to modify the map, one or more `Node`s have to be selected by clicking or clicking-and-dragging on the map. The context menu that opens after a right mouse click gives the option to change the terrain or to set, clear or
toggle fog on the selected `Node`s.

If only a single `Node` has been selected, it is also possible to move the __Player__ or __Target__ `Entity` to the selected `Node`.


## Path Finding




# Sequence of Events during Path Finding
![alt text](https://github.com/mystyfly/304cr-assignment-1/blob/master/pathfinding.png "Path finding workflow")

# Examples of Path Finding


# Known Issues/Limitations


Interesting stuff
- multi-step removal of shitty foggy nodes
- partition into segments
- looks for alternative paths
- mention console output

Issues
- after unsuccessfully investigating fog, moves back to start and from there to next fog
 -> reason: would have to restart pathfinding all the time
 -> this is slightly reduced by ignoring common parts of the path
- can NOT find path when one would be possible if clear patch is found after investigating fog but that clear patch doesn't lead to the target
 -> no backtracking to last clear area
