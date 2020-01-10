using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Es.InkPainter.Sample
{
	[RequireComponent(typeof(Collider), typeof(MeshRenderer))]
	public class CollisionPainter : MonoBehaviour
	{
		[SerializeField]
		private Brush brush = null;

		[SerializeField]
		private int wait = 3;

		private int waitCount;

//        List<Collision> collisions = new List<Collision>();

        public void Awake()
		{
			GetComponent<MeshRenderer>().material.color = brush.Color;
		}

		public void FixedUpdate()
		{
			++waitCount;

 //           foreach (Collision collision in collisions)
 //           {
 //               foreach (ContactPoint contact in collision.contacts)
 //               {
 //                  Debug.DrawRay(contact.point, contact.normal, Color.blue);
 //               }
 //           }
        }

		public void OnCollisionStay(Collision collision)
		{
			if(waitCount < wait)
				return;
			waitCount = 0;

			foreach(var p in collision.contacts)
			{
				var canvas = p.otherCollider.GetComponent<InkCanvas>();
				if(canvas != null)
					canvas.Paint(brush, p.point);

                //                if (!collisions.Contains(collision))
                //                    collisions.Add(collision);
 //               Debug.Log("+++" + collision.GetContacts(collision.contacts));
               
            }
		}

//        void OnCollisionEnter(Collision collision)
//        {
//            if (!collisions.Contains(collision))
//                collisions.Add(collision);
//        }
    }
}