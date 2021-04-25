using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Neonmoe.MetalMiner {
    public class VolumeManager : MonoBehaviour {
        public float MasterVolume { get; set; }
        public float WindVolume { get; set; }
        public float DrillVolume { get; set; }
        public float DrillingVolume { get; set; }
        public AudioMixer AudioMixer;

        private void Start() {
            MasterVolume = 0.3f;
            WindVolume = 1.0f;
            DrillVolume = 1.0f;
            DrillingVolume = 0.8f;
        }

        private void Update() {
            AudioMixer.SetFloat("Master", MasterVolume == 0.0f ? -80f : (20f * Mathf.Log10(MasterVolume)));
            AudioMixer.SetFloat("Wind", WindVolume == 0.0f ? -80f : (20f * Mathf.Log10(WindVolume)));
            AudioMixer.SetFloat("Drill", DrillVolume == 0.0f ? -80f : (20f * Mathf.Log10(DrillVolume)));
            AudioMixer.SetFloat("Drilling", DrillingVolume == 0.0f ? -80f : (20f * Mathf.Log10(DrillingVolume)));
        }
    }
}
