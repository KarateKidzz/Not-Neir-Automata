using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IInitialize
{
    void Initialize();
}

public interface IBeginPlay
{
    void BeginPlay();
}

/// <summary>
/// Defines different reasons for the object to end playing
/// </summary>
public enum EndPlayModeReason
{
    /// <summary>
    /// Destory was called on this object
    /// </summary>
    Destroyed,
    /// <summary>
    /// The game is about to load the next level
    /// </summary>
    LevelTransition,
    /// <summary>
    /// The application is exiting
    /// </summary>
    ApplicationQuit
}

public interface IEndPlay
{
    void EndPlay(EndPlayModeReason Reason);
}

public interface ITick
{
    void Tick(float DeltaTime);
}

public interface IPhysicsTick
{
    void PhysicsTick(float PhysicsDeltaTime);
}

public interface ILateTick
{
    void LateTick(float DeltaTime);
}

/// <summary>
/// Handles calling start, update etc. in the correct order
/// </summary>
public class ScriptExecution : MonoBehaviour
{
    private class ActorStore<T> where T : class
    {
        public List<T> Actors { get; } = new List<T>();
        private readonly HashSet<T> ActorsSet = new HashSet<T>();

        public void Register(T Actor)
        {
            if (Actor == null || ActorsSet.Add(Actor))
            {
                return;
            }
            Actors.Add(Actor);
        }

        public void Unregister(T Actor)
        {
            if (Actor == null || ActorsSet.Remove(Actor))
            {
                return;
            }
            Actors.Remove(Actor);
        }

        /// <summary>
        /// Clear all actors from storage
        /// </summary>
        public void Clear()
        {
            Actors.Clear();
            ActorsSet.Clear();
        }
    }

    private static readonly ActorStore<IInitialize> InitializeActors = new ActorStore<IInitialize>();
    private static readonly ActorStore<IBeginPlay> BeginPlayActors = new ActorStore<IBeginPlay>();
    private static readonly ActorStore<IEndPlay> EndPlayActors = new ActorStore<IEndPlay>();
    private static readonly ActorStore<ITick> TickActors = new ActorStore<ITick>();
    private static readonly ActorStore<IPhysicsTick> PhysicsTickActors = new ActorStore<IPhysicsTick>();
    private static readonly ActorStore<ILateTick> LateTickActors = new ActorStore<ILateTick>();

    private static bool StartedPlay;

    public static void Register(Actor Actor)
    {
        InitializeActors.Register(Actor as IInitialize);
        BeginPlayActors.Register(Actor as IBeginPlay);
        EndPlayActors.Register(Actor as IEndPlay);
        TickActors.Register(Actor as ITick);
        PhysicsTickActors.Register(Actor as IPhysicsTick);
        LateTickActors.Register(Actor as ILateTick);
    }

    public static void Unregister(Actor Actor)
    {
        InitializeActors.Unregister(Actor as IInitialize);
        BeginPlayActors.Unregister(Actor as IBeginPlay);
        EndPlayActors.Unregister(Actor as IEndPlay);
        TickActors.Unregister(Actor as ITick);
        PhysicsTickActors.Unregister(Actor as IPhysicsTick);
        LateTickActors.Unregister(Actor as ILateTick);
    }

    /// <summary>
    /// Starts the game and calls all actors' BeginPlay methods
    /// </summary>
    public static void BeginPlay()
    {
        foreach (IInitialize Initialize in InitializeActors.Actors)
        {
            Initialize.Initialize();
        }

        InitializeActors.Clear();

        foreach (IBeginPlay BeginPlay in BeginPlayActors.Actors)
        {
            BeginPlay.BeginPlay();
        }
        BeginPlayActors.Clear();    // clear this so any actors created later in the game can have BeginPlay called without worrying

        StartedPlay = true;
    }

    /// <summary>
    /// Alert all actors that a level transition is about to begin and should clean itself up
    /// </summary>
    public static void StartLevelTransition()
    {
        foreach (IEndPlay EndPlay in EndPlayActors.Actors)
        {
            EndPlay.EndPlay(EndPlayModeReason.LevelTransition);
        }

        StartedPlay = false;
    }

    private void Update()
    {
        if (!StartedPlay)
        {
            return;
        }

        foreach (IInitialize Initialize in InitializeActors.Actors)
        {
            Initialize.Initialize();
        }

        InitializeActors.Clear();

        foreach (IBeginPlay BeginPlay in BeginPlayActors.Actors)
        {
            BeginPlay.BeginPlay();
        }

        BeginPlayActors.Clear();

        float DeltaTime = Time.deltaTime;
        foreach (ITick Tick in TickActors.Actors)
        {
            Tick.Tick(DeltaTime);
        }
    }

    private void LateUpdate()
    {
        if (!StartedPlay)
        {
            return;
        }

        float DeltaTime = Time.deltaTime;
        foreach (ILateTick Tick in LateTickActors.Actors)
        {
            Tick.LateTick(DeltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (!StartedPlay)
        {
            return;
        }

        float DeltaTime = Time.fixedDeltaTime;
        foreach (IPhysicsTick Tick in PhysicsTickActors.Actors)
        {
            Tick.PhysicsTick(DeltaTime);
        }
    }

    private void OnApplicationQuit()
    {
        foreach(IEndPlay EndPlay in EndPlayActors.Actors)
        {
            EndPlay.EndPlay(EndPlayModeReason.ApplicationQuit);
        }
    }
}
