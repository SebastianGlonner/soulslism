using Godot;
using System;
using System.Collections.Generic;
using Soulslism;

public partial class NavAgent
{
    const float SPEED = 10.0f;

    private float adjustSPEED = 0;
    private float stopMovement = 0;

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

    private CharacterBody3D actor;

    private bool isWalking = false;

    private Navigation navigationNode;

    private EventManaged<Actor> targetMovingManagedEvent = new EventManaged<Actor>();
    private EventManaged<Actor> targetDyingManagedEvent = new EventManaged<Actor>();

    private ImmediateMesh draw;

    private Vector3 upVector = new Vector3(0, 1, 0);
    private float bodySize;

    private float checkDestinationPosition = 0;
    private float checkDestinationPositionTreshold = 0.1f;

    public NavAgent(Navigation navigationNode, CharacterBody3D actor, ImmediateMesh draw = null)
    {
        this.actor = actor;
        this.draw = draw;

        this.navigationNode = navigationNode;
        
        bodySize = 1f;

    }

    public float Process(float delta)
    {
        if (checkDestinationPosition > 0)
            checkDestinationPosition -= delta;

        if (stopMovement > 0)
        {
            stopMovement -= delta;
            if (stopMovement <= 0)
            {
                stopMovement = -1;
            }
            else
            {
                return remainingDistance;
            }
        }


        if (path != null && currentPathPoint < path.Length)
        {

            int pathLength = path.Length;

            Transform3D actorTransform = actor.GlobalTransform;
            Vector3 actorOrigin = actorTransform.origin;

            //remainingDistance = actorOrigin.DistanceTo(end) - targetDistance;
            //if ( remainingDistance < 0 )
            //{
            //    path = null;
            //    isWalking = false;
            //    return remainingDistance;
            //}



            Vector3 targetPointWorld = path[currentPathPoint];
            float nextPointDistance = actorOrigin.DistanceSquaredTo(targetPointWorld);

            if (currentPathPoint + 1 >= pathLength && nextPointDistance < targetDistanceSquare)
            {
                path = null;
                isWalking = false;
                remainingDistance = (float)Math.Sqrt(nextPointDistance);
                return remainingDistance;
            }

            if (nextPointDistance < 4)
            {
                currentPathPoint++;
                return Process(delta);
            }

            Vector3 desiredVelocity = (targetPointWorld - actorOrigin).Normalized() * (SPEED);
            Vector3 directedVelocity = desiredVelocity - actorVelocity;
            actor.LookAt(actorOrigin + directedVelocity, upVector);


            Transform3D testMoveTransform = actorTransform;
            //testMoveTransform.origin += directedVelocity.Normalized();

            drawVelocityVector(actorOrigin, actorOrigin + directedVelocity);

       
            bool wouldCollide = false;

            //wouldCollide = actor.TestMove(
            //    testMoveTransform,
            //    directedVelocity * delta,
            //    true
            //    );

            if (wouldCollide == false)
            {
                //actor.MoveAndSlide(
                //        directedVelocity,
                //        upVector,
                //        true,
                //        1,
                //        0.78f,
                //        false
                //    );

                //actor.MoveAndCollide(
                //        desiredVelocity * delta
                //    );

                actor.GlobalTranslate(desiredVelocity * delta);

                // int collisionCount = actor.GetSlideCount();

                if (false && actor.GetSlideCount() > 0)
                {
                    KinematicCollision3D collided = actor.GetSlideCollision(0);
                    if (collided != null)
                    {
                        Godot.Object collidedObject = collided.GetCollider();
                        if (collidedObject is Actor)
                        {
                            Actor collidedActor = collidedObject as Actor;
                            if (collidedActor.GetActorFaction() == (actor as Actor).GetActorFaction())
                            {
                                stopMovement = .2f;
                            }
                        }
                    }
                    // Logging.Log("" + (collided.GetCollider() is Actor));
                }
            } else
            {
                stopMovement = 1;
            }
        }
        else
        {
            isWalking = false;
        }

        return remainingDistance;
    }

