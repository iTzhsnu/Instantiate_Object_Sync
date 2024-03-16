
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using System;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class Object_Manager_Sub : UdonSharpBehaviour {
    //Smoothing
    [NonSerialized] public bool tracking = false;
    [NonSerialized] public bool drop = false;
    [NonSerialized] public long nano_sec_old = 0;
    [NonSerialized] public float move_x = 0;
    [NonSerialized] public float move_y = 0;
    [NonSerialized] public float move_z = 0;
    [NonSerialized] public float rot_x = 0;
    [NonSerialized] public float rot_y = 0;
    [NonSerialized] public float rot_z = 0;

    //UdonSynced
    [NonSerialized] [UdonSynced] public Vector3 pos;
    [NonSerialized] [UdonSynced] public Vector3 rot;
    [NonSerialized] [UdonSynced] public ushort obj_id;

    private void Update() {
        if (tracking) {
            SyncSmoothing();
        }
    }

    public void InitManager() {
        if (GetComponent<Object_Manager>().sync_objects.Length > obj_id) {
            drop = true;
            SyncObject(obj_id);
        } else {
            drop = false;
            tracking = false;
        }
    }

    public void SyncSmoothing() {
        Vector3 pos_old = GetComponent<Object_Manager>().sync_objects[obj_id].transform.localPosition;
        Vector3 rot_old = GetComponent<Object_Manager>().sync_objects[obj_id].transform.eulerAngles;

        GetComponent<Object_Manager>().sync_objects[obj_id].transform.localPosition = new Vector3(pos_old.x + move_x * Time.deltaTime, pos_old.y + move_y * Time.deltaTime, pos_old.z + move_z * Time.deltaTime);
        GetComponent<Object_Manager>().sync_objects[obj_id].transform.eulerAngles = new Vector3(rot_old.x + rot_x * Time.deltaTime, rot_old.y + rot_y * Time.deltaTime, rot_old.z + move_z * Time.deltaTime);
    }

    public void SyncObject(ushort id) {
        pos = GetComponent<Object_Manager>().sync_objects[id].transform.localPosition;
        rot = GetComponent<Object_Manager>().sync_objects[id].transform.eulerAngles;
        obj_id = id;

        RequestSerialization();
        if (drop) {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "SyncToEveryone_Drop_Sub");
        } else {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "SyncToEveryone_Sub");
        }
    }

    public void SyncToEveryone_Sub() {
        if (GetComponent<Object_Manager>().sync_objects.Length > obj_id) {
            if (!tracking) {
                if (!Networking.LocalPlayer.IsOwner(this.gameObject)) {
                    GetComponent<Object_Manager>().sync_objects[obj_id].transform.localPosition = pos;
                    GetComponent<Object_Manager>().sync_objects[obj_id].transform.eulerAngles = rot;
                }

                nano_sec_old = DateTime.Now.Ticks;
                tracking = true;
            } else {
                long mili_sec = (DateTime.Now.Ticks - nano_sec_old) / 10000;
                float sec = mili_sec / 1000F;
                Vector3 obj_pos = GetComponent<Object_Manager>().sync_objects[obj_id].transform.localPosition;
                Vector3 obj_rot = GetComponent<Object_Manager>().sync_objects[obj_id].transform.eulerAngles;
                move_x = (pos.x - obj_pos.x) / sec;
                move_y = (pos.y - obj_pos.y) / sec;
                move_z = (pos.z - obj_pos.z) / sec;

                float rot_x_fixed = obj_rot.x;
                if (rot_x_fixed < 0) rot_x_fixed += 360;
                if (rot.x < 0) {
                    rot_x = (rot.x + 360 - rot_x_fixed) / sec;
                } else {
                    rot_x = (rot.x - rot_x_fixed) / sec;
                }

                float rot_y_fixed = obj_rot.y;
                if (rot_y_fixed < 0) rot_y_fixed += 360;
                if (rot.y < 0) {
                    rot_y = (rot.y + 360 - rot_y_fixed) / sec;
                } else {
                    rot_y = (rot.y - rot_y_fixed) / sec;
                }

                float rot_z_fixed = obj_rot.z;
                if (rot_z_fixed < 0) rot_z_fixed += 360;
                if (rot.z < 0) {
                    rot_z = (rot.z + 360 - rot_z_fixed) / sec;
                } else {
                    rot_z = (rot.z - rot_z_fixed) / sec;
                }

                GetComponent<Object_Manager>().sync_objects[obj_id].transform.eulerAngles = new Vector3(rot_x_fixed, rot_y_fixed, rot_z_fixed);

                nano_sec_old = DateTime.Now.Ticks;
            }
        }
    }

    public void SyncToEveryone_Drop_Sub() {
        if (GetComponent<Object_Manager>().sync_objects.Length > obj_id && !Networking.LocalPlayer.IsOwner(this.gameObject)) {
            GetComponent<Object_Manager>().sync_objects[obj_id].transform.localPosition = pos;
            GetComponent<Object_Manager>().sync_objects[obj_id].transform.eulerAngles = rot;
        }

        tracking = false;
        drop = false;
        nano_sec_old = 0;
    }
}
