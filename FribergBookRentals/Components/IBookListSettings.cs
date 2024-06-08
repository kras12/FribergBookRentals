namespace FribergBookRentals.Components
{
    public interface IBookListSettings
    {
        public bool EnableBookBorrowing { get; set; }

        public bool EnableCloseLoan { get; set; }

        public bool EnableProlongLoan { get; set; }
    }
}
