namespace LibraryManagementMVC.Models
{
    public class DailyReportModel
    {
        public string BookTitle { get; set; }
        public string ISBN { get; set; }

        public int MemberId { get; set; }
        public string MemberName { get; set; }

        public int TransactionId { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsLateDueDate { get; set; }
        public double Penalty { get; set; }
    }
}
