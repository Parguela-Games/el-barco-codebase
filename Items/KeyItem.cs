using Manicomio.ActionableObjects;

public class KeyItem : ActionableObject
{
    protected override void InnerStart(){}

    protected override void InnerInteract() {
        ItemPickEvents.NotifyItemPick(this);
        Destroy(gameObject);
    }

    public override bool IsAnimationPlaying() => false;
}
