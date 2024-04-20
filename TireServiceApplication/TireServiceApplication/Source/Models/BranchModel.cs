using TireServiceApplication.Source.Data;
using TireServiceApplication.Source.Entities;

namespace TireServiceApplication.Source.Models;

public static class BranchModel
{
    private static List<Branch> _branches = new List<Branch>();
    
    // Метод для получения всех филиалов
    public static async Task<List<Branch>?> GetBranches()
    {
        var branches = await BranchData.GetBranches();
        if (branches != null)
        {
            _branches = new List<Branch>();
            TextForListView(branches);
            // Проверка - если филиал удален - не показывать
            foreach (var branch in branches.Where(branch => branch.Deleted == false))
                    _branches.Add(branch);
        }
        return _branches;
    }
    
    // Метод для добавления филиала
    public static async Task<bool> AddBranch(Branch branch)
    {
        var newBranch = await BranchData.AddBranch(branch);
        if (newBranch != null) _branches.Add(newBranch);
        return newBranch != null;
    }
    
    
    // Метод для изменения филиала
    public static async Task<bool> UpdateBranch(Branch branch)
    {
        var newBranch = await BranchData.UpdateBranch(branch);
        if (newBranch != null)
        {
            var index = _branches.FindIndex(x => x.Id == branch.Id);
            _branches[index] = newBranch;
        }
        return newBranch != null;
    }

    // Метод для удаления филиала
    public static async Task<bool> DeleteBranch(Branch branch)
    {
        if (branch.Id == null) return false;
        var result = await BranchData.DeleteBranch(branch.Id);
        if (result)
        {
            var index = _branches.FindIndex(x => x.Id == branch.Id);
            _branches.RemoveAt(index);
        }
        return result;
    }
    
    // Метод для составления текста для отображения в ListView
    public static void TextForListView(List<Branch> branches)
    {
        foreach (var branch in branches)
        {
            branch.AdressView = $"Адрес: {branch.Adress}";
        }
    }
}