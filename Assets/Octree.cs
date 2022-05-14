using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octree
{
    public Octree(BoundingBox size, List<BoundingBox> objs)
    {
        children = new Octree[8];
        region = size;
        objects = objs;
        parent = null;
    }

    public Octree()
    {
        children = new Octree[8];
        objects = new List<BoundingBox>();
        region = new BoundingBox(Vector3.zero, Vector3.zero);
        parent = null;
    }

    private Octree CreateNode(BoundingBox region, List<BoundingBox> objects)
    {
        if (objects.Count == 0)
            return null;

        Octree ret = new Octree(region, objects);
        ret.parent = this;
        return ret;
    }

    public static List<BoundingBox> TestCollision(Octree tree, BoundingBox b)
    {
        List<BoundingBox> collisions = new List<BoundingBox>();

        if (tree == null)
            return new List<BoundingBox>();

        if (tree.region.IsColliding(b))
        {
            //Loop through current trees objects
            foreach(BoundingBox obj in tree.objects)
            {
                if (obj.IsColliding(b))
                {
                    collisions.Add(obj);
                }
            }

            if (tree.children != null)
            {
                //Loop through suboctants
                for (int i = 0; i < 8; i++)
                {
                    if (tree.children[i] != null)
                    {
                        collisions.AddRange(TestCollision(tree.children[i], b));
                    }
                }
            }
        }

        return collisions;
    }

    public void Insert()
    {

    }

    public static BoundingBox[] CreateOctants(BoundingBox box)
    {
        BoundingBox[] octants = new BoundingBox[8];

        octants = new BoundingBox[8];

        Vector3 bounds = box.bounds;
        Vector3 halfBounds = bounds / 2;
        Vector3 center = box.center;
        Vector3 fourthBounds = bounds / 4;

        //North east top octant
        octants[0] = new BoundingBox(center + fourthBounds, halfBounds);

        //North west top octant
        octants[1] = new BoundingBox(new Vector3(center.x - fourthBounds.x, center.y + fourthBounds.y, center.z + fourthBounds.z), halfBounds);

        //South west top octant
        octants[2] = new BoundingBox(new Vector3(center.x - fourthBounds.x, center.y + fourthBounds.y, center.z - fourthBounds.z), halfBounds);

        //South east top octant
        octants[3] = new BoundingBox(new Vector3(center.x + fourthBounds.x, center.y + fourthBounds.y, center.z - fourthBounds.z), halfBounds);

        //North east bottom octant
        octants[4] = new BoundingBox(new Vector3(center.x + fourthBounds.x, center.y - fourthBounds.y, center.z + fourthBounds.z), halfBounds);

        //North west bottom octant
        octants[5] = new BoundingBox(new Vector3(center.x - fourthBounds.x, center.y - fourthBounds.y, center.z + fourthBounds.z), halfBounds);

        //South west bottom octant
        octants[6] = new BoundingBox(new Vector3(center.x - fourthBounds.x, center.y - fourthBounds.y, center.z - fourthBounds.z), halfBounds);

        //South east bottom octant
        octants[7] = new BoundingBox(new Vector3(center.x + fourthBounds.x, center.y - fourthBounds.y, center.z - fourthBounds.z), halfBounds);

        return octants;
    }

    public void BuildTree()
    {
        if (objects.Count <= 1)
            return;

        if (region.bounds.x <= 1.0f && region.bounds.y <= 1.0 && region.bounds.z <= 1.0)
            return;

        //Create the tree suboctants
        octants = CreateOctants(region);

        List<BoundingBox>[] octLists = new List<BoundingBox>[8];

        for (int i = 0; i < 8; i++)
        {
            octLists[i] = new List<BoundingBox>();
        }

        List<BoundingBox> delist = new List<BoundingBox>();

        foreach (BoundingBox obj in objects)
        {
            for (int a = 0; a < 8; a++)
            {
                if (octants[a].Contains(obj))
                {
                    octLists[a].Add(obj);
                    delist.Add(obj);
                    break;
                }
            }
        }

        //delist every moved object from this node.
        foreach (BoundingBox obj in delist)
            objects.Remove(obj);

        for (int a = 0; a < 8; a++)
        {
            if (octLists[a].Count != 0)
            {
                children[a] = CreateNode(octants[a], octLists[a]);
                children[a].BuildTree();
            }
        }
    }

    public BoundingBox region; //Region encapsulating the entire octant
    public BoundingBox[] octants; //This objects 8 suboctants
    public List<BoundingBox> objects; //Bounding box objects in this region

    public Octree[] children; //Child octrees.

    public Octree parent { get; private set; }
}
