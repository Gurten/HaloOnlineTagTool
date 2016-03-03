using HaloOnlineTagTool.Commands;
using HaloOnlineTagTool.TagStructures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSP = HaloOnlineTagTool.TagStructures.CollisionModel.Region.Permutation.Bsp;

namespace HaloOnlineTagTool.Resources.Geometry
{
    /// <summary>
    /// A group of utilities for BSP data 
    /// </summary>
    class BSPUtils
    {

        /// <summary>
        /// Creates a CollisionModel bsp from a Scenario StructureBsp.
        /// This does not work for sbsps with > 65536 planes, which use a
        ///  larger encoding for their bsp related structs differing from the
        ///  struct used in common with collision model bsps.
        /// </summary>
        /// <returns></returns>
        public static BSP fromSbsp(ScenarioStructureBsp sbsp, OpenTagCache info)
        {
            // Need to work out how to do that class attribute enumeration thing
            // so all of this folds down to < 10 lines
            BSP bsp = fromSbspInit(sbsp);
            var resource = sbsp.Resource3;
            var resourceManager = new ResourceDataManager();
            try
            {
                resourceManager.LoadCachesFromDirectory(info.CacheFile.DirectoryName);
            }
            catch
            {
                Console.WriteLine("Unable to load the resource .dat files.");
                Console.WriteLine("Make sure that they all exist and are valid.");
            }

            //Create a binary reader for the resource
            Stream stream = new MemoryStream();
            resourceManager.Extract(sbsp.Resource3, stream);
            BinaryReader reader = new BinaryReader(stream);
            reader.BaseStream.Position = 0;
            Console.WriteLine("Stream position: {0:X8}", reader.BaseStream.Position);
            for (int i = 0; i < bsp.Bsp3dNodes.Count; ++i)
            {
                BSP.Bsp3dNode node = new BSP.Bsp3dNode();
                node.Plane = reader.ReadInt16();
                node.BackChildLower = reader.ReadByte();
                node.BackChildMid = reader.ReadByte();
                node.BackChildUpper = reader.ReadByte();
                node.FrontChildLower = reader.ReadByte();
                node.FrontChildMid = reader.ReadByte();
                node.FrontChildUpper = reader.ReadByte();
                bsp.Bsp3dNodes[i] = node;
            }

            //Align to the next multiple of 16
            reader.BaseStream.Position = -((-reader.BaseStream.Position) & ~0xf);
            Console.WriteLine("Stream position: {0:X8}", reader.BaseStream.Position);
            for (int i = 0; i < bsp.Planes.Count; ++i)
            {
                BSP.Plane plane = new BSP.Plane();
                plane.PlaneI = reader.ReadSingle();
                plane.PlaneJ = reader.ReadSingle();
                plane.PlaneK = reader.ReadSingle();
                plane.PlaneD = reader.ReadSingle();
                bsp.Planes[i] = plane;
            }

            //Put here for consistency
            reader.BaseStream.Position = -((-reader.BaseStream.Position) & ~0xf);
            Console.WriteLine("Stream position: {0:X8}", reader.BaseStream.Position);
            for (int i = 0; i < bsp.Leaves.Count; ++i)
            {
                BSP.Leaf leaf = new BSP.Leaf();
                leaf.Flags = reader.ReadInt16();
                leaf.Bsp2dReferenceCount = reader.ReadInt16();
                leaf.Unknown = reader.ReadInt16();
                leaf.FirstBsp2dReference = reader.ReadInt16();
                bsp.Leaves[i] = leaf;
            }

            //Align to the next multiple of 16
            reader.BaseStream.Position = -((-reader.BaseStream.Position) & ~0xf);
            Console.WriteLine("Stream position: {0:X8}", reader.BaseStream.Position);
            for (int i = 0; i < bsp.Bsp2dReferences.Count; ++i)
            {
                BSP.Bsp2dReference bsp2dref = new BSP.Bsp2dReference();
                bsp2dref.Plane = reader.ReadInt16();
                bsp2dref.Bsp2dNode = reader.ReadInt16();
                bsp.Bsp2dReferences[i] = bsp2dref;
            }

            //Align to the next multiple of 16
            reader.BaseStream.Position = -((-reader.BaseStream.Position) & ~0xf);
            Console.WriteLine("Stream position: {0:X8}", reader.BaseStream.Position);
            for (int i = 0; i < bsp.Bsp2dNodes.Count; ++i)
            {
                BSP.Bsp2dNode node = new BSP.Bsp2dNode();
                node.PlaneI = reader.ReadSingle();
                node.PlaneJ = reader.ReadSingle();
                node.PlaneD = reader.ReadSingle();
                node.LeftChild = reader.ReadInt16();
                node.RightChild = reader.ReadInt16();
                bsp.Bsp2dNodes[i] = node;
            }

            //Put here for consistency
            reader.BaseStream.Position = -((-reader.BaseStream.Position) & ~0xf);
            Console.WriteLine("Stream position: {0:X8}", reader.BaseStream.Position);
            for (int i = 0; i < bsp.Surfaces.Count; ++i)
            {
                BSP.Surface surface = new BSP.Surface();
                surface.Plane = reader.ReadUInt16();
                surface.FirstEdge = reader.ReadUInt16();
                surface.Material = reader.ReadInt16();
                surface.Unknown = reader.ReadInt16();
                surface.BreakableSurface = reader.ReadInt16();
                surface.Unknown2 = reader.ReadInt16();
                bsp.Surfaces[i] = surface;
            }

            //Align to the next multiple of 16
            reader.BaseStream.Position = -((-reader.BaseStream.Position) & ~0xf);
            Console.WriteLine("Stream position: {0:X8}", reader.BaseStream.Position);
            for (int i = 0; i < bsp.Edges.Count; ++i)
            {
                BSP.Edge edge = new BSP.Edge();
                edge.StartVertex = reader.ReadUInt16();
                edge.EndVertex = reader.ReadUInt16();
                edge.ForwardEdge = reader.ReadUInt16();
                edge.ReverseEdge = reader.ReadUInt16();
                edge.LeftSurface = reader.ReadUInt16();
                edge.RightSurface = reader.ReadUInt16();
                bsp.Edges[i] = edge;
            }

            //Align to the next multiple of 16
            reader.BaseStream.Position = -((-reader.BaseStream.Position) & ~0xf);
            Console.WriteLine("Stream position: {0:X8}", reader.BaseStream.Position);
            for (int i = 0; i < bsp.Vertices.Count; ++i)
            {
                BSP.Vertex vert = new BSP.Vertex();
                vert.PointX = reader.ReadSingle();
                vert.PointY = reader.ReadSingle();
                vert.PointZ = reader.ReadSingle();
                vert.FirstEdge = reader.ReadInt16();
                vert.Unknown = reader.ReadInt16();
                bsp.Vertices[i] = vert;
            }

            return bsp;
        }


