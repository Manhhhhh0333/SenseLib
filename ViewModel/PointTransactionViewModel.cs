namespace DOAN.ViewModel
{
    public class PointTransactionViewModel
    {
        public DateTime Date { get; set; }
        public int Points { get; set; }
        public string Description { get; set; }
        public string ActionType { get; set; } // Recharge, Download, Earn, Approve, ...
    }
}
