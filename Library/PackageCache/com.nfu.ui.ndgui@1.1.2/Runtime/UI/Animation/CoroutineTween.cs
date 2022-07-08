using System.Collections;
using UnityEngine.Events;

using UnityEngine;

namespace ND.UI.NDUI.CoroutineTween
{
	// Base interface for tweeners,
	// using an interface instead of
	// an abstract class as we want the
	// tweens to be structs.
	public interface INDTweenValue
	{
		void TweenValue(float floatPercentage, bool finish);
		bool ignoreTimeScale { get; }
		float duration { get; }
		bool ValidTarget();
	}

	// Color tween class, receives the
	// TweenValue callback and then sets
	// the value on the target.
	struct ColorTween2 : INDTweenValue
	{
		public enum ColorTweenMode
		{
			All,
			RGB,
			Alpha
		}

		public class ColorTweenCallback : UnityEvent<Color> { }

		private ColorTweenCallback m_Target;
		private Color m_StartColor;
		private Color m_TargetColor;
		private ColorTweenMode m_TweenMode;

		private float m_Duration;
		private bool m_IgnoreTimeScale;

		public Color startColor
		{
			get { return m_StartColor; }
			set { m_StartColor = value; }
		}

		public Color targetColor
		{
			get { return m_TargetColor; }
			set { m_TargetColor = value; }
		}

		public ColorTweenMode tweenMode
		{
			get { return m_TweenMode; }
			set { m_TweenMode = value; }
		}

		public float duration
		{
			get { return m_Duration; }
			set { m_Duration = value; }
		}

		public bool ignoreTimeScale
		{
			get { return m_IgnoreTimeScale; }
			set { m_IgnoreTimeScale = value; }
		}

		public void TweenValue(float floatPercentage, bool finish)
		{
			if (!ValidTarget())
				return;

			var newColor = Color.Lerp(m_StartColor, m_TargetColor, floatPercentage);

			if (m_TweenMode == ColorTweenMode.Alpha)
			{
				newColor.r = m_StartColor.r;
				newColor.g = m_StartColor.g;
				newColor.b = m_StartColor.b;
			}
			else if (m_TweenMode == ColorTweenMode.RGB)
			{
				newColor.a = m_StartColor.a;
			}
			m_Target.Invoke(newColor);
		}

		public void AddOnChangedCallback(UnityAction<Color> callback)
		{
			if (m_Target == null)
				m_Target = new ColorTweenCallback();

			m_Target.AddListener(callback);
		}

		public bool GetIgnoreTimescale()
		{
			return m_IgnoreTimeScale;
		}

		public float GetDuration()
		{
			return m_Duration;
		}

		public bool ValidTarget()
		{
			return m_Target != null;
		}
	}

	// Float tween class, receives the
	// TweenValue callback and then sets
	// the value on the target.
	struct IndFloatTween : INDTweenValue
	{
		public class FloatTweenCallback : UnityEvent<float, bool> { }

		private FloatTweenCallback m_Target;
		private float m_StartValue;
		private float m_TargetValue;

		private float m_Duration;
		private bool m_IgnoreTimeScale;

		public float startValue
		{
			get { return m_StartValue; }
			set { m_StartValue = value; }
		}

		public float targetValue
		{
			get { return m_TargetValue; }
			set { m_TargetValue = value; }
		}

		public float duration
		{
			get { return m_Duration; }
			set { m_Duration = value; }
		}

		public bool ignoreTimeScale
		{
			get { return m_IgnoreTimeScale; }
			set { m_IgnoreTimeScale = value; }
		}

		public void TweenValue(float floatPercentage, bool finish)
		{
			if (!ValidTarget())
				return;

			var newValue = Mathf.Lerp(m_StartValue, m_TargetValue, floatPercentage);
			m_Target.Invoke(newValue, finish);
		}

