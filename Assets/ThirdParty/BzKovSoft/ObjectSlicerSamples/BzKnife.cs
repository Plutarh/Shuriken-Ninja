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

		public List<Pawn> hittedPawns = new List<Pawn>();
		public int triggerCount;

		public Transform stopPos;

		public Vector3 hitRotation;

        private void Awake()
        {
			//sliceable = false;
			SliceID = UnityEngine.Random.Range(1, 10000);
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
			if (ksa != null && ksa.owner != null)
			{
				if (hittedPawns.Contains(ksa.owner) || ksa.owner.characterSlicer.sliced) return;
				hittedPawns.Add(ksa.owner);
				triggerCount++;
				if (sliceable && weapon.durability > 0)
				{
					if (ksa.bodyPart == KnifeSliceableAsync.EBodyPart.Head)
					{
						EventService.OnHitEnemyHead?.Invoke();
						//Debug.LogError($"HEAD {ksa.owner}", ksa);
					}
                    else
                    {
						EventService.OnEnemyHit?.Invoke();
						//Debug.LogError($"HIT {ksa.owner}", ksa);
					}
					ksa.BeginSlice(this);
					weapon.durability--;
				}
				else
				{
					StopSlice();
					
					weapon.transform.root.SetParent(ksa.transform);
					weapon.transform.position = ksa.GetComponent<Collider>().bounds.center;
					weapon.transform.rotation.SetLookRotation(MoveDirection.normalized);
					weapon.transform.position = stopPos.position;

					if(hitRotation != Vector3.zero)
                    {
						(weapon as Shuriken).secondaryRotateObject.transform.rotation = Quaternion.identity;
						weapon.transform.rotation = Quaternion.Euler(hitRotation);

					}

					if (ksa.owner != null)
                    {
						if (ksa.bodyPart == KnifeSliceableAsync.EBodyPart.Head)
						{
							ksa.owner.TakeDamage(weapon.damage * 3, MoveDirection, EDamageType.Hit);
							EventService.OnHitEnemyHead?.Invoke();
						}
						else
						{ 
							ksa.owner.TakeDamage(weapon.damage, MoveDirection, EDamageType.Hit);
							EventService.OnEnemyHit?.Invoke();
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
						//weapon.transform.SetParent(null);
						

						weapon.transform.root.SetParent(ksa.transform);
						weapon.transform.position = ksa.GetComponent<Collider>().bounds.center;
						weapon.transform.rotation.SetLookRotation(MoveDirection.normalized);
						weapon.transform.position = stopPos.position;
						Destroy(weapon.gameObject, 3f);
					}
				}
				StopSlice();
				
				GetComponent<Collider>().enabled = false;
			}
		
		}

     
    }
}
