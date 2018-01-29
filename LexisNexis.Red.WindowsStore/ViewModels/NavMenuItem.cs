namespace LexisNexis.Red.WindowsStore.ViewModels
{
    /// <summary>
    /// Data to represent an item in the nav menu.
    /// </summary>
    public class NavMenuItem
    {
        public string LabelId { get; set; }
        public string Symbol { get; set; }
        public string DestPage { get; set; }
        public object Arguments { get; set; }
    }
}
