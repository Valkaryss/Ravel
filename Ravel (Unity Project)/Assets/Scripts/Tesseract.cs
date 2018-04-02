using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class Tesseract : MonoBehaviour {
    private class Matrix {
        public float[][] contents;
        public int n,m;

        public Matrix(int newN, int newM){
            n = newN;
            m = newM;
            contents = new float[n][];
            for (int i = 0; i < n; i ++){
                contents[i] = new float[m];
                for (int j = 0; j < m; j ++){
                    contents[i][j] = 0f;
                }
            }
        }

        private Matrix multiply(float[][] c1, float[][] c2, int newN, int newM){
            Matrix result = new Matrix(newN,newM);
            for (int i = 0; i < newN; i ++){
                for (int j = 0; j < newM; j ++){
                    float sum = 0;
                    for (int k = 0; k < c2.Length; k ++){
                        sum = sum + c1[i][k] * c2[k][j];
                    }
                result.contents[i][j] = sum;
                }
            }
            return result;
        }

        public Matrix Multiply(Matrix other){
            float[][] c2 = other.contents;
            return multiply(contents,c2, n, other.m);
        }

        public void Scale(float sX, float sY,float sZ,float sW){
            for (int i = 0; i < m; i ++){
                contents[0][i] *= sX;
                contents[1][i] *= sY;
                contents[2][i] *= sZ;
                contents[3][i] *= sW;
            }
        }
    }

    public float pScale = .33f; //Scaling for the points
    public float edgeSize = .125f; // Scaling for the edge
    public float rotationsPerSecondX = .5f;
    float rpsX;
    public float rotationsPerSecondW = .25f;
    float rpsW;

    public float scaleX = 1;
    public float scaleY = 1;
    public float scaleZ = 1;

    GameObject[] objs;
    Matrix points;
    int[][] edges;
    int objMax = 100;

    public float lx = 5f;
    public float ly = 5f;
    public float lz = 5f;



	// Use this for initialization
	void Start () {
        objs = new GameObject[objMax];


        points = new Matrix(4,16);
        points.contents = new float[][] {
          new float[]{-1, 1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1},
          new float[]{-1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1, 1},
          new float[]{-1, -1, -1, -1, 1, 1, 1, 1, -1, -1, -1, -1, 1, 1, 1, 1},
          new float[]{-1, -1, -1, -1, -1, -1, -1, -1, 1, 1, 1, 1, 1, 1, 1, 1}
        };

        points.Scale(scaleX, scaleY, scaleZ, 1);

        edges = new int[][]{
            new int[]{0,1}, new int[]{1,2},
            new int[]{2,3}, new int[]{3,0},
            new int[]{4,5}, new int[]{5,6},
            new int[]{6,7}, new int[]{7,4},
            new int[]{8,9}, new int[]{9,10},
            new int[]{10,11}, new int[]{11,8},
            new int[]{12,13}, new int[]{13,14},
            new int[]{14,15}, new int[]{15,12},

            new int[]{0,4}, new int[]{1,5},
            new int[]{2,6}, new int[]{3,7},
            new int[]{8,12}, new int[]{9,13},
            new int[]{10,14}, new int[]{11,15},

            new int[]{0,8}, new int[]{1,9},
            new int[]{2,10}, new int[]{3,11},
            new int[]{4,12}, new int[]{5,13},
            new int[]{6,14}, new int[]{7,15}
                };
	}

	// Update is called once per frame
	void Update () {
        clear();
        rpsX = rotationsPerSecondX;
        rpsW = rotationsPerSecondW;
        tesseract();
	}


    // Convert radians to degrees
    float degrees(float rad){
        return rad * (180.0f / (float)Math.PI);
    }

    // Draw a single edge from the first point to the
    // second point. This is approximated as a cube in
    // space, rotated and resized.
    GameObject drawEdge(float x0, float y0, float z0, 
                        float x1, float y1, float z1) {
        // For testing
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GameObject pivot = new GameObject();
        pivot.transform.SetParent(transform);
        pivot.transform.localPosition = new Vector3(x0,y0,z0);
        cube.transform.SetParent(pivot.transform);

        float dist = (float)  Math.Pow(Math.Pow((x0 - x1),2) + 
                              Math.Pow((y0 - y1),2) + 
                              Math.Pow((z0 - z1),2), 0.5);

        cube.transform.localScale = new Vector3 (dist, edgeSize, edgeSize);
        cube.transform.localPosition = new Vector3 (dist/2,0,0);

        float h1 = (float) Math.Pow(Math.Pow((x0-x1),2)+Math.Pow((z0-z1),2), 0.5f);
        float a1 = (x0-x1);
        float h2 = dist;
        float a2 = h1;

        float thetaY = (float)((h1==0) ? 0:degrees((float)Math.Acos(a1 / h1)));
        float thetaZ = (float)((h2==0) ? 0:degrees((float)Math.Acos(a2 / h2)));

        // Reflect the places where the results don't line up
        if (z1 < z0){ thetaY = 180 - thetaY;}
        else {thetaY = 180 + thetaY;}
        if (y1 < y0){ thetaZ = -thetaZ;}

        pivot.transform.Rotate(new Vector3(0,thetaY,thetaZ));
        cube.transform.SetParent(transform);
        GameObject.Destroy(pivot);

        return cube;
    }

    // draw a single sphere to represent a point.
    GameObject drawPoint (float x, float y, float z) {
        GameObject p = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        p.transform.SetParent(transform);
        p.transform.localScale = new Vector3 (pScale, pScale, pScale);
        p.transform.localPosition = new Vector3(x,y,z);
        return p;
    }

    void clear(){
        if (objs != null) {
            for (int i = 0; i < objs.Length; i ++){
                GameObject.Destroy(objs[i]);
            }
        }
        objs = new GameObject[objMax];
    }

    Matrix rotate(Matrix m, float t1, float t2)
    {
        Matrix rMatrix = new Matrix(4,4);
        rMatrix.contents[0][0] = (float) Math.Cos(t1);
        rMatrix.contents[0][1] = (float) -Math.Sin(t1);
        rMatrix.contents[1][0] = (float) Math.Sin(t1);
        rMatrix.contents[1][1] = (float) Math.Cos(t1);

        rMatrix.contents[2][2] = (float) Math.Cos(t2);
        rMatrix.contents[2][3] = (float) -Math.Sin(t2);
        rMatrix.contents[3][2] = (float) Math.Sin(t2);
        rMatrix.contents[3][3] = (float) Math.Cos(t2);

        Matrix result = rMatrix.Multiply(m);
        return result;
    }

    Matrix project(Matrix M)
    {
        Matrix result = new Matrix(3,16);

        for (int i = 0; i < M.m; i ++){
            float x = M.contents[0][i];
            float y = M.contents[1][i];
            float z = M.contents[2][i];
            float w = M.contents[3][i];

            x = x/(lx - w);
            y = y/(ly - w);
            z = z/(lz - w);
            result.contents[0][i] = x;
            result.contents[1][i] = y;
            result.contents[2][i] = z;
        }
        return result;
    }

    void tesseract() {
        int count = 0;
        // Rotation per frame for the two rotations
        float t1 = (float) (2 * Math.PI * rpsX) * Time.deltaTime;
        float t2 = (float) (2 * Math.PI * rpsW) * Time.deltaTime;
        points = rotate(points, t1,t2);

        // Different projections offer vastly different final objects
        float[][] pPoints = project(points).contents; // projectedPoints

        // Draw the edges and points as cubes and spheres
        for (int i = 0; i < pPoints[0].Length; i ++){
            float x = pPoints[0][i];
            float y = pPoints[1][i];
            float z = pPoints[2][i];
            objs[count] = drawPoint(x,y,z);
            count += 1;
        }

        for (int i = 0; i < edges.Length; i ++){
            int p0 = edges[i][0];
            int p1 = edges[i][1];

            float x0 = pPoints[0][p0];
            float y0 = pPoints[1][p0];
            float z0 = pPoints[2][p0];

            float x1 = pPoints[0][p1];
            float y1 = pPoints[1][p1];
            float z1 = pPoints[2][p1];

            objs[count] = drawEdge(x0,y0,z0, x1,y1,z1);
            count += 1;
        }
    }
}
