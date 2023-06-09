using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingCubesOld : MonoBehaviour
{
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;

    //public float[,,] map;
    public float cubeSize = 1, cutOff;

    public int testnode = 0;

    CubeGrid cg;
    // Start is called before the first frame update
    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void run(Vector4[] map, int size)
    {
        cg = new CubeGrid(map,size, cubeSize);
        updateMesh();
    }
    public void updateMesh()
    {


        Mesh mesh = new Mesh();
        mesh.Clear();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = cg.vertices.ToArray();
        mesh.triangles = cg.tris.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        mesh.Optimize();
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    public class Node
    {
        public Vector3 position;
        public int index = -1;
        public Node()
        {

        }
    }
    public class ControlNode : Node
    {
        public bool active;

        public ControlNode()
        {

        }
    }
    public class Cube
    {
        //public Vector3 position = new Vector3(0,0,0);
        //public float cubeSize;

        public ControlNode[] controlNodes = new ControlNode[8];
        public Node[] nodes = new Node[12];

        public void PositionNodes()
        {

            for (int i = 0; i < 8; i++)
            {
                controlNodes[i] = new ControlNode();
            }

            //align control nodes
            controlNodes[0].position = new Vector3(0, 0, 0);
            controlNodes[1].position = new Vector3(1, 0, 0);
            controlNodes[2].position = new Vector3(1, 0, 1);
            controlNodes[3].position = new Vector3(0, 0, 1);
            controlNodes[4].position = new Vector3(0, 1, 0);
            controlNodes[5].position = new Vector3(1, 1, 0);
            controlNodes[6].position = new Vector3(1, 1, 1);
            controlNodes[7].position = new Vector3(0, 1, 1);


            for (int i = 0; i < 12; i++)
            {
                nodes[i] = new Node();
            }
            //align nodes



            nodes[0].position = new Vector3(.5f, 0, 0);
            nodes[1].position = new Vector3(1, 0, .5f);
            nodes[2].position = new Vector3(.5f, 0, 1);
            nodes[3].position = new Vector3(0, 0, .5f);
            nodes[4].position = new Vector3(.5f, 1, 0);
            nodes[5].position = new Vector3(1, 1, .5f);
            nodes[6].position = new Vector3(.5f, 1, 1);
            nodes[7].position = new Vector3(0, 1, .5f);
            nodes[8].position = new Vector3(0, .5f, 0);
            nodes[9].position = new Vector3(1, .5f, 0);
            nodes[10].position = new Vector3(1, .5f, 1);
            nodes[11].position = new Vector3(0, .5f, 1);

        }

        public int evaluate()
        {
            int value = 0;
            if (controlNodes[0].active) { value += 1; }
            if (controlNodes[1].active) { value += 2; }
            if (controlNodes[2].active) { value += 4; }
            if (controlNodes[3].active) { value += 8; }
            if (controlNodes[4].active) { value += 16; }
            if (controlNodes[5].active) { value += 32; }
            if (controlNodes[6].active) { value += 64; }
            if (controlNodes[7].active) { value += 128; }
            return value;
        }

        public Cube()
        {
            PositionNodes();

        }
    }
    

    public class CubeGrid
    {
        public Cube[,,] cubes;
        public Vector4[] floatMap;
        public int size;
        public float scale;
        float cutOff = .5f;
        public List<Vector3> vertices = new List<Vector3>();
        public List<int> tris = new List<int>();
        //int vertIndex = 0;

        public int to1D(int x, int y, int z)
        {
            return (z * size * size) + (y * size) + x;
        }

        //public IEnumerator ieGenerateCubes(float time)
        //{
        //    cubes = new Cube[
        //        floatMap.GetLength(0),
        //        floatMap.GetLength(1),
        //        floatMap.GetLength(2)
        //        ];
        //    for (int x = 0; x < cubes.GetLength(0) - 1; x++)
        //    {
        //        for (int y = 0; y < cubes.GetLength(1) - 1; y++)
        //        {
        //            for (int z = 0; z < cubes.GetLength(2) - 1; z++)
        //            {
        //                cubes[x, y, z] = new Cube();
        //                cubes[x, y, z].controlNodes[0].active = floatMap[x, y, z] > cutOff;
        //                cubes[x, y, z].controlNodes[1].active = floatMap[x + 1, y, z] > cutOff;
        //                cubes[x, y, z].controlNodes[2].active = floatMap[x + 1, y, z + 1] > cutOff;
        //                cubes[x, y, z].controlNodes[3].active = floatMap[x, y, z + 1] > cutOff;
        //                cubes[x, y, z].controlNodes[4].active = floatMap[x, y + 1, z] > cutOff;
        //                cubes[x, y, z].controlNodes[5].active = floatMap[x + 1, y + 1, z] > cutOff;
        //                cubes[x, y, z].controlNodes[6].active = floatMap[x + 1, y + 1, z + 1] > cutOff;
        //                cubes[x, y, z].controlNodes[7].active = floatMap[x, y + 1, z + 1] > cutOff;

        //                int cubeVal = cubes[x, y, z].evaluate();
        //                for (int i = 0; triTable[cubeVal, i] != -1; i += 3)
        //                {

        //                }
        //                yield return new WaitForSecondsRealtime(1);
        //            }
        //        }
        //    }
        //    print("AAAAAAAAAAA");
        //}
        public void generateCubes()
        {
            cubes = new Cube[
                size,
                size,
                size
                ];
            //int cubeNum = 0;
            int vertNum = 0;
            for (int x = 0; x < cubes.GetLength(0) - 1; x++)
            {
                for (int y = 0; y < cubes.GetLength(1) - 1; y++)
                {
                    for (int z = 0; z < cubes.GetLength(2) - 1; z++)
                    {
                        cubes[x, y, z] = new Cube();
                        Cube cube = cubes[x, y, z];
                        cube.controlNodes[0].active = floatMap[to1D(x, y, z)].w > cutOff;
                        cube.controlNodes[1].active = floatMap[to1D(x + 1, y, z)].w > cutOff;
                        cube.controlNodes[2].active = floatMap[to1D(x + 1, y, z + 1)].w > cutOff;
                        cube.controlNodes[3].active = floatMap[to1D(x, y, z + 1)].w > cutOff;
                        cube.controlNodes[4].active = floatMap[to1D(x, y + 1, z)].w > cutOff;
                        cube.controlNodes[5].active = floatMap[to1D(x + 1, y + 1, z)].w > cutOff;
                        cube.controlNodes[6].active = floatMap[to1D(x + 1, y + 1, z + 1)].w > cutOff;
                        cube.controlNodes[7].active = floatMap[to1D(x, y + 1, z + 1)].w > cutOff;
                                                               
                        int cubeVal = cube.evaluate();
                        for (int i = 0; triTable[cubeVal, i] != -1; i += 3)
                        {
                            for (int v = 0; v < 3; v++)
                            {
                                int num = triTable[cubeVal, i + v];

                                switch (num)
                                {
                                    case 0:
                                        {

                                            if (y > 0 && z > 0)
                                            {

                                                tris.Add(cubes[x, y - 1, z - 1].nodes[6].index);

                                            }
                                            else if (z > 0)
                                            {
                                                tris.Add(cubes[x, y, z - 1].nodes[2].index);
                                            }

                                            else
                                            {
                                                //print(5);
                                                vertices.Add((cube.nodes[triTable[cubeVal, i + v]].position + new Vector3(x, y, z)) * scale);
                                                cube.nodes[triTable[cubeVal, i + v]].index = vertNum;
                                                tris.Add(vertNum);
                                                vertNum++;
                                            }
                                            break;
                                        }

                                    case 1:
                                        {

                                            if (y > 0)
                                            {

                                                tris.Add(cubes[x, y - 1, z].nodes[5].index);

                                            }


                                            else
                                            {

                                                vertices.Add((cube.nodes[triTable[cubeVal, i + v]].position + new Vector3(x, y, z)) * scale);
                                                cube.nodes[triTable[cubeVal, i + v]].index = vertNum;
                                                tris.Add(vertNum);
                                                vertNum++;
                                            }
                                            break;
                                        }

                                    case 2:
                                        {

                                            if (y > 0)
                                            {

                                                tris.Add(cubes[x, y - 1, z].nodes[6].index);

                                            }


                                            else
                                            {
                                                //print(5);
                                                vertices.Add((cube.nodes[triTable[cubeVal, i + v]].position + new Vector3(x, y, z)) * scale);
                                                cube.nodes[triTable[cubeVal, i + v]].index = vertNum;
                                                tris.Add(vertNum);
                                                vertNum++;
                                            }
                                            break;
                                        }

                                    case 3:
                                        {

                                            if (x > 0 && y > 0)
                                            {

                                                tris.Add(cubes[x - 1, y - 1, z].nodes[5].index);

                                            }
                                            else if (y > 0)
                                            {
                                                tris.Add(cubes[x, y - 1, z].nodes[7].index);
                                            }

                                            else
                                            {
                                                //print(5);
                                                vertices.Add((cube.nodes[triTable[cubeVal, i + v]].position + new Vector3(x, y, z)) * scale);
                                                cube.nodes[triTable[cubeVal, i + v]].index = vertNum;
                                                tris.Add(vertNum);
                                                vertNum++;
                                            }
                                            break;
                                        }
                                    case 4:
                                        {

                                            if (z > 0)
                                            {

                                                tris.Add(cubes[x, y, z - 1].nodes[6].index);

                                            }


                                            else
                                            {
                                                //print(5);
                                                vertices.Add((cube.nodes[triTable[cubeVal, i + v]].position + new Vector3(x, y, z)) * scale);
                                                cube.nodes[triTable[cubeVal, i + v]].index = vertNum;
                                                tris.Add(vertNum);
                                                vertNum++;
                                            }
                                            break;
                                        }
                                    case 7:
                                        {

                                            if (x > 0)
                                            {

                                                tris.Add(cubes[x - 1, y, z].nodes[5].index);

                                            }


                                            else
                                            {
                                                //print(5);
                                                vertices.Add((cube.nodes[triTable[cubeVal, i + v]].position + new Vector3(x, y, z)) * scale);
                                                cube.nodes[triTable[cubeVal, i + v]].index = vertNum;
                                                tris.Add(vertNum);
                                                vertNum++;
                                            }
                                            break;
                                        }
                                    case 8:
                                        {

                                            if (x > 0 && z > 0)
                                            {

                                                tris.Add(cubes[x - 1, y, z - 1].nodes[10].index);

                                            }
                                            else if (z > 0)
                                            {
                                                tris.Add(cubes[x, y, z - 1].nodes[11].index);
                                            }

                                            else
                                            {
                                                //print(5);
                                                vertices.Add((cube.nodes[triTable[cubeVal, i + v]].position + new Vector3(x, y, z)) * scale);
                                                cube.nodes[triTable[cubeVal, i + v]].index = vertNum;
                                                tris.Add(vertNum);
                                                vertNum++;
                                            }
                                            break;
                                        }
                                    case 9:
                                        {

                                            if (z > 0)
                                            {

                                                tris.Add(cubes[x, y, z - 1].nodes[10].index);

                                            }


                                            else
                                            {
                                                //print(5);
                                                vertices.Add((cube.nodes[triTable[cubeVal, i + v]].position + new Vector3(x, y, z)) * scale);
                                                cube.nodes[triTable[cubeVal, i + v]].index = vertNum;
                                                tris.Add(vertNum);
                                                vertNum++;
                                            }
                                            break;
                                        }
                                    case 11:
                                        {
                                            if (x > 0)
                                            {
                                                tris.Add(cubes[x - 1, y, z].nodes[10].index);
                                            }

                                            else
                                            {
                                                //print(5);
                                                vertices.Add((cube.nodes[triTable[cubeVal, i + v]].position + new Vector3(x, y, z)) * scale);
                                                cube.nodes[triTable[cubeVal, i + v]].index = vertNum;
                                                tris.Add(vertNum);
                                                vertNum++;
                                            }
                                            break;
                                        }




                                    default:
                                        {
                                            vertices.Add((cube.nodes[triTable[cubeVal, i + v]].position + new Vector3(x, y, z)) * scale);
                                            cube.nodes[triTable[cubeVal, i + v]].index = vertNum;
                                            tris.Add(vertNum);
                                            vertNum++;
                                            break;
                                        }
                                }

                            }



                            //vertNum += 3;
                        }

                        /*
						cubes[x, y, z] = new Cube();
						Cube cube = cubes[x, y, z];
						cube.controlNodes[0].active = boolMap[x    , y    , z    ];
						cube.controlNodes[1].active = boolMap[x + 1, y    , z    ];
						cube.controlNodes[2].active = boolMap[x + 1, y    , z + 1];
						cube.controlNodes[3].active = boolMap[x    , y    , z + 1];
						cube.controlNodes[4].active = boolMap[x    , y + 1, z    ];
						cube.controlNodes[5].active = boolMap[x + 1, y + 1, z    ];
						cube.controlNodes[6].active = boolMap[x + 1, y + 1, z + 1];
						cube.controlNodes[7].active = boolMap[x    , y + 1, z + 1];

						int cubeVal = cube.evaluate();
						for(int i = 0; triTable[cubeVal,i] != -1; i += 3)
						{

							vertices.Add((cube.nodes[triTable[cubeVal, i    ]].position + new Vector3(x, y, z)) * size);
							vertices.Add((cube.nodes[triTable[cubeVal, i + 1]].position + new Vector3(x, y, z)) * size);
							vertices.Add((cube.nodes[triTable[cubeVal, i + 2]].position + new Vector3(x, y, z)) * size);

							tris.Add(0 + vertNum);
							tris.Add(1 + vertNum);
							tris.Add(2 + vertNum);
							vertNum += 3;
						}
						*/
                        //cubeNum++;
                    }
                }
            }
        }



        public CubeGrid(Vector4[] map,int _size, float cubeSize)
        {
            size = _size;
            scale = cubeSize;
            floatMap = map;
            print("Generating mesh...");
            generateCubes();

        }

    }
    private void OnDrawGizmos()
    {
        //return;

        //if (cg == null)
        //{
        //    return;
        //}
        //for (int x = 0; x < cg.cubes.GetLength(0) - 1; x++)
        //{
        //    for (int y = 0; y < cg.cubes.GetLength(1) - 1; y++)
        //    {
        //        for (int z = 0; z < cg.cubes.GetLength(2) - 1; z++)
        //        {
        //            Cube testcube = cg.cubes[x, y, z];
        //            for (int i = 0; i < 8; i++)
        //            {
        //                Gizmos.color = testcube.controlNodes[i].active ? Color.green : Color.red;
        //                Gizmos.DrawWireSphere((testcube.controlNodes[i].position * cubeSize) + new Vector3(x, y, z), .15f);
        //            }
        //            for (int i = 0; i < 12; i++)
        //            {
        //                if (testnode == i)
        //                {
        //                    Gizmos.color = Color.blue;
        //                    Gizmos.DrawWireSphere((testcube.nodes[i].position * cubeSize) + new Vector3(x, y, z), .11f);
        //                }
        //                else
        //                {
        //                    Gizmos.color = Color.magenta;
        //                    Gizmos.DrawWireSphere((testcube.nodes[i].position * cubeSize) + new Vector3(x, y, z), .10f);
        //                }
        //            }

        //        }
        //    }
        //}

    }


    public static int[,] triTable = new int[256, 16]
    {
        {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 1, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 8, 3, 9, 8, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 8, 3, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {9, 2, 10, 0, 2, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {2, 8, 3, 2, 10, 8, 10, 9, 8, -1, -1, -1, -1, -1, -1, -1},
        {3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 11, 2, 8, 11, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 9, 0, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 11, 2, 1, 9, 11, 9, 8, 11, -1, -1, -1, -1, -1, -1, -1},
        {3, 10, 1, 11, 10, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 10, 1, 0, 8, 10, 8, 11, 10, -1, -1, -1, -1, -1, -1, -1},
        {3, 9, 0, 3, 11, 9, 11, 10, 9, -1, -1, -1, -1, -1, -1, -1},
        {9, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {4, 3, 0, 7, 3, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 1, 9, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {4, 1, 9, 4, 7, 1, 7, 3, 1, -1, -1, -1, -1, -1, -1, -1},
        {1, 2, 10, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {3, 4, 7, 3, 0, 4, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1},
        {9, 2, 10, 9, 0, 2, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1},
        {2, 10, 9, 2, 9, 7, 2, 7, 3, 7, 9, 4, -1, -1, -1, -1},
        {8, 4, 7, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {11, 4, 7, 11, 2, 4, 2, 0, 4, -1, -1, -1, -1, -1, -1, -1},
        {9, 0, 1, 8, 4, 7, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1},
        {4, 7, 11, 9, 4, 11, 9, 11, 2, 9, 2, 1, -1, -1, -1, -1},
        {3, 10, 1, 3, 11, 10, 7, 8, 4, -1, -1, -1, -1, -1, -1, -1},
        {1, 11, 10, 1, 4, 11, 1, 0, 4, 7, 11, 4, -1, -1, -1, -1},
        {4, 7, 8, 9, 0, 11, 9, 11, 10, 11, 0, 3, -1, -1, -1, -1},
        {4, 7, 11, 4, 11, 9, 9, 11, 10, -1, -1, -1, -1, -1, -1, -1},
        {9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {9, 5, 4, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 5, 4, 1, 5, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {8, 5, 4, 8, 3, 5, 3, 1, 5, -1, -1, -1, -1, -1, -1, -1},
        {1, 2, 10, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {3, 0, 8, 1, 2, 10, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1},
        {5, 2, 10, 5, 4, 2, 4, 0, 2, -1, -1, -1, -1, -1, -1, -1},
        {2, 10, 5, 3, 2, 5, 3, 5, 4, 3, 4, 8, -1, -1, -1, -1},
        {9, 5, 4, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 11, 2, 0, 8, 11, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1},
        {0, 5, 4, 0, 1, 5, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1},
        {2, 1, 5, 2, 5, 8, 2, 8, 11, 4, 8, 5, -1, -1, -1, -1},
        {10, 3, 11, 10, 1, 3, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1},
        {4, 9, 5, 0, 8, 1, 8, 10, 1, 8, 11, 10, -1, -1, -1, -1},
        {5, 4, 0, 5, 0, 11, 5, 11, 10, 11, 0, 3, -1, -1, -1, -1},
        {5, 4, 8, 5, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1},
        {9, 7, 8, 5, 7, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {9, 3, 0, 9, 5, 3, 5, 7, 3, -1, -1, -1, -1, -1, -1, -1},
        {0, 7, 8, 0, 1, 7, 1, 5, 7, -1, -1, -1, -1, -1, -1, -1},
        {1, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {9, 7, 8, 9, 5, 7, 10, 1, 2, -1, -1, -1, -1, -1, -1, -1},
        {10, 1, 2, 9, 5, 0, 5, 3, 0, 5, 7, 3, -1, -1, -1, -1},
        {8, 0, 2, 8, 2, 5, 8, 5, 7, 10, 5, 2, -1, -1, -1, -1},
        {2, 10, 5, 2, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1},
        {7, 9, 5, 7, 8, 9, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1},
        {9, 5, 7, 9, 7, 2, 9, 2, 0, 2, 7, 11, -1, -1, -1, -1},
        {2, 3, 11, 0, 1, 8, 1, 7, 8, 1, 5, 7, -1, -1, -1, -1},
        {11, 2, 1, 11, 1, 7, 7, 1, 5, -1, -1, -1, -1, -1, -1, -1},
        {9, 5, 8, 8, 5, 7, 10, 1, 3, 10, 3, 11, -1, -1, -1, -1},
        {5, 7, 0, 5, 0, 9, 7, 11, 0, 1, 0, 10, 11, 10, 0, -1},
        {11, 10, 0, 11, 0, 3, 10, 5, 0, 8, 0, 7, 5, 7, 0, -1},
        {11, 10, 5, 7, 11, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 8, 3, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {9, 0, 1, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 8, 3, 1, 9, 8, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1},
        {1, 6, 5, 2, 6, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 6, 5, 1, 2, 6, 3, 0, 8, -1, -1, -1, -1, -1, -1, -1},
        {9, 6, 5, 9, 0, 6, 0, 2, 6, -1, -1, -1, -1, -1, -1, -1},
        {5, 9, 8, 5, 8, 2, 5, 2, 6, 3, 2, 8, -1, -1, -1, -1},
        {2, 3, 11, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {11, 0, 8, 11, 2, 0, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1},
        {0, 1, 9, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1},
        {5, 10, 6, 1, 9, 2, 9, 11, 2, 9, 8, 11, -1, -1, -1, -1},
        {6, 3, 11, 6, 5, 3, 5, 1, 3, -1, -1, -1, -1, -1, -1, -1},
        {0, 8, 11, 0, 11, 5, 0, 5, 1, 5, 11, 6, -1, -1, -1, -1},
        {3, 11, 6, 0, 3, 6, 0, 6, 5, 0, 5, 9, -1, -1, -1, -1},
        {6, 5, 9, 6, 9, 11, 11, 9, 8, -1, -1, -1, -1, -1, -1, -1},
        {5, 10, 6, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {4, 3, 0, 4, 7, 3, 6, 5, 10, -1, -1, -1, -1, -1, -1, -1},
        {1, 9, 0, 5, 10, 6, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1},
        {10, 6, 5, 1, 9, 7, 1, 7, 3, 7, 9, 4, -1, -1, -1, -1},
        {6, 1, 2, 6, 5, 1, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1},
        {1, 2, 5, 5, 2, 6, 3, 0, 4, 3, 4, 7, -1, -1, -1, -1},
        {8, 4, 7, 9, 0, 5, 0, 6, 5, 0, 2, 6, -1, -1, -1, -1},
        {7, 3, 9, 7, 9, 4, 3, 2, 9, 5, 9, 6, 2, 6, 9, -1},
        {3, 11, 2, 7, 8, 4, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1},
        {5, 10, 6, 4, 7, 2, 4, 2, 0, 2, 7, 11, -1, -1, -1, -1},
        {0, 1, 9, 4, 7, 8, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1},
        {9, 2, 1, 9, 11, 2, 9, 4, 11, 7, 11, 4, 5, 10, 6, -1},
        {8, 4, 7, 3, 11, 5, 3, 5, 1, 5, 11, 6, -1, -1, -1, -1},
        {5, 1, 11, 5, 11, 6, 1, 0, 11, 7, 11, 4, 0, 4, 11, -1},
        {0, 5, 9, 0, 6, 5, 0, 3, 6, 11, 6, 3, 8, 4, 7, -1},
        {6, 5, 9, 6, 9, 11, 4, 7, 9, 7, 11, 9, -1, -1, -1, -1},
        {10, 4, 9, 6, 4, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {4, 10, 6, 4, 9, 10, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1},
        {10, 0, 1, 10, 6, 0, 6, 4, 0, -1, -1, -1, -1, -1, -1, -1},
        {8, 3, 1, 8, 1, 6, 8, 6, 4, 6, 1, 10, -1, -1, -1, -1},
        {1, 4, 9, 1, 2, 4, 2, 6, 4, -1, -1, -1, -1, -1, -1, -1},
        {3, 0, 8, 1, 2, 9, 2, 4, 9, 2, 6, 4, -1, -1, -1, -1},
        {0, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {8, 3, 2, 8, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1},
        {10, 4, 9, 10, 6, 4, 11, 2, 3, -1, -1, -1, -1, -1, -1, -1},
        {0, 8, 2, 2, 8, 11, 4, 9, 10, 4, 10, 6, -1, -1, -1, -1},
        {3, 11, 2, 0, 1, 6, 0, 6, 4, 6, 1, 10, -1, -1, -1, -1},
        {6, 4, 1, 6, 1, 10, 4, 8, 1, 2, 1, 11, 8, 11, 1, -1},
        {9, 6, 4, 9, 3, 6, 9, 1, 3, 11, 6, 3, -1, -1, -1, -1},
        {8, 11, 1, 8, 1, 0, 11, 6, 1, 9, 1, 4, 6, 4, 1, -1},
        {3, 11, 6, 3, 6, 0, 0, 6, 4, -1, -1, -1, -1, -1, -1, -1},
        {6, 4, 8, 11, 6, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {7, 10, 6, 7, 8, 10, 8, 9, 10, -1, -1, -1, -1, -1, -1, -1},
        {0, 7, 3, 0, 10, 7, 0, 9, 10, 6, 7, 10, -1, -1, -1, -1},
        {10, 6, 7, 1, 10, 7, 1, 7, 8, 1, 8, 0, -1, -1, -1, -1},
        {10, 6, 7, 10, 7, 1, 1, 7, 3, -1, -1, -1, -1, -1, -1, -1},
        {1, 2, 6, 1, 6, 8, 1, 8, 9, 8, 6, 7, -1, -1, -1, -1},
        {2, 6, 9, 2, 9, 1, 6, 7, 9, 0, 9, 3, 7, 3, 9, -1},
        {7, 8, 0, 7, 0, 6, 6, 0, 2, -1, -1, -1, -1, -1, -1, -1},
        {7, 3, 2, 6, 7, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {2, 3, 11, 10, 6, 8, 10, 8, 9, 8, 6, 7, -1, -1, -1, -1},
        {2, 0, 7, 2, 7, 11, 0, 9, 7, 6, 7, 10, 9, 10, 7, -1},
        {1, 8, 0, 1, 7, 8, 1, 10, 7, 6, 7, 10, 2, 3, 11, -1},
        {11, 2, 1, 11, 1, 7, 10, 6, 1, 6, 7, 1, -1, -1, -1, -1},
        {8, 9, 6, 8, 6, 7, 9, 1, 6, 11, 6, 3, 1, 3, 6, -1},
        {0, 9, 1, 11, 6, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {7, 8, 0, 7, 0, 6, 3, 11, 0, 11, 6, 0, -1, -1, -1, -1},
        {7, 11, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {3, 0, 8, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 1, 9, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {8, 1, 9, 8, 3, 1, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1},
        {10, 1, 2, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 2, 10, 3, 0, 8, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1},
        {2, 9, 0, 2, 10, 9, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1},
        {6, 11, 7, 2, 10, 3, 10, 8, 3, 10, 9, 8, -1, -1, -1, -1},
        {7, 2, 3, 6, 2, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {7, 0, 8, 7, 6, 0, 6, 2, 0, -1, -1, -1, -1, -1, -1, -1},
        {2, 7, 6, 2, 3, 7, 0, 1, 9, -1, -1, -1, -1, -1, -1, -1},
        {1, 6, 2, 1, 8, 6, 1, 9, 8, 8, 7, 6, -1, -1, -1, -1},
        {10, 7, 6, 10, 1, 7, 1, 3, 7, -1, -1, -1, -1, -1, -1, -1},
        {10, 7, 6, 1, 7, 10, 1, 8, 7, 1, 0, 8, -1, -1, -1, -1},
        {0, 3, 7, 0, 7, 10, 0, 10, 9, 6, 10, 7, -1, -1, -1, -1},
        {7, 6, 10, 7, 10, 8, 8, 10, 9, -1, -1, -1, -1, -1, -1, -1},
        {6, 8, 4, 11, 8, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {3, 6, 11, 3, 0, 6, 0, 4, 6, -1, -1, -1, -1, -1, -1, -1},
        {8, 6, 11, 8, 4, 6, 9, 0, 1, -1, -1, -1, -1, -1, -1, -1},
        {9, 4, 6, 9, 6, 3, 9, 3, 1, 11, 3, 6, -1, -1, -1, -1},
        {6, 8, 4, 6, 11, 8, 2, 10, 1, -1, -1, -1, -1, -1, -1, -1},
        {1, 2, 10, 3, 0, 11, 0, 6, 11, 0, 4, 6, -1, -1, -1, -1},
        {4, 11, 8, 4, 6, 11, 0, 2, 9, 2, 10, 9, -1, -1, -1, -1},
        {10, 9, 3, 10, 3, 2, 9, 4, 3, 11, 3, 6, 4, 6, 3, -1},
        {8, 2, 3, 8, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1},
        {0, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 9, 0, 2, 3, 4, 2, 4, 6, 4, 3, 8, -1, -1, -1, -1},
        {1, 9, 4, 1, 4, 2, 2, 4, 6, -1, -1, -1, -1, -1, -1, -1},
        {8, 1, 3, 8, 6, 1, 8, 4, 6, 6, 10, 1, -1, -1, -1, -1},
        {10, 1, 0, 10, 0, 6, 6, 0, 4, -1, -1, -1, -1, -1, -1, -1},
        {4, 6, 3, 4, 3, 8, 6, 10, 3, 0, 3, 9, 10, 9, 3, -1},
        {10, 9, 4, 6, 10, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {4, 9, 5, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 8, 3, 4, 9, 5, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1},
        {5, 0, 1, 5, 4, 0, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1},
        {11, 7, 6, 8, 3, 4, 3, 5, 4, 3, 1, 5, -1, -1, -1, -1},
        {9, 5, 4, 10, 1, 2, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1},
        {6, 11, 7, 1, 2, 10, 0, 8, 3, 4, 9, 5, -1, -1, -1, -1},
        {7, 6, 11, 5, 4, 10, 4, 2, 10, 4, 0, 2, -1, -1, -1, -1},
        {3, 4, 8, 3, 5, 4, 3, 2, 5, 10, 5, 2, 11, 7, 6, -1},
        {7, 2, 3, 7, 6, 2, 5, 4, 9, -1, -1, -1, -1, -1, -1, -1},
        {9, 5, 4, 0, 8, 6, 0, 6, 2, 6, 8, 7, -1, -1, -1, -1},
        {3, 6, 2, 3, 7, 6, 1, 5, 0, 5, 4, 0, -1, -1, -1, -1},
        {6, 2, 8, 6, 8, 7, 2, 1, 8, 4, 8, 5, 1, 5, 8, -1},
        {9, 5, 4, 10, 1, 6, 1, 7, 6, 1, 3, 7, -1, -1, -1, -1},
        {1, 6, 10, 1, 7, 6, 1, 0, 7, 8, 7, 0, 9, 5, 4, -1},
        {4, 0, 10, 4, 10, 5, 0, 3, 10, 6, 10, 7, 3, 7, 10, -1},
        {7, 6, 10, 7, 10, 8, 5, 4, 10, 4, 8, 10, -1, -1, -1, -1},
        {6, 9, 5, 6, 11, 9, 11, 8, 9, -1, -1, -1, -1, -1, -1, -1},
        {3, 6, 11, 0, 6, 3, 0, 5, 6, 0, 9, 5, -1, -1, -1, -1},
        {0, 11, 8, 0, 5, 11, 0, 1, 5, 5, 6, 11, -1, -1, -1, -1},
        {6, 11, 3, 6, 3, 5, 5, 3, 1, -1, -1, -1, -1, -1, -1, -1},
        {1, 2, 10, 9, 5, 11, 9, 11, 8, 11, 5, 6, -1, -1, -1, -1},
        {0, 11, 3, 0, 6, 11, 0, 9, 6, 5, 6, 9, 1, 2, 10, -1},
        {11, 8, 5, 11, 5, 6, 8, 0, 5, 10, 5, 2, 0, 2, 5, -1},
        {6, 11, 3, 6, 3, 5, 2, 10, 3, 10, 5, 3, -1, -1, -1, -1},
        {5, 8, 9, 5, 2, 8, 5, 6, 2, 3, 8, 2, -1, -1, -1, -1},
        {9, 5, 6, 9, 6, 0, 0, 6, 2, -1, -1, -1, -1, -1, -1, -1},
        {1, 5, 8, 1, 8, 0, 5, 6, 8, 3, 8, 2, 6, 2, 8, -1},
        {1, 5, 6, 2, 1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 3, 6, 1, 6, 10, 3, 8, 6, 5, 6, 9, 8, 9, 6, -1},
        {10, 1, 0, 10, 0, 6, 9, 5, 0, 5, 6, 0, -1, -1, -1, -1},
        {0, 3, 8, 5, 6, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {10, 5, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {11, 5, 10, 7, 5, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {11, 5, 10, 11, 7, 5, 8, 3, 0, -1, -1, -1, -1, -1, -1, -1},
        {5, 11, 7, 5, 10, 11, 1, 9, 0, -1, -1, -1, -1, -1, -1, -1},
        {10, 7, 5, 10, 11, 7, 9, 8, 1, 8, 3, 1, -1, -1, -1, -1},
        {11, 1, 2, 11, 7, 1, 7, 5, 1, -1, -1, -1, -1, -1, -1, -1},
        {0, 8, 3, 1, 2, 7, 1, 7, 5, 7, 2, 11, -1, -1, -1, -1},
        {9, 7, 5, 9, 2, 7, 9, 0, 2, 2, 11, 7, -1, -1, -1, -1},
        {7, 5, 2, 7, 2, 11, 5, 9, 2, 3, 2, 8, 9, 8, 2, -1},
        {2, 5, 10, 2, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1},
        {8, 2, 0, 8, 5, 2, 8, 7, 5, 10, 2, 5, -1, -1, -1, -1},
        {9, 0, 1, 5, 10, 3, 5, 3, 7, 3, 10, 2, -1, -1, -1, -1},
        {9, 8, 2, 9, 2, 1, 8, 7, 2, 10, 2, 5, 7, 5, 2, -1},
        {1, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 8, 7, 0, 7, 1, 1, 7, 5, -1, -1, -1, -1, -1, -1, -1},
        {9, 0, 3, 9, 3, 5, 5, 3, 7, -1, -1, -1, -1, -1, -1, -1},
        {9, 8, 7, 5, 9, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {5, 8, 4, 5, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1},
        {5, 0, 4, 5, 11, 0, 5, 10, 11, 11, 3, 0, -1, -1, -1, -1},
        {0, 1, 9, 8, 4, 10, 8, 10, 11, 10, 4, 5, -1, -1, -1, -1},
        {10, 11, 4, 10, 4, 5, 11, 3, 4, 9, 4, 1, 3, 1, 4, -1},
        {2, 5, 1, 2, 8, 5, 2, 11, 8, 4, 5, 8, -1, -1, -1, -1},
        {0, 4, 11, 0, 11, 3, 4, 5, 11, 2, 11, 1, 5, 1, 11, -1},
        {0, 2, 5, 0, 5, 9, 2, 11, 5, 4, 5, 8, 11, 8, 5, -1},
        {9, 4, 5, 2, 11, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {2, 5, 10, 3, 5, 2, 3, 4, 5, 3, 8, 4, -1, -1, -1, -1},
        {5, 10, 2, 5, 2, 4, 4, 2, 0, -1, -1, -1, -1, -1, -1, -1},
        {3, 10, 2, 3, 5, 10, 3, 8, 5, 4, 5, 8, 0, 1, 9, -1},
        {5, 10, 2, 5, 2, 4, 1, 9, 2, 9, 4, 2, -1, -1, -1, -1},
        {8, 4, 5, 8, 5, 3, 3, 5, 1, -1, -1, -1, -1, -1, -1, -1},
        {0, 4, 5, 1, 0, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {8, 4, 5, 8, 5, 3, 9, 0, 5, 0, 3, 5, -1, -1, -1, -1},
        {9, 4, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {4, 11, 7, 4, 9, 11, 9, 10, 11, -1, -1, -1, -1, -1, -1, -1},
        {0, 8, 3, 4, 9, 7, 9, 11, 7, 9, 10, 11, -1, -1, -1, -1},
        {1, 10, 11, 1, 11, 4, 1, 4, 0, 7, 4, 11, -1, -1, -1, -1},
        {3, 1, 4, 3, 4, 8, 1, 10, 4, 7, 4, 11, 10, 11, 4, -1},
        {4, 11, 7, 9, 11, 4, 9, 2, 11, 9, 1, 2, -1, -1, -1, -1},
        {9, 7, 4, 9, 11, 7, 9, 1, 11, 2, 11, 1, 0, 8, 3, -1},
        {11, 7, 4, 11, 4, 2, 2, 4, 0, -1, -1, -1, -1, -1, -1, -1},
        {11, 7, 4, 11, 4, 2, 8, 3, 4, 3, 2, 4, -1, -1, -1, -1},
        {2, 9, 10, 2, 7, 9, 2, 3, 7, 7, 4, 9, -1, -1, -1, -1},
        {9, 10, 7, 9, 7, 4, 10, 2, 7, 8, 7, 0, 2, 0, 7, -1},
        {3, 7, 10, 3, 10, 2, 7, 4, 10, 1, 10, 0, 4, 0, 10, -1},
        {1, 10, 2, 8, 7, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {4, 9, 1, 4, 1, 7, 7, 1, 3, -1, -1, -1, -1, -1, -1, -1},
        {4, 9, 1, 4, 1, 7, 0, 8, 1, 8, 7, 1, -1, -1, -1, -1},
        {4, 0, 3, 7, 4, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {4, 8, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {9, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {3, 0, 9, 3, 9, 11, 11, 9, 10, -1, -1, -1, -1, -1, -1, -1},
        {0, 1, 10, 0, 10, 8, 8, 10, 11, -1, -1, -1, -1, -1, -1, -1},
        {3, 1, 10, 11, 3, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 2, 11, 1, 11, 9, 9, 11, 8, -1, -1, -1, -1, -1, -1, -1},
        {3, 0, 9, 3, 9, 11, 1, 2, 9, 2, 11, 9, -1, -1, -1, -1},
        {0, 2, 11, 8, 0, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {3, 2, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {2, 3, 8, 2, 8, 10, 10, 8, 9, -1, -1, -1, -1, -1, -1, -1},
        {9, 10, 2, 0, 9, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {2, 3, 8, 2, 8, 10, 0, 1, 8, 1, 10, 8, -1, -1, -1, -1},
        {1, 10, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 3, 8, 9, 1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 3, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}
    };
}