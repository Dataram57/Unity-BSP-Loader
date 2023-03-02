using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Dataram57.BSPImporter
{
    public class BSPImporter : EditorWindow
    {
        public float scale=0.5f;

        //static BSPImporter window;    //unused
        [MenuItem("Tools/Dataram_57/BSP Importer")]
        public static void Open()
        {
            //window = (BSPImporter)GetWindow(typeof(BSPImporter));
            GetWindow(typeof(BSPImporter));
        }

        private void OnGUI()
        {
            GUILayout.Label("Import scale:");
            scale = EditorGUILayout.FloatField(scale);
            GUILayout.Label("Actions");
            if (GUILayout.Button("Import BSP"))
            {
                string path = EditorUtility.OpenFilePanel("Select BSP", "", "bsp");
                if (File.Exists(path))
                {
                    ImportBSP(path);
                }
            }
        }

        public void DevLog(string obj)
        {
            Debug.Log(obj);
        }

        public void Log(string obj)
        {
            Debug.Log(obj);
        }

        public void ImportBSP(string path)
        {
            //vars
            int i, f, g, h, j, k;
            uint I, F, G, H, J, K;
            string temp;

            //Main init
            FileStream stream = File.Open(path, FileMode.Open);
            if (stream == null)
                return;
            BinaryReader br = new BinaryReader(stream);

            //Read header
            BSPHeader bspHeader = br.ReadBSPHeader();
            
            //Is BSP2
            bool isBSP2 = bspHeader.version == 844124994;
            //Log version
            if (isBSP2)
                Log("Version: BSP2");
            else
                Log("Version: BSP");

            //Get more info about the map
            int num_models = bspHeader.models_size / BSPModel.GetSize();
            int num_verts = bspHeader.verts_size / BSPVertex.GetSize();
            int num_faces = bspHeader.faces_size / BSPFace.GetSize(isBSP2);
            int num_edges = bspHeader.edges_size / BSPEdge.GetSize(isBSP2);
            //Log
            Log("Number of models: " + num_models);
            Log("Number of vertices: " + num_verts);
            Log("Number of faces: " + num_faces);
            Log("Number of edges: " + num_edges);

            //Load texture names
            List<string> textures=new List<string>();
            br.SetOffset(bspHeader.miptex_ofs);
            int num_textures = br.ReadInt32();
            Log("Number of textures: " + num_textures);
            for (i = 0; i < num_textures; i++)
            {
                //go back to the list of data offsets
                br.SetOffset(bspHeader.miptex_ofs + 4 + i * 4);
                //read another data offset
                br.SetOffset(bspHeader.miptex_ofs + br.ReadInt32());
                //read texture
                BSPMipTex mipTex = br.ReadMipTex();
                DevLog("Texture: " + mipTex.name);
                textures.Add(mipTex.name);
            }

            //Read edges
            List<int> edges = new List<int>();
            br.SetOffset(bspHeader.ledges_ofs); // no ledges are not edges(BSPEdges)
            f = bspHeader.ledges_size / 4;
            for (i = 0; i < f; i++)
                edges.Add(br.ReadInt32());

            //Read vertices
            List<Vector3> verticies = new List<Vector3>();
            br.SetOffset(bspHeader.verts_ofs);
            for (i = 0; i < num_verts; i++)
            {
                Vector3 v = br.ReadVector3();
                //90 deg rotation
                float y = v.z;
                v.z = v.y;
                v.y = y;

                v *= scale;
                verticies.Add(v);
            }

            //Load models
            for (i = 0; i < num_models; i++)
            {
                DevLog("Model " + i);
                //go back and pick another model
                br.SetOffset(bspHeader.models_ofs + BSPModel.GetSize() * i);
                //...
                BSPModel model = br.ReadBSPModel();
                List<uint> modelUsedVerticiesId = new List<uint>();
                List<uint[]> modelFaceVerticiesId = new List<uint[]>();
                List<BSPTexInfo> modelFaceTextureInfo = new List<BSPTexInfo>();
                List<uint> modelUsedTextureIds = new List<uint>(); //count tells how many submeshes will have to be made;
                //Load model faces
                for (f = 0; f < model.face_num; f++)
                {
                    DevLog("Face " + f);
                    //go back and pick another face of this model
                    br.SetOffset(bspHeader.faces_ofs + (model.face_id + f) * BSPFace.GetSize(isBSP2));
                    //...
                    BSPFace face = br.ReadBSPFace(isBSP2);
                    List<uint> faceVertexIds = new List<uint>();
                    //get texture info
                    br.SetOffset(bspHeader.texinfo_ofs + face.texinfo_id * BSPTexInfo.GetSize());
                    BSPTexInfo texInfo = br.ReadBSPTexInfo();
                    //register texture id in used
                    I = texInfo.texture_id;
                    for (g = modelUsedTextureIds.Count - 1; g > -1; g--)
                        if (modelUsedTextureIds[g] == I)
                            break;
                    if (g == -1)
                        modelUsedTextureIds.Add(I);
                    //...
                    DevLog("Texture " + textures[(int)texInfo.texture_id]);
                    //What verticies are being used in this face
                    DevLog("Vertex start " + face.ledge_id);
                    for (g = 0; g < face.ledge_num; g++)
                    {
                        //Calc index of the edge
                        h = edges[face.ledge_id + g];
                        //offset
                        j = h * BSPEdge.GetSize(isBSP2);
                        if (h < 0) j *= -1;
                        //get edge info
                        br.SetOffset(bspHeader.edges_ofs + j);
                        BSPEdge edge = br.ReadBSPEdge(isBSP2);
                        //get vertex used
                        //I - index/id of the vertex that is being used in this face
                        if (h < 0) I = edge.vertex1;
                        else I = edge.vertex0;
                        //register used vertex
                        for (h = modelUsedVerticiesId.Count - 1; h > -1; h--)
                            if (modelUsedVerticiesId[h] == I)
                                break;
                        if (h == -1)
                        {
                            modelUsedVerticiesId.Add(I);
                            h = modelUsedVerticiesId.Count - 1;
                        }
                        //add id of the vertex to the face vertex ids.
                        faceVertexIds.Add((uint)h);
                        //...
                        DevLog("Vertex " + g + " " + verticies[(int)I]);
                    }
                    //add this face to the model faces
                    modelFaceVerticiesId.Add(faceVertexIds.ToArray());
                    modelFaceTextureInfo.Add(texInfo);
                }
                //create mesh for the model
                Mesh mesh = new Mesh();
                //convert array of vertex ids into an array of vector3
                Vector3[] modelVerticies = new Vector3[modelUsedVerticiesId.Count];
                for (f = modelUsedVerticiesId.Count - 1; f > -1; f--)
                    modelVerticies[f] = verticies[(int)modelUsedVerticiesId[f]];
                modelUsedVerticiesId.Clear();
                modelUsedVerticiesId = null;
                //each face add into specific submesh
                //vars
                List<Vector3> meshVerticies = new List<Vector3>();
                List<int>[] subMeshTriangles = new List<int>[modelUsedTextureIds.Count];
                for (f = subMeshTriangles.Length - 1; f > -1; f--)
                    subMeshTriangles[f] = new List<int>();
                //loop
                for (f = modelFaceTextureInfo.Count - 1; f > -1; f--)
                {
                    //get texture info
                    BSPTexInfo texInfo = modelFaceTextureInfo[f];
                    //find index submesh of this textureid
                    I = texInfo.texture_id;
                    for (g = modelUsedTextureIds.Count - 1; g > -1; g--)
                        if (modelUsedTextureIds[g] == I)
                            break;
                    if (g == -1)
                    {
                        Log("Something really weird has just happend: " + texInfo.texture_id);
                        string s = "";
                        foreach (uint b in modelUsedTextureIds)
                            s += b.ToString() + " ";
                        //Log(s);

                        return;
                    }
                    //add face to submesh of index `g`
                    h = meshVerticies.Count; //can be optimised
                    k = 0;
                    //add triangles(polygon out of triangles)
                    for(j = modelFaceVerticiesId[f].Length - 2; j > 0; j--)
                    {
                        subMeshTriangles[g].Add(h);
                        subMeshTriangles[g].Add(h + k + 1);
                        subMeshTriangles[g].Add(h + k + 2);
                        k++;
                    }
                    //add vertexes
                    uint[] faceVerts = modelFaceVerticiesId[f];
                    foreach (uint V in faceVerts)
                        meshVerticies.Add(verticies[(int)V]);
                    //h++
                }
                //assign data to mesh
                mesh.SetVertices(meshVerticies);
                mesh.subMeshCount = subMeshTriangles.Length;
                for (f = subMeshTriangles.Length - 1; f > -1; f--)
                {
                    mesh.SetTriangles(subMeshTriangles[f].ToArray(), f);
                }
                mesh.RecalculateNormals();
                mesh.RecalculateTangents();
                mesh.Optimize();
                //create GameObject
                GameObject go = new GameObject();
                //assign mesh
                MeshFilter mf=go.AddComponent<MeshFilter>();
                mf.mesh = mesh;
                //allow render
                MeshRenderer mr = go.AddComponent<MeshRenderer>();
                mr.materials = new Material[subMeshTriangles.Length];
            }

            //Close
            br.Close();
            stream.Close();
        }
    }
}
