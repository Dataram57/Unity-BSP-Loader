using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Dataram57.BSPImporter
{
    public static class BinaryReaderExtensions
    {
        public static void SetOffset(this BinaryReader br, long offset) => br.BaseStream.Seek(offset, SeekOrigin.Begin);

        public static byte[] ReadBuffer(this BinaryReader br, long offset, int size)
        {
            br.BaseStream.Seek(offset, SeekOrigin.Begin);
            return br.ReadBytes(size);
        }

        public static Vector3 ReadVector3(this BinaryReader br)
        {
            Vector3 v = new Vector3();
            v.x = br.ReadSingle();
            v.y = br.ReadSingle();
            v.z = br.ReadSingle();
            return v;
        }

        public static BSPHeader ReadBSPHeader(this BinaryReader br)
        {
            BSPHeader header = new BSPHeader();
            header.version = br.ReadInt32();
            header.entities_ofs = br.ReadInt32();
            header.entities_size = br.ReadInt32();
            header.planes_ofs = br.ReadInt32();
            header.planes_size = br.ReadInt32();
            header.miptex_ofs = br.ReadInt32();
            header.miptex_size = br.ReadInt32();
            header.verts_ofs = br.ReadInt32();
            header.verts_size = br.ReadInt32();
            header.visilist_ofs = br.ReadInt32();
            header.visilist_size = br.ReadInt32();
            header.nodes_ofs = br.ReadInt32();
            header.nodes_size = br.ReadInt32();
            header.texinfo_ofs = br.ReadInt32();
            header.texinfo_size = br.ReadInt32();
            header.faces_ofs = br.ReadInt32();
            header.faces_size = br.ReadInt32();
            header.lightmaps_ofs = br.ReadInt32();
            header.lightmaps_size = br.ReadInt32();
            header.clipnodes_ofs = br.ReadInt32();
            header.clipnodes_size = br.ReadInt32();
            header.leaves_ofs = br.ReadInt32();
            header.leaves_size = br.ReadInt32();
            header.lface_ofs = br.ReadInt32();
            header.lface_size = br.ReadInt32();
            header.edges_ofs = br.ReadInt32();
            header.edges_size = br.ReadInt32();
            header.ledges_ofs = br.ReadInt32();
            header.ledges_size = br.ReadInt32();
            header.models_ofs = br.ReadInt32();
            header.models_size = br.ReadInt32();
            return header;
        }

        public static BSPMipTex ReadMipTex(this BinaryReader br)
        {
            BSPMipTex mipTex = new BSPMipTex();
            mipTex.name = System.Text.Encoding.UTF8.GetString(br.ReadBytes(16)).Replace("\0", "");
            mipTex.width = br.ReadUInt32();
            mipTex.height = br.ReadUInt32();
            mipTex.ofs1 = br.ReadUInt32();
            mipTex.ofs2 = br.ReadUInt32();
            mipTex.ofs4 = br.ReadUInt32();
            mipTex.ofs8 = br.ReadUInt32();
            return mipTex;
        }

        public static BSPModel ReadBSPModel(this BinaryReader br)
        {
            BSPModel model = new BSPModel();
            model.bbox_min_x = br.ReadSingle();
            model.bbox_min_y = br.ReadSingle();
            model.bbox_min_z = br.ReadSingle();
            model.bbox_max_x = br.ReadSingle();
            model.bbox_max_y = br.ReadSingle();
            model.bbox_max_z = br.ReadSingle();
            model.origin_x = br.ReadSingle();
            model.origin_y = br.ReadSingle();
            model.origin_z = br.ReadSingle();
            model.node_id0 = br.ReadUInt32();
            model.node_id1 = br.ReadUInt32();
            model.node_id2 = br.ReadUInt32();
            model.node_id3 = br.ReadUInt32();
            model.numleafs = br.ReadUInt32();
            model.face_id = br.ReadUInt32();
            model.face_num = br.ReadUInt32();
            return model;
        }

        public static BSPFace ReadBSPFace(this BinaryReader br,bool isBSP2)
        {
            BSPFace face = new BSPFace();
            if (isBSP2)
            {
                face.plane_id = br.ReadUInt32();
                face.size = br.ReadUInt32();
                face.ledge_id = br.ReadInt32();
                face.ledge_num = br.ReadUInt32();
                face.texinfo_id = br.ReadUInt32();
            }
            else
            {
                face.plane_id = br.ReadUInt16();
                face.size = br.ReadUInt16();
                face.ledge_id = br.ReadInt32();
                face.ledge_num = br.ReadUInt16();
                face.texinfo_id = br.ReadUInt16();
            }
            face.lighttype = br.ReadChar();
            face.lightlevel = br.ReadChar();
            face.light0 = br.ReadChar();
            face.light1 = br.ReadChar();
            face.lightmap = br.ReadInt32();
            return face;
        }

        public static BSPTexInfo ReadBSPTexInfo(this BinaryReader br)
        {
            BSPTexInfo texInfo = new BSPTexInfo();
            texInfo.s = br.ReadVector3();
            texInfo.s_dist = br.ReadSingle();
            texInfo.t = br.ReadVector3();
            texInfo.t_dist = br.ReadSingle();
            texInfo.texture_id = br.ReadUInt32();
            texInfo.animated = br.ReadUInt32();
            return texInfo;
        }

        public static BSPEdge ReadBSPEdge(this BinaryReader br, bool isBSP2)
        {
            BSPEdge edge = new BSPEdge();
            if (isBSP2)
            {
                edge.vertex0 = br.ReadUInt32();
                edge.vertex1 = br.ReadUInt32();
            }
            else
            {
                edge.vertex0 = br.ReadUInt16();
                edge.vertex1 = br.ReadUInt16();
            }
            return edge;
        }

    }
}
