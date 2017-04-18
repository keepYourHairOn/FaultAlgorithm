# FaultAlgorithm
2D Terrain Generation Algorithms created as the project for the **Procedural Content Generation** (PCG) course.

Fault algorithm is used generally in cases when it is needed to simulate natural terrain corrosion. 
The key point of the algorithm is to bisect the terrain multiple times Fig. 2. Detailed description of algorithm’s work:
1.	Draw a random line through the plane surface.
2.	Increase heights of all particles from one side of the line on specified value. Decrease the value for which you change the height, each time the step is repeated.
3.	Repeat step 1 and 2 until you won’t see lines, and the heights of mountain range appear. 
