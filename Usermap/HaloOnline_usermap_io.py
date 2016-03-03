'''
Author: Gurten


    There can be a total of 640 placements in a usermap with
    256 different possible items to be placed.


    0x278 is the offset of the placement list (640 entries, 84 bytes each)
    0xd498 is the offset of the item table (256 entries, 12 bytes each)

'''

import struct
import os
import bpy
from mathutils import Vector, Matrix
from bpy_extras.io_utils import ImportHelper, ExportHelper
from bpy.props import StringProperty, IntProperty, FloatProperty

bl_info = {
    "name": "HaloOnline Forge usermap",
    "author": "Gurten",
    "version": (1, 0, 1),
    "blender": (2, 6, 3),
    "location": "File > Import-Export",
    "description": "Import-Export Forge usermap .map",
    "warning": "",
    "wiki_url": "",
    "category": "Import-Export"}

def import_obj_directory(d_path, collapse_ratio=1.0, use_planar_reduce=False):
    '''
    Imports 'obj' files from a directory, returning the
    bpy.data.objs that were imported
        Arguments: d_path : directory path
    Two optional arguments can be used to reduce the level
    of detail that the models will have using the Decimator modifier.
         
    '''
    pre_objs = set(bpy.data.objects) # objects in the scene before importing
    obj_list = [item for item in os.listdir(d_path) if item[-3:] == 'obj']
    for fname in obj_list:
        bpy.ops.import_scene.obj(filepath=d_path + "/" + fname, axis_up='Z', axis_forward='Y')
    post_objs = set(bpy.data.objects) # objects in the scene after importing
    imported_objs = list(post_objs - pre_objs)
    if use_planar_reduce or collapse_ratio < 1.0:
        for obj in imported_objs:
            modifier = obj.modifiers.new("decimator", "DECIMATE")
            if use_planar_reduce:
                modifier.decimate_type = 'DISSOLVE'
                mesh = obj.to_mesh(scene=bpy.context.scene, apply_modifiers=True, settings="PREVIEW")
                obj.data = mesh
            modifier.decimate_type = 'COLLAPSE'
            modifier.ratio = collapse_ratio
            mesh = obj.to_mesh(scene=bpy.context.scene, apply_modifiers=True, settings="PREVIEW")
            obj.data = mesh
            obj.modifiers.remove(modifier)
    return imported_objs # get the difference, sort it

def import_palette_objs(dirpath, collapse_ratio=0.2, use_planar_reduce=True, layer=1):
    #Create an object to be the the container of all tag objects
    objs = import_obj_directory(dirpath, collapse_ratio, use_planar_reduce)
    for o in objs:
        #Move visibility to a non-default layer
        if layer != 0:
            o.layers[layer] = True
            o.layers[0] = False
    #If the 
    tag_objs = [obj for obj in objs if isHex(obj.name)]
    if len(tag_objs) > 0:
        container = bpy.data.objects.new("Tags" ,None)
        bpy.context.scene.objects.link(container)
        if layer != 0:
            container.layers[layer] = True;container.layers[0] = False
        for o in tag_objs:
            o.parent = container
    return objs
    

def isHex(s):
    try:
        x = int(s, 16)
        return True
    except ValueError:
        return False

def get_tag(s):
    placement_object = bpy.data.objects[s]
    tag_objs = [obj for obj in bpy.data.objects if isHex(obj.name)]
    for obj in tag_objs:
        if obj.data == placement_object.data:
            return obj.name

def get_data(data, offset, size):
    return data[offset: (offset + size)]

class ItemTableEntry:
    def __init__(self,l):
        if not l or len(l) < 3:
            raise Exception("Array must have at least 3 elements")
        self.index = l[0]
        self.unk04 = l[1]
        self.unk08 = l[2]
    def to_list(self):
        return [self.index, self.unk04, self.unk08]

