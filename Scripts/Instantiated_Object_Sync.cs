
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class Instantiated_Object_Sync : UdonSharpBehaviour {
    public bool drop = false;
    public bool pickup = false;

    public override void OnPickup() {
        pickup = true;
    }

    public override void OnDrop() {
        drop = true;
    }

    public void SystemDone() {
        pickup = false;
        drop = false;
    }
}
