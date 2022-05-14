using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BVH : MonoBehaviour
{
    public BoundingBoxComponent[] boxes = new BoundingBoxComponent[8];
    Octree octree;
    public Vector3 size;

    private void Start()
    {
        List<BoundingBox> bBoxes = new List<BoundingBox>();
        foreach (BoundingBoxComponent b in boxes)
            bBoxes.Add(b.box);

        octree = new Octree(new BoundingBox(Vector3.zero, size), new List<BoundingBox>(bBoxes));
        octree.BuildTree();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(Vector3.zero, size);
        RenderOctree(octree);
    }

    public void TestCollision(BoundingBox b)
    {
        var collisions = Octree.TestCollision(octree, b);

        foreach(BoundingBox c in collisions)
        {
            Debug.Log("Collision: " + c.center);
        }
    }

    private void Update()
    {
        List<BoundingBox> bBoxes = new List<BoundingBox>();
        foreach (BoundingBoxComponent b in boxes)
            bBoxes.Add(b.box);

        octree = new Octree(new BoundingBox(Vector3.zero, size), new List<BoundingBox>(bBoxes));
        octree.BuildTree();
    }

    
    void RenderOctree(Octree tree)
    {
        if (tree == null)
            return;

        foreach(BoundingBox b in tree.octants)
        {
            b.Draw();
        }

        if (tree.children == null)
            return;

        foreach (Octree child in tree.children)
        {
            if (child == null || child.octants == null)
                continue;

            foreach(BoundingBox b in child.octants)
            {
                b.Draw();
                RenderOctree(child);
            }
        }
    }
}
