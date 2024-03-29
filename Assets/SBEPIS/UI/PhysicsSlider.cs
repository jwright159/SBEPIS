using System;
using UnityEngine;
using UnityEngine.Events;

namespace SBEPIS.UI
{
	[RequireComponent(typeof(Rigidbody), typeof(ConfigurableJoint))]
	public class PhysicsSlider : MonoBehaviour
	{
		public ButtonAxis axis;

		public UnityEvent<float> onProgressChanged = new();

		[NonSerialized]
		public float progress;
		private float lastProgress;

		public new Rigidbody rigidbody { get; private set; }
		public ConfigurableJoint joint { get; private set; }

		private Vector3 initialRelativeConnectedAnchor;
		private Vector3 initialLocalRotation;

		private void Awake()
		{
			rigidbody = GetComponent<Rigidbody>();
			joint = GetComponent<ConfigurableJoint>();
		}

		private void Start()
		{
			initialLocalRotation = transform.localRotation.eulerAngles;
			joint.autoConfigureConnectedAnchor = false;
			initialRelativeConnectedAnchor = joint.connectedAnchor - (transform.parent ? transform.parent.position : Vector3.zero);
		}

		private void Update()
		{
			Evaluate();
		}

		protected virtual void Evaluate()
		{
			lastProgress = progress;
			progress = RelativeProgress;
			if (lastProgress != progress)
				onProgressChanged.Invoke(progress);
		}

		protected float RelativeProgress
		{
			get
			{
				switch (axis)
				{
					case ButtonAxis.XPosition:
					case ButtonAxis.YPosition:
					case ButtonAxis.ZPosition:
						return GetDirectionValue(transform.position).Map(StartDirectionValue, EndDirectionValue, 0, 1);

					case ButtonAxis.XRotation:
					case ButtonAxis.YRotation:
					case ButtonAxis.ZRotation:
						return GetDirectionValue(transform.localRotation.eulerAngles - initialLocalRotation).Map(StartDirectionValue, EndDirectionValue, 0, 1);

					default:
						return 0;
				}
			}
		}

		protected void SetRelativeProgress(float progress)
		{
			switch (axis)
			{
				case ButtonAxis.XPosition:
				case ButtonAxis.YPosition:
				case ButtonAxis.ZPosition:
					Vector3 position = transform.position;
					SetDirectionValue(ref position, progress.Map(0, 1, StartDirectionValue, EndDirectionValue));
					transform.position = position;
					return;

				case ButtonAxis.XRotation:
				case ButtonAxis.YRotation:
				case ButtonAxis.ZRotation:
					Vector3 eulers = transform.localRotation.eulerAngles;
					SetDirectionValue(ref eulers, progress.Map(0, 1, StartDirectionValue, EndDirectionValue));
					eulers += initialLocalRotation;
					transform.localRotation = Quaternion.Euler(eulers);
					return;

				default:
					return;
			}
		}

		protected float StartDirectionValue => GetDirectionValue(StartPoint);

		protected float EndDirectionValue => GetDirectionValue(EndPoint);

		protected Vector3 StartPoint
		{
			get
			{
				switch (axis)
				{
					case ButtonAxis.XPosition:
					case ButtonAxis.YPosition:
					case ButtonAxis.ZPosition:
						return Anchor - joint.linearLimit.limit * GetAxis(transform);

					case ButtonAxis.XRotation:
					case ButtonAxis.YRotation:
					case ButtonAxis.ZRotation:
						return LowRotationalLimit * Axis;

					default:
						return Vector3.zero;
				}
			}
		}

		protected Vector3 EndPoint
		{
			get
			{
				switch (axis)
				{
					case ButtonAxis.XPosition:
					case ButtonAxis.YPosition:
					case ButtonAxis.ZPosition:
						return Anchor + joint.linearLimit.limit * GetAxis(transform);

					case ButtonAxis.XRotation:
					case ButtonAxis.YRotation:
					case ButtonAxis.ZRotation:
						return HighRotationalLimit * Axis;

					default:
						return Vector3.zero;
				}
			}
		}

		protected Vector3 Anchor => joint.connectedBody ? joint.connectedBody.transform.TransformPoint(joint.connectedAnchor) : joint.connectedAnchor;

		protected float LowRotationalLimit
		{
			get
			{
				switch (axis)
				{
					case ButtonAxis.XRotation:
						return joint.lowAngularXLimit.limit;

					case ButtonAxis.YRotation:
						return -joint.angularYLimit.limit;

					case ButtonAxis.ZRotation:
						return -joint.angularZLimit.limit;

					default:
						return 0;
				}
			}
		}

		protected float HighRotationalLimit
		{
			get
			{
				switch (axis)
				{
					case ButtonAxis.XRotation:
						return joint.highAngularXLimit.limit;

					case ButtonAxis.YRotation:
						return joint.angularYLimit.limit;

					case ButtonAxis.ZRotation:
						return joint.angularZLimit.limit;

					default:
						return 0;
				}
			}
		}

		protected Vector3 Axis
		{
			get
			{
				switch (axis)
				{
					case ButtonAxis.XPosition:
					case ButtonAxis.XRotation:
						return Vector3.right;

					case ButtonAxis.YPosition:
					case ButtonAxis.YRotation:
						return Vector3.up;

					case ButtonAxis.ZPosition:
					case ButtonAxis.ZRotation:
						return Vector3.forward;

					default:
						return Vector3.zero;
				}
			}
		}

		protected Vector3 GetAxis(Transform transform)
		{
			switch (axis)
			{
				case ButtonAxis.XPosition:
				case ButtonAxis.XRotation:
					return transform.right;

				case ButtonAxis.YPosition:
				case ButtonAxis.YRotation:
					return transform.up;

				case ButtonAxis.ZPosition:
				case ButtonAxis.ZRotation:
					return transform.forward;

				default:
					return Vector3.zero;
			}
		}

		protected float GetDirectionValue(Vector3 vector)
		{
			switch (axis)
			{
				case ButtonAxis.XPosition:
				case ButtonAxis.XRotation:
					return vector.x;

				case ButtonAxis.YPosition:
				case ButtonAxis.YRotation:
					return vector.y;

				case ButtonAxis.ZPosition:
				case ButtonAxis.ZRotation:
					return vector.z;

				default:
					return 0;
			}
		}

		protected void SetDirectionValue(ref Vector3 vector, float value)
		{
			switch (axis)
			{
				case ButtonAxis.XPosition:
				case ButtonAxis.XRotation:
					vector.x = value;
					return;

				case ButtonAxis.YPosition:
				case ButtonAxis.YRotation:
					vector.y = value;
					return;

				case ButtonAxis.ZPosition:
				case ButtonAxis.ZRotation:
					vector.z = value;
					return;

				default:
					return;
			}
		}

		public void ResetAnchor(float progress)
		{
			joint.connectedAnchor = initialRelativeConnectedAnchor + (transform.parent ? transform.parent.position : Vector3.zero);
			SetRelativeProgress(progress);
		}

		public void Yeah()
		{
			print(gameObject + " " + progress);
		}

		public enum ButtonAxis
		{
			XPosition, YPosition, ZPosition,
			XRotation, YRotation, ZRotation
		}
	}
}