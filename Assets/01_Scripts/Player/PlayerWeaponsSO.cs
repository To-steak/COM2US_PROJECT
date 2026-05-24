using UnityEngine;
using Player;

public abstract class PlayerWeaponSO : ScriptableObject
{
    public GameObject WeaponPrefab;
    public string WeaponName;
    public float Damage;

    public abstract WeaponInstance BaseCreateInstance();
    public abstract void BaseInitialize(WeaponInstance instance, Transform weaponSocket);
    public abstract BaseState BaseGetAttackState(PlayerMediator mediator, WeaponInstance instance);
    public abstract void BaseBeginAttack(WeaponInstance instance, Transform playerTransform, Transform muzzle);
    public abstract void BaseTickAttack(WeaponInstance instance, Transform playerTransform, Transform muzzle);
    public abstract void BaseEndAttack(WeaponInstance instance, Transform playerTransform, Transform muzzle);

    public abstract bool BaseCanReload(WeaponInstance instance);
    public abstract void BaseReload(WeaponInstance instance);
    public abstract string BaseGetAmmoText(WeaponInstance instance);

#if UNITY_EDITOR
    public virtual void OnDrawAttackGizmos(Transform transform, Transform muzzle) { }
#endif
}

public abstract class PlayerWeaponSO<T> : PlayerWeaponSO where T : WeaponInstance
{
    public override WeaponInstance BaseCreateInstance() => CreateInstance();
    public abstract T CreateInstance();

    public override void BaseInitialize(WeaponInstance instance, Transform weaponSocket) => Initialize((T)instance, weaponSocket);
    public abstract void Initialize(T instance, Transform weaponSocket);

    public override BaseState BaseGetAttackState(PlayerMediator mediator, WeaponInstance instance) => GetAttackState(mediator, (T)instance);
    public abstract BaseState GetAttackState(PlayerMediator mediator, T instance);

    public override void BaseBeginAttack(WeaponInstance instance, Transform playerTransform, Transform muzzle) => BeginAttack((T)instance, playerTransform, muzzle);
    public abstract void BeginAttack(T instance, Transform playerTransform, Transform muzzle);

    public override void BaseTickAttack(WeaponInstance instance, Transform playerTransform, Transform muzzle) => TickAttack((T)instance, playerTransform, muzzle);
    public abstract void TickAttack(T instance, Transform playerTransform, Transform muzzle);

    public override void BaseEndAttack(WeaponInstance instance, Transform playerTransform, Transform muzzle) => EndAttack((T)instance, playerTransform, muzzle);
    public abstract void EndAttack(T instance, Transform playerTransform, Transform muzzle);

    public override bool BaseCanReload(WeaponInstance instance) => CanReload((T)instance);
    public virtual bool CanReload(T instance) => false;

    public override void BaseReload(WeaponInstance instance) => Reload((T)instance);
    public virtual void Reload(T instance) { }

    public override string BaseGetAmmoText(WeaponInstance instance) => GetAmmoText((T)instance);
    public abstract string GetAmmoText(T instance);
}