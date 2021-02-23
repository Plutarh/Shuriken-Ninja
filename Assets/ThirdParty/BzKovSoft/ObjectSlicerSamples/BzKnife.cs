using BzKovSoft.CharacterSlicerSamples;
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

		public Weapon weapon;

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
		}

        private void OnTriggerEnter(Collider other)
        {
			if (other == null) return;


			var ksa = other.gameObject.GetComponent<KnifeSliceableAsync>();
			if (ksa != null)
			{

				if (sliceable)
				{
					if (ksa.bodyPart == KnifeSliceableAsync.EBodyPart.Head)
					{
						EventService.OnHitEnemyHead?.Invoke();
					}
                    else
                    {
						EventService.OnEnemyHit?.Invoke();
					}
					ksa.BeginSlice(this);
					SliceID++;
				}
				else
				{
					StopSlice();

					
					weapon.transform.root.SetParent(ksa.transform);
					weapon.transform.position = ksa.GetComponent<Collider>().bounds.center;
					weapon.transform.rotation.SetLookRotation(MoveDirection.normalized);

					if (ksa.owner != null)
                    {
						if (ksa.bodyPart == KnifeSliceableAsync.EBodyPart.Head)
						{
							ksa.owner.TakeDamage(weapon.damage * 3);
							EventService.OnHitEnemyHead?.Invoke();
						}
						else
						{ 
							ksa.owner.TakeDamage(weapon.damage);
							EventService.OnEnemyHit?.Invoke();
						}

						if (ksa.owner.health.heathPoint <= 0)
						{
							ksa.GetComponentInParent<CharacterSlicerSampleFast>().ConvertToRagdollSimple(this.MoveDirection.normalized * 2, Vector3.zero);
						}
					}
				}
			}
            else
            {
			

				if (sliceable)
				{
					
					
				}
                else
                {
					var weapon = other.GetComponent<Weapon>();
					if (weapon != null)
					{
						weapon.owner = null;
						weapon.gameObject.GetComponent<Rigidbody>().isKinematic = false;
						weapon.gameObject.GetComponent<Rigidbody>().useGravity = true;
						weapon.transform.SetParent(null);
						Destroy(weapon.gameObject, 3f);
					}
				}
				StopSlice();
				transform.root.SetParent(other.transform);
			}

		
		

		
		}

     
    }
}