class Placement:
    #The placement has been deleted and can be replaced
    property_bit_deleted = 0x20
    def __init__(self, l):
        if not l or len(l) < 38:
            raise Exception("Constructor must be provided an array with at least 38 elements, got %d" % len(l))
        self.property = l[0]
        self.unk04 = l[1]
        self.unk08 = l[2]
        self.index = l[3]
        #matrix columns: partial components of vectors: forward, right, and up
        self.pos = Vector(l[4:7])
        self.col0 = Vector(l[7:10])
        self.col2 = Vector(l[10:13])
        #
        self.unk52 = l[13]
        self.unk56 = l[14]
        #
        self.unk60 = l[15]
        #
        self.unk62 = l[16]
        self.unk63 = l[17]
        self.unk64 = l[18]
        self.respawn_time = l[19]
        self.unk66 = l[20]
        self.unk67 = l[21]
        self.unk68 = l[22]
        self.unk69 = l[23]
        self.unk70 = l[24]
        self.unk71 = l[25]
        self.unk72 = l[26]
        self.unk73 = l[27]
        self.unk74 = l[28]
        self.unk75 = l[29]
        self.unk76 = l[30]
        self.unk77 = l[31]
        self.unk78 = l[32]
        self.unk79 = l[33]
        self.unk80 = l[34]
        self.unk81 = l[35]
        self.unk82 = l[36]
        self.unk83 = l[37]
    def to_list(self):
        return [self.property, self.unk04, self.unk08, self.index, \
            \
            self.pos[0], self.pos[1], self.pos[2], \
            self.col0[0], self.col0[1], self.col0[2], \
            self.col2[0], self.col2[1], self.col2[2], \
            \
            self.unk52, self.unk56, 
            \
            self.unk60, 
            \
            self.unk62, self.unk63, self.unk64, self.respawn_time, \
            self.unk66,self.unk67, self.unk68, self.unk69, self.unk70, \
            self.unk71, self.unk72, self.unk73, self.unk74, self.unk75, \
            self.unk76, self.unk77, self.unk78, self.unk79, self.unk80, \
            self.unk81, self.unk82, self.unk83 ]

