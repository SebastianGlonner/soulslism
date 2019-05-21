using Godot;
using System;
using System.Collections.Generic;
using Soulslism;

public class NavAgent
{
    const float SPEED = 14.0f;

    private Vector3 begin;
    private Vector3 end;

    private Actor target;
    private float targetDistance;
    private float targetDistanceSquare;

    private float currentDistance;
    private float currentDistanceSquare;

    private float remainingDistance;

    private Vector3[] path;
    private Vector3 actorVelocity;
    private int currentPathPoint;

    private KinematicBody actor;

    private bool isWalking = false;

    private Navigation navigationNode;

    private EventManaged<Actor> targetMovingManagedEvent = new EventManaged<Actor>();
    private EventManaged<Actor> targetDyingManagedEvent = new EventManaged<Actor>();

    private ImmediateGeometry draw;

    private Vector3 upVector = new Vector3(0, 1, 0);

    public NavAgent(Navigation navigationNode, KinematicBody actor, ImmediateGeometry draw = null)
    {
        this.actor = actor;
        this.draw = draw;

        this.navigationNode = navigationNode;
    }

    public float Process(float delta)
    {

        if ( path != null && currentPathPoint < path.Length )
        {

            int pathLength = path.Length;

            Vector3 actorOrigin = actor.GetGlobalTransform().origin;

            //remainingDistance = actorOrigin.DistanceTo(end) - targetDistance;
            //if ( remainingDistance < 0 )
            //{
            //    path = null;
            //    isWalking = false;
            //    return remainingDistance;
            //}



            Vector3 targetPointWorld = path[currentPathPoint];
            float nextPointDistance = actorOrigin.DistanceSquaredTo(targetPointWorld);

            if (currentPathPoint + 1 >= pathLength && nextPointDistance < targetDistanceSquare )
            {
                path = null;
                isWalking = false;
                remainingDistance = (float) Math.Sqrt(nextPointDistance);
                return remainingDistance;
            }

            if (nextPointDistance < 4)
            {
                currentPathPoint++;
                return Process(delta);
            }

            Vector3 desiredVelocity = (targetPointWorld - actorOrigin).Normalized() * SPEED;
            Vector3 directedVelocity = desiredVelocity - actorVelocity;
            actor.LookAt(actorOrigin + directedVelocity, upVector);
            actor.MoveAndSlide(
                directedVelocity,
                upVector
            );

        } else
        {
            isWalking = false;
        }

        return remainingDistance;
    }

    private void updatePath()
    {
        path = this.navigationNode.GetSimplePath(begin, end, true);

        //  Logging.Log("Update NavAgent path - Point.Count: " + path.Length);
        if ( path.Length <= 0 )
        {
            this.path = null;
            return;
        }

        actorVelocity = new Vector3();
        currentPathPoint = 1;
        remainingDistance = begin.DistanceTo(end);
        isWalking = true;

        if ( draw != null )
        {
            draw.Clear();
            draw.Begin(Mesh.PrimitiveType.Points, null);
            draw.AddVertex(begin);
            draw.AddVertex(end);
            draw.End();
            draw.Begin(Mesh.PrimitiveType.LineStrip, null);
            foreach ( Vector3 point in this.path )
            {
                draw.AddVertex(point);
            }
            draw.End();
        }
    }

    public void SetDestination(Actor destination, float distance)
    {
        this.targetDistance = distance;
        this.targetDistanceSquare = distance * distance;

        begin = actor.GetGlobalTransform().origin;
        end = destination.GetGlobalTransform().origin;

        //Logging.Log("NavAgent.SetDestination.begin: " + begin);
        //Logging.Log("NavAgent.SetDestination.end: " + end);
        updatePath();

        if ( this.target != null )
        {
            targetMovingManagedEvent.RemoveFrom(this.target.eventMoving);
            targetDyingManagedEvent.RemoveFrom(this.target.eventDying);
        }

        this.target = destination;
        targetMovingManagedEvent.AddTo(destination.eventMoving, OnDestinationMovement);
        targetDyingManagedEvent.AddTo(destination.eventDying, OnDestinationDying);
    }

    public Actor GetDestination()
    {
        return this.target;
    }

    private void OnDestinationDying(Actor destination)
    {
        targetMovingManagedEvent.RemoveFrom(destination.eventMoving);
        targetDyingManagedEvent.RemoveFrom(destination.eventDying);
        Stop();
    }

    private void OnDestinationMovement(Actor destination)
    {
        Vector3 newEnd = destination.GetGlobalTransform().origin;
        //Logging.Log("x: " + Math.Abs(newEnd.x - this.end.x));
        //Logging.Log("z: " + Math.Abs(newEnd.z - this.end.z));
        if ( (Math.Abs(newEnd.x - this.end.x) + Math.Abs(newEnd.z - this.end.z)) > 0.3f )
        {
            begin = this.actor.GetGlobalTransform().origin;
            end = newEnd;
            updatePath();
        }
    }

    public void Stop()
    {
        isWalking = false;
    }

    public void Start()
    {
        isWalking = path != null && path.Length > 1;
    }

    public void Free()
    {
        targetMovingManagedEvent.RemoveAll();
        targetDyingManagedEvent.RemoveAll();
    }
}

