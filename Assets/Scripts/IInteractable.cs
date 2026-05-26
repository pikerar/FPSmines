public interface IInteractable
{
    string GetInteractHint();
    void Interact();
    bool CanInteract();
}