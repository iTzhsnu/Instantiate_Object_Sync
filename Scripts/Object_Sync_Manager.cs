
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class Object_Sync_Manager : UdonSharpBehaviour {
    public Object_Manager[] syncs;

    public void Assign() {
        VRCPlayerApi[] players = VRCPlayerApi.GetPlayers(new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()]);

        for (int i = 0; players.Length > i; ++i) {
            Networking.SetOwner(players[i], syncs[i].gameObject);
        }

        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Syncs_SetActive");
    }

    public void Syncs_SetActive() {
        VRCPlayerApi[] players = VRCPlayerApi.GetPlayers(new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()]);

        for (int i = 0; players.Length > i; ++i) {
            syncs[i].gameObject.SetActive(true);
            syncs[i].InitManager();
        }

        if (syncs.Length > players.Length) {
            for (int i = players.Length; syncs.Length > i; ++i) {
                syncs[i].gameObject.SetActive(false);
            }
        }
    }

    public override void OnPlayerLeft(VRCPlayerApi player) {
        if (Networking.LocalPlayer.IsOwner(this.gameObject)) {
            Assign();
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player) {
        if (Networking.LocalPlayer.IsOwner(this.gameObject)) {
            Assign();
        }
    }

    public int AddObject(GameObject o) {
        GameObject[] objects = new GameObject[syncs[0].sync_objects.Length + 1];

        for (int i = 0; syncs[0].sync_objects.Length > i; ++i) {
            objects[i] = syncs[0].sync_objects[i];
        }

        objects[syncs[0].sync_objects.Length] = o;

        foreach (Object_Manager manager in syncs) {
            manager.sync_objects = objects;
        }

        return syncs[0].sync_objects.Length;
    }

    public void RemoveObject(int pos) {
        GameObject[] objects = new GameObject[syncs[0].sync_objects.Length - 1];

        for (int i = 0; syncs[0].sync_objects.Length > i; ++i) {
            if (i == pos) continue;
            if (i > pos) {
                objects[i - 1] = syncs[0].sync_objects[i - 1];
            } else {
                objects[i] = syncs[0].sync_objects[i];
            }
        }

        foreach (Object_Manager manager in syncs) {
            manager.sync_objects = objects;
        }
    }
}
