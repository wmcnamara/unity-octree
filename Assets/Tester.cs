using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : BoundingBoxComponent
{
    public BVH tree;

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        tree.TestCollision(box);
    }
}
