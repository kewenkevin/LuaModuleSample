using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace ND.UI.NDEvents
{
	public delegate void UnityAction<T1, T2, T3, T4, T5>(T1 arg0, T2 arg1, T3 arg2, T4 arg3, T5 arg4);

	/// <summary>
	/// Events 2.0 for Unity with five generic types
	/// </summary>
	[Serializable]
	public abstract class UnityEvent2<T1, T2, T3, T4, T5> : UnityEventBase2
	{
		private readonly object[] m_InvokeArray = new object[5];

		/// <summary>
		///   <para>Constructor.</para>
		/// </summary>
		public UnityEvent2() { }

		/// <summary>
		/// Add a non persistent listener to the UnityEvent.
		/// </summary>
		/// <param name="call">Callback function.</param>
		public void AddListener(UnityAction<T1, T2, T3, T4, T5> call)
		{
			AddCall(GetDelegate(call));
		}

		/// <summary>
		/// Remove a non persistent listener from the UnityEvent.
		/// </summary>
		/// <param name="call">Callback function.</param>
		public void RemoveListener(UnityAction<T1, T2, T3, T4, T5> call)
		{
			RemoveListener(call.Target, call.Method);
		}

		protected override MethodInfo FindMethod_Impl(Type targetObj, string name)
		{
			return GetValidMethodInfo(targetObj, name, new Type[5] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) });
		}

		internal override BaseInvokableCall2 GetDelegate(object target, MethodInfo theFunction)
		{
			return new InvokableCall2<T1, T2, T3, T4, T5>(target, theFunction);
		}

		private static BaseInvokableCall2 GetDelegate(UnityAction<T1, T2, T3, T4, T5> action)
		{
			return new InvokableCall2<T1, T2, T3, T4, T5>(action);
		}

		/// <summary>
		/// Invoke all registered callbacks (runtime and persistent).
		/// </summary>
		/// <param name="arg1">Dynamic argument 1</param>
		/// <param name="arg2">Dynamic argument 2</param>
		/// <param name="arg3">Dynamic argument 3</param>
		/// <param name="arg4">Dynamic argument 4</param>
		/// <param name="arg5">Dynamic argument 5</param>
		public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			List<BaseInvokableCall2> calls = PrepareInvoke(arg1, arg2, arg3, arg4, arg5);
			for (var i = 0; i < calls.Count; i++)
			{
				var curCall = calls[i] as InvokableCall2<T1, T2, T3, T4, T5>;
				if (curCall != null)
					curCall.Invoke(arg1, arg2, arg3, arg4, arg5);
				else
				{
					var staticCurCall = calls[i] as InvokableCall2;
					if (staticCurCall != null)
						staticCurCall.Invoke();
					else
					{
						m_InvokeArray[0] = arg1;
						m_InvokeArray[1] = arg2;
						m_InvokeArray[2] = arg3;
						m_InvokeArray[3] = arg4;
						m_InvokeArray[4] = arg5;
						calls[i].Invoke(m_InvokeArray);
					}
				}
			}
		}

		internal void AddPersistentListener(UnityAction<T1, T2, T3, T4, T5> call)
		{
			AddPersistentListener(call, UnityEventCallState.RuntimeOnly);
		}

		internal void AddPersistentListener(UnityAction<T1, T2, T3, T4, T5> call, UnityEventCallState callState)
		{
			int persistentEventCount = GetPersistentEventCount();
			AddPersistentListener();
			RegisterPersistentListener(persistentEventCount, call);
			SetPersistentListenerState(persistentEventCount, callState);
		}

		internal void RegisterPersistentListener(int index, UnityAction<T1, T2, T3, T4, T5> call)
		{
			if (call == null)
				Debug.LogWarning("Registering a Listener requires an action");
			else
				RegisterPersistentListener(index, call.Target, call.Method);
		}
	}
}