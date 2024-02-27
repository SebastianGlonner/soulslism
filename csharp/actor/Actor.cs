using Godot;
using System;
using Soulslism;

public partial class Actor : CharacterBody3D
{
	public float attackRange = 4f;
	public double attackSpeed = .8f;
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
	private double nextAttack = 0;

	private AiActionCollection actionCollection;

	private bool isDead = false;

	private bool isActing = true;

	private MeshInstance3D meshInstance;

	private Area3D sonar;

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

		meshInstance = GetNode("Body/MeshInstance3D") as MeshInstance3D;

		sonar = GetNode("Sonar") as Area3D;
		sonar.Connect("body_entered",new Callable(this,"OnTriggerEnter"));
		AddCollisionExceptionWith(this);
		
	}

	public NavigationAgent3D NavigationAgent
	{
		get
		{
			return GetNode("NavigationAgent3D") as NavigationAgent3D;
		}
	}

	private void ToggleMovementNotification(int movementListenerCount)
	{
		SetNotifyTransform(movementListenerCount > 0);
	}

	public override void _Notification(int what)
	{
		// Logging.Log("_notification: " + what);
		if (what == Node3D.NotificationTransformChanged)
		{
			this.eventMoving.Trigger(this);
		} else if ( what == GodotObject.NotificationPredelete )
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

	public override void _Process(double delta)
	{
		if (agent == null)
			return;

		if ( nextAttack > 0 )
			nextAttack -= delta;
		else if ( nextAttack <= 0 && physicsTargetRemainingDistance <= attackRange )
		{
			Actor other = agent.GetDestination();
			if (other != null)
				AttackTarget(other);
		}

	}

	private double physicsTargetRemainingDistance = .1f;

	public override void _PhysicsProcess(double delta)
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
		else if ( agent != null )
		{

			this.currentTarget = newTarget;
			this.currentDestination = newTarget.GetTarget().GlobalTransform.Origin;
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
		// g35 ProfilingCollection.add("attackTarget");
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
				meshInstance.MaterialOverride = Soulslism.MaterialsManager.Get(Soulslism.MaterialsManager.Material.ENEMY_BODY);
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
}
