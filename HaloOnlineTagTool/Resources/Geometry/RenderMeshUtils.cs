using HaloOnlineTagTool.TagStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloOnlineTagTool.Resources.Geometry
{ 

    class RenderModelUtils
    {
        /// <summary>
        /// Get an Assimp matrix for the mesh of a rendermesh.
        /// This is an approximation of the logic applied by the 
        ///  game to scaling the model using the assigned node.
        /// </summary>
        public static Assimp.Matrix4x4 getMatForMesh(RenderModel model, Mesh mesh)
        {
            var rot_mat = Assimp.Matrix3x3.Identity;
            int node_idx = mesh.RigidNodeIndex;
            if (node_idx < 0) {
                node_idx = 0;
            }
            if (node_idx < model.Nodes.Count)
            {
                float s = model.Nodes[node_idx].DefaultScale;
                RenderModel.Node node = model.Nodes[node_idx];
                rot_mat = new Assimp.Matrix3x3(node.InverseForward.X *s , node.InverseForward.Y * s, node.InverseForward.Z*s,
                                                    node.InverseLeft.X*s, node.InverseLeft.Y*s, node.InverseLeft.Z*s,
                                                    node.InverseUp.X*s, node.InverseUp.Y*s, node.InverseUp.Z*s);
                //Console.WriteLine("Loaded matrix: {0}", rot_mat);
                
            }

            var matrix_local = new Assimp.Matrix4x4(rot_mat);
            return matrix_local;
        }

    }
}
