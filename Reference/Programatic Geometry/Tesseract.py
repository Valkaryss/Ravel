import maya.cmds as cmds
import numpy as np
import math
import time

if not 'objs' in globals():
    objs = []

# A frankly obnoxious amount the trig. 
def drawEdge(x0,y0,z0,x1,y1,z1):
    create = cmds.polyCube()
    objs.append(create)
    distance = ((x0 - x1)**2 + (y0-y1)**2 + (z0 - z1)**2)**0.5
    cmds.xform(pivots = [-.5,0,0])
    cmds.xform(scale = [distance,.25,.25])
    cmds.move(.5,0,0)

    # Edge has been sized and zeroed. Now put it in the right place.
    cmds.move(x0,y0,z0, relative = True)
    h1 = ((x0-x1)**2 + (z0-z1)**2)**0.5
    a1 = (x0-x1)
    h2 = ((x0-x1)**2 + (y0 - y1)**2 + (z0-z1)**2)**0.5
    a2 = h1


    thetaY = 0 if (h1 == 0) else math.degrees(math.acos(a1 / h1))
    thetaZ = 0 if (h2 == 0) else math.degrees(math.acos(a2 / h2))

    # Reflect the places where the results don't line up
    if (z1 > z0): thetaY = 180 - thetaY
    else: thetaY = 180 + thetaY
    if (y1 < y0): thetaZ = -thetaZ
    
    cmds.rotate(0,0,thetaZ, relative = True)
    cmds.rotate(0,-thetaY, 0, relative = True)
    

# Draw a point as a sphere
def drawPoint(x,y,z, radius =.25):
    create = cmds.polySphere(radius = radius)
    objs.append(create)
    cmds.move(x,y,z)
    

def project(a):
    # Projection 1: Relative offset
    # projection = np.array([(1,0,0), (0,1,0), (0,0,1), (.5,.5,.5)])
    # return a.dot( projection)

    # Projection 2: Lightsource
    res = []
    (lx,ly,lz) = (1.75,1.75,1.75)
    for p in a:
        (x,y,z,w) = p[0],p[1],p[2],p[3]
        x = x/(lx-w)
        y = y/(ly-w)
        z = z/(lz-w)
        res += [(x,y,z)]
    return res

# All of the points and edges that make up the 4D tesseract. Note that 
# it's easier to conceptualize (and read out) these as row vectors, 
# so sometimes transposition is required to do the math.
def getTesseract():
    points = np.array([ (0,0,0,0), # 1
                        (1,0,0,0), # 2
                        (1,1,0,0), # 3
                        (0,1,0,0), # 4
                        (0,0,1,0), # 5
                        (1,0,1,0), # 6
                        (1,1,1,0), # 7
                        (0,1,1,0), # 8
                        (0,0,0,1), # 9
                        (1,0,0,1), # 10
                        (1,1,0,1), # 11
                        (0,1,0,1), # 12
                        (0,0,1,1), # 13
                        (1,0,1,1), # 14
                        (1,1,1,1), # 15
                        (0,1,1,1), # 16
                        ])
    # Offset to surround (0,0)
    points *= 2
    points -= 1
    edges = np.array([
             (0,1),(1,2),(2,3),(3,0),
             (4, 5),(5,6),(6,7),(7,4),
             (8, 9),(9,10),(10,11),(11,8),
             (12, 13),(13,14),(14,15),(15,12),

             (0,4),(1,5),(2,6),(3,7),
             (8,12),(9,13),(10,14),(11,15),

             (0,8),(1,9),(2,10),(3,11),
             (4,12),(5,13),(6,14),(7,15),
             ])
    return (points,edges)

# Do a double-rotation through four dimensional space. t1 represents rotation
# along the x-axis, while t2 represents rotation along the w-axis. Note that 
# the matrix comes in as row-vectors, but the multiplication has to happen 
# along column vectors.
def rotate(points, t1, t2):
    points = np.transpose(points)
    cos = math.cos
    sin = math.sin
    rMatrix = np.array([[cos(t1), -sin(t1), 0,0],
                        [sin(t1),  cos(t1), 0,0],
                        [0,0, cos(t2), -sin(t2)],
                        [0,0, sin(t2),  cos(t2)]])

    return np.transpose(np.dot(rMatrix,points))


# Tesseract is generated from this function. It will create a different
# rotation based on the frame.
def tesseract():
    (points,edges) = getTesseract()
    frame = cmds.currentTime(query = True)

    # Rotation per frame for the two rotations
    t1Factor = 4 * math.pi / 200
    t2Factor = 2 * math.pi / 200
    t1 = frame * t1Factor
    t2 = frame * t2Factor
    points = rotate(points, t1,t2)

    # Different projections offer vastly different final objects
    points = project(points)

    # Draw the edges and points as cubes and spheres
    for point in points:
        (x,y,z) = point[0],point[1],point[2]
        drawPoint(x,y,z)

    for edge in edges:
        p1 = points[edge[0]]
        p2 = points[edge[1]]
        (x0,y0,z0) = p1[0],p1[1],p1[2]
        (x1,y1,z1) = p2[0],p2[1],p2[2]
        drawEdge(x0,y0,z0, x1,y1,z1)

# Remove every object that this script has ever created
def clear():
    global objs
    for obj in objs:
        try:
            cmds.select(obj)
            cmds.delete()
        except:
            pass
    objs = []
       
# Return the string of the frame, with a pad of 0's at the beginning,
# like 007.
def stringOf(frame, buf = 3):
    frame = int(frame)
    buffer = buf - len(str(frame))       
    return "0" * buffer + str(frame)
    
# Recursive function that generates a tesseract, playblasts it, and moves on
# This could be instead just a call to tesseract linked to a script that runs
# on frame, but my laptop can't handle the geometry caches that requires.
def runTesseract(first = False, maxFrame = 200):
    if first: cmds.currentTime(1)
    frame = cmds.currentTime(query = True)
    if (frame < maxFrame):
        clear()
        tesseract()
        print(cmds.playblast(filename = "../tesseract/frame" + stringOf(frame), frame = [frame], viewer = False, h = 1080, w = 1920))
        cmds.currentTime(frame + 1)
        runTesseract()

clear()
tesseract()
