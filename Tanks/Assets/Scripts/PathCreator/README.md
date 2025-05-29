Hello, this is my Path Creator!

This uses cubic beziers created using Berstein polynomials 

The code was heavily inspired by Freya Holmer's video, the Beauty of Bezier Curves 
(https://www.youtube.com/watch?v=aVwxzDHniEw&ab_channel=FreyaHolm%C3%A9r)
as well as Sebastian Lague's video on path creation
(https://www.youtube.com/watch?v=saAQNRSYU9k&ab_channel=SebastianLague).


Some hotkeys (KEEP IN MIND YOU SHOULD HAVE GIZMOS ENABLED):
B = free movement
N = mesh snapping
M = planar snapping

CAPSLOCK = move each individual vertex using transform gizmos


Some useful functions you can use is VertexPath's MoveConstantVelocity which takes in a velocity
and will return a Vector3 of the next position in the path that the object must move toward.

You need a reference to a float t, a value that records where you are in the path. For example,
if t=3.4, that means you are 0.4 of the way between vertices[3] and vertices[4].

If advanceforward is enabled, VertexPath will automatically increase t, linear interpolating (or bezier interpolating)
between points depending on the mode.

