﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jackout.Interaction {
	public class ObjectRotateable : MonoBehaviour {
		public bool rotationEnabled = false;
		public Shared.Axis rotationAxis = Shared.Axis.X;
		public float allowedPositiveOffset = 89.0f; /* in degrees */
		public float allowedNegativeOffset = -10.0f;
		public float centerDeadzone = 15.0f;
		public float initialOffset = 90.0f;
		public GameObject hinge;
		public AudioClip audioFromInitial, audioToInitial, audioDenied;
		public AudioSource audioSource;
		public Vector3 feedbackActiveRotation;
		private Quaternion feedbackInactiveRotation;
		private Quaternion hingeInitialRotation;
		private GameObject pivot;
		private bool inRotation = false;
		private bool inFeedback;
		private float animationStep = 1.0f;

		void Start () {
			if(hinge == null) {
				hinge = gameObject;
			}

			feedbackInactiveRotation = transform.localRotation;
			hingeInitialRotation = hinge.transform.rotation;
		}

		void Update () {
			if(animationStep < 1.0f) {
				if(inFeedback) {
					transform.localRotation = Quaternion.Lerp(transform.localRotation, feedbackInactiveRotation * Quaternion.Euler(feedbackActiveRotation), animationStep);
				}
				else {
					transform.localRotation = Quaternion.Lerp(transform.localRotation, feedbackInactiveRotation, animationStep);
				}
				animationStep += Time.deltaTime;
			}

			if(rotationEnabled && inRotation) {
				hinge.transform.rotation = GetRotationAroundAxis(hinge.transform, pivot.transform, rotationAxis);
			}
		}

		public void RotateTowardPivot(GameObject _pivot) {
			inFeedback = true;
			animationStep = 0.0f;

			if(!rotationEnabled && audioDenied != null) {
				audioSource.clip = audioDenied;
				audioSource.Play();
			}
			else {
				pivot = _pivot;
				inRotation = true;

				if(hinge.transform.rotation == hingeInitialRotation && audioFromInitial != null) {
					audioSource.clip = audioFromInitial;
					audioSource.Play();
				}
			}
		}

		public void StopRotating() {
			inRotation = false;
			inFeedback = false;
			animationStep = 0.0f;

			if(hinge.transform.rotation == hingeInitialRotation && audioToInitial != null) {
				audioSource.clip = audioToInitial;
				audioSource.Play();
			}
		}

		public void Enable() {
			rotationEnabled = true;
		}

		public void Disable() {
			hinge.transform.rotation = hingeInitialRotation;
			rotationEnabled = false;
		}

		private Quaternion GetRotationAroundAxis(Transform from, Transform to, Shared.Axis axis) {
			Vector3 direction = to.position - from.transform.position;
			Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
			Vector3 rotationEuler = rotation.eulerAngles;

			switch(axis) {
				case Shared.Axis.X:
					rotationEuler.x += initialOffset;
					rotationEuler.x = Shared.Map(rotationEuler.x, initialOffset - allowedNegativeOffset, initialOffset + allowedPositiveOffset);
					if(Mathf.Abs(rotationEuler.x - hingeInitialRotation.eulerAngles.x) < centerDeadzone) {
						rotationEuler.x = hingeInitialRotation.eulerAngles.x;
					}
					rotationEuler.y = 0.0f;
					rotationEuler.z = 0.0f;
					break;
				case Shared.Axis.Y:
					rotationEuler.x = 0.0f;
					rotationEuler.y += initialOffset;
					rotationEuler.y = Shared.Map(rotationEuler.y, initialOffset - allowedNegativeOffset, initialOffset + allowedPositiveOffset);
					if(Mathf.Abs(rotationEuler.y - hingeInitialRotation.eulerAngles.y) < centerDeadzone) {
						rotationEuler.y = hingeInitialRotation.eulerAngles.y;
					}
					rotationEuler.z = 0.0f;
					break;
				default:
					rotationEuler.x = 0.0f;
					rotationEuler.y = 0.0f;
					rotationEuler.z += initialOffset;
					rotationEuler.z = Shared.Map(rotationEuler.z, initialOffset - allowedNegativeOffset, initialOffset + allowedPositiveOffset);
					if(Mathf.Abs(rotationEuler.z - hingeInitialRotation.eulerAngles.z) < centerDeadzone) {
						rotationEuler.z = hingeInitialRotation.eulerAngles.z;
					}
					break;
			}

			return Quaternion.Euler(rotationEuler);
		}
	}
}