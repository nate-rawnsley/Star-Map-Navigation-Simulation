# Star Map Navigation Simulation

A scene that allows the user to generate 'stars' with random names in random locations, and routes between them (both within parameters set by the user). The user can then click on two stars, and the program will calculate the most efficient route between them, using Dijkstra's algorithm, and display the result.

Hovering over an element in the UI gives details on what exactly it changes and if it can be avoided by being set to 0 (as is the case for most parameters in route generation).

Each route between stars has a 'weight' based on the distance between them, for the algorithm to compare. The user can make the scale of each star (which is randomised within a specified range) affect the route cost, applying a multiplier to make larger planets less costly to approach and more costly to leave.

The UI appears best in built versions of the project, and may not display correctly in-editor.

## CONTROLS

**Change parameters in UI** - Mouse click / drag

**Read object name & details / UI details** - Mouse hover

**Move camera centre** - W/A/S/D/Z/C

**Zoom in / out** - Q / E

**Rotate camera view** - Arrow keys

**Reset camera view** - R

**Quit** - Esc

## CREDITS

**Zoom in Icon** - Himmlisch, L. (2022). ui zoom in [Image]. iconduck.com. `https://iconduck.com/icons/249378/ui-zoom-in`

**DijkstraSimplified.cs & all_words.txt** - Supplied by University of Gloucestershire, edited.

**RectExtensions.cs** - Juhata, M. (2017, January 14). Changing RectTransform's Height from script. Unity Forums. `https://forum.unity.com/threads/changing-recttransforms-height-from-script.451071/`

**Galaxy Skybox** - Terrel, R. Space 3D. wwwtyro.net. `http://wwwtyro.github.io/space-3d/`