		public void AddOnChangedCallback(UnityAction<float,bool> callback)
		{
			if (m_Target == null)
				m_Target = new FloatTweenCallback();

			m_Target.AddListener(callback);
		}

		public bool GetIgnoreTimescale()
		{
			return m_IgnoreTimeScale;
		}

		public float GetDuration()
		{
			return m_Duration;
		}

		public bool ValidTarget()
		{
			return m_Target != null;
		}
	}


	// Float tween class, receives the
	// TweenValue callback and then sets
	// the value on the target.
	public struct IndAnimationCurveTween : INDTweenValue
	{
		public class FloatTweenCallback : UnityEvent<float> { }

		private FloatTweenCallback m_Target;

		private float m_Duration;
		private bool m_IgnoreTimeScale;

		private float m_scaleTime;

		private AnimationCurve m_AnimationCurve;
		public AnimationCurve animationCurve
        {
			get { return m_AnimationCurve; }
			set { m_AnimationCurve = value;
				  m_Duration = m_AnimationCurve[m_AnimationCurve.length - 1].time;
				  m_scaleTime = 1;
			}
        }

		public float duration
		{
			get { return m_Duration; }
			set{
				if (value <= 0) return;
				m_Duration = value;
				Debug.Assert(m_AnimationCurve != null);
				m_scaleTime = m_AnimationCurve[m_AnimationCurve.length - 1].time / value;
			}
		}

		public bool ignoreTimeScale
		{
			get { return m_IgnoreTimeScale; }
			set { m_IgnoreTimeScale = value; }
		}

		public void TweenValue(float floatPercentage, bool finish)
		{
			if (!ValidTarget())
				return;

			var newValue = m_AnimationCurve.Evaluate(duration * m_scaleTime * floatPercentage);
			m_Target.Invoke(newValue);
		}

		private bool CurveVaild(AnimationCurve curve)
		{
			return (curve != null && curve.length > 0);
		}

		public void AddOnChangedCallback(UnityAction<float> callback)
		{
			if (m_Target == null)
				m_Target = new FloatTweenCallback();

			m_Target.AddListener(callback);
		}

		public bool GetIgnoreTimescale()
		{
			return m_IgnoreTimeScale;
		}

		public float GetDuration()
		{
			return m_Duration;
		}

		public bool ValidTarget()
		{
			return m_Target != null && CurveVaild(m_AnimationCurve);
		}
	}


	// Tween runner, executes the given tween.
	// The coroutine will live within the given
	// behaviour container.
	public class NDTweenRunner<T> where T : struct, INDTweenValue
	{
		protected MonoBehaviour m_CoroutineContainer;
		protected IEnumerator m_Tween;

		// utility function for starting the tween
		private static IEnumerator Start(T tweenInfo)
		{
			if (!tweenInfo.ValidTarget())
				yield break;

			var elapsedTime = 0.0f;
			while (elapsedTime < tweenInfo.duration)
			{
				elapsedTime += tweenInfo.ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
				var percentage = Mathf.Clamp01(elapsedTime / tweenInfo.duration);
				tweenInfo.TweenValue(percentage, false);
				yield return null;
			}
			tweenInfo.TweenValue(1.0f, true);
		}

		public void Init(MonoBehaviour coroutineContainer)
		{
			m_CoroutineContainer = coroutineContainer;
		}

		public void StartTween(T info)
		{
			if (m_CoroutineContainer == null)
			{
				Debug.LogWarning("Coroutine container not configured... did you forget to call Init?");
				return;
			}

			StopTween();

			if (!m_CoroutineContainer.gameObject.activeInHierarchy)
			{
				info.TweenValue(1.0f, true);
				return;
			}

			m_Tween = Start(info);
			m_CoroutineContainer.StartCoroutine(m_Tween);
		}

		public void StopTween()
		{
			if (m_Tween != null)
			{
				m_CoroutineContainer.StopCoroutine(m_Tween);
				m_Tween = null;
			}
		}
	}
}