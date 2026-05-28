using UnityEngine;

public interface IInteractable 
{ 
    void Interact(); 
}

public interface IPickupable 
{ 
    void OnPickedUp(Transform parent); 
    void DropBox();
}