using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyUtility
{
    public class PositionClass : MonoBehaviour
    {
        public static Vector3 MoveTowards(Transform obj, Transform target)
        {
            Vector3 direction = target.transform.position - obj.transform.position;
            Vector3 transformDir = obj.transform.TransformDirection(direction);
            return transformDir;
        }
    }

    public class RotationClass: MonoBehaviour
    {
        public static Quaternion LookAtTarget(Transform target)
        {

            Quaternion rot = Quaternion.LookRotation(target.transform.position);

            return rot;
        }
        public static Quaternion LookAtTarget(Vector3 target)
        {
            Quaternion rot = Quaternion.LookRotation(target);
            return rot;
        }
        public static Quaternion LookAtTarget(Transform obj, Transform target)
        {
            Vector3 direction = target.position - obj.position;
            direction.y = 0;
            Quaternion rot = Quaternion.LookRotation(direction);
            return rot;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj">The object to rotate</param>
        /// <param name="target">the target the object should rotate to</param>
        /// <param name="rotToExclude">the direction in which the rotation would be excluded</param>
        /// <returns></returns>
        public static Quaternion LookAtTarget(Transform obj, Transform target, char rotToExclude)
        {
            Vector3 direction = target.position - obj.position;

            if (rotToExclude == 'x') { direction.x = 0; } else if (rotToExclude == 'y') { direction.y = 0; } else if (rotToExclude == 'z') { direction.z = 0; }

            Quaternion rot = Quaternion.LookRotation(direction);
            return rot;
        }

        public static Quaternion RotateTowardsAtDirection(Transform targetPosition, Transform obj, float turnSpeed)
        {
            Vector3 targetDirection = targetPosition.position - obj.position;
            Vector3 objCurrentDirection = obj.position;

            Vector3 resultDirection = Vector3.RotateTowards(objCurrentDirection, targetDirection, turnSpeed* Mathf.Deg2Rad * Time.deltaTime, 1f);
            Quaternion finalRot = Quaternion.LookRotation(resultDirection);

            return finalRot;
        }

    }
}