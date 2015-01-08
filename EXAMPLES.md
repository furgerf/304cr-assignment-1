# Example 1
This shows the situation where the main focus of the assignment lies: A map where the A\*-Algorithm is supposed to find the shortest path to the target.

![alt text](https://github.com/mystyfly/304cr-assignment-1/blob/master/examples/e1-path-found.png "Example 1: Path found")

After _a_ shortest cost path has been found, other equal-cost nodes are also investigated in order to find other paths that are just as short. Eventually, all shortest cost paths are displayed with those paths that visit fewer nodes being drawn in green and the longer ones in red.

![alt text](https://github.com/mystyfly/304cr-assignment-1/blob/master/examples/e1-alternatives.png "Example 1: Alternative paths found")

In contrast to the A\*-Algorithm, Dijkstra's Algorithm by definition visits at least as many nodes on every map. In this example, nearly every passible node is visited (99.47%).

![alt text](https://github.com/mystyfly/304cr-assignment-1/blob/master/examples/e1-dijkstra.png "Example 1: Dijkstra")


# Example 2
The second example shows basic behavior that is exhibited when no clear path to the target can be found.

![alt text](https://github.com/mystyfly/304cr-assignment-1/blob/master/examples/e2-no-path.png "Example 2: No clear path found")

According to the [path finding workflow](https://github.com/mystyfly/304cr-assignment-1/blob/master/pathfinding.png), the next step is to choose a foggy node to explore.

![alt text](https://github.com/mystyfly/304cr-assignment-1/blob/master/examples/e2-foggy-node.png "Example 2: Choose a foggy node")




![alt text](https://github.com/mystyfly/304cr-assignment-1/blob/master/examples/e2-fog-stuck.png "Example 2: Stuck in the fog")

![alt text](https://github.com/mystyfly/304cr-assignment-1/blob/master/examples/e2-backtracked.png "Example 2: Backtracking in the fog")


![alt text](https://github.com/mystyfly/304cr-assignment-1/blob/master/examples/e2-escaped-fog.png "Example 2: Eventually escaped the fog")


![alt text](https://github.com/mystyfly/304cr-assignment-1/blob/master/examples/e2-found-target.png "Example 2: Eventually found the target")




![alt text](https://github.com/mystyfly/304cr-assignment-1/blob/master/examples/e3-mincost-closesttoplayer.png "Example 3: Choose a foggy node")

![alt text](https://github.com/mystyfly/304cr-assignment-1/blob/master/examples/e3-mindist-closesttoplayer.png "Example 3: Choose a foggy node")

![alt text](https://github.com/mystyfly/304cr-assignment-1/blob/master/examples/e3-mincost-closesttotarget.png "Example 3: Choose a foggy node")





![alt text](https://github.com/mystyfly/304cr-assignment-1/blob/master/examples/e4-about-to-be-stuck.png "Example 4: Choose a foggy node")


![alt text](https://github.com/mystyfly/304cr-assignment-1/blob/master/examples/e4-stuck.png "Example 4: Choose a foggy node")