        /// <summary>
        /// Uses the winged edge adjacency model of the BSP
        /// to output a visual representation as an OBJ file.
        /// </summary>
        public static void toOBJ(BSP bsp, string fpath)
        {
            using (var objFile = new StreamWriter(File.Open(fpath, FileMode.Create, FileAccess.Write)))
            {

                List<Tuple<int, int, int>> triples = new List<Tuple<int, int, int>>();
                for (int i = 0; i < bsp.Surfaces.Count; ++i)
                {
                    List<ushort> loop = vertLoopFromSurface(i, bsp);
                    if (loop == null)
                    {
                        Console.WriteLine("Failed to get loop for surface {0}.", i);
                        return;
                    }
                    if (loop.Count == 0)
                    {
                        Console.WriteLine("Failed to get loop for surface {0}. Skipping.", i);
                    }

                    //Below is how to triangulate an n-gon where the first vertex index
                    // is also the last. If the last index was not the first, one more
                    // loop would have to occur (i.e  condition: 'j < loop.Count -2') 
                    for (int j = 0; j < loop.Count - 3; ++j)
                    {
                        //Tuple indices are ordered so that the surfaces face outward
                        triples.Add(new Tuple<int, int, int>(loop[j + 1], loop[0], loop[j + 2]));
                    }
                }

                objFile.WriteLine("o bsp");
                foreach(BSP.Vertex v in bsp.Vertices)
                {
                    objFile.WriteLine("v {0} {1} {2}", v.PointX, v.PointY, v.PointZ);
                }
                objFile.WriteLine("s off");

                foreach (Tuple<int, int, int> f in triples)
                {
                    //obj files begin numbering at 1 for vertices. Must add 1 to each index
                    objFile.WriteLine("f {0} {1} {2}", f.Item1+1, f.Item2+1, f.Item3+1);
                }

                objFile.Close();
            }
        }

