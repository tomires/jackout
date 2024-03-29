﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jackout.Logic {
	public class ElevatorControl : PuzzleControl {
		public StateControl stateControl;
		public Interaction.PuzzleButton[] puzzleButtons;
		public GateControl elevatorDoor1, elevatorDoor2;
		public Interaction.ElevatorFloorButton secondFloorButton;
		public AudioSource audioSource;
		public AudioClip audioElevatorEngine;
		public float timeToReachDestination = 11.0f;
		private float animationStep;
		private bool inMotion = false;
		private bool currentlyOnUpperFloor = false;
		private bool audioTriggered = false;

		void Setup () {
			animationStep = timeToReachDestination;
		}

		void Update () {
			if(inMotion) {
				if(animationStep > (elevatorDoor1.openingTime + 0.5f) && !audioTriggered) {
					audioSource.clip = audioElevatorEngine;
					audioSource.Play();
					audioTriggered = true;

					/* since the player doesn't see outside, change scene */
					if(currentlyOnUpperFloor) {
						stateControl.ChangeState(Shared.State.AptArrivedUpperFloor);
					}
					else {
						stateControl.ChangeState(Shared.State.AptArrivedLowerFloor);
					}
				}
				else if(animationStep > (timeToReachDestination - 0.5f)) {
					elevatorDoor1.Switch(true);
					elevatorDoor2.Switch(true);
					inMotion = false;
				}
				animationStep += Time.deltaTime;
			}
		}

		public void GoToFloor(bool upperFloor) {
			if(currentlyOnUpperFloor == upperFloor) {
				elevatorDoor1.Switch(true);
				elevatorDoor2.Switch(true);
				return;
			}

			currentlyOnUpperFloor = upperFloor;
			inMotion = true;
			audioTriggered = false;
			animationStep = 0.0f;

			elevatorDoor1.Switch(false);
			elevatorDoor2.Switch(false);
		}

		public override void CheckButtonStates(Interaction.PuzzleButton button) {
			/* if we reach a specific combination, unlock button for second floor */
			if(puzzleButtons[0].currentIcon == 0 &&
				puzzleButtons[1].currentIcon == 1 &&
				puzzleButtons[2].currentIcon == 2 &&
				puzzleButtons[3].currentIcon == 3) {
				
				secondFloorButton.SetEnabled(true);
			}
			else {
				secondFloorButton.SetEnabled(false);
			}
		}
	}
}