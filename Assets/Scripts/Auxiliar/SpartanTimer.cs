using System;
using UnityEngine;

namespace Auxiliars {

	public enum TimeMode {
		Fixed,
		Framed,
		RealTime
	}

	public enum TimeScaleMode {
		Seconds = 0,
		Milliseconds,
		Minutes
	}

	public struct SpartanTimer {
		private static readonly float[] SCALING_VALUES = { 1f, 1000f, 0.0166667f };
		//TODO: Make sure this is dynamic
		public float CurrentTimeMS => GetCurrentTime(TimeScaleMode.Milliseconds);

		public bool Started { get; private set; }

		public TimeMode ReferenceTimeMode { get; private set; }

		private float startingTime;
		private DateTime realTimeStartingTime;


		public SpartanTimer(TimeMode timeMode) {
			this.ReferenceTimeMode = timeMode;
			this.startingTime = 0f;
			this.Started = false;
			this.realTimeStartingTime = DateTime.MinValue;
		}

		public void Start() {
			if (Started) return;
			switch (ReferenceTimeMode) {
				case TimeMode.Fixed:
					this.startingTime = Time.fixedTime;
					break;
				case TimeMode.Framed:
					this.startingTime = Time.time;
					break;
				case TimeMode.RealTime:
					this.realTimeStartingTime = DateTime.Now;
					break;
				default:
					throw new Exception("No proper time mode was found!");
			}
			this.Started = true;
		}

		public float GetCurrentTime(TimeScaleMode scaleMode) {
			if (!this.Started) return 0f;
			float finalTime = 0f;
			switch (ReferenceTimeMode) {
				case TimeMode.Fixed:
					finalTime = (Time.fixedTime - this.startingTime) * SCALING_VALUES[(int)scaleMode];
					break;
				case TimeMode.Framed:
					finalTime = (Time.time - this.startingTime) * SCALING_VALUES[(int)scaleMode];
					break;
				case TimeMode.RealTime:
					TimeSpan diff = DateTime.Now - this.realTimeStartingTime;
					finalTime = (float)(diff.TotalSeconds * SCALING_VALUES[(int)scaleMode]);
					break;
			}
			return finalTime;
		}

		public void Stop() {
			this.startingTime = 0f;
			this.Started = false;
		}

		public float Reset() {
			float result = GetCurrentTime(TimeScaleMode.Milliseconds);
			this.Stop();
			Start();
			return result;
		}

	}
}
