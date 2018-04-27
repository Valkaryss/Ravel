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
    int[] faces;
    int objMax = 100;
    public MeshFilter meshFilter;
    private Mesh mesh;

    public float lx = 5f;
    public float ly = 5f;
    public float lz = 5f;



    // Use this for initialization
    void Start () {
        objs = new GameObject[objMax];
        mesh = meshFilter.mesh;


        points = new Matrix(4,16);
        points.contents = new float[][] {
          new float[]{-1, 1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1},
          new float[]{-1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1, 1},
          new float[]{-1, -1, -1, -1, 1, 1, 1, 1, -1, -1, -1, -1, 1, 1, 1, 1},
          new float[]{-1, -1, -1, -1, -1, -1, -1, -1, 1, 1, 1, 1, 1, 1, 1, 1}
        };

        points.Scale(scaleX, scaleY, scaleZ, 1);

        // Trust me
        faces = new int[] {8,0,9,0,1,9,9,1,2,2,10,9,2,3,11,11,10,2,8,11,0,0,11,3,
                           0,4,5,5,1,0,2,1,5,5,6,2,0,3,4,4,3,7,7,3,6,6,3,2,4,7,15,
                           15,12,4,11,15,14,14,10,11,12,15,11,11,8,12,8,9,12,9,13,12,
                           9,10,14,14,13,9,14,6,5,5,13,14,14,15,7,7,6,14,13,5,12,12,5,4};
        mesh.triangles = faces;
        mesh.MarkDynamic();
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


    void clear(){
        if (objs != null) {
            for (int i = 0; i < objs.Length; i ++){
                Destroy(objs[i]);
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
        mesh.Clear();
        // Rotation per frame for the two rotations
        float t1 = (float) (2 * Math.PI * rpsX) * Time.deltaTime;
        float t2 = (float) (2 * Math.PI * rpsW) * Time.deltaTime;
        points = rotate(points, t1,t2);

        // Different projections offer vastly different final objects
        float[][] pPoints = project(points).contents; // projectedPoints
        Vector3[] verts = convertToV3(pPoints);
        mesh.vertices = verts;
        mesh.triangles = faces;

        flatten(mesh);

    }

    Vector3[] convertToV3(float[][] ps){
        Vector3[] result = new Vector3[ps[0].Length];
        for (int i = 0; i < ps[0].Length; i++){
            result[i] = new Vector3(ps[0][i], ps[1][i], ps[2][i]);
        }
        return result;
    }


    void drawText(string text, Vector3 location){
        GameObject result = new GameObject();
        result.transform.position = location;
        result.AddComponent<TextMesh>();
        TextMesh textMesh = result.GetComponent<TextMesh>();
        textMesh.text = text;
        textMesh.characterSize = 5f;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        result.transform.Rotate(new Vector3(0, 180, 0));
    }

    void flatten(Mesh m){
        int[] tris = m.triangles;
        Vector3[] verts = m.vertices;

        int[] resultTris = new int[tris.Length];
        Vector3[] resultVerts = new Vector3[tris.Length];

        for (int i = 0; i < tris.Length; i ++){
            int t = tris[i];
            Vector3 p = verts[t];
            resultVerts[i] = p;
            resultTris[i] = i;
        }

        m.vertices = resultVerts;
        m.triangles = resultTris;
        mesh.RecalculateNormals();
    }
}