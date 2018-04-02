import maya.cmds as cmds
import random

def randomize():
    base = cmds.ls(selection = True)
    num = 100
    hiDist = 50
    loDist = -50
    hiScale = 2
    loScale = .5
    for i in range(num):
        cmds.select(base)
        create = cmds.duplicate()
        x = random.random() * (hiDist - loDist) + loDist
        z = random.random() * (hiDist - loDist) + loDist
        print (x,z)
        cmds.move(x,0,z, relative = True)
        s = random.random() * (hiScale - loScale) + loScale
        cmds.scale(s,s,s, relative = True)
randomize()