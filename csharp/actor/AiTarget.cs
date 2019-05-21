using Godot;
using System;

public class AiTarget {

    private Actor targetActor;
    public string myObjectName;
    private int priority;

    public AiTarget(Actor spatial, int priority)
    {
        this.targetActor = spatial;
        this.priority = priority;
    }

    public Actor GetTarget()
    {
        return targetActor;
    }

    public bool HasHigherPriorityThan(AiTarget other)
    {
        if ( other == null )
        {
            return true;
        }

        if ( priority > other.priority )
        {
            return true;
        }

        return false;
    }

    public override bool Equals(object obj)
    {
        AiTarget item = obj as AiTarget;

        if (item == null)
        {
            return false;
        }

        return targetActor.Equals(item.targetActor);
    }

    public override int GetHashCode()
    {
        return this.targetActor.GetHashCode();
    }
}
