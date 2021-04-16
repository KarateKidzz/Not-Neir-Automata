using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

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
            if (Actor == null || !ActorsSet.Add(Actor))
            {
                return;
            }
            Actors.Add(Actor);
        }

        public void Unregister(T Actor)
        {
            if (Actor == null || !ActorsSet.Remove(Actor))
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

        public void ClearFromList(List<T> ListToRemove)
        {
            if (ListToRemove.Count == 0)
            {
                return;
            }

            foreach(T Remove in ListToRemove)
            {
                if (ActorsSet.Remove(Remove))
                {
                    Actors.Remove(Remove);
                }
            }
        }
    }

    private static readonly ActorStore<IInitialize> InitializeActors = new ActorStore<IInitialize>();
    private static readonly ActorStore<IBeginPlay> BeginPlayActors = new ActorStore<IBeginPlay>();
    private static readonly ActorStore<IEndPlay> EndPlayActors = new ActorStore<IEndPlay>();
    private static readonly ActorStore<ITick> TickActors = new ActorStore<ITick>();
    private static readonly ActorStore<IPhysicsTick> PhysicsTickActors = new ActorStore<IPhysicsTick>();
    private static readonly ActorStore<ILateTick> LateTickActors = new ActorStore<ILateTick>();

    private readonly Dictionary<object, string> nameCache = new Dictionary<object, string>();

    private string GetName(object obj)
    {
        if (!nameCache.TryGetValue(obj, out string ret))
            nameCache.Add(obj, ret = obj.GetType().Name);
        return ret;
    }

    private static bool StartedPlay;

    public static void Register(MonoBehaviour Actor)
    {
        InitializeActors.Register(Actor as IInitialize);
        BeginPlayActors.Register(Actor as IBeginPlay);
        EndPlayActors.Register(Actor as IEndPlay);
        TickActors.Register(Actor as ITick);
        PhysicsTickActors.Register(Actor as IPhysicsTick);
        LateTickActors.Register(Actor as ILateTick);
    }

    public static void Unregister(MonoBehaviour Actor)
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
        Debug.Log($"[Script Execution] Starting Initialization");

        List<IInitialize> Inits = new List<IInitialize>(InitializeActors.Actors);

        foreach (IInitialize Initialize in Inits)
        {
            Initialize.Initialize();
        }

        InitializeActors.ClearFromList(Inits);

        Debug.Log($"[Script Execution] Starting Play");

        List<IBeginPlay> beginPlays = new List<IBeginPlay>(BeginPlayActors.Actors);

        foreach (IBeginPlay BeginPlay in beginPlays)
        {
            BeginPlay.BeginPlay();
        }

        BeginPlayActors.ClearFromList(beginPlays);

        StartedPlay = true;
    }

    /// <summary>
    /// Alert all actors that a level transition is about to begin and should clean itself up
    /// </summary>
    public static void StartLevelTransition()
    {
        Debug.Log($"[Script Execution] Starting Level Transition");

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

        Profiler.BeginSample("Script Update");

        List<IInitialize> Inits = new List<IInitialize>(InitializeActors.Actors);

        for(int i = 0; i < Inits.Count; i++)
        {
            IInitialize Initialize = Inits[i];
            Profiler.BeginSample(GetName(Initialize));
            try
            {
                Initialize.Initialize();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            Profiler.EndSample();
        }

        InitializeActors.ClearFromList(Inits); 

        List<IBeginPlay> beginPlays = new List<IBeginPlay>(BeginPlayActors.Actors);

        for (int i = 0; i < beginPlays.Count; i++)
        {
            IBeginPlay BeginPlay = beginPlays[i];
            Profiler.BeginSample(GetName(BeginPlay));
            try
            {
                BeginPlay.BeginPlay();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            Profiler.EndSample();
        }

        BeginPlayActors.ClearFromList(beginPlays);

        float DeltaTime = Time.deltaTime;

        List<ITick> Ticks = new List<ITick>(TickActors.Actors);

        for (int i = 0; i < Ticks.Count; i++)
        {
            ITick Tick = Ticks[i];
            Profiler.BeginSample(GetName(Tick));
            try
            {
                Tick.Tick(DeltaTime);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            Profiler.EndSample();
        }

        Profiler.EndSample();
    }

    private void LateUpdate()
    {
        if (!StartedPlay)
        {
            return;
        }

        Profiler.BeginSample("Script Late Update");

        float DeltaTime = Time.deltaTime;
        List<ILateTick> Ticks = new List<ILateTick>(LateTickActors.Actors);

        for (int i = 0; i < Ticks.Count; i++)
        {
            ILateTick Tick = Ticks[i];
            Profiler.BeginSample(GetName(Tick));
            try
            {
                Tick.LateTick(DeltaTime);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            Profiler.EndSample();
        }

        Profiler.EndSample();
    }

    private void FixedUpdate()
    {
        if (!StartedPlay)
        {
            return;
        }

        Profiler.BeginSample("Script Fixed Update");

        float DeltaTime = Time.fixedDeltaTime;
        List<IPhysicsTick> Ticks = new List<IPhysicsTick>(PhysicsTickActors.Actors);

        for (int i = 0; i < Ticks.Count; i++)
        {
            IPhysicsTick Tick = Ticks[i];
            Profiler.BeginSample(GetName(Tick));
            try
            {
                Tick.PhysicsTick(DeltaTime);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            Profiler.EndSample();
        }

        Profiler.EndSample();
    }

    private void OnApplicationQuit()
    {
        Debug.Log($"[Script Execution] On Application Quit");

        foreach (IEndPlay EndPlay in EndPlayActors.Actors)
        {
            EndPlay.EndPlay(EndPlayModeReason.ApplicationQuit);
        }
    }
}
