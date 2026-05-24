using UnityEngine;
using Player;

public class PlayerWeapons : MonoBehaviour
{
    [SerializeField] private PlayerWeaponSO[] weaponSO;
    [SerializeField] private Transform weaponSocket;
    [SerializeField] private Transform muzzle;

    public PlayerWeaponSO CurrentWeapon => _instance?.WeaponsSO;
    public float MuzzleHeight => muzzle != null ? muzzle.position.y : 0f;
    public string CurrentAmmoText => CurrentWeapon != null ? CurrentWeapon.BaseGetAmmoText(_instance) : "No Weapon is here";

    private WeaponInstance[] _weaponInstances;
    private WeaponInstance _instance => (_weaponInstances != null && _weaponInstances.Length > 0) ? _weaponInstances[_currentWeaponIndex] : null;
    private int _currentWeaponIndex = 0;
    private PlayerEvents _events;
    private int _tempWeaponIndex;

    public void Initialize(PlayerEvents events)
    {
        _events = events;
        _weaponInstances = new WeaponInstance[weaponSO.Length];

        for (int i = 0; i < weaponSO.Length; i++)
        {
            if (weaponSO[i] != null)
            {
                WeaponInstance instance = weaponSO[i].BaseCreateInstance();

                weaponSO[i].BaseInitialize(instance, weaponSocket);

                _weaponInstances[i] = instance;
            }
        }

        var weapon = _weaponInstances[_currentWeaponIndex];
        if (weapon?.SpawnedPrefab != null)
        {
            weapon.SpawnedPrefab.SetActive(true);
        }
    }

    public bool CanSwap(int index)
    {
        // 현재 무기일 때
        if (index == _currentWeaponIndex)
        {
            return false;
        }

        // 1 ~ 3이 아닌 다른 값이 들어올 때
        if (index < 0 || index >= _weaponInstances.Length)
        {
            return false;
        }

        // 무기가 없을 때
        if (_weaponInstances[index] == null)
        {
            return false;
        }

        return true;
    }

    public void PendingSwap(int index)
    {
        _tempWeaponIndex = index;
    }

    public void ExecuteSwap()
    {
        var oldWeapon = _weaponInstances[_currentWeaponIndex];
        if (oldWeapon?.SpawnedPrefab != null) oldWeapon.SpawnedPrefab.SetActive(false);

        _currentWeaponIndex = _tempWeaponIndex;

        var newWeapon = _weaponInstances[_currentWeaponIndex];
        if (newWeapon != null)
        {
            if (newWeapon?.SpawnedPrefab != null)
            {
                newWeapon.SpawnedPrefab.SetActive(true);
            }
            _events?.CallbackOnAmmoChanged(CurrentWeapon.BaseGetAmmoText(newWeapon));
        }
    }

    public BaseState GetAttackState(PlayerMediator mediator)
    {
        if (CurrentWeapon == null)
        {
            return null;
        }

        return CurrentWeapon.BaseGetAttackState(mediator, _instance);
    }

    public bool CanReload()
    {
        if (CurrentWeapon == null)
        {
            return false;
        }

        return CurrentWeapon.BaseCanReload(_instance);
    }

    public void Reload()
    {
        if (CurrentWeapon != null)
        {
            CurrentWeapon.BaseReload(_instance);
            _events?.CallbackOnAmmoChanged(CurrentWeapon.BaseGetAmmoText(_instance));
        }
    }

    public void BeginAttack()
    {
        if (CurrentWeapon != null)
        {
            CurrentWeapon.BaseBeginAttack(_instance, transform, muzzle);
            _events?.CallbackOnAmmoChanged(CurrentWeapon.BaseGetAmmoText(_instance));
        }
    }

    public void TickAttack()
    {
        if (CurrentWeapon != null)
        {
            CurrentWeapon.BaseTickAttack(_instance, transform, muzzle);
        }
    }

    public void EndAttack()
    {
        if (CurrentWeapon != null)
        {
            CurrentWeapon.BaseEndAttack(_instance, transform, muzzle);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            CurrentWeapon?.OnDrawAttackGizmos(transform, muzzle);
        }
        else
        {
            if (weaponSO != null && weaponSO.Length > _currentWeaponIndex && weaponSO[_currentWeaponIndex] != null)
            {
                weaponSO[_currentWeaponIndex].OnDrawAttackGizmos(transform, muzzle);
            }
        }
    }
#endif
}