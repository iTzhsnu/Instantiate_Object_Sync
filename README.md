# Instantiate_Object_Sync
## 使い方
ObjectSyncManager.prefabをHierarchyに入れて  
Instantiateで生成するオブジェクトにInstantiated_Object_Syncを入れます  
※VRCPickupが必要です。RigidbodyにはIs Kinematicに✓を付けてください。  
そしてObjectSyncManagerの中にあるObject_Managerのsync_objectsに生成したオブジェクトを入れる事で  
ピックアップ時にオブジェクトの同期が取れるようになります。  

## 機能
ピックアップ時に、オブジェクトの位置が同期されます。  
両手対応

## 生成したオブジェクトの同期設定例 (仮)
※SendCustomNetworkEventでInstantiate_Objectを実行する前提でのコードです。  
※不明な理由で、遅延を付けないとObjectが反映されませんでした。
```diff_csharp
public Object_Sync_Manager manager;
public GameObject object;
public GameObject parent;

public GameObject o;
public void Instantiate_Object() {
  o = Instantiate(object, parent.transform);
  SendCustomEventDelayedFrames("AddObject", 10);
}

public void AddObject() {
  manager.AddObject(o);
}
```

## TODO
ピックアップ以外でのオブジェクトの移動時にも同期するようにする  
Objectを簡単に入れれるメソッドを追加する  
回転も綺麗に見えるようにする
