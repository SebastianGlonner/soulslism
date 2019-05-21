using System.Collections.Generic;

public class AiActionCollection {

    HashSet<AiTarget> targets = new HashSet<AiTarget>();

    private AiTarget baseTarget;

    public AiTarget GetNextTarget()
    {
        AiTarget highest = null;
        foreach (AiTarget current in targets)
        {
            if (current.HasHigherPriorityThan(highest))
            {
                highest = current;
            }
        }

        return highest;
    }

    public AiTarget AddTarget(AiTarget target, AiTarget currentTarget)
    {
        if ( target == null )
        {
            return currentTarget;
        }

        targets.Add(target);

        if ( target.HasHigherPriorityThan(currentTarget) )
        {
            return target;
        }

        return currentTarget;
    }

    public void RemoveTarget(AiTarget target)
    {
        targets.Remove(target);
    }
}