        /// <summary>
        /// Gets the vertex indices for a polygon of a surface of a BSP
        /// </summary>
        /// <param name="surface_idx"></param>
        /// <param name="edges"></param>
        /// <param name="current_edge_idx"></param>
        /// <returns></returns>
        private static List<ushort> vertLoopFromSurface(int surface_idx, BSP bsp)
        {
            //Variables
            ushort current_edge_idx = bsp.Surfaces[surface_idx].FirstEdge;

            //Get the indexed 'first edge' of the surface
            BSP.Edge edge = bsp.Edges[current_edge_idx];
            //A loop will have been completed when the first edge is encountered again
            BSP.Edge first_edge = edge;
            //The indices of the n-gon's loop of vertex 
            List<ushort> loop_indices = new List<ushort>();
            //The last vertex index encountered 
            int last_vertex_idx;

            //A special case for the first edge- must add start and end vertices

            //When the current surface is to the right of the edge then moving from
            // the start-vertex to the end-vertex is done automatically in a clockwise 
            // order. 
            if (surface_idx == edge.RightSurface)
            {
                loop_indices.Add(edge.StartVertex);
                loop_indices.Add(edge.EndVertex);
                last_vertex_idx = edge.EndVertex;
                edge = bsp.Edges[edge.ForwardEdge];
            }
            else
            //However if the current surface is to the left of the edge then it is 
            // anti-clockwise and the ordering must be changed manually.
            {
                loop_indices.Add(edge.EndVertex);
                loop_indices.Add(edge.StartVertex);
                last_vertex_idx = edge.StartVertex;
                edge = bsp.Edges[edge.ReverseEdge];
            }
            
            //Complete a traversal of edges around a surface to get the indices
            // of all the vertices. This will make a n-gon that can be reduced
            // to tris.
            while (edge != first_edge)
            {
                /*
                if (edge.ForwardEdge >= bsp.Edges.Count || edge.ForwardEdge < 0
                    || edge.ReverseEdge >= bsp.Edges.Count || edge.ReverseEdge < 0)
                {
                    Console.WriteLine("Degenerate edge detected:\nForward: {0}, Reverse: {1}.", edge.ForwardEdge, edge.ReverseEdge);

                    if (edge.ReverseEdge < 0)
                        edge.ReverseEdge = (short)-edge.ReverseEdge;

                    if (edge.ForwardEdge < 0)
                        edge.ForwardEdge = (short)-edge.ForwardEdge;
                }
                */
                if (edge.RightSurface == surface_idx)
                {
                    loop_indices.Add(edge.EndVertex);
                    last_vertex_idx = edge.EndVertex;
                    edge = bsp.Edges[edge.ForwardEdge];
                }
                else if (edge.LeftSurface == surface_idx)
                {
                    loop_indices.Add(edge.StartVertex);
                    last_vertex_idx = edge.StartVertex;
                    edge = bsp.Edges[edge.ReverseEdge];
                }
                else {
                    if (edge.StartVertex == last_vertex_idx)
                    {
                        edge = bsp.Edges[edge.ReverseEdge]; //the previous edge must have ended on the vertex
                    }
                    else if (edge.EndVertex == last_vertex_idx)
                    {
                        edge = bsp.Edges[edge.ForwardEdge]; //the next edge must begin on the vertex
                    }
                    else {
                        //Neither are the last vertex index. This means 
                        // the traversal went off the loop mysteriously,
                        // perhaps due to incorrect data.
                        //Console.WriteLine("'vertLoopFromSurface' traversal left edge loop with no way back");
                        return loop_indices;
                    }
                }
            } 
            return loop_indices;
        }