    public float Process_backup(float delta)
    {
        if (checkDestinationPosition > 0)
            checkDestinationPosition -= delta;

        if (stopMovement > 0)
        {
            stopMovement -= delta;
            if (stopMovement <= 0)
            {
                stopMovement = -1;
            }
            else
            {
                return remainingDistance;
            }
        }


        if (path != null && currentPathPoint < path.Length)
        {

            int pathLength = path.Length;

            Transform3D actorTransform = actor.GetGlobalTransform();
            Vector3 actorOrigin = actorTransform.origin;

            //remainingDistance = actorOrigin.DistanceTo(end) - targetDistance;
            //if ( remainingDistance < 0 )
            //{
            //    path = null;
            //    isWalking = false;
            //    return remainingDistance;
            //}



            Vector3 targetPointWorld = path[currentPathPoint];
            float nextPointDistance = actorOrigin.DistanceSquaredTo(targetPointWorld);

            if (currentPathPoint + 1 >= pathLength && nextPointDistance < targetDistanceSquare)
            {
                path = null;
                isWalking = false;
                remainingDistance = (float)Math.Sqrt(nextPointDistance);
                return remainingDistance;
            }

            if (nextPointDistance < 4)
            {
                currentPathPoint++;
                return Process(delta);
            }

            Vector3 desiredVelocity = (targetPointWorld - actorOrigin).Normalized() * (SPEED + adjustSPEED);
            Vector3 directedVelocity = desiredVelocity - actorVelocity;
            actor.LookAt(actorOrigin + directedVelocity, upVector);


            Transform3D testMoveTransform = actorTransform;
            testMoveTransform.origin += directedVelocity.Normalized();

            drawVelocityVector(actorOrigin, actorOrigin + directedVelocity * delta);

            //bool wouldCollide = actor.TestMove(
            //    testMoveTransform,
            //    directedVelocity * delta,
            //    false
            //    );

            bool wouldCollide = false;

            if (wouldCollide == false)
            {
                //actor.MoveAndSlide(
                //        directedVelocity,
                //        upVector,
                //        true,
                //        1,
                //        0.78f,
                //        false
                //    );
                actor.MoveAndCollide(
                        directedVelocity * delta
                    );

                // int collisionCount = actor.GetSlideCount();

                if (false && actor.GetSlideCount() > 0)
                {
                    KinematicCollision3D collided = actor.GetSlideCollision(0);
                    if (collided != null)
                    {
                        Godot.Object collidedObject = collided.GetCollider();
                        if (collidedObject is Actor)
                        {
                            Actor collidedActor = collidedObject as Actor;
                            if (collidedActor.GetActorFaction() == (actor as Actor).GetActorFaction())
                            {
                                stopMovement = .2f;
                            }
                        }
                    }
                    // Logging.Log("" + (collided.GetCollider() is Actor));
                }
            }
        }
        else
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

        if ( false && draw != null )
        {
            Logging.Log("path - begin:" + begin);
            Logging.Log("path - end:" + end);
            draw.Clear();
            draw.Begin(Mesh.PrimitiveType.Points, null);
            draw.AddVertex(begin);
            draw.AddVertex(end);
            draw.End();
            draw.Begin(Mesh.PrimitiveType.LineStrip, null);
            foreach ( Vector3 point in this.path )
            {
                Logging.Log("path - line:" + point);
                draw.AddVertex(point);
            }
            draw.End();
        }
    }

    private void updatePathToEncounteredEnemy(Vector3 enemy)
    {
        path = new Vector3[2];
        path[0] = begin;
        path[1] = enemy;

        actorVelocity = new Vector3();
        currentPathPoint = 1;
        remainingDistance = begin.DistanceTo(end);
        isWalking = true;

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

    static private int countUpdateForTargetMovement = 0;

    private void OnDestinationMovement(Actor destination)
    {
        if (checkDestinationPosition > 0)
            return;

        checkDestinationPosition = checkDestinationPositionTreshold;

        Vector3 newEnd = destination.GetGlobalTransform().origin;
        //Logging.Log("x: " + Math.Abs(newEnd.x - this.end.x));
        //Logging.Log("z: " + Math.Abs(newEnd.z - this.end.z));
        if ( (Math.Abs(newEnd.x - this.end.x) + Math.Abs(newEnd.z - this.end.z)) > 4.0f )
        {
            countUpdateForTargetMovement++;
            begin = this.actor.GetGlobalTransform().origin;
            end = newEnd;
            updatePathToEncounteredEnemy(newEnd);
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

    public void drawVelocityVector(Vector3 from, Vector3 velocity)
    {
        if (draw == null)
            return;

        draw.Clear();
        draw.Begin(Mesh.PrimitiveType.LineStrip, null);
        draw.AddVertex(from);
        draw.AddVertex(velocity);
        draw.End();

    }
}

