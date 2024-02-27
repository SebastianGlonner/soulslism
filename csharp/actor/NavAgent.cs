using Godot;
using System;
using System.Collections.Generic;
using Soulslism;

public partial class NavAgent
{
    const float SPEED = 10.0f;

    private double adjustSPEED = 0;
    private double stopMovement = 0;

    private Vector3 begin;
    private Vector3 end;

    private Actor target;
    private double targetDistance;
    private double targetDistanceSquare;

    private double currentDistance;
    private double currentDistanceSquare;

    private double remainingDistance;

    private Vector3[] path;
    private Vector3 actorVelocity;
    private int currentPathPoint;

    private Actor actor;

    private bool isWalking = false;

    // g35 private Navigation navigationNode;
    private World3D world;

    private EventManaged<Actor> targetMovingManagedEvent = new EventManaged<Actor>();
    private EventManaged<Actor> targetDyingManagedEvent = new EventManaged<Actor>();

    private ImmediateMesh draw;

    private Vector3 upVector = new Vector3(0, 1, 0);
    private double bodySize;

    private double checkDestinationPosition = 0;
    private double checkDestinationPositionTreshold = 0.1f;


    bool godotNavigationAgent = true;

    public NavAgent(World3D world, Actor actor, ImmediateMesh draw = null)
    {
        this.actor = actor;
        this.draw = draw;

        // g35 this.navigationNode = navigationNode;
        this.world = world;


        bodySize = 1f;

    }

    public double Process(double delta)
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

        Vector3 targetPointWorld;
        Vector3 desiredVelocity;
        Vector3 directedVelocity;

        Transform3D actorTransform = actor.GlobalTransform;
        Vector3 actorOrigin = actorTransform.Origin;

        if (godotNavigationAgent)
        {
            targetPointWorld = actor.NavigationAgent.GetNextPathPosition();

            desiredVelocity = (targetPointWorld - actorOrigin).Normalized() * (SPEED);
            directedVelocity = desiredVelocity - actorVelocity;
            Vector3 targetLookAt = actorOrigin + directedVelocity;
            if (!actorOrigin.IsEqualApprox(targetLookAt))
            {
                targetLookAt.Y = 0;
                actor.LookAt(targetLookAt, upVector);
            }

            // !! move and slide
            //actor.Velocity = desiredVelocity;
            //actor.MoveAndSlide();
            // !! move and collide
            actor.MoveAndCollide(desiredVelocity * (float)delta);

            remainingDistance = (float)Math.Sqrt(actorOrigin.DistanceSquaredTo(targetPointWorld));
            return remainingDistance;
        }

        if (path != null && currentPathPoint < path.Length)
        {


            int pathLength = path.Length;

            //remainingDistance = actorOrigin.DistanceTo(end) - targetDistance;
            //if ( remainingDistance < 0 )
            //{
            //    path = null;
            //    isWalking = false;
            //    return remainingDistance;
            //}



            targetPointWorld = path[currentPathPoint];
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

            desiredVelocity = (targetPointWorld - actorOrigin).Normalized() * (SPEED);
            directedVelocity = desiredVelocity - actorVelocity;
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
                actor.Velocity = desiredVelocity;
                actor.MoveAndSlide();

                //actor.MoveAndCollide(
                //        desiredVelocity * (float)delta
                //    );

                // move on our own
                // actor.GlobalTranslate(desiredVelocity * (float)delta);

                // int collisionCount = actor.GetSlideCount();

                // g35 if (false && actor.GetSlideCount() > 0)
                bool test = false;
                if ( test )
                {
                    KinematicCollision3D collided = actor.GetSlideCollision(0);
                    if (collided != null)
                    {
                        GodotObject collidedObject = collided.GetCollider();
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

    private void updatePath()
    {
        if (godotNavigationAgent)
        {
            actor.NavigationAgent.TargetPosition = end;
            return;
        }


        Rid map = this.world.NavigationMap;

        // g35 path = this.navigationNode.GetSimplePath(begin, end, true);
        // Vector3 targetPoint = NavigationServer3D.MapGetClosestPointToSegment(map, begin, end, true);
        path = NavigationServer3D.MapGetPath(map, begin, end, true);


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

        /*
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
        */
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

        begin = actor.GlobalTransform.Origin;
        end = destination.GlobalTransform.Origin;

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

        Vector3 newEnd = destination.GlobalTransform.Origin;
        //Logging.Log("x: " + Math.Abs(newEnd.x - this.end.x));
        //Logging.Log("z: " + Math.Abs(newEnd.z - this.end.z));
        if ( (Math.Abs(newEnd.X - this.end.X) + Math.Abs(newEnd.Z - this.end.Z)) > 4.0f )
        {
            countUpdateForTargetMovement++;
            begin = this.actor.GlobalTransform.Origin;
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

        draw.ClearSurfaces();
        draw.SurfaceBegin(Mesh.PrimitiveType.LineStrip, null);
        draw.SurfaceAddVertex(from);
        draw.SurfaceAddVertex(velocity);
        draw.SurfaceEnd();

    }
}

