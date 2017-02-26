using System.Collections;
using System.Collections.Generic;

public class StateMachineException : System.Exception
{
    protected string message;
    public StateMachineException(string message) { this.message = message; }
    public override string Message { get { return message; } }
    public override string ToString() { return "State Machine Exception: " + message; }
}


public class StateMachine
{
    public delegate void UpdateDelegate();
    Dictionary<string, UpdateDelegate> stateUpdateMethods;

    public delegate void StateChangeDelegate();
    Dictionary<string, List<StateChangeDelegate>> exitStateListeners;
    Dictionary<string, List<StateChangeDelegate>> enterStateListeners;

    public void addEnterStateListener(string state, StateChangeDelegate method)
    {
        if (state.Length == 0) throw new StateMachineException("string 'state' canont be empty.");
        if (method == null) throw new StateMachineException("StateChangeDelegate 'method' cannot be null.");
        if (stateUpdateMethods.ContainsKey(state) == false) throw new StateMachineException("Cannot add listener because state, " + state + ", does not exist.");

        if (enterStateListeners.ContainsKey(state) == false)
        {
            enterStateListeners.Add(state, new List<StateChangeDelegate>());
            enterStateListeners[state].Add(method);
        }
        else if (enterStateListeners[state].Contains(method) == false)
        {
            enterStateListeners[state].Add(method);
        }
    }

    public void removeEnterStateListener(string state, StateChangeDelegate method)
    {
        if (state.Length == 0) throw new StateMachineException("string 'state' canont be empty.");
        if (method == null) throw new StateMachineException("StateChangeDelegate 'method' cannot be null.");
        if (stateUpdateMethods.ContainsKey(state) == false) throw new StateMachineException("Cannot remove listener because state, " + state + ", does not exist.");

        if (enterStateListeners.ContainsKey(state) && enterStateListeners[state].Contains(method) == false)
        {
            enterStateListeners[state].Remove(method);
        }
    }

    void notifyEnterStateListeners(string state)
    {
        if (enterStateListeners.ContainsKey(state))
        {
            foreach (StateChangeDelegate stateChangeDelegate in enterStateListeners[state])
            {
                stateChangeDelegate();
            }
        }
    }

    public void addExitStateListener(string state, StateChangeDelegate method)
    {
        if (state.Length == 0) throw new StateMachineException("string 'state' canont be empty.");
        if (method == null) throw new StateMachineException("StateChangeDelegate 'method' cannot be null.");
        if (stateUpdateMethods.ContainsKey(state) == false) throw new StateMachineException("Cannot add listener because state, " + state + ", does not exist.");

        if (exitStateListeners.ContainsKey(state) == false)
        {
            exitStateListeners.Add(state, new List<StateChangeDelegate>());
            exitStateListeners[state].Add(method);
        }
        else if (exitStateListeners[state].Contains(method) == false)
        {
            exitStateListeners[state].Add(method);
        }
    }

    public void removeExitStateListener(string state, StateChangeDelegate method)
    {
        if (state.Length == 0) throw new StateMachineException("string 'state' canont be empty.");
        if (method == null) throw new StateMachineException("StateChangeDelegate 'method' cannot be null.");
        if (stateUpdateMethods.ContainsKey(state) == false) throw new StateMachineException("Cannot remove listener because state, " + state + ", does not exist.");

        if (exitStateListeners.ContainsKey(state) && exitStateListeners[state].Contains(method) == false)
        {
            exitStateListeners[state].Remove(method);
        }
    }

    void notifyExitStateListeners(string state)
    {
        if (exitStateListeners.ContainsKey(state))
        {
            foreach (StateChangeDelegate stateChangeDelegate in exitStateListeners[state])
            {
                stateChangeDelegate();
            }
        }
    }



    public delegate void stateChangeDelegate(string oldState, string newState);
    public event stateChangeDelegate onStateChange;

    string _state;
    public string getState() { return _state; }
    public bool isInState(string state) { return _state.Equals(state); }

    public StateMachine()
    {
        stateUpdateMethods = new Dictionary<string, UpdateDelegate>();
        enterStateListeners = new Dictionary<string, List<StateChangeDelegate>>();
        exitStateListeners = new Dictionary<string, List<StateChangeDelegate>>();
        _state = "";
    }

    public void addState(string state, UpdateDelegate method)
    {
        if (state.Length == 0) throw new StateMachineException("string 'state' canont be empty.");
        if (method == null) throw new StateMachineException("UpdateDelegate 'method' cannot be null.");
        if (stateUpdateMethods.ContainsKey(state)) throw new StateMachineException("Cannot add state, " + state + ", because it already exists.");

        stateUpdateMethods.Add(state, method);
    }


    public void setState(string state)
    {
        if (state.Length > 0 && stateUpdateMethods.ContainsKey(state) == false) 
            throw new StateMachineException("Cannot set state, " + state + ", because it does not exist.");

        string oldState = _state;

        notifyExitStateListeners(oldState);
        _state = state;
        if (onStateChange != null) onStateChange(oldState, state);
        if (state.Length > 0)
            notifyEnterStateListeners(state);
    }

    public void updateForCurrentState()
    {
        // We don't need to check stateUpdatesMethods.ContainsKey(_state) && stateUpdatesMethods[_state] != null
        // because setState won't allow invlaid state changes and addState won't allow null methods.

        if (_state.Length > 0)
            stateUpdateMethods[_state]();
    }
}