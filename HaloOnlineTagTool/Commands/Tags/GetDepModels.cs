using HaloOnlineTagTool.Commands.Models;
using HaloOnlineTagTool.Resources;
using HaloOnlineTagTool.Resources.Geometry;
using HaloOnlineTagTool.Serialization;
using HaloOnlineTagTool.TagStructures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HaloOnlineTagTool.Commands
{
    class GetDepModels : Command
    {
        private readonly OpenTagCache _info;

        public GetDepModels(OpenTagCache info) : base(
            CommandFlags.None,
            "getdepmodels",
            "Extracts the render models of gameobjects referenced by a map.",
            "getdepmodels <map-file-path> <output-dir-path>",
            "Extract the render models of all game-objects referenced by a map." +
            "The map-file-path arg must be of an existing map (i.e in HaloOnline\\maps\\)"
        )
        {
            _info = info;
        }

        /// <summary>
        /// Finds the Model referenced by the tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public Model getModel(TagInstance tag)
        {
            if (tag == null)
                return null;

            using (var cacheStream = _info.CacheFile.Open(FileMode.Open, FileAccess.ReadWrite))
            {
                var tagContext = new TagSerializationContext(cacheStream, _info.Cache, _info.StringIds, tag);
                TagInstance model = null;
                GameObject obj = null;
                obj = (GameObject)_info.Deserializer.Deserialize(tagContext, TagStructureTypes.FindByGroupTag(tag.Group.ToString()));
                if (obj == null)
                {
                    Console.WriteLine("Could not get GameObject from TagInstance: " + tag.Group.ToString());
                    return null;
                }
                model = obj.Model;
                if (model == null) {
                    return null; //Some obje tags, such as some weapons for vehicles can have no hlmt (Model)
                }
                tagContext = new TagSerializationContext(cacheStream, _info.Cache, _info.StringIds, model);
                return (tagContext!=null)?_info.Deserializer.Deserialize<Model>(tagContext):null;
            }
        }
        //Problems:
        // 1. (FIXED) Models with no variant named 'default' are not exported
        // 2. (FIXED) Model scale parameters, including scaling of nodes are not applied
        // 3. Some maps have a shit load of unusable placement slots
        public override bool Execute(List<string> args)
        {
            if (args.Count < 2)
            {
                return false;
            }
            if (!Directory.Exists(args[1]))
            {
                Console.WriteLine("<dirpath> was not a valid path of an existing directory.");
                return false;
            }

            int scnrIndex;
            using (var mapReader = new BinaryReader(File.OpenRead(args[0])))
            {
                if (mapReader.ReadInt32() != new Tag("head").Value)
                {
                    Console.Error.WriteLine("Invalid map file");
                    return false;
                }
                mapReader.BaseStream.Position = 0x2DF0;
                scnrIndex = mapReader.ReadInt32();
            }
            //Get models from scenario
            List<Tuple<Model, int>> models = new List<Tuple<Model, int>>();
            Scenario scenario = null;
            TagInstance scnrTag = null;
            using (var cacheStream = _info.CacheFile.Open(FileMode.Open, FileAccess.ReadWrite))
            {
                scnrTag = _info.Cache.Tags[scnrIndex]; //ArgumentParser.ParseTagIndex(_info.Cache, );
                var scenarioContext = new TagSerializationContext(cacheStream, _info.Cache, _info.StringIds, scnrTag);
                scenario = _info.Deserializer.Deserialize<Scenario>(scenarioContext);
            }
            //Scan dependencies for models of gameobjects
            IEnumerable<TagInstance> dependencies = _info.Cache.Tags.FindDependencies(scnrTag);
            int n_deps = 0, n_obje = 0;
            try {
                foreach (TagInstance tag in dependencies)
                {
                    if (tag == null) {
                        Console.WriteLine("null tag");
                        continue;
                    }
                    //tags that are descendants of "obje" have a hlmt (Model).
                    if (tag.IsInGroup(new Tag("obje")))
                    {
                        Console.WriteLine("Getting model for tag: " + tag);
                        Model m = getModel(tag);
                        if (m == null) {
                            Console.WriteLine("model was null");
                            continue;
                        }
                        models.Add(new Tuple<Model, int>(m, tag.Index));
                        n_obje++;
                    }
                    n_deps++;
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine("Problem encountered with tag: {0:X8}", dependencies.ElementAt(n_deps).Index);
                
            }
            Console.WriteLine("scanned {0}/{1} deps. Extracted {2} gameobject descendent tags", n_deps, dependencies.Count(), n_obje);

            //Make a subdirectory to put the model files into
            string outdir = Directory.CreateDirectory(args[1]
                + "/" + Path.GetFileNameWithoutExtension(args[0])).FullName;

            //Get the sbsp model (collision-model). 
             
            //    This has been intentionally commented due to incompatibility
            //    with larger bsps (that have more than 65536 planes). It will 
            //    work however with simpler maps (turf, guardian, edge..) although
            //    some models may appear to be incomplete.
            
            /*
            ScenarioStructureBsp sbsp = null;
            using (var cacheStream = _info.CacheFile.Open(FileMode.Open, FileAccess.ReadWrite))
            {
                Console.WriteLine("getting sbsp");
                TagInstance sbspTag = scenario.StructureBsps[0].StructureBsp2;
                var sbspContext = new TagSerializationContext(cacheStream, _info.Cache, _info.StringIds, sbspTag);
                sbsp = _info.Deserializer.Deserialize<ScenarioStructureBsp>(sbspContext);
                Console.WriteLine("deserialized sbsp");
            }

            if (sbsp == null) {
                Console.WriteLine("SBSP tag ref was NULL");
                return false;
            }
            try
            {
                string fname = Path.GetFileNameWithoutExtension(args[0]) + ".obj";
                CollisionModel.Region.Permutation.Bsp bsp = BSPUtils.fromSbsp(sbsp, _info);
                BSPUtils.toOBJ(bsp, outdir + "/" +  fname);
            }
            catch(Exception e) {
                Console.WriteLine(e.StackTrace);
            }
            *///End of commented sbsp code

            foreach (Tuple<Model, int> t in models)
            {
                string variant = "default";
                if (t.Item1.Variants.Count > 0)
                    variant = _info.StringIds.GetString(t.Item1.Variants[0].Name);
                Console.WriteLine("First variant is: {0}", variant);
                ExtractModelCommand c = new ExtractModelCommand(_info, t.Item1);
                c.Execute(new List<string>(new string[] {variant,
                    "obj", String.Format("{0}/{1:X8}.obj", outdir, t.Item2)}));
            }
            Console.WriteLine("output to: " + outdir);
            return true;
        }
    }
}
