import maya.cmds as cmds
if 'objs' not in globals():
    objs = []

# A frankly obnoxious amount the trig. 
def drawEdge(x0,y0,z0,x1,y1,z1):
    create = cmds.polyCube()
    objs.append(create)
    distance = ((x0 - x1)**2 + (y0-y1)**2 + (z0 - z1)**2)**0.5
    cmds.xform(pivots = [-.5,0,0])
    cmds.xform(scale = [distance,.0625/4,.0625/4])
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
def drawPoint(x,y,z, radius =.0625/2):
    create = cmds.polySphere(radius = radius)
    objs.append(create)
    cmds.move(x,y,z)
    
def project(a):
    # Projection 1: Relative offset
    # projection = np.array([(1,0,0), (0,1,0), (0,0,1), (.5,.5,.5)])
    # return a.dot( projection)

    # Projection 2: Lightsource
    res = []
    (lx,ly,lz) = (3,3,3)
    for p in a:
        (x,y,z,w) = p[0],p[1],p[2],p[3]
        x = x/(lx-w)
        y = y/(ly-w)
        z = z/(lz-w)
        res += [(x,y,z)]
    return res

##################################################
##################################################
##################################################

# Computes the average (center) of a triangle
def avg(p1,p2,p3):
    (x1,y1,z1) = p1[0],p1[1],p1[2]
    (x2,y2,z2) = p2[0],p2[1],p2[2]
    (x3,y3,z3) = p3[0],p3[1],p3[2]
    x = (x1 + x2 + x3)/3.0
    y = (y1 + y2 + y3)/3.0
    z = (z1 + z2 + z3)/3.0
    return (x,y,z)

# Intakes a point, a center point to scale around, and a scaling factor
def resize(p, c, s):
    px,py,pz = p[0],p[1],p[2]
    cx,cy,cz = c[0],c[1],c[2]
    px,py,pz = px-cx, py-cy, pz-cz
    px,py,pz = px*s,  py*s,  pz*s
    px,py,pz = px+cx, py+cy, pz+cz
    return (px,py,pz)

def normalPoint(p1,p2,p3, c):
    a1 = p2[0] - p1[0]
    a2 = p2[1] - p1[1]
    a3 = p2[2] - p1[2]

    b1 = p3[0] - p1[0]
    b2 = p3[1] - p1[1]
    b3 = p3[2] - p1[2]

    x = c[0] + a2 * b3 - a3 * b2
    y = c[1] + -a1 * b3 + a3 * b1
    z = c[2] + a1 * b2 - a2 * b1

    return (x,y,z)

# Recursively generate the snowflake
def snowflake(p1, p2, p3, depth, scale, points, edges):
    i = len(points)
    x1,y1,z1 = p1[0], p1[1], p1[2]
    x2,y2,z2 = p2[0], p2[1], p2[2]
    x3,y3,z3 = p3[0], p3[1], p3[2]
    q1 = (x1,y1,z1,0)
    q2 = (x2,y2,z2,0)
    q3 = (x3,y3,z3,0)

    q4 = (x1,y1,z1,1)
    q5 = (x2,y2,z2,1)
    q6 = (x3,y3,z3,1)

    points.extend([q1,q2,q3,q4,q5,q6])
    edges.extend([
        (i + 0,i + 1), (i + 0,i + 1), 
        (i + 0,i + 2),

        (i + 3, i + 4), (i + 4, i + 5), 
        (i + 5, i + 3),

        (i + 0,i + 3), 
        (i + 1, i + 4), (i + 2,i + 5)])

    if depth == 0:
        return
    else:
        c = avg(p1,p2,p3)
        scale *= .75
        p4 = resize(p1,c,scale)
        p5 = resize(p2,c,scale)
        p6 = resize(p3,c,scale)
        p7 = resize(normalPoint(p2,p3,p1, c), c, scale * .75)
        snowflake(p5,p6,p7,depth - 1, scale, points, edges)
        snowflake(p5,p7,p4,depth - 1, scale, points, edges)
        snowflake(p6,p4,p7,depth - 1, scale, points, edges)

def getSnowflake():
    p1 = (0,1,0)
    p2 = (.94281, -.3333, 0)
    p3 = (-.47140, -.3333,.81650)
    p4 = (-.47140,-.3333,-.81650) 

    q1 = (0,1,0, 0)
    q2 = (.94281, -.3333, 0, 0)
    q3 = (-.47140, -.3333,.81650, 0)
    q4 = (-.47140,-.3333,-.81650, 0) 

    q5 = (0,1,0, 1)
    q6 = (.94281, -.3333, 0, 1)
    q7 = (-.47140, -.3333,.81650, 1)
    q8 = (-.47140,-.3333,-.81650, 1) 


    depth = 1
    points = []
    edges = []
    snowflake(p1,p3,p2,depth, 1, points, edges)
    snowflake(p1,p4,p3,depth, 1, points, edges)
    snowflake(p1,p2,p4,depth, 1, points, edges)
    snowflake(p4,p2,p3,depth, 1, points, edges)
    return points,edges

cache = ()   
def getSnowflakeCache():
    global cache
    if len(cache) == 0:
        cache = getSnowflake()
    return cache[0], cache[1]

def drawSnowflake():
    (points,edges) = getSnowflakeCache()
    frame = cmds.currentTime(query = True)

    # Rotation per frame for the two rotations
    t1Factor = 2 * math.pi / 200
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

# Recursive function that generates a snowflake, playblasts it, and moves on
# This could be instead just a call to drawSnoflake linked to a script that 
# runs on frame, but my laptop can't handle the geometry caches that requires.
def runSnowflake(first = False, maxFrame = 200):
    if first: cmds.currentTime(1)
    frame = cmds.currentTime(query = True)
    if (frame < maxFrame):
        clear()
        drawSnowflake()
        print(cmds.playblast(filename = "../snowflake/frame" + stringOf(frame), frame = [frame], viewer = False, h = 1080, w = 1920))
        cmds.currentTime(frame + 1)
        runSnowflake(maxFrame = maxFrame)
runSnowflake(True)