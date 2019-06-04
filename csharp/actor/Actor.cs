using Godot;
using System;
using Soulslism;

public class Actor : KinematicBody
{
    public float attackRange = 4f;
    public float attackSpeed = .8f;
    public int totalLife = 5;
    public int damagePerAttack = 1;

    private Soulslism.Faction faction = Soulslism.Faction.Friend;

    private int currentLife;
    private LifeLine lifeLine;
    private NavAgent agent;
    private AiTarget currentTarget;

    private AiTarget aiTarget;

    private Vector3 currentDestination;

    private Actor otherAi;
    private float nextAttack = 0;

    private AiActionCollection actionCollection;

    private bool isDead = false;

    private bool isActing = true;

    private MeshInstance meshInstance;

    private Area sonar;

    public Event<Actor> eventDying = new Event<Actor>();
    public EventObservable<Actor> eventMoving = new EventObservable<Actor>();

    private EventManaged<Actor> targetDyingManagedEvent = new EventManaged<Actor>();

    public Soulslism.Faction GetActorFaction()
    {
        return faction;
    }

    public override void _Ready()
    {
        SetNotifyTransform(true);
        //eventMoving.eventAdd.Add(ToggleMovementNotification);
        //eventMoving.eventRemove.Add(ToggleMovementNotification);

        actionCollection = new AiActionCollection();

        currentLife = totalLife;

        lifeLine = GetNode("LifeLine") as LifeLine;
        lifeLine.GetIndex();
        aiTarget = new AiTarget(this, Properties.PRIORITY_ENEMY);

        meshInstance = GetNode("Body/MeshInstance") as MeshInstance;

        sonar = GetNode("Sonar") as Area;
        sonar.Connect("body_entered", this, "OnTriggerEnter");
        AddCollisionExceptionWith(this);

    }

    private void ToggleMovementNotification(int movementListenerCount)
    {
        SetNotifyTransform(movementListenerCount > 0);
    }

    public override void _Notification(int what)
    {
        // Logging.Log("_notification: " + what);
        if (what == Spatial.NotificationTransformChanged)
        {
            this.eventMoving.Trigger(this);
        } else if ( what == Godot.Object.NotificationPredelete )
        {
            // Logging.Log("_notification: NotificationPredelete");
            // OnQueueFree();
        }
    }

    public NavAgent Agent
    {
        set {
            agent = value;
        }
    }

    public override void _Process(float delta)
    {
        if ( nextAttack > 0 )
            nextAttack -= delta;

        if ( agent != null )
        {
            if (physicsTargetRemainingDistance <= attackRange )
            {
                // target is in range
                if ( nextAttack <= 0 )
                {
                    Actor other = agent.GetDestination();
                    if (other != null)
                        AttackTarget(other);
                }
            }
        }

    }

    private float physicsTargetRemainingDistance = .1f;

    public override void _PhysicsProcess(float delta)
    {
        if (agent != null)
            physicsTargetRemainingDistance = agent.Process(delta);
    }

    public void AddTarget(AiTarget targetObject)
    {
        targetDyingManagedEvent.AddTo(targetObject.GetTarget().eventDying, OnTargetDying);
        UpdateCurrentTarget(actionCollection.AddTarget(targetObject, this.currentTarget));
    }

    public void OnTargetDying(Actor target)
    {
        actionCollection.RemoveTarget(target.AiTarget);
        PerformNextAction();
    }

    private void PerformNextAction()
    {
        UpdateCurrentTarget(actionCollection.GetNextTarget());
    }

    private void UpdateCurrentTarget(AiTarget newTarget)
    {
        if (newTarget == null)
        {
            // this.currentDestination = null;
            this.currentTarget = null;
            this.otherAi = null;
            // this.agent.Stop();
        }
        else
        {
            this.currentTarget = newTarget;
            this.currentDestination = newTarget.GetTarget().GetGlobalTransform().origin;
            //this.otherAi = newTarget.Target;
            agent.SetDestination(newTarget.GetTarget(), attackRange);
        }
    }

    private void OnTriggerEnter(Node other)
    {
        // Logging.Log(GetName() + ": sonar hit something: " + other.GetClass() + " - " + other.GetType() + " - " + other.GetName());

        Actor otherAi = other as Actor;
        if ( otherAi != null )
        {
            // Logging.Log("entered enemy sonar : " + otherAi.faction);

            if (this.faction == otherAi.faction)
            {
                return;
            }

            AddTarget(otherAi.AiTarget);
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    Actor otherAi = other.gameObject.GetComponentInParent<Actor>();
    //    if (otherAi)
    //    {
    //        if (this.faction == otherAi.faction)
    //        {
    //            return;
    //        }

    //        actionCollection.RemoveTarget(otherAi.AiTarget);
    //    }
    //}

    private void AttackTarget(Actor other)
    {
        other.ReceiveAttack(this.damagePerAttack);
        nextAttack = attackSpeed;
    }

    public bool ReceiveAttack(int damagePerAttack)
    {

        currentLife -= damagePerAttack;

        if (currentLife <= 0)
        {
            lifeLine.SetLife(0);
        }
        else
        {
            lifeLine.SetLife(currentLife * 100 / totalLife);
        }

        this.isDead = currentLife <= 0;

        if (isDead)
        {
            if ( agent != null )
                agent.Stop();

            eventDying.Trigger(this);
            QueueDestroyNode();
        }

        return isDead;
    }

    public void SetTotalLife(int amount)
    {
        currentLife = totalLife = amount;
    }

    public AiTarget AiTarget
    {
        get { return aiTarget; }
        private set { }
    }

    public Soulslism.Faction Faction
    {
        set
        {
            faction = value;

            if ( faction == Soulslism.Faction.Enemy )
            {
                meshInstance.SetMaterialOverride(Soulslism.MaterialsManager.Get(Soulslism.MaterialsManager.Material.ENEMY_BODY));
            }
        }
        get
        {
            return faction;
        }
    }

    public void QueueDestroyNode()
    {
        if ( agent != null )
            agent.Free();

        targetDyingManagedEvent.RemoveAll();
        Node parent = GetParent();
        if ( parent != null )
            parent.RemoveChild(this);
        QueueFree();
    }


    /*******************************************
     * 
     * checkout afterwards
     * 
     * *****************************************/
     
    // Update is called once per frame
    void Update(float delta)
    {

        if (ActorManager.actorsPaused)
        {
            if (isActing)
            {
                // agent.isStopped = true;
            }

            isActing = false;
            return;
        }
        else
        {
            // agent.isStopped = false;
            isActing = true;
        }

        // Debug.Log(agent.hasPath);


        if (currentDestination != null)
        {
            // agent.SetDestination(currentDestination);

            //if (agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance <= Properties.CLOSE_RANGED_DISTANCE)
            //{
            //    AttackTarget();
            //}
        }
    }

    /*******************************
     * 
     * old functions
     * 
     * ****************************/
}