        /// <summary>
        /// Initialises a BSP with the correct number of elements for each
        /// of the eight lists.
        /// </summary>
        /// <param name="sbsp"></param>
        /// <returns></returns>
        private static BSP fromSbspInit(ScenarioStructureBsp sbsp)
        {
            BSP bsp = new BSP();
            //Resource 3 of the sbsp tag has bsp data 
            var resource = sbsp.Resource3;
            BinaryReader rsrcDef = new BinaryReader(new MemoryStream(resource.DefinitionData));

            //The position in the resource definition for the number of bsp3dnodes
            rsrcDef.BaseStream.Position = 0;
            //set initial size (not capacity) to the number read from the reader
            bsp.Bsp3dNodes = new List<BSP.Bsp3dNode>(new BSP.Bsp3dNode[rsrcDef.ReadInt32()]);
            Console.WriteLine("{0} Bsp3dNodes", bsp.Bsp3dNodes.Count);

            //Position for number of planes
            rsrcDef.BaseStream.Position = 12;
            bsp.Planes = new List<BSP.Plane>(new BSP.Plane[rsrcDef.ReadInt32()]);
            Console.WriteLine("{0} Planes", bsp.Planes.Count);


            //Position for number of leaves
            rsrcDef.BaseStream.Position = 24;
            bsp.Leaves = new List<BSP.Leaf>(new BSP.Leaf[rsrcDef.ReadInt32()]);
            Console.WriteLine("{0} Leaves", bsp.Leaves.Count);

            //Position for number of bsp2dreferences
            rsrcDef.BaseStream.Position = 36;
            bsp.Bsp2dReferences = new List<BSP.Bsp2dReference>(new BSP.Bsp2dReference[rsrcDef.ReadInt32()]);
            Console.WriteLine("{0} Bsp2dReferences", bsp.Bsp2dReferences.Count);

            //Position for number of bsp2dnodes
            rsrcDef.BaseStream.Position = 48;
            bsp.Bsp2dNodes = new List<BSP.Bsp2dNode>(new BSP.Bsp2dNode[rsrcDef.ReadInt32()]);
            Console.WriteLine("{0} Bsp2dNodes", bsp.Bsp2dNodes.Count);

            //Position for number of surfaces
            rsrcDef.BaseStream.Position = 60;
            bsp.Surfaces = new List<BSP.Surface>(new BSP.Surface[rsrcDef.ReadInt32()]);
            Console.WriteLine("{0} Surfaces", bsp.Surfaces.Count);

            //Position for number of edges
            rsrcDef.BaseStream.Position = 72;
            bsp.Edges = new List<BSP.Edge>(new BSP.Edge[rsrcDef.ReadInt32()]);
            Console.WriteLine("{0} Edges", bsp.Edges.Count);

            //Position for number of vertices
            rsrcDef.BaseStream.Position = 84;
            bsp.Vertices = new List<BSP.Vertex>(new BSP.Vertex[rsrcDef.ReadInt32()]);
            Console.WriteLine("{0} Vertices", bsp.Vertices.Count);

            return bsp;
        }

    }
}