class UserMap:
    #The offset at which the number of item placements used is located
    n_placement_offset = 0x244
    #0x24C-0x264 = map bounding box: xmin,xmax,ymin,ymax,zmin,zmax
    #The offset at which the item placements begin
    placement_begin_offset = 0x278
    #The offset where table of tag indices begins
    item_table_begin_offset = 0xD498
    #The hard limit on the number of table items
    n_table_items = 256
    #The size of a table item
    item_table_entry_size = 12
    #How to parse the table item, where each character represents a type (see struct module)
    table_item_s = "iii"
    #The size of the placement struct
    placement_size = 84
    #How to parse a placement, where each character represents a type (see struct module)
    placement_s = "i"*4 + "f"*9 + "i"*2 + "H" + "B"*22 
    #The maximum number of placements in the file. This is a hard limit of the engine.
    max_placements = 640
    def __init__(self):
        #The data of the file opened
        self.data = None
        #The current number of item placements
        self.n_placements = 0
        #The offset of the first word after the placements in the file
        #The item placements
        self.placements = []
        #The table items
        self.table_items = []
        #Whether a file has been opened and read successfully
        self.hasOpened = False
    def readFromFile(self, fpath):
        '''
            Reads a HaloOnline usermap and loads data into the appropriate attributes of 
            this object. 
            
            'fpath' must be the valid path of a 'sandbox.map' file
        '''
        #Open the file
        f = open(fpath, "rb")
        self.data = f.read()
        f.close()
        #Check that the file has magic values at offsets consistent
        # with a usermap file.
        isUserMap = get_data(self.data, 0x138, 4) == b"mapv"
        isUserMap &= get_data(self.data, 0, 4) == b"_blf"
        if not isUserMap:
            self.data = None
            print("Not a usermap.")
        self.fpath = fpath
        #Read the number of placement slots used
        n_placements = struct.unpack("H", get_data(self.data, self.n_placement_offset, 2))[0]
        for i in range(n_placements):
            p = Placement(struct.unpack(self.placement_s, \
                get_data(self.data, \
                i*self.placement_size + self.placement_begin_offset, self.placement_size)))
            self.placements.append(p)
        #Read the tag table from the file
        for i in range(self.n_table_items):
            item = struct.unpack(self.table_item_s, get_data(self.data, \
            self.item_table_begin_offset + i * self.item_table_entry_size, \
                self.item_table_entry_size))
            item = ItemTableEntry(item)
            self.table_items.append(item)
        #The object now contains all valid data
        self.hasOpened = True
        self.to_Scene()
    def writeToFile(self, fpath):
        '''
            Writes a HaloOnline usermap from the data contained in the object.
            
            'fpath' should be the path of the desired file output
        '''
        #The file must have data in it from an opened file
        self.from_Scene()
        n_placements = len(self.placements)
        p_end_offset = self.placement_size * n_placements + self.placement_begin_offset
        i_end_offset = self.item_table_begin_offset + self.item_table_entry_size * self.n_table_items
        # Get the attributes of each placement in an ordered list
        plists = [p.to_list() for p in self.placements]
        # Flatten the lists into one long list
        pflat_list = [item for sublist in plists for item in sublist]
        ilists = [i.to_list() for i in self.table_items]
        iflat_list = [item for sublist in ilists for item in sublist]
        # Serialize the data of the placements in place
        out_data = self.data[:self.n_placement_offset] + struct.pack("H", n_placements) \
            + self.data[(self.n_placement_offset + 2): self.placement_begin_offset] \
            + struct.pack(self.placement_s*n_placements, *pflat_list) \
            + self.data[p_end_offset: self.item_table_begin_offset] \
            + struct.pack(self.table_item_s*self.n_table_items, *iflat_list) \
            + self.data[i_end_offset:]
        f = open(fpath, "wb")
        f.write(out_data)
        f.close()
        if len(out_data) != len(self.data):
            print("Input file size not equal to output.\nSomething went wrong.")
    def to_Scene(self):
        if self.hasOpened:
            #      Load table
            #
            #Get the tag objects from the scene by filtering objects with 
            # hexadecimal strings as the first 8 chars of their name
            tag_objs = {obj.name[:8]:obj for obj in bpy.data.objects if isHex(obj.name[:8])}
            #
            #      Load placements
            #
            #Create a parent object to contain all placement objects
            for i, p in enumerate(self.placements):
                mesh = None # the mesh for the object
                #The placement's index references a table item that has a tag index
                if p.index >= 0 and p.index < self.n_table_items: # the index must be in the range of the table items
                    palette_tag_idx = self.table_items[p.index].index 
                    try:
                        #apply the data if found
                        mesh = tag_objs[str('%08x' % palette_tag_idx).upper()].data
                    except KeyError:
                        print("Could not find tag '%08x' for placement %d" % (palette_tag_idx, i))
                object = bpy.data.objects.new("Placement.%03d" % i, mesh)
                #link the object to the active scene (ususally scene 0)
                bpy.context.scene.objects.link(object)
                for j in range(3):
                    object.lock_scale[j] = True #objects in forge can be only moved or rotated
                #
                #    Apply a matrix to the object to place it correctly in the scene
                #
                #Recreate col1 using cross product of column vectors: col0 
                # and col2 that are perpendicular to one another
                col2 = p.col2
                col0 = p.col0
                col1 = col0.cross(col2)
                pos = p.pos
                #Construct 3x3 matrix with column vectors for rows
                mat = Matrix((col0, col1, col2))
                mat.transpose() # Matrix is now correct
                #Increase size from 3x3 to 4x4 to move 3D points 
                # as well as to rotate/scale them like a 3x3 could
                mat = mat.to_4x4() 
                for i in range(3):
                    mat[i][3] = pos[i]
                object.matrix_local = mat
                #object.
                #Assign 'Custom Properties' to the object with values from the Placement
                # in order to serialize the placement later
                pd = vars(p)
                for key in pd.keys():
                    object[key] = pd[key]
                #Assign the item table to the scene in order to serialize later
                bpy.data.scenes[0]['item_table'] = [l.to_list() for l in self.table_items]
                #Assign the entire 60k file to the scene
                bpy.data.scenes[0]['usermap_data'] = struct.unpack("B" * len(self.data), self.data)
    
    def from_Scene(self):
        #Filter placement objects which have 'Placement' in the first 9 letters of the name
        objs = [obj for obj in bpy.data.objects if "Placement" == obj.name[:9]]
        if len(objs) > self.max_placements:
            for obj in bpy.data.objects:
                obj.select = False
            for obj in objs[self.max_placements:]:
                obj.select = True
            print("Number of placements exceeeded %d. The objects that have been selected will not be saved.", self.max_placements)
            objs = objs[:self.max_placements]
        try:
            self.data = struct.pack("B" * len(bpy.data.scenes[0]['usermap_data']), *bpy.data.scenes[0]['usermap_data'])
            #Parse the data from the scene's custom property storing the item-table
            self.table_items = [ItemTableEntry(i.to_list()) for i in bpy.data.scenes[0]["item_table"]]
        except KeyError:
            raise Exception("You must have imported a usermap to export one")
        newPlacements = []
        for o in objs:
            try:
                p = Placement([int(o['property']), int(o['unk04']), int(o['unk08']), int(o['index']) \
                    , 0.0 , 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0  \
                    , int(o['unk52']), int(o['unk56']) \
                    , int(o['unk60']) \
                    , int(o['unk62']), int(o['unk63']), int(o['unk64']), int(o['respawn_time']) ] + \
                    [int(o['unk%02d' % i]) for i in range(66,84)] )
                m = o.matrix_local.to_3x3() 
                m.transpose()    
                p.pos = o.location
                p.col0 = Vector(m[0])
                p.col2 = Vector(m[2])
                newPlacements.append(p)
            except KeyError:
                print("Object: '%s' did not have the needed data to be saved to the map." % o.name)
        self.placements = newPlacements
        print("Parsed %d objects." % len(newPlacements))

