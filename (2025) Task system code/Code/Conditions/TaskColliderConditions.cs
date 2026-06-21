using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColliderCondition : TaskCondition
{
    public Collider targetCol;
    public Collider otherCol;

    public override bool IsMet()
    {
       bool overlapping = Physics.ComputePenetration(
            targetCol, targetCol.transform.position, targetCol.transform.rotation, 
            otherCol, otherCol.transform.position, otherCol.transform.rotation, 
            out Vector3 direction, out float distance);

        if (overlapping)
        {
            return true;
        }
        return false;
    }
}

public class MultiColliderCondition : TaskCondition
{
    public enum ColliderFilter
    {
        None,
        ExcludeTrigger,
        ExcludeNonTrigger,
    }

    public GameObject target;
    [Tooltip("Exclude colliders")]
    public ColliderFilter targetFilter;
    public bool targIncludeChildren;
    public GameObject other;
    [Tooltip("Exclude colliders")]
    public ColliderFilter otherFilter;
    public bool otherIncludeChildren;

    private Collider[] targCols;
    private Collider[] otherCols;


    public override bool IsMet()
    {
        if (targCols == null) targCols = GetColliders(target, targetFilter, targIncludeChildren);
        if (otherCols == null) otherCols = GetColliders(other, otherFilter, otherIncludeChildren);

        if (otherCols.Length == 0) return false;

        foreach (Collider targetCol in targCols)
        {
            foreach (Collider otherCol in otherCols)
            {
                bool overlapping = Physics.ComputePenetration(
            targetCol, targetCol.transform.position, targetCol.transform.rotation,
            otherCol, otherCol.transform.position, otherCol.transform.rotation,
            out Vector3 direction, out float distance);
                if (overlapping)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private Collider[] GetColliders(GameObject obj, ColliderFilter filter, bool includeChildren)
    {

        switch (filter)
        {
            case ColliderFilter.ExcludeTrigger:
                if (includeChildren) return obj.GetComponentsInChildren<Collider>().Where(x => !x.isTrigger).ToArray();
                else return obj.GetComponents<Collider>().Where(x => !x.isTrigger).ToArray();

            case ColliderFilter.ExcludeNonTrigger:
                if (includeChildren) return obj.GetComponentsInChildren<Collider>().Where(x => x.isTrigger).ToArray();
                else return obj.GetComponents<Collider>().Where(x => x.isTrigger).ToArray();

            default:
                if (includeChildren) return obj.GetComponentsInChildren<Collider>().ToArray();
                else return obj.GetComponents<Collider>().ToArray();
        }
    }

}
