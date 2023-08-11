public class BossHitBody : LivingEntity
{
    private Boss _boss;
    private void Awake()
    {
        _boss = GetComponentInParent<Boss>();
    }

    public override void TakeDamage(DamageMessage damageMessage)
    {
        _boss.TakeDamage(damageMessage);
    }
}
