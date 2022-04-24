using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : BoundingBoxComponent
{
    public BVH tree;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        tree.TestCollision(box);
    }
}
