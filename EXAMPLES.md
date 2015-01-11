# Example 1
This shows the situation where the main focus of the assignment lies: A map where the A\*-Algorithm is supposed to find the shortest path to the target.

![Example 1: Path found](https://github.com/mystyfly/304cr-assignment-1/blob/master/examples/e1-path-found.png "Example 1: Path found")

After _a_ shortest cost path has been found, other equal-cost nodes are also investigated in order to find other paths that are just as short. Eventually, all shortest cost paths are displayed with those paths that visit fewer nodes being drawn in green and the longer ones in red.

![Example 1: Alternative paths found](https://github.com/mystyfly/304cr-assignment-1/blob/master/examples/e1-alternatives.png "Example 1: Alternative paths found")

In contrast to the A\*-Algorithm, Dijkstra's Algorithm by definition visits at least as many nodes on every map. In this example, nearly every passible node is visited (99.47%).

![Example 1: Dijkstra](https://github.com/mystyfly/304cr-assignment-1/blob/master/examples/e1-dijkstra.png "Example 1: Dijkstra")


# Example 2
The second example shows basic behavior that is exhibited when no clear path to the target can be found.

![Example 2: No clear path found](https://github.com/mystyfly/304cr-assignment-1/blob/master/examples/e2-no-path.png "Example 2: No clear path found")

According to the [path finding workflow](https://github.com/mystyfly/304cr-assignment-1/blob/master/pathfinding.png), the next step is to choose a foggy node to explore.

![Example 2: Choose a foggy node](https://github.com/mystyfly/304cr-assignment-1/blob/master/examples/e2-foggy-node.png "Example 2: Choose a foggy node")

The __Player__ might get stuck while exploring...

![Example 2: Stuck in the fog](https://github.com/mystyfly/304cr-assignment-1/blob/master/examples/e2-fog-stuck.png "Example 2: Stuck in the fog")

... so he would backtrack until he finds new explorable foggy Nodes...

![Example 2: Backtracking in the fog](https://github.com/mystyfly/304cr-assignment-1/blob/master/examples/e2-backtracked.png "Example 2: Backtracking in the fog")

... until he eventually finds a way out of the fog.

![Example 2: Eventually escaped the fog](https://github.com/mystyfly/304cr-assignment-1/blob/master/examples/e2-escaped-fog.png "Example 2: Eventually escaped the fog")

From there, normal path finding is carried out to find a path to the __Target__.

![Example 2: Eventually found the target](https://github.com/mystyfly/304cr-assignment-1/blob/master/examples/e2-found-target.png "Example 2: Eventually found the target")


# Example 3
The different means of choosing a foggy Node to start the exploration and to choose a foggy neighboring Node in the fog have a significant impact on the result of the path finding.

Generally, moving to the foggy Node that is closest to the player and, in the fog, moving to the Node that has lowest cost yields the worst results.

![Example 3: Choose closest Node to the player and move to cheapest Node](https://github.com/mystyfly/304cr-assignment-1/blob/master/examples/e3-mincost-closesttoplayer.png "Example 3: Choose closest Node to the player and move to cheapest Node")

Just by chaning the behavior in the fog to moving to the Node that gets the __Player__ closest to the __Target__, the result usually is already greatly improved.

![Example 3: Choose closest Node to the player and move to nearest Node to Target](https://github.com/mystyfly/304cr-assignment-1/blob/master/examples/e3-mindist-closesttoplayer.png "Example 3: Choose closest Node to the player and move to nearest Node to Target")

However, normally the best results are achieved by exploring a foggy Node that is already closest to the __Target__ since the path to the Node is likely fairly good as it has been discovered by path finding rather than try-and-error. Even when moving
to the Node with lowest cost when in the fog, this method reduces the total cost by another 20% in this example.

![Example 3: Choose closest Node to the target and move to cheapest Node](https://github.com/mystyfly/304cr-assignment-1/blob/master/examples/e3-mincost-closesttotarget.png "Example 3: Choose closest Node to the target and move to cheapest Node")


# Example 4
Unfortunately, it's not always possible to find a path through complex fog, as displayed here. This shows the situation in a hand-crafted map where the player finds a clear area from where all bordering fog in turn borders on already explored clear
sections of the map.

![Example 4: About to be stuck](https://github.com/mystyfly/304cr-assignment-1/blob/master/examples/e4-about-to-be-stuck.png "Example 4: About to be stuck")

Since everything around that clear area either is already explored or doesn't lead to new territory, the __Player__ gets stuck even though there would be paths to the __Target__.

![Example 4: Stuck!](https://github.com/mystyfly/304cr-assignment-1/blob/master/examples/e4-stuck.png "Example 4: Stuck!")

