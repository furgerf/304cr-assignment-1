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

__Path find algorithm__
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

### Overview
The most interesting component clearly is the path finding logic. Instances of the path find algorithm and the fog explore algorithms are stored in dictionaries in `Utils` where they can be accessed with the algorithm's enum member.

Path finding is started by calling the `AbstractPathFindAlgorithm`s static `FindPath(PathFindName name, Node playerNode, Node targetNode, FogMethod fogMethod, FogExploreName fogExploreName)` method. The `name` parameter is used to obtain the path finding
instance from the dictionary in `Utils` as mentionned above. `fogMethod` and `fogExploreName` are two further enum members that control by which criteria the foggy node to explore should be selected and by which criteria a neighboring `Node` should be
selected when exploring fog.

More details about how the path finding works is explained below.

Path finding algorithms have to implement a few methods that carry out algorithm-specific tasks such as updating the cost of a `Node` or obtaining the cost of a `Node`. Most importantly, Algorithms must implement `FindShortestPath(Node playerNode, Node targetNode)` which kicks off the concrete path finding.

### A\* Algorithm
`AStarAlgorithm` is such an implementation. It has three fields to store all relevant data: two lists that hold the currently open `Node`s and those `Node`s that have already been processed and a dictionary that assigns two integer values to each `Node` -
one that is the cost _g_ to the `Node` and the other that is the heuristic distance to the target _h_.

When looking for the shortest path, the first `Node` in the list of open `Node`s is being processed until the __Target__ is found or the list is empty (in which case the search returns unsuccessfully). It is always the first `Node` in the list that should
be processed since the list is always ordered according to the _f_-value which is _f = g+h_.

Processing a `Node` moves it from the list of open `Node`s to the list of closed `Node`s. The neighbors are examined and possibly added to a list:
- if the neighbor is passible and foggy and not yet in the `AbstractAlgorithm`'s list of foggy `Node`s, it is added to that list
- if the neighbor is passible and not foggy and is not yet in the list of open `Node`s, it is added to that list


## Fog Exploration

### Overview
Fog exploration is started by calling `AbstractFogExploreAlgorithm`'s static `ExploreFog(...)` method. Analogously to `AbstractPathFindAlgorithm`, it uses an enum member to start fog exploration with a specific implementation that is contained in a
dictionary in `Utils`.

Merely one method is left to implement for concrete algorithms to explore fog: `GetMetric(Node candidate, Func<Node, int> getDistanceToTarget)`. This method returns a metric value for the candidate according to the implementing class' criteria, while all
information is either retrieved directly from the candidate `Node` or with the passed function pointer which retrieves the heuristic distance to the __Target__.

__NOTE__: When using a path finding algorithm that always uses 0 ("zero") for its heuristic distance like Dijkstra, the fog exploration is affected as well since this will basically call `AbstractPathFindAlgorithm`'s `GetHeuristic(Node node1, Node node2)`
method.

When fog exploration generally follows these steps, which are repeated until the __Target__ or a clear area is found or there are no further foggy `Node`s to explore.
- select a foggy neighbor
- if no valid neighbor exists, backtrack
- if a valid neighbor exists, move there

Which neighbor is selected in the first step depends on the fog explore algorithm implementation since it is determined solely by the metric that is calculated for each candidate.


# Sequence of Events during Path Finding
The sequence of steps that are being taken in order to try and find a path to the __Target__ are best summed up in the following diagram.

![alt text](https://github.com/mystyfly/304cr-assignment-1/blob/master/pathfinding.png "Path finding workflow")

Initially, path finding is started which attempts to find a path through the clear `Node`s to the __Target__. If such a path is discovered, alternative, equal-cost paths are being investigated as well, after which the program terminates.

If the __Target__ has not been found and no foggy `Node`s have been encountered that could be explored, the program terminates as well. Otherwise, one of the foggy `Node`s is being chosen where the fog exploration will start.

If the fog exploration "stumbles" upon the __Target__, the program terminates (without looking for alternative paths since the __Player__ currently is in the fog. Otherwise, if a previously unknown clear area is discovered, path finding restarts. If no new
area has been discovered, the __Player__ will backtrack until he returns to the foggy `Node` where he initially entered the fog. From there, if there is another foggy `Node` he could explore, he moves to that, otherwise the search terminates without
finding a path.

__NOTE__: More details on what happens during the different stages can be found in the console output.


# Interesting features
Before chosing a foggy `Node` to explore, _bad_ candidates are removed in two steps:
- firstly, all `Node`s are removed that border on a foggy `Node` that has previously been explored and does not contain further interesting neighbors
- secondly, all foggy `Node`s where each neighbor is either clear, impassible or contained in the foggy `Node` list are removed since they won't offer interesting foggy paths to explore

This is done for the sake of optimization since unsuccessful fog exploration is extremely costly: Moving through the clear section to the fog, moving through the fog, backtracking trough the fog, and eventually moving towards another foggy `Node`.

Finding a path from the __Player__ to the __Target__ is partitionned into _segments_. Each segment either is path finding or fog exploration. One of the big advantages of this approach is related to drawing: When a segment is complete, the action that
draws the complete segment is added to a list of actions that will, from now on, always be drawn. That means that the path through previous segments is always visible.

I've chosen to not terminate the path finding as soon as a path has been found but to look for alternative paths with equal cost first. These paths are always equal in cost but may differ in the number of `Node`s that are covered. This information may be
desirable for example when taking other parameters into account, like locations of friendly or opposing units.


# Known Issues/Limitations
Most issues are related to the fog exploration. Firstly, the player must always start on a clear tile.

Also, if a foggy area has been explored without result (and the __Player__ backtracked to the initial `Node`, if the next foggy `Node` is chosen based on the cost to the player, the cost will be calculated starting from the `Node` where the __Player__
started the path finding in the clear area. That means that further foggy `Node`s will not have lowest cost from the current __Player__ position and that the __Player__ must also first move back to that `Node` rather than moving directly to the new foggy
`Node`. If the path back to the starting `Node` and the path to the new foggy `Node` share a common part then this part is being ignored of course.

In order to avoid this sub-optimal behavior, path finding for the already known clear area would have to be restarted which is linked with several new problems and would exceed the scope of this project which is why I decided to neglect that.

Another issue is that, during fog exploration, if the __Player__ finds another clear area that has already been explored previously, he will never move there (except when backtracking to the `Node` where he started the fog exploration). This is primarily to avoid endless loops where the __Player__ keeps moving back and forth between
two clear areas. That could of course be avoided as well but again, this goes beyond the scope of this project. A downside of never visiting known clear areas is that the player may get stuck and terminate path finding without a result even though a path
would indeed exist. An example of that can be seen in the [examples](https://github.com/mystyfly/304cr-assignment-1/blob/master/EXAMPLES.md).

Maps with complex foggy area/clear area structures can break the program by entering an infinite loop.

