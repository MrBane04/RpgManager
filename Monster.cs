class Monster : IAttacker
{
    public void Attack(Character target)
    {
        target.TakeDamage(40);
    }
}