using UnityEngine;
using System.Collections.Generic;
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
		[SerializeField]
		List<Transform> wheelTransforms;

		Rigidbody2D rigidBody;
		Transform currentTargetNode;
		Animator animator;
		
		readonly int wheatDroppedAnimHash = Animator.StringToHash("isWheatDropped");

		void Awake()
		{
			rigidBody = GetComponent<Rigidbody2D>();
			animator = GetComponent<Animator>();
		}

		void Start()
		{
			currentTargetNode = pathNodes.GetRandomElement();
		}

		void Update()
		{
			if(Input.GetKeyDown(KeyCode.K))
			{
				OnBlockDropped(null);
			}
			
			if((UnityEngine.Object)currentTargetNode == null)
				return;

			var currPos = rigidBody.transform.position;
			var targetPos = currentTargetNode.position;
			
			var targetDir = (targetPos - currPos).normalized;
			var distance = (currPos - targetPos).sqrMagnitude;

			// Debug.Log(distance.ToString(CultureInfo.InvariantCulture));
			if(distance < 3f)
			{
				currentTargetNode = pathNodes.FirstOrDefault(n => n != currentTargetNode);
				return;
			}
			
			rigidBody.MovePosition(currPos + movementVelocity * Time.deltaTime * targetDir);

			foreach(var wheelTransform in wheelTransforms)
			{
				var targetRotation = wheelTransform.localRotation.eulerAngles;
				targetRotation.z += 300f * Time.deltaTime;
				wheelTransform.localEulerAngles = targetRotation;
			}
		}
		
		public void OnBlockDropped(FallingBlock fallingBlock)
		{
			animator.SetTrigger(wheatDroppedAnimHash);
			CameraController.Instance.ShakeCamera();
		}
	}
}