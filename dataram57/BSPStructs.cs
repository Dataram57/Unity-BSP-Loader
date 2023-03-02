using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dataram57.BSPImporter
{
    public struct BSPHeader
    {
        public int version,
        entities_ofs, entities_size,
        planes_ofs, planes_size,
        miptex_ofs, miptex_size,
        verts_ofs, verts_size,
        visilist_ofs, visilist_size,
        nodes_ofs, nodes_size,
        texinfo_ofs, texinfo_size,
        faces_ofs, faces_size,
        lightmaps_ofs, lightmaps_size,
        clipnodes_ofs, clipnodes_size,
        leaves_ofs, leaves_size,
        lface_ofs, lface_size,
        edges_ofs, edges_size,
        ledges_ofs, ledges_size,
        models_ofs, models_size;
    }

    public struct BSPModel
    {
        public float
        bbox_min_x, bbox_min_y, bbox_min_z,
        bbox_max_x, bbox_max_y, bbox_max_z,
        origin_x, origin_y, origin_z;
        public uint
        node_id0, node_id1, node_id2, node_id3,
        numleafs,
        face_id,
        face_num;

        public static int GetSize() => 64;
    }

    public struct BSPVertex
    {
        public float x,y,z;
        public static int GetSize() => 12;
    }

    public struct BSPMipTex
    {
        public string name;
        public uint width, height, ofs1, ofs2, ofs4, ofs8;
        public static int GetSize() => 40;
    }

    //extended for BSP2
    public struct BSPEdge
    {
        public uint vertex0, vertex1;

        public static int GetSize(bool isBSP2)
        {
            if (isBSP2) return 8;
            return 4;
        }
    }

    //extended for BSP2
    public struct BSPFace
    {
        public uint plane_id;
        public uint size;
        public int ledge_id;
        public uint ledge_num;
        public uint texinfo_id;
        public char lighttype;
        public char lightlevel;
        public char light0, light1;
        public int lightmap;

        public static int GetSize(bool isBSP2)
        {
            if (isBSP2) return 28;
            return 20;
        }
    }

    public struct BSPTexInfo
    {
        public Vector3
        s,
        t;
        public float
        s_dist,
        t_dist;
        public uint
        texture_id,
        animated;

        public static int GetSize() => 40;
    }
}