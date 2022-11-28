namespace Common.Interfaces.Inventory;

public interface IStorageInventory : IInventoryService
{
    void DepositMoney(uint amount);
    void WithdrawMoney(uint amount);
}