using UnityEngine;

public interface IItem
{
    void Pickup(Transform hand);
    void Drop();
    void PrimaryAction();
    void SecondaryAction();
}