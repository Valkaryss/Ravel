import maya.cmds as cmds
if 'objs' not in globals():
    objs = []
objs = []
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
def snowflake(p1, p2, p3, depth, scale = 1):
    if depth == 0:
        create = cmds.polyCreateFacet(p=[p1,p2,p3])
        objs.append(create)
    else:
        c = avg(p1,p2,p3)
        scale *= .75
        p4 = resize(p1,c,scale)
        p5 = resize(p2,c,scale)
        p6 = resize(p3,c,scale)
        p7 = resize(normalPoint(p2,p3,p1, c), c, scale * .75)
        create = cmds.polyCreateFacet(p = [p1,p2,p3, (), p4,p5,p6])
        objs.append(create)
        snowflake(p5,p6,p7,depth - 1, scale)
        snowflake(p5,p7,p4,depth - 1, scale)
        snowflake(p6,p4,p7,depth - 1, scale)

p1 = (0,1,0)
p2 = (.94281, -.3333, 0)
p3 = (-.47140, -.3333,.81650)
p4 = (-.47140,-.3333,-.81650) 
depth = 3
snowflake(p1,p3,p2,depth)
snowflake(p1,p4,p3,depth)
snowflake(p1,p2,p4,depth)
snowflake(p4,p2,p3,depth)

cmds.select(objs[0])
for obj in objs:
    try:
        cmds.select(obj, add = True)
    except:
        pass
objs = [cmds.polyUnite()]
cmds.polyMergeVertex(d = 0.0)
