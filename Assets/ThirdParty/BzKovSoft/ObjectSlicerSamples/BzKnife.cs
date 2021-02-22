using System;
using System.Collections.Generic;
using UnityEngine;

namespace BzKovSoft.ObjectSlicerSamples
{
	/// <summary>
	/// The script must be attached to a GameObject that have collider marked as a "IsTrigger".
	/// </summary>
	public class BzKnife : MonoBehaviour
	{
		public int SliceID { get; set; }
		Vector3 _prevPos;
		Vector3 _pos;

		[SerializeField]
		private Vector3 _origin = Vector3.down;

		[SerializeField]
		private Vector3 _direction = Vector3.up;

		public Action OnSliceBegin;
		public Action OnStopSlice;

		public int durability;

		public bool sliceable;

        private void Awake()
        {
			//sliceable = false;

		}

        private void Update()
		{
			_prevPos = _pos;
			_pos = transform.position;
		}

		public Vector3 Origin
		{
			get
			{
				Vector3 localShifted = transform.InverseTransformPoint(transform.position) + _origin;
				return transform.TransformPoint(localShifted);
			}
		}

		public Vector3 BladeDirection { get { return transform.rotation * _direction.normalized; } }
		public Vector3 MoveDirection { get { return (_pos - _prevPos).normalized; } }

		public void BeginNewSlice()
		{
			durability--;
			OnSliceBegin?.Invoke();
			
		}

		public void StopSlice()
        {
			OnStopSlice?.Invoke();
			GetComponent<Collider>().enabled = false;
			Debug.LogError("Stop slice");
		}

        private void OnTriggerEnter(Collider other)
        {
			if (other == null) return;

            if (sliceable)
            {
				var ksa = other.gameObject.GetComponent<KnifeSliceableAsync>();
				if(ksa != null)
                {
					ksa.BeginSlice(this);
                }
			}
            else
            {
				StopSlice();
				transform.root.SetParent(other.transform);
			}
		

		
		}

     
    }
}
