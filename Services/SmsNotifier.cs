using StudentInfoLoginRoles.Models;

namespace StudentInfoLoginRoles.Services
{
    public class SmsNotifier : IFeeNotifier
    {
        private readonly ApplicationContext _context;
        public SmsNotifier(ApplicationContext context)
        {
            _context = context;
        }
        public string Notify(int studentId)
        {
            var student = _context.Students.Find(studentId);
            if (student == null)
            {
                return $"Student with ID {studentId} not found.";
            }
            if (student.FeePending > 0)
            {
                return $"Sending SMS to {student.Phone}: Dear {student.FirstName} {student.LastName}, your pending fee is {student.FeePending:C}. Please pay at the earliest.";
            }
            return $"No pending fees for {student.FirstName} {student.LastName}.";
        }
    }
}
