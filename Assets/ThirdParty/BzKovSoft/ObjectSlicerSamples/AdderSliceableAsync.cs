using UnityEngine;
using Zenject;

namespace BzKovSoft.ObjectSlicerSamples
{
	public class AdderSliceableAsync : MonoBehaviour
	{

		TimeControllService timeService;

		[Inject]
		void Construct(TimeControllService timeControll)
		{
			timeService = timeControll;
		}

		void Start()
		{
			
		}

		public void SetupSliceableParts(Pawn _owner)
        {
			var rigids = GetComponentsInChildren<Rigidbody>();

			for (int i = 0; i < rigids.Length; i++)
			{
				var rigid = rigids[i];
				var go = rigid.gameObject;

				if (go == gameObject)
					continue;

				if (go.GetComponent<KnifeSliceableAsync>() != null)
					continue;

				var ksa = go.AddComponent<KnifeSliceableAsync>();

				
				if(ksa != null)
                {
					ksa.owner = _owner;
					ksa.timeService = timeService;
				}
				

			}
		}
	}
}