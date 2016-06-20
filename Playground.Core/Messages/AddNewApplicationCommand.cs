namespace Playground.Core.Messages
{
    public class AddNewApplicationCommand
    {
        public short ApplicationType { get; set; }
        public string Description { get; set; }
        public int AccountId { get; set; }
    }
}