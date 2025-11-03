using StudentInfoLoginRoles.Models;

namespace StudentInfoLoginRoles.Services
{
    public interface IFeeNotifier
    {
        string Notify(int studentId);
    }
}