class ImportUsermap(bpy.types.Operator, ImportHelper):
    ''' Import a HaloOnline forge usermap'''
    bl_idname = "import.map"
    bl_label = "Import map"
    __doc__ = "HaloOnline usermap importer (.map)"
    filename_ext = ".map"
    filter_glob = StringProperty( default = "*.map", options = {'HIDDEN'} )
    
    filepath = StringProperty( 
        name = "File Path",
        description = "File path to usermap file",
        maxlen = 1024,
        default = "" )
    
    def execute(self, context):
        usermap = UserMap()
        usermap.readFromFile(self.filepath)
        return {'FINISHED'}
    
    def draw( self, context ):
        layout = self.layout
        box = layout.box()

class ExportUsermap(bpy.types.Operator, ExportHelper):
    ''' Export a HaloOnline forge usermap'''
    bl_idname = "export.map"
    bl_label = "Export map"
    __doc__ = "HaloOnline usermap exporter (.map)"
    filename_ext = ".map"
    filter_glob = StringProperty( default = "*.map", options = {'HIDDEN'} )
    
    filepath = StringProperty( 
        name = "File Path",
        description = "File path to usermap file",
        maxlen = 1024,
        default = "" )
    
    def execute(self, context):
        usermap = UserMap()
        usermap.writeToFile(self.filepath)
        return {'FINISHED'}
    
    def draw( self, context ):
        layout = self.layout
        box = layout.box()

class ImportObjDir(bpy.types.Operator, ImportHelper):
    bl_idname = "import.objdir"
    bl_label = "Import"
    __doc__ = "Mass obj/tag import (.obj). Select an .obj in a directory. All other .objs in the directory will be imported too."
    filename_ext = ".vtk"
    filter_glob = StringProperty( default = "*.obj", options = {'HIDDEN'} )
    
    filepath = StringProperty( 
        name = "File Path",
        description = "File path to .obj in a directory of numerous .obj files",
        maxlen = 1024,
        default = "" )
        
    layer = IntProperty(name="Layer Placed", default=1, min=0, max=19)
    
    use_planar = bpy.props.BoolProperty(name="Use planar mesh simplification", default = True)
    
    quality = FloatProperty(name="Mesh Quality", default = 0.5, min = 0.0, max = 1.0, step = 0.01, precision = 2)
    
    def draw( self, context ):
        layout = self.layout
        box = layout.box()
        row = box.row()
        row = layout.row(align=True)
        row.prop(self, "use_planar")
        row = box.row()
        row.prop(self, "quality")
        row = box.row()
        row.prop(self, "layer")

    def execute(self, context):
        dirpath = os.path.dirname(self.filepath) + "/" #additional directory separator
        import_palette_objs(dirpath, collapse_ratio=self.quality, use_planar_reduce=self.use_planar, layer=self.layer)
        return {'FINISHED'}
        
# Blender register plugin 
def menu_func_import( self, context ):
    self.layout.operator( ImportUsermap.bl_idname, text = "HaloOnline Forge usermap importer (.map)" )

def menu_func_export( self, context ):
    self.layout.operator( ExportUsermap.bl_idname, text = "HaloOnline Forge usermap exporter (.map)" )

def menu_func_obj_import( self, context ):
    self.layout.operator( ImportObjDir.bl_idname, text = "Mass obj/tag importer (.obj)" )

def register():
    bpy.utils.register_class( ImportUsermap )
    bpy.utils.register_class( ExportUsermap )
    bpy.utils.register_class( ImportObjDir )
    bpy.types.INFO_MT_file_import.append( menu_func_import )
    bpy.types.INFO_MT_file_import.append( menu_func_obj_import )
    bpy.types.INFO_MT_file_export.append( menu_func_export )

def unregister():
    bpy.utils.unregister_class( ImportUsermap )
    bpy.utils.unregister_class( ExportUsermap )
    bpy.utils.unregister_class( ImportObjDir )
    bpy.types.INFO_MT_file_import.remove( menu_func_import )
    bpy.types.INFO_MT_file_import.remove( menu_func_obj_import )
    bpy.types.INFO_MT_file_export.remove( menu_func_export )

if __name__ == "__main__":
    register()