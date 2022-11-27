namespace Common.InventoryX.Interfaces;

public interface IStorageInventory : IOperatableInventory
{
    void DepositMoney(uint amount);
    void WithdrawMoney(uint amount);
}