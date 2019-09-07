using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WSGJ.Utils;

namespace WSGJ
{
	public class TruckController : MonoBehaviour
	{
		[SerializeField, Header("Truck Settings")]
		List<Transform> pathNodes;

		[SerializeField]
		float movementVelocity = 2f;

		Rigidbody2D rigidBody;
		Transform currentTargetNode;
		bool wasLastNodeLeft = false; 

		void Awake()
		{
			rigidBody = GetComponent<Rigidbody2D>();
		}

		void Start()
		{
			currentTargetNode = pathNodes.GetRandomElement();
			wasLastNodeLeft = pathNodes.IndexOf(currentTargetNode) == 0;
		}

		void Update()
		{
			if((UnityEngine.Object)currentTargetNode == null)
				return;

			var currPos = rigidBody.transform.position;
			var targetPos = currentTargetNode.position;
			
			var targetDir = (targetPos - currPos).normalized;
			var distance = (currPos - targetPos).sqrMagnitude;

			// Debug.Log(distance.ToString(CultureInfo.InvariantCulture));
			if(distance < 2f)
			{
				currentTargetNode = wasLastNodeLeft
					? pathNodes.LastOrDefault() 
					: pathNodes.FirstOrDefault();
				wasLastNodeLeft = !wasLastNodeLeft;
				return;
			}
			
			rigidBody.MovePosition(currPos + movementVelocity * Time.deltaTime * targetDir);
		}
		
		public void OnFallingBlockAttached(FallingBlock fallingBlock)
		{
			
		}
	}
}