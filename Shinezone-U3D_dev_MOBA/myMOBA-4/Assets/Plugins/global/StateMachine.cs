using System;
using System.Collections.Generic;
using System.Reflection;

public class StateMachine
{
	private DictionaryView<string, IState> _registedState = new DictionaryView<string, IState>();

	private Stack<IState> _stateStack = new Stack<IState>();

	public IState tarState
	{
		get;
		private set;
	}

	public int Count
	{
		get
		{
			return this._stateStack.Count;
		}
	}

	public void RegisterState(string name, IState state)
	{
		if (name == null || state == null)
		{
			return;
		}
		if (this._registedState.ContainsKey(name))
		{
			return;
		}
		this._registedState.Add(name, state);
	}

	public ClassEnumerator RegisterStateByAttributes<TAttributeType>(Assembly InAssembly, params object[] args) where TAttributeType : AutoRegisterAttribute
	{
		ClassEnumerator classEnumerator = new ClassEnumerator(typeof(TAttributeType), typeof(IState), InAssembly, true, false, false);
		ListView<Type>.Enumerator enumerator = classEnumerator.results.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Type current = enumerator.Current;
			IState state = (IState)Activator.CreateInstance(current, args);
			this.RegisterState<IState>(state, state.name);
		}
		return classEnumerator;
	}

	public ClassEnumerator RegisterStateByAttributes<TAttributeType>(Assembly InAssembly) where TAttributeType : AutoRegisterAttribute
	{
		ClassEnumerator classEnumerator = new ClassEnumerator(typeof(TAttributeType), typeof(IState), InAssembly, true, false, false);
		ListView<Type>.Enumerator enumerator = classEnumerator.results.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Type current = enumerator.Current;
			IState state = (IState)Activator.CreateInstance(current);
			this.RegisterState<IState>(state, state.name);
		}
		return classEnumerator;
	}

	public void RegisterState<TStateImplType>(TStateImplType State, string name) where TStateImplType : IState
	{
		this.RegisterState(name, State);
	}

	public IState UnregisterState(string name)
	{
		if (name == null)
		{
			return null;
		}
		IState result;
		if (!this._registedState.TryGetValue(name, out result))
		{
			return null;
		}
		this._registedState.Remove(name);
		return result;
	}

	public IState GetState(string name)
	{
		if (name == null)
		{
			return null;
		}
		IState state;
		IState arg_24_0;
		if (this._registedState.TryGetValue(name, out state))
		{
			IState state2 = state;
			arg_24_0 = state2;
		}
		else
		{
			arg_24_0 = null;
		}
		return arg_24_0;
	}

	public string GetStateName(IState state)
	{
		if (state == null)
		{
			return null;
		}
		DictionaryView<string, IState>.Enumerator enumerator = this._registedState.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<string, IState> current = enumerator.Current;
			if (current.Value == state)
			{
				return current.Key;
			}
		}
		return null;
	}

	public void Push(IState state)
	{
		if (state == null)
		{
			return;
		}
		if (this._stateStack.Count > 0)
		{
			this._stateStack.Peek().OnStateOverride();
		}
		this._stateStack.Push(state);
		state.OnStateEnter();
	}

	public void Push(string name)
	{
		if (name == null)
		{
			return;
		}
		IState state;
		if (!this._registedState.TryGetValue(name, out state))
		{
			return;
		}
		this.Push(state);
	}

	public IState PopState()
	{
		if (this._stateStack.Count <= 0)
		{
			return null;
		}
		IState state = this._stateStack.Pop();
		state.OnStateLeave();
		if (this._stateStack.Count > 0)
		{
			this._stateStack.Peek().OnStateResume();
		}
		return state;
	}

	public IState ChangeState(IState state)
	{
		if (state == null)
		{
			return null;
		}
		this.tarState = state;
		IState state2 = null;
		if (this._stateStack.Count > 0)
		{
			state2 = this._stateStack.Pop();
			state2.OnStateLeave();
		}
		this._stateStack.Push(state);
		state.OnStateEnter();
		return state2;
	}

	public IState ChangeState(string name)
	{
		if (name == null)
		{
			return null;
		}
		IState state;
		if (!this._registedState.TryGetValue(name, out state))
		{
			return null;
		}
		return this.ChangeState(state);
	}

	public IState TopState()
	{
		if (this._stateStack.Count <= 0)
		{
			return null;
		}
		return this._stateStack.Peek();
	}

	public string TopStateName()
	{
		if (this._stateStack.Count <= 0)
		{
			return null;
		}
		IState state = this._stateStack.Peek();
		return this.GetStateName(state);
	}

	public void Clear()
	{
		while (this._stateStack.Count > 0)
		{
			this._stateStack.Pop().OnStateLeave();
		}
	}
}
